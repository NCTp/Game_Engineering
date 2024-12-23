using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteoInstance : MonoBehaviour
{
    BossAgent_a m_agent;

    float MAXDIST = 2f + 0.5f * Mathf.Sqrt(2f);
    float preDelay = 0.5f;
    float damage = 0f;
    float speed = 30f;

    // Update is called once per frame
    void Update()
    {
        if (preDelay > 0f)
        {
            preDelay -= Time.deltaTime;
            return;
        }

        if (transform.localPosition.y < 2f)
            Destroy(transform.parent.gameObject); 

        transform.position += Vector3.down * speed * Time.deltaTime;
    }

    public void Initialize(BossAgent_a agent, float preDelay, float damage, float speed)
    {
        m_agent = agent;
        this.preDelay = preDelay;
        this.damage = damage;
        this.speed = speed;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            float distX = Mathf.Abs(other.transform.position.x - transform.position.x);
            float distZ = Mathf.Abs(other.transform.position.z - transform.position.z);
            float dist = Mathf.Sqrt(distX * distX + distZ * distZ);
            m_agent.AddReward(0.1f * Mathf.Max(MAXDIST - dist, 0f) / MAXDIST);
            other.GetComponent<Player_a>().TakeDamage(damage);
            Destroy(transform.parent.gameObject);
        }
    }
}
