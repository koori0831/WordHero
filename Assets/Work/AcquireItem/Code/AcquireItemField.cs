using System;
using UnityEngine;
using Work.Core.Utils.EventBus;

namespace Work.AcquireItem.Code
{
    public class AcquireItemField : MonoBehaviour
    {
        [SerializeField] private RectTransform rootTrm;
        [SerializeField] private AcquireItemFieldSlot slotPrefab;

        public void Awake()
        {
            Bus<OnGetItemEvent>.Events += OnGetItem;
        }

        private void OnDestroy()
        {
            Bus<OnGetItemEvent>.Events -= OnGetItem;
        }

        private void OnGetItem(OnGetItemEvent evt)
        {
            AddSlot(evt.Name, evt.Type, evt.Color);
        }

        public void AddSlot(string name, string type, Color color)
        {
            AcquireItemFieldSlot slot = Instantiate(slotPrefab, rootTrm);
            slot.SetInfo(name, type, color);
            slot.Init();
        }
    }
}