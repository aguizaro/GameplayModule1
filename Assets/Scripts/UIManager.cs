using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public enum UIState
{
    Main,
    Settings,
    Credits,
    HUD,
    Pause
}

public class UserSettings : MonoBehaviour
{
    public static string PlayerName {get; set;}
    public static float ProjectileSize { get; set; }
    public static bool IsPaused { get; set; }
    public static bool IsGameOver { get; set; }

}

public class UIManager : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private float textFadeOutSeconds = 8f;

    private float fadeSec;

    // slider
    private Slider _slider;
    private TMP_Text _sliderVal;
    // nickname and projectlile size
    private TMP_InputField _nameInput;
    private TMP_Text _nameText;
    private TMP_Text _sizeText;
    // score text
    private TMP_Text _scoreCount;
    private TMP_Text _incrementText;
    private TMP_Text _lootText;
    // timer text
    private TMP_Text _timerCount;
    private UIState _previousState;
    private UIState _currentState;
    //Name placeholder text
    private TMP_Text _placeHolderText;


    private readonly UIState _startState = UIState.Main;

    private void Awake()
    {

        UserSettings.IsPaused = false;
        UserSettings.IsGameOver = true;
        UserSettings.PlayerName = "Guest User";
        UserSettings.ProjectileSize = 0.5f;

        _canvas.enabled = true;
        // Assign button controls
        _canvas.transform.Find("Main").transform.Find("StartButton").GetComponent<Button>().onClick.AddListener(PlayGame);
        _canvas.transform.Find("Main").transform.Find("SettingsButton").GetComponent<Button>().onClick.AddListener(PlaySettings);
        _canvas.transform.Find("Main").transform.Find("CreditsButton").GetComponent<Button>().onClick.AddListener(PlayCredits);

        _canvas.transform.Find("Settings").transform.Find("ConfirmButton").GetComponent<Button>().onClick.AddListener(ConfirmSettings);
        _canvas.transform.Find("Settings").transform.Find("BackButton").GetComponent<Button>().onClick.AddListener(PlayPrev);

        _canvas.transform.Find("Credits").transform.Find("BackButton").GetComponent<Button>().onClick.AddListener(PlayPrev);

        _canvas.transform.Find("Pause").transform.Find("ResumeButton").GetComponent<Button>().onClick.AddListener(PlayerUnPause);
        _canvas.transform.Find("Pause").transform.Find("QuitButton").GetComponent<Button>().onClick.AddListener(QuitGame);
        _canvas.transform.Find("Pause").transform.Find("SettingsButton").GetComponent<Button>().onClick.AddListener(PlaySettings);
        _canvas.transform.Find("Pause").transform.Find("CreditsButton").GetComponent<Button>().onClick.AddListener(PlayCredits);
        // Set slider props
        _slider = _canvas.transform.Find("Settings").transform.Find("SizeSlider").GetComponent<Slider>();
        _slider.onValueChanged.AddListener(delegate { UpdateSliderVal(); });
        _sliderVal = _slider.transform.Find("SizeValue").GetComponent<TMP_Text>();
        // set nickname props
        _nameInput = _canvas.transform.Find("Settings").transform.Find("NameInput").transform.Find("InputField").GetComponent<TMP_InputField>();
        _placeHolderText = _canvas.transform.Find("Settings").transform.Find("NameInput").transform.Find("InputField").transform.Find("TextArea").transform.Find("Placeholder").GetComponent<TMP_Text>();
        _nameText = _canvas.transform.Find("HUD").transform.Find("TopLeft").transform.Find("Nickname").transform.Find("NameText").GetComponent<TMP_Text>();
        _sizeText = _canvas.transform.Find("HUD").transform.Find("TopLeft").transform.Find("Size").transform.Find("SizeText").GetComponent<TMP_Text>();
        // set score Props
        _scoreCount = _canvas.transform.Find("HUD").transform.Find("Score").transform.Find("ScoreCount").GetComponent<TMP_Text>();
        _lootText = _canvas.transform.Find("HUD").transform.Find("Loot").transform.Find("LootText").GetComponent<TMP_Text>();
        _incrementText = _canvas.transform.Find("HUD").transform.Find("Loot").transform.Find("IncrementText").GetComponent<TMP_Text>();
        // set timer props
        _timerCount = _canvas.transform.Find("HUD").transform.Find("Timer").transform.Find("TimerCount").GetComponent<TMP_Text>();

        _currentState = _startState;
        StartUI(_startState);
    }
    private void StartUI(UIState state)
    {
        
        Debug.Log("Prev: " + _previousState + "\nCurrent: " + state);

        if(_currentState != _previousState) _previousState = _currentState;

        // disable all child canvases
        foreach (Transform child in _canvas.transform)
        {
            Canvas childCanvas = child.GetComponent<Canvas>();
            if (child != null) childCanvas.enabled = false;
        }

        // enable the selected canvas
        Canvas selectedCanvas = GetCanvasForState(state);
        if (selectedCanvas != null)
        {
            selectedCanvas.enabled = true;
            _currentState = state;
        }

        // unlock/lock cursor depending on selected canvas
        if (_currentState != UIState.HUD)
        {
            //unlock cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            // lock the cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

    }

    private Canvas GetCanvasForState(UIState state)
    {
        string canvasName = state.ToString();
        Transform selectedCanvasTransform = _canvas.transform.Find(canvasName);

        if (selectedCanvasTransform != null)
        {
            return selectedCanvasTransform.GetComponent<Canvas>();
        }

        return null;
    }

    private void ApplySettings(string name, float size)
    {
        UserSettings.PlayerName = name;
        UserSettings.ProjectileSize = size;

        _nameText.text = !string.IsNullOrEmpty(name) ? name : "Guest User";
        _sizeText.text = "Size :     " + ((int)(size * 100)).ToString() + "%"; //get percent

        StartUI(_previousState);

        // After screen has deactivated, update the placeholder name in settings
        _placeHolderText.text = !string.IsNullOrEmpty(name) ? name : "Yurname...";
    }

    private void UpdateSliderVal() => _sliderVal.text = ((int)(_slider.value * 100)).ToString() + " %";

    // public functions used for buttons
    public void PlayMain() => StartUI(UIState.Main);
    public void PlayPrev() => StartUI(_previousState);
    public void PlaySettings() => StartUI(UIState.Settings);
    public void PlayCredits() => StartUI(UIState.Credits);
    public void ConfirmSettings() => ApplySettings(_nameInput.text, _slider.value);

    // public functions used for UI Text updates
    public void UpdateTimerCount(int count) => _timerCount.text = count.ToString();
    public void UpdateScoreCount(int score) => _scoreCount.text = score.ToString();
    public void UpdateLootText(string description) => _lootText.text = description;
    public void UpdateIncrement(int value) => _incrementText.text = "+ " + value.ToString();
    public void StartFadeOut() => StartCoroutine(nameof(FadeOut));

    public void PlayerPause()
    {
        Debug.Log("Player Pause");
        UserSettings.IsPaused = true;

        StartUI(UIState.Pause);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        _gameManager.EndGame();
        StartUI(UIState.Main);
    }

    public void PlayerUnPause()
    {
        Debug.Log("Player Resume");
        UserSettings.IsPaused = false;
        StartUI(UIState.HUD);
    }

    public void PlayAgain()
    {
        PlayMain();
        _canvas.transform.Find("Main").transform.Find("StartButton").transform.Find("Text").GetComponent<TMP_Text>().text = "Play Again";

    }

    public void PlayGame()
    {
        StartUI(UIState.HUD);
        // slight delay to avoid immediate shoot
        Invoke(nameof(StartGame), 0.25f);
        // set start text back to default
        _canvas.transform.Find("Main").transform.Find("StartButton").transform.Find("Text").GetComponent<TMP_Text>().text = "PLAY NOW";
    }
    // encapsulates game manager StartGame() to be called after delay
    private void StartGame() => _gameManager.StartGame(); 

    // fade out loot text and loot value
    public IEnumerator FadeOut()
    {
        fadeSec = textFadeOutSeconds;
        while (fadeSec > 2f)
        {
            if (_incrementText != null && _lootText != null)
            {
                float alpha = fadeSec / textFadeOutSeconds;
                _incrementText.color = new Color(_incrementText.color.r, _incrementText.color.g, _incrementText.color.b, alpha);
                _lootText.color = new Color(_lootText.color.r, _lootText.color.g, _lootText.color.b, alpha);
            }
            fadeSec -= Time.deltaTime;
            yield return null;
        }

        // remove text and reset alpha
        _incrementText.text = "";
        _lootText.text = "";
        _incrementText.color = new Color(_incrementText.color.r, _incrementText.color.g, _incrementText.color.b);
        _lootText.color = new Color(_lootText.color.r, _lootText.color.g, _lootText.color.b);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !UserSettings.IsGameOver){
            if (!UserSettings.IsPaused) PlayerPause();
            else PlayerUnPause();
        }
    }

}
