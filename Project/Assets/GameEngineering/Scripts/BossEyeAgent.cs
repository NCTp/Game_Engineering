using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;

public class BossEyeAgent : Agent
{
    public bool useVectorObs;
    private Rigidbody m_BossEyeRb;
    private EnvironmentParameters m_ResetParams;
    [SerializeField] private GameObject player;
    private Rigidbody m_PlayerRb;
    private float m_EpisodeTimer;
    private const float MAX_EPISODE_TIME = 5f;
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

        m_PlayerRb = player.GetComponent<Rigidbody>();
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
            // X, Z 축 회전.
            transform.Rotate(Vector3.right, continuousActions[0] * 360f * Time.deltaTime);
            transform.Rotate(Vector3.forward, continuousActions[1] * 360f * Time.deltaTime);
            // 플레이어를 바라보는지 확인하기.
            Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
            float dotProduct = Vector3.Dot(transform.forward, directionToPlayer);
            SetReward(dotProduct * 0.01f);
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
        player.transform.localPosition = new Vector3(Random.Range(-5.0f, 3.0f), 0.0f, Random.Range(-5.0f, 3.0f));
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
    }

    // Start is called before the first frame update
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {

    }
}
