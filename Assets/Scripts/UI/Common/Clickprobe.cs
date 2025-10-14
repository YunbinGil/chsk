using UnityEngine;
using UnityEngine.EventSystems;

namespace chsk.UI.Common
{
    public class Clickprobe : MonoBehaviour
    {
        public void OnPointerClick(PointerEventData e)
        {
            Debug.Log($"[ClickProbe] clicked {name} by {e.button}");
        }
    }
}
    