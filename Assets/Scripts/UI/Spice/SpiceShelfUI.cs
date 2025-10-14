using System.Collections.Generic;
using UnityEngine;
using chsk.Core.Services;

namespace chsk.UI.Spice
{
    // Panel.Spice/Content 에 부착
    public class SpiceShelfUI : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private SpiceManager manager;       // 비워두면 자동 연결
        [SerializeField] private Transform gridParent;       // 비워두면 this.transform
        [SerializeField] private GameObject shelfCellPrefab; // ShelfCell 프리팹

        // slotId -> 생성된 셀 컴포넌트
        private readonly Dictionary<int, SpiceCell> _cells = new();

        void Awake()
        {
            if (!manager) manager = SpiceManager.Instance;
            if (!gridParent) gridParent = transform;
        }

        void OnEnable()
        {
            if (manager == null) return;

            manager.OnSlotAdded += HandleAdded;
            manager.OnSlotUpdated += HandleUpdated;
            manager.OnSlotRemoved += HandleRemoved;

            ClearUI();

            // 초기 렌더(씬에 이전 상태가 있으면 그려줌)
            foreach (var s in manager.GetAllSlots())
                HandleAdded(s);
        }

        private void ClearUI()
        {
            for (int i = gridParent.childCount - 1; i >= 0; --i)
                Destroy(gridParent.GetChild(i).gameObject);

            _cells.Clear();
        }

        void OnDisable()
        {
            if (manager == null) return;

            manager.OnSlotAdded -= HandleAdded;
            manager.OnSlotUpdated -= HandleUpdated;
            manager.OnSlotRemoved -= HandleRemoved;
        }

        private void HandleAdded(CabinetSlot slot)
        {
            if (_cells.TryGetValue(slot.slotId, out var existing) && existing)
            {
                existing.SetCount(slot.count);
                return;
            }

            var go = Instantiate(shelfCellPrefab, gridParent);
            var cell = go.GetComponent<SpiceCell>() ?? go.AddComponent<SpiceCell>();

            var data = manager.GetData(slot.spiceId);
            cell.BindSlot(slot.slotId, data, manager.GetMaxPerSlot);
            cell.SetCount(slot.count);

            _cells[slot.slotId] = cell;
        }
        private void HandleUpdated(CabinetSlot slot)
        {
            if (_cells.TryGetValue(slot.slotId, out var cell))
                cell.SetCount(slot.count);
        }

        private void HandleRemoved(int slotId)
        {
            if (_cells.TryGetValue(slotId, out var cell) && cell)
            {
                Destroy(cell.gameObject);   // 프리팹 파괴
                _cells.Remove(slotId);      // 딕셔너리 정리
            }
        }
    }
}
