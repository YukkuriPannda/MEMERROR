using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopController : MonoBehaviour
{
	public enum ShopItemType
	{
		ChangeSkill,
		AddSkill,
		HealItem,
	}

	[Serializable]
	public class ShopItem
	{
		public string itemName;
		public Sprite cardIcon;
		public int price;
		public ShopItemType itemType;

		public PlayerSkillBase skillPrefab;
		public float healAmount;

	}

	[SerializeField] GameObject shopCanvas;
	[SerializeField] Transform cardContainer;
	[SerializeField] ShopItem[] shopItems;
	[SerializeField] PlayerController player;
	[SerializeField] List<UnityEngine.InputSystem.InputActionReference> newSlotActions;

	const int DisplayCount = 3;

	int[] currentShopItemIndexes = Array.Empty<int>();
	readonly List<GameObject> spawnedCards = new();

	public bool IsShopOpen { get; set; }
	[SerializeField] GameObject cardPrefab;

	public void OpenShop()
	{
		IsShopOpen = true;
		currentShopItemIndexes = PickRandomIndexes(DisplayCount);

		foreach (GameObject card in spawnedCards)
			Destroy(card);
		spawnedCards.Clear();

		float[] cardPositionsX = { -200f, 0f, 200f };
		for (int i = 0; i < currentShopItemIndexes.Length; i++)
		{
			ShopItem item = shopItems[currentShopItemIndexes[i]];

			GameObject card = Instantiate(cardPrefab, cardContainer);
			card.name = item.itemName;
			card.GetComponent<Image>().sprite = item.cardIcon;
			card.transform.Find("Price").GetComponent<TextMeshProUGUI>().text = item.price.ToString();
			card.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = item.itemName.ToString();
			RectTransform rect = card.GetComponent<RectTransform>();
			if (rect != null)
			{
				Vector2 pos = rect.anchoredPosition;
				pos.x = cardPositionsX[i];
				rect.anchoredPosition = pos;
			}

			Button button = card.GetComponent<Button>();
			Debug.Log($"Created shop card for {item.itemName}, button found: {button != null}");
			if (button != null)
			{
				ShopItem captured = item;
				Debug.Log($"Adding click listener for {button.onClick}");
				button.onClick.AddListener(() =>
				{
					TryPurchase(captured, player);
					if (captured.itemType != ShopItemType.ChangeSkill)
						GameMaster.Instance.CloseShop();
					Debug.Log($"Clicked on {captured.itemName}, purchase attempted.");
				});
			}

			spawnedCards.Add(card);
		}

		if (shopCanvas != null) shopCanvas.SetActive(true);
	}

	int[] PickRandomIndexes(int count)
	{
		count = Mathf.Min(count, shopItems.Length);
		List<int> pool = new();
		for (int i = 0; i < shopItems.Length; i++) pool.Add(i);

		int[] result = new int[count];
		for (int i = 0; i < count; i++)
		{
			int pick = UnityEngine.Random.Range(0, pool.Count);
			result[i] = pool[pick];
			pool.RemoveAt(pick);
		}
		return result;
	}

	public bool TryPurchase(ShopItem item, PlayerController player)
	{
		if (!GameMaster.Instance.TrySpendStorage(item.price)) return false;

		switch (item.itemType)
		{
			case ShopItemType.ChangeSkill:
				if (item.skillPrefab != null)
					StartCoroutine(SelectSlotRoutine(item.skillPrefab, player));
				break;

			case ShopItemType.AddSkill:
				if (newSlotActions.Count > 0 && item.skillPrefab != null)
				{
					Debug.Log($"Adding new skill slot with action {newSlotActions[0].name} for skill {item.skillPrefab.name}");
					UnityEngine.InputSystem.InputActionReference newSlotAction = newSlotActions[0];
					AddSkillSlot(player, newSlotAction, item.skillPrefab);
					newSlotActions.RemoveAt(0);
				}
				break;

			case ShopItemType.HealItem:
				HPController hp = player.GetComponent<HPController>();
				if (hp != null) hp.Heal(item.healAmount);
				break;
		}

		return true;
	}

	IEnumerator SelectSlotRoutine(PlayerSkillBase skillPrefab, PlayerController player)
	{
		PlayerController.SpecialSkillData[] slots = player.specialSkills;

		while (true)
		{
			for (int i = 0; i < slots.Length; i++)
			{
				if (slots[i].activateAction.action.triggered)
				{
					AssignSpecialSkill(skillPrefab, player, i);
					GameMaster.Instance.CloseShop();
					yield break;
				}
			}
			yield return null;
		}
	}

	void AssignSpecialSkill(PlayerSkillBase skillPrefab, PlayerController player, int slotIndex)
	{
		PlayerSkillBase skill = Instantiate(skillPrefab, player.transform);
		player.specialSkills[slotIndex].skill = skill;
	}

	const int MaxSkillSlots = 3;

	void AddSkillSlot(PlayerController player, UnityEngine.InputSystem.InputActionReference newSlotAction, PlayerSkillBase skillPrefab)
	{
		if (player.specialSkills.Length >= MaxSkillSlots) return;

		PlayerSkillBase skill = Instantiate(skillPrefab, player.transform);
		PlayerController.SpecialSkillData newSlot = new()
		{
			activateAction = newSlotAction,
			isActivated = false,
			skill = skill,
		};
		newSlotAction.action.Enable();

		int oldLength = player.specialSkills.Length;
		Array.Resize(ref player.specialSkills, oldLength + 1);
		player.specialSkills[oldLength] = newSlot;
	}
}
