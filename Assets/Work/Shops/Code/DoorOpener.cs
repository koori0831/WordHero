using UnityEngine;
using Work.Core.Utils.EventBus;
using Work.Stages.Code;

namespace Work.Shops.Code
{
    public class DoorOpener : MonoBehaviour
    {
        private bool _isOpen = false;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && !_isOpen)
            {
                _isOpen = true;
                Bus<StageClearEvent>.Raise(new StageClearEvent());
            }
        }
    }
}