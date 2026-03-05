using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Work.Combat.Code;

namespace Work.Combat.TestCode
{
    public class DamageTextTester : MonoBehaviour
    {
        [SerializeField] private DamageText damageTextPrefab;

        private void Update()
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                SpawnDamageText();
            }
        }

        private void SpawnDamageText()
        {
            Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * 2f;
            DamageText damageText = Instantiate(damageTextPrefab, transform.position + (Vector3)randomOffset, Camera.main.transform.rotation);
            damageText.Init(UnityEngine.Random.Range(100, 500), UnityEngine.Random.value > 0.5f);
        }
    }
}