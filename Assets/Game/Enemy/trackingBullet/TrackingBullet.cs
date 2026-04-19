using UnityEngine;

public class TrackingBullet : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float rotationSpeed = 180f;

    public Transform targetObj;

    DMGObj dmgObj;

    void Awake()
    {
        dmgObj = GetComponent<DMGObj>();
        if (dmgObj != null)
            dmgObj.onHit += _ => Destroy(gameObject);
    }

    void Start()
    {
        if (targetObj == null)
        {
            var playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
                targetObj = playerObj.transform;
        }
    }

    void Update()
    {
        if (targetObj != null)
        {
            Vector2 dir = (targetObj.position - transform.position).normalized;
            float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            float newAngle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, targetAngle, rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f, 0f, newAngle);
        }

        transform.Translate(Vector2.up * moveSpeed * Time.deltaTime);
    }
}
