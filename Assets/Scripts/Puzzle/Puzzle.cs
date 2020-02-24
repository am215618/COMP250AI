using System.Collections;
using System.Collections.Generic;
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
    int i;

    public Solver solver;

    GameObject blockObject;
    Block block;

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
        i = (blocksPerLine * blocksPerLine) - (blocksPerLine - 1);
        blocks = new Block[blocksPerLine, blocksPerLine];
        Texture2D[,] imageSlices = ImageSlicer.GetSlices(image, blocksPerLine);
        solver.initialPos = new Vector2Int[(blocksPerLine * blocksPerLine)];
        solver.currentPos = new Vector2Int[(blocksPerLine * blocksPerLine)];
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
                block.id = i;
                blocks[x, y] = block;
                if (isDivisible(i, blocksPerLine))
                {
                    i = i - (blocksPerLine * 2);
                }
                i++;

                if (y == 0 && x == blocksPerLine - 1)
                {
                    emptyBlock = block;
                    block.id = 0;
                }
                solver.initialPos[block.id] = block.coord;
                solver.currentPos[block.id] = block.coord;
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
        }
    }

    void MoveBlock(Block blockToMove, float duration)
    {
        /*check to see how many possible moves there are.
        basically, if there are no pieces next to it, then add one to the possible move count
        then create an fx/hx array with that many moves.
         */
        for (int j = 0; j < 4; j++)
        {
            if((blockToMove.coord - emptyBlock.coord).sqrMagnitude == 1)
            {
                PossibleMoves++;
            }
        }

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
            }
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
        int randomIndex = Random.Range(0, offsets.Length);

        for (int i = 0; i < offsets.Length; i++)
        {
            Vector2Int offset = offsets[(randomIndex + i) % offsets.Length];
            if(offset !=previousShuffleOffset * -1)
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
