using UnityEngine;
using Work.Core.Utils.EventBus;

namespace Work.ETC.LocationUI.Code
{
    public class LocationUIPlayer : MonoBehaviour
    {
        [SerializeField] private string locationName;

        private bool _isPlayed;

        private void Awake()
        {
            Bus<PlayLocationUIEvent>.Events += HandlePlayLocationUIEvent;
        }

        private void OnDestroy()
        {
            Bus<PlayLocationUIEvent>.Events -= HandlePlayLocationUIEvent;
        }

        private void HandlePlayLocationUIEvent(PlayLocationUIEvent evt)
        {
            if (_isPlayed)
            {
                return;
            }

            _isPlayed = true;
            Bus<OnShowLocationNameEvent>.Raise(new OnShowLocationNameEvent(locationName));
            Destroy(gameObject);
        }
    }
}
