using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Behavior;
using Unity.Behavior.GraphFramework;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Work.Combat.Code;
using Work.Entities;

namespace Work.Enemies.Code
{
    public abstract class Enemy : MonoBehaviour, ICrowd, IDamageable, IKnockbackable
    {
        public UnityEvent<int> OnHitEvent;
        public UnityEvent<KnockbackData> OnKnockbackEvent;

        public EnemyManager Spawner { get; private set; }
        public BehaviorGraphAgent BehaviorAgent { get; private set; }
        public float NeighborRadius { get; set; } = 5.0f;
        public Guid Guid { get; } = Guid.NewGuid();
        public Transform Transform => gameObject != null ? transform : null;
        public NavMeshAgent NavAgent { get; private set; }

        [SerializeField] protected List<VariableSO> variableSOs = new List<VariableSO>();
        [SerializeField] protected LayerMask targetLayerMask;
        [SerializeField] protected float detectRange = 10.0f;
        [SerializeField] protected float chaseRange = 25.0f;

        protected Dictionary<BTVariables, SerializableGUID> guids = new Dictionary<BTVariables, SerializableGUID>();
        protected Dictionary<Type, IEnemyModule> _modules = new Dictionary<Type, IEnemyModule>();
        protected ChangeStateEvent _stateChangeChannel;
        public ChangeStateEvent StateChangeChannel => _stateChangeChannel;

        public void Init(EnemyManager spawner)
        {
            Spawner = spawner;
            BehaviorAgent = GetComponent<BehaviorGraphAgent>();
            NavAgent = GetComponent<NavMeshAgent>();
            Debug.Assert(BehaviorAgent != null, "BehaviorAgent component is missing.");
            AddModule();
            ModuleInit();
            ModuleAfterInit();
        }

        protected void ModuleAfterInit()
        {
            foreach (var module in _modules.Values)
            {
                if (module is IAfterInit afterInitModule)
                {
                    afterInitModule.AfterInitialize();
                }
            }
        }

        protected void ModuleInit()
        {
            foreach (var module in _modules.Values)
            {
                module.Initialize(this);
            }
        }

        protected void AddModule()
        {
            _modules = GetComponentsInChildren<IEnemyModule>(true).ToList().ToDictionary(item => item.GetType());

            string m = $"이름 : {name} \n";
            foreach (var kvp in _modules)
            {

                m += $"{kvp.Value.GetType().ToString()} \n";
            }
            Debug.Log(m);
        }

        protected void Start()
        {
            foreach (VariableSO item in variableSOs)
            {
                if (BehaviorAgent.GetVariableID(item.VariableName.ToString(), out SerializableGUID id))
                {
                    guids.Add(item.VariableName, id);
                }
                else
                    Debug.LogError($"Variable {item.VariableName} not found in BehaviorAgent.");
            }



            VariableSetting();
        }

        public virtual void VariableSetting()
        {
            _modules.Values.ToList().ForEach(item =>
            {
                if (item is IVariableModule variable)
                {
                    variable.BTInit();
                }
            });

            _stateChangeChannel = GetBlackboardVariable<ChangeStateEvent>(BTVariables.ChangeStateEvent).Value;
            SetBlackboardVariable<int>(BTVariables.TargetLayerNumber, targetLayerMask);
            SetBlackboardVariable<float>(BTVariables.DetectRange, detectRange);
            SetBlackboardVariable<float>(BTVariables.AttackRange, GetModule<EnemyAttackModule>(true).AttackRange);
            SetBlackboardVariable<float>(BTVariables.ChaseRange, chaseRange);
        }

        public BlackboardVariable<T> GetBlackboardVariable<T>(BTVariables variableName)
        {
            if (guids.TryGetValue(variableName, out SerializableGUID id))
            {
                if (BehaviorAgent.GetVariable(id, out BlackboardVariable<T> variable))
                    return variable;
            }

            Debug.LogError($"Variable {variableName} not found in BehaviorAgent.");
            return default;
        }

        public bool ExistVarialbe(BTVariables variableName) => guids.ContainsKey(variableName);

        public void SetBlackboardVariable<T>(BTVariables variableName, T value)
        {
            if (guids.TryGetValue(variableName, out SerializableGUID id))
            {
                BehaviorAgent.SetVariableValue(id, value);
                return;
            }

            Debug.LogError($"Variable {variableName} not found in BehaviorAgent.");
        }

        public T GetModule<T>(bool isAssignable = false) where T : class, IEnemyModule
        {
            if (_modules.TryGetValue(typeof(T), out var compo))
                return compo as T;
            if (isAssignable == false)
            {
                Debug.LogError($"Not Find {typeof(T)}");
                return null;
            }

            foreach (var kvp in _modules)
            {
                if (kvp.Value is T tComp)
                    return tComp;
            }

            Debug.LogError($"Not Find {typeof(T)}");
            return null;
        }

        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectRange);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, chaseRange);
        }

        public void TakeDamage(int damageAmount)
        {
            OnHitEvent?.Invoke(damageAmount);
        }

        public void TakeKnockback(KnockbackData knockbackData)
        {
            OnKnockbackEvent?.Invoke(knockbackData);
        }

        public void Die()
        {
            _stateChangeChannel.SendEventMessage(EnemyState.Death);
        }
    }
}
