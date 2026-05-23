using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class StoryManager : MonoBehaviour
{
    [SerializeField] private StoryData[] storyDatas;
    [SerializeField] private Image background;
    [SerializeField] private Image characterImage;
    [SerializeField] private TextMeshProUGUI storyText;
    [SerializeField] private TextMeshProUGUI characterName;

    public float textSpeed = 0.05f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int storyIndex { get; private set; }
    public int textIndex { get; private set; }

    private bool finishText = false;
    private void Start()
    {
        storyText.text = "";
        setStoryElement(storyIndex, textIndex);
    }

    private void Update()
    {
        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            if (!finishText)
            {
                finishText = true;
            }
            else
            {
                textIndex++;
                storyText.text = "";
                characterName.text = "";
                setStoryElement(storyIndex, textIndex);
            }
        }
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
        characterName.text = storyElement.CharacterName;
        string storyTextString = storyElement.StoryText;
        //1文字づつ表示するコルーチン
        StartCoroutine(TypeSentence(storyTextString));
    }

    private IEnumerator TypeSentence(string _storyTextString)
    {
        finishText = false;
        foreach(var letter in _storyTextString.ToCharArray())
        {
            if (finishText) 
            {
                storyText.text = _storyTextString;
                break;
            }
            storyText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }
        finishText = true;
    }
}
