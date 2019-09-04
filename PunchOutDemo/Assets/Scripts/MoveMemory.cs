using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMemory
{
    private Queue<float[]> memory;

    /// <summary>
    /// Creates a MoveMemory instance
    /// </summary>
    /// <param name="size">The number of moves to remember</param>
    /// <param name="fill">The moves to fill the empty memory with</param>
    public MoveMemory(int size, float[] fill)
    {
        memory = new Queue<float[]>(size);
        for(var i = 0; i < size; i++)
        {
            memory.Enqueue(fill);
        }
    }

    /// <summary>
    /// Add a move to memory
    /// </summary>
    /// <param name="moves">The move</param>
    public void Add(float[] moves)
    {
        memory.Dequeue();
        memory.Enqueue(moves);
    }

    /// <summary>
    /// Get the moves
    /// </summary>
    /// <returns>The moves in memory</returns>
    public float[][] Get()
    {
        return memory.ToArray();
    }

    /// <summary>
    /// Get the last move in memory
    /// </summary>
    /// <returns>The last move</returns>
    public float[] GetLastMove()
    {
        return memory.Peek();
    }
}
