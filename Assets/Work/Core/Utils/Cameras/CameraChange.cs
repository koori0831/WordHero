using Unity.Cinemachine;
using UnityEngine;

namespace Work.Core.Utils.Cameras
{
    public class CameraChange : MonoBehaviour
    {
        [SerializeField] private CinemachineCamera mainCam;
        [SerializeField] private CinemachineCamera aimCam;

        private void OnTriggerEnter(Collider other)
        {
            mainCam.Priority = -1;
            aimCam.Priority = 1;
        }

        private void OnTriggerExit(Collider other)
        {
            mainCam.Priority = 1;
            aimCam.Priority = -1;
        }
    }
}