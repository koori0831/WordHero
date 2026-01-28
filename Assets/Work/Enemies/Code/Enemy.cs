using System;
using System.Collections.Generic;
using System.Linq;
using Unity.AppUI.UI;
using Unity.Behavior;
using Unity.Behavior.GraphFramework;
using UnityEngine;
using Work.Entities;

namespace Work.Enemies.Code
{
    public abstract class Enemy : MonoBehaviour, ICrowd, IDamageable
    {
        public EnemyManager Spawner { get; private set; }
        public BehaviorGraphAgent BehaviorAgent { get; private set; }
        public float NeighborRadius { get; set; } = 5.0f;

        public Transform Transform => gameObject != null ? transform : null;

        [SerializeField] private List<VariableSO> variableSOs = new List<VariableSO>();
        [SerializeField] private LayerMask targetLayerMask;
        [SerializeField] private float detectRange = 10.0f;
        [SerializeField] private float chaseRange = 25.0f;
        [SerializeField] private float attackRange = 4.0f;

        private Dictionary<BTVariables, SerializableGUID> guids = new Dictionary<BTVariables, SerializableGUID>();
        private Dictionary<Type,IEnemyModule> _modules = new Dictionary<Type, IEnemyModule>();
        public Guid Guid { get; } = Guid.NewGuid();

        public void Init(EnemyManager spawner)
        {
            Spawner = spawner;
            BehaviorAgent = GetComponent<BehaviorGraphAgent>();
            Debug.Assert(BehaviorAgent != null, "BehaviorAgent component is missing.");
            AddModule();
            ModuleInit();
            ModuleAfterInit();
        }

        private void ModuleAfterInit()
        {
            foreach (var module in _modules.Values)
            {
                if(module is IAfterInit afterInitModule)
                {
                    afterInitModule.AfterInitialize();
                }
            }
        }

        private void ModuleInit()
        {
            foreach(var module in _modules.Values)
            {
                module.Initialize(this);
            }
        }

        private void AddModule()
        {
            _modules = GetComponentsInChildren<IEnemyModule>(true).ToList().ToDictionary(item => item.GetType());
        }

        private void Start()
        {
            foreach(VariableSO item in variableSOs)
            {
                if(BehaviorAgent.GetVariableID(item.VariableName.ToString(),out SerializableGUID id))
                {
                    guids.Add(item.VariableName, id);
                }
                else 
                    Debug.LogError($"Variable {item.VariableName} not found in BehaviorAgent.");
            }

            SetBlackboardVariable<int>(BTVariables.TargetLayerNumber, targetLayerMask);
            SetBlackboardVariable<float>(BTVariables.DetectRange, detectRange);
            SetBlackboardVariable<float>(BTVariables.AttackRange, attackRange);
            SetBlackboardVariable<float>(BTVariables.ChaseRange, chaseRange);
        }

        public BlackboardVariable<T> GetBlackboardVariable<T>(BTVariables variableName)
        {
            if(guids.TryGetValue(variableName, out SerializableGUID id))
            {
                if(BehaviorAgent.GetVariable(id,out BlackboardVariable<T> variable))
                    return variable;
            }

            Debug.LogError($"Variable {variableName} not found in BehaviorAgent.");
            return default;
        }

        public void SetBlackboardVariable<T>(BTVariables variableName, T value)
        {
            if (guids.TryGetValue(variableName, out SerializableGUID id))
            {
                BehaviorAgent.SetVariableValue(id, value);
                return;
            }

            Debug.LogError($"Variable {variableName} not found in BehaviorAgent.");
        }

        public T GetModule<T>()
        {
            if(_modules.TryGetValue(typeof(T), out IEnemyModule module))
            {
                return (T)module;
            }
            Debug.LogError($"Module of type {typeof(T)} not found.");
            return default;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectRange);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, chaseRange);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }

        public void TakeDamage(int damageAmount)
        {

        }
    }
}
