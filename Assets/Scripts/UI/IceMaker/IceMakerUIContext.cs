// Assets/Scripts/UI/IceMaker/IceMakerUIContext.cs
using UnityEngine;
using chsk.GamePlay.Production;

namespace chsk.UI.IceMaker
{
    // 간단 전역 컨텍스트: 현재 패널을 연 버블
    public static class IceMakerUIContext
    {
        public static IceProductionController CurrentController;
    }
}