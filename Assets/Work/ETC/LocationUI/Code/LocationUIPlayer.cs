using UnityEngine;
using Work.Core.Utils.EventBus;

namespace Work.ETC.LocationUI.Code
{
    public class LocationUIPlayer : MonoBehaviour
    {
        [SerializeField] private string locationName;
        [SerializeField] private float delayTime = 1.0f;

        public void Start()
        {
            Destroy(gameObject, delayTime);
        }

        private void OnDestroy()
        {
            Debug.Log("LocationUIPlayer OnDestroy");
            Bus<OnShowLocationNameEvent>.Raise(new OnShowLocationNameEvent(locationName));
        }
    }
}