using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Data", menuName = "testStoryData")]
public class TestStoryData : ScriptableObject
{
    [SerializeReference]
    public List<StoryEvent> stories = new List<StoryEvent>();
}

[System.Serializable]
public abstract class StoryEvent
{
    
}

[System.Serializable]
public class Selif : StoryEvent
{
    public Sprite Background;
    public Sprite CharacterImage;
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

public class JumpEvent : StoryEvent
{
    public TestStoryData jumpTargetStoryData;
}