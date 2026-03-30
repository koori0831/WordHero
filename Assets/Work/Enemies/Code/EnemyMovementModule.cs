using System;
using UnityEngine;
using UnityEngine.AI;
using Work.Agents.Code;
using Work.Combat.Code;

namespace Work.Enemies.Code
{
    public class EnemyMovementModule : AgentMovementModule, IVariableModule, IAfterInitialize
    {
        private Enemy _enemy;
        private Transform _target;
        private NavMeshAgent _agent;
        private EnemyAnimatorModule _animator;
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
        private float defualtSpeed = 0f;
        [SerializeField] private float speedAnimationMultiflier = 1f;

        public override void Initialize(Agent agent)
        {
            base.Initialize(agent);
            _enemy = agent as Enemy;
            _animator = _enemy.GetModule<EnemyAnimatorModule>();
            _agent = _enemy.NavAgent;
            SetSpeed(Speed);
            defualtSpeed = Speed;
        }

        public void AfterInitialize()
        {
            _enemy.EnemyInfoData.StatusValue.OnstateusChangeEvent += HandleStatusChangeEvent;
        }

        private void HandleStatusChangeEvent(StatusType statusType, bool isTrue)
        {

            Debug.Log(statusType.ToString() + " : " + isTrue);

            switch(statusType)
            {
                case StatusType.Slow:
                    {
                        if(isTrue)
                        {
                            SetSpeed(defualtSpeed/10);
                        }
                        else
                            SetSpeed(defualtSpeed);
                    }
                    break;
            }
        }

        public void BTInit()
        {
            _enemy.SetBlackboardVariable<float>(BTVariables.RunSpeed, Speed);
            if (_enemy.ExistVarialbe(BTVariables.WalkSpeed))
                _enemy.SetBlackboardVariable<float>(BTVariables.WalkSpeed, Speed / 3);
        }

        public void SetTarget(Transform target)
        {
            _target = target;
        }

        public override void KnockBack(KnockbackData knockbackData)
        {
            SetStop(true); //네비게이션은 정지시켜주고

            base.KnockBack(knockbackData);

            WarpToPosition(_enemy.transform.position);
            SetStop(false); //넉백이 끝나면 다시 네비게이션을 시작합니다.
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
            Vector3 direction = target - _enemy.transform.position;
            direction.y = 0;
            Quaternion lookRotation = Quaternion.LookRotation(direction.normalized);

            if (isSmooth)
            {
                _enemy.transform.rotation = Quaternion.Slerp(_enemy.transform.rotation,
                                                lookRotation, Time.deltaTime * rotateSpeed);
            }
            else
            {
                _enemy.transform.rotation = lookRotation;
            }

            return lookRotation;
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