using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Data", menuName = "StoryData")]
public class StoryData : ScriptableObject
{
    [SerializeReference, SubclassSelector]
    public List<StoryEvent> stories = new List<StoryEvent>();
}

[System.Serializable]
public abstract class StoryEvent {
    public Sprite Background;
    public Sprite CharacterImage;
}

[System.Serializable]
public class SelifEvent : StoryEvent
{
    [TextArea]
    public string StoryText;
    public string CharacterName;
}

[System.Serializable]
public class ChoiceEvent : StoryEvent
{
    public List<string> choices;
    public int JumpTargetIndex;
}

[System.Serializable]
public class JumpEvent : StoryEvent
{
    public StoryData jumpTargetStoryData;
}