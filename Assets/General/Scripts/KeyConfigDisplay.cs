using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeyConfigDisplay : MonoBehaviour
{
    [Serializable]
    public class WakuConfig
    {
        public Transform waku;
        public int keyIndex;
    }

    [SerializeField] WakuConfig[] wakuConfigs;

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
        foreach (WakuConfig config in wakuConfigs)
        {
            var list = kb[config.keyIndex];
            KeyCode kc = list != null && list.Count > 0 ? list[0] : KeyCode.None;
            ApplyKey(config.waku, kc);
        }
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
