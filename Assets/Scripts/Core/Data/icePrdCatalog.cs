using System;
using System.Collections.Generic;
using UnityEngine;
using chsk.Core.Data;

namespace chsk.Core.Data
{
[CreateAssetMenu(menuName = "MYBAR/Ice Production Catalog", fileName = "IcePrdCatalog")]

    public class IcePrdCatalog : ScriptableObject
    {
        public List<IcePrdItemData> items = new List<IcePrdItemData>();
        public IcePrdItemData Get(string itemId) => items.Find(x => x.id == itemId);
    }

    [Serializable]
    public class IcePrdItemData
    {
        public string id;          // ICE:<makerLv>:<prdIce> e.g. 1:10
        public string displayName;     // 표시명
        public int priceGold = 0;      // 구매가
        public int prdIce = 0;         // 생산량
        public Duration time;          // 소요 시간
        public Sprite icon;            // 생산기에서 보일 아이콘
        public int TimeSec => time.ToSeconds(); //런타임용 초단위 저장
    }
}