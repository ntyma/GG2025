using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyController))]
public class FovEditor : Editor
{
    // Start is called before the first frame update
    private void OnSceneGUI()
    {
        EnemyController enemyController = (EnemyController)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(enemyController.transform.position, Vector3.forward, Vector3.left, 360, enemyController.viewDst);
        Vector3 viewAngleA = enemyController.DirFromAngle(-enemyController.fov / 2);
        Vector3 viewAngleB = enemyController.DirFromAngle(enemyController.fov / 2);

        Handles.DrawLine(enemyController.transform.position, enemyController.transform.position + viewAngleA * enemyController.viewDst);
        Handles.DrawLine(enemyController.transform.position, enemyController.transform.position + viewAngleB * enemyController.viewDst);
    }
}
