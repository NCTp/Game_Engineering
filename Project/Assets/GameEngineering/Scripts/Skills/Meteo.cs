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

    public void LaunchSkill(Vector3[] strikingPos)
    {
        foreach(Vector3 pos in strikingPos)
        {
            Vector3 finalPos = new Vector3(pos.x, height, pos.z);
            MeteoInstance instance = Instantiate(meteoPrefab, finalPos, Quaternion.Euler(0f, 0f, 0f)).GetComponentInChildren<MeteoInstance>();
            instance.Initialize(preDelay, damage, speed);
        }
    }
}
