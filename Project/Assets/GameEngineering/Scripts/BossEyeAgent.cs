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
    private const float MAX_EPISODE_TIME = 3f;
    public override void Initialize()
    {
        m_BossEyeRb = GetComponent<Rigidbody>();
        m_PlayerRb = player.GetComponent<Rigidbody>();
        m_ResetParams = Academy.Instance.EnvironmentParameters;
        SetResetParameters();
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        // 벡터 센서를 통한 관측 데이터의 수 9개
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
            // Player와 BossEye의 거리 관측 (3개)
            sensor.AddObservation((m_PlayerRb.position - m_BossEyeRb.position).magnitude);
        }
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        m_EpisodeTimer += Time.deltaTime;
        //Debug.Log("에피소드 타임 : " + m_EpisodeTimer);
        if(m_EpisodeTimer >= MAX_EPISODE_TIME)
        {
            AddReward(-1f);
            EndEpisode();
        }
    }

    public override void OnEpisodeBegin()
    {
        // BossEye의 회전량 초기화 및 랜덤 회전 추가
        gameObject.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        gameObject.transform.Rotate(new Vector3(1, 0, 0), Random.Range(-10f, 10f));
        gameObject.transform.Rotate(new Vector3(0, 0, 1), Random.Range(-10f, 10f));
        // Player의 위치 초기화
        player.transform.position = new Vector3(Random.Range(-3.0f, 3.0f), 0.0f, Random.Range(-3.0f, 3.0f)); 
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
