using UnityEngine;
using Work.Core.Utils.Cameras;
using Work.Core.Utils.EventBus;
using Work.Input.Code;
using Work.Interaction.Code;
using Work.Players.Code;
using Work.Stages.Code;

namespace Work.Chests.Code
{
    public enum ChestType
    {
        Wood = 0,
        Stone = 1,
        Iron = 2,
        Gold = 3,
    }

    public class Chest : MonoBehaviour, IInteractable
    {
        [SerializeField] private Transform chestHeadTransform;
        [SerializeField] private Renderer inBoxRenderer;

        public Vector3 cameraMovePosition { get; set; }

        private static readonly int _InBoxColor = Shader.PropertyToID("_Color");
        private ChestType _chestType;
        private bool _isOpened;

        [ColorUsage(true, true)]
        private Color[] colors = new Color[]
        {
            new Color(164,164,164) /    13, // Wood
            new Color(42,191,46) /      13, // Stone
            new Color(38,57,191) /      13, // Iron
            new Color(191,166,38) /     13 // Gold
        };

        private const float _1_MOVE_ROTATE = -16f; // 1초 
        private const float _2_MOVE_ROTATE = -135f; // 0.5f 초

        [SerializeReference]
        public ICollectAction CollectAction;

        public void Initialize(ChestType chestType)
        {
            _chestType = chestType;
        }

        private void Awake()
        {
            inBoxRenderer.material.SetColor(_InBoxColor, Color.white);
        }

        public void Start()
        {
            Debug.Assert(CollectAction != null);
            CollectAction.Initialize();
        }

        public void Interact(GameObject interactor)
        {
            if (_isOpened) return;

            if (interactor.TryGetComponent(out Player player))
            {
                _isOpened = true;
                // 연출 나오고
                inBoxRenderer.material.SetColor(_InBoxColor, colors[(int)_chestType]);
                Open(player);
            }
        }

        public async void Open(Player player)
        {
            Bus<PlayerInputEnableEvent>.Raise(new PlayerInputEnableEvent(false));

            float timer = 0;
            while (timer <= 1f)
            {
                float currentRotate = _1_MOVE_ROTATE * (timer / 1f);
                chestHeadTransform.localRotation = Quaternion.Euler(0, 0, currentRotate);
                timer += Time.fixedDeltaTime;
                await Awaitable.FixedUpdateAsync();
            }

            timer = 0;
            while (timer <= 0.4f)
            {
                timer += Time.fixedDeltaTime;
                await Awaitable.FixedUpdateAsync();
            }

            timer = 0;
            while (timer <= 0.5f)
            {
                float currentRotate = _2_MOVE_ROTATE * (timer / 0.5f) + _1_MOVE_ROTATE;
                chestHeadTransform.localRotation = Quaternion.Euler(0, 0, currentRotate);
                timer += Time.fixedDeltaTime;
                await Awaitable.FixedUpdateAsync();
            }

            CollectAction.Collect(player);

            // TODO: Camera direction hook point after chest open

            CameraController.Instance.MoveTo(cameraMovePosition, duration: 0.75f);
            CameraController.Instance.ZoomOut(15f, duration:1f,onComplete: () =>
            {
                Bus<StageClearEvent>.Raise(new StageClearEvent());
            });


            
        }
    }
}
