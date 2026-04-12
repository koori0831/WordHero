using System;
using UnityEngine;
using Work.Agents.Code;
using Work.Players.Code;

namespace Work.Shops.Code
{
    public class PressurePlate : MonoBehaviour
    {
        public Action<Player> OnPressed;
        private bool isPressed = false;

        private void OnTriggerEnter(Collider other)
        {
            if (isPressed) return;
            if (other.CompareTag("Player"))
            {
                isPressed = true;
                Player player = other.GetComponent<Player>();
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - 0.05f, transform.localPosition.z);
                OnPressed?.Invoke(player);
            }
        }
    }
}