using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class LifeManager : MonoBehaviour
{
    public static LifeManager instance;
    public int maxLives = 5;
    public int currentLives;
    public float lifeRegenTime = 300f;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI timerText02;

    public float timer;
    bool fullOnceCheck = false;

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(instance);
    }

    void Start()
    {
        currentLives = PlayerPrefs.GetInt("CurrentLives", maxLives);
        timer = PlayerPrefs.GetFloat("LifeRegenTimer", lifeRegenTime);
        UpdateUI();
    }

    void Update()
    {
        if (currentLives < maxLives)
        {
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                currentLives++;
                timer = lifeRegenTime;

                UpdateUI();
            }

            UpdateTimerUI();
        }
        else
        {
            if(!fullOnceCheck)
            {
                timerText.text = "Full";
                if (timerText02 != null)
                    timerText02.text = "FUll";
                fullOnceCheck = true;
            }
        }
    }

    public void LoseLife()
    {
        if (currentLives > 0)
        {
            currentLives--;
            PlayerPrefs.SetInt("CurrentLives", currentLives);
            timer = lifeRegenTime;
            fullOnceCheck = false;
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        livesText.text = currentLives.ToString();
    }

    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(timer / 60F);
        int seconds = Mathf.FloorToInt(timer - minutes * 60);
        timerText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
        if (timerText02 != null)
            timerText02.text = string.Format("{0:0}:{1:00}", minutes, seconds);
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("CurrentLives", currentLives);
        PlayerPrefs.SetFloat("LifeRegenTimer", timer);
        PlayerPrefs.Save();
    }
}
