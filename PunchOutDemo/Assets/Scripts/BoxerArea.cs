using MLAgents;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoxerArea : Area
{
    public GameObject boxer;
    public GameObject enemy;

    public TextMeshPro scoreText;

    private Vector3 targetStartingPos;

    public override void ResetArea()
    {
        BoxerAgent boxerScript = boxer.GetComponent<BoxerAgent>();
        boxerScript.ResetDodges();

        BoxerAgent enemyScript = enemy.GetComponent<BoxerAgent>();
        enemyScript.ResetDodges();
    }

   
}