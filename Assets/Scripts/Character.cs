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


    float takeAttacktime = 0f;
    float attackRate = 1f;
    bool isShielded;

    const string ATTACK_ANIMATION_1 = "Attack01";
    const string ATTACK_ANIMATION_2 = "Attack02";
    const string SHIELD_ANIMATION = "Defend";


    [Networked(OnChanged = nameof(OnHPChanged))]
    float Health { get; set; }

    [Networked(OnChanged = nameof(OnDead))]
    NetworkBool IsDead { get; set; }



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
        if (GetInput(out NetworkInputData data))
        {
            SetAnimations(data);

            transform.Translate(5 * new Vector3(0, 0, data.movement * dir).normalized * Runner.DeltaTime);
            if (Time.time >= takeAttacktime)
            {
                if (data.isAttack01)
                    Attack(10f);
                else if (data.isAttack02)
                    Attack(20f);
                isShielded = data.isShielded;
            }
        }
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -2.5f, 2.5f), 0, 0);

    }

    void Attack(float power)
    {
        Collider[] cols = Physics.OverlapSphere(attackPoint.transform.position, attackRadious);

        foreach (var col in cols)
        {
            Character chara = col.GetComponent<Character>();
            if (chara != this)
                chara.TakeDamage(power);
        }
        takeAttacktime = Time.time + 2f / attackRate;
    }
    void SetAnimations(NetworkInputData data)
    {
        foreach (var anim in characterAnimators)
        {
            anim.SetBool(ATTACK_ANIMATION_1, data.isAttack01);
            anim.SetBool(ATTACK_ANIMATION_2, data.isAttack02);
            anim.SetBool(SHIELD_ANIMATION, data.isShielded);
        }
    }
  
    public float GetPlayerHealth()
    {
        return Health;
    }

    float takeDamage = 0;
    public void TakeDamage(float damage)
    {
        if (!isShielded)
        {
            takeDamage = damage;
            //Health -= damage;
            Invoke(nameof(Damage), .2f);
        };
    }
    void Damage()
    {
        Health -= takeDamage;
    }

    public void CheckHealth()
    {
        if (IsDead) return;
        if (Health <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        Debug.LogError("Dead" + team.ToString());
        IsDead = true;
        //  GameManager.Instance.RoundFinished(Utility.GetOppositeSide(side));

    }
    private static void OnDead(Changed<Character> changed)
    {
        if (changed.Behaviour.IsDead)
        {
            Debug.LogError("Dead" + changed.Behaviour.team.ToString() + changed.Behaviour.side.ToString());
            GameManager.Instance.RoundFinished(Utility.GetOppositeSide(changed.Behaviour.side));

        }
    }

}
