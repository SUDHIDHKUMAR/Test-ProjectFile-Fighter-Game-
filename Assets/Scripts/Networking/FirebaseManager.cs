
using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseManager : MonoBehaviour
{
    string uID;
    DatabaseReference dbRef;
    public static FirebaseManager Instance;
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

    private void Start()
    {
        uID = SystemInfo.deviceUniqueIdentifier;
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        SaveData(GameTeam.BLUE_TEAM, true);
    }

    public void SaveData(GameTeam team, bool iswinner)
    {
        PlayerData data = new PlayerData(team, iswinner);
        string jsonData = JsonUtility.ToJson(data);
        dbRef.Child("Players").Child(uID).Child(System.DateTime.Now.ToString()).SetRawJsonValueAsync(jsonData);
    }
}

public class PlayerData
{
    public string Team;
    public bool isWinner;
    public PlayerData(GameTeam _team, bool _iswinner)
    {
        Team = _team.ToString();
        isWinner = _iswinner;
    }
}
