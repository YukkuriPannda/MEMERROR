using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

/// <summary>
/// Tools > Create HP Bar でシーンに uGUI HP バー一式を生成する。
/// 選択中の GameObject があればその子に追加、なければシーンルートに配置。
/// </summary>
public static class HPBarSetup
{
    const string BG_PATH   = "Assets/Game/Sprites/HPBar@2x.png";
    const string FILL_PATH = "Assets/Game/Sprites/HPBar_fill@2x.png";

    [MenuItem("Tools/Create HP Bar")]
    static void CreateHPBar()
    {
        var bgSprite   = AssetDatabase.LoadAssetAtPath<Sprite>(BG_PATH);
        var fillSprite = AssetDatabase.LoadAssetAtPath<Sprite>(FILL_PATH);

        if (bgSprite == null || fillSprite == null)
        {
            Debug.LogError($"スプライトが見つかりません。\n{BG_PATH}\n{FILL_PATH}");
            return;
        }

        // ── Canvas (World Space) ──────────────────────────────
        var canvasGO = new GameObject("HPBar_Canvas");
        var canvas   = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        var canvasRT     = canvasGO.GetComponent<RectTransform>();
        canvasRT.sizeDelta  = new Vector2(200f, 30f);
        canvasRT.localScale = Vector3.one * 0.01f;

        // ── HPBar (root) ──────────────────────────────────────
        var hpBarGO = new GameObject("HPBar");
        hpBarGO.transform.SetParent(canvasGO.transform, false);
        var hpBarRT       = hpBarGO.AddComponent<RectTransform>();
        hpBarRT.anchorMin = Vector2.zero;
        hpBarRT.anchorMax = Vector2.one;
        hpBarRT.offsetMin = Vector2.zero;
        hpBarRT.offsetMax = Vector2.zero;
        var hpBar = hpBarGO.AddComponent<HPBar>();

        // ── background ────────────────────────────────────────
        var bgGO   = CreateFullStretchChild(hpBarGO, "background");
        var bgImage = bgGO.AddComponent<Image>();
        bgImage.sprite = bgSprite;
        bgImage.type   = Image.Type.Simple;

        // ── red (ダメージ残像) ────────────────────────────────
        var redGO    = CreateFullStretchChild(hpBarGO, "red");
        var redImage = redGO.AddComponent<Image>();
        redImage.sprite     = fillSprite;
        redImage.type       = Image.Type.Filled;
        redImage.fillMethod = Image.FillMethod.Horizontal;
        redImage.fillOrigin = (int)Image.OriginHorizontal.Left;
        redImage.fillAmount = 1f;
        redImage.color      = new Color(0.85f, 0.15f, 0.15f);

        // ── fill ──────────────────────────────────────────────
        var fillGO    = CreateFullStretchChild(hpBarGO, "fill");
        var fillImage = fillGO.AddComponent<Image>();
        fillImage.sprite     = fillSprite;
        fillImage.type       = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Horizontal;
        fillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
        fillImage.fillAmount = 1f;

        // ── HPBar にリファレンスを注入 ────────────────────────
        var so = new SerializedObject(hpBar);
        so.FindProperty("fillImage").objectReferenceValue = fillImage;
        so.FindProperty("redImage").objectReferenceValue  = redImage;
        so.ApplyModifiedProperties();

        // ── 親設定・Undo 登録 ─────────────────────────────────
        var selected = Selection.activeGameObject;
        if (selected != null)
            canvasGO.transform.SetParent(selected.transform, false);

        Undo.RegisterCreatedObjectUndo(canvasGO, "Create HP Bar");
        Selection.activeGameObject = canvasGO;

        Debug.Log("HP Bar (uGUI) を生成しました。HPBar の Hp Controller に HPController をアサインしてください。");
    }

    static GameObject CreateFullStretchChild(GameObject parent, string name)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        var rt       = go.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        return go;
    }
}
