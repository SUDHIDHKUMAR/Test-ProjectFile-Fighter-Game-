
using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance;

    private const string ROOT_USERS = "Players";

    private string uID;
    private DatabaseReference dbRef;


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
    }
    // Save user game data
    public void SaveData(GameTeam team, bool iswinner)
    {
        PlayerData data = new PlayerData(team, iswinner);
        string jsonData = JsonUtility.ToJson(data);
        dbRef.Child(ROOT_USERS).Child(uID).Child(System.DateTime.Now.ToString()).SetRawJsonValueAsync(jsonData);
    }
}

public class PlayerData
{
    public string playerTeam;
    public bool isWinner;
    public PlayerData(GameTeam _team, bool _iswinner)
    {
        playerTeam = _team.ToString();
        isWinner = _iswinner;
    }
}
