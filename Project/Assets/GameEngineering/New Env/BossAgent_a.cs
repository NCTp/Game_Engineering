using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
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
}
