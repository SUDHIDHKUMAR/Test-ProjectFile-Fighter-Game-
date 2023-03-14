using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace FighterGame.UI
{
    public class ChooseTeamScreen : UIScreen
    {
        [Header("Team UI")]
        [SerializeField] Toggle teamRedToggle;
        [SerializeField] Toggle teamBlueToggle;
        [Header("Choose UI")]
        [SerializeField] Button chooseButton;
        [SerializeField] TMP_Text chooseButtonText;

        [SerializeField] Networking matchmaker;
        GameTeam playerTeam;

        public static Action onTeamSelected;

        private void OnEnable()
        {
            teamRedToggle.onValueChanged.AddListener(OnChoosingRed);
            teamBlueToggle.onValueChanged.AddListener(OnChoosingBlue);
            chooseButton.onClick.AddListener(OnChoodeButtonClick);
        }

        private void OnChoodeButtonClick()
        {
            Utility.teamChoosed = playerTeam;
            onTeamSelected?.Invoke();
        }

        private void OnChoosingRed(bool isOn)
        {

            chooseButtonText.text = "CONFIRM RED";
            playerTeam = GameTeam.RED_TEAM;
            SetChooseButton();
        }

        private void OnChoosingBlue(bool isOn)
        {
            chooseButtonText.text = "CONFIRM BLUE";
            playerTeam = GameTeam.BLUE_TEAM;
            SetChooseButton();
        }

        bool SetChooseButton()
        {
            if (teamBlueToggle.isOn || teamRedToggle.isOn)
            {
                chooseButton.interactable = true;
                return true;
            }
            else
            {
                chooseButtonText.text = "CHOOSE ANY";
                chooseButton.interactable = false;
                return false;
            }
        }
        private void OnDisable()
        {
            teamRedToggle.onValueChanged.RemoveListener(OnChoosingRed);
            teamBlueToggle.onValueChanged.RemoveListener(OnChoosingBlue);
            chooseButton.onClick.RemoveListener(OnChoodeButtonClick);
        }
    }
}