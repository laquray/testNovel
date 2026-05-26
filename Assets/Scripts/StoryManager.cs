using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class StoryManager : MonoBehaviour
{
    [SerializeField] private StoryData storyData;
    [SerializeField] private Image background;
    [SerializeField] private Image characterImage;
    [Header("SelifWindow")]
    [SerializeField] private GameObject selifWindow;
    [SerializeField] private TextMeshProUGUI storyText;
    [SerializeField] private TextMeshProUGUI characterName;
    [Header("ChoiceWindow")]
    [SerializeField] private GameObject choiceWindow;
    [SerializeField] private GameObject choiceButtonPrefab;

    [Header("Config")]
    public float textSpeed = 0.05f;
    [SerializeField] private float fadeSpeed = 0.01f;
    [SerializeField] private int fadeStep = 100;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int eventIndex { get; private set; }

    private StoryCommand storyEvent;
    private bool finishText = false;
    private void Start()
    {
        storyText.text = "";
        eventIndex = 0;
        StartCoroutine(setStoryElement());
    }

    private void Update()
    {
        // エンターキー押した時のそれぞれの処理
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
                    StartCoroutine(setStoryElement());
                }
            }

            // 仮で用意
            else if (storyEvent is ChoiceEvent)
            {
                Debug.Log("とりあえず進行するよ");
                eventIndex++;
                StartCoroutine(setStoryElement());
            }
            else if (storyEvent is JumpStoryDataCommand)
            {
                Debug.Log("何も起こさない処理だよ");
            }
           
        }
    }

    private IEnumerator setStoryElement()
    {
        storyEvent = storyData.events[eventIndex];
        // 背景画像が設定されている時は描画、そうでないときは黒一色
        if (storyEvent.Background != null)
        {
            background.sprite = storyEvent.Background;
            yield return FadeImage(background, new Color32(255, 255, 255, 255));
        }
        else
        {
            yield return FadeImage(background, new Color32(0, 0, 0, 255));
        }
        // キャラクター画像が設定されている時は描画、そうでないときは透過
        if (storyEvent.CharacterImage != null)
        {
            characterImage.sprite = storyEvent.CharacterImage;
            yield return FadeImage(characterImage, new Color32(255, 255, 255, 255));
        }
        else
        {
            yield return FadeImage(characterImage, new Color32(255, 255, 255, 0));
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
        else if (storyEvent is JumpStoryDataCommand)
        {
            JumpStoryDataCommand jumpEvent = storyEvent as JumpStoryDataCommand;
            storyData = jumpEvent.JumpTargetStoryData;
            eventIndex = 0;
            StartCoroutine(setStoryElement());
        }
        else if (storyEvent is JumpEventCommand)
        {
            JumpEventCommand jumpEvent = storyEvent as JumpEventCommand;
            eventIndex = jumpEvent.JumpTargetEventNum;
            StartCoroutine(setStoryElement());
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

    private IEnumerator FadeImage(Image _fadeImage, Color32 _targetColor)
    {
        float diffR = _targetColor.r - _fadeImage.color.r * 255.0f;
        float diffG = _targetColor.g - _fadeImage.color.g * 255.0f;
        float diffB = _targetColor.b - _fadeImage.color.b * 255.0f;
        float diffA = _targetColor.a - _fadeImage.color.a * 255.0f;
        float deltaR = diffR / fadeStep;
        float deltaG = diffG / fadeStep;
        float deltaB = diffB / fadeStep;
        float deltaA = diffA / fadeStep;

        if (diffR != 0 || diffG != 0 || diffB != 0 || diffA != 0)
        {
            for (int i = 0; i < fadeStep; i++)
            {
                _fadeImage.color = new Color32(
                    (byte)AddColor(_fadeImage.color.r * 255.0f, deltaR),
                    (byte)AddColor(_fadeImage.color.g * 255.0f, deltaG),
                    (byte)AddColor(_fadeImage.color.b * 255.0f, deltaB),
                    (byte)AddColor(_fadeImage.color.a * 255.0f, deltaA));
                yield return new WaitForSeconds(fadeSpeed);
            }
            _fadeImage.color = _targetColor;
        }
        yield return null;
    }

    private float AddColor(float _target, float _delta)
    {
        float result = _target + _delta;
        if (result < 0.0f) return 0.0f;
        else if (result > 255.0f) return 255.0f;
        return result;
    }

    private void CreateChoices(ChoiceEvent _choiceEvent)
    {
        foreach (ChoiceData choiceData in _choiceEvent.ChoiceDataList)
        {
            GameObject gameObject = Instantiate(choiceButtonPrefab, choiceWindow.transform);
            Button button = gameObject.GetComponent<Button>();
            TMP_Text text = gameObject.GetComponentInChildren<TMP_Text>();

            text.text = choiceData.Text;
            int jumpIndex = choiceData.JumpTargetEventNum;
            button.onClick.AddListener(() => { Choice(jumpIndex); });
        }
    }

    public void Choice(int _targetIndex)
    {
        eventIndex = _targetIndex;
        StartCoroutine(setStoryElement());
    }
}
