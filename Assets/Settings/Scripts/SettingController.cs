using TMPro;
using UnityEngine;

public class SettingController : MonoBehaviour
{
    public GameObject keyConfigPrefab;
    public Transform bodyTransform;

    void Start()
    {
        SaveDataManager.SaveData saveData = SaveDataManager.Load();
        SaveDataManager.Keybindings kb = saveData.keyBindings ?? new SaveDataManager.Keybindings();

        foreach (Transform child in bodyTransform)
            Destroy(child.gameObject);

        var entries = new (string label, KeyCode key)[]
        {
            ("move_forward",   kb.moveUp),
            ("move_back",      kb.moveDown),
            ("move_left",      kb.moveLeft),
            ("move_right",     kb.moveRight),
            ("skill_1",        kb.Skill1),
            ("skill_2",        kb.Skill2),
            ("skill_3",        kb.Skill3),
            ("activate_skill", kb.activateSkill),
        };

        for (int i = 0; i < entries.Length; i++)
        {
            GameObject instance = Instantiate(keyConfigPrefab, bodyTransform);
            RectTransform rt = instance.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(0f, i * -64f + 256f);

            instance.transform.Find("Title")
                .GetComponent<TextMeshProUGUI>().text = entries[i].label;

            instance.transform.Find("KeyBoxes/KeyBox_Text/Text (TMP)")
                .GetComponent<TextMeshProUGUI>().text = entries[i].key.ToString();
        }
    }
}
