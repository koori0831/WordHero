using UnityEngine;

namespace Work.Indicator.Code
{
    /// <summary>
    /// 인디케이터 표시 상태 계산기
    /// </summary>
    public class IndicatorViewStateCalculator
    {
        /// <summary>
        /// 인디케이터 표시 상태 계산
        /// </summary>
        public IndicatorViewState Calculate(IndicatorTargetModel targetModel, Camera mainCamera, Rect containerRect, float margin)
        {
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(targetModel.TargetTransform.position);
            bool isInside = screenPosition.z > 0f
                            && screenPosition.x > margin
                            && screenPosition.x < Screen.width - margin
                            && screenPosition.y > margin
                            && screenPosition.y < Screen.height - margin;

            if (isInside)
            {
                return new IndicatorViewState(false, Vector2.zero, Quaternion.identity);
            }

            Vector2 direction = CalculateScreenEdgeDirection(targetModel, mainCamera);
            Vector2 anchoredPosition = CalculateEdgePosition(direction, containerRect, margin);
            Quaternion rotation = CalculateRotation(direction);

            return new IndicatorViewState(true, anchoredPosition, rotation);
        }

        /// <summary>
        /// 화면 가장자리 방향 계산
        /// </summary>
        private Vector2 CalculateScreenEdgeDirection(IndicatorTargetModel targetModel, Camera mainCamera)
        {
            Vector3 cameraForward = mainCamera.transform.forward;
            cameraForward.y = 0f;
            cameraForward.Normalize();

            Vector3 cameraRight = mainCamera.transform.right;
            cameraRight.y = 0f;
            cameraRight.Normalize();

            Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            Plane groundPlane = new Plane(Vector3.up, targetModel.TargetTransform.position);
            Vector3 centerPosition;
            if (groundPlane.Raycast(ray, out float enter))
            {
                centerPosition = ray.GetPoint(enter);
            }
            else
            {
                centerPosition = mainCamera.transform.position + cameraForward * 10f;
            }

            Vector3 diff = targetModel.TargetTransform.position - centerPosition;
            float xDirection = Vector3.Dot(diff.normalized, cameraRight);
            float yDirection = Vector3.Dot(diff.normalized, cameraForward);
            Vector2 direction = new Vector2(xDirection, yDirection);

            if (direction.sqrMagnitude < 0.001f)
            {
                return Vector2.up;
            }

            direction.Normalize();
            return direction;
        }

        /// <summary>
        /// 화면 가장자리 위치 계산
        /// </summary>
        private Vector2 CalculateEdgePosition(Vector2 direction, Rect containerRect, float margin)
        {
            float halfWidth = containerRect.width * 0.5f - margin;
            float halfHeight = containerRect.height * 0.5f - margin;
            float slope = Mathf.Approximately(direction.x, 0f) ? direction.y * 1000000f : direction.y / direction.x;
            float screenRatio = halfHeight / halfWidth;
            Vector2 offset = Vector2.zero;

            if (Mathf.Abs(slope) <= screenRatio)
            {
                float x = direction.x > 0f ? halfWidth : -halfWidth;
                offset = new Vector2(x, slope * x);
            }
            else
            {
                float y = direction.y > 0f ? halfHeight : -halfHeight;
                offset = new Vector2(y / slope, y);
            }

            if (float.IsNaN(offset.x) || float.IsNaN(offset.y))
            {
                return Vector2.zero;
            }

            return offset;
        }

        /// <summary>
        /// 인디케이터 회전값 계산
        /// </summary>
        private Quaternion CalculateRotation(Vector2 direction)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            return Quaternion.Euler(0f, 0f, angle);
        }
    }
}
