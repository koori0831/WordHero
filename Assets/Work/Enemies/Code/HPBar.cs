using UnityEngine;

namespace Work.Enemies.Code
{
    public class HPBar : MonoBehaviour
    {
        public void SetActive(bool isTrue = true)
        {
            gameObject.SetActive(isTrue);
        }

        public void Update()
        {
            Transform cam = Camera.main.transform;

            transform.LookAt(cam);
            transform.Rotate(90f, 0f, 0f);
        }
    }
}