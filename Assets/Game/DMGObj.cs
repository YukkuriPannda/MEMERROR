using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class DMGObj : MonoBehaviour
{
    [SerializeField] float damage = 10f;
    [SerializeField] float damageInterval = 1f;
    [SerializeField] HPController.HPType[] targetTypes;

    public Action<float> onHit;


    // 衝突中のオブジェクトとそれぞれのタイマーを管理
    Dictionary<Collider2D, float> contactTimers = new Dictionary<Collider2D, float>();
    void OnEnable()
    {
        contactTimers.Clear();

        var col = GetComponent<Collider2D>();
        if (col == null) return;

        var results = new Collider2D[10];
        int count = col.Overlap(new ContactFilter2D().NoFilter(), results);
        for (int i = 0; i < count; i++)
        {
            if (!IsTarget(results[i])) continue;
            TryDamage(results[i]);
            if (this == null) return;
            contactTimers[results[i]] = damageInterval;
        }
    }
    void Update()
    {
        var keys = new List<Collider2D>(contactTimers.Keys);

        foreach (var key in keys)
        {
            if (key == null)
            {
                contactTimers.Remove(key);
                continue;
            }

            contactTimers[key] -= Time.deltaTime;
            if (contactTimers[key] <= 0f)
            {
                TryDamage(key);
                contactTimers[key] = damageInterval;
            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!IsTarget(other)) return;
        if (contactTimers.ContainsKey(other)) return;

        TryDamage(other);
        contactTimers[other] = damageInterval;
        Debug.Log(contactTimers);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        contactTimers.Remove(other);
    }

    bool IsTarget(Collider2D other)
    {
        HPController hp = other.GetComponent<HPController>();
        if (hp == null) return false;
        return Array.IndexOf(targetTypes, hp.Type) >= 0;
    }

    void TryDamage(Collider2D other)
    {
        HPController hp = other.GetComponent<HPController>();
        if (hp == null) return;
        if (Array.IndexOf(targetTypes, hp.Type) < 0) return;
        hp.TakeDamage(damage);
        onHit?.Invoke(damage);
    }
}
