using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeyConfigDisplay : MonoBehaviour
{
    [SerializeField] int KeyIndex0 = 4; // 0:moveUp 1:moveDown 2:moveLeft 3:moveRight 4:Skill1 5:Skill2 6:Skill3 7:activateSkill
    [SerializeField] Transform waku0;
    [SerializeField] int keyIndex1 = 5;
    [SerializeField] Transform waku1;

    void Start()
    {
        Refresh();
    }

    void OnEnable()
    {
        Refresh();
    }

    void Refresh()
    {
        var kb = SaveDataManager.Load().keyBindings ?? new SaveDataManager.Keybindings();
        var list0 = kb[KeyIndex0];
        var list1 = kb[keyIndex1];

        ApplyKey(waku0, list0 != null && list0.Count > 0 ? list0[0] : KeyCode.None);
        ApplyKey(waku1, list1 != null && list1.Count > 0 ? list1[0] : KeyCode.None);
    }

    void ApplyKey(Transform waku, KeyCode kc)
    {
        if (waku == null) return;
        Transform key = waku.Find("key");
        if (key == null) return;

        var text = key.Find("Text (TMP)")?.GetComponent<TextMeshProUGUI>();
        var arrow = key.Find("ArrowIcon")?.GetComponent<Image>();

        bool isArrow = kc == KeyCode.UpArrow || kc == KeyCode.DownArrow ||
                       kc == KeyCode.LeftArrow || kc == KeyCode.RightArrow;

        if (text != null) text.gameObject.SetActive(!isArrow);
        if (arrow != null) arrow.gameObject.SetActive(isArrow);

        if (kc == KeyCode.None)
        {
            if (text != null) { text.gameObject.SetActive(true); text.text = "-"; }
            if (arrow != null) arrow.gameObject.SetActive(false);
            return;
        }

        if (isArrow && arrow != null)
        {
            arrow.rectTransform.rotation = kc switch
            {
                KeyCode.UpArrow => Quaternion.Euler(0, 0, 0),
                KeyCode.LeftArrow => Quaternion.Euler(0, 0, 90),
                KeyCode.DownArrow => Quaternion.Euler(0, 0, 180),
                KeyCode.RightArrow => Quaternion.Euler(0, 0, 270),
                _ => Quaternion.identity
            };
        }
        else if (text != null)
        {
            text.text = kc.ToString();
        }
    }
}
