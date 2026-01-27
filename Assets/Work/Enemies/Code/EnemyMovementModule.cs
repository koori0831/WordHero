using System.Collections.Generic;
using UnityEngine;

namespace Work.Enemies.Code
{
    public class EnemyMovementModule : MonoBehaviour, IEnemyModule
    {
        private Enemy _owner;
        private Transform _target;
        private List<ICrowd> nearNeighbors = new List<ICrowd>();
        private Vector3 velocity;

        public bool IsCanMove { get; private set; } = true;

        public bool IsMoving => velocity.magnitude > 0.1f;

        [SerializeField] private float stopDistance = 0.5f;
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
            if (!IsCanMove && _target != null)
                UpdateMovement();
        }

        public void UpdateMovement()
        {
            if (stopDistance > Vector2.Distance(_owner.transform.position, _target.position))
                return;

            FindNeighbors();

            velocity += CalculateSeparation() * _owner.Spawner.separationWeight;
            Vector3 next = _owner.Spawner.GetNextMove(_owner.transform.position, _target.position, _owner.Guid) - _owner.transform.position;
            Debug.Log($"{gameObject.name} / 방향 : {next}");
            velocity += next;

            if (velocity.magnitude > speed) // prevent infinite accelation
                velocity = velocity.normalized * speed;


            _owner.transform.position += velocity * Time.deltaTime; // 가속도
            _owner.transform.rotation = Quaternion.Slerp(_owner.transform.rotation, Quaternion.LookRotation(velocity), Time.deltaTime * 10);
        }

        public void SetMovement(bool isValue)
        {
            IsCanMove = isValue;
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