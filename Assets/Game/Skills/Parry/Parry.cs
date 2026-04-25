using System.Collections;
using UnityEngine;
public class Parry : PlayerSkillBase
{
  public GameObject parryEffectPrefab;
  public override void ActivateSkill(PlayerController player)
  {
    var effect = Instantiate(parryEffectPrefab, player.transform.position, player.anker.rotation, player.anker.transform);
    Destroy(effect, 1f);
    player.GetComponent<HPController>().parry = true;
    player.moveSpeed *= 0.2f;
    StartCoroutine(ParryDuration(player, 0.5f * Time.timeScale));
  }
  public IEnumerator ParryDuration(PlayerController player, float duration)
  {
    yield return new WaitForSeconds(duration);
    player.GetComponent<HPController>().parry = false;
    player.moveSpeed /= 0.2f;
  }
}