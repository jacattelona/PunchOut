using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveHistory : MonoBehaviour
{

    public struct Move
    {
        public float[] Actions;
        public float Time;
    }


    public List<Move> moves = new List<Move>();
}
