using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    public enum PlayerType // 후반부 타겟 우선순위 학습을 위한 역할 분류
    {
        Dealer,
        Tanker,
        Healer
    }

    [Header("Player Settings")]
    public PlayerType playerType;
    public float maxHealth;
    public float m_CurrentHealth {get; private set;}
    public float speed;
    private Rigidbody m_rb;

    [Header("Attack Settings")]
    public float attackPower = 1.0f;
    public float attackRate = 1.0f;

    public float attackTimer = 0.0f;

    [Header("Boss Detection")]
    [SerializeField]
    public GameObject bossEyeAgent;

    private Vector3 m_incomingVector;
    private float m_CurrentSpeed;
    public void ResetParameters()
    {
        InitPlayer();
        Move(); // Move 메서드를 통해 velocity를 정해준다.
    }
    void InitPlayer()
    {
        attackTimer = 0.0f;
        if (playerType == PlayerType.Dealer)
        {
            m_CurrentSpeed = speed * 2.0f;
            m_CurrentHealth = maxHealth;
        }
        else if (playerType == PlayerType.Tanker)
        {
            m_CurrentSpeed = speed / 2.0f;
            m_CurrentHealth  = maxHealth * 2.0f;
            attackPower /= 2.0f;

        }
        else if (playerType == PlayerType.Healer)
        {
            m_CurrentSpeed = speed;
            m_CurrentHealth  = maxHealth * 1.5f;
            attackPower /= 5.0f;
        }

        bossEyeAgent = GameObject.Find("BossEyeAgent");
        if (bossEyeAgent != null)
        {
            Debug.Log("보스의 이름은 : " + bossEyeAgent.name);
        }
        else
        {
            Debug.Log("보스를 찾지 못했습니다.");
        }
    }
    void Move()
    {
        if(m_rb != null)
        {

            m_rb.velocity = new Vector3(
                Random.Range(-m_CurrentSpeed, m_CurrentSpeed),
                0f,
                Random.Range(-m_CurrentSpeed, m_CurrentSpeed)).normalized * m_CurrentSpeed;

            //m_rb.velocity = Vector3.forward * m_CurrentSpeed;
            m_incomingVector = m_rb.velocity;
        }

    }
    public void TakeDamage(float amount)
    {
        m_CurrentHealth -= amount;
        Debug.Log(m_CurrentHealth);
        if(m_CurrentHealth <= 0)
        {
            Dead();
        }
    }

    void Attack()
    {
        if (bossEyeAgent != null)
        {
            //Debug.Log("보스를 향해 공격합니다. Target : " + bossEyeAgent.transform.position);
            bossEyeAgent.GetComponent<BossEyeAgent>().TakeDamage(attackPower);
        }
    }

    void Dead()
    {
        Destroy(this.gameObject);
    }

    void Awake()
    {
        m_rb = GetComponent<Rigidbody>();
        m_CurrentSpeed = Random.Range(-speed, speed);
        m_CurrentHealth = maxHealth;
    }
    // Start is called before the first frame update
    void Start()
    {
        Move();
    }

    void Update()
    {
        //transform.position = new Vector3(transform.position.x, 0.0f, transform.position.z);
        attackTimer += Time.deltaTime;
        if (attackTimer >= attackRate)
        {
            Attack();
            attackTimer = 0.0f;
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("wall"))
        {
            Debug.Log("플레이어가 벽에 부딪혔습니다.");
            if (m_rb != null)
            {
                // 충돌 지점의 법선 벡터를 가져옵니다
                Vector3 normal = other.contacts[0].normal;
                // 현재 속도 벡터를 반사시킵니다
                Vector3 reflectedVelocity = Vector3.Reflect(m_rb.velocity, normal);
                // 반사된 방향으로 속도를 적용합니다
                m_rb.velocity = reflectedVelocity.normalized * m_CurrentSpeed;
                // 다음 충돌을 위해 입사 벡터를 업데이트합니다
                m_incomingVector = m_rb.velocity;
            }
        }
    }
}
