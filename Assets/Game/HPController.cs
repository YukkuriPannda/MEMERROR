using System;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
public class HPController : MonoBehaviour
{
	[SerializeField] float maxHP = 100f;

	public float CurrentHP { get; private set; }
	public float MaxHP => maxHP;

	public event Action<float> OnDamaged;
	public event Action OnDeath;
	public event Action OnHPChanged;
	public bool parry = false;
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
		OnDeath += () => GameMaster.Instance?.DeathEvent.Invoke(this);
	}

	public void TakeDamage(DMGObj dmgObj)
	{
		float amount = dmgObj.damage;
		if (parry)
		{
			OnDamaged?.Invoke(0f);
			OnHPChanged?.Invoke();
			if (dmgObj.parryPrefab != null)
			{
				GameObject parryObj = Instantiate(dmgObj.parryPrefab, transform.position, dmgObj.transform.rotation);
				parryObj.transform.Rotate(0f, 0f, 180f);//flip
				Destroy(dmgObj.gameObject);
				Destroy(parryObj, 0.5f);
			}
			if (dmgObj.owner != null)
			{
				HPController ownerHP = dmgObj.owner.GetComponent<HPController>();
				if (ownerHP != null)
				{
					ownerHP.TakeDamage(new DMGObj { damage = amount, owner = gameObject });
				}
			}
			Destroy(dmgObj.gameObject);
			StartCoroutine(ParryDuration(0.5f));
			return;
		}
		if (CurrentHP <= 0f) return;

		CurrentHP = Mathf.Max(0f, CurrentHP - amount);
		OnDamaged?.Invoke(amount);
		OnHPChanged?.Invoke();

		if (CurrentHP <= 0f)
			OnDeath?.Invoke();
		Debug.Log($"{gameObject.name} took {amount} damage. Current HP: {CurrentHP}/{maxHP}");
	}

	public void Heal(float amount)
	{
		CurrentHP = Mathf.Min(maxHP, CurrentHP + amount);
		OnHPChanged?.Invoke();
	}
	IEnumerator ParryDuration(float duration)
	{
		Time.timeScale = 0.2f;
		yield return new WaitForSeconds(duration * Time.timeScale);
		Time.timeScale = 1f;
	}
}
