using Microsoft.Win32.SafeHandles;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SettingController : MonoBehaviour
{
    public GameObject keyConfigPrefab;
    public GameObject keyBoxPrefab;
    public Transform bodyTransform;
    private static readonly string[] Labels =
    {
        "move_forward", "move_back", "move_left", "move_right",
        "skill_1", "skill_2", "skill_3", "activate_skill",
    };

    private int settingKeyIndex = -1;

    void Start()
    {
        RefreshKeyBindingsUI();
    }

    void RefreshKeyBindingsUI()
    {
        SaveDataManager.SaveData saveData = SaveDataManager.Load();
        SaveDataManager.Keybindings kb = saveData.keyBindings ?? new SaveDataManager.Keybindings();

        foreach (Transform child in bodyTransform)
            Destroy(child.gameObject);

        for (int i = 0; i < SaveDataManager.Keybindings.Count; i++)
        {
            int capturedIndex = i;
            GameObject instance = Instantiate(keyConfigPrefab, bodyTransform);
            RectTransform rt = instance.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(0f, i * -64f + 256f);
            instance.name = Labels[i];

            instance.transform.Find("Title")
                .GetComponent<TextMeshProUGUI>().text = Labels[i];

            Transform keyBoxes = instance.transform.Find("KeyBoxes");
            foreach (Transform child in keyBoxes)
                Destroy(child.gameObject);
            foreach (KeyCode kc in kb[i])
            {
                if (kc == KeyCode.None) continue;
                GameObject keyBox = Instantiate(keyBoxPrefab, keyBoxes);
                if (kc == KeyCode.DownArrow || kc == KeyCode.UpArrow || kc == KeyCode.LeftArrow || kc == KeyCode.RightArrow)
                {
                    keyBox.transform.Find("Text (TMP)").gameObject.SetActive(false);
                    keyBox.transform.Find("ArrowIcon").gameObject.SetActive(true);
                }
                if (kc == KeyCode.UpArrow) keyBox.transform.Find("ArrowIcon").GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 0);
                if (kc == KeyCode.LeftArrow) keyBox.transform.Find("ArrowIcon").GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 90);
                if (kc == KeyCode.DownArrow) keyBox.transform.Find("ArrowIcon").GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 180);
                if (kc == KeyCode.RightArrow) keyBox.transform.Find("ArrowIcon").GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 270);
                keyBox.transform.Find("Text (TMP)")
                    .GetComponent<TextMeshProUGUI>().text = kc.ToString();
            }

            var eventTrigger = instance.GetComponent<EventTrigger>() ?? instance.AddComponent<EventTrigger>();
            var entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
            entry.callback.AddListener(_ => OnKeyConfigClicked(capturedIndex));
            eventTrigger.triggers.Add(entry);
        }
    }
    void OnKeyConfigClicked(int index)
    {
        Debug.Log("Key config clicked: " + Labels[index]);
        settingKeyIndex = index;
    }

    void Update()
    {
        if (settingKeyIndex < 0) return;

        var keyboard = Keyboard.current;
        if (keyboard == null || !keyboard.anyKey.wasPressedThisFrame) return;

        foreach (var control in keyboard.allKeys)
        {
            if (!control.wasPressedThisFrame) continue;
            if (!System.Enum.TryParse(control.keyCode.ToString(), out KeyCode kc)) continue;
            SaveDataManager.SaveData saveData = SaveDataManager.Load();
            if (saveData.keyBindings[settingKeyIndex].Contains(kc))
            {
                Debug.Log("Key already bound: " + kc);
                settingKeyIndex = -1;
                return;
            }
            if (kc == KeyCode.Escape)
            {
                saveData.keyBindings[settingKeyIndex].Clear();
                SaveDataManager.Save(saveData);
                RefreshKeyBindingsUI();
                settingKeyIndex = -1;
                return;
            }
            if (saveData.keyBindings[settingKeyIndex].Count >= 2)
            {
                Debug.Log("Maximum keys bound for this action.");
                settingKeyIndex = -1;
                return;
            }
            saveData.keyBindings[settingKeyIndex].Add(kc);
            SaveDataManager.Save(saveData);
            settingKeyIndex = -1;
            RefreshKeyBindingsUI();
            break;
        }
    }
}