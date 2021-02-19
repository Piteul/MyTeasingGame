using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text timerText;
    public Text bestScoreText;

    public float timeTotal = 180f;
    public float currentTimerGame;

    Vector3 initialMousePosition;
    Vector3 actualMousePosition;
    Vector3 lastMousePosition;
    bool isMouseButtonPressed;

    GameObject selectedCase;




    public enum Platform
    {
        Android,
        Apple
    }

    public Platform platform;
    void Start()
    {
        currentTimerGame = timeTotal;
    }

    private void Update()
    {
        InputManager();
    }

    void FixedUpdate()
    {
        TimerHandler();
    }

    void TimerHandler()
    {
        currentTimerGame -= Time.fixedDeltaTime;
        timerText.text = (int)(currentTimerGame / 60) + ":" +
                         ((int)(((currentTimerGame % 60) < 0) ? 0 : (currentTimerGame % 60))).ToString("00");

        if (currentTimerGame <= 0)
        {
            SetGameOver();
        }
    }

    public void QuitGame()
    {
        SceneManager.LoadScene(0);
    }

    public void SetVictory()
    {
        print("Victory");

    }

    public void SetGameOver()
    {
        print("GameOver");
    }

    void InputManager()
    {
        if (Input.GetMouseButtonDown(0) && isMouseButtonPressed == false)
        {
            selectedCase = GetSelectCase();
            //print("Current case : " + currentCase.GetComponent<Case>().index);
            initialMousePosition = Input.mousePosition;


        }


        if (Input.GetMouseButtonUp(0))
        {
            isMouseButtonPressed = false;
            lastMousePosition = Input.mousePosition;

            MoveCase();

        }
    }

    void MoveCase()
    {
        if (selectedCase == null)
        {
            return;
        }

        Vector3 direction = lastMousePosition - initialMousePosition;
        direction = direction.normalized;

        Vector3 temp = actualMousePosition - lastMousePosition;
        float dotResultUp = Vector2.Dot(direction, Vector2.up);
        float dotResultRight = Vector2.Dot(direction, Vector2.right);

        Vector2Int caseCoodinate = selectedCase.GetComponent<Case>().coordinate;
        GameObject emptyCase = null;
        Vector2Int emptyCaseCoodinate;


        //Move Up
        if (dotResultUp > 0.7f)
        {
            //print("MoveUp");

            if (caseCoodinate.x > 0)
            {
                emptyCase = PuzzleManager.Instance.puzzleGrid[caseCoodinate.x - 1, caseCoodinate.y];
            }
        }
        //Move Down
        else if (dotResultUp < -0.7f)
        {
            //print("MoveDown");

            if (caseCoodinate.x < PuzzleManager.Instance.maxRowAndColumn)
            {
                emptyCase = PuzzleManager.Instance.puzzleGrid[caseCoodinate.x + 1, caseCoodinate.y];
            }
        }
        //Move Right
        else if (dotResultRight > 0.7f)
        {
            //print("MoveRight");
            if (caseCoodinate.y < PuzzleManager.Instance.maxRowAndColumn)
            {
                emptyCase = PuzzleManager.Instance.puzzleGrid[caseCoodinate.x , caseCoodinate.y + 1];
            }
        }
        //Move Left
        else if (dotResultRight < -0.7f)
        {
            //print("MoveLeft");
            if (caseCoodinate.y > 0)
            {
                emptyCase = PuzzleManager.Instance.puzzleGrid[caseCoodinate.x, caseCoodinate.y - 1];
            }
        }
        else
        {
            Debug.LogError("Wrong Movement");
        }

        emptyCaseCoodinate = emptyCase.GetComponent<Case>().coordinate;

        if (emptyCase.GetComponent<Case>().isEmptyCase)
        {
            //Temporarily saves the position of the case to be moved
            Vector3 tempLocalPos = PuzzleManager.Instance.puzzleGrid[caseCoodinate.x, caseCoodinate.y].GetComponent<RectTransform>().anchoredPosition;

            //Switch reference in puzzle grid
            PuzzleManager.Instance.puzzleGrid[emptyCaseCoodinate.x, emptyCaseCoodinate.y] = selectedCase;
            PuzzleManager.Instance.puzzleGrid[caseCoodinate.x, caseCoodinate.y] = emptyCase;


            //Update coordinate in each case
            PuzzleManager.Instance.puzzleGrid[caseCoodinate.x, caseCoodinate.y].GetComponent<Case>().coordinate = caseCoodinate;
            PuzzleManager.Instance.puzzleGrid[emptyCaseCoodinate.x, emptyCaseCoodinate.y].GetComponent<Case>().coordinate = emptyCaseCoodinate;

            //Switch position on screen
            PuzzleManager.Instance.puzzleGrid[emptyCaseCoodinate.x, emptyCaseCoodinate.y].GetComponent<RectTransform>().anchoredPosition = PuzzleManager.Instance.puzzleGrid[caseCoodinate.x, caseCoodinate.y].GetComponent<RectTransform>().anchoredPosition;
            PuzzleManager.Instance.puzzleGrid[caseCoodinate.x, caseCoodinate.y].GetComponent<RectTransform>().anchoredPosition = tempLocalPos;

            //We don't update index of each case, because this initial index is use to check the puzzle resolution


        }
    }

    GameObject GetSelectCase()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        // RaycastHit2D ray =  Physics2D.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), Vector2.zero);
        // RaycastHit2D hit;

        if (hit.collider != null)
        {
            if (hit.collider.GetComponent("Case"))
            {
                return hit.transform.gameObject;
            }
        }

        return null;
    }


    #region Singleton

    static GameManager instance;

    public static GameManager Instance
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
