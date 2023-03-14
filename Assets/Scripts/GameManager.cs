using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;
using FighterGame.UI;
using Fusion;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static Action<PlayerSide> OnRoundFinished;
    public static Action<PlayerSide> OnGameOver;
    public static Action OnStartRound;

    [SerializeField] int numberOfRounds = 3;
    int CurrentRound = 1;
    bool isGameOver;

    List<PlayerSide> winningSide = new List<PlayerSide>();

    bool isCalledOnce;
    private void OnEnable()
    {
        Networking.OnPlayerLeftGame += OnPlayerLeft;
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

    public int GetCurrentRound()
    {
        return CurrentRound;
    }

    private void OnPlayerLeft(PlayerSide side)
    {
        if (!isGameOver)
            OnGameOver?.Invoke(Utility.GetOppositeSide(side));
    }


    public void RoundFinished(PlayerSide winnerSide)
    {
        if (isCalledOnce) return;
        CurrentRound++;
        winningSide.Add(winnerSide);
        if (CurrentRound > numberOfRounds || IsStreak())
        {
            GameOver();
        }
        else
        {
            OnRoundFinished?.Invoke(winnerSide);
        }
        Invoke(nameof(ResetCall), 1f);
        isCalledOnce = true;
    }
    void ResetCall()
    {
        isCalledOnce = false;
    }
    public void ResetGame()
    {
        winningSide.Clear();
        CurrentRound = 1;
    }



    PlayerSide GetWinnerSide()
    {
        int leftWin = 0;
        foreach (var win in winningSide)
        {
            if (win == PlayerSide.Left)
                leftWin++;
        }
        if (leftWin >= 2)
            return PlayerSide.Left;
        else return PlayerSide.Right;
    }

    bool IsStreak()
    {
        if (winningSide.Count > 1)
            return winningSide[0] == winningSide[1];
        else return false;
    }
    public void StartRound()
    {
        OnStartRound?.Invoke();
    }

    public void GameOver()
    {
        isGameOver = true;
        OnGameOver?.Invoke(GetWinnerSide());

        if (Utility.side == GetWinnerSide())
        {
            FirebaseManager.Instance.SaveData(Utility.teamChoosed, true);
        }
        else
            FirebaseManager.Instance.SaveData(Utility.teamChoosed, false);

    }
    private void OnDisable()
    {
        Networking.OnPlayerLeftGame -= OnPlayerLeft;
    }

}
