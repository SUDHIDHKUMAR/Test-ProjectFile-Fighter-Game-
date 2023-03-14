using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public float movement;
    public NetworkBool isAttack01;
    public NetworkBool isAttack02;
    public NetworkBool isShielded;
}