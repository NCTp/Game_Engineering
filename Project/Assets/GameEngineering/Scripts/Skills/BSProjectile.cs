using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSProjectile : MonoBehaviour
{
    GameObject target;
    float damage = 0f;
    float speed;
    float lifeTime = 10f; // 너무 긴 시간 동안 적중되지 않은 투사체를 자동 소멸시키기 위한 타이머

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
            Destroy(gameObject); // Target이 사라졌거나 lifeTime이 0 이하로 떨어지면 소멸
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
            other.GetComponent<Player_a>().TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Floor"))
            Destroy(gameObject); // 땅에 닿으면 소멸
    }
}
