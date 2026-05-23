using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoryManager : MonoBehaviour
{
    [SerializeField] private StoryData[] storyDatas;
    [SerializeField] private Image background;
    [SerializeField] private Image characterImage;
    [SerializeField] private TextMeshProUGUI storyText;
    [SerializeField] private TextMeshProUGUI characterName;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int storyIndex { get; private set; }
    public int characterIndex { get; private set; }
    private void Start()
    {
        setStoryElement(storyIndex, characterIndex);
    }

    private void setStoryElement(int _storyIndex, int _textIndex)
    {
        var storyElement = storyDatas[_storyIndex].stories[_textIndex];
        // 背景画像が設定されている時は描画、そうでないときは黒一色
        if (storyElement.Background != null)
        {
            background.sprite = storyElement.Background;
        }
        else
        {
            background.color = new Color32(0, 0, 0, 255);
        }
        // キャラクター画像が設定されている時は描画、そうでないときは透過
        if (storyElement.CharacterImage != null)
        {
            characterImage.sprite = storyElement.CharacterImage;
        }
        else
        {
            characterImage.color = new Color32(255, 255, 255, 0);
        }
        storyText.text = storyElement.StoryText;
        characterName.text = storyElement.CharacterName;
    }
}
