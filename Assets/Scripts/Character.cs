using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;
using FighterGame.UI;

public class Character : NetworkBehaviour
{

    public GameTeam team;
    public PlayerSide side;
    public int dir = 1;

    [SerializeField] Animator[] characterAnimators;

    [SerializeField] Transform attackPoint;
    [SerializeField] float attackRadious = 1f;

    [SerializeField] int attack1Power;
    [SerializeField] int attack2Power;


    float takeAttacktime = 1f;


    [Networked(OnChanged = nameof(OnHPChanged))]
    byte Health { get; set; }

    [Networked(OnChanged = nameof(OnDead))]
    NetworkBool IsDead { get; set; }

  
    bool defence;

    private void OnEnable()
    {
        GameManager.OnStartRound += RestPlayerData;
    }
    private void OnDisable()
    {
        GameManager.OnStartRound -= RestPlayerData;

    }

    private void RestPlayerData()
    {
        Health = 100;
        IsDead = false;
        if (side == PlayerSide.Right)
            transform.position = new Vector3(1.5f, 0, 0);
        else
            transform.position = new Vector3(-1.5f, 0, 0);
    }

    private void Start()
    {
        Health = 100;
        if (transform.position.x > 0)
        {
            side = PlayerSide.Right;
            dir = -1;
        }
    }
    private static void OnHPChanged(Changed<Character> changed)
    {
        float health = changed.Behaviour.Health;

        if (changed.Behaviour.side == PlayerSide.Left)
        {
            GameUI.Instance.healthBarLeft.SetHeathBar(health / 100);
        }
        if (changed.Behaviour.side == PlayerSide.Right)
            GameUI.Instance.healthBarRight.SetHeathBar(health / 100);

        changed.Behaviour.CheckHealth();
        /* if (health < 100)
             changed.Behaviour.SetAnimations("Hit");*/

    }

    public override void FixedUpdateNetwork()
    {
        takeAttacktime -= Runner.DeltaTime;
        if (GetInput(out NetworkInputData data))
        {
            transform.Translate(5 * new Vector3(0, 0, data.horDir * dir).normalized * Runner.DeltaTime);
            SetAnimations(data);
            if (takeAttacktime < 0)
            {
                if (data.isAttack01)
                    Attack(10);
                else if (data.isAttack02)
                    Attack(20);
                defence = data.defend;
            }

        }
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -2.5f, 2.5f), 0, 0);

    }

    void Attack(byte power)
    {
        Collider[] cols = Physics.OverlapSphere(attackPoint.transform.position, attackRadious);

        foreach (var col in cols)
        {
            Character chara = col.GetComponent<Character>();
            if (chara != this)
                chara.Damage(power);
        }
        takeAttacktime = 1;
    }

    void SetAnimations(NetworkInputData data)
    {
        foreach (var anim in characterAnimators)
        {
            anim.SetBool("Attack01", data.isAttack01);
            anim.SetBool("Attack02", data.isAttack02);
            anim.SetBool("Defend", data.defend);
        }
    }
    void SetAnimations(string animName)
    {
        foreach (var anim in characterAnimators)
        {
            anim.SetTrigger(animName);
        }
    }
    public float GetPlayerHealth()
    {
        return Health;
    }
    public void Damage(byte damage)
    {
        if (!defence)
        {
            Health -= damage;
        };
    }
    public void CheckHealth()
    {
        if (Health <= 0)
        {
            if (!IsDead)
                Die();
        }
    }
    void Die()
    {
        Debug.LogError("Dead" + team.ToString());
        IsDead = true;
    }
    private static void OnDead(Changed<Character> changed)
    {
        if (changed.Behaviour.IsDead)
            GameManager.Instance.RoundFinished(Utility.GetOppositeTeam(changed.Behaviour.team), Utility.GetOppositeSide(changed.Behaviour.side));
    }

}
