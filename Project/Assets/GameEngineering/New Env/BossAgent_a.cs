using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System;

public class BossAgent_a : Agent
{
    public EnvManager_a env;
    private Boss_a boss;

    [Header("스킬 쿨다운 정보")]
    [SerializeField] private float baseBasicAttackCoolDown;
    private float remainBasicAttackCoolDown;
    [SerializeField] private float baseMeteoCoolDown;
    private float remainMeteoCoolDown;

    public override void Initialize()
    {
        boss = GetComponent<Boss_a>();
        if (!env)
            Debug.LogError("Agent의 소속 환경을 찾을 수 없습니다.");
        if (!boss)
            Debug.LogError("Agnet가 제어할 Boss_a를 찾을 수 없습니다.");
    }

    public override void OnEpisodeBegin()
    {
        boss.Reset();
        env.Reset();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        for(int i = 0; i < 8; i++)
        {
            if(i < env.players.Length && env.players[i] != null)
            {
                sensor.AddObservation(env.players[i].transform.position.x);
                sensor.AddObservation(env.players[i].transform.position.z);
            }
            else
            {
                sensor.AddObservation(Vector3.zero.x); // 빈 부분에는 0으로 채운다.
                sensor.AddObservation(Vector3.zero.z);
            }
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Discrete Actions
        // branch 0 size : 3
        // branch 1 size : 9
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
                List<Vector3> strikePos = new List<Vector3>();
                for(int i  = 0; i < 5; i++)
                    strikePos.Add(new Vector3(actions.ContinuousActions[i * 2], 0f, actions.ContinuousActions[i * 2 + 1]));
                boss.m_meteo.LaunchSkill(strikePos.ToArray());
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
        actionMask.SetActionEnabled(1, 0, !basicAttackReady);
        for (int i = 1; i < 9; i++)
        {
            // 살아있는 player만 타겟으로 지정 가능
            actionMask.SetActionEnabled(1, i, basicAttackReady && i <= env.numPlayers && env.players[i-1].isAlive);
        }

        actionMask.SetActionEnabled(0, 2, remainMeteoCoolDown <= 0f);
    }

    public void Dead()
    {
        AddReward(-2.0f); // 보스가 죽을 경우 패널티를 줍니다.
        EndEpisode(); // 보스가 죽을 경우 에피소드를 종료합니다.
    }
}