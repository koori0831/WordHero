using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RaycastVisualizer : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽 클릭 시
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            foreach (var result in results)
            {
                Debug.Log($"[UI Raycast Hit] 이름: {result.gameObject.name}", result.gameObject);
            }
        }
    }
}