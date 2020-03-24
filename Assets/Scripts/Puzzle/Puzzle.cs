using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Puzzle : MonoBehaviour
{
    public Texture2D image;
    public int blocksPerLine = 4;
    public int shuffleLength = 20;
    public float defaultMoveDuration = .2f;
    public float shuffleMoveDuration = .1f;

    public TimerScript timer;

    [SerializeField]
    int PossibleMoves;
    public enum PuzzleState { Solved, Shuffling, InPlay, Solving };
    public PuzzleState state;

    Block emptyBlock;
    Block[,] blocks;
    Queue<Block> inputs;
    bool blockIsMoving;
    int shuffleMovesRemaining;
    Vector2Int previousShuffleOffset;
    Vector2Int previousSolveOffset;
    int id;
    [HideInInspector]
    public int nrEmptyBlocks = 0;

    public Solver solver;

    GameObject blockObject;
    Block block;

    [HideInInspector]
    public bool isSolving = false;

    void Awake()
    {
        CreatePuzzle();
    }

    void Update()
    {
        if (state == PuzzleState.Solved && Input.GetKeyDown(KeyCode.Space))
        {
            StartShuffle();
        }
    }

    void CreatePuzzle()
    {
        id = (blocksPerLine * blocksPerLine) - (blocksPerLine - 1);
        blocks = new Block[blocksPerLine, blocksPerLine];
        Texture2D[,] imageSlices = ImageSlicer.GetSlices(image, blocksPerLine);
        solver.initialPos = new Vector2Int[(blocksPerLine * blocksPerLine)];
        solver.currentPos = new Vector2Int[(blocksPerLine * blocksPerLine)];
        solver.idOrder = new int[(blocksPerLine * blocksPerLine)];
        solver.correctidOrder = new int[(blocksPerLine * blocksPerLine)];

        for (int y = 0; y < blocksPerLine; y++)
        {
            for (int x = 0; x < blocksPerLine; x++)
            {
                blockObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
                blockObject.transform.position = -Vector2.one * (blocksPerLine - 1) * .5f + new Vector2(x, y);
                blockObject.transform.parent = transform;

                block = blockObject.AddComponent<Block>();
                block.OnBlockPressed += PlayerMoveBlockInput;
                block.OnFinishedMoving += OnBlockFinishedMoving;
                block.Init(new Vector2Int(x, y), imageSlices[x, y]);
                block.id = id;
                blocks[x, y] = block;
                if (isDivisible(id, blocksPerLine))
                {
                    id = id - (blocksPerLine * 2);
                }
                id++;

                if (y == 0 && x == blocksPerLine - 1)
                {
                    emptyBlock = block;
                    block.id = 0;

                }
                solver.initialPos[block.id] = block.coord;
                solver.currentPos[block.id] = block.coord;
                solver.idOrder[block.id] = block.id;
                solver.correctidOrder[block.id] = block.id;
            }
        }

        Camera.main.orthographicSize = blocksPerLine * .55f;
        inputs = new Queue<Block>();
    }

    void PlayerMoveBlockInput(Block blockToMove)
    {
        if (state == PuzzleState.InPlay)
        {
            timer.hasStarted = true;
            inputs.Enqueue(blockToMove);
            MakeNextPlayerMove();
        }
    }

    void MakeNextPlayerMove()
    {
        while (inputs.Count > 0 && !blockIsMoving)
        {
            MoveBlock(inputs.Dequeue(), defaultMoveDuration);
            solver.gx++;
        }
    }

    public void CheckSurroundingBlock()
    { 
        //Vector2Int nearCoord;
        for (int j = 1; j < solver.initialPos.Length; j++)
        {
            if ((solver.currentPos[j] - solver.currentPos[0]).sqrMagnitude == 1)
            {
                nrEmptyBlocks++;
                //nearCoord = solver.currentPos[0];

                /*for (int i = 0; i < nrEmptyBlocks; i++)
                {
                    solver.surroundingIDs[i] = block.id;
                }*/
                state = PuzzleState.Solving;
                //solver.StartSolving(nrEmptyBlocks);
                
            }
        }
        solver.StartSolving(4);
        //Debug.LogWarning(nrEmptyBlocks);
    }

    public void MakeNextSolveMove(int blockMoveIndex)
    {
        Vector2Int[] offsets = { new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(-1, 0) };
        Vector2Int[] newCoord = { new Vector2Int(0, 0), new Vector2Int(0, 0), new Vector2Int(0, 0), new Vector2Int(0, 0) };
        int[] newCoordIDs = new int[offsets.Length];
        int moveBlockID = 0;

        if (state == PuzzleState.Solving)
        {
            for (int i = 0; i < offsets.Length; i++)
            {
                Vector2Int offset = offsets[(blockMoveIndex + i) % offsets.Length];

                newCoord[i] = new Vector2Int(emptyBlock.coord.x + offset.x, emptyBlock.coord.y + offset.y);
                Debug.Log(newCoord[i].x + ", " + newCoord[i].y);

                if (newCoord[i] == solver.initialPos[moveBlockID])
                {
                    solver.hx[i] -= 1;
                }
                //have hx/fx have only have up to 4 elements.

                if (offset != previousSolveOffset * -1)
                {
                    Vector2Int moveBlockCoord = emptyBlock.coord + offset;

                    if (moveBlockCoord.x >= 0 && moveBlockCoord.x < blocksPerLine && moveBlockCoord.y >= 0 && moveBlockCoord.y < blocksPerLine)
                    {
                        previousSolveOffset = offset;
                    }
                }
            }

            for (int k = 0; k < solver.hx.Length; k++)
            {
                solver.hx[k] = solver.currentHx;

                var blockToMove = offsets[k] + emptyBlock.coord;

                if (blockToMove.x < 0 || blockToMove.x >= blocksPerLine || blockToMove.y < 0 || blockToMove.y >= blocksPerLine || k >= offsets.Length)
                {
                    solver.hx[k] = 1000;
                }
                else
                {
                    for (int j = 0; j < solver.currentPos.Length; j++)
                    {
                        if (solver.currentPos[j] == newCoord[k])
                        {
                            newCoordIDs[k] = j;
                        }
                        else
                        {
                            newCoordIDs[k] = -1;
                        }

                        if (solver.currentPos[j] == solver.initialPos[j] && solver.currentPos[j] - offsets[k] != solver.initialPos[j])
                        {
                            solver.hx[k] += 1;
                        }
                        else if (solver.currentPos[j] - offsets[k] == solver.initialPos[j])
                        {
                            solver.hx[k] -= 1;
                        }
                        //Debug.LogError(newCoordIDs[k]);
                        //if block's current position - offset = its initial position, decrement hx.
                        //if the block is in the initial position and it moves out of it, then add 1.

                        /*if (solver.currentPos[j] == newCoord[j])
                        {
                            newCoordIDs[j] = j;
                        }*/
                    }
                }

                
                //MakeNextSolveMove();
                //CheckSurroundingBlock();
            }

            int min = solver.hx.Min();
            int minIndex = Array.IndexOf(solver.hx, min);

            Vector2Int vectorToMove = offsets[minIndex] + emptyBlock.coord;
            Block blockToActuallyMove = blocks[vectorToMove.x, vectorToMove.y];

            Debug.LogWarning(blockToActuallyMove.id);

            //Go through each move. Check if it is valid.
            //if invalid, give it a massive score.
            MoveBlock(blockToActuallyMove, defaultMoveDuration);
            
            //MakeNextPlayerMove();
            solver.lowestHx = solver.hx[minIndex];
            solver.ResetArrays();
            //MoveBlock(newCoordIDs[min])
        }
    }

    void MoveBlock(Block blockToMove, float duration)
    {
        /*check to see how many possible moves there are.
        basically, if there are no pieces next to it, then add one to the possible move count
        then create an fx/hx array with that many moves.
        */

        if ((blockToMove.coord - emptyBlock.coord).sqrMagnitude == 1)
        {
            blocks[blockToMove.coord.x, blockToMove.coord.y] = emptyBlock;
            blocks[emptyBlock.coord.x, emptyBlock.coord.y] = blockToMove;

            Vector2Int targetCoord = emptyBlock.coord;
            emptyBlock.coord = blockToMove.coord;
            solver.currentPos[emptyBlock.id] = blockToMove.coord;
            blockToMove.coord = targetCoord;
            solver.currentPos[blockToMove.id] = targetCoord;

            Vector2 targetPosition = emptyBlock.transform.position;
            emptyBlock.transform.position = blockToMove.transform.position;
            blockToMove.MoveToPosition(targetPosition, duration);
            blockIsMoving = true;
            //CheckSurroundingBlock();
            /*if (state == PuzzleState.InPlay)
            {
                CheckSurroundingBlock();
                solver.piecesMoved.Add(blockToMove.id);
            }*/
        }
    }

    void OnBlockFinishedMoving()
    {
        //add the block id to the history of pieces moved.
        blockIsMoving = false;
        CheckIfSolved();

        if(state == PuzzleState.InPlay)
        {
            MakeNextPlayerMove();
        }
        else if (state == PuzzleState.Shuffling)
        {
            if (shuffleMovesRemaining > 0)
            {
                MakeNextShuffleMove();
            }
            else
            {
                solver.currentPos[block.id] = block.coord;
                state = PuzzleState.InPlay;
                solver.Shuffled();
                CheckSurroundingBlock();
            }
        }
        else if (state == PuzzleState.Solving)
        {
            CheckSurroundingBlock();
        }
    }

    public void StartShuffle()
    {
        state = PuzzleState.Shuffling;
        shuffleMovesRemaining = shuffleLength;
        emptyBlock.gameObject.SetActive(false);
        MakeNextShuffleMove();
    }

    void MakeNextShuffleMove()
    {
        Vector2Int[] offsets = { new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(0, -1) };
        int randomIndex = UnityEngine.Random.Range(0, offsets.Length);

        for (int i = 0; i < offsets.Length; i++)
        {
            Vector2Int offset = offsets[(randomIndex + i) % offsets.Length];
            if(offset != previousShuffleOffset * -1)
            {
                Vector2Int moveBlockCoord = emptyBlock.coord + offset;

                if (moveBlockCoord.x >= 0 && moveBlockCoord.x < blocksPerLine && moveBlockCoord.y >= 0 && moveBlockCoord.y < blocksPerLine)
                {
                    MoveBlock(blocks[moveBlockCoord.x, moveBlockCoord.y], shuffleMoveDuration);
                    shuffleMovesRemaining--;
                    previousShuffleOffset = offset;
                    break;
                }
            }
        }
    }

    void CheckIfSolved()
    {
        foreach (Block block in blocks)
        {
            if (!block.IsAtStartingCoord())
            {
                return;
            }
        }

        state = PuzzleState.Solved;
        timer.hasStarted = false;
        timer.ResetTimer();
        emptyBlock.gameObject.SetActive(true);
    }

    public bool isDivisible(int n, int m)
    {
        return (n % m) == 0;
    }
}