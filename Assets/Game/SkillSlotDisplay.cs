using UnityEngine;
using UnityEngine.UI;

public class SkillSlotDisplay : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] Image[] wakuImages;
    [SerializeField] Sprite emptySprite;

    void Start()
    {
        player.OnSkillsChanged += Refresh;
        Refresh();
    }

    void OnDestroy()
    {
        if (player != null)
            player.OnSkillsChanged -= Refresh;
    }

    void Refresh()
    {
        for (int i = 0; i < wakuImages.Length; i++)
        {
            if (wakuImages[i] == null) continue;

            Sprite icon = null;
            if (i < player.specialSkills.Length)
                icon = player.specialSkills[i].icon;

            wakuImages[i].sprite = icon != null ? icon : emptySprite;
        }
    }
}
