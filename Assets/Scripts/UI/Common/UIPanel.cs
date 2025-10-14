using UnityEngine;

namespace chsk.UI.Common
{
    public class UIPanel : MonoBehaviour
    {
        [SerializeField] CanvasGroup group;

        void Reset() { group = GetComponent<CanvasGroup>(); }

        public void Show(bool show, bool setActive = true)
        {
            if (setActive) gameObject.SetActive(true);
            group.alpha = show ? 1f : 0f;
            group.interactable = show;
            group.blocksRaycasts = show;
            if (setActive) gameObject.SetActive(show);
        }

        public void Open() => Show(true);
        public void Close() => Show(false);
    }
}