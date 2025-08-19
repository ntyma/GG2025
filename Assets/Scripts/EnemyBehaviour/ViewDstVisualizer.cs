using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewDstVisualizer : MonoBehaviour
{
    EnemyOverhead targetController;
    public Transform targetTransform;
    
    // Start is called before the first frame update
    void Start()
    {
        targetController = GetComponentInParent<EnemyOverhead>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector2.one * targetController.viewDst * 4;
        transform.position = targetTransform.position;
    }
}
