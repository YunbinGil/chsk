// ToastManager.cs
using UnityEngine;
using TMPro;
using System.Collections;

public class ToastManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup group;
    [SerializeField] private TMP_Text text;

    public void Show(string msg, float sec = 2f)
    {
        StopAllCoroutines();
        StartCoroutine(CoShow(msg, sec));
    }

    IEnumerator CoShow(string msg, float sec)
    {
        text.text = msg;
        group.alpha = 1f;
        yield return new WaitForSeconds(sec);
        group.alpha = 0f;
    }
}
