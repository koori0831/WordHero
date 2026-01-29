using System;
using Unity.Behavior;
using UnityEngine;
using Work.Enemies.Code;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/ChangeStateEvent")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "ChangeStateEvent", message: "Set [nextAnim]", category: "Events", id: "e36997c1e6fd0e1b8a9d91a1d2e393a2")]
public sealed partial class ChangeStateEvent : EventChannel<EnemyState> 
{
}


