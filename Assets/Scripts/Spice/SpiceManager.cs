using System;
using System.Collections.Generic;
using UnityEngine;
using Game.Currencies; // CurrencyType.Gold
using Game.Managers;

// === 캐비닛 슬롯(스택) ===
[Serializable]
public class CabinetSlot
{
    public int slotId;        // 고유 슬롯 ID
    public string spiceId;    // 어떤 향신료인지
    public int count;         // 0~maxPerSlot
}

// === 매니저 ===
public class SpiceManager : MonoBehaviour, IShopCatalogProvider, IShopPurchaseProvider
{
    public static SpiceManager Instance { get; private set; }

    [Header("Catalog (예: 12종)")]
    [SerializeField] private SpiceCatalog catalog;

    [Header("Capacity")]
    [SerializeField] private int maxSlots = 8;     // 캐비닛 총 칸 수
    [SerializeField] private int maxPerSlot = 10;   // 슬롯당 보유 한도

    // 인벤토리(슬롯 리스트)
    private readonly List<CabinetSlot> _slots = new List<CabinetSlot>();
    private int _nextSlotId = 1;

    // 이벤트
    public event Action<CabinetSlot> OnSlotAdded;     // 새 프리팹 생성 필요
    public event Action<CabinetSlot> OnSlotUpdated;   // 기존 셀 UI 갱신
    public event Action<int> OnSlotRemoved;   // slotId

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Debug.Log($"[SpiceManager] awake on {gameObject.name} (scene={gameObject.scene.name})", gameObject);

        // TODO: 필요 시 PlayerPrefs 등에서 로드
        // (샘플: 비어있는 상태로 시작)
    }

    // 외부 접근용
    public IReadOnlyList<CabinetSlot> GetAllSlots() => _slots;
    public IReadOnlyList<SpiceData> GetCatalog() => catalog.items;

    public SpiceData GetData(string spiceId)
        => catalog.items.Find(d => d.spiceId == spiceId);

    public int GetMaxPerSlot() => maxPerSlot;
    public int GetMaxSlots() => maxSlots;

    // 규칙 1/2/3을 모두 만족하는 구매 로직
    /// 1. 캐비넷에 없음 -> 이미지 prefab생성 
    /// 2. 캐비넷에 있음, 10개 안참 -> 보유개수 데이터 수정, ui 바인딩 
    /// 3. 캐비넷에 잇는데 10개 다참 -> 새로 이미지 prefab 생성 해야됨
    public bool TryBuy(SpiceData data, int amount = 1)
    {
        if (data == null || amount <= 0) return false;

        // 결제
        int totalCost = data.priceGold * amount; 
        var cm = CurrencyManager.Instance; // Game.Managers.CurrencyManager // null도 가능
        if (cm != null&& !cm.TrySpend(CurrencyType.Gold, totalCost))
                return false;
        
        

        for (int k = 0; k < amount; k++)
        {
            // 2) 같은 향신료 슬롯 중 count<maxPerSlot 있으면 → 그 슬롯 증가
            var slot = _slots.Find(s => s.spiceId == data.spiceId && s.count < maxPerSlot);
            if (slot != null)
            {
                slot.count++;
                OnSlotUpdated?.Invoke(slot);
                continue;
            }

            // 1 or 3) 없으면 새 슬롯 필요. 공간이 없으면 실패
            if (_slots.Count >= maxSlots)
                return false;

            var newSlot = new CabinetSlot
            {
                slotId = _nextSlotId++,
                spiceId = data.spiceId,
                count = 1
            };
            _slots.Add(newSlot);
            OnSlotAdded?.Invoke(newSlot);
        }
        return true;
    }

    // 0 되면 리스트에서 제거 + 이벤트 브로드캐스트
    public bool ConsumeOne(int slotId)
    {
        var idx = _slots.FindIndex(s => s.slotId == slotId);
        if (idx < 0) return false;

        var slot = _slots[idx];
        if (slot.count <= 0) return false;

        slot.count--;

        if (slot.count <= 0)
        {
            _slots.RemoveAt(idx); // 리스트에서 제거
            OnSlotRemoved?.Invoke(slotId); // UI 쪽에 프리팹 파괴 지시
        }
        else
        {
            OnSlotUpdated?.Invoke(slot);
        }
        return true;
    }

    // === IShopCatalogProvider 구현 ===
    public bool TryGetPrice(string itemId, out int price)
    {
        var d = GetData(itemId);
        price = d != null ? d.priceGold : 0;
        return d != null;
    }

    public string GetDisplayName(string itemId) => GetData(itemId)?.displayName;

    public Sprite GetIcon(string itemId) => GetData(itemId)?.icon;
    
    // ====== IShopPurchaseProvider ======
    // 상점 공용 호출에 맞추기: itemId만 받음
    public bool TryBuy(string id)
    {
        var d = GetData(id);
        if (d == null) return false;
        return TryBuy(d, 1);
    }
}
