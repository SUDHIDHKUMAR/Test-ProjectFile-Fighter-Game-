using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;
using FighterGame.UI;
using Fusion;

public class GameManager : MonoBehaviour
{
    public static Action<GameTeam, PlayerSide> OnRoundFinished;
    public static Action<PlayerSide> OnGameOver;
    public static Action OnStartRound;

    [SerializeField] int numberOfRounds = 3;
    public int currentRound = 1;

    public static GameManager Instance;

    List<PlayerSide> winngSides = new List<PlayerSide>();
    private void OnEnable()
    {
        Networking.OnPlayerLeftGame += OnPlayerLeft;
    }

    private void OnDisable()
    {
        Networking.OnPlayerLeftGame -= OnPlayerLeft;

    }
    private void OnPlayerLeft(PlayerSide side)
    {
        OnGameOver?.Invoke(Utility.GetOppositeSide(side));
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
    public void RoundFinished(GameTeam winnerTeam, PlayerSide winnerSide)
    {
        currentRound++;
        if (currentRound > numberOfRounds || IsStreak())
        {
            GameOver();
        }
        else
        {
            winngSides.Add(winnerSide);
            OnRoundFinished?.Invoke(winnerTeam, winnerSide);
        }
    }

    public void ResetGame()
    {
        winngSides.Clear();
        currentRound = 1;
    }

    public void GameOver()
    {
        ResetGame();
        OnGameOver?.Invoke(GetWinnerSide());

        if (Utility.side == GetWinnerSide())
        {
            FirebaseManager.Instance.SaveData(Utility.teamChoosed, true);
        }
        else
            FirebaseManager.Instance.SaveData(Utility.teamChoosed, false);

    }

    PlayerSide GetWinnerSide()
    {
        int leftWin = 0;
        foreach (var win in winngSides)
        {
            if (win == PlayerSide.Left)
                leftWin++;
        }
        if (leftWin > 1)
            return PlayerSide.Left;
        else return PlayerSide.Right;
    }

    bool IsStreak()
    {
        if (winngSides.Count < 2)
            return false;
        else
        {
            PlayerSide firstElem = winngSides[0];
            for (int i = 1; i < winngSides.Count; i++)
            {
                if (winngSides[i] != firstElem)
                    return false;
            }

            return true;
        }
    }
    public void StartRound()
    {
        OnStartRound?.Invoke();
    }


}
