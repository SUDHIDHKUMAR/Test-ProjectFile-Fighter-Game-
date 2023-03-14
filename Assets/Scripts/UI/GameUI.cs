using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace FighterGame.UI
{
    public class GameUI : UIScreen
    {
        public static GameUI Instance;

        public HealthBarUI healthBarLeft;
        public HealthBarUI healthBarRight;
        [SerializeField] TMP_Text roundText;
        [SerializeField] PopUpUI popUp;



        private void OnEnable()
        {
            GameManager.OnRoundFinished += OnRoundFinish;
            GameManager.OnGameOver += OnGameOver;

            healthBarLeft.SetColorAndName(Utility.side == PlayerSide.Left ? Utility.GetPlayerColor() : Utility.GetOpponentColor(), Utility.side == PlayerSide.Left ? "You" : "Opp");

            healthBarRight.SetColorAndName(Utility.side == PlayerSide.Right ? Utility.GetPlayerColor() : Utility.GetOpponentColor(), Utility.side == PlayerSide.Right ? "You" : "Opp");

            roundText.text = GameManager.Instance.currentRound.ToString();
        }
        private void OnDisable()
        {
            GameManager.OnRoundFinished -= OnRoundFinish;
            GameManager.OnGameOver -= OnGameOver;

        }
        private void OnGameOver(PlayerSide side)
        {
            popUp.Initialize("Game Over! \n" + (Utility.side == side ? "You-" : "Opp-") + " Win", 0, true);
        }

        private void OnRoundFinish(GameTeam team, PlayerSide side)
        {
            popUp.Initialize("Round Over! \n" + (Utility.side == side ? "You-" : "Opp-") + " Winned", 3);
            roundText.text = GameManager.Instance.currentRound.ToString();

        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        } 
    }
}