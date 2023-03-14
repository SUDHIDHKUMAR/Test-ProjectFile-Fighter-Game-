using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public float horDir;
    public NetworkBool isAttack01;
    public NetworkBool isAttack02;
    public NetworkBool defend;
}