// IceProductionController.cs
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace chsk.GamePlay.Production
{
    public class IceProductionController : MonoBehaviour
    {
        public enum ProdStatus { Idle, Generating, Done }

        public event Action<ProdStatus> OnStatusChanged;

        public static readonly HashSet<IceProductionController> All = new();
        public static event System.Action<IceProductionController, bool> OnRegistryChanged;

        [SerializeField] private string itemId;  // 현재 선택된 생산 아이템
        public string ItemId => itemId;

        ProdStatus _status = ProdStatus.Idle;
        public ProdStatus Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnStatusChanged?.Invoke(_status);
                }
            }
        }

        Coroutine _routine;

        public void SetItemId(string id) => itemId = id;

        void OnEnable()
        {
                All.Add(this);
                OnRegistryChanged?.Invoke(this, true);
        }

        void OnDisable()
        {
                All.Remove(this);
                OnRegistryChanged?.Invoke(this, false);
           
        }
        public void BeginProduction(int seconds)
        {
            if (_routine != null) StopCoroutine(_routine);
            Status = ProdStatus.Generating;
            _routine = StartCoroutine(Run(seconds));
        }

        IEnumerator Run(int seconds)
        {
            float end = Time.time + Mathf.Max(1, seconds);
            while (Time.time < end) yield return null;
            Status = ProdStatus.Done;
            _routine = null;
        }

        public static bool HasIdleOtherThan(IceProductionController except = null)
        {
            foreach (var c in All)
                if (c && c != except && c.Status == ProdStatus.Idle)
                    return true;
            return false;
        }
    }
}
