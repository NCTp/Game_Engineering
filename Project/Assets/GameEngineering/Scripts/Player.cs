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
        }
        else if (playerType == PlayerType.Healer)
        {
            m_CurrentSpeed = speed;
            m_CurrentHealth  = maxHealth * 1.5f;
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
            /*
            m_rb.velocity = new Vector3(
                Random.Range(-m_CurrentSpeed, m_CurrentSpeed),
                0f,
                Random.Range(-m_CurrentSpeed, m_CurrentSpeed));
                */
            m_rb.velocity = new Vector3(m_CurrentSpeed, 0, m_CurrentSpeed);
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
            Debug.Log("보스를 향해 공격합니다. Target : " + bossEyeAgent.transform.position);
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
                Vector3 normal = other.contacts[0].normal;
                Vector3 incomingVelocity = m_incomingVector;
                Vector3 reflectedVelocity = Vector3.Reflect(incomingVelocity, normal).normalized;
                Debug.Log("반사 노말 " + normal);
                Debug.Log("현재 속도 " + incomingVelocity);
                Debug.Log("반사 속도 " + reflectedVelocity);
                m_rb.velocity = -reflectedVelocity * m_CurrentSpeed;
                m_incomingVector = m_rb.velocity;

            }
        }
    }
}
