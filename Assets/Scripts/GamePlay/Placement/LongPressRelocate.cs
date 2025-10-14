// LongPressRelocate.cs
using UnityEngine;
using UnityEngine.Events;
using chsk.Core.Services;

namespace chsk.Gameplay.Placement{
    public class LongPressRelocate : MonoBehaviour
    {
        [SerializeField] public string toolId;
        [SerializeField] private float holdSec = 0.5f;

        float _downT;
        bool _pressing;

        void OnMouseDown()  { _pressing = true; _downT = Time.time; }
        void OnMouseUp()    { _pressing = false; }

        void Update()
        {
            if (_pressing && Time.time - _downT >= holdSec)
            {
                _pressing = false; // 한 번만
                KitchenManager.Instance?.BeginRelocate(gameObject, toolId);
            }
        }

        // 설치 프리팹 루트에 붙이고, 해당 프리팹의 toolId 셋업
        public void SetToolId(string id) => toolId = id;
    }
}
