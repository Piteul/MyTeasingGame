using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{

    public GameObject[] cases;

    public Sprite[] androidSprite;
    public Sprite[] appleSprite;

    public GameObject[,] puzzleGrid;

    float squareLength = 0;
    int intSquareLength;
    public int maxRowAndColumn = 0;
    public GameObject emptyCase;
    public Vector2Int defaultEmptyCaseCoordinate;
    bool isGridInitialisation;

    [Header("Random Initialization")]
    public int NumberOfInitPermutation = 50;
    enum Direction { Up, Down, Right, Left };


    // Start is called before the first frame update
    void Start()
    {
        isGridInitialisation = true;
        InitGrid();
        RandomizeGrid();
        isGridInitialisation = false;


    }



    // Update is called once per frame
    void Update()
    {

    }

    public void InitGrid()
    {
        squareLength = Mathf.Sqrt(cases.Length);
        intSquareLength = Mathf.RoundToInt(squareLength);
        int index = 0;


        if (squareLength == intSquareLength)
        {

            puzzleGrid = new GameObject[intSquareLength, intSquareLength];
            maxRowAndColumn = intSquareLength - 1;

            for (int i = 0; i < squareLength; i++)
            {
                for (int j = 0; j < squareLength; j++)
                {
                    index = (int)(i * squareLength) + j;
                    //print("Index : " + index);

                    puzzleGrid[i, j] = cases[index];

                    puzzleGrid[i, j].GetComponent<Case>().index = index;
                    puzzleGrid[i, j].GetComponent<Case>().coordinate = new Vector2Int(i, j);

                    switch (GameManager.Instance.platform)
                    {
                        case GameManager.Platform.Android:
                            if (!puzzleGrid[i, j].GetComponent<Case>().isEmptyCase)
                            {
                                puzzleGrid[i, j].GetComponent<SpriteRenderer>().sprite = androidSprite[index];
                            }

                            break;
                        case GameManager.Platform.Apple:
                            if (!puzzleGrid[i, j].GetComponent<Case>().isEmptyCase)
                            {
                                puzzleGrid[i, j].GetComponent<SpriteRenderer>().sprite = appleSprite[index];
                            }
                            break;
                        default:
                            break;
                    }

                }
            }
        }
    }


    private void RandomizeGrid()
    {
        Direction direction;

        for (int i = 0; i < NumberOfInitPermutation; i++)
        {
            Vector2Int emptyCaseCoordinate = emptyCase.GetComponent<Case>().coordinate;
            direction = (Direction)UnityEngine.Random.Range(0, Enum.GetValues(typeof(Direction)).Length);
            //print("Direction :" + direction);

            switch (direction)
            {
                case Direction.Up:
                    if (emptyCaseCoordinate.x > 0)
                        SwitchCase(puzzleGrid[emptyCaseCoordinate.x - 1, emptyCaseCoordinate.y], emptyCase);
                    break;
                case Direction.Down:
                    if (emptyCaseCoordinate.x < maxRowAndColumn)
                        SwitchCase(puzzleGrid[emptyCaseCoordinate.x + 1, emptyCaseCoordinate.y], emptyCase);
                    break;
                case Direction.Right:
                    if (emptyCaseCoordinate.y < maxRowAndColumn)
                        SwitchCase(puzzleGrid[emptyCaseCoordinate.x, emptyCaseCoordinate.y + 1], emptyCase);
                    break;
                case Direction.Left:
                    if (emptyCaseCoordinate.y > 0)
                        SwitchCase(puzzleGrid[emptyCaseCoordinate.x, emptyCaseCoordinate.y - 1], emptyCase);
                    break;
                default:
                    break;
            }
        }

    }

    public void SwitchCase(GameObject selectedCase, GameObject emptyCase)
    {
        Vector2Int caseCoodinate = selectedCase.GetComponent<Case>().coordinate;
        Vector2Int emptyCaseCoodinate = emptyCase.GetComponent<Case>().coordinate;

        if (emptyCase.GetComponent<Case>().isEmptyCase)
        {
            //Temporarily saves the position of the case to be moved
            Vector3 tempLocalPos = puzzleGrid[caseCoodinate.x, caseCoodinate.y].GetComponent<RectTransform>().anchoredPosition;

            //Switch reference in puzzle grid
            puzzleGrid[emptyCaseCoodinate.x, emptyCaseCoodinate.y] = selectedCase;
            puzzleGrid[caseCoodinate.x, caseCoodinate.y] = emptyCase;

            //Update coordinate in each case
            emptyCase.GetComponent<Case>().coordinate = caseCoodinate;
            selectedCase.GetComponent<Case>().coordinate = emptyCaseCoodinate;

            //Switch position on screen
            selectedCase.GetComponent<RectTransform>().anchoredPosition = emptyCase.GetComponent<RectTransform>().anchoredPosition;
            emptyCase.GetComponent<RectTransform>().anchoredPosition = tempLocalPos;

            //We don't update index of each case, because this initial index is use to check the puzzle resolution

            if (!isGridInitialisation)
            {
                if (emptyCase.GetComponent<Case>().coordinate == defaultEmptyCaseCoordinate)
                {
                    if (CheckResolution())
                    {
                        GameManager.Instance.SetVictory();
                    }
                }
            }
        }

    }

    public bool CheckResolution()
    {
        int index = 0;

        for (int i = 0; i < squareLength; i++)
        {
            for (int j = 0; j < squareLength; j++)
            {
                index = (int)(i * squareLength) + j;


                if (puzzleGrid[i, j].GetComponent<Case>().index != index)
                {
                    return false;
                }

                //print("Index : " + index + ", Case Index : " + puzzleGrid[i, j].GetComponent<Case>().index);
            }

        }

        return true;
    }

    #region Singleton

    static PuzzleManager instance;

    public static PuzzleManager Instance
    {
        get { return instance; }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("A singleton can only be instantiated once!");
            Destroy(gameObject);
            return;
        }
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    #endregion
}
