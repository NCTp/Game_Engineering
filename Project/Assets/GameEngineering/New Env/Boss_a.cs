using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;
using UnityEngine.UI;

public class Boss_a : MonoBehaviour
{
    private float baseHealth = 100f;
    private float k = 1f;
    public float maxHealth;
    public float CurrentHealth{get; private set;}

    [HideInInspector] public BossAgent_a agent;
    [HideInInspector] public BasicAttack m_basicAttack;
    [HideInInspector] public Meteo m_meteo;
    private Slider hpBar;
    private Camera cameraToFollow;

    void Awake()
    {
        agent = GetComponent<BossAgent_a>();
        m_basicAttack = GetComponentInChildren<BasicAttack>();
        m_meteo = GetComponentInChildren<Meteo>();
        hpBar = GetComponentInChildren<Slider>();
        cameraToFollow = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    private void Update()
    {
        hpBar.transform.rotation = Quaternion.LookRotation(hpBar.transform.position - cameraToFollow.transform.position);
    }

    public void ResetBoss(int numPlayers)
    {
        maxHealth = baseHealth * numPlayers / (float)EnvManager_a.MAX_PLAYER * (1 + k * Mathf.Log10(numPlayers + 1f));
        CurrentHealth = maxHealth;
        hpBar.value = CurrentHealth / maxHealth;
    }

    public void TakeDamage(float value)
    {
        CurrentHealth -= value;
        hpBar.value = CurrentHealth / maxHealth;
        if(CurrentHealth < 0)
            agent.Dead();
    }
}

