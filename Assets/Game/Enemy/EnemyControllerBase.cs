using UnityEngine;

public abstract class EnemyControllerBase : MonoBehaviour
{
    public enum State
    {
        following,
        charging,
        attacking,
        damaging,
        deathing,
    }

    [SerializeField] protected float moveSpeed = 2f;
    [SerializeField] protected float attackDistance = 1.5f;
    [SerializeField] protected float damageDuration = 0.4f;
    [SerializeField] protected Animator animator;
    [SerializeField] protected int scoreValue = 100;
    [SerializeField] protected int threatValue = 1;

    protected Transform target;
    protected HPController hpController;
    [SerializeField] protected State currentState = State.following;
    protected float stateTimer;

    protected virtual void Awake()
    {
        hpController = GetComponent<HPController>();
        hpController.OnDamaged += OnDamaged;
        hpController.OnDeath += OnDeath;
    }

    protected virtual void Start()
    {
        var playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            target = playerObj.transform;
        GameMaster.Instance?.RegisterEnemy(hpController);
    }

    protected virtual void Update()
    {
        stateTimer -= Time.deltaTime;
        switch (currentState)
        {
            case State.following: UpdateFollowing(); break;
            case State.charging: UpdateCharging(); break;
            case State.attacking: UpdateAttacking(); break;
            case State.damaging: UpdateDamaging(); break;
            case State.deathing: break;
        }
        if (GameMaster.Instance != null)
            transform.position = GameMaster.Instance.ClampEnemyPosition(transform.position);
    }

    protected virtual void UpdateFollowing()
    {
        if (target == null) return;
        Vector2 dir = ((Vector2)target.position - (Vector2)transform.position).normalized;
        animator.transform.rotation = FacingRotation(dir);
        transform.Translate(dir * moveSpeed * Time.deltaTime);
        if (Vector2.Distance(transform.position, target.position) <= attackDistance)
            ChangeState(State.charging);
    }

    protected abstract void UpdateCharging();
    protected abstract void UpdateAttacking();

    protected virtual void UpdateDamaging()
    {
        if (stateTimer <= 0f)
            ChangeState(State.following);
    }

    protected abstract void ChangeState(State next);

    // 死亡演出の共通処理
    protected void EnterDying()
    {
        animator.Play("Death");
        Destroy(gameObject, 1f);
    }

    // animator.transform.up が「target と逆向き」になる回転を返すヘルパー
    protected static Quaternion FacingRotation(Vector2 toTarget) =>
        Quaternion.LookRotation(Vector3.forward, -(toTarget.normalized));

    protected virtual void OnDamaged(float amount)
    {
        if (currentState == State.following || currentState == State.charging)
        {
            ChangeState(State.damaging);
            animator.Play("Dmg");
        }
    }

    protected virtual void OnDeath()
    {
        ChangeState(State.deathing);
    }
}
