using GondrLib.Dependencies;
using UnityEngine;
using Work.Agents.Code;
using Work.Players.Code;

namespace Work.HPBar.Code
{
    public class PlayerHpPanel : MonoBehaviour
    {
        [SerializeField] private HPField hpField;

        [Inject] private Player _player;

        public void Start()
        {
            AgentHealthModule health = _player.GetModule<AgentHealthModule>(true);
            health.HpValue.OnHpChanged += HandleHPChangeEvent;
            health.HpValue.OnDead += HandlePlayerDeathEvent;
            hpField.EnableFor(health.MaxHealth, health.MaxHealth);
        }

        private void HandlePlayerDeathEvent()
        {

        }

        private void HandleHPChangeEvent(int cur, int max)
        {
            hpField.HpChange(cur, max);
        }
    }
}