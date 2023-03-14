using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FighterGame.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] List<UIScreen> UIScreens;

        private void OnEnable()
        {
            ChooseTeamScreen.onTeamSelected += LoadingScreen;
            MatchScreen.OnMatchingFinished += GoToGame;
            Networking.OnGettingOpponent += MatchingScreen;
            Networking.OnDisconnect += GetBackToHomeScreen;
        }

        private void OnDisable()
        {
            ChooseTeamScreen.onTeamSelected -= LoadingScreen;
            Networking.OnGettingOpponent -= MatchingScreen;
            Networking.OnDisconnect -= GetBackToHomeScreen;
            MatchScreen.OnMatchingFinished -= GoToGame;
        }
        private void Start()
        {
            HideAllScreens();
            UIScreens[0].InitializeScreen();
        }
        void GoToGame()
        {
            UIScreens[2].DeinitializeScreen();
            UIScreens[3].InitializeScreen();
        }
        private void MatchingScreen()
        {
            UIScreens[1].DeinitializeScreen();
            UIScreens[2].InitializeScreen();
        }

        private void LoadingScreen()
        {
            UIScreens[0].DeinitializeScreen();
            UIScreens[1].InitializeScreen();
        }

        void HideAllScreens()
        {
            foreach (var screens in UIScreens)
            {
                screens.DeinitializeScreen();
            }
        }

        public void GetBackToHomeScreen()
        {
            Networking.Instance.Disconnect();
            HideAllScreens();
            UIScreens[0].InitializeScreen();
            GameManager.Instance.ResetGame();
        }
    }
}