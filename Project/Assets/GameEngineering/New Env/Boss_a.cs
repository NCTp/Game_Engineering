using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;

public class Boss_a : MonoBehaviour
{
    public float maxHealth;
    public float CurrentHealth{get; private set;}

    [HideInInspector] public BossAgent_a agent;
    [HideInInspector] public BasicAttack m_basicAttack;
    [HideInInspector] public Meteo m_meteo;

    void Awake()
    {
        m_basicAttack = GetComponentInChildren<BasicAttack>();
        m_meteo = GetComponentInChildren<Meteo>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Reset()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(float value)
    {
        CurrentHealth -= value;
        if(CurrentHealth < 0)
            agent.Dead();
    }
}

