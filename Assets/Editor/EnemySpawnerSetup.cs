using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class EnemySpawnerSetup
{
    [MenuItem("Tools/Setup/EnemySpawner をシーンに配置")]
    static void Setup()
    {
        if (!EnsureGameScene()) return;

        GameObject existing = GameObject.Find("EnemySpawner");
        if (existing != null)
        {
            Debug.LogWarning("EnemySpawner はすでにヒエラルキーに存在します。");
            Selection.activeGameObject = existing;
            return;
        }

        GameObject go = new GameObject("EnemySpawner");
        EnemySpawner spawner = go.AddComponent<EnemySpawner>();

        EditorUtility.SetDirty(go);
        EditorSceneManager.MarkSceneDirty(go.scene);

        Selection.activeGameObject = go;
        Debug.Log("EnemySpawner をヒエラルキーに配置しました。Inspector で EnemyEntry を設定してください。");
    }

    static bool EnsureGameScene()
    {
        UnityEngine.SceneManagement.Scene scene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
        if (scene.name != "Game")
        {
            bool ok = EditorUtility.DisplayDialog(
                "シーン確認",
                $"現在のシーン「{scene.name}」に配置しますか？\n（Game シーンへ切り替えてから実行することを推奨します）",
                "このシーンに配置する",
                "キャンセル");
            return ok;
        }
        return true;
    }
}
