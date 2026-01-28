using Code.Entities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.FSM
{
    public class StateCompo : MonoBehaviour, IEntityComponent, IAfterInitCompo
    {
        public Entity Owner { get; protected set; }
        public StateMachine StateMachine { get; private set; }

        [SerializeField] private List<StateSO> stateDataList;

        public void InitCompo(Entity entity)
        {
            Owner = entity;
            StateMachine = new StateMachine();
        }

        public void AfterInit()
        {
            foreach (var data in stateDataList)
            {
                Type type = Type.GetType(data.targetClass);
                if (type != null)
                {
                    try
                    {
                        int animationHash = data.animationHash;
                        var state = Activator.CreateInstance(type, StateMachine, Owner, animationHash) as State;
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
            StateMachine.Update();
        }

        public void TriggerEnd()
        {
            StateMachine.TriggerEnd();
        }
    }
}
