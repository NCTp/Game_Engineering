using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public float maxHealth;
    public float CurrentHealth{get; private set;}

    private int totalPlayers = 8;//임의값 현재 플레이어수

    public float BasicAttackInterval = 1f; //기본공격 딜레이 1초 설정(임의)
    private float currentAttackInterval;

    public GameObject zonePrefab;
    public Transform zoneSpawnPoint;//구역 생성 위치
    public float zoneSpawnDuration = 3f;//구역 제한시간 3초


    private bool isZoneActive = false;
    private bool isZoneTriggered = false;

    bool FirstBossCondition() => !isZoneActive && CurrentHealth <= maxHealth*0.75 && CurrentHealth <= maxHealth*0.5f;
   //첫패턴 발동 보스HP 조건 75~50사이일시.

    // Start is called before the first frame update
    void Start()
    {
        CurrentHealth = maxHealth;
        currentAttackInterval = BasicAttackInterval;
        StartCoroutine(BasicAttackRoutine());   
    }

    // Update is called once per frame
    void Update()
    {
        if (!isZoneTriggered && FirstBossCondition())
        {
            StartCoroutine(FirstPattern());
            isZoneTriggered = true; 
        }
        if (CurrentHealth < maxHealth*0.15f)
        {
            currentAttackInterval = BasicAttackInterval / 2f;
        }
    }

    IEnumerator BasicAttackRoutine()
    {
        while(CurrentHealth > 0)
        {
            //기본 공격
            yield return new WaitForSeconds(currentAttackInterval);
        }
    }

    IEnumerator FirstPattern()
    {
        isZoneActive = true;

        int reqPlayer = totalPlayers;
        List<int> zoneNumbers = HowManyZones(reqPlayer);

        List<GameObject> zones = new List<GameObject>();
        foreach (int zoneNumber in zoneNumbers)
        {
            Vector3 randomPosition = zoneSpawnPoint.position + new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f)); // 구역을 랜덤 위치에 생성
            GameObject zone = Instantiate(zonePrefab, randomPosition, Quaternion.identity);
            ZoneController zoneController = zone.GetComponent<ZoneController>();
            if (zoneController != null)
            {
                zoneController.SetZoneNumber(zoneNumber);
            }
            zones.Add(zone);
            
        }

        yield return new WaitForSeconds(zoneSpawnDuration);

        foreach (GameObject zone in zones)
        {
            int playersInZone = CountPlayersInZone(zone);
            ZoneController zoneController = zone.GetComponent<ZoneController>();
            if (zoneController != null && playersInZone < zoneController.RequiredPlayers)
            {
               
                DealDamageToPlayersOutsideZone(zone);
                DealDamageToPlayersInZone(zone);
            }
        }
        foreach (GameObject zone in zones)
        {
            Destroy(zone);
        }

        isZoneActive = false;
    }

    List <int> HowManyZones(int totalPlayers)
    {
        List<int> zones = new List<int>();
        while (totalPlayers > 0)
        {
            int ReqPlayers = Random.Range(1, totalPlayers+1);
            zones.Add(ReqPlayers);
            totalPlayers -= ReqPlayers;
        }
        return zones;
    }

    int CountPlayersInZone(GameObject zone)
    {
       
        Collider[] colliders = Physics.OverlapSphere(zone.transform.position, 5f); //구역반경 5 임의설정
        int playerCount = 0;

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Player")) //Player태그 확인
            {
                playerCount++;
            }
        }

        return playerCount;
    }

    void DealDamageToPlayersOutsideZone(GameObject zone) // 구역 외부의 플레이어에게 데미지
    {
    
        Collider[] colliders = Physics.OverlapSphere(transform.position, 50f); // 전체 반경 50 내 탐지

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                float distanceToZone = Vector3.Distance(collider.transform.position, zone.transform.position);

                if (distanceToZone > 5f) //구역밖인지 확인
                {
                    PlayerHealth playerHealth = collider.GetComponent<PlayerHealth>();
                    if (playerHealth != null)
                    {
                        playerHealth.TakeDamage(200f); //즉사
      
                    }
                }
            }
        }
    }

    void DealDamageToPlayersInZone(GameObject zone)// 구역 내부의 플레이어에게 데미지
    {
        
        Collider[] colliders = Physics.OverlapSphere(zone.transform.position, 5f); // 구역 반경 5로 설정

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                PlayerHealth playerHealth = collider.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(200f);
                }
            }
        }
    }


}
