using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class StoryManager : MonoBehaviour
{
    [SerializeField] private StoryData storyData;

    // 画像クロスフェード用変数
    [SerializeField] private GameObject oldBackground;
    [SerializeField] private GameObject newBackground;
    [SerializeField] private GameObject oldCharacterImage;
    [SerializeField] private GameObject newCharacterImage;
    [Header("SelifWindow")]
    [SerializeField] private GameObject selifWindow;
    [SerializeField] private TextMeshProUGUI storyText;
    [SerializeField] private GameObject nameWindow;
    [SerializeField] private TextMeshProUGUI characterName;
    [Header("ChoiceWindow")]
    [SerializeField] private GameObject choiceWindow;
    [SerializeField] private GameObject choiceButtonPrefab;
    [Header("Music")]
    [SerializeField] private AudioSource BGMSource;
    [SerializeField] private AudioSource SESource;

    [Header("Config")]
    [SerializeField, Range(0.1f, 2.0f)] public float SErate = 1.0f;
    [SerializeField, Range(0.1f, 2.0f)] public float BGMrate = 1.0f;
    public float textSpeed = 0.05f;
    [SerializeField] private float duration = 0.5f;



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
                Debug.Log("何も起こさない処理だよ");
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

        // フェード処理、同じ画像の時は処理しない(背景はnullをスルー)
        if (storyEvent.Background != null && oldBackground.GetComponent<Image>().sprite != storyEvent.Background)
        {
            yield return CrossFade(oldBackground, newBackground, storyEvent.Background);
        }

        if (oldCharacterImage.GetComponent<Image>().sprite != storyEvent.CharacterImage)
        {
            yield return CrossFade(oldCharacterImage, newCharacterImage, storyEvent.CharacterImage);
        }

        if (storyEvent is SelifEvent)
        {
            SelifEvent selifEvent = storyEvent as SelifEvent;
            selifWindow.gameObject.SetActive(true);
            choiceWindow.gameObject.SetActive(false);

            if (selifEvent.CharacterName == "") nameWindow.SetActive(false);
            else
            {
                nameWindow.SetActive(true);
                characterName.text = selifEvent.CharacterName;
            }

            if (selifEvent.SE != null)
            {
                SESource.volume = selifEvent.SEAmount * SErate;
                SESource.PlayOneShot(selifEvent.SE);
            }
                
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
        else if (storyEvent is PlayMusic)
        {
            PlayMusic playMusic = storyEvent as PlayMusic;
            BGMSource.clip = playMusic.BGM;
            BGMSource.volume = playMusic.BGMAmount * BGMrate;
            BGMSource.Play();
            eventIndex++;
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

    private IEnumerator CrossFade(GameObject _oldGo, GameObject _newGo, Sprite _targetSprite)
    {
        float time = 0;
        _newGo.GetComponent<Image>().sprite = _targetSprite;
        CanvasGroup _oldCg = _oldGo.GetComponent<CanvasGroup>();
        CanvasGroup _newCg = _newGo.GetComponent<CanvasGroup>();

        while (time < duration)
        {
            time += Time.deltaTime;

            float t = time / duration;

            if (_oldGo.GetComponent<Image>().sprite != null)
            {
                _oldCg.alpha = 1 - t;
            }
            if (_newGo.GetComponent<Image>().sprite != null)
            {
                _newCg.alpha = t;
            }
            yield return null;
        }

        _oldCg.alpha = 0;
        if (_newGo.GetComponent<Image>().sprite != null)
        {
            _newCg.alpha = 1;
        }
        else
        {
            _newCg.alpha = 0;
        }

        (_newCg.alpha, _oldCg.alpha) = (_oldCg.alpha, _newCg.alpha);

        Sprite tempS = _oldGo.GetComponent<Image>().sprite;
        _oldGo.GetComponent<Image>().sprite = _newGo.GetComponent<Image>().sprite;
        _newGo.GetComponent<Image>().sprite = tempS;
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
        foreach(Transform child in choiceWindow.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
