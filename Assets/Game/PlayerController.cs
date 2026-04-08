using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] public Transform anker;
    [SerializeField] InputActionReference moveAction;
    public PlayerSkillBase normalSkill;
    public float normalSkillFrequency = 0.5f;
    private float nextNormalSkillTime = 0f;


    void OnEnable()
    {
        moveAction.action.Enable();
    }

    void OnDisable()
    {
        moveAction.action.Disable();
    }

    void Start()
    {
    }

    void Update()
    {
        Move();
        AnkerRotate();
        if (Time.time >= nextNormalSkillTime)
        {
            NormalSkill();
            nextNormalSkillTime = Time.time + normalSkillFrequency;
        }
    }

    void Move()
    {
        Vector2 input = moveAction.action.ReadValue<Vector2>();
        transform.Translate(input * moveSpeed * Time.deltaTime);
    }

    void AnkerRotate()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorld.z = 0f;
        Vector2 dir = (Vector2)(mouseWorld - anker.position);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        anker.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void NormalSkill()
    {
        normalSkill.Skill(this);
    }
}
