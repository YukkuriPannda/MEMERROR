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
    [SerializeField] SpecialSkillData specialSkill0;
    [SerializeField] SpecialSkillData specialSkill1;


    void OnEnable()
    {
        moveAction.action.Enable();
        specialSkill0.activateAction.action.Enable();
        specialSkill1.activateAction.action.Enable();
    }

    void OnDisable()
    {
        moveAction.action.Disable();
        specialSkill0.activateAction.action.Disable();
        specialSkill1.activateAction.action.Disable();
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
        if (specialSkill0.activateAction.action.triggered)
        {
            ActivateSpecialSkill0();
        }
        if (specialSkill1.activateAction.action.triggered)
        {
            ActivateSpecialSkill1();
        }
        if (specialSkillExecuteAction.action.triggered)
        {
            if (specialSkill0.isActivated)
            {
                ExecuteSpecialSkill0();
            }
            if (specialSkill1.isActivated)
            {
                ExecuteSpecialSkill1();
            }
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
    void ActivateSpecialSkill0()
    {
        specialSkill0.isActivated = true;
        Debug.Log("ActivateSpecialSkill0");
        specialSkill0.skill.ActivateSkill(this);
    }
    void ExecuteSpecialSkill0()
    {
        Debug.Log("ExecuteSpecialSkill0");
        specialSkill0.skill.ExecuteSkill(this);
    }

    void ActivateSpecialSkill1()
    {

        specialSkill1.isActivated = true;
        specialSkill1.skill.ActivateSkill(this);
    }
    void ExecuteSpecialSkill1()
    {
        specialSkill1.skill.ExecuteSkill(this);
    }
}
