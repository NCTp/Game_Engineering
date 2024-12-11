using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using NUnit.Framework.Constraints;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class BossEyeAgent : Agent
{

    [Header("BossEye 파라미터")]
    public float maxHealth = 100.0f;
    public float currentHealth {get; private set;}

    public float rotationSpeed = 1.0f;

    public GameObject targetPosIndicator;
    //public float rayDistance = 10.0f;

    [Header("Agent 세팅")]
    public bool useVectorObs;
    public float arenaSize = 10.0f; // 보스 행동 범위 제한
    private Rigidbody m_BossEyeRb;
    private EnvironmentParameters m_ResetParams;
    public float attackCoolDown = 10.0f;
    private float m_AttackTimer = 0.0f;

    [Header("Player 관련 파라미터")]
    public GameObject playerPrefab; // 후에 플레이어 스폰을 위한 플레이어 프리팹
    [SerializeField] private GameObject[] players;
    private Rigidbody[] m_PlayerRbs;
    private float m_EpisodeTimer;
    public float MAX_EPISODE_TIME = 5f;
    public int numPlayers = 1;
    public Transform[] spawnPoints;

    [Header("DropMissile 스킬 세팅")]
    public float missileDamage = 50.0f;
    public float dropMissileTime = 0.5f;
    public float explosionRadius = 5.0f;

    private Vector3 m_GizmoPosition;
    private bool m_ShowGizmo;
    private float m_GizmoDuration = 1f;
    private float m_GizmoTimer;

    private Vector3 missileDropPos;

    [Header("디버깅 설정")]
    private LineRenderer m_LineRenderer;

    //public bool showLine = true;
    public float rayDistance = 10f; // Ray의 길이
    public float rayWidth = 0.1f; // Ray의 두께
    public Color rayColor = Color.red; // Ray의 색상

    //private const int a_DoNothing = 0;
    //private const int a_RotRight = 1;
    //private const int a_RotLeft = 2;
    //private const int a_RotDown = 3;
    //private const int a_RotUp = 4;

    private Vector3 targetPos = new Vector3(0f, 0f, 0f);

    private const int a_DoNothing = 0;
    private const int a_DropMissile = 1;


    void SetUpPlayerInfos()
    {
        m_PlayerRbs = new Rigidbody[players.Length];
        for(int i = 0; i < players.Length; i++)
        {
            if (players[i] != null)
            {
                // player들의 파라미터 리셋
                players[i].GetComponent<Player>().ResetParameters();
                // player들의 리지드바디 불러오기.
                m_PlayerRbs[i] = players[i].GetComponent<Rigidbody>();
                if (m_PlayerRbs[i] == null)
                {
                    Debug.LogError("Player의 Rigidbody를 찾을 수 없습니다!");
                    return;
                }
            }
        }
    }
    public override void Initialize()
    {

        m_BossEyeRb = GetComponent<Rigidbody>();
        m_LineRenderer = GetComponent<LineRenderer>();
        m_LineRenderer.startWidth = rayWidth; // 시작 두께
        m_LineRenderer.endWidth = rayWidth;   // 끝 두께
        m_LineRenderer.material = new Material(Shader.Find("Sprites/Default")); // 단순한 Shader
        m_LineRenderer.startColor = rayColor;
        m_LineRenderer.endColor = rayColor;

        if (m_BossEyeRb == null)
        {
            Debug.LogError("Rigidbody 컴포넌트를 찾을 수 없습니다!");
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
            sensor.AddObservation(targetPos.normalized); // 1

            // Player의 위치 관측 (3 * 4 = 12개)
            // 너무 큰 Space Size가 되므로 y좌표는 관측하지 않음.
            /*
            for(int i = 0; i < 4; i++)
            {
                if(i < players.Length && players[i] != null)
                {
                    sensor.AddObservation(players[i].transform.position.x);
                    sensor.AddObservation(players[i].transform.position.z);
                    sensor.AddObservation(Vector3.Distance(targetPos, players[i].transform.position));
                }
                else
                {
                    sensor.AddObservation(Vector3.zero.x); // 빈 부분에는 0으로 채운다.
                    sensor.AddObservation(Vector3.zero.z);
                    sensor.AddObservation(0.0f);
                }
            }
            */
        }
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        // 수정 필요
        // 결과 수렴까지 오랜 시간이 필요함.
        m_EpisodeTimer += Time.deltaTime;
        m_AttackTimer += Time.deltaTime;
        //Debug.Log("에피소드 타임 : " + m_EpisodeTimer);

        if(m_EpisodeTimer >= MAX_EPISODE_TIME)
        {
            AddReward(-1f);
            EndEpisode();
        }
        else
        {
            // 플레이어의 위치 추정 -> Continuous Actions
            var xPos = actions.ContinuousActions[0];
            var zPos = actions.ContinuousActions[1];

            targetPos += new Vector3(xPos, 0f, zPos);
            //Debug.Log("Target Position : " + targetPos);
            targetPosIndicator.transform.position = targetPos + new Vector3(0f, 3.0f, 0f);

            //positionBounds += transform.position;

            if (Vector3.Distance(targetPos, transform.position) >= 30.0f)
            {
                Debug.Log("에이전트가 공간 밖으로 나감. 에피소드 종료.");
                AddReward(-1.0f); // 공간 밖으로 나가면 -1 보상
                EndEpisode(); // 에피소드 종료
            }

            for (int i = 0; i < players.Length; i++)
            {
                if (players[i] != null)
                {
                    if (Vector3.Distance(targetPos, players[i].transform.position) <= 10.0f)
                    {
                        AddReward(0.01f);
                    }
                    if (Vector3.Distance(targetPos, players[i].transform.position) <= 5.0f)
                    {
                        AddReward(0.05f);
                    }
                    else if (Vector3.Distance(targetPos, players[i].transform.position) <= 1.0f)
                    {
                        AddReward(0.1f);
                    }
                    else
                    {
                        AddReward(-0.01f);
                    }
                }
            }



            /*
            // targetPos가 아레나 밖으로 나갈 경우 에피소드 리셋
            if (targetPos.x >= arenaSize || targetPos.x <= -arenaSize || targetPos.z >= arenaSize ||
                targetPos.z <= arenaSize)
            {
                Debug.Log(" !!!! ");
                AddReward(-0.5f);
                EndEpisode();
            }
            */

            if (m_AttackTimer >= attackCoolDown)
            {
                // 사용할 스킬 선택 -> Discrete Actions
                var discreteActions = actions.DiscreteActions[0];

                switch (discreteActions)
                {
                    case a_DoNothing:
                        Debug.Log("액션 선택 : Do Nothing");
                        break;
                    case a_DropMissile:
                        Debug.Log("액션 선택 : Drop Missile");
                        DropMissile(targetPos);
                        break;
                    default:
                        Debug.Log("설정보다 높은 DiscreteAction 값을 불러왔습니다.");
                        break;

                }

                m_AttackTimer = 0f;
            }
            /*
            switch (rotAction)
            {
                case a_DoNothing:
                    break;
                case a_RotRight:
                    transform.Rotate(Vector3.right * 1.0f);
                    break;
                case a_RotLeft:
                    transform.Rotate(Vector3.left * 1.0f);
                    break;
                case a_RotDown:
                    transform.Rotate(Vector3.down * 1.0f);
                    break;
                case a_RotUp:
                    transform.Rotate(Vector3.up * 1.0f);
                    break;
            }
            //private const int a_DoNothing = 0;
            //private const int a_RotRight = 1;
            //private const int a_RotLeft = 2;
            //private const int a_RotDown = 3;
            //private const int a_RotUp = 4;

            // Ray를 정면으로 발사
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, rayDistance))
            {
                // 충돌한 오브젝트의 태그가 "player"인 경우
                if (hit.collider.CompareTag("Floor"))
                {
                    // 보상 추가
                    AddReward(1f);
                    EndEpisode();
                }
            }
            */

        }
    }

    public override void OnEpisodeBegin()
    {
        if (players == null || !gameObject)
        {
            Debug.LogError("필수 오브젝트가 없습니다!");
            return;
        }
        // BossEye의 회전량 초기화 및 랜덤 회전 추가
        //gameObject.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        //gameObject.transform.localPosition = new Vector3(0f, 1.5f, 0f);
        //gameObject.transform.Rotate(new Vector3(1, 0, 0), Random.Range(-10f, 10f));
        //gameObject.transform.Rotate(new Vector3(0, 1, 0), Random.Range(-10f, 10f));
        //gameObject.transform.Rotate(new Vector3(0, 0, 1), Random.Range(-10f, 10f));
        targetPos = transform.position;
        targetPos.y = 0.0f;
        // Player의 위치 초기화
        SpawnPlayers();
        SetResetParameters();
        Debug.Log("에피소드 시작!!");
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {

    }

    public void SetResetParameters()
    {
        m_EpisodeTimer = 0f;
        numPlayers = Mathf.RoundToInt(m_ResetParams.GetWithDefault("num_of_players",1));
        currentHealth = maxHealth;
        //SetUpPlayerInfos();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        //Debug.Log("BossEye 현재 체력 : " + currentHealth);
        if (currentHealth <= 0)
        {
            AddReward(-2.0f); // 보스가 죽을 경우 패널티를 줍니다.
            EndEpisode(); // 보스가 죽을 경우 에피소드를 종료합니다.
        }
    }

    void SpawnPlayers()
    {
        //Debug.Log("플레이어를 스폰합니다.");
        foreach(GameObject player in players)
        {
            Destroy(player);
        }
        players = new GameObject[numPlayers];
        m_PlayerRbs = new Rigidbody[numPlayers];
        for (int i = 0; i < numPlayers; i++)
        {
            GameObject player = Instantiate(playerPrefab);
            players[i] = player;
            m_PlayerRbs[i] = player.GetComponent<Rigidbody>();
            //player.transform.localPosition = new Vector3(Random.Range(-20.0f, 20.0f), 0.0f, Random.Range(-20.0f, 20.0f));
            int spawnIdx = Random.Range(0,4);
            player.transform.position = spawnPoints[spawnIdx].position;
            players[i].GetComponent<Player>().ResetParameters();
            m_PlayerRbs[i].velocity = new Vector3(Random.Range(-3.0f, 3.0f), 0.0f, Random.Range(-3.0f, 3.0f));
        }
}

    /////////////////////////// 보스의 액션 구현부 //////////////////////////////


    // DropMissile(Vector3 dropPos)
    // 특정 지점에 1초 뒤 미사일을 투하합니다.
    private void DropMissile(Vector3 dropPos)
    {
        //Debug.Log("Drop Missile Ready");

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

        //Debug.Log("Missile Drop");

        Collider[] hitColliders = Physics.OverlapSphere(dropPos, explosionRadius);

        foreach(var hitCollider in hitColliders)
        {
            if(hitCollider.CompareTag("Player"))
            {
                //Debug.Log("Player Missile Hit!");
                hitCollider.GetComponent<Player>()?.TakeDamage(missileDamage); // 폭발 적중시 바로 플레이어 사망
                AddReward(5.0f);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if(m_ShowGizmo)
        {
            /*
            Gizmos.color = new Color(1,0,0,0.5f);
            Gizmos.DrawWireSphere(m_GizmoPosition, explosionRadius);
            */
        }
    }

    void Update()
    {
        // LineRenderer로 Ray 시각화
        Vector3 startPosition = transform.position;
        Vector3 endPosition = transform.position + transform.forward * rayDistance;

        m_LineRenderer.SetPosition(0, startPosition); // 시작점
        m_LineRenderer.SetPosition(1, endPosition);   // 끝점
    }

}
