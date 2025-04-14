using UnityEngine.UI;

public class NotInteractiveScrollRect : ScrollRect
{
    // ScrollRect에서 입력을 차단
    public override void OnBeginDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {
        // 입력 무시
    }

    public override void OnDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {
        // 입력 무시
    }

    public override void OnEndDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {
        // 입력 무시
    }
}
