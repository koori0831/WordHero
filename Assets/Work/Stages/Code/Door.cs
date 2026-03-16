using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        [field: SerializeField] public GameObject DoorObject { get; private set; }
    }

    public class Door : MonoBehaviour, IInteractable
    {
        private Stage _stage;
        private DoorType _doorType;
        private bool _isInteract;
        private bool _isOpen;

        [SerializeField]
        private List<DoorModel> doorModels = new List<DoorModel>();
        private Dictionary<DoorType, GameObject> doorModelDictionay = new Dictionary<DoorType, GameObject>();

        public void DoorInit(Stage stage)
        {
            _stage = stage;
            doorModelDictionay = doorModels.ToDictionary(x => x.DoorType, x => x.DoorObject);

            
        }

        public void SetDoorType(DoorType doorType)
        {
            _doorType = doorType;
            GameObject doorObjectPrefab = doorModelDictionay[doorType];

            if (doorObjectPrefab != null)
            {
                GameObject doorObject = Instantiate(doorObjectPrefab, transform);
                doorObject.transform.localPosition = Vector3.zero;
            }

        }

        public void Interact(GameObject interactor)
        {
            if (_isInteract == true) return;
            if (!_isOpen) return;

            _isInteract = true;
            _stage.HandleGoNextRoom(interactor, _doorType);
        }

        public void Open()
        {
            _isOpen = true;
            //여기서 문 열리는 연출 표현
        }
    }
}