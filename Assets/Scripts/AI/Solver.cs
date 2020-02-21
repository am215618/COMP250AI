using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Solver : MonoBehaviour
{
    int[] fx; //sum of gx and hx
    int gx = 0; //distance from root node
    int[] hx; //number of misplaced tiles

    [SerializeField]
    int initialHx = 0; //initial number of misplaced tiles.

    public Vector2Int[] initialPos;
    public Vector2Int[] currentPos;
    public Puzzle puzzle;

    public void Shuffled()
    {
        for (int i = 1; i < initialPos.Length; i++)
        {
            if(initialPos[i] != currentPos[i])
            {
                initialHx++;
            }
        }
    }

    public void AStar()
    {
        /*calculate the different outcomes' hx
        add that to gx
        get the lowest one
        move that particular piece (the one with lowest fx)
        repeat
        */
    }
}
