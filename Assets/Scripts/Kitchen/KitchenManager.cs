// KitchenManager.cs
using System;
using System.Collections.Generic;
using UnityEngine;
using Game.Managers;
using Game.Currencies;

namespace Game.Kitchen
{
    [Serializable]
    public class InventorySlot
    {
        public int slotId;
        public string toolId;     // 1칸 = 1개(스택X)
    }

    [Serializable]
    public class PlacedInfo
    {
        public int placedId;
        public string toolId;
        public GameObject instance;  // 씬에 설치된 오브젝트
    }

    public class KitchenManager : MonoBehaviour, IShopCatalogProvider, IShopPurchaseProvider
    {
        public static KitchenManager Instance { get; private set; }

        [Header("Catalog")]
        [SerializeField] private KitchenCatalog catalog;

        [Header("Inventory")]
        [SerializeField, Tooltip("인벤 최대 7칸")]
        private int maxInventory = 7;
        private readonly List<InventorySlot> _inventory = new();
        private int _nextInvId = 1;

        [Header("Placed")]
        private readonly List<PlacedInfo> _placed = new();
        private int _nextPlacedId = 1;

        // 배치 컨트롤러 (고스트/충돌체크/확정/취소/집으로)
        [Header("Placement")]
        [SerializeField] private KitchenPlacementController placementController;
        [SerializeField] private GameObject defaultPlaceablePrefab; // = PlaceableTool.prefab

        [Header("Bubble Prefab)")]
        [SerializeField] private RectTransform bubblePanel;
         [SerializeField] private GameObject bubbleBtnPrefab_gold;
         [SerializeField] private GameObject bubbleBtnPrefab_ice;
        // 배치 금지 구역/이미 설치물 레이어 마스크
        [Header("Masks")]
        [SerializeField] private LayerMask blockMask;   // (핑크벽, 인벤 하단 등)
        [SerializeField] private LayerMask placedMask;  // 이미 설치된 도구들

        // 이벤트
        public event Action<InventorySlot> OnInventoryAdded;
        public event Action<int> OnInventoryRemoved;         // slotId
        public event Action<PlacedInfo> OnPlaced;
        public event Action<int> OnRemovedFromScene;         // placedId

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            Debug.Log($"[KitchenManager] awake on {gameObject.name} (scene={gameObject.scene.name})", gameObject);
        }

        public KitchenItemData GetData(string toolId) => catalog?.Get(toolId);

        // ====== 조회 API ======
        public bool HasInstalled(string toolId, int minCount = 1)
        {
            int c = 0;
            for (int i = 0; i < _placed.Count; i++)
                if (_placed[i].toolId == toolId && _placed[i].instance != null) c++;
            return c >= minCount;
        }

        public IReadOnlyList<InventorySlot> GetInventory() => _inventory;
        public IReadOnlyList<PlacedInfo> GetPlaced() => _placed;

        public int GetInventoryCapacity() => maxInventory;
        public int GetInventoryCount() => _inventory.Count;

        // ====== 구매 → 인벤토리 추가 (스택 없음) ======
        public bool TryBuyTool(string toolId)
        {
            var data = GetData(toolId);
            if (data == null) return false;
            if (_inventory.Count >= maxInventory) return false;

            var cm = CurrencyManager.Instance;
            if (cm != null && !cm.TrySpend(Game.Currencies.CurrencyType.Gold, data.priceGold))
                return false;

            var slot = new InventorySlot { slotId = _nextInvId++, toolId = toolId };
            _inventory.Add(slot);
            OnInventoryAdded?.Invoke(slot);
            return true;
        }

        // ====== 인벤 터치 → 배치모드 시작 ======

        public void BeginPlaceFromInventory(int slotId)
        {
            var idx = _inventory.FindIndex(s => s.slotId == slotId);
            if (idx < 0) return;

            var tool = GetData(_inventory[idx].toolId);
            if (tool == null) return;

            // 사용할 프리팹 결정(특정 도구만 예외 프리팹이 있으면 그것, 아니면 제네릭)
            var prefab = tool.prefabOverride ? tool.prefabOverride : defaultPlaceablePrefab;
            if (prefab == null)
            {
                Debug.LogError("[Kitchen] defaultPlaceablePrefab 미지정 또는 prefabOverride 없음");
                return;
            }

            placementController.BeginPreview(tool, blockMask, placedMask,
                onConfirm: (pos) =>
                {
                    // 설치 확정
                    var go = Instantiate(prefab, pos, Quaternion.identity);
                    var placedTool = go.GetComponent<PlaceableTool>();
                    if (placedTool)
                    {
                        var toolId = placedTool.GetComponent<LongPressRelocate>().toolId;
                        var bubblePrefab = toolId == "ice_maker" ? bubbleBtnPrefab_ice : bubbleBtnPrefab_gold;
                        placedTool.Init(bubblePanel, bubblePrefab, Camera.main);
                    }
                    go.layer = LayerMask.NameToLayer("UI");

                    // 도구 데이터로 외형/콜라이더/ID 세팅
                    var placeable = go.GetComponent<PlaceableTool>();
                    if (placeable) placeable.Setup(tool);

                    // 배치 레이어 지정(충돌 체크용)
                    SetLayerRecursively(go, LayerMaskToLayer(placedMask));

                    var placed = new PlacedInfo { placedId = _nextPlacedId++, toolId = tool.toolId, instance = go };
                    _placed.Add(placed);
                    OnPlaced?.Invoke(placed);

                    // 인벤에서 제거
                    _inventory.RemoveAt(idx);
                    OnInventoryRemoved?.Invoke(slotId);
                },
                onReturnHome: () =>
                {
                    // "집" 버튼: 그냥 배치모드 종료 (인벤 유지)
                });
        }

        // ====== 길게 눌러 이동/철거 모드 진입 ======
        public void BeginRelocate(GameObject targetInstance, string toolId)
        {
            if (!targetInstance) return;
            var idx = _placed.FindIndex(p => p.instance == targetInstance);
            if (idx < 0) return;

            var tool = GetData(toolId);
            if (tool == null) return;

            var renderers = targetInstance.GetComponentsInChildren<Renderer>(true);
            foreach (var r in renderers) r.enabled = false;
            var colliders = targetInstance.GetComponentsInChildren<BoxCollider2D>(true);
            foreach (var c in colliders) c.enabled = false;


            var startPos = targetInstance.transform.position;

            placementController.BeginPreview(tool, blockMask, placedMask,
                onConfirm: (pos) =>
                {
                    targetInstance.transform.position = pos;
                    foreach (var r in renderers) r.enabled = true;
                    foreach (var c in colliders) c.enabled = true;
                },
                onReturnHome: () =>
                {
                    // 씬에서 제거하고 인벤으로 회수
                    Destroy(targetInstance);
                    var removed = _placed[idx];
                    _placed.RemoveAt(idx);
                    OnRemovedFromScene?.Invoke(removed.placedId);

                    // 인벤 공간 있으면 회수, 없으면 드랍(= 소멸) 처리해도 되지만 우선 회수 실패시 로그
                    if (_inventory.Count < maxInventory)
                    {
                        var slot = new InventorySlot { slotId = _nextInvId++, toolId = toolId };
                        _inventory.Add(slot);
                        OnInventoryAdded?.Invoke(slot);
                    }
                    else
                    {
                        Debug.LogWarning("[Kitchen] 인벤토리 가득차서 회수 실패(소멸).");
                    }
                }, onCancel: () =>
                {
                    foreach (var r in renderers) r.enabled = true;
                    foreach (var c in colliders) c.enabled = true;
                },
                startPos: startPos
                );
        }


        // ====== IShopCatalogProvider ======
        public bool TryGetPrice(string id, out int price)
        {
            var d = GetData(id);
            price = d != null ? d.priceGold : 0;
            return d != null;
        }
        public string GetDisplayName(string id) => GetData(id)?.displayName;
        public Sprite GetIcon(string id) => GetData(id)?.icon;

        // ====== IShopPurchaseProvider ======
        // 인터페이스 표준 메서드 이름은 TryBuy(string)
        public bool TryBuy(string id) => TryBuyTool(id);

        // ====== 유틸 ======
        static int LayerMaskToLayer(LayerMask mask)
        {
            int layerNumber = 0;
            int maskValue = mask.value;
            while (maskValue > 1) { maskValue >>= 1; layerNumber++; }
            return layerNumber;
        }

        static void SetLayerRecursively(GameObject go, int layer)
        {
            go.layer = layer;
            foreach (Transform t in go.transform) SetLayerRecursively(t.gameObject, layer);
        }
    }
}
