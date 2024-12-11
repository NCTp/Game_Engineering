using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_a : MonoBehaviour
{
    public float maxHealth;
    public float CurrentHealth{get; private set;}

    public float BasicAttackInterval = 1f; //기본공격 딜레이 1초 설정(임의)
    private float currentAttackInterval;

    // Start is called before the first frame update
    void Start()
    {
        CurrentHealth = maxHealth;
        currentAttackInterval = BasicAttackInterval;  
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Reset()
    {

    }
}

