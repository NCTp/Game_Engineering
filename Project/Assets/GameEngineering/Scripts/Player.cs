using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    private Vector3 m_incomingVector;
    private float m_CurrentSpeed;
    public enum PlayerType // 후반부 타겟 우선순위 학습을 위한 역할 분류
    {
        Dealer,
        Tanker,
        Healer
    }
    public float maxHealth;
    public float m_CurrentHealth {get; private set;}
    public float speed;
    private Rigidbody m_rb;

    public void ResetParameters()
    {
        m_CurrentSpeed = Random.Range(-speed, speed);
        Move();
    }
    void Move()
    {
        if(m_rb != null)
        {
            m_rb.velocity = new Vector3(
                Random.Range(-m_CurrentSpeed, m_CurrentSpeed),
                0f,
                Random.Range(-m_CurrentSpeed, m_CurrentSpeed));
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
