using System;
using System.Collections.Generic;
using UnityEngine;
using ClipperLib;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;


public class RealtimeLightShadowColliders2D : MonoBehaviour
{
    [Tooltip("The generated lights will be put as children of this object. Make sure it is at (0,0,0) position")]
    public GameObject container;

    [Header("Position Controls")]
    public Camera mainCamera;
    [Tooltip("How fast the light is moving, unit: angles per second.")]
    public float turningSpeed = 50f; // IF YOU WANT TO MAKE IT SLOWER WHEN IT IS NOT LIGHTING, USE THIS 
    public bool clockWise = false;
    [Tooltip("Say the light turns 180 degrees, set this to turn it off between 0->cutoffAngle and (180-cutoffAngle)->180")]
    public float cutoffAngle = 30f;
    [Tooltip("How fast the light will fade off, choose 0->(90-cutoffangle), unit: angles")]
    public float fadeOffAngleDelta = 20;
    private float currentAngle = 0f;
    private Vector2 lightDirection;

    [Header("Layers")]
    public LayerMask windowLayer;
    public LayerMask shadowCastLayer;

    [Header("Generated collider settings")]
    [Tooltip("The layer number the generated lights will be set to.")]
    public int generatedLayer = -1;
    public string generatedTag = "Light";

    [Header("Light settings")]
    [Tooltip("The maximum intensity the light will have.")]
    public float maxIntensityLight = 1f;
    public float falloffIntensityLight = 0.2f;
    private float currentLightIntensity = 0.5f;

    [Header("Geometry (only used when mainCamera is null)")]
    public float extrusionLength = 20f;
    [Tooltip("Radius for how far the light and shadow searches for objects. Only used when camera is not defined, else it'll just search whatever is on screen.")]
    public float searchRadius = 20f;

    [Header("Approximations")]
    [Range(6, 64)] public int circleSegments = 20;

    [Header("Clipper")]
    [Tooltip("Scale factor when converting floats -> Clipper Int points. Increase for more precision.")]
    public int clipperScale = 1000;
    public bool cleanGeometry = true;
    [Tooltip("Tolerance (world units) used by Clipper.CleanPolygons")]
    public float cleanTolerance = 0.005f;

    [Header("Debug")]
    [Tooltip("Draw the area that the script is searching for window colliders.")]
    public bool drawGizmos = false;

    // Hash the generated lights
    struct SourcePathKey : IEquatable<SourcePathKey>
    {
        public Collider2D source;
        public bool isWindow;

        public bool Equals(SourcePathKey other)
        {
            return source == other.source && isWindow == other.isWindow;
        }
        public override bool Equals(object obj) => obj is SourcePathKey o && Equals(o);
        public override int GetHashCode()
        {
            unchecked
            {
                int h = 17;
                h = h * 31 + (source ? source.GetInstanceID() : 0);
                h = h * 31 + (isWindow ? 1 : 0);
                return h;
            }
        }
    }

    class Gen
    {
        public GameObject go;
        public PolygonCollider2D poly;
        public Light2D light;
        public List<List<Vector2>> lastWorldPolys = new List<List<Vector2>>();
    }

    readonly Dictionary<SourcePathKey, Gen> _pool = new Dictionary<SourcePathKey, Gen>();
    // Used to destroy any ligths that did not interact in the frame. (i.e. they were out of range)
    readonly HashSet<SourcePathKey> _touchedThisFrame = new HashSet<SourcePathKey>();

    // ------------------------------------------------------------------------

    public void RebuildNow() => UpdateAll();

    private void Awake()
    {
        if (mainCamera == null) return;

        // Assume camera is not going to change
        float camHeight = 2f * mainCamera.orthographicSize;
        float camWidth = camHeight * mainCamera.aspect;

        // Make the extrusion length the maximum possible in a scene; diagonal length of camera
        extrusionLength = Mathf.Sqrt(camHeight * camHeight + camWidth * camWidth);
    }

    // Move the light in circles
    private void Update()
    {
        // Only make the lighthouse movement when there is a camera attached to object.
        if (mainCamera == null) return;

        if (fadeOffAngleDelta <= 0 || fadeOffAngleDelta > 90) Debug.LogError("fadeOffAngleDelta is not between 0-90");

        // Update angle
        currentAngle += turningSpeed * Time.deltaTime * (clockWise ? -1 : 1);
        currentAngle = (currentAngle + 360f) % 360f;

        // Light direction is determined by a turning angle
        float rad = currentAngle * Mathf.Deg2Rad;
        lightDirection = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;
    }

    void LateUpdate()
    {
        // Generate light for the scene
        UpdateAll();
    }

    void OnDisable()
    {
        ClearAllGenerated();
    }

    void ClearAllGenerated()
    {
        foreach (var kv in _pool) if (kv.Value.go) DestroyImmediate(kv.Value.go);
        _pool.Clear();
    }

    void UpdateAll()
    {
        _touchedThisFrame.Clear();


        // Only display light is pointing downwards
        if (currentAngle > (180 + cutoffAngle) && currentAngle < (360 - cutoffAngle))
        {
            // Edges of the downward cone
            float lowerEdge = 180f + cutoffAngle;
            float upperEdge = 360f - cutoffAngle;

            // Baseline intensity meaning "no light"
            const float baseline = 0.5f;


                // Inside the cone [lowerEdge .. upperEdge]
                // Compute how far from each cutoff (0 at the cutoff, fadeOffAngleDelta when fully "safe")
                float tLower = Mathf.Clamp01((currentAngle - lowerEdge) / fadeOffAngleDelta); // 0..1
                float tUpper = Mathf.Clamp01((upperEdge - currentAngle) / fadeOffAngleDelta); // 0..1

                // The nearest edge limits the intensity (we want fade in/out anchored at each edge)
                float t = Mathf.Min(tLower, tUpper); // 0 = at nearest edge, 1 = >= fadeOffAngleDelta away from both edges

                // Interpolate between baseline (no light) and max intensity
                currentLightIntensity = Mathf.Lerp(baseline, maxIntensityLight, t);


        }
        else
        {
            // Turn off light and delete the existing ones due to the light being invisible. 
            currentLightIntensity = 0f;
            CullUntouched();
            return;
        }

        Vector2 lightPos = transform.position;

        Collider2D[] shadowCols;
        if (mainCamera != null) // Use the camera as search anchor if defined
        {
            var cameraPosition = new Vector2(mainCamera.transform.position.x, mainCamera.transform.position.y);
            shadowCols = Physics2D.OverlapCircleAll(cameraPosition, searchRadius, shadowCastLayer);
        }
        else
        {
            shadowCols = Physics2D.OverlapCircleAll(lightPos, searchRadius, shadowCastLayer);
        }

        var shadowPaths = new List<List<Vector2>>();
        foreach (var col in shadowCols)
        {
            if (!IsSupported(col)) continue;
            var paths = GetWorldPaths(col);
            for (int p = 0; p < paths.Count; p++)
            {
                EnsureCCW(paths[p]);
                var runs = BuildExtrudedRuns(paths[p], isWindow: false);
                shadowPaths.AddRange(runs);
            }
        }

        // Get window colliders that are within the search range
        Collider2D[] windows;
        if (mainCamera != null) // Use the camera as search anchor if defined
        {
            var cameraPosition = new Vector2(mainCamera.transform.position.x, mainCamera.transform.position.y);
            windows = Physics2D.OverlapCircleAll(cameraPosition, searchRadius, windowLayer);
        }
        else
        {
            windows = Physics2D.OverlapCircleAll(lightPos, searchRadius, windowLayer);
        }

        foreach (var col in windows)
        {
            if (!IsSupported(col)) continue;
            var paths = GetWorldPaths(col); // Get vertex coords of windows
            for (int p = 0; p < paths.Count; p++)
            {
                EnsureCCW(paths[p]);
                var runs = BuildExtrudedRuns(paths[p], isWindow: true);

                // Get the minimum distance a window vertex is from the light
                //this is hacky and will not handle cases where objects overlap with the window
                //while having part of it closer to the light well. But it's more
                //efficient and should do the job.
                float dWindow = MinProjectionAlongLight(lightDirection, paths[p]);

                foreach (var subjRun in runs)
                {
                    var finalWorldPolys = new List<List<Vector2>>();
                    if (subjRun.Count >= 3)
                    {
                        var eligibleClips = new List<List<IntPoint>>();
                        foreach (var shadow in shadowPaths)
                        {
                            float dShadow = MinProjectionAlongLight(lightDirection, shadow);

                            // Shadow is behind the window if it's further along lightDir
                            if (dShadow > dWindow) eligibleClips.Add(WorldToIntPath(shadow));
                        }

                        if (eligibleClips.Count > 0)
                        {
                            // Subtract the extruded shadows from the extruded light polygon
                            var subjInt = WorldToIntPath(subjRun);
                            var c = new Clipper();
                            var sol = new List<List<IntPoint>>();

                            c.AddPath(subjInt, ClipperLib.PolyType.ptSubject, true);
                            c.AddPaths(eligibleClips, ClipperLib.PolyType.ptClip, true);

                            c.Execute(ClipperLib.ClipType.ctDifference, sol,
                                      ClipperLib.PolyFillType.pftNonZero, ClipperLib.PolyFillType.pftNonZero);

                            foreach (var s in sol)
                            {
                                var poly = IntPathToWorld(s);
                                if (poly.Count >= 3) finalWorldPolys.Add(poly);
                            }
                        }
                        else
                        {
                            finalWorldPolys.Add(new List<Vector2>(subjRun));
                        }
                    }

                    // Hashkey for the newly generated light
                    var key = new SourcePathKey { source = col, isWindow = true };

                    // If nothing remains, remove/ensure no collider
                    if (finalWorldPolys.Count == 0)
                    {
                        if (_pool.TryGetValue(key, out var g))
                        {
                            if (g.go) DestroyImmediate(g.go);
                            _pool.Remove(key);
                        }
                    }
                    else
                    {
                        // Generate the object if not already generated
                        //else update it and tell that the collider
                        //is still relevant, so don't delete it yet
                        UpsertGenerated(key, finalWorldPolys);
                        _touchedThisFrame.Add(key);
                    }
                }
            }
        }

        // Cull stale generated colliders
        CullUntouched();
    }

    void CullUntouched()
    {
        var toRemove = new List<SourcePathKey>();
        foreach (var kv in _pool)
        {
            if (!_touchedThisFrame.Contains(kv.Key))
            {
                if (kv.Value.go) DestroyImmediate(kv.Value.go);
                toRemove.Add(kv.Key);
            }
        }
        foreach (var k in toRemove) _pool.Remove(k);
    }

    // Updates light shape and generates lights if it doesn't exist yet
    void UpsertGenerated(SourcePathKey key, List<List<Vector2>> worldPolys)
    {
        if (worldPolys == null) worldPolys = new List<List<Vector2>>();

        if (worldPolys.Count == 0)
        {
            if (_pool.TryGetValue(key, out var ex))
            {
                if (ex.go) DestroyImmediate(ex.go);
                _pool.Remove(key);
            }
            return;
        }

        // Generate a game object with light collider and a 2D freeform light
        if (!_pool.TryGetValue(key, out var gen) || gen == null || gen.go == null)
        {
            gen = new Gen();
            string baseName = "LightCollider";
            gen.go = new GameObject($"{baseName}_{key.source.GetInstanceID()}");

            // Collider
            gen.poly = gen.go.AddComponent<PolygonCollider2D>();
            gen.poly.isTrigger = true;

            // Light2D
            gen.light = gen.go.AddComponent<Light2D>();
            gen.light.lightType = Light2D.LightType.Freeform;
            gen.light.color = Color.white;
            gen.light.overlapOperation = Light2D.OverlapOperation.AlphaBlend; // Make sure light can overlap 

            // Apply tagging/layer
            if (generatedLayer >= 0 && generatedLayer <= 31) gen.go.layer = generatedLayer;
            gen.go.tag = generatedTag;

            // Place at origin so paths can use world coords directly
            gen.go.transform.position = Vector3.zero;
            gen.go.transform.rotation = Quaternion.identity;
            gen.go.transform.localScale = Vector3.one;
            if (container != null) // Create light as children for the container
                gen.go.transform.SetParent(container.transform, false);

            _pool[key] = gen;
        }

        // Set collider paths (vertices)
        gen.poly.pathCount = worldPolys.Count;
        for (int i = 0; i < worldPolys.Count; i++)
        {
            var p = worldPolys[i];
            if (p == null || p.Count < 3)
            {
                gen.poly.SetPath(i, new Vector2[0]);
                continue;
            }
            EnsureCCW(p);
            gen.poly.SetPath(i, p.ToArray());
        }

        // Update Light2D freeform shape and intensity
        // Merge all polygons into one continuous shape (outer polygon)
        // or assign only the first (Freeform Light2D supports 1 path at a time).
        var firstPoly = worldPolys[0];
        var shape = new Vector3[firstPoly.Count];
        for (int i = 0; i < firstPoly.Count; i++)
        {
            shape[i] = firstPoly[i]; // Vector2 -> Vector3 implicit
        }
        gen.light.SetShapePath(shape);
        gen.light.intensity = (mainCamera == null) ? maxIntensityLight : currentLightIntensity;
        gen.light.falloffIntensity = falloffIntensityLight;


        // Save polys
        gen.lastWorldPolys.Clear();
        foreach (var w in worldPolys) gen.lastWorldPolys.Add(new List<Vector2>(w));
    }

    // -------------------- Geometry helpers ----------------------------------

    bool IsSupported(Collider2D c)
    {
        return c is PolygonCollider2D
            || c is BoxCollider2D
            || c is CircleCollider2D
            || c is CompositeCollider2D
            || c is TilemapCollider2D;
    }

    List<List<Vector2>> GetWorldPaths(Collider2D col)
    {
        var paths = new List<List<Vector2>>();

        if (col is PolygonCollider2D p)
        {
            for (int i = 0; i < p.pathCount; i++)
            {
                var local = p.GetPath(i);
                var world = new List<Vector2>(local.Length);
                for (int k = 0; k < local.Length; k++)
                    world.Add(p.transform.TransformPoint(local[k]));
                paths.Add(world);
            }
        }
        else if (col is BoxCollider2D box)
        {
            paths.Add(GetBoxWorldPath(box));
        }
        else if (col is CircleCollider2D circle)
        {
            paths.Add(GetCircleApproxWorldPath(circle, circleSegments));
        }
        else if (col is CompositeCollider2D comp)
        {
            var t = comp.transform;
            var pts = new Vector2[comp.pointCount];
            for (int i = 0; i < comp.pathCount; i++)
            {
                int count = comp.GetPath(i, pts);
                var world = new List<Vector2>(count);
                for (int k = 0; k < count; k++)
                    world.Add(t.TransformPoint(pts[k]));
                paths.Add(world);
            }
        }
        else if (col is TilemapCollider2D tilemap)
        {
            // If tilemap is linked to composite, prefer that
            var comp_col = tilemap.GetComponent<CompositeCollider2D>();
            if (comp_col != null)
            {
                paths.AddRange(GetWorldPaths(comp_col));
            }
            else
            {
                Debug.LogError("Please enable composite colliders for your tilemap colliders.");
            }

        }

        return paths;
    }


    List<Vector2> GetBoxWorldPath(BoxCollider2D box)
    {
        var t = box.transform;
        Vector2 off = box.offset;
        Vector2 h = box.size * 0.5f;
        Vector2[] pts =
        {
            off + new Vector2(-h.x, -h.y),
            off + new Vector2(-h.x,  h.y),
            off + new Vector2( h.x,  h.y),
            off + new Vector2( h.x, -h.y)
        };
        var list = new List<Vector2>(4);
        for (int i = 0; i < 4; i++) list.Add(t.TransformPoint(pts[i]));
        return list;
    }

    List<Vector2> GetCircleApproxWorldPath(CircleCollider2D circle, int segments)
    {
        var t = circle.transform;
        Vector2 center = circle.offset;
        float r = circle.radius;
        var list = new List<Vector2>(segments);
        for (int i = 0; i < segments; i++)
        {
            float ang = (Mathf.PI * 2f) * (i / (float)segments);
            Vector2 local = center + new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * r;
            list.Add(t.TransformPoint(local));
        }
        return list;
    }

    void EnsureCCW(List<Vector2> pts)
    {
        if (pts == null || pts.Count < 3) return;
        if (SignedArea(pts) < 0f) pts.Reverse();
    }

    float SignedArea(List<Vector2> pts)
    {
        float a = 0f;
        for (int i = 0, j = pts.Count - 1; i < pts.Count; j = i, i++)
            a += (pts[j].x * pts[i].y - pts[i].x * pts[j].y);
        return a * 0.5f;
    }

    // Build extruded polygon runs
    List<List<Vector2>> BuildExtrudedRuns(List<Vector2> basePath, bool isWindow)
    {
        var result = new List<List<Vector2>>();
        int n = basePath.Count;
        if (n < 2) return result;

        var ext = new Vector2[n];

        for (int i = 0; i < n; i++)
        {
            Vector2 p = basePath[i];
            Vector2 d = lightDirection.normalized;

            // Dummy direction if light is too close to the point to determine direction
            if (d.sqrMagnitude < 1e-8f) d = Vector2.down;
            d.Normalize();

            // Extrude
            ext[i] = p + d * extrusionLength;
        }

        bool[] silhouette = new bool[n];
        for (int i = 0; i < n; i++)
        {
            Vector2 A = basePath[i];
            Vector2 B = basePath[(i + 1) % n];
            Vector2 e = B - A;

            float cross;
            // Figure out if the vector between two points is pointing towards
            //the light or away
            cross = Cross(e, lightDirection); // use light direction instead of lightPos

            // Invert these to make the polygon include what it hits instead of going around it
            silhouette[i] = isWindow ? (cross > 1e-6f) : (cross < -1e-6f);
        }

        // Get continuous edge indicies that are pointing the correct direction towards the light
        var runs = new List<(int start, int end)>();
        bool inRun = false;
        int runStart = 0;
        for (int i = 0; i < n; i++)
        {
            if (silhouette[i])
            {
                if (!inRun) { inRun = true; runStart = i; }
            }
            else
            {
                if (inRun) { runs.Add((runStart, i - 1)); inRun = false; }
            }
        }
        if (inRun) runs.Add((runStart, n - 1));

        if (runs.Count >= 2 && runs[0].start == 0 && runs[runs.Count - 1].end == n - 1)
        {
            var first = runs[0];
            var last = runs[runs.Count - 1];
            runs[0] = (last.start, first.end);
            runs.RemoveAt(runs.Count - 1);
        }

        foreach (var r in runs)
        {
            int s = r.start;
            int e = r.end;
            int runEdgeCount = (e >= s) ? (e - s + 1) : (n - s + e + 1); // nr edges
            if (runEdgeCount <= 0 || runEdgeCount > n) continue; // Sanity check, should not happen
            int vertexCount = (runEdgeCount == n) ? n : (runEdgeCount + 1); // nr vertices

            var path = new List<Vector2>(vertexCount * 2); // List to store vertices of original polygon and extruded points

            // Add the relevant source polygon vertices
            for (int k = 0; k < vertexCount; k++)
            {
                int idx = (s + k) % n;
                path.Add(basePath[idx]);
            }

            // Add the extruded vertices
            for (int k = vertexCount - 1; k >= 0; k--)
            {
                int idx = (s + k) % n;
                path.Add(ext[idx]);
            }

            // Make sure points are in CCW order
            if (path.Count >= 3)
            {
                EnsureCCW(path);
                result.Add(path);
            }
        }

        return result;
    }

    float Cross(Vector2 a, Vector2 b) => a.x * b.y - a.y * b.x;

    // Imagine the opposite direction of the light vector is up
    //then this computes what is heighest when up is defined like that
    float MinProjectionAlongLight(Vector2 lightDir, List<Vector2> poly)
    {
        float minProj = float.PositiveInfinity;
        foreach (var v in poly)
        {
            // Projection of point onto light axis
            float proj = Vector2.Dot(v, lightDir.normalized);
            if (proj < minProj) minProj = proj;
        }
        return minProj;
    }
    // -------------------- Clipper conversion helpers -------------------------
    List<IntPoint> WorldToIntPath(List<Vector2> world)
    {
        var path = new List<IntPoint>(world.Count);
        for (int i = 0; i < world.Count; i++)
        {
            long x = (long)Math.Round(world[i].x * clipperScale);
            long y = (long)Math.Round(world[i].y * clipperScale);
            path.Add(new IntPoint(x, y));
        }
        return path;
    }

    List<Vector2> IntPathToWorld(List<IntPoint> path)
    {
        var w = new List<Vector2>(path.Count);
        double inv = 1.0 / clipperScale;
        for (int i = 0; i < path.Count; i++)
            w.Add(new Vector2((float)(path[i].X * inv), (float)(path[i].Y * inv)));
        return w;
    }

    // -------------------- Gizmos --------------------------------------------
    // Draw search area
    void OnDrawGizmos()
    {
        if (!drawGizmos) return;
        Gizmos.color = Color.cyan;
        if (mainCamera != null)
        {
            Vector2 camPos = new Vector2(mainCamera.transform.position.x, mainCamera.transform.position.y);
            Gizmos.DrawWireSphere(camPos, searchRadius);
        }
        else
        {
            Gizmos.DrawWireSphere(transform.position, searchRadius);
        }

    }
    // GL with making the level design with this o7
}
