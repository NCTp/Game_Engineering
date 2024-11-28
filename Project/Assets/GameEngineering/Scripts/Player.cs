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
    public float maxHealth;
    public float speed;
    private Rigidbody m_rb;
    void Move()
    {
        if(m_rb != null)
        {
            m_rb.velocity = new Vector3(
                Random.Range(-speed, speed), 
                0f, 
                Random.Range(-speed, speed));
        }
        
    }
    void Awake()
    {
        m_rb = GetComponent<Rigidbody>();
    }
    // Start is called before the first frame update
    void Start()
    {
        Move();
    }

    // Update is called once per frame
    void Update()
    {   
    }
}
