using System;
using UnityEngine;
using Work.Core.Utils.EventBus;
using Work.Enemies.Code;
using Work.Information.Code;

namespace Work.Cursor.Code
{
    public class SelectManager : MonoBehaviour
    {
        private GameObject _currentSelectObject;

        private void Awake()
        {
            Bus<InfoDataEvent>.Events += OnInfoDataEvent;
            Bus<HideInfoDataEvent>.Events += OnHideInfoDataEvent;
        }

        private void OnHideInfoDataEvent(HideInfoDataEvent evt)
        {
            OutLineActiveFalse();

            _currentSelectObject = null;
        }

        private void OutLineActiveFalse()
        {
            if (_currentSelectObject == null) return;

            Renderer[] renderers = _currentSelectObject.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                foreach (Material mat in renderer.materials)
                {
                    mat.SetColor("_OutLineColor", Color.black);
                }
            }
        }

        private void OnInfoDataEvent(InfoDataEvent evt)
        {
            OutLineActiveFalse();

            if (!(evt.Info is EnemyInfoDataSO info)) return;

            Renderer[] renderers = info.Owner.gameObject.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                foreach (Material mat in renderer.materials)
                {
                    mat.SetColor("_OutLineColor", Color.white);
                }
            }

            _currentSelectObject = info.Owner.gameObject;
        }
    }
}