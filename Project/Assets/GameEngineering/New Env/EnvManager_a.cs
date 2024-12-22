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
    public GameObject go_player;
    public BossAgent_a bossAgent;
    public Boss_a boss;
    public Player_a[] players;
    public Transform[] spawnPoints; // Assign using Unity Inspector

    void Awake()
    {
        m_ResetParams = Academy.Instance.EnvironmentParameters;
        players = new Player_a[MAX_PLAYER];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetEntireEnv()
    {
        boss.Reset();
        numPlayers = Mathf.RoundToInt(m_ResetParams.GetWithDefault("num_of_players",1));
        foreach(Player_a p in players)
        {
            if(p != null)
                Destroy(p.gameObject);
        } 
        Array.Resize(ref players, numPlayers);
        for (int i = 0; i < numPlayers; i++)
        {
            players[i] = Instantiate(go_player, spawnPoints[i % 4].position, Quaternion.identity).GetComponent<Player_a>();
            players[i].Reset();
        }
    }

}
