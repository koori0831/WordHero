using System.Collections.Generic;
using Unity.AppUI.UI;
using UnityEngine;

namespace Work.Enemies.Code
{
    public class EnemyMovementModule : MonoBehaviour, IEnemyModule
    {
        private Enemy _owner;
        private Transform _target;
        private List<ICrowd> nearNeighbors = new List<ICrowd>();
        private Vector3 velocity;

        [SerializeField] private float speed = 3.0f;

        public void Initialize(Enemy enemy)
        {
            _owner = enemy;
        }

        public void SetTarget(Transform target)
        {
            _target = target;
        }

        public void Update()
        {
            UpdateMovement();
        }

        public void UpdateMovement()
        {
            FindNeighbors();

            //velocity += CalculateSeparation() * _owner.Spawner.separationWeight;
            Vector3 next = _owner.Spawner.GetNextMove(_owner.transform.position, _target.position, _owner.Guid) - _owner.transform.position;
            Debug.Log($"{gameObject.name} / 방향 : {next}");
            velocity += next;

            if (velocity.magnitude > speed) // prevent infinite accelation
                velocity = velocity.normalized * speed;


            _owner.transform.position += velocity * Time.deltaTime; // 가속도
            _owner.transform.rotation = Quaternion.Slerp(_owner.transform.rotation, Quaternion.LookRotation(velocity),Time.deltaTime * 10);
        }

        public void StopMovement()
        {
            // Implementation for stopping movement if needed
        }

        private void FindNeighbors()
        {
            nearNeighbors.Clear();

            foreach (ICrowd neighbor in _owner.Spawner.Neighdors) // 전체 이웃 탐색
            {
                if (nearNeighbors.Count >= _owner.Spawner.maxNeighbors)
                    return;

                if (neighbor.Transform.gameObject == this.gameObject)
                {
                    Debug.Log("Pass, because neighbor is me");
                    continue;
                }

                Vector3 diff = neighbor.Transform.position - _owner.transform.position;

                if (diff.sqrMagnitude < _owner.Spawner.neighborDistance * _owner.Spawner.neighborDistance) // 범위 내 이웃만 남기기
                {
                    nearNeighbors.Add(neighbor);
                }
            }
        }

        private Vector3 CalculateSeparation()
        {
            Vector3 separationDirection = Vector3.zero;

            if (nearNeighbors.Count > 0)
            {
                for (int i = 0; i < nearNeighbors.Count; ++i)
                {
                    separationDirection += (_owner.transform.position - nearNeighbors[i].Transform.position);
                }

                separationDirection.Normalize();
            }

            return separationDirection;
        }
    }
}