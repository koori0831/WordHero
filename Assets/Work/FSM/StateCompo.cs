using System;
using System.Collections.Generic;
using UnityEngine;
using Work.Agents.Code;

namespace Code.FSM
{
    public class StateCompo : MonoBehaviour, IAgentModule, IAfterInitialize
    {
        public Agent Owner { get; protected set; }
        public StateMachine StateMachine { get; private set; }

        [SerializeField] private List<StateSO> stateDataList;

        public void Initialize(Agent owner)
        {
            Owner = owner;
            StateMachine = new StateMachine();
        }

        public void AfterInitialize()
        {
            foreach (var data in stateDataList)
            {
                Type type = Type.GetType(data.targetClass);
                if (type != null)
                {
                    try
                    {
                        int animationHash = data.animationHash;
                        State state = Activator.CreateInstance(type, StateMachine, Owner, animationHash) as State;
                        StateMachine.AddState(data.stateName, state);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"[StateCompo] Failed to create state {data.stateName}: {e.Message}");
                    }
                }
                else
                {
                    Debug.LogError($"[StateCompo] Class not found: {data.targetClass}");
                }
            }

            if (stateDataList.Count > 0)
            {
                StateMachine.ChangeState(stateDataList[0].stateName);
            }
        }

        private void Update()
        {
            StateMachine?.Update();
        }

        public void TriggerEvent(AnimationEventType eventType)
        {
            StateMachine?.TriggerEvent(eventType);
        }

        private void OnDestroy()
        {
            StateMachine?.DisposeAll();
        }
    }
}
