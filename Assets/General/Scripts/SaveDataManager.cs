using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveDataManager
{
	public static string filePath = Application.persistentDataPath + "/save_data.json";

	[System.Serializable]
	public class Keybindings
	{
		public List<KeyCode> moveUp;
		public List<KeyCode> moveDown;
		public List<KeyCode> moveLeft;
		public List<KeyCode> moveRight;
		public List<KeyCode> Skill1;
		public List<KeyCode> Skill2;
		public List<KeyCode> Skill3;
		public List<KeyCode> activateSkill;

		public Keybindings()
		{
			moveUp = new List<KeyCode> { KeyCode.W };
			moveDown = new List<KeyCode> { KeyCode.S };
			moveLeft = new List<KeyCode> { KeyCode.A };
			moveRight = new List<KeyCode> { KeyCode.D };
			Skill1 = new List<KeyCode> { KeyCode.X };
			Skill2 = new List<KeyCode> { KeyCode.C };
			Skill3 = new List<KeyCode> { KeyCode.V };
			activateSkill = new List<KeyCode> { KeyCode.Space };
		}

		public const int Count = 8;

		public List<KeyCode> this[int i]
		{
			get => i switch
			{
				0 => moveUp,
				1 => moveDown,
				2 => moveLeft,
				3 => moveRight,
				4 => Skill1,
				5 => Skill2,
				6 => Skill3,
				7 => activateSkill,
				_ => throw new System.IndexOutOfRangeException()
			};
		}
	}

	[System.Serializable]
	public class PlayerData
	{
		public int highScore;
		public int[] bossList;
		public int[] achievements;
		public string playerName;

		public PlayerData()
		{
			highScore = 0;
			bossList = new int[0];
			achievements = new int[0];
			playerName = "Player";
		}
	}

	[System.Serializable]
	public class SaveData
	{
		public Keybindings keyBindings;
		public PlayerData playerData;
	}
	public static SaveData Load()
	{
		if (!File.Exists(filePath))
		{
			Save();
			return new SaveData();
		}
		string json = File.ReadAllText(filePath);
		SaveData data = JsonUtility.FromJson<SaveData>(json);
		UnityEngine.Debug.Log("Save data loaded from: " + filePath);
		UnityEngine.Debug.Log("Loaded data: " + data.playerData.playerName + ", High Score: " + data.playerData.highScore);
		return data;
	}

	public static void Save(SaveData data = null)
	{
		if (data == null)
		{
			data = new SaveData();
			data.keyBindings = new Keybindings
			{
				moveUp = new List<KeyCode> { KeyCode.W },
				moveDown = new List<KeyCode> { KeyCode.S },
				moveLeft = new List<KeyCode> { KeyCode.A },
				moveRight = new List<KeyCode> { KeyCode.D },
				Skill1 = new List<KeyCode> { KeyCode.X },
				Skill2 = new List<KeyCode> { KeyCode.C },
				Skill3 = new List<KeyCode> { KeyCode.V },
				activateSkill = new List<KeyCode> { KeyCode.Space }
			};
			data.playerData = new PlayerData
			{
				highScore = 0,
				bossList = new int[0],
				achievements = new int[0],
				playerName = "Player"
			};
			UnityEngine.Debug.Log("No save file found. Creating a new one with default values.");
		}
		string json = JsonUtility.ToJson(data, true);
		File.WriteAllText(filePath, json);
		UnityEngine.Debug.Log("Save data has been saved to: " + filePath);
	}
}
