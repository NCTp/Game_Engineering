using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Processors;

public class Player_a : MonoBehaviour
{
    [Header("PlayerSettings")]
    public Transform boss;
    public Transform attackPoint;
    public float attackRange = 7f;
    public float moveSpeed = 4f;
    public float health = 100f;
    public float attackCooldown = 1f;
    public float attackDMG = 1f;
    public bool isAlive;

    private Vector3 startPosition;
    private float lastAttackTime = 0f;

    void Awake()
    {
        startPosition = transform.position;
        ResetPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToBoss = Vector3.Distance(transform.position, boss.position);

        if(IsBossAttacking())
        {
            AvoidBossAttack();
        }
        else if (distanceToBoss > attackRange)
        {
            MoveTowardsBoss();
        }
        else
        {
            Attack();
        }
    }

    private bool IsBossAttacking()
    {
        BossAgent_a bossAgent = boss.GetComponent<BossAgent_a>();
        if (bossAgent != null )
        {
            return bossAgent.isAttacking;
        }
        return false;
    }

    private void AvoidBossAttack()
    {
        Vector3 avoidDirection = (transform.position - boss.position).normalized;
        Vector3 movePosition = transform.position + avoidDirection * moveSpeed * Time.deltaTime;
        transform.position = movePosition;
    }

    private void MoveTowardsBoss()
    {
       Vector3 direction = (boss.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    private void Attack()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            if (Vector3.Distance(transform.position, boss.position) < attackRange)
            {
                Boss_a bossComponent = boss.GetComponent<Boss_a>();
                if (bossComponent != null)
                {
                    bossComponent.TakeDamage(attackDMG);
                }
            }
        }
    }

    public void ResetPlayer()
    {
        isAlive = true;
        health = 100f;
        transform.position = startPosition;
        lastAttackTime = 0f;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health < 0f)
            Dead();
    }

    public void Dead()
    {
        isAlive = false;
        transform.localPosition = Vector3.zero;
        gameObject.SetActive(false);
    }
}
