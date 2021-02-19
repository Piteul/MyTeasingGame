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
        if (Input.GetMouseButtonDown(0))
        {
            GameObject currentCase = GetSelectCase();
            print("Current case : " + currentCase.GetComponent<Case>().index);
        }


    }

    GameObject GetSelectCase()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
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
