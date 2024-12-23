using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System;

public class BossAgent_a : Agent
{
    private EnvManager_a env;
    private Boss_a boss;

    [Header("스킬 쿨다운 정보")]
    [SerializeField] private float baseBasicAttackCoolDown;
    private float remainBasicAttackCoolDown;
    [SerializeField] private float baseMeteoCoolDown;
    private float remainMeteoCoolDown;
    public bool isAttacking;

    public override void Initialize()
    {
        env = GetComponentInParent<EnvManager_a>();
        boss = GetComponent<Boss_a>();
        if (!env)
            Debug.LogError("Agent의 소속 환경을 찾을 수 없습니다.");
        if (!boss)
            Debug.LogError("Agnet가 제어할 Boss_a를 찾을 수 없습니다.");
    }

    public override void OnEpisodeBegin()
    {
        remainBasicAttackCoolDown = 0f;
        remainMeteoCoolDown = 0f;
        isAttacking = false;
        env.ResetEntireEnv();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        for(int i = 0; i < EnvManager_a.MAX_PLAYER; i++)
        {
            if((i < env.players.Length) && (env.players[i] != null) && (env.players[i].health > 0f))
            {
                sensor.AddObservation(env.players[i].transform.localPosition.x);
                sensor.AddObservation(env.players[i].transform.localPosition.z);
                sensor.AddObservation(env.players[i].health);
            }
            else
            {
                sensor.AddObservation(Vector3.zero.x); // 빈 부분에는 0으로 채운다.
                sensor.AddObservation(Vector3.zero.z);
                sensor.AddObservation(0f);
            }
        }
        sensor.AddObservation(boss.CurrentHealth);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Discrete Actions
        // branch 0 size : 3
        // branch 1 size : 5
        // Continuous Actions: 10
        int baseAction = actions.DiscreteActions[0];
        int targetCode = actions.DiscreteActions[1];
        switch(baseAction)
        {
            case 1:
                boss.m_basicAttack.LaunchSkill(env.players[targetCode - 1].gameObject);
                remainBasicAttackCoolDown = baseBasicAttackCoolDown;
                break;
            case 2:
                Vector3[] strikePos = new Vector3[5];
                for(int i = 0; i < 5; i++)
                {
                    float actionX = 20f * Mathf.Clamp(actions.ContinuousActions[i * 2], -1f, 1f);
                    float actionZ = 20f * Mathf.Clamp(actions.ContinuousActions[i * 2 + 1], -1f, 1f);
                    strikePos[i] = new Vector3(actionX, 0f, actionZ);
                }
                boss.m_meteo.LaunchSkill(strikePos);
                remainMeteoCoolDown = baseMeteoCoolDown;
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        remainBasicAttackCoolDown = 0f;
        remainMeteoCoolDown = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        remainBasicAttackCoolDown = Math.Max(0f, remainBasicAttackCoolDown - Time.deltaTime);
        remainMeteoCoolDown = Math.Max(0f, remainMeteoCoolDown - Time.deltaTime);
    }
    
    public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
    {
        bool basicAttackReady = remainBasicAttackCoolDown <= 0f;
        actionMask.SetActionEnabled(0, 1, basicAttackReady);
        bool isAPlayerAlive = false;
        for (int i = 1; i < 5; i++)
        {
            // 살아있는 player만 타겟으로 지정 가능
            bool value = basicAttackReady && i <= env.numPlayers && env.players[i - 1].isAlive;
            actionMask.SetActionEnabled(1, i, value);
            if (value)
                isAPlayerAlive = true;
        }
        if (isAPlayerAlive)
            actionMask.SetActionEnabled(1, 0, !basicAttackReady);
        else
            actionMask.SetActionEnabled(1, 0, true);

        actionMask.SetActionEnabled(0, 2, remainMeteoCoolDown <= 0f);
    }

    public void Dead()
    {
        env.GameOver(false);
    }
}
