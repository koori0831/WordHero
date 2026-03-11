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
            Bus<DamageTextEvent>.Events += HandleDamageTextEvent;
        }

        private void HandleDamageTextEvent(DamageTextEvent evt) =>
            GenerateDamageText(evt.Damage, evt.Owner, evt.IsCritical);

        public void GenerateDamageText(int damage, GameObject owner, bool isCritical = false)
        {
            float randomX = Random.Range(-offset.x, offset.x);
            float randomY = Random.Range(-offset.y, offset.y);
            Debug.Log($"DamageTextGenerator: Generating damage text at position {owner.transform.position} with random offset ({randomX}, {randomY})");
            Vector3 pos = owner.transform.position + new Vector3(randomX, randomY + 5, -2);

            GameObject damageTextObj = Instantiate(damageTextPrefab, pos, Camera.main.transform.rotation);
            //damageTextObj.transform.parent = owner.transform;
            DamageText damageText = damageTextObj.GetComponent<DamageText>();
            damageText.Init(damage, isCritical);
        }
    }
}