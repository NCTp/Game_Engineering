using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;

public class BossEyeAgent : Agent
{

    [Header("Agent 세팅")]
    public bool useVectorObs;
    private Rigidbody m_BossEyeRb;
    private EnvironmentParameters m_ResetParams;
    [SerializeField] private GameObject[] player;
    private Rigidbody m_PlayerRb;
    private float m_EpisodeTimer;
    public float MAX_EPISODE_TIME = 5f;

    [Header("DropMissile 스킬 세팅")]
    public float dropMissileTime = 1.0f;
    public float explosionRadius = 3.0f;

    private Vector3 m_GizmoPosition;
    private bool m_ShowGizmo;
    private float m_GizmoDuration = 1f;
    private float m_GizmoTimer;




    public override void Initialize()
    {
        if (player == null)
        {
            Debug.LogError("Player 오브젝트가 할당되지 않았습니다!");
            return;
        }

        m_BossEyeRb = GetComponent<Rigidbody>();
        if (m_BossEyeRb == null)
        {
            Debug.LogError("Rigidbody 컴포넌트를 찾을 수 없습니다!");
            return;
        }

        m_PlayerRb = player[0].GetComponent<Rigidbody>();
        if (m_PlayerRb == null)
        {
            Debug.LogError("Player의 Rigidbody를 찾을 수 없습니다!");
            return;
        }

        m_ResetParams = Academy.Instance.EnvironmentParameters;
        SetResetParameters();
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        // 벡터 센서를 통한 관측 데이터의 수 7개
        if(useVectorObs)
        {
            // BossEye의 회전량 관측 (3개)
            sensor.AddObservation(gameObject.transform.rotation.x);
            sensor.AddObservation(gameObject.transform.rotation.y);
            sensor.AddObservation(gameObject.transform.rotation.z);
            // Player의 위치 관측 (3개)
            sensor.AddObservation(m_PlayerRb.velocity.normalized.x);
            sensor.AddObservation(m_PlayerRb.velocity.normalized.y);
            sensor.AddObservation(m_PlayerRb.velocity.normalized.z);
            // Player와 BossEye의 거리 관측 (1개)
            sensor.AddObservation((m_PlayerRb.position - m_BossEyeRb.position).magnitude);
        }
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        m_EpisodeTimer += Time.deltaTime;
        //Debug.Log("에피소드 타임 : " + m_EpisodeTimer);
        if(m_EpisodeTimer >= MAX_EPISODE_TIME)
        {
            SetReward(-1f);
            EndEpisode();
        }
        else
        {
            // 연속행동값 받기.
            var continuousActions = actions.ContinuousActions;
        }
    }

    public override void OnEpisodeBegin()
    {
        if (player == null || !gameObject)
        {
            Debug.LogError("필수 오브젝트가 없습니다!");
            return;
        }
        // BossEye의 회전량 초기화 및 랜덤 회전 추가
        gameObject.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        gameObject.transform.localPosition = new Vector3(0f, 1.5f, 0f);
        gameObject.transform.Rotate(new Vector3(1, 0, 0), Random.Range(-10f, 10f));
        gameObject.transform.Rotate(new Vector3(0, 1, 0), Random.Range(-10f, 10f));
        gameObject.transform.Rotate(new Vector3(0, 0, 1), Random.Range(-10f, 10f));
        // Player의 위치 초기화
        player[0].transform.localPosition = new Vector3(Random.Range(-5.0f, 3.0f), 0.0f, Random.Range(-5.0f, 3.0f));
        //m_PlayerRb.velocity = new Vector3(Random.Range(-3.0f, 3.0f), 0.0f, Random.Range(-3.0f, 3.0f));
        SetResetParameters();
        Debug.Log("에피소드 시작!!");
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {

    }

    public void SetResetParameters()
    {
        m_EpisodeTimer = 0f;
        player[0].GetComponent<Player>().ResetParameters();
    }

    /////////////////////////// 보스의 액션 구현부 //////////////////////////////

    // DropMissile(Vector3 dropPos)
    // 특정 지점에 1초 뒤 미사일을 투하합니다.
    private void DropMissile(Vector3 dropPos)
    {
        Debug.Log("Drop Missile Ready");

        // Gizmo 표시를 위한 설정 파트
        m_GizmoPosition = dropPos;
        m_GizmoDuration = dropMissileTime;
        m_ShowGizmo = true;
        m_GizmoTimer = m_GizmoDuration;

        StartCoroutine(DropMissileCoroutine(dropPos));
    }

    IEnumerator DropMissileCoroutine(Vector3 dropPos)
    {
        yield return new WaitForSeconds(dropMissileTime);
        m_ShowGizmo = false;

        Debug.Log("Missile Drop");

        Collider[] hitColliders = Physics.OverlapSphere(dropPos, explosionRadius);

        foreach(var hitCollider in hitColliders)
        {
            if(hitCollider.CompareTag("Player"))
            {
                Debug.Log("Player Missile Hit!");
                hitCollider.GetComponent<Player>()?.TakeDamage(110.0f); // 폭발 적중시 바로 플레이어 사망
                AddReward(2.0f);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if(m_ShowGizmo)
        {
            Gizmos.color = new Color(1,0,0,0.5f);
            Gizmos.DrawWireSphere(m_GizmoPosition, explosionRadius);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            DropMissile(new Vector3(4.0f, 0.0f, 4.0f));
        }

    }

}
