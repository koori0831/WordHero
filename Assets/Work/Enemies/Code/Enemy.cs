using System.Collections.Generic;
using System.Linq;
using Unity.Behavior;
using Unity.Behavior.GraphFramework;
using UnityEngine;
using UnityEngine.AI;
using Work.Agents.Code;
using Work.Information.Code;

namespace Work.Enemies.Code
{
    public abstract class Enemy : Agent, ISelectable
    {
        protected ChangeStateEvent _stateChangeChannel;
        public ChangeStateEvent StateChangeChannel => _stateChangeChannel;
        public EnemyInfoDataSO EnemyInfoData { get; protected set; }
        public NavMeshAgent NavAgent { get; private set; }
        public BehaviorGraphAgent BehaviorAgent { get; private set; }
        public bool IsCanShowInfo { get; protected set; }


        [SerializeField] protected List<VariableSO> variableSOs = new List<VariableSO>();
        [SerializeField] protected EnemyInfoDataSO enemyInfoData;
        [SerializeField] protected LayerMask targetLayerMask;
        [SerializeField] protected float detectRange = 10.0f;
        [SerializeField] protected float chaseRange = 25.0f;

        protected Dictionary<BTVariables, SerializableGUID> guids = new Dictionary<BTVariables, SerializableGUID>();

        public override void Init()
        {
            BehaviorAgent = GetComponent<BehaviorGraphAgent>();
            NavAgent = GetComponent<NavMeshAgent>();
            Debug.Assert(BehaviorAgent != null, "BehaviorAgent component is missing.");
            IsCanShowInfo = EnemyInfoData != null;
            base.Init();
        }

        public override void InitInfo(InfoDataSO infoData)
        {
            base.InitInfo(infoData);
            EnemyInfoData = enemyInfoData.GetInfo(this);
            InitInfo(EnemyInfoData);
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



        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectRange);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, chaseRange);
        }



        public override void Die()
        {
            base.Die();
            _stateChangeChannel.SendEventMessage(EnemyState.Death);
        }
    }
}
