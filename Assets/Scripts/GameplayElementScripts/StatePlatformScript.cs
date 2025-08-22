using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePlatformScript : MonoBehaviourWithReset
{
    public override void ResetToInstantiation()
    {
        foreach (Transform childTransform in this.transform)
        {
            StateFloorScript stateFloorScript;
            if (!(childTransform.gameObject.TryGetComponent<StateFloorScript>(out stateFloorScript)))
                continue;

            stateFloorScript.ResetToInstantiation();
        }
    }
}
