using System.Collections.Generic;
using UnityEngine;
using Work.Core.Utils.EventBus;

namespace Work.Dialogue.Code
{
    public readonly record struct DialogueStartEvent(DialogueInformationSO DialogueSO) : IEvent;
    public readonly record struct ContinueDialogueEvent : IEvent;
    public readonly record struct DialogueSkipEvent : IEvent;
    public readonly record struct DialogueEndEvent(DialogueInformationSO DialogueSO) : IEvent;

    public readonly record struct DialogueProgressEvent(
        string DialogueDetail,
        string CharacterName,
        Sprite CharacterSprite,
        Sprite BackgroundSprite,
        NameTagPositionType NameTagPosition,
        bool HasChoices) : IEvent;

    public readonly record struct DialogueShowChoiceEvent(List<DialogueChoiceViewData> Choices) : IEvent;
    public readonly record struct DialogueChoiceSelectedEvent(int ChoiceIndex) : IEvent;
    public readonly record struct UIContinueButtonPressedEvent : IEvent;
    public readonly record struct DialogueTypingFinishedEvent : IEvent;
}
