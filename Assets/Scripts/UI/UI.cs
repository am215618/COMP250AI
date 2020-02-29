using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    public Puzzle puzzleScript;

    public void Shuffle()
    {
        if(puzzleScript.state == Puzzle.PuzzleState.Solved)
        {
            puzzleScript.StartShuffle();
        }
    }
}
