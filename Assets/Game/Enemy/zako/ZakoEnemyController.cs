using UnityEngine;

public class ZakoEnemyController : EnemyControllerBase
{
    [SerializeField] float chargeDuration = 0.6f;
    [SerializeField] float attackDuration = 0.3f;
    [SerializeField] float lungeSpeed = 6f;
    [SerializeField] float lungeDrag = 12f;
    [SerializeField] GameObject attackInstance;

    Vector2 lungeVelocity;

    protected override void UpdateCharging()
    {
        if (stateTimer <= 0f)
            ChangeState(State.attacking);
    }

    protected override void UpdateAttacking()
    {
        lungeVelocity = Vector2.Lerp(lungeVelocity, Vector2.zero, lungeDrag * Time.deltaTime);
        transform.Translate(lungeVelocity * Time.deltaTime);

        if (stateTimer <= 0f)
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
                    DMGObj dmgObj = Instantiate(
                        attackInstance,
                        transform.position + animator.transform.up * -1f,
                        animator.transform.rotation
                    ).GetComponent<DMGObj>();
                    Destroy(dmgObj.gameObject, 0.5f);
                    if (dmgObj != null)
                        dmgObj.owner = gameObject;
                }
                if (target != null)
                {
                    lungeVelocity = ((Vector2)target.position - (Vector2)transform.position).normalized * lungeSpeed;
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
