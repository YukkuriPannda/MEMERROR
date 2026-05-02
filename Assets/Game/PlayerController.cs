using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public float moveSpeed = 5f;
    [SerializeField] public Transform anker;
    [SerializeField] InputActionReference moveAction;

    [Space(10)]
    [Header("Move Limit")]
    [SerializeField] float minX = -8f;
    [SerializeField] float maxX = 8f;
    [SerializeField] float minY = -4f;
    [SerializeField] float maxY = 4f;

    [Space(10)]
    [Header("Skills")]
    [Header("Normal Skill")]
    public PlayerSkillBase normalSkill;
    public float normalSkillFrequency = 0.5f;
    private float nextNormalSkillTime = 0f;

    [Header("Special Skill")]
    [SerializeField] InputActionReference specialSkillExecuteAction;
    [Serializable]
    public class SpecialSkillData
    {
        public PlayerSkillBase skill;
        public bool isActivated;
        public InputActionReference activateAction;
    }
    public SpecialSkillData[] specialSkills;


    void OnEnable()
    {
        moveAction.action.Enable();
        foreach (SpecialSkillData slot in specialSkills)
            slot.activateAction.action.Enable();
    }

    void OnDisable()
    {
        moveAction.action.Disable();
        foreach (SpecialSkillData slot in specialSkills)
            slot.activateAction.action.Disable();
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
        foreach (SpecialSkillData slot in specialSkills)
        {
            if (slot.activateAction.action.triggered)
                ActivateSpecialSkill(slot);
        }
        if (specialSkillExecuteAction.action.triggered)
        {
            foreach (SpecialSkillData slot in specialSkills)
            {
                if (slot.isActivated)
                    ExecuteSpecialSkill(slot);
            }
        }
    }

    void Move()
    {
        Vector2 input = moveAction.action.ReadValue<Vector2>();
        transform.Translate(input * moveSpeed * Time.deltaTime);
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.position = pos;
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

    void ActivateSpecialSkill(SpecialSkillData slot)
    {
        slot.isActivated = true;
        slot.skill.ActivateSkill(this);
    }

    void ExecuteSpecialSkill(SpecialSkillData slot)
    {
        slot.skill.ExecuteSkill(this);
    }
}
