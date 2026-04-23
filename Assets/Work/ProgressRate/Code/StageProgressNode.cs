using UnityEngine;
using UnityEngine.UI;
using LitMotion;

namespace Work.ProgressRate.Code
{
    public class StageProgressNode : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image nodeImage;
        [SerializeField] private Image highlightImage;
        [SerializeField] private Image iconImage; 
        
        [Header("Boss Settings")]
        [SerializeField] private Sprite normalSprite;
        [SerializeField] private Sprite bossSprite;
        [SerializeField] private Vector3 bossScale = new Vector3(1.3f, 1.3f, 1.3f);

        [Header("State Colors")]
        [SerializeField] private Color completedColor = Color.green;
        [SerializeField] private Color currentColor = Color.white;
        [SerializeField] private Color lockedColor = Color.gray;

        public void Setup(bool isBoss, bool isCompleted, bool isCurrent)
        {
            if (isBoss)
            {
                transform.localScale = bossScale;
                if (iconImage != null) iconImage.sprite = bossSprite;
            }
            else
            {
                transform.localScale = Vector3.one;
                if (iconImage != null) iconImage.sprite = normalSprite;
            }

            SetState(isCompleted, isCurrent);
        }

        public void SetState(bool isCompleted, bool isCurrent)
        {
            if (isCurrent)
            {
                nodeImage.color = currentColor;
                highlightImage.color = currentColor; // 현재 노드 하이라이트는 기본색
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
        /// 노드가 현재 위치에서 완료 상태로 변하는 연출
        /// </summary>
        public void PlayCompleteAnimation(float duration)
        {
            // 하이라이트 색상을 완료 색상으로 동시 변경하며 서서히 끄기
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

            // 노드 색상을 완료 색상으로 변경
            LMotion.Create(nodeImage.color, completedColor, duration)
                .Bind(c => nodeImage.color = c)
                .AddTo(gameObject);
        }

        /// <summary>
        /// 새로운 현재 노드가 점등되는 연출
        /// </summary>
        public void PlayActivateAnimation(float duration)
        {
            // 점등 시 하이라이트 색상을 기본 현재 색상으로 초기화
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
        }
    }
}
