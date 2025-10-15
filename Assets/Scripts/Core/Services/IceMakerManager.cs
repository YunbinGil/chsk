using System;
using System.Collections.Generic;
using UnityEngine;
using chsk.Core.Data;
using chsk.Core.Services;
using chsk.UI.IceMaker;

namespace chsk.Core.Services
{
    public class IceMakerManager : MonoBehaviour
    {
        public static IceMakerManager Instance { get; private set; }

        [Header("Catalog")]
        [SerializeField] private IcePrdCatalog catalog;


        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log($"[IceMakerManager] awake on {gameObject.name} (scene={gameObject.scene.name})", gameObject);
        }

        public IReadOnlyList<IcePrdItemData> GetCatalog() => catalog.items;
        
        public IcePrdItemData GetData(string itemId)
            => catalog.items.Find(d => d.id == itemId);

        //생산 로직
        public bool TryGenerate(string itemId)
        {
            var data = GetData(itemId);
            if (data == null) return false;

            //결제
            var cm = CurrencyManager.Instance;
            if (cm != null && !cm.TrySpend(CurrencyType.Gold, data.priceGold)) return false;
            return true;

        }
        public bool TryGetPrice(string itemId, out int price)
        {
            var d = GetData(itemId);
            price = d != null ? d.priceGold : 0;
            return d != null;
        }
        public Sprite GetIcon(string itemId) => GetData(itemId)?.icon;
        public int GetPrdIce(string itemId) => GetData(itemId).prdIce;
        public string GetTime(string itemId) => GetData(itemId)?.time.ToString();

    }
}