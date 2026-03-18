using System.Diagnostics;
using System.IO;
using UnityEngine;

public static class SaveDataManager
{
	public static string filePath = Application.persistentDataPath + "/save_data.json";

	[System.Serializable]
	public class Keybindings
	{
		public KeyCode moveUp;
		public KeyCode moveDown;
		public KeyCode moveLeft;
		public KeyCode moveRight;
		public KeyCode Skill1;
		public KeyCode Skill2;
		public KeyCode Skill3;
		public KeyCode activateSkill;

		public Keybindings()
		{
			moveUp = KeyCode.W;
			moveDown = KeyCode.S;
			moveLeft = KeyCode.A;
			moveRight = KeyCode.D;
			Skill1 = KeyCode.X;
			Skill2 = KeyCode.C;
			Skill3 = KeyCode.V;
			activateSkill = KeyCode.Space;
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
				moveUp = KeyCode.W,
				moveDown = KeyCode.S,
				moveLeft = KeyCode.A,
				moveRight = KeyCode.D,
				Skill1 = KeyCode.X,
				Skill2 = KeyCode.C,
				Skill3 = KeyCode.V,
				activateSkill = KeyCode.Space
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
