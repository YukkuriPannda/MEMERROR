using System.Transactions;
using UnityEngine;
public class RapidFireNormalBullet : PlayerSkillBase
{
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Vector2 firePoint;

    public override void Skill(PlayerController player)
    {
        Debug.Log("RapidFireNormalBullet");
        Instantiate(bulletPrefab, new Vector3(firePoint.x, firePoint.y, 0) + transform.position, player.anker.rotation);
    }
}