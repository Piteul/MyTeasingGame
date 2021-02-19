﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//using SceneTransitionSystem;


namespace TeasingGame
{
    public enum TeasingGameScene : int
    {
        Home,
        Game,
    }
    public class TeasingGameHomeSceneController : MonoBehaviour
    {
        public TeasingGameScene SceneForButton;
        public Text bestPlayerScoreText;
        public float bestPlayerScore = 0;

        // Start is called before the first frame update
        void Start()
        {
            LoadPlayerInfo();
            if (bestPlayerScoreText)
            {
                bestPlayerScoreText.text = (int)(bestPlayerScore / 60) + ":" +
                            ((int)(((bestPlayerScore % 60) < 0) ? 0 : (bestPlayerScore % 60))).ToString("00");
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void GoToGameScene()
        {
            SceneManager.LoadScene(1);
        }

        void LoadPlayerInfo()
        {
            bestPlayerScore = PlayerPrefs.HasKey("BestPlayerScore") ? PlayerPrefs.GetFloat("BestPlayerScore") : 0;
            print("BPS" + bestPlayerScore);
        }
    }
}