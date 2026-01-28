using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Code.Entities
{
    public class Entity : MonoBehaviour
    {
        #region Members 
        private Dictionary<Type, IEntityComponent> components = new();
        #endregion

        #region Init
        private void Awake()
        {
            GetEntityComponents();
            InitializeCompo();
            AfterInitCompo();
        }

        private void AfterInitCompo()
        {
            components.ToList().ForEach(kvp =>
            {
                if (kvp.Value is IAfterInitCompo afterInitCompo)
                    afterInitCompo.AfterInit();
            });
        }

        private void InitializeCompo() => components.Values.ToList().ForEach(compo => compo.InitCompo(this));

        public void GetEntityComponents() => components = GetComponentsInChildren<IEntityComponent>(true).ToDictionary(compo => compo.GetType());
        #endregion

        #region Methods
        public T GetCompo<T>(bool isAssignable = false) where T : class, IEntityComponent
        {
            if (components.TryGetValue(typeof(T), out var compo))
                return compo as T;
            if (isAssignable == false)
            {
                Debug.LogError($"Not Find {typeof(T)}");
                return null;
            }

            foreach (var kvp in components)
            {
                if (kvp.Value is T tComp)
                    return tComp;
            }

            Debug.LogError($"Not Find {typeof(T)}");
            return null;
        }
        #endregion
    }
}