using System;
using UnityEngine;

public class HPController : MonoBehaviour
{
	[SerializeField] float maxHP = 100f;

	public float CurrentHP { get; private set; }
	public float MaxHP => maxHP;

	public event Action<float> OnDamaged;
	public event Action OnDeath;

	public enum HPType
	{
		Player,
		Enemy
	}

	[SerializeField] HPType hpType;
	public HPType Type => hpType;

	void Awake()
	{
		CurrentHP = maxHP;
	}

	public void TakeDamage(float amount)
	{

		if (CurrentHP <= 0f) return;

		CurrentHP = Mathf.Max(0f, CurrentHP - amount);
		OnDamaged?.Invoke(amount);

		if (CurrentHP <= 0f)
			OnDeath?.Invoke();
		Debug.Log($"{gameObject.name} took {amount} damage. Current HP: {CurrentHP}/{maxHP}");
	}

	public void Heal(float amount)
	{
		CurrentHP = Mathf.Min(maxHP, CurrentHP + amount);
	}
}
