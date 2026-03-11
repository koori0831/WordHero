using Alchemy.Inspector;
using System.Collections.Generic;
using UnityEngine;

namespace Work.ETC.Effect.Code
{
    public class EffectPlayer : MonoBehaviour
    {
        [SerializeField] private bool isRange = false;
        [ShowIf(nameof(isRange))] [SerializeField] private float range = 1f;

        private List<ParticleSystem> paticles = new List<ParticleSystem>();


        private void Awake()
        {
            GetComponentsInChildren(paticles);


            foreach (ParticleSystem particle in paticles)
            {
                if (isRange)
                {
                    particle.transform.localPosition += new Vector3((Random.value - 0.5f * 2) * range, (Random.value - 0.5f * 2) * range, (Random.value - 0.5f * 2) * range);
                }

                particle.Stop();
            }
        }

        public void Play()
        {
            foreach (ParticleSystem particle in paticles)
            {
                particle.Play();
            }
        }

        internal void Stop()
        {
            foreach (ParticleSystem particle in paticles)
            {
                particle.Stop();
            }

            Destroy(gameObject);
        }
    }
}