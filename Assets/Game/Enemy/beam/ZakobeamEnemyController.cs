using UnityEngine;

public class ZakobeamEnemyController : EnemyControllerBase
{
    [SerializeField] float chargeDuration = 0.6f;
    [SerializeField] float attackDuration = 0.3f;
    [SerializeField] GameObject attackInstance;

    bool isEscaping;

    protected override void UpdateFollowing()
    {
        if (target == null) return;

        if (!isEscaping)
        {
            base.UpdateFollowing();
            return;
        }

        Vector2 dir = ((Vector2)transform.position - (Vector2)target.position).normalized;
        animator.transform.rotation = Quaternion.LookRotation(Vector3.forward, dir);
        transform.Translate(dir * moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target.position) >= attackDistance)
        {
            isEscaping = false;
            ChangeState(State.charging);
        }
    }

    protected override void UpdateCharging()
    {
        if (stateTimer <= 0f)
            ChangeState(State.attacking);
    }

    protected override void UpdateAttacking()
    {
        if (stateTimer <= 0f)
        {
            animator.Play("Attack");
            UpdateEscapeFlagAndFollow();
        }
    }

    protected override void UpdateDamaging()
    {
        if (stateTimer <= 0f)
            UpdateEscapeFlagAndFollow();
    }

    void UpdateEscapeFlagAndFollow()
    {
        float dist = target != null
            ? Vector2.Distance(transform.position, target.position)
            : float.MaxValue;
        isEscaping = dist <= attackDistance;
        ChangeState(State.following);
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
                stateTimer = attackDuration;
                if (attackInstance != null)
                {
                    Vector3 spawnPos = transform.position;
                    float angle = animator.transform.eulerAngles.z - 90;
                    Quaternion spawnRot = Quaternion.Euler(0, 0, angle);
                    Destroy(Instantiate(attackInstance, spawnPos, spawnRot), 1.2f);
                }
                break;

            case State.damaging:
                stateTimer = damageDuration;
                break;

            case State.deathing:
                EnterDying();
                break;
        }
    }
}
