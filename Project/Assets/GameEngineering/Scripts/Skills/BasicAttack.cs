using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAttack : MonoBehaviour
{
    Boss_a boss;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform shootPos;
    [SerializeField] float damage;
    [SerializeField] float bulletSpeed;

    void Awake()
    {
        boss = transform.GetComponentInParent<Boss_a>();
    }
    public void LaunchSkill(GameObject target)
    {
        //boss.transform.LookAt(target.transform);
        BSProjectile projectile = Instantiate(projectilePrefab, shootPos.position, Quaternion.identity).GetComponent<BSProjectile>();
        projectile.Initialize(target, damage, bulletSpeed);
    }
}
