using UnityEngine;
using Work.ETC.Effect.Code;

namespace Work.Core.Utils.Feedbacks
{
    public class EffectCreateFeedback : Feedback
    {
        [SerializeField] private EffectPlayer particlePrefab;

        private EffectPlayer particle;

        public override void CreateFeedback()
        {
            particle = Instantiate(particlePrefab, transform.position, Quaternion.identity);
            particle.Play();
        }

        public override void StopFeedback()
        {
            particle.Stop();
            particle = null;
        }
    }
}