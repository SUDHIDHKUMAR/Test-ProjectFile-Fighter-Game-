using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class GameInput : MonoBehaviour
{

    private const string MOVEMENT_INPUT = "Horizontal";
    private const string ATTACK_01 = "Fire1";
    private const string ATTACK_02 = "Fire2";
    private const string SHIELD = "Fire3";


    public float GetHorizondalInput()
    {
        return CrossPlatformInputManager.GetAxis(MOVEMENT_INPUT);
    }

    public bool GetGameButtonDown(string buttonName)
    {
        return CrossPlatformInputManager.GetButtonDown(buttonName);
    }

    public NetworkInputData GetAllInputs()
    {
        return new NetworkInputData()
        {
            isShielded = GetGameButtonDown(SHIELD),
            isAttack01 = GetGameButtonDown(ATTACK_01),
            isAttack02 = GetGameButtonDown(ATTACK_02),
            movement = GetHorizondalInput()
        };
    }
}
