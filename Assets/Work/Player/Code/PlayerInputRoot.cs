using Code.Entities;
using System.Collections;
using UnityEngine;
using Work.Input.Code;

namespace Work.Player.Code
{
    public class PlayerInputRoot : MonoBehaviour, IEntityComponent
    {
        public Entity Owner { get; private set; }

        public void InitCompo(Entity entity)
        {
            Owner = entity;
        }

        public InputContainer Input { get; private set; }

        public Vector2 MoveVector => Input.MoveVector;

        private void Awake()
        {
            Input = new InputContainer();
            Input.Init();
        }

        private void OnDestroy()
        {
            Input.Deinit();
        }
    }
}