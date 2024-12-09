using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class BossEyeEnv : MonoBehaviour
{
    public GameObject playerPrefab;
    private int numPlayers;

    private EnvironmentParameters m_EnvironmentParameters;
    // Start is called before the first frame update
    void Start()
    {
        m_EnvironmentParameters = Academy.Instance.EnvironmentParameters;

        UpdatePlayerCount();

    }

    // Update is called once per frame
    void Update()
    {
        // 필요 시 매 프레임마다 값을 업데이트할 수 있습니다.
        //int updatedNumPlayers = Mathf.RoundToInt(m_EnvironmentParameters.GetWithDefault("num_players", 1));
        //if (updatedNumPlayers != numPlayers)
        //{
            //numPlayers = updatedNumPlayers;
            //AdjustPlayers();
        //}
    }

    private void UpdatePlayerCount()
    {
        // 환경에서 초기 num_players 값을 읽어오고, 레슨에 따라 업데이트
        numPlayers = Mathf.RoundToInt(m_EnvironmentParameters.GetWithDefault("num_players", 1));
        AdjustPlayers();
    }

    private void AdjustPlayers()
    {
        Debug.Log($"Adjusting to {numPlayers} players");

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < numPlayers; i++)
        {
            // 플레이어를 스폰하고 위치와 이동 방향을 랜덤하게 정해줍니다.
            GameObject player = Instantiate(playerPrefab, transform);
            player.transform.localPosition = new Vector3(
                Random.Range(-5.0f, 3.0f),
                0.0f,
                Random.Range(-5.0f, 3.0f));
            player.GetComponent<Rigidbody>().velocity = new Vector3(
                Random.Range(-3.0f, 3.0f),
                0.0f,
                Random.Range(-3.0f, 3.0f));
        }
    }
}
