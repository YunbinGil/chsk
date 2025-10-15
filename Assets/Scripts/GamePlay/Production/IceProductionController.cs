// IceProductionController.cs
using UnityEngine;
using System;
using System.Collections;

namespace chsk.GamePlay.Production
{
    public class IceProductionController : MonoBehaviour
    {
        public enum ProdStatus { Idle, Generating, Done }

        public event Action<ProdStatus> OnStatusChanged;

        [SerializeField] private string itemId;  // 현재 선택된 생산 아이템
        public string ItemId => itemId;

        public ProdStatus Status { get; set; } = ProdStatus.Idle;

        Coroutine _routine;

        public void SetItemId(string id) => itemId = id;

        public void BeginProduction(int seconds)
        {
            if (_routine != null) StopCoroutine(_routine);
            Status = ProdStatus.Generating;
            OnStatusChanged?.Invoke(Status);
            _routine = StartCoroutine(Run(seconds));
        }

        IEnumerator Run(int seconds)
        {
            float end = Time.time + Mathf.Max(1, seconds);
            while (Time.time < end) yield return null;
            Status = ProdStatus.Done;
            OnStatusChanged?.Invoke(Status);
            _routine = null;
        }
    }
}
