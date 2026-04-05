using GondrLib.Dependencies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Work.Core.Utils.EventBus;
using Work.Players.Code;

namespace Work.Core.RequestInjectors
{
    public class RequestInjector : MonoBehaviour
    {
        private const BindingFlags _bindingFlags =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private readonly Dictionary<Type, object> _registry = new Dictionary<Type, object>();

        private void Awake()
        {
            IEnumerable<IDependencyProvider> providers = FindMonoBehaviours().OfType<IDependencyProvider>();
            foreach (var provider in providers)
            {
                RegisterProvider(provider);
            }

            Bus<RequestInjectEvent, DependencyReturnValue>.Events += HandleRequestInjectEvent;
        }

        private DependencyReturnValue HandleRequestInjectEvent(RequestInjectEvent evt)
        {
            return new DependencyReturnValue((IDependencyProvider)Resolve(evt.ProviderType));
        }

        private object Resolve(Type type)
        {
            _registry.TryGetValue(type, out object instance);
            return instance;
        }

        private void RegisterProvider(IDependencyProvider provider)
        {
            //클래스 그 자체가 Provide되는 경우라서 별도의 리플렉션 없이 가져오면 된다.
            if (Attribute.IsDefined(provider.GetType(), typeof(ProvideAttribute)))
            {
                _registry.Add(provider.GetType(), provider);
                return;
            }

            MethodInfo[] methods = provider.GetType().GetMethods(_bindingFlags);

            foreach (var method in methods)
            {
                if (!Attribute.IsDefined(method, typeof(ProvideAttribute))) continue;

                //정의되었다면 매서드를 실행해서 리턴타입을 레지스트리에 담아야 해.
                Type returnType = method.ReturnType;
                object providedInstance = method.Invoke(provider, null);
                Debug.Assert(providedInstance != null, $"provided instance is null : {method.Name}");
                _registry.Add(returnType, providedInstance);
            }
        }

        private static MonoBehaviour[] FindMonoBehaviours()
        {
            return FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None); //정렬없이 모든 모노 가져오기
        }
    }
}