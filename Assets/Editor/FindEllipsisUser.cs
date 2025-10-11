// FindEllipsisUsers.cs (Assets/Editor/FindEllipsisUsers.cs)
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using TMPro;

public static class FindEllipsisUsers
{
    [MenuItem("Tools/TMP/Log Ellipsis Users In Scene")]
    public static void LogEllipsisUsers()
    {
        var texts = Object.FindObjectsByType<TMP_Text>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var t in texts)
        {
            if (t.overflowMode == TextOverflowModes.Ellipsis)
            {
                Debug.Log($"[Ellipsis] {GetPath(t.transform)} | Font={t.font?.name}", t);
            }
        }
        Debug.Log($"Scanned {texts.Length} TMP_Text objects.");
    }

    static string GetPath(Transform tr)
    {
        string path = tr.name;
        while (tr.parent) { tr = tr.parent; path = tr.name + "/" + path; }
        return path;
    }
}
#endif
