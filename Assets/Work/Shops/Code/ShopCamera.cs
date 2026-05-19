using UnityEngine;
using Work.Core.Utils.Cameras;

namespace Work.Shops.Code
{
    public class ShopCamera : MonoBehaviour
    {
        [SerializeField] private Transform camTrm;

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Player"))
            {
                CameraController.Instance.ResetPosition();
                CameraController.Instance.ResetRotate();
                CameraController.Instance.ResetZoom();
                CameraController.Instance.MoveTo(camTrm.position,0.5f);
                CameraController.Instance.RotateTo(camTrm.rotation,0.5f);
                CameraController.Instance.ZoomIn(15,0.5f);
            }
        }


        private void OnTriggerExit(Collider other)
        {
            if(other.CompareTag("Player"))
            {
                CameraController.Instance.ResetPosition(0.2f);
                CameraController.Instance.ResetRotate(0.2f);
                CameraController.Instance.ResetZoom(0.2f);
            }
        }

    }
}