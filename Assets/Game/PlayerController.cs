using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] Transform anker;
    [SerializeField] InputActionReference moveAction;

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
}
