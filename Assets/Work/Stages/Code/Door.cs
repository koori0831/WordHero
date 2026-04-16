using System;
using System.Collections.Generic;
using UnityEngine;
using Work.Core.Utils.EventBus;
using Work.Input.Code;

namespace Work.Stages.Code
{
    public enum DoorType
    {
        Wood = 0,
        Stone = 1,
        Iron = 2,
        Gold = 3,
        Question = 4,
        Boss = 5,
        Shop = 6,
        None = 7
    }

    [Serializable]
    public class DoorModel
    {
        [field: SerializeField] public DoorType DoorType { get; private set; }
        [field: SerializeField] public DoorObject DoorObject { get; private set; }
    }

    public class Door : MonoBehaviour, IInteractable
    {
        private Stage _stage;
        private DoorType _doorType;
        public bool IsInteract { get; private set; } = false;
        public bool IsOpen { get; private set; } = false;

        [SerializeField]
        private List<DoorModel> doorModels;
        private Dictionary<DoorType, DoorObject> doorModelDictionay = new Dictionary<DoorType, DoorObject>();

        private DoorObject _currentDoorObject;

        public void DoorInit(Stage stage)
        {
            _stage = stage;
            RebuildDoorModelDictionary(logWarnings: true);
        }

        public void SetDoorType(DoorType doorType)
        {
            _doorType = doorType;

            if (doorModelDictionay.Count == 0)
            {
                RebuildDoorModelDictionary(logWarnings: true);
            }

            if (!doorModelDictionay.TryGetValue(doorType, out DoorObject doorObjectPrefab) || doorObjectPrefab == null)
            {
                Debug.LogError($"[Door] DoorType '{doorType}' model is missing on '{name}'.", this);
                return;
            }

            if (_currentDoorObject != null)
            {
                Destroy(_currentDoorObject.gameObject);
                _currentDoorObject = null;
            }

            DoorObject doorObject = Instantiate(doorObjectPrefab, transform);
            doorObject.transform.localPosition = Vector3.zero;
            _currentDoorObject = doorObject;

        }

        public void Interact(GameObject interactor)
        {
            if (IsOpen == false || IsInteract == true) return;
            Bus<PlayerInputEnableEvent>.Raise(new PlayerInputEnableEvent(false));
            IsInteract = true;
            _stage.HandleGoNextRoom(interactor, _doorType);
        }


        public void Open()
        {
            IsOpen = true;
            if (_currentDoorObject == null)
            {
                Debug.LogWarning($"[Door] Open called but current door object is null on '{name}'.", this);
                return;
            }

            _currentDoorObject.Open();
            //여기서 문 열리는 연출 표현
        }

        private void OnValidate()
        {
            RebuildDoorModelDictionary(logWarnings: true);
        }

        private void RebuildDoorModelDictionary(bool logWarnings)
        {
            if (doorModelDictionay == null)
            {
                doorModelDictionay = new Dictionary<DoorType, DoorObject>();
            }
            else
            {
                doorModelDictionay.Clear();
            }

            if (doorModels == null)
            {
                return;
            }

            for (int i = 0; i < doorModels.Count; i++)
            {
                DoorModel model = doorModels[i];

                if (model == null)
                {
                    if (logWarnings)
                    {
                        Debug.LogWarning($"[Door] doorModels[{i}] is null on '{name}'.", this);
                    }
                    continue;
                }

                if (model.DoorObject == null)
                {
                    if (logWarnings)
                    {
                        Debug.LogWarning($"[Door] DoorObject is null for DoorType '{model.DoorType}' on '{name}'.", this);
                    }
                    continue;
                }

                if (doorModelDictionay.ContainsKey(model.DoorType))
                {
                    if (logWarnings)
                    {
                        Debug.LogWarning($"[Door] Duplicate DoorType '{model.DoorType}' found on '{name}'.", this);
                    }
                    continue;
                }

                doorModelDictionay.Add(model.DoorType, model.DoorObject);
            }
        }
    }
}
