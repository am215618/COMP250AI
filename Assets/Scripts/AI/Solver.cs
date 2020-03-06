using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Solver : MonoBehaviour
{
    int[] fx; //sum of gx and hx
    [HideInInspector]
    public int gx = 0; //distance from root node
    public int[] hx; //number of misplaced tiles
    //fx = gx + hx

    bool isSolving = false;

    [SerializeField]
    int initialHx = 0; //initial number of misplaced tiles.
    public int currentHx = 0;

    public Vector2Int[] initialPos;
    public Vector2Int[] currentPos;

    public int[] surroundingIDs;
    //history of moves, position class.
    public List<int> piecesMoved;
    public Puzzle puzzle;

    void Start()
    {
        List<int> piecesMoved = new List<int>();
    }

    /*void Update()
    {
        if(puzzle.state == Puzzle.PuzzleState.InPlay)
        {
            for (int i = 0; i < fx.Length; i++)
            {
                fx[i] = gx + hx[i];
            }
        }
    }*/

    public void Shuffled()
    {
        for (int i = 1; i < initialPos.Length; i++)
        {
            if(initialPos[i] != currentPos[i])
            {
                initialHx++;
                currentHx++;
            }
        }
    }

    public void CreateArrays(int nextToEmptyBlock)
    {
        fx = new int[nextToEmptyBlock];
        hx = new int[nextToEmptyBlock];
        for (int y = 0; y < hx.Length; y++)
        {
            hx[y] = initialHx;
        }
        surroundingIDs = new int[fx.Length];
        /*for (int y = 0; y < surroundingIDs.Length; y++)
        {
            Debug.Log(surroundingIDs[y]);
        }*/
    }

    public void AStar()
    {
        CalculateHx();

        /*
        calculate the different outcomes' hx
        - get the surrounding blocks' id and positions
        - simulate them going in that partciular direction
        - get the initialHx
        - if it was in correct position, then +1 to that hx
        - if it goes into a correct position, then -1 to that hx

        add those to gx
        get the lowest one
        move that particular piece (the one with lowest fx)
        repeat
        if hx = 0, then solved.
        */
    }

    public void CalculateHx()
    {

        Vector2Int[] offsets = { new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(0, -1) };


        for (int x = 0; x < offsets.Length; x++)
        {
            //Vector2 offset = offsets[(blockindex + x) % offsets.Length];
            //if ()
        }
    }
}
