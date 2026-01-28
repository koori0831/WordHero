using Unity.Cinemachine;
using UnityEngine;

namespace Work.Core.Utils.Feedbacks
{
    [RequireComponent(typeof(CinemachineImpulseSource))]
    public class CameraImpulsFeedback : Feedback
    {
        private CinemachineImpulseSource impulseSource;
        [SerializeField] private float force = 0.1f;
        private void Awake()
        {
            impulseSource = GetComponent<CinemachineImpulseSource>();
        }
        public override void CreateFeedback()
        {
            impulseSource.GenerateImpulse(force);
        }

        public override void StopFeedback()
        {
            //impulseSource.
        }
    }
}
