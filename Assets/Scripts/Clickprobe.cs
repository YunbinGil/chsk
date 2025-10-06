using UnityEngine;
using UnityEngine.EventSystems;

public class Clickprobe : MonoBehaviour
{
    public void OnPointerClick(PointerEventData e)
    {
        Debug.Log($"[ClickProbe] clicked {name} by {e.button}");
    }
}
    