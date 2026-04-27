using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitMotion;
using Work.Stages.Code;

namespace Work.ProgressRate.Code
{
    [Serializable]
    public class StageIconConfig
    {
        public DoorType type;
        public Sprite icon;
    }

    public class StageProgressNode : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image nodeImage;
        [SerializeField] private Image highlightImage;
        [SerializeField] private Image iconImage; 
        
        [Header("Icon Settings")]
        [SerializeField] private Sprite lockedSprite; // 물음표 아이콘 등
        [SerializeField] private List<StageIconConfig> iconConfigs;
        
        [Header("Boss Settings")]
        [SerializeField] private Sprite bossSprite;
        [SerializeField] private Vector3 bossScale = new Vector3(1.3f, 1.3f, 1.3f);

        [Header("State Colors")]
        [SerializeField] private Color completedColor = Color.green;
        [SerializeField] private Color currentColor = Color.white;
        [SerializeField] private Color lockedColor = Color.gray;

        private DoorType _actualType;

        public void Setup(DoorType type, bool isBoss, bool isCompleted, bool isCurrent)
        {
            _actualType = type;

            if (isBoss)
            {
                transform.localScale = bossScale;
                if (iconImage != null) iconImage.sprite = bossSprite;
            }
            else
            {
                transform.localScale = Vector3.one;
                // 클리어 전에는 무조건 잠금 아이콘, 클리어된 상태라면 실제 아이콘 표시
                if (iconImage != null)
                {
                    iconImage.sprite = isCompleted ? GetSpriteByType(type) : lockedSprite;
                }
            }

            SetState(isCompleted, isCurrent);
        }

        private Sprite GetSpriteByType(DoorType type)
        {
            StageIconConfig config = iconConfigs.Find(x => x.type == type);
            return config?.icon;
        }

        public void SetState(bool isCompleted, bool isCurrent)
        {
            if (isCurrent)
            {
                nodeImage.color = currentColor;
                highlightImage.color = currentColor;
                highlightImage.gameObject.SetActive(true);
                highlightImage.color = new Color(highlightImage.color.r, highlightImage.color.g, highlightImage.color.b, 1f);
            }
            else if (isCompleted)
            {
                nodeImage.color = completedColor;
                highlightImage.gameObject.SetActive(false);
            }
            else
            {
                nodeImage.color = lockedColor;
                highlightImage.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 노드가 현재 위치에서 완료 상태로 변하며 아이콘을 공개하는 연출
        /// </summary>
        public void PlayCompleteAnimation(float duration)
        {
            // 1. 하이라이트 색상을 완료 색상으로 동시 변경하며 서서히 끄기
            LMotion.Create(highlightImage.color, completedColor, duration)
                .Bind(c => highlightImage.color = c)
                .AddTo(gameObject);

            LMotion.Create(1f, 0f, duration)
                .WithOnComplete(() => highlightImage.gameObject.SetActive(false))
                .Bind(a => 
                {
                    Color co = highlightImage.color;
                    co.a = a;
                    highlightImage.color = co;
                })
                .AddTo(gameObject);

            // 2. 노드 배경색 변경
            LMotion.Create(nodeImage.color, completedColor, duration)
                .Bind(c => nodeImage.color = c)
                .AddTo(gameObject);

            // 3. 아이콘 공개 연출 (잠금 아이콘 -> 실제 아이콘)
            if (iconImage != null && iconImage.sprite != bossSprite)
            {
                // 페이드 아웃 -> 스프라이트 교체 -> 페이드 인
                LMotion.Create(1f, 0f, duration * 0.5f)
                    .WithOnComplete(() => 
                    {
                        iconImage.sprite = GetSpriteByType(_actualType);
                        LMotion.Create(0f, 1f, duration * 0.5f)
                            .Bind(a => 
                            {
                                Color co = iconImage.color;
                                co.a = a;
                                iconImage.color = co;
                            })
                            .AddTo(gameObject);
                    })
                    .Bind(a => 
                    {
                        Color co = iconImage.color;
                        co.a = a;
                        iconImage.color = co;
                    })
                    .AddTo(gameObject);
            }
        }

        public void PlayActivateAnimation(float duration)
        {
            highlightImage.color = currentColor;
            highlightImage.gameObject.SetActive(true);
            highlightImage.color = new Color(highlightImage.color.r, highlightImage.color.g, highlightImage.color.b, 0f);
            
            LMotion.Create(0f, 1f, duration)
                .Bind(a => 
                {
                    Color co = highlightImage.color;
                    co.a = a;
                    highlightImage.color = co;
                })
                .AddTo(gameObject);
            
            nodeImage.color = currentColor;
        }
        
        public void CompleteImmediate()
        {
            highlightImage.gameObject.SetActive(false);
            nodeImage.color = completedColor;
            if (iconImage != null && iconImage.sprite != bossSprite)
            {
                iconImage.sprite = GetSpriteByType(_actualType);
            }
        }
    }
}
