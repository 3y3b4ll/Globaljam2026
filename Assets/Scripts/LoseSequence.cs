using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class LoseSequence : MonoBehaviour
{
    [Header("Lose Audio")]
    public AudioSource loseAudio;
    public AudioClip loseClip;

    [Header("References")]
    public Volume globalVolume;
    public CanvasGroup fadeCanvas;
    public GameObject endUI;
    public FPSController playerController;

    [Header("Timings")]
    public float vignetteTarget = 0.85f;
    public float vignetteFadeTime = 1.5f;
    public float blackFadeTime = 1.2f;

    Vignette vignette;
    bool triggered = false;
    bool canRestart = false;

    // =========================
    // INIT
    // =========================

    void Start()
    {
        // ensure audio works after scene reload
        AudioListener.pause = false;
    }

    // =========================
    // UPDATE
    // =========================

    void Update()
    {
        if (canRestart && Input.GetKeyDown(KeyCode.R))
        {
            AudioListener.pause = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    // =========================
    // TRIGGER
    // =========================

    public void TriggerLose()
    {
        if (triggered) return;
        triggered = true;

        // allow lose audio to bypass pause
        if (loseAudio)
            loseAudio.ignoreListenerPause = true;

        // pause everything else
        AudioListener.pause = true;

        // play lose sting
        if (loseAudio && loseClip)
            loseAudio.PlayOneShot(loseClip);

        if (playerController)
            playerController.locked = true;

        StartCoroutine(LoseRoutine());
    }


    // =========================
    // SEQUENCE
    // =========================

    System.Collections.IEnumerator LoseRoutine()
    {
        yield return new WaitForSeconds(0.15f);

        // --- safe volume check ---
        if (globalVolume == null || globalVolume.profile == null)
        {
            Debug.LogError("LoseSequence: Volume missing");
            yield break;
        }

        if (!globalVolume.profile.TryGet(out vignette))
        {
            Debug.LogError("LoseSequence: No vignette in profile");
            yield break;
        }

        float start = vignette.intensity.value;
        float t = 0;

        // --- vignette close ---
        while (t < vignetteFadeTime)
        {
            t += Time.deltaTime;
            vignette.intensity.value =
                Mathf.Lerp(start, vignetteTarget, t / vignetteFadeTime);
            yield return null;
        }

        // --- black fade ---
        t = 0;
        while (t < blackFadeTime)
        {
            t += Time.deltaTime;

            if (fadeCanvas != null)
                fadeCanvas.alpha = t / blackFadeTime;

            yield return null;
        }

        if (endUI != null)
            endUI.SetActive(true);

        canRestart = true;
    }
}
