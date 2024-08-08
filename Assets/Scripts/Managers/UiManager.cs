using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using CandyCoded;
using CandyCoded.HapticFeedback;

public class UiManager : MonoBehaviour
{
    public static UiManager instance;

    [Header("Menus")]
    [SerializeField] GameObject playingMenu;
    [SerializeField] GameObject levelCompleteMenu;
    [SerializeField] GameObject levelFailedMenu;
    [SerializeField] GameObject SettingsMenu;
    [SerializeField] ParticleSystem CheersParticleEff;

    [SerializeField] Animator settingsAnimator;

    [Header("Toggle")]
    [SerializeField] Toggle hapticsToggle;
    [SerializeField] Toggle darkModeToggle;
    [SerializeField] Color darkModeColor;
    [SerializeField] Image[] bgImages;
    bool inDarkMode = false;
    [HideInInspector]
    public bool enableHaptics = true;
    Camera cam;

    [Header("Lives")]
    [SerializeField] GameObject NoLivesMenu;

    [Header("LevelNo")]
    [SerializeField] TextMeshProUGUI levelNoText;
    [SerializeField] TextMeshProUGUI[] levelNoTextwithName;

    [Header("PowerUp")]
    [SerializeField] int undoCount = 1;
    [SerializeField] GameObject undoHeadObj;
    [SerializeField] TextMeshProUGUI undoCountText;
    [SerializeField] int eraseCount = 0;
    [SerializeField] GameObject eraseHeadObj;
    [SerializeField] TextMeshProUGUI eraseCountText;
    [SerializeField] int phantomCount = 0;
    [SerializeField] GameObject phantomHeadOBj;
    [SerializeField] TextMeshProUGUI phantomCountText;


    [Header("MainMenu")]
    [SerializeField] bool mainMenu = false;
    [SerializeField] int selectedLevel = 1;

    [Header("Level Nos Buttons")]
    [SerializeField]int currentLevel = 1;
    [SerializeField] TextMeshProUGUI[] levelNoTexts;
    

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(instance);
    }

    private void Start()
    {
        if (playingMenu != null)
            playingMenu.SetActive(true);
        if (levelCompleteMenu != null)
            levelCompleteMenu.SetActive(false);
        if (levelFailedMenu != null)
            levelFailedMenu.SetActive(false);
        if (SettingsMenu != null)
            SettingsMenu.SetActive(false);

        if (cam == null)
            cam = Camera.main;

        if (PlayerPrefs.GetInt("Haptics", 1) == 0)
        {
            hapticsToggle.isOn = false;
            EnableHapticsButton(false);
        }
        else
        {
            hapticsToggle.isOn = true;
            EnableHapticsButton(true);
        }

        if (PlayerPrefs.GetInt("DarkMode", 0) == 0)
        {
            darkModeToggle.isOn = false;
            EnableDarkMode(false);
        }
        else
        {
            darkModeToggle.isOn = true;
            EnableDarkMode(true);
        }


        if(levelNoText != null)
            levelNoText.text = SceneManager.GetActiveScene().buildIndex.ToString();

        foreach (TextMeshProUGUI _text in levelNoTextwithName)
        {
            _text.text = "LEVEL " + SceneManager.GetActiveScene().buildIndex.ToString();
        }

        if(undoCountText != null)
            undoCountText.text = undoCount.ToString();
        if (eraseCountText != null)
            eraseCountText.text = eraseCount.ToString();
        if (phantomCountText != null)
            phantomCountText.text = phantomCount.ToString();

        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        if(levelNoTexts.Length > 0)
        {
            for (int i = 0; i < levelNoTexts.Length; i++)
            {
                levelNoTexts[i].text = (i + currentLevel).ToString();
            }
        }

    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }
    }

    #region Settings
    public void EnableSettings()
    {
        PlayHaptic();
        if (SettingsMenu != null)
            SettingsMenu.SetActive(true);
        if (settingsAnimator != null)
            settingsAnimator.SetTrigger("Play");
        //anima
    }

    public void DisableSettings()
    {
        PlayHaptic();
        if (SettingsMenu != null)
            SettingsMenu.SetActive(false);
    }
    #endregion


    public void LevelComplete()
    {
        currentLevel++;
        PlayerPrefs.SetInt("CurrentLevel", currentLevel);
        if (CheersParticleEff != null)
            CheersParticleEff.Play();
        if(playingMenu!= null)
            playingMenu.SetActive(false);
        if (levelCompleteMenu != null)
            levelCompleteMenu.SetActive(true);
        if (SettingsMenu != null)
            SettingsMenu.SetActive(false);

    }

    public void LevelFailed()
    {
        if (playingMenu != null)
            playingMenu.SetActive(false);
        if (levelFailedMenu != null)
            levelFailedMenu.SetActive(true);
        if (SettingsMenu != null)
            SettingsMenu.SetActive(false);
        if(enableHaptics)
            Handheld.Vibrate();
    }



    #region Scene Loadings
    public void NextLevel()
    {
        PlayHaptic();
        if (LifeManager.instance != null)
            PlayerPrefs.SetFloat("LifeRegenTimer", LifeManager.instance.timer);
        if (SceneExists(SceneManager.GetActiveScene().buildIndex + 1))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
            Debug.Log("There are no more levels");

    }

    bool SceneExists(int buildIndex)
    {
        string scenePath = SceneUtility.GetScenePathByBuildIndex(buildIndex);
        return !string.IsNullOrEmpty(scenePath);
    }

    public void Restart()
    {
        PlayHaptic();
        if (LifeManager.instance != null)
        {
            LifeManager.instance.LoseLife();
            PlayerPrefs.SetFloat("LifeRegenTimer", LifeManager.instance.timer);
            if (LifeManager.instance.currentLives > 0)
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            else
                SceneManager.LoadScene(0);
        }
    }

    public void HomeButton()
    {
        PlayHaptic();
        if (LifeManager.instance != null)
            LifeManager.instance.LoseLife();
        if (LifeManager.instance != null)
            PlayerPrefs.SetFloat("LifeRegenTimer", LifeManager.instance.timer);
        SceneManager.LoadScene(0);
    }
    public void LoadSelectedLevel()
    {
        PlayHaptic();
        if (LifeManager.instance != null)
            PlayerPrefs.SetFloat("LifeRegenTimer", LifeManager.instance.timer);
        if (LifeManager.instance != null)
            if (LifeManager.instance.currentLives > 0)
                SceneManager.LoadScene(currentLevel);
            else
                if (NoLivesMenu != null)
                NoLivesMenu.SetActive(true);
    }
    #endregion

    public void SelectLevelButton(int _levelNo)
    {
        PlayHaptic();
        selectedLevel = _levelNo;
    }


    #region Haptics
    public void PlayHaptic()
    {
        if (enableHaptics)
        {
            HapticFeedback.LightFeedback();
        }
    }
    public void EnableHapticsButton(bool _enable)
    {
        enableHaptics = _enable;
        if (enableHaptics)
            PlayerPrefs.SetInt("Haptics", 1);
        else
            PlayerPrefs.SetInt("Haptics", 0);
    }

    #endregion


    #region UNDO Feature
    public void ActivateUNDOUI()
    {
        PlayHaptic();
        if (undoCount > 0)
        {
            if (playingMenu != null)
                playingMenu.SetActive(false);
            undoHeadObj.SetActive(true);
            if (LevelManager.instance != null)
                LevelManager.instance.ActivateUNDO();
        }

    }

    public void DeactivateUNDOUI()
    {
        PlayHaptic();
        if (playingMenu != null)
            playingMenu.SetActive(true);
        undoHeadObj.SetActive(false);
        if (LevelManager.instance != null)
            LevelManager.instance.DeActivateUNDO();
    }

    public void UpdatePowerUpCount(int _decrement)
    {
        undoCount -= _decrement;
        undoCountText.text = undoCount.ToString();
    }
    #endregion

    #region Erase PowerUp
    public void ActivateErasePowerUp()
    {
        PlayHaptic();
        if(eraseCount > 0)
        {
            if (playingMenu != null)
                playingMenu.SetActive(false);
            eraseHeadObj.SetActive(true);
            if (LevelManager.instance != null)
                LevelManager.instance.ActivateErase();
        }
    }

    public void DeactivateErasePowerUp()
    {
        PlayHaptic();
        if (playingMenu != null)
            playingMenu.SetActive(true);
        eraseHeadObj.SetActive(false);
        if (LevelManager.instance != null)
            LevelManager.instance.DeactivateErase();
    }

    public void updateEraseCount(int _decrement)
    {
        eraseCount -= _decrement;
        eraseCountText.text = eraseCount.ToString();
    }
    #endregion

    #region Phantom PowerUp
    public void ActivatePhantom()
    {
        PlayHaptic();
        if (phantomCount > 0)
        {
            if (playingMenu != null)
                playingMenu.SetActive(false);
            phantomHeadOBj.SetActive(true);
            if (LevelManager.instance != null)
                LevelManager.instance.ActivatePhantom();
        }
    }

    public void DeactivatePhantom()
    {
        PlayHaptic();
        if (playingMenu != null)
            playingMenu.SetActive(true);
        phantomHeadOBj.SetActive(false);
        if (LevelManager.instance != null)
            LevelManager.instance.DeactivatePhantom();
    }

    public void UpdatePhantomCount(int _decrement)
    {
        phantomCount -= _decrement;
        phantomCountText.text = phantomCount.ToString();
    }
    #endregion


    public void EnableDarkMode(bool _enable)
    {
        inDarkMode = _enable;        
        if (inDarkMode)
        {
            cam.backgroundColor = darkModeColor;
            foreach (Image _image in bgImages)
            {
                _image.color = darkModeColor;
            }
            PlayerPrefs.SetInt("DarkMode", 1);
        }
        else
        {
            cam.backgroundColor = Color.white;
            foreach (Image _image in bgImages)
            {
                _image.color = Color.white;
            }
            PlayerPrefs.SetInt("DarkMode", 0);
        }
    }
}
