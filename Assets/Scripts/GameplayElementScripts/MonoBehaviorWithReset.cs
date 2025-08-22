using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoBehaviourWithReset : MonoBehaviour
{
    public virtual void ResetToInstantiation()
    {
        Debug.Log("ResetToInstantiation() of Base class used! - from ResetToInstantiation() in MonoBehaviourWithResetScript");
    }
}
