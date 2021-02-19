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

    // Start is called before the first frame update
    void Start()
    {
        InitGrid();

        if (CheckResolution())
        {

        }
        //print(test);
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

            for (int i = 0; i < squareLength; i++)
            {
                for (int j = 0; j < squareLength; j++)
                {
                    index = (int)(i * squareLength) + j;
                    //print("Index : " + index);

                    puzzleGrid[i, j] = cases[index];

                    puzzleGrid[i, j].GetComponent<Case>().index = index;

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

    public bool CheckResolution()
    {
        int index = 0;
        if (puzzleGrid[1, 1].GetComponent<Case>().isEmptyCase)
        {
            for (int i = 0; i < squareLength; i++)
            {
                for (int j = 0; j < squareLength; j++)
                {
                    index = (int)(i * squareLength) + j;

                    if (puzzleGrid[i, j].GetComponent<Case>().index != index)
                    {
                        return false;
                    }
                }

            }

            return true;
        }
        return false;
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
