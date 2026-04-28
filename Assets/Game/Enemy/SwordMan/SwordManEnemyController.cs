using UnityEngine;

public class SwordManEnemyController : EnemyControllerBase
{
    [SerializeField] float chargeDuration = 0.8f;
    [SerializeField] float chargeRotationSpeed = 90f;
    [SerializeField] float reaimDuration = 0.25f;
    [SerializeField] float reaimRotationSpeed = 720f;
    [SerializeField] float lungeSpeed = 10f;
    [SerializeField] float minLungeSpeed = 3f;
    [SerializeField] float afterAttackDuration = 0.5f;
    [SerializeField] Animator swordAnimator;
    [SerializeField] GameObject dmgObjPrefab;
    [SerializeField] float dmgObjLifetime = 0.2f;

    // attacking サブフェーズ: 0=1撃目突進, 1=再狙い, 2=2撃目突進, 3=余韻
    int attackPhase;
    Vector2 lungeVelocity;
    float lungeDistanceTraveled;

    protected override void UpdateCharging()
    {
        if (target != null)
        {
            Vector2 dir = ((Vector2)target.position - (Vector2)transform.position).normalized;
            animator.transform.rotation = Quaternion.RotateTowards(
                animator.transform.rotation,
                FacingRotation(dir),
                chargeRotationSpeed * Time.deltaTime);
        }
        if (stateTimer <= 0f)
            ChangeState(State.attacking);
    }

    protected override void UpdateAttacking()
    {
        switch (attackPhase)
        {
            case 0: // 1撃目突進
            case 2: // 2撃目突進
                Vector2 delta = lungeVelocity * Time.deltaTime;
                lungeDistanceTraveled += delta.magnitude;
                transform.Translate(delta);
                if (lungeDistanceTraveled >= attackDistance)
                {
                    SwordSwing();
                    if (attackPhase == 0) { attackPhase = 1; stateTimer = reaimDuration; }
                    else { attackPhase = 3; stateTimer = afterAttackDuration; }
                }
                break;

            case 1: // 再狙い（素早くangle追尾、停止）
                if (target != null)
                {
                    Vector2 dir = ((Vector2)target.position - (Vector2)transform.position).normalized;
                    animator.transform.rotation = Quaternion.RotateTowards(
                        animator.transform.rotation,
                        FacingRotation(dir),
                        reaimRotationSpeed * Time.deltaTime);
                }
                if (stateTimer <= 0f) { attackPhase = 2; BeginLunge(); }
                break;

            case 3: // 余韻
                if (stateTimer <= 0f)
                    ChangeState(State.following);
                break;
        }
    }

    protected override void ChangeState(State next)
    {
        currentState = next;
        switch (next)
        {
            case State.charging:
                stateTimer = chargeDuration;
                break;

            case State.attacking:
                attackPhase = 0;
                BeginLunge();
                break;

            case State.damaging:
                stateTimer = damageDuration;
                break;

            case State.deathing:
                EnterDying();
                break;
        }
    }

    void BeginLunge()
    {
        lungeDistanceTraveled = 0f;
        float dist = target != null ? Vector2.Distance(transform.position, target.position) : attackDistance;
        float speed = Mathf.Clamp(lungeSpeed * (dist / attackDistance), minLungeSpeed, lungeSpeed);
        lungeVelocity = -(Vector2)animator.transform.up * speed;
    }

    void SwordSwing()
    {
        swordAnimator?.SetTrigger("Attack");
        if (dmgObjPrefab == null) return;
        var go = Instantiate(
            dmgObjPrefab,
            transform.position + animator.transform.up * -1f,
            animator.transform.rotation);
        var dmg = go.GetComponent<DMGObj>();
        if (dmg != null)
            dmg.owner = gameObject;
        Destroy(go, dmgObjLifetime);
    }
}
