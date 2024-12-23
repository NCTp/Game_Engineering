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

    public float remainTime;
    public bool gameOver;


    void Awake()
    {
        m_ResetParams = Academy.Instance.EnvironmentParameters;
        players = new Player_a[MAX_PLAYER];
    }

    private void Update()
    {
        remainTime -= Time.deltaTime;

        bool allPlayerDead = true;
        foreach (Player_a p in players)
        {
            if (p.isAlive)
            {
                allPlayerDead = false;
                break;
            }
        }
        if (allPlayerDead)
            GameOver(true);
        else if (remainTime <= 0f)
            GameOver(false);
    }

    /// <summary>
    /// numPlayer 받아와 player 새로 생성 및 player와 boss 초기화 
    /// </summary>
    public void ResetEntireEnv()
    {
        numPlayers = Mathf.RoundToInt(m_ResetParams.GetWithDefault("num_of_players",1));
        remainTime = numPlayers * 60f;
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

    public void GameOver(bool win)
    {
        if (win)
        {
            bossAgent.AddReward(remainTime / (numPlayers * 60f));
            bossAgent.AddReward(2f);
            Debug.Log(name + ": Agent WIN with Reward value " + boss.agent.GetCumulativeReward());
        }
        else
        {
            bossAgent.AddReward(-remainTime / (numPlayers * 60f));
            int alivePlayers = 0;
            float totalHealth = 0f;
            foreach(Player_a p in players)
            {
                if (p.isAlive)
                {
                    alivePlayers++;
                    if (p.health > 0)
                        totalHealth += p.health;
                }
            }
            bossAgent.AddReward(-alivePlayers / numPlayers);
            bossAgent.AddReward(-totalHealth / (numPlayers * 100f));
            bossAgent.AddReward(-2f);
            Debug.Log(name + ": Agent LOSE with Reward value " + boss.agent.GetCumulativeReward());
        }
        bossAgent.EndEpisode();
    }
}
