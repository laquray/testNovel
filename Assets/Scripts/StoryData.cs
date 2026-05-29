using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Data", menuName = "StoryData")]
public class StoryData : ScriptableObject
{
    [SerializeReference, SubclassSelector]
    public List<StoryCommand> events = new List<StoryCommand>();
}

[System.Serializable]
public abstract class StoryCommand {
    public Sprite Background;
    public Sprite CharacterImage;
}

[System.Serializable]
public class SelifEvent : StoryCommand
{
    [TextArea]
    public string StoryText;
    public string CharacterName;
    public AudioClip SE;
    public float SEAmount = 1.0f;
}

[System.Serializable]
public class ChoiceEvent : StoryCommand
{
    public List<ChoiceData> ChoiceDataList;
}

[System.Serializable]
public class ChoiceData
{
    public string Text;
    public int JumpTargetEventNum;
}

[System.Serializable]
public class JumpStoryDataCommand : StoryCommand
{
    public StoryData JumpTargetStoryData;
}

[System.Serializable]
public class JumpEventCommand : StoryCommand
{
    public int JumpTargetEventNum;
}

[System.Serializable]
public class PlayMusic : StoryCommand
{
    public AudioClip BGM;
    public float BGMAmount = 1.0f;
}