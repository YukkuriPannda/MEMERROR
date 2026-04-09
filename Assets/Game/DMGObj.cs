using UnityEngine;

public class DMGObj : MonoBehaviour
{
    [SerializeField] float damage = 10f;
    [SerializeField] HPController.HPType[] targetTypes;

    void OnTriggerEnter2D(Collider2D other)
    {
        HPController hp = other.GetComponent<HPController>();
        if (hp == null) return;
        if (System.Array.IndexOf(targetTypes, hp.Type) < 0) return;
        hp.TakeDamage(damage);
        Destroy(gameObject);
    }
}
