using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSProjectile : MonoBehaviour
{
    GameObject target;
    float damage;
    float speed;
    float lifeTime = 10f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        lifeTime -= Time.deltaTime;

        if(target && lifeTime > 0f)
        {
            Vector3 targetPos = target.transform.position;
            transform.LookAt(target.transform);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        }
        else
            Destroy(gameObject);
    }

    public void Initialize(GameObject target, float damage, float speed)
    {
        this.target = target;
        this.damage = damage;
        this.speed = speed;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == target)
        {
            // Damage Logic
            Destroy(gameObject);
        }
        else if (other.CompareTag("Floor"))
            Destroy(gameObject);
    }
}
