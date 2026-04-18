using UnityEngine;
public class ZakobeamEnemyController : MonoBehaviour
{
    public enum State
    {
        following,
        escaping,
        charging,
        attacking,
        damaging,
        deathing,
    }

    [SerializeField] float moveSpeed = 2f;
    [SerializeField] float attackDistance = 1.5f;
    [SerializeField] float chargeDuration = 0.6f;
    [SerializeField] float attackDuration = 0.3f;
    [SerializeField] float damageDuration = 0.4f;
    [SerializeField] GameObject attackInstance;
    [SerializeField] Animator animator;

    Transform target;
    HPController hpController;
    [SerializeField] State currentState = State.following;
    float stateTimer = 0f;

    void Awake()
    {
        hpController = GetComponent<HPController>();
        hpController.OnDamaged += OnDamaged;
        hpController.OnDeath += OnDeath;
    }

    void Start()
    {
        var playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            target = playerObj.transform;
    }

    void Update()
    {
        stateTimer -= Time.deltaTime;

        switch (currentState)
        {
            case State.following: UpdateFollowing(); break;
            case State.escaping: UpdateEscaping(); break;
            case State.charging: UpdateCharging(); break;
            case State.attacking: UpdateAttacking(); break;
            case State.damaging: UpdateDamaging(); break;
            case State.deathing: break;
        }
    }

    void UpdateFollowing()
    {
        if (target == null) return;

        Vector2 dir = (target.position - transform.position).normalized;
        animator.transform.rotation = Quaternion.LookRotation(Vector3.forward, dir * -1f);
        transform.Translate(dir * moveSpeed * Time.deltaTime);

        float dist = Vector2.Distance(transform.position, target.position);
        if (dist <= attackDistance)
            ChangeState(State.charging);
    }
    void UpdateEscaping()
    {
        if (target == null) return;

        Vector2 dir = (transform.position - target.position).normalized;
        animator.transform.rotation = Quaternion.LookRotation(Vector3.forward, dir * 1f);
        transform.Translate(dir * moveSpeed * Time.deltaTime);

        float dist = Vector2.Distance(transform.position, target.position);
        if (dist >= attackDistance)
            ChangeState(State.charging);
    }

    void UpdateCharging()
    {
        if (stateTimer <= 0f)
            ChangeState(State.attacking);
    }

    void UpdateAttacking()
    {
        if (stateTimer <= 0f)
        {
            animator.Play("Attack");
            float dist = Vector2.Distance(transform.position, target.position);
            if (dist <= attackDistance)
                ChangeState(State.escaping);
            else
                ChangeState(State.following);
        }
    }
    void UpdateDamaging()
    {
        if (stateTimer <= 0f)
        {
            float dist = Vector2.Distance(transform.position, target.position);
            if (dist <= attackDistance)
                ChangeState(State.escaping);
            else
                ChangeState(State.following);
        }
    }

    void ChangeState(State next)
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
                animator.Play("Death");
                Destroy(gameObject, 1f);
                break;
        }
    }

    void OnDamaged(float amount)
    {
        if (currentState == State.following || currentState == State.charging || currentState == State.escaping)
        {
            ChangeState(State.damaging);
            animator.Play("Dmg");
        }


    }

    void OnDeath()
    {
        ChangeState(State.deathing);
    }

}