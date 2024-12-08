using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteo : MonoBehaviour
{
    [SerializeField] GameObject meteoPrefab;
    [SerializeField] float height; // floor 높이에 맞춰 스킬 오브젝트 생성

    void LaunchSkill(Vector2[] strikingPos)
    {
        int count = strikingPos.Length;
        foreach(Vector2 pos2 in strikingPos)
        {
            Vector3 pos3 = (Vector3)pos2 + new Vector3(0f, 0f, height);
            MeteoInstance instance = Instantiate(meteoPrefab, pos3, Quaternion.Euler(0f, 0f, 0f)).GetComponent<MeteoInstance>();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            // Damage Logic
            Destroy(gameObject);
        }
        else if (other.CompareTag("Floor"))
            Destroy(gameObject);
    }
}
