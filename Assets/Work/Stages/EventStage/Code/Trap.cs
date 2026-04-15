using LitMotion;
using UnityEngine;
using Work.Core.Utils.Cameras;
using Work.Core.Utils.EventBus;
using Work.Players.Code;

namespace Work.Stages.EventStage.Code
{
    public record struct OnTrapDownEvent() : IEvent;

    public class Trap : MonoBehaviour
    {
        private bool _isActivated = true;

        private void OnTriggerEnter(Collider other)
        {
            if (!_isActivated) return;  // 이미 발동된 트랩은 무시
            if (other.gameObject.TryGetComponent<Player>(out Player player))
            {
                player.TakeDamage(20);
                CameraController.Instance.PlayImpulse(3f, 0.2f);
                Bus<OnTrapDownEvent>.Raise(new OnTrapDownEvent());
            }
        }

        private void Awake()
        {
            Bus<OnTrapDownEvent>.Events += OnTrapDown;
        }

        private void OnDestroy()
        {
            Bus<OnTrapDownEvent>.Events -= OnTrapDown;
        }

        private void OnTrapDown(OnTrapDownEvent evt)
        {
            //Lmotion을 활용해서 해당 오브젝트가 아래로 내려가도록 만들어줘
            _isActivated = true;  // 트랩이 발동되었음을 표시

            LMotion.Create(0, -1.2f, 0.45f)
                .WithEase(Ease.OutCubic)
                .Bind(y => transform.position = new Vector3(transform.position.x, y, transform.position.z));
        }
    }
}