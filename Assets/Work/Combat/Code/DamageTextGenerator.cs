using UnityEngine;
using Work.Core.Utils.EventBus;

namespace Work.Combat.Code
{
    public class DamageTextGenerator : MonoBehaviour
    {
        [SerializeField] private GameObject damageTextPrefab;
        [SerializeField] private Vector2 offset;

        public void Awake()
        {
            Bus<CombatHitEvent>.Events += HandleDamageTextEvent;
        }

        private void HandleDamageTextEvent(CombatHitEvent evt) =>
            GenerateDamageText(evt.Damage, evt.Target, evt.IsCritical);

        public void GenerateDamageText(int damage, GameObject target, bool isCritical = false)
        {
            float randomX = Random.Range(-offset.x, offset.x);
            float randomY = Random.Range(-offset.y, offset.y);
            Debug.Log($"DamageTextGenerator: Generating damage text at position {target.transform.position} with random offset ({randomX}, {randomY})");
            Vector3 pos = target.transform.position + new Vector3(randomX, randomY + 5, -2);

            GameObject damageTextObj = Instantiate(damageTextPrefab, pos, Camera.main.transform.rotation);
            //damageTextObj.transform.parent = owner.transform;
            DamageText damageText = damageTextObj.GetComponent<DamageText>();

            bool isPlayer = false;
            if (target.layer  == LayerMask.NameToLayer("Player"))
                isPlayer = true;
           

            damageText.Init(damage, isCritical, isPlayer);
        }
    }
}