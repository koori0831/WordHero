using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/ChangeAnimationEvent")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "ChangeAnimationEvent", message: "Set [nextAnimation]", category: "Events", id: "98d30b0b7ba40f2e8376abe852b27ff3")]
public sealed partial class ChangeAnimationEvent : EventChannel<string> { }

