using TMPro;
using UnityEngine;
using Work.Weapons.Code;

namespace Work.MainEntrance.Code
{
    /// <summary>
    /// 투영 바빌론에 배치되는 시작 무기 전시대
    /// </summary>
    public sealed class StarterWeaponDisplayStand : MonoBehaviour, IInteractable
    {
        private static readonly int OUTLINE_COLOR_ID = Shader.PropertyToID("_OutLineColor");
        private static readonly int COLOR_ID = Shader.PropertyToID("_Color");
        private static readonly int BASE_COLOR_ID = Shader.PropertyToID("_BaseColor");

        private static readonly Color AVAILABLE_COLOR = new Color(0.65f, 0.65f, 0.65f, 1f);
        private static readonly Color SELECTED_COLOR = new Color(0.25f, 0.85f, 1f, 1f);
        private static readonly Color UNAVAILABLE_COLOR = new Color(0.25f, 0.25f, 0.25f, 1f);

        private EraWeaponSelectionInteractable _owner;
        private StarterWeaponOption _option;
        private BoxCollider _interactionCollider;
        private Renderer _pedestalRenderer;
        private Renderer[] _renderers;
        private MaterialPropertyBlock _propertyBlock;
        private TextMeshPro _labelText;
        private bool _isSelected;

        public StarterWeaponOption Option => _option;

        /// <summary>
        /// 전시대 초기화 및 전시 모델 생성
        /// </summary>
        /// <param name="owner">선택 상태를 관리하는 병기고 컴포넌트</param>
        /// <param name="option">전시할 시작 무기 항목</param>
        /// <param name="displayLocalPosition">전시 모델 로컬 위치</param>
        /// <param name="displayLocalEulerAngles">전시 모델 로컬 회전</param>
        /// <param name="displayLocalScale">전시 모델 로컬 크기</param>
        /// <param name="labelLocalPosition">라벨 로컬 위치</param>
        /// <param name="interactionColliderSize">상호작용 콜라이더 크기</param>
        public void Initialize(
            EraWeaponSelectionInteractable owner,
            StarterWeaponOption option,
            Vector3 displayLocalPosition,
            Vector3 displayLocalEulerAngles,
            Vector3 displayLocalScale,
            Vector3 labelLocalPosition,
            Vector3 interactionColliderSize)
        {
            _owner = owner;
            _option = option;
            _propertyBlock = new MaterialPropertyBlock();

            CreateInteractionCollider(interactionColliderSize);
            CreatePedestal();
            CreateDisplayModel(displayLocalPosition, displayLocalEulerAngles, displayLocalScale);
            CreateLabel(labelLocalPosition);

            _renderers = GetComponentsInChildren<Renderer>();
            SetSelected(owner != null && owner.IsSelected(option.WeaponType));
        }

        /// <summary>
        /// 전시대 상호작용으로 무기군 선택 상태 전환
        /// </summary>
        /// <param name="interactor">상호작용을 수행한 오브젝트</param>
        public void Interact(GameObject interactor)
        {
            if (!_option.IsAvailable || _owner == null)
            {
                return;
            }

            _owner.TryToggleSelection(this);
        }

        /// <summary>
        /// 선택 상태에 맞춰 시각 표시 갱신
        /// </summary>
        /// <param name="isSelected">선택 여부</param>
        public void SetSelected(bool isSelected)
        {
            _isSelected = isSelected;
            RefreshLabel();
            RefreshRendererState();
        }

        /// <summary>
        /// 라벨이 카메라를 바라보도록 회전
        /// </summary>
        private void LateUpdate()
        {
            if (_labelText == null || Camera.main == null)
            {
                return;
            }

            Vector3 direction = _labelText.transform.position - Camera.main.transform.position;
            if (direction.sqrMagnitude <= 0.001f)
            {
                return;
            }

            _labelText.transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
        }

        /// <summary>
        /// 상호작용 판정용 트리거 콜라이더 생성
        /// </summary>
        /// <param name="interactionColliderSize">상호작용 콜라이더 크기</param>
        private void CreateInteractionCollider(Vector3 interactionColliderSize)
        {
            _interactionCollider = gameObject.AddComponent<BoxCollider>();
            _interactionCollider.isTrigger = true;
            _interactionCollider.size = interactionColliderSize;
            _interactionCollider.center = new Vector3(0f, interactionColliderSize.y * 0.5f, 0f);
            _interactionCollider.enabled = _option.IsAvailable;
        }

        /// <summary>
        /// 무기를 올려둘 받침대 생성
        /// </summary>
        private void CreatePedestal()
        {
            GameObject pedestal = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pedestal.name = "Pedestal";
            pedestal.transform.SetParent(transform);
            pedestal.transform.localPosition = new Vector3(0f, 0.12f, 0f);
            pedestal.transform.localRotation = Quaternion.identity;
            pedestal.transform.localScale = new Vector3(0.9f, 0.12f, 0.9f);

            Collider pedestalCollider = pedestal.GetComponent<Collider>();
            if (pedestalCollider != null)
            {
                pedestalCollider.enabled = false;
            }

            _pedestalRenderer = pedestal.GetComponent<Renderer>();
            if (_pedestalRenderer != null)
            {
                _pedestalRenderer.material.color = _option.IsAvailable ? AVAILABLE_COLOR : UNAVAILABLE_COLOR;
            }
        }

        /// <summary>
        /// 전시용 무기 모델 또는 잠김 표시 생성
        /// </summary>
        /// <param name="displayLocalPosition">전시 모델 로컬 위치</param>
        /// <param name="displayLocalEulerAngles">전시 모델 로컬 회전</param>
        /// <param name="displayLocalScale">전시 모델 로컬 크기</param>
        private void CreateDisplayModel(Vector3 displayLocalPosition, Vector3 displayLocalEulerAngles, Vector3 displayLocalScale)
        {
            GameObject displayObject;

            if (_option.IsAvailable)
            {
                displayObject = Instantiate(_option.WeaponPrefab.gameObject, transform);
                displayObject.name = $"{_option.GetDisplayName()} DisplayModel";
                PrepareDisplayInstance(displayObject);
            }
            else
            {
                displayObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                displayObject.name = $"{_option.GetDisplayName()} LockedModel";
                displayObject.transform.SetParent(transform);

                Collider lockedCollider = displayObject.GetComponent<Collider>();
                if (lockedCollider != null)
                {
                    lockedCollider.enabled = false;
                }

                Renderer lockedRenderer = displayObject.GetComponent<Renderer>();
                if (lockedRenderer != null)
                {
                    lockedRenderer.material.color = UNAVAILABLE_COLOR;
                }
            }

            displayObject.transform.localPosition = displayLocalPosition;
            displayObject.transform.localRotation = Quaternion.Euler(displayLocalEulerAngles);
            displayObject.transform.localScale = displayLocalScale;
        }

        /// <summary>
        /// 전시용 무기 인스턴스의 게임플레이 기능 비활성화
        /// </summary>
        /// <param name="displayObject">전시용 무기 인스턴스</param>
        private void PrepareDisplayInstance(GameObject displayObject)
        {
            Collider[] colliders = displayObject.GetComponentsInChildren<Collider>();
            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].enabled = false;
            }

            Rigidbody[] rigidbodies = displayObject.GetComponentsInChildren<Rigidbody>();
            for (int i = 0; i < rigidbodies.Length; i++)
            {
                rigidbodies[i].isKinematic = true;
                rigidbodies[i].detectCollisions = false;
            }

            MonoBehaviour[] behaviours = displayObject.GetComponentsInChildren<MonoBehaviour>();
            for (int i = 0; i < behaviours.Length; i++)
            {
                behaviours[i].enabled = false;
            }
        }

        /// <summary>
        /// 무기군 이름과 상태를 표시하는 월드 라벨 생성
        /// </summary>
        /// <param name="labelLocalPosition">라벨 로컬 위치</param>
        private void CreateLabel(Vector3 labelLocalPosition)
        {
            GameObject labelObject = new GameObject("Label");
            labelObject.transform.SetParent(transform);
            labelObject.transform.localPosition = labelLocalPosition;
            labelObject.transform.localRotation = Quaternion.identity;
            labelObject.transform.localScale = Vector3.one;

            _labelText = labelObject.AddComponent<TextMeshPro>();
            _labelText.alignment = TextAlignmentOptions.Center;
            _labelText.fontSize = 0.45f;
            _labelText.textWrappingMode = TextWrappingModes.NoWrap;
            _labelText.rectTransform.sizeDelta = new Vector2(4f, 1f);
            RefreshLabel();
        }

        /// <summary>
        /// 선택 상태와 활성 여부에 맞춰 라벨 텍스트 갱신
        /// </summary>
        private void RefreshLabel()
        {
            if (_labelText == null)
            {
                return;
            }

            string text = _option.GetDisplayName();
            if (!_option.IsAvailable)
            {
                text += "\n준비 중";
                _labelText.color = Color.gray;
            }
            else if (_isSelected)
            {
                text = "선택됨\n" + text;
                _labelText.color = SELECTED_COLOR;
            }
            else
            {
                _labelText.color = Color.white;
            }

            _labelText.text = text;
        }

        /// <summary>
        /// 선택 상태와 활성 여부에 맞춰 렌더러 색상 갱신
        /// </summary>
        private void RefreshRendererState()
        {
            if (_pedestalRenderer != null)
            {
                _pedestalRenderer.material.color = GetStateColor();
            }

            if (_renderers == null || _propertyBlock == null)
            {
                return;
            }

            Color stateColor = GetStateColor();
            for (int i = 0; i < _renderers.Length; i++)
            {
                _renderers[i].GetPropertyBlock(_propertyBlock);
                _propertyBlock.SetColor(OUTLINE_COLOR_ID, _isSelected ? SELECTED_COLOR : Color.black);

                if (!_option.IsAvailable)
                {
                    _propertyBlock.SetColor(COLOR_ID, stateColor);
                    _propertyBlock.SetColor(BASE_COLOR_ID, stateColor);
                }

                _renderers[i].SetPropertyBlock(_propertyBlock);
            }
        }

        /// <summary>
        /// 현재 상태에 대응하는 표시 색상 반환
        /// </summary>
        /// <returns>상태 표시 색상</returns>
        private Color GetStateColor()
        {
            if (!_option.IsAvailable)
            {
                return UNAVAILABLE_COLOR;
            }

            return _isSelected ? SELECTED_COLOR : AVAILABLE_COLOR;
        }
    }
}
