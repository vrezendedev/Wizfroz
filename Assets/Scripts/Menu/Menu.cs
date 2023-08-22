using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Levels;
using System.Collections.Generic;
using System.Linq;

public class Menu : MonoBehaviour
{
    [SerializeField] private UIDocument mainMenu;
    [SerializeField] private UIDocument optionsMenu;
    [SerializeField] private UIDocument playMenu;

    public AudioClip Click;
    public AudioClip StartLevel;
    public AudioClip Hover;

    private VisualElement rootElemMainMenu;
    private VisualElement rootOptionsMenu;
    private VisualElement rootPlayMenu;

    void Awake()
    {
        rootElemMainMenu = mainMenu.rootVisualElement;
        rootOptionsMenu = optionsMenu.rootVisualElement;
        rootPlayMenu = playMenu.rootVisualElement;

        rootPlayMenu.style.visibility = Visibility.Hidden;
        rootOptionsMenu.style.visibility = Visibility.Hidden;

        string[] keys = { "SfxVolume", "MusicVolume" };

        foreach (var key in keys)
        {
            if (!PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.SetFloat(key, 1);
            }
        }

        AudioEventManager.ChangeAudioVolume();

        SetMainMenuEvents();
        SetOptionsMenuEvents();
        SetPlayMenuEvents();
    }

    void Start()
    {
        GenerateLoadLevelButtons();
    }

    private void SetMainMenuEvents()
    {
        rootElemMainMenu.Q<Button>("LevelSelect").clicked += HandlePlayButtonClick;
        rootElemMainMenu.Q<Button>("Options").clicked += HandleOptionsButtonClick;
        rootElemMainMenu.Q<Button>("Exit").clicked += HandleExitButtonClick;

        rootElemMainMenu.Q<Button>("LevelSelect").RegisterCallback<MouseOverEvent>(callback => AudioEventManager.PlayUISound(Hover));
        rootElemMainMenu.Q<Button>("Options").RegisterCallback<MouseOverEvent>(callback => AudioEventManager.PlayUISound(Hover));
        rootElemMainMenu.Q<Button>("Exit").RegisterCallback<MouseOverEvent>(callback => AudioEventManager.PlayUISound(Hover));
    }

    private void HandleOptionsButtonClick()
    {
        AudioEventManager.PlayUISound(Click);
        rootElemMainMenu.style.visibility = Visibility.Hidden;
        rootPlayMenu.style.visibility = Visibility.Hidden;
        rootOptionsMenu.style.visibility = Visibility.Visible;
        var sliderSfx = rootOptionsMenu.Q<Slider>("FxVolume");
        var sliderMusic = rootOptionsMenu.Q<Slider>("MusicVolume");
        sliderSfx.value = PlayerPrefs.GetFloat("SfxVolume") * 100;
        sliderMusic.value = PlayerPrefs.GetFloat("MusicVolume") * 100;
        AudioEventManager.ChangeMusic(null, "SelectLevel");
    }

    private void HandlePlayButtonClick()
    {
        AudioEventManager.PlayUISound(Click);
        rootElemMainMenu.style.visibility = Visibility.Hidden;
        rootPlayMenu.style.visibility = Visibility.Visible;
        rootOptionsMenu.style.visibility = Visibility.Hidden;
        AudioEventManager.ChangeMusic(null, "SelectLevel");
    }

    private void HandleExitButtonClick()
    {
        Application.Quit();
    }

    private void SetOptionsMenuEvents()
    {
        rootOptionsMenu.Q<Slider>("FxVolume").RegisterValueChangedCallback(obj => HandleFxVolumeValueChange(obj));
        rootOptionsMenu.Q<Slider>("MusicVolume").RegisterValueChangedCallback(obj => HandleMusicVolumeValueChange(obj));
        rootOptionsMenu.Q<Slider>("FxVolume").RegisterCallback<ClickEvent>(obj =>
        {
            if (Input.GetMouseButtonUp(0))
            {
                AudioEventManager.PlayUISound(StartLevel);
            }
        });

        rootOptionsMenu.Q<Button>("Back").clicked += HandleBackToMainMenuButton;
        rootOptionsMenu.Q<Button>("Back").RegisterCallback<MouseOverEvent>(callback => AudioEventManager.PlayUISound(Hover));
    }

    private void HandleFxVolumeValueChange(ChangeEvent<float> e)
    {
        PlayerPrefs.SetFloat("SfxVolume", e.newValue / 100);
        AudioEventManager.ChangeAudioVolume();
    }

    private void HandleMusicVolumeValueChange(ChangeEvent<float> e)
    {
        PlayerPrefs.SetFloat("MusicVolume", e.newValue / 100);
        AudioEventManager.ChangeAudioVolume();
    }

    private void HandleBackToMainMenuButton()
    {
        AudioEventManager.PlayUISound(Click);
        rootElemMainMenu.style.visibility = Visibility.Visible;
        rootPlayMenu.style.visibility = Visibility.Hidden;
        rootOptionsMenu.style.visibility = Visibility.Hidden;
        AudioEventManager.ChangeMusic(null, "Menu");
    }

    private void GenerateLoadLevelButtons()
    {
        var levelContainer = rootPlayMenu.Q<VisualElement>("LevelContainer");
        levelContainer.Clear();

        int index = 0;
        bool wasPreviousCompleted = false;
        foreach (var level in Levels.LevelsInfo.Levels)
        {
            Button newButton = new Button();
            newButton.name = index.ToString();
            newButton.text = (index + 1).ToString();
            newButton.AddToClassList("buttons");
            newButton.AddToClassList("RoundButton");

            levelContainer.Add(newButton);

            if (wasPreviousCompleted || index == 0)
            {
                newButton.RegisterCallback<MouseOverEvent>((callback) => MouseOverOnLevelButton(callback));
                newButton.RegisterCallback<MouseLeaveEvent>((callback) => MouseLeaveOnLevelButton(callback));
                newButton.RegisterCallback<ClickEvent>((callback) => ClickOnLevelButton(callback));
                newButton.CaptureMouse();
            }
            else
            {
                newButton.SetEnabled(false);
            }

            index++;
            wasPreviousCompleted = level.PlayerAlreadyCompleted;
        }
    }

    private void MouseOverOnLevelButton(MouseOverEvent e)
    {

        AudioEventManager.PlayUISound(Hover);

        string level = e.target.GetType().GetProperty("name").GetValue(e.target).ToString();
        Level levelData = LevelsInfo.Levels.Where(obj => obj.Name == level).FirstOrDefault();

        var levelInfo = rootPlayMenu.Q<Label>("LevelInfo");
        VisualElement[] stars = new VisualElement[3] { rootPlayMenu.Q<VisualElement>("Star1"), rootPlayMenu.Q<VisualElement>("Star2"), rootPlayMenu.Q<VisualElement>("Star3") };

        for (int i = 0; i < levelData.PlayersStars; i++)
        {
            stars[i].style.visibility = Visibility.Visible;
        }

        string defaultText = $"Best Time: {levelData.PlayersBestTime} s - Octobears Collected: {levelData.PlayersOctobearsCollected} - Mana Shards Collected: {levelData.PlayersManaCollected} - Death Count: {levelData.PlayersDeathCount}";

        if (levelData.PlayersStars == 3)
        {
            levelInfo.text = $"{defaultText} <br> Congrats! You've achieved all stars of Level {int.Parse(level) + 1}...";
        }
        else
        {
            levelInfo.text = $"{defaultText} <br> You've achieved {levelData.PlayersStars} stars of Level {int.Parse(level) + 1}...";
        }
    }

    private void MouseLeaveOnLevelButton(MouseLeaveEvent e)
    {
        var levelInfo = rootPlayMenu.Q<Label>("LevelInfo");

        levelInfo.text = $"Click the Level button to play it or pass the mouse over <br> the button to get the current progress!";

        var firstStar = rootPlayMenu.Q<VisualElement>("Star1");
        var secondStar = rootPlayMenu.Q<VisualElement>("Star2");
        var thirdStar = rootPlayMenu.Q<VisualElement>("Star3");
        firstStar.style.visibility = Visibility.Hidden;
        secondStar.style.visibility = Visibility.Hidden;
        thirdStar.style.visibility = Visibility.Hidden;
    }

    private void ClickOnLevelButton(ClickEvent e)
    {
        string level = e.target.GetType().GetProperty("name").GetValue(e.target).ToString();
        LevelsInfo.CurrentLevel = int.Parse(level);
        AudioEventManager.ChangeMusic(null, level);
        AudioEventManager.PlayUISound(StartLevel);
        SceneManager.LoadScene(level);
    }

    private void SetPlayMenuEvents()
    {
        rootPlayMenu.Q<Button>("Back").clicked += HandleBackToMainMenuButton;
        rootPlayMenu.Q<Button>("Back").RegisterCallback<MouseOverEvent>(callback => AudioEventManager.PlayUISound(Hover));
    }
}
