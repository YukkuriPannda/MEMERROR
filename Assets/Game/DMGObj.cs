using UnityEngine;
using System;
public class DMGObj : MonoBehaviour
{
    [SerializeField] float damage = 10f;
    [SerializeField] HPController.HPType[] targetTypes;

    void Start()
    {
        var col = GetComponent<Collider2D>();
        if (col == null) return;

        var results = new Collider2D[10];
        int count = col.Overlap(new ContactFilter2D().NoFilter(), results);
        for (int i = 0; i < count; i++)
        {
            TryDamage(results[i]);
            if (this == null) return;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        TryDamage(other);
    }

    void TryDamage(Collider2D other)
    {
        Debug.Log("DMGObj TryDamage: " + other.name + " from " + gameObject.name);
        HPController hp = other.GetComponent<HPController>();
        Debug.Log("DMGObj TryDamage HPController: " + (hp != null ? hp.name : "null"));
        if (hp == null) return;
        if (Array.IndexOf(targetTypes, hp.Type) < 0) return;
        hp.TakeDamage(damage);
        Destroy(gameObject);
    }
}
