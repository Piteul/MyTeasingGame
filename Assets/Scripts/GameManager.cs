using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// GameManager
/// in this project I mainly use it for movements and secondary systems
/// Promise I will not do all my code here
/// </summary>
public class GameManager : MonoBehaviour
{

    bool isEndGame = false;

    [Header("UI Elements")]
    public GameObject endGamePanel;
    public Text timerText;
    public Text bestScoreText;
    public Text endGameTitleText;
    public Text endGameTimerText;

    public float timeTotal = 180f;
    public float currentTimerGame;
    float bestPlayerScore;

    Vector3 initialMousePosition;
    Vector3 actualMousePosition;
    Vector3 lastMousePosition;
    bool isMouseButtonPressed;

    //Refer to the selected case that the player want to move
    GameObject selectedCase;

    public enum Platform
    {
        Android,
        Apple
    }
    public Platform platform;


    void Start()
    {
        LoadPlayerInfo();
        SelectPlatform();

        isEndGame = false;
        endGamePanel.SetActive(false);
        currentTimerGame = timeTotal;
        bestScoreText.text = (int)(bestPlayerScore / 60) + ":" +
                         ((int)(((bestPlayerScore % 60) < 0) ? 0 : (bestPlayerScore % 60))).ToString("00");
    }

    private void Update()
    {
        InputManager();
    }

    void FixedUpdate()
    {
        if (!isEndGame)
        {
            TimerHandler();
        }
    }

    /// <summary>
    /// Use to know which sprites use for the grid
    /// </summary>
    void SelectPlatform()
    {
#if UNITY_EDITOR
        platform = Platform.Android;
#elif UNITY_IOS
        platform = Platform.Apple;
#elif UNITY_ANDROID
        platform = Platform.Android;
#endif
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
        //print("Victory");
        isEndGame = true;
        endGameTitleText.text = "Victory!";
        endGameTimerText.text = timerText.text;

        endGamePanel.SetActive(true);
        SavePlayerInfo();
    }

    public void SetGameOver()
    {
        //print("GameOver");
        isEndGame = true;
        endGameTitleText.text = "Game Over!";
        endGameTimerText.text = "00:00";

        endGamePanel.SetActive(true);
    }

    #region Save/Load System
    void LoadPlayerInfo()
    {
        bestPlayerScore = PlayerPrefs.HasKey("BestPlayerScore") ? PlayerPrefs.GetFloat("BestPlayerScore") : 0f;
    }

    public void SavePlayerInfo()
    {
        bestPlayerScore = currentTimerGame;
        PlayerPrefs.SetFloat("BestPlayerScore", bestPlayerScore);
        PlayerPrefs.Save();
    }

    #endregion


    #region Input / Movement
    void InputManager()
    {
        if (Input.GetMouseButtonDown(0) && isMouseButtonPressed == false)
        {
            isMouseButtonPressed = true;
            selectedCase = GetSelectCase();
            selectedCase.GetComponent<Case>().selectedSprite.SetActive(true);
            //print("Current case : " + currentCase.GetComponent<Case>().index);
            initialMousePosition = Input.mousePosition;


        }

        if (Input.GetMouseButtonUp(0))
        {
            isMouseButtonPressed = false;
            lastMousePosition = Input.mousePosition;

            MoveCase();

            selectedCase.GetComponent<Case>().selectedSprite.SetActive(false);
        }
    }

    /// <summary>
    /// Check player's movement direction to handle case move
    /// </summary>
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

        Vector2Int caseCoordinate = selectedCase.GetComponent<Case>().coordinate;
        GameObject emptyCase = null;

        //Move Up
        if (dotResultUp > 0.7f)
        {
            //print("MoveUp");

            if (caseCoordinate.x > 0)
            {
                emptyCase = PuzzleManager.Instance.puzzleGrid[caseCoordinate.x - 1, caseCoordinate.y];
            }
        }
        //Move Down
        else if (dotResultUp < -0.7f)
        {
            //print("MoveDown");

            if (caseCoordinate.x < PuzzleManager.Instance.maxRowAndColumn)
            {
                emptyCase = PuzzleManager.Instance.puzzleGrid[caseCoordinate.x + 1, caseCoordinate.y];
            }
        }
        //Move Right
        else if (dotResultRight > 0.7f)
        {
            //print("MoveRight");
            if (caseCoordinate.y < PuzzleManager.Instance.maxRowAndColumn)
            {
                emptyCase = PuzzleManager.Instance.puzzleGrid[caseCoordinate.x, caseCoordinate.y + 1];
            }
        }
        //Move Left
        else if (dotResultRight < -0.7f)
        {
            //print("MoveLeft");
            if (caseCoordinate.y > 0)
            {
                emptyCase = PuzzleManager.Instance.puzzleGrid[caseCoordinate.x, caseCoordinate.y - 1];
            }
        }
        else
        {
            Debug.LogWarning("Wrong / No Movement");
            return;
        }

        if (emptyCase != null)
        {
            PuzzleManager.Instance.SwitchCase(selectedCase, emptyCase);
        }
    }

    /// <summary>
    /// Find on which case the player clicked
    /// </summary>
    /// <returns></returns>
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
    #endregion


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
