using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace FighterGame.UI
{
    public class MatchScreen : UIScreen
    {
        [SerializeField] PlayerProfileUI opponent;
        [SerializeField] PlayerProfileUI player;
        [SerializeField] Timer timer;
        public static Action OnMatchingFinished;

        public override void InitializeScreen()
        {
            opponent.SetColorAndName(Utility.GetOpponentColor(), "Opponent");
            player.SetColorAndName(Utility.GetPlayerColor(), "You");
            base.InitializeScreen();
            timer.StartTimer(3f, () => { OnMatchingFinished?.Invoke(); });
        }
    }
}