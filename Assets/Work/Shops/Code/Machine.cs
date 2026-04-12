using LitMotion;
using System.Collections.Generic;
using UnityEngine;
using Work.Agents.Code;
using Work.Core.Utils.Cameras;
using Work.Core.Utils.EventBus;
using Work.Input.Code;
using Work.Players.Code;
using Random = UnityEngine.Random;

namespace Work.Shops.Code
{
    public class Machine : MonoBehaviour
    {
        [SerializeField] private PressurePlate pressurePlate;
        [SerializeField] private GameObject spike;
        [SerializeField] private GameObject lever;
        [SerializeField] private GameObject bone;

        [SerializeReference] public List<IMachineAction> mationActions = new List<IMachineAction>();

        public void Awake()
        {
            pressurePlate.OnPressed += OnPressurePlatePressedEvent;
        }

        private void OnPressurePlatePressedEvent(Player player)
        {
            //플레이어 이동 막고
            //살짝 줌인
            //spike 올라옴
            //카메라 기계에 고정
            //카메라 줌
            //기계팔 내려옴
            //플레이어 hp 10% 감소
            //플레이어 이동 가능

            Bus<PlayerInputEnableEvent>.Raise(new PlayerInputEnableEvent(false));
            CameraController.Instance.ZoomIn(12f, 0.4f, onComplete: () =>
            {
                float startY = spike.transform.localPosition.y;
                LMotion.Create(startY, 0, 0.5f)
                    .WithEase(Ease.OutBounce)
                    .WithOnComplete(() =>
                    {
                        AgentHealthModule hpModule = player.GetModule<AgentHealthModule>(true);
                        HpValue hpValue = hpModule.HpValue;

                        int damage = (int)(hpValue.MaxHp * 0.1f);

                        int cur = hpValue.CurrentHp;

                        if (cur - damage <= 0)
                        {
                            damage = 1;
                        }

                        hpModule.TakeDamage(damage);

                        CameraController.Instance.MoveTo(bone.transform.position, 0.4f, onComplete: () =>
                        {
                            float startX = -90;
                            float endX = 15f;

                            LMotion.Create(startX, endX, 0.3f)
                                .WithEase(Ease.OutElastic)
                                .WithOnComplete(() =>
                                {
                                    mationActions[Random.Range(0, mationActions.Count)].Apply(player);
                                    CameraController.Instance.ResetPosition(0.4f);
                                    CameraController.Instance.ResetZoom(0.4f);
                                    Bus<PlayerInputEnableEvent>.Raise(new PlayerInputEnableEvent(true));
                                })
                                .Bind(x => lever.transform.localRotation = Quaternion.Euler(x, 0, 0));
                        });
                    })
                    .Bind(x => spike.transform.localPosition = new Vector3(-6.29f, x, -10.65f));

            });

        }
    }
}