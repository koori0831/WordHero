using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Work.Core.Utils.EventBus;
using Work.Information.Code;

namespace Work.Cursor.Code
{
    //마우스가 움직일떄마다 레이를 쏴서 충돌한 오브젝트의 정보를 이벤트로 쏴주는 매니저
    public class CursorManager : MonoBehaviour
    {
        private GameObject _lastTarget;
        private Vector2 _lastMousePos;

        public void Update()
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            if (mousePos == _lastMousePos)
                return;
            _lastMousePos = mousePos;

            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 0.1f);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                if(_lastTarget == hit.collider.gameObject)
                    return;

                _lastTarget = hit.collider.gameObject;
                IInformationable inforable = hit.collider.GetComponent<IInformationable>();

                if (inforable != null)
                {
                    InfoDataSO info = inforable.InfoData;
                    if (info != null)
                    {
                        //UI나 다른 시스템에서 정보를 보여주도록 이벤트를 쏜다.
                        Bus<InfoDataEvent>.Raise(new InfoDataEvent(_lastTarget,info));
                    }
                }
                else
                {
                    //UI나 다른 시스템에서 정보를 숨기도록 이벤트를 쏜다.
                    Bus<HideInfoDataEvent>.Raise(new HideInfoDataEvent());
                }
            }
        }
    }
}