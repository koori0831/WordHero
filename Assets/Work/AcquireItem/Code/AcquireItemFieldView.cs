using UnityEngine;

namespace Work.AcquireItem.Code
{
    /// <summary>
    /// 획득 아이템 슬롯 표시 영역 뷰
    /// </summary>
    public class AcquireItemFieldView : MonoBehaviour
    {
        [SerializeField] private RectTransform rootTrm;
        [SerializeField] private AcquireItemSlotView slotPrefab;

        /// <summary>
        /// 슬롯 추가 처리
        /// </summary>
        public void AddSlot(string name, string type, Color color)
        {
            AcquireItemSlotView slot = Instantiate(slotPrefab, rootTrm);
            slot.SetInfo(name, type, color);
            slot.Init();
        }
    }
}
