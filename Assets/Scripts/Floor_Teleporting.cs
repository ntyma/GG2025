using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor_Teleporting : MonoBehaviour
{
    [SerializeField] private BoxCollider2D Floor_Teleporting_Collider;
    [SerializeField] private GameObject Teleport_Points_GameObject;

    public int Teleport_Points_CurrIndex = 0;
    [SerializeField] private Vector3[] Teleport_Points;
    [SerializeField] private int Teleport_Points_Count;

    // Start is called before the first frame update
    void Start()
    {
        // Collect all Teleport Points from Teleport_Points_GameObject
        Teleport_Points_Count = Teleport_Points_GameObject.transform.childCount;
        Teleport_Points = new Vector3[Teleport_Points_Count];

        // Index all points (Order seems to be preserved based on the order in the Hierarchy)
        int i = 0;
        foreach (Transform childTransform in Teleport_Points_GameObject.transform)
        {
            Teleport_Points[i++] = childTransform.position;
        }
        // Set position of object based on starting Index
        this.transform.position = Teleport_Points[Teleport_Points_CurrIndex];
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Light")
        {
            Teleport();
        }
    }
    // Teleport Object to the position in the next Index/ next Teleport_Point in Hierarchy
    private void Teleport()
    {
        Teleport_Points_CurrIndex = (Teleport_Points_CurrIndex + 1) % Teleport_Points_Count;
        this.transform.position = Teleport_Points[Teleport_Points_CurrIndex];
    }    
}
