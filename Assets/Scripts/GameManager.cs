using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text timer;
    public Text bestScore;



    void Start()
    {

    }

    void Update()
    {

    }

    public void QuitGame()
    {
        SceneManager.LoadScene(0);
    }

}
