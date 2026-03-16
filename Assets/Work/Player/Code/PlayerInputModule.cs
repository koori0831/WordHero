using UnityEngine;
using Work.Agents.Code;
using Work.Input.Code;

namespace Work.Player.Code
{
    public class PlayerInputModule : MonoBehaviour, IAgentModule
    {
        private Agent _owner;

        public void Initialize(Agent agent)
        {
            _owner = agent;
        }

        public InputContainer Input { get; private set; }

        public Vector2 MoveVector => Input.MoveVector;
        public bool IsMovePressed => Input.IsMovePressed;

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
