using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAttack : MonoBehaviour
{
    [SerializeField] Boss boss;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform shootPos;
    [SerializeField] float damage;
    [SerializeField] float bulletSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LaunchSkill(GameObject target)
    {
        boss.transform.LookAt(target.transform);
        BSProjectile projectile = Instantiate(projectilePrefab, shootPos.position, Quaternion.identity).GetComponent<BSProjectile>();
        projectile.Initialize(target, damage, bulletSpeed);
    }
}
