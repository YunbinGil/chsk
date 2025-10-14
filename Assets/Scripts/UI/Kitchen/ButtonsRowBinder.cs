// ButtonsRowBinder.cs
using UnityEngine;
using UnityEngine.UI;

namespace chsk.UI.Kitchen
{
    public class ButtonsRowBinder : MonoBehaviour
    {
        public Canvas canvas;          // 같은 오브젝트의 Canvas
        public Button okBtn;
        public Button cancelBtn;
        public Button homeBtn;

        void Reset()
        {
            if (!canvas) canvas = GetComponentInParent<Canvas>() ?? GetComponent<Canvas>();
            if (!okBtn || !cancelBtn || !homeBtn)
            {
                var btns = GetComponentsInChildren<Button>(true);
                // 이름 기준으로 자동 매핑 시도
                foreach (var b in btns)
                {
                    var n = b.name.ToLower();
                    if (n.Contains("ok")) okBtn = b;
                    else if (n.Contains("cancel") || n.Contains("x")) cancelBtn = b;
                    else if (n.Contains("home") || n.Contains("inven")) homeBtn = b;
                }
            }
        }
    }
}
