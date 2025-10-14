using System;
using System.Collections.Generic;
using UnityEngine;

namespace chsk.Core.Data
{
[CreateAssetMenu(menuName = "MYBAR/Spice Catalog", fileName = "SpiceCatalog")]

    public class SpiceCatalog : ScriptableObject
    {
        public List<SpiceData> items = new List<SpiceData>();
    }

    [Serializable]
    public class SpiceData
    {
        public string spiceId;      // "salt", "cinnamon" 등
        public string displayName;  // 표시 이름
        public int priceGold;       // 가격
        public Sprite icon;         // 아이콘
    }
}