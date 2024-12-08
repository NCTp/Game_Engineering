using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteoInstance : MonoBehaviour
{
    GameObject stone;
    float preDelay = 0.1f;
    float speed = 17.5f;

    // Start is called before the first frame update
    void Start()
    {
        stone = transform.Find("stone").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (preDelay > 0f)
        {
            preDelay -= Time.deltaTime;
            return;
        }

        if (stone.transform.localPosition.y < 3f)
            Destroy(gameObject); 

        stone.transform.position += Vector3.down * speed * Time.deltaTime;
    }

    public void Initialize(float preDelay, float speed)
    {
        this.preDelay = preDelay;
        this.speed = speed;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            other.GetComponent<Player>().TakeDamage(0f);
            Destroy(gameObject);
        }
    }
}
