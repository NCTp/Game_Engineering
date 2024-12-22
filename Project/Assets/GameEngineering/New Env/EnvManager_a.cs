using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using System;

public class EnvManager_a : MonoBehaviour
{
    static public int MAX_PLAYER = 4;

    private EnvironmentParameters m_ResetParams;
    public int numPlayers;
    public GameObject playerPrefab;
    public BossAgent_a bossAgent;
    public Boss_a boss;
    public Player_a[] players;
    public Transform[] spawnPoints; // Assign using Unity Inspector

    void Awake()
    {
        m_ResetParams = Academy.Instance.EnvironmentParameters;
        players = new Player_a[MAX_PLAYER];
    }

    /// <summary>
    /// numPlayer 받아와 player 새로 생성 및 player와 boss 초기화 
    /// </summary>
    public void ResetEntireEnv()
    {
        numPlayers = Mathf.RoundToInt(m_ResetParams.GetWithDefault("num_of_players",1));
        foreach(Player_a p in players)
        {
            if(p != null)
                Destroy(p.gameObject);
        } 
        Array.Resize(ref players, numPlayers);
        for (int i = 0; i < numPlayers; i++)
        {
            players[i] = Instantiate(playerPrefab, spawnPoints[i % 4].position, Quaternion.identity, transform).GetComponent<Player_a>();
            players[i].boss = this.boss.transform;
            players[i].ResetPlayer();
        }
        boss.ResetBoss(numPlayers);
    }

}
