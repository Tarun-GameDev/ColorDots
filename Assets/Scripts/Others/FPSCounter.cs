using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    public TextMeshProUGUI fpsText;
    private int frameCount = 0;
    private float deltaTime = 0.0f;
    private float fps = 0.0f;
    private float updateInterval = 0.5f;

    void Start()
    {
        if (!fpsText)
        {
            Debug.LogError("FPSCounter: No Text component assigned.");
        }
    }

    void Update()
    {
        frameCount++;
        deltaTime += Time.unscaledDeltaTime;
        if (deltaTime > updateInterval)
        {
            fps = frameCount / deltaTime;
            frameCount = 0;
            deltaTime -= updateInterval;

            if (fpsText)
            {
                fpsText.text = string.Format("FPS: {0:0.0}", fps);
            }
        }
    }
}
