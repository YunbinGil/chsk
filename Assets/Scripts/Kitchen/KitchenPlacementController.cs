// KitchenPlacementController.cs
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Kitchen
{
    public class KitchenPlacementController : MonoBehaviour
    {
        [Header("Ghost/Overlay")]
        [SerializeField] private GameObject ghostPrefab;      // 240x240 스프라이트 프리팹(컬러 변경 가능한 SpriteRenderer)
        [SerializeField] private Color okColor   = new Color(0f, 1f, 0f, 0.35f);
        [SerializeField] private Color badColor  = new Color(1f, 0f, 0f, 0.35f);

        [Header("Buttons (V / X / Home)")]
        [SerializeField] private GameObject buttonsRowPrefab; // 세 개 버튼 수평 정렬된 UI(월드캔버스)
        [SerializeField] private Vector3 buttonsOffset = new Vector3(0, -1.6f, 0);

        Camera _cam;
        GameObject _ghost;
        SpriteRenderer _ghostSr;
        GameObject _buttonsRow;

        KitchenItemData _current;
        LayerMask _blockMask, _placedMask;

        Action<Vector3> _onConfirm;
        Action _onReturnHome;

        bool _active;
        bool _isOk;
        Vector3 _lastPos;
        bool _frozen;    // 프리뷰 이동 잠금
        Vector3 _frozenPos;        // 버튼/고스트 고정 위치
        
        Action _onCancel;
        void Awake() { _cam = Camera.main; }

        public void BeginPreview(
            KitchenItemData data,
            LayerMask blockMask,
            LayerMask placedMask,
            Action<Vector3> onConfirm,
            Action onReturnHome,
            Action onCancel = null,
            Vector3? startPos = null)
        {
            EndPreview(); // 안전 초기화

            _current = data;
            _blockMask = blockMask;
            _placedMask = placedMask;
            _onConfirm = onConfirm;
            _onReturnHome = onReturnHome;
            _onCancel    = onCancel;

            _ghost = Instantiate(ghostPrefab);
            _ghostSr = _ghost.GetComponentInChildren<SpriteRenderer>();
            if (_ghostSr && data.icon) _ghostSr.sprite = data.icon;

            _lastPos = startPos ?? ( _cam ? (Vector3)_cam.ScreenToWorldPoint(Input.mousePosition) : Vector3.zero );
            _frozen = false;
            _lastPos.z = 0f;
            if (_ghost) _ghost.transform.position = _lastPos;
            _active = true;
        }

        public void EndPreview()
        {
            _active = false;
            if (_ghost) Destroy(_ghost);
            if (_buttonsRow) Destroy(_buttonsRow);
            _ghost = null; _ghostSr = null; _buttonsRow = null;
            _current = null;
            _onConfirm = null; _onReturnHome = null;
        }

        void Update()
        {
            if (!_active || _current == null) return;

            // 마우스 위치에 고스트 따라가기
            if (!_frozen)
            {
                Vector3 m = Input.mousePosition;
                if (_cam) _lastPos = _cam.ScreenToWorldPoint(new Vector3(m.x, m.y, Mathf.Abs(_cam.transform.position.z)));
                _lastPos.z = 0f;
                if (_ghost) _ghost.transform.position = _lastPos;
            }

            // 충돌 체크
            _isOk = CheckPlaceable(_lastPos, _current.footprint, _blockMask, _placedMask);
            if (_ghostSr) _ghostSr.color = _isOk ? okColor : badColor;

            // 좌클릭: 버튼 토글
            if (Input.GetMouseButtonDown(0) && !PointerOverUI())
            {
                ToggleButtons();
            }
        }

        bool PointerOverUI() => EventSystem.current && EventSystem.current.IsPointerOverGameObject();

        void ToggleButtons()
        {
            if (_buttonsRow) {
                Destroy(_buttonsRow); _buttonsRow = null;
                _frozen = false; 
                return; }

            _frozen = true;
            _frozenPos = _lastPos;

            if (_ghost) _ghost.transform.position = _frozenPos;

            _buttonsRow = Instantiate(buttonsRowPrefab);

            var binder = _buttonsRow.GetComponentInChildren<ButtonsRowBinder>(true);
            var canvas = binder ? binder.canvas : _buttonsRow.GetComponentInParent<Canvas>();
            var content = _buttonsRow.transform.Find("Content") as RectTransform;

            if (canvas && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                // Overlay → 스크린 포인트 → 로컬 포인트 변환 후 Content에 대입
                Vector2 screen = _cam.WorldToScreenPoint(_frozenPos + buttonsOffset);
                RectTransform canvasRt = canvas.GetComponent<RectTransform>();

                Vector2 local;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRt, screen, null, out local);
                content.anchoredPosition = local;         // ★ 핵심
            }

            // 자식 버튼: OK, CANCEL, HOME 순서라고 가정
            var ok     = binder ? binder.okBtn : null;
            var cancel = binder ? binder.cancelBtn : null;
            var home = binder ? binder.homeBtn : null;

            ok.onClick.AddListener(() =>
            {
                if (_isOk)
                {
                    _onConfirm?.Invoke(_frozenPos);
                    EndPreview();
                }
            });
            cancel.onClick.AddListener(() =>
            {
                // X: 버튼 닫고 계속 프리뷰 상태 유지
                if (_buttonsRow) { Destroy(_buttonsRow); _buttonsRow = null; }
                    _frozen = false;
                    _onCancel?.Invoke(); 
            });
            home.onClick.AddListener(() =>
            {
                // 집: 인벤으로 되돌리기
                _onReturnHome?.Invoke();
                EndPreview();
            });
        }

        // 오버랩 박스 충돌 검사
        bool CheckPlaceable(Vector3 center, Vector2 size, LayerMask block, LayerMask placed)
        {
            // 금지구역
            var hit1 = Physics2D.OverlapBoxAll(center, size, 0f, block);
            if (hit1 != null && hit1.Length > 0) return false;

            // 기존 설치물
            var hit2 = Physics2D.OverlapBoxAll(center, size, 0f, placed);
            if (hit2 != null && hit2.Length > 0) return false;

            return true;
        }

        // 에디터에서 박스 확인용
        void OnDrawGizmosSelected()
        {
            if (_active && _current != null)
            {
                Gizmos.color = _isOk ? Color.green : Color.red;
                Gizmos.DrawWireCube(_lastPos, _current.footprint);
            }
        }
    }
}
