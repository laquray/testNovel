using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class StoryManager : MonoBehaviour
{
    [SerializeField] private StoryData storyData;
    [SerializeField] private Image background;
    [SerializeField] private Image characterImage;
    [SerializeField] private GameObject selifWindow;
    [SerializeField] private TextMeshProUGUI storyText;
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private GameObject choiceWindow;
    [SerializeField] private GameObject choiceButtonPrefab;

    public float textSpeed = 0.05f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int eventIndex { get; private set; }

    private StoryEvent storyEvent;
    private bool finishText = false;
    private void Start()
    {
        storyText.text = "";
        eventIndex = 0;
        setStoryElement();
    }

    private void Update()
    {
        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            if (storyEvent is SelifEvent)
            {
                if (!finishText)
                {
                    finishText = true;
                }
                else
                {
                    eventIndex++;
                    storyText.text = "";
                    characterName.text = "";
                    setStoryElement();
                }
            }
            else if (storyEvent is ChoiceEvent)
            {
                Debug.Log("とりあえず進行するよ");
                eventIndex++;
                setStoryElement();
            }
            else if (storyEvent is JumpEvent)
            {
                Debug.Log("何も起こさない処理だよ");
            }
           
        }
    }

    private void setStoryElement()
    {
        storyEvent = storyData.events[eventIndex];
        // 背景画像が設定されている時は描画、そうでないときは黒一色
        if (storyEvent.Background != null)
        {
            background.sprite = storyEvent.Background;
        }
        else
        {
            background.color = new Color32(0, 0, 0, 255);
        }
        // キャラクター画像が設定されている時は描画、そうでないときは透過
        if (storyEvent.CharacterImage != null)
        {
            characterImage.sprite = storyEvent.CharacterImage;
        }
        else
        {
            characterImage.color = new Color32(255, 255, 255, 0);
        }
        if (storyEvent is SelifEvent)
        {
            SelifEvent selifEvent = storyEvent as SelifEvent;
            selifWindow.gameObject.SetActive(true);
            choiceWindow.gameObject.SetActive(false);
            
            characterName.text = selifEvent.CharacterName;
            string storyTextString = selifEvent.StoryText;
            //1文字づつ表示するコルーチン
            StartCoroutine(TypeSentence(storyTextString));
        }
        else if (storyEvent is ChoiceEvent)
        {
            choiceWindow.gameObject.SetActive(true);
            selifWindow.gameObject.SetActive(false);
            ChoiceEvent choiceEvent = storyEvent as ChoiceEvent;
            CreateChoices(choiceEvent);
        }
        else if (storyEvent is JumpEvent)
        {
            JumpEvent jumpEvent = storyEvent as JumpEvent;
            storyData = jumpEvent.JumpTargetStoryData;
            eventIndex = 0;
            setStoryElement();
        }
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

    private void CreateChoices(ChoiceEvent _choiceEvent)
    {
        foreach (var letter in _choiceEvent.Choices)
        {
            Debug.Log(letter.ToString());
        }
    }

    public void Choice(int _targetIndex)
    {
        eventIndex = _targetIndex;
        setStoryElement();
    }
}
