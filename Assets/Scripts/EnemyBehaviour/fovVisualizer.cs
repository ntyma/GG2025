using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fovVisualizer : MonoBehaviour
{
    public GameObject target;
    EnemyController targetController;
    Transform targetTransform;

    public bool leftSide;

    // Start is called before the first frame update
    void Start()
    {
        targetTransform = target.GetComponent<Transform>();
        targetController = target.GetComponent<EnemyController>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log((leftSide ? -1 : 1) * targetController.fov);
        transform.localScale = new Vector2(1f * targetController.viewDst, 0.05f);
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, targetTransform.rotation.eulerAngles.z + (leftSide ? -1 : 1) * targetController.fov));
        transform.position = targetTransform.position + new Vector3(Mathf.Cos(Mathf.Deg2Rad * targetTransform.rotation.eulerAngles.z + targetController.fov), Mathf.Sin(Mathf.Deg2Rad * targetTransform.rotation.eulerAngles.z + targetController.fov), 0f) * transform.localScale.x / 2;
    }
}
