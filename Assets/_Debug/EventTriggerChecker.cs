using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventTriggerChecker : MonoBehaviour
{
    IEnumerator Start()
    {
        yield return null; // SettingController.Start() が終わるのを待つ

        var obj = GameObject.Find("move_forward");
        if (obj == null) { Debug.LogError("[Checker] move_forward not found"); yield break; }

        var trigger = obj.GetComponent<EventTrigger>();
        if (trigger == null) { Debug.LogError("[Checker] EventTrigger not found"); yield break; }

        if (trigger.triggers.Count == 0) { Debug.LogError("[Checker] triggers list is empty"); yield break; }

        var cb = trigger.triggers[0].callback;
        var field = typeof(UnityEngine.Events.UnityEventBase)
            .GetField("m_Calls", BindingFlags.NonPublic | BindingFlags.Instance);
        var calls = field.GetValue(cb);
        var countProp = calls.GetType().GetProperty("Count");
        int count = (int)countProp.GetValue(calls);

        Debug.Log($"[Checker] move_forward EventTrigger[0] runtime listener count: {count}");
    }
}
