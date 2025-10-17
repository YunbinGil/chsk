// Assets/Scripts/UI/IceMaker/QuickModeWatcher.cs
using UnityEngine;
using chsk.UI.IceMaker;
using chsk.GamePlay.Production;

public class QuickModeWatcher : MonoBehaviour
{
    void OnEnable()
    {
        IceMakerUIContext.OnQuickModeChanged += OnQuickChanged;
        IceProductionController.OnRegistryChanged += OnRegistryChanged;
        IceMakerUIContext.OnCurrentChanged += OnCurrentChanged;

        // 씬 시작 시 퀵모드가 이미 켜져있을 수도 있으니 한 번 정합 체크
        if (IceMakerUIContext.QuickModeActive)
            HookAllControllers(true);
    }

    void OnDisable()
    {
        IceMakerUIContext.OnQuickModeChanged -= OnQuickChanged;
        IceProductionController.OnRegistryChanged -= OnRegistryChanged;
        IceMakerUIContext.OnCurrentChanged -= OnCurrentChanged;
        HookAllControllers(false);
    }
    void OnCurrentChanged(IceProductionController c) // ★ 추가
{
    if (IceMakerUIContext.QuickModeActive) Reevaluate();
}


    void OnQuickChanged(bool active, string itemId)
    {
        // 퀵모드 켜질 때 모든 컨트롤러 상태 변화를 감시, 꺼지면 전부 해제
        HookAllControllers(active);
        if (active) Reevaluate(); // 켜진 직후에도 한번 평가해서 필요 없으면 즉시 끄기
    }

    void OnRegistryChanged(IceProductionController c, bool added)
    {
        if (!IceMakerUIContext.QuickModeActive || c == null) return;
        // 퀵모드 중 새 컨트롤러가 생기면 그 컨트롤러만 훅 추가
        c.OnStatusChanged -= OnAnyControllerStatus;
        if (added) c.OnStatusChanged += OnAnyControllerStatus;
        // 변화가 생겼으니 재평가
        Reevaluate();
    }

    void HookAllControllers(bool hook)
    {
        foreach (var c in IceProductionController.All)
        {
            if (!c) continue;
            c.OnStatusChanged -= OnAnyControllerStatus;
            if (hook) c.OnStatusChanged += OnAnyControllerStatus;
        }
    }

    void OnAnyControllerStatus(IceProductionController.ProdStatus _)
    {
        Reevaluate();
    }

    void Reevaluate()
    {
        var cur = IceMakerUIContext.CurrentController; // null이면 안전하게 즉시 종료해도 됨
        var toolId = cur ? cur.ToolId : null;

        if (string.IsNullOrEmpty(toolId))
        {
            // 기준 라인을 알 수 없으면 보수적으로 끈다(혹은 전역 판단으로 유지하고 싶으면 아래 줄을 바꿔)
            IceMakerUIContext.StopQuickMode();
            return;
        }

        if (!IceProductionController.HasIdleForTool(toolId))
            IceMakerUIContext.StopQuickMode();
    }
}
