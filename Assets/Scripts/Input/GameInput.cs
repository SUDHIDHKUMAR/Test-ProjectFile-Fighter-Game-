using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class GameInput : MonoBehaviour
{
    public float GetHorizondalInput()
    {
        return CrossPlatformInputManager.GetAxis("Horizontal");
    }

    public bool IsAttack01()
    {
        return CrossPlatformInputManager.GetButtonDown("Fire1");
    }
    public bool IsAttack02()
    {
        return CrossPlatformInputManager.GetButtonDown("Fire2");
    }
    public bool IsDefend()
    {
        return CrossPlatformInputManager.GetButtonDown("Fire3");
    }

    public NetworkInputData GetAllInputs()
    {
        return new NetworkInputData()
        {
            defend = IsDefend(),
            isAttack01 = IsAttack01(),
            isAttack02 = IsAttack02(),
            horDir = GetHorizondalInput()
        };
    }
}
