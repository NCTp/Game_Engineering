using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteo : MonoBehaviour
{
    [SerializeField] GameObject meteoPrefab; // 낙하시킬 오브젝트 프리팹
    [SerializeField] float height; // floor 높이에 맞춰 스킬 오브젝트 생성시키기 위한 y 좌표값
    [SerializeField] float preDelay = 0.5f;
    [SerializeField] float damage = 0f;
    [SerializeField] float speed = 30f;

    private BossAgent_a agent;

    private void Awake()
    {
        agent = GetComponentInParent<BossAgent_a>();
    }

    public void LaunchSkill(Vector3[] strikingPos)
    {
        StartCoroutine(ManipAttackFlag());

        foreach (Vector3 pos in strikingPos)
        {
            Vector3 finalPos = new Vector3(pos.x + agent.transform.position.x, height, pos.z + agent.transform.position.z);
            MeteoInstance instance = Instantiate(meteoPrefab, finalPos, Quaternion.Euler(0f, 0f, 0f)).GetComponentInChildren<MeteoInstance>();
            instance.Initialize(agent, preDelay, damage, speed);
        }
    }

    IEnumerator ManipAttackFlag()
    {
        agent.isAttacking = true;
        yield return new WaitForSeconds(0.75f);
        agent.isAttacking = false;
    }
}
