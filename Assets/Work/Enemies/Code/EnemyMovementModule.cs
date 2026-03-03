using Code.Entities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Work.Combat.Code;

namespace Work.Enemies.Code
{
    public class EnemyMovementModule : MonoBehaviour, IEnemyModule, IVariableModule
    {
        private Enemy _owner;
        private Transform _target;
        private NavMeshAgent _agent;
        private EnemyAnimatorModule _animator;
        private List<ICrowd> nearNeighbors = new List<ICrowd>();
        private Vector3 _destination;
        private Vector3 velocity;

        //도착헀는가 , 현재 패스계산중이 아니고 남은 거리가 StopDistance보다 적다면 true;
        public bool IsArrived => !_agent.pathPending && _agent.remainingDistance < _agent.stoppingDistance + stopDistance;
        //남은거리
        public float RemainDistance => _agent.pathPending ? -1 : _agent.remainingDistance;
        public bool IsAutoMove { get; private set; } = false;
        public bool IsFocusingTarget { get; private set; }
        public bool IsMoving => velocity.magnitude > 0.1f;
        public bool IsCanMove { get; private set; } = true;

        [SerializeField] private float stopDistance = 0.05f;
        [SerializeField] private float rotateSpeed = 5f;
        [field: SerializeField] public float Speed { get; private set; } = 3f;
        [SerializeField] private float speedAnimationMultiflier = 1f;

        public void Initialize(Enemy enemy)
        {
            _owner = enemy;
            _animator = enemy.GetModule<EnemyAnimatorModule>();
            _agent = enemy.NavAgent;
            SetSpeed(Speed);
        }

        public void BTInit()
        {
            _owner.SetBlackboardVariable<float>(BTVariables.RunSpeed, Speed);
            if (_owner.ExistVarialbe(BTVariables.WalkSpeed))
                _owner.SetBlackboardVariable<float>(BTVariables.WalkSpeed, Speed / 3);
        }

        public void SetTarget(Transform target)
        {
            _target = target;
        }

        public void Update()
        {

            if (IsCanMove && _target != null)
            {
                NavMoveUpdate(_target.position);
            }

            if (IsAutoMove)
            {
                NavMoveUpdate(_destination);
            }

            RotateUpdate();
        }

        private void RotateUpdate()
        {
            if (IsCanMove)
            {
                if (IsFocusingTarget)
                {
                    if (_target != null)
                    {
                        LookAtTarget(_target.position);
                    }
                    else
                    {
                        LookAtTarget(_destination);
                    }
                }
                else if (IsArrived)
                {
                    LookAtTarget(_target.position);
                }
                else
                {
                    LookAtTarget(_agent.steeringTarget);
                }
            }
        }

        private void NavMoveUpdate(Vector3 targetPos)
        {
            if (Vector3.Distance(_agent.destination, targetPos) > 0.25f)
            {
                _agent.SetDestination(targetPos);
            }
        }

        public Quaternion LookAtTarget(Vector3 target, bool isSmooth = true)
        {
            Vector3 direction = target - _owner.transform.position;
            direction.y = 0;
            Quaternion lookRotation = Quaternion.LookRotation(direction.normalized);

            if (isSmooth)
            {
                _owner.transform.rotation = Quaternion.Slerp(_owner.transform.rotation,
                                                lookRotation, Time.deltaTime * rotateSpeed);
            }
            else
            {
                _owner.transform.rotation = lookRotation;
            }

            return lookRotation;
        }

        public async void KnockBack(KnockbackData knockbackData)
        {
            //넉백에 대한 면역이 있는지 체크하는건 EnemyKnockbackModule에서 하자.
            SetStop(true); //네비게이션은 정지시켜주고

            float duration = knockbackData.Duration;
            Vector3 direction = knockbackData.Direction.normalized;
            direction.y = 0; // 수평 방향으로만 넉백이 적용되도록 y축은 제거
            float currentTime = 0;
            float maxSpeed = knockbackData.Force;
            AnimationCurve moveCurve = knockbackData.KnockbackCurve;

            while (currentTime < duration)
            {
                float normalizeTime = currentTime / duration;
                float currentSpeed = maxSpeed * moveCurve.Evaluate(normalizeTime);
                Vector3 currentMovement = direction * currentSpeed;
                _owner.transform.Translate(currentMovement * Time.fixedDeltaTime, Space.World);
                currentTime += Time.fixedDeltaTime;
                await Awaitable.FixedUpdateAsync();
            }
            //여기서 추가 작업을 안해주면 넉백이 이상해진다. 일단 이상하게 해서 봅시다.
            WarpToPosition(_owner.transform.position);
            SetStop(false); //넉백이 끝나면 다시 네비게이션을 시작합니다.

        }

        public void SetMovement(bool isValue)
        {
            IsCanMove = isValue;
            if (_agent.enabled == false) return;
            _agent.isStopped = !isValue && !IsAutoMove;
        }

        public void SetAutoMove(bool isValue)
        {
            IsAutoMove = isValue;
            if (_agent.enabled == false) return;
            _agent.isStopped = !isValue && !IsCanMove;
        }

        public void SetDestination(Vector3 destination)
        {
            _destination = destination;
            if (_agent.enabled == false) return;
            _agent.SetDestination(destination);
        }

        public void SetSpeed(float speed)
        {
            this.Speed = speed;
            //float mul = IsPatroling ? speedAnimationMultiflier / 2 : speedAnimationMultiflier;
            _animator.SetParam(Animator.StringToHash("MOVE_SPEED"), speed * speedAnimationMultiflier);
            if (_agent.enabled == false) return;
            _agent.speed = speed;
        }

        public void SetRotate(Vector3 position)
        {
            LookAtTarget(position, false);
        }

        public void SetForcusingTarget(bool canForcusing) => IsFocusingTarget = canForcusing;

        public bool CanMovePoint(Vector3 movePoint)
        {
            NavMeshPath path = new NavMeshPath();
            if (_agent.enabled == false) return false;
            _agent.CalculatePath(movePoint, path);

            return path.status == NavMeshPathStatus.PathComplete;
        }

        public void SetStop(bool isStop) => _agent.isStopped = isStop;

        public void WarpToPosition(Vector3 position) => _agent.Warp(position);

        public void EnableRootMotion(bool enable)
        {
            _animator.Animator.applyRootMotion = enable;
            _agent.updatePosition = !enable;
        }


    }
}