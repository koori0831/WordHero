using System;
using UnityEngine;
using Work.Core.Utils.Cameras;
using Work.Core.Utils.EventBus;
using Work.Input.Code;
using Work.Stages.Code;

namespace Work.Stages.Code
{
    [Serializable]
    public class IronCageEffecter : DoorEffect
    {
        [SerializeField] private AnimationCurve cageMoveAnimationCurve;
        [SerializeField] private float duration;
        [SerializeField] private float maxYPos;
        [SerializeField] private float changeValue = 4f;
        [SerializeField] private Transform ironCage_1, ironCage_2;

        [SerializeField] private bool nextDoor;

        private void Awake()
        {
            if (!nextDoor)
                Close();
        }


        public override async void Open()
        {

            float timer = 0;
            ironCage_1.gameObject.SetActive(true);
            ironCage_2.gameObject.SetActive(false);

            Bus<PlayerInputEnableEvent>.Raise(new PlayerInputEnableEvent(false));
            CameraController.Instance.PlayImpulse(0.45f, 0.2f);

            while (duration >= timer)
            {
                float normalizeTime = timer / duration;
                float currentYPos = maxYPos * cageMoveAnimationCurve.Evaluate(normalizeTime);
                currentYPos = Mathf.Clamp(currentYPos, 0, maxYPos);
                timer += Time.fixedDeltaTime;

                Transform target = ironCage_1;

                if (currentYPos >= changeValue)
                {
                    ironCage_1.gameObject.SetActive(false);
                    ironCage_2.gameObject.SetActive(true);

                    target = ironCage_2;
                }
                else
                {
                    ironCage_1.gameObject.SetActive(true);
                    ironCage_2.gameObject.SetActive(false);

                    target = ironCage_1;
                }

                target.position = new Vector3(target.position.x, currentYPos, target.position.z);
                await Awaitable.FixedUpdateAsync();
            }

            //CameraController.Instance.PlayImpulse(0.45f, 0.2f, onComplete: () =>
            //{

            //});

            CameraController.Instance.ResetPosition(duration: 0.75f);
            CameraController.Instance.ResetZoom(duration: 0.75f, onComplete: () =>
            {
                Bus<PlayerInputEnableEvent>.Raise(new PlayerInputEnableEvent(true));
            });

        }

        [ContextMenu("Close Test")]
        public override async void Close()
        {

            float timer = 0;
            ironCage_1.gameObject.SetActive(true);
            ironCage_2.gameObject.SetActive(false);

            Bus<PlayerInputEnableEvent>.Raise(new PlayerInputEnableEvent(false));

            while (duration >= timer)
            {
                float normalizeTime = timer / duration;
                float currentYPos = maxYPos - maxYPos * cageMoveAnimationCurve.Evaluate(normalizeTime);
                currentYPos = Mathf.Clamp(currentYPos, 0, maxYPos);
                timer += Time.fixedDeltaTime;

                Transform target = ironCage_1;

                if (currentYPos >= changeValue)
                {
                    ironCage_1.gameObject.SetActive(false);
                    ironCage_2.gameObject.SetActive(true);

                    target = ironCage_2;
                }
                else
                {
                    ironCage_1.gameObject.SetActive(true);
                    ironCage_2.gameObject.SetActive(false);

                    target = ironCage_1;
                }

                target.position = new Vector3(target.position.x, currentYPos, target.position.z);
                await Awaitable.FixedUpdateAsync();
            }

            Bus<PlayerInputEnableEvent>.Raise(new PlayerInputEnableEvent(true));
        }
    }
}