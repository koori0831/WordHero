using LitMotion;
using UnityEngine;
using UnityEngine.UI;
using Work.Core.Utils.EventBus;

namespace Work.Fade
{
    public class FadeObject : MonoBehaviour
    {
        [SerializeField] private Image fadeObject;
        [SerializeField] private float fadeDuration = 1.0f;

        private MotionHandle _handle;

        public void Awake()
        {
            Bus<OnFadeEvent>.Events += HandleFadeEvent;



            _handle = LMotion.Create(1f, 0f, fadeDuration)
                    .Bind(a =>
                    {
                        Color co = new Color(fadeObject.color.r, fadeObject.color.b, fadeObject.color.g, a);
                        fadeObject.color = co;
                    })
                    .AddTo(gameObject);
        }


        private void HandleFadeEvent(OnFadeEvent evt)
        {
            _handle.TryCancel();

            if (evt.isFadeIn)
            {
                _handle = LMotion.Create(0f, 1f, fadeDuration)
                    .WithOnComplete(() => Bus<OnFadeCompletedEvent>.Raise(new OnFadeCompletedEvent(evt.isFadeIn)))
                    .Bind(a =>
                    {
                        Color co = new Color(fadeObject.color.r, fadeObject.color.b, fadeObject.color.g, a);
                        fadeObject.color = co;
                    })
                    .AddTo(gameObject);
            }
            else
            {
                _handle = LMotion.Create(1f, 0f, fadeDuration)
                    .WithOnComplete(() => Bus<OnFadeCompletedEvent>.Raise(new OnFadeCompletedEvent(evt.isFadeIn)))
                    .Bind(a =>
                    {
                        Color co = new Color(fadeObject.color.r, fadeObject.color.b, fadeObject.color.g, a);
                        fadeObject.color = co;
                    })
                    .AddTo(gameObject);
            }
        }

        public void OnDestroy()
        {
            Bus<OnFadeEvent>.Events -= HandleFadeEvent;
            _handle.TryCancel();
        }
    }
}