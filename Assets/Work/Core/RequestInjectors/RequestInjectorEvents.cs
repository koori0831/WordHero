using GondrLib.Dependencies;
using System;
using UnityEngine;
using Work.Core.Utils.EventBus;

namespace Work.Core.RequestInjectors
{
    public class RequestInjectorEvents
    {

    }

    public struct RequestInjectEvent : IEvent
    {
        public Type ProviderType { get; private set; }

        public RequestInjectEvent(Type prov)
        {
            ProviderType = prov;
        }
    }

    public struct DependencyReturnValue : IReturnValue
    {
        public IDependencyProvider dependencyProvider { get; private set; }

        public DependencyReturnValue(IDependencyProvider provider)
        {
            dependencyProvider = provider;
        }   
    }
}