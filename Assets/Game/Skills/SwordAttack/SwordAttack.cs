using System.Collections;
using System.Transactions;
using UnityEngine;
public class SwordAttack : PlayerSkillBase
{
    [SerializeField] GameObject swordPrefab;
    [SerializeField] GameObject dmgOBJPrefab;
    private GameObject sword;
    private Animator swordAnimator;
    public override void ActivateSkill(PlayerController player)
    {
        player.moveSpeed *= 0.5f;

        if (sword != null) return;
        sword = Instantiate(swordPrefab, player.transform.position, player.anker.rotation, player.anker);
        swordAnimator = sword.GetComponent<Animator>();
    }
    [SerializeField] float dashSpeed = 10f;
    [SerializeField] float dashDuration = 1f;
    private IEnumerator Dash(PlayerController player)
    {
        Vector3 dir = Quaternion.Euler(0f, 0f, player.anker.eulerAngles.z) * Vector3.up;
        float elapsed = 0f;

        Vector3 spawnPos = player.transform.position + dir * dashDuration * dashSpeed * 0.5f;
        GameObject dmgOBJ = Instantiate(dmgOBJPrefab, spawnPos, player.anker.rotation);
        Destroy(dmgOBJ, dashDuration);
        while (elapsed < dashDuration)
        {
            player.transform.Translate(dir * dashSpeed * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        Destroy(sword, 0.5f);
        player.moveSpeed = player.moveSpeed / 0.5f;
    }
    public override void ExecuteSkill(PlayerController player)
    {
        swordAnimator.SetTrigger("Execute");
        StartCoroutine(Dash(player));
    }
}