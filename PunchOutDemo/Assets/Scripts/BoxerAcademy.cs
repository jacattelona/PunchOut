using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxerAcademy : Academy
{


    private BoxerArea[] areas;

    public override void AcademyReset()
    {
        if (areas == null)
        {
            areas = GameObject.FindObjectsOfType<BoxerArea>();
        }

        foreach (BoxerArea area in areas)
        {
            area.ResetArea();
        }
    }

}