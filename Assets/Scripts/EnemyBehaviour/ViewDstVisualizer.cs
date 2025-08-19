using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewDstVisualizer : MonoBehaviour
{
    public GameObject target;
    EnemyController targetController;
    Transform targetTransform;
    
    // Start is called before the first frame update
    void Start()
    {
        targetTransform = target.GetComponent<Transform>();
        targetController = target.GetComponent<EnemyController>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector2.one * targetController.viewDst * 2;
        transform.position = targetTransform.position;
    }
}
