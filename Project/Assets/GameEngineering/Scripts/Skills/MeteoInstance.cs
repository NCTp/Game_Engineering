using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteoInstance : MonoBehaviour
{
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

        if (transform.localPosition.y < 3f)
            Destroy(transform.parent.gameObject); 

        transform.position += Vector3.down * speed * Time.deltaTime;
    }

    public void Initialize(float preDelay, float damage, float speed)
    {
        this.preDelay = preDelay;
        this.damage = damage;
        this.speed = speed;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            other.GetComponent<Player>().TakeDamage(damage);
            Destroy(transform.parent.gameObject);
        }
    }
}
