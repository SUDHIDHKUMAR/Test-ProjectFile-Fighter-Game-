using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class TestMovement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float movement = CrossPlatformInputManager.GetAxis("Horizontal");
        transform.Translate(new Vector3(movement, 0, 0) * Time.deltaTime * 10);
        if (CrossPlatformInputManager.GetButtonDown("Fire3"))
        {
            Debug.LogError("Attack");
        }
    }
}
