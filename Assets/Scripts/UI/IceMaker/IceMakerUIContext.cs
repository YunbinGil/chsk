// Assets/Scripts/UI/IceMaker/IceMakerUIContext.cs
using UnityEngine;
using chsk.GamePlay.Production;
using System;

namespace chsk.UI.IceMaker
{
    // 간단 전역 컨텍스트: 현재 패널을 연 버블
    public static class IceMakerUIContext
    {
        public static IceProductionController CurrentController { get; private set; }
        public static event Action<IceProductionController> OnCurrentChanged;

        // === Quick Produce Mode ===
        public static bool QuickModeActive { get; private set; }
        public static string QuickItemId { get; private set; }

        public static event Action<bool, string> OnQuickModeChanged; // (active, itemId)

        public static void StartQuickMode(string itemId)
        {
            QuickItemId = itemId;
            QuickModeActive = true;
            OnQuickModeChanged?.Invoke(true, QuickItemId);
        }

        public static void StopQuickMode()
        {
            QuickModeActive = false;
            QuickItemId = null;
            OnQuickModeChanged?.Invoke(false, null);
        }
        public static void SetCurrent(IceProductionController c)
        {
            if (CurrentController == c) return;
            CurrentController = c;
            OnCurrentChanged?.Invoke(CurrentController);
        }

        public static void Clear()
        {
            if (CurrentController == null) return;
            CurrentController = null;
            OnCurrentChanged?.Invoke(null);
        }
    }
}