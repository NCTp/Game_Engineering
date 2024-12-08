using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteo : MonoBehaviour
{
    [SerializeField] GameObject meteoPrefab;
    [SerializeField] float height; // floor 높이에 맞춰 스킬 오브젝트 생성
    [SerializeField] float preDelay = 0.5f;
    [SerializeField] float damage = 0f;
    [SerializeField] float speed = 30f;

    public void LaunchSkill(Vector2[] strikingPos)
    {
        foreach(Vector2 pos2 in strikingPos)
        {
            Vector3 pos3 = (Vector3)pos2 + new Vector3(0f, 0f, height);
            MeteoInstance instance = Instantiate(meteoPrefab, pos3, Quaternion.Euler(0f, 0f, 0f)).GetComponentInChildren<MeteoInstance>();
            instance.Initialize(preDelay, damage, speed);
        }
    }
}
