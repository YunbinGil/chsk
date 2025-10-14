#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Linq;

public static class FindBadGUIStyle
{
    [MenuItem("Tools/Scan/Find GUIStyle risky usages")]
    public static void Scan()
    {
        var guids = AssetDatabase.FindAssets("t:TextAsset");
        int hits = 0;
        foreach (var g in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(g);
            if (!path.EndsWith(".cs")) continue;
            var txt = AssetDatabase.LoadAssetAtPath<TextAsset>(path)?.text;
            if (string.IsNullOrEmpty(txt)) continue;

            // 필드 초기화/정적 초기화에서 자주 쓰는 패턴들
            string[] needles = {
                "new GUIStyle(\"",  // 이름 GUIStyle
                "GUI.skin.FindStyle(", "GUI.skin.GetStyle(",
                "static GUIStyle ",  // 정적 GUIStyle
                "GUIStyle ",         // 필드 선언(대략)
            };

            if (needles.Any(n => txt.Contains(n)))
            {
                // Editor/ 폴더면 더 의심
                bool inEditor = path.Contains("/Editor/");
                Debug.Log($"[GUIStyle?] {(inEditor ? "[Editor]" : "")} {path}");
                hits++;
            }
        }
        Debug.Log($"Scan done. Suspects: {hits}");
    }
}
#endif
