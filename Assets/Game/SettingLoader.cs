using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SettingLoader : MonoBehaviour
{
    [SerializeField] InputActionAsset inputActionAsset;

    void Awake()
    {
        SaveDataManager.SaveData saveData = SaveDataManager.Load();
        SaveDataManager.Keybindings kb = saveData.keyBindings ?? new SaveDataManager.Keybindings();

        InputActionMap playerMap = inputActionAsset.FindActionMap("Player", throwIfNotFound: true);

        ApplyMoveBindings(playerMap.FindAction("Move",         throwIfNotFound: true), kb);
        ApplyButtonBinding(playerMap.FindAction("Skill1",        throwIfNotFound: true), kb.Skill1);
        ApplyButtonBinding(playerMap.FindAction("Skill2",        throwIfNotFound: true), kb.Skill2);
        ApplyButtonBinding(playerMap.FindAction("Skill3",        throwIfNotFound: true), kb.Skill3);
        ApplyButtonBinding(playerMap.FindAction("activateSkill", throwIfNotFound: true), kb.activateSkill);
    }

    void ApplyMoveBindings(InputAction action, SaveDataManager.Keybindings kb)
    {
        var counts = new Dictionary<string, int> { { "up", 0 }, { "down", 0 }, { "left", 0 }, { "right", 0 } };
        var keys = new Dictionary<string, List<KeyCode>>
        {
            { "up",    kb.moveUp    },
            { "down",  kb.moveDown  },
            { "left",  kb.moveLeft  },
            { "right", kb.moveRight }
        };

        for (int i = 0; i < action.bindings.Count; i++)
        {
            InputBinding b = action.bindings[i];
            if (!b.isPartOfComposite) continue;

            string dir = b.name.ToLower();
            if (!counts.ContainsKey(dir)) continue;

            int slot = counts[dir]++;
            string path = slot < keys[dir].Count ? KeyCodeToPath(keys[dir][slot]) : "";
            action.ApplyBindingOverride(i, path);
        }
    }

    void ApplyButtonBinding(InputAction action, List<KeyCode> keyList)
    {
        int slot = 0;
        for (int i = 0; i < action.bindings.Count; i++)
        {
            InputBinding b = action.bindings[i];
            // 空パス（プレースホルダー）またはキーボードバインディングのみ対象
            if (!string.IsNullOrEmpty(b.path) && !b.path.StartsWith("<Keyboard>"))
                continue;

            string path = slot < keyList.Count ? KeyCodeToPath(keyList[slot]) : "";
            action.ApplyBindingOverride(i, path);
            slot++;
            if (slot >= 2) break;
        }
    }

    static string KeyCodeToPath(KeyCode kc) => kc switch
    {
        KeyCode.UpArrow      => "<Keyboard>/upArrow",
        KeyCode.DownArrow    => "<Keyboard>/downArrow",
        KeyCode.LeftArrow    => "<Keyboard>/leftArrow",
        KeyCode.RightArrow   => "<Keyboard>/rightArrow",
        KeyCode.Space        => "<Keyboard>/space",
        KeyCode.Return       => "<Keyboard>/enter",
        KeyCode.Backspace    => "<Keyboard>/backspace",
        KeyCode.Tab          => "<Keyboard>/tab",
        KeyCode.Escape       => "<Keyboard>/escape",
        KeyCode.Delete       => "<Keyboard>/delete",
        KeyCode.LeftShift    => "<Keyboard>/leftShift",
        KeyCode.RightShift   => "<Keyboard>/rightShift",
        KeyCode.LeftControl  => "<Keyboard>/leftCtrl",
        KeyCode.RightControl => "<Keyboard>/rightCtrl",
        KeyCode.LeftAlt      => "<Keyboard>/leftAlt",
        KeyCode.RightAlt     => "<Keyboard>/rightAlt",
        KeyCode.Alpha0       => "<Keyboard>/0",
        KeyCode.Alpha1       => "<Keyboard>/1",
        KeyCode.Alpha2       => "<Keyboard>/2",
        KeyCode.Alpha3       => "<Keyboard>/3",
        KeyCode.Alpha4       => "<Keyboard>/4",
        KeyCode.Alpha5       => "<Keyboard>/5",
        KeyCode.Alpha6       => "<Keyboard>/6",
        KeyCode.Alpha7       => "<Keyboard>/7",
        KeyCode.Alpha8       => "<Keyboard>/8",
        KeyCode.Alpha9       => "<Keyboard>/9",
        KeyCode.Keypad0      => "<Keyboard>/numpad0",
        KeyCode.Keypad1      => "<Keyboard>/numpad1",
        KeyCode.Keypad2      => "<Keyboard>/numpad2",
        KeyCode.Keypad3      => "<Keyboard>/numpad3",
        KeyCode.Keypad4      => "<Keyboard>/numpad4",
        KeyCode.Keypad5      => "<Keyboard>/numpad5",
        KeyCode.Keypad6      => "<Keyboard>/numpad6",
        KeyCode.Keypad7      => "<Keyboard>/numpad7",
        KeyCode.Keypad8      => "<Keyboard>/numpad8",
        KeyCode.Keypad9      => "<Keyboard>/numpad9",
        _                    => $"<Keyboard>/{kc.ToString().ToLower()}"
    };
}
