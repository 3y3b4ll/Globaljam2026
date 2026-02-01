using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class LoseSequence : MonoBehaviour
{
    [Header("Lose Audio")]
    public AudioSource loseAudio;
    public AudioClip loseClip;

    public Volume globalVolume;
    public CanvasGroup fadeCanvas;
    public GameObject endUI;
    public FPSController playerController;


    public float vignetteTarget = 0.85f;
    public float vignetteFadeTime = 1.5f;
    public float blackFadeTime = 1.2f;

    Vignette vignette;
    bool triggered = false;
    bool canRestart = false;

    void Update()
    {
        if (canRestart && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void TriggerLose()
    {
        if (triggered) return;
        triggered = true;

        if (loseAudio && loseClip)
            loseAudio.PlayOneShot(loseClip);

        if (playerController)
            playerController.locked = true;


        StartCoroutine(LoseRoutine());
    }

    System.Collections.IEnumerator LoseRoutine()
    {
        yield return new WaitForSeconds(0.15f);

        globalVolume.profile.TryGet(out vignette);

        float start = vignette.intensity.value;
        float t = 0;

        // vignette close
        while (t < vignetteFadeTime)
        {
            t += Time.deltaTime;
            vignette.intensity.value = Mathf.Lerp(start, vignetteTarget, t / vignetteFadeTime);
            yield return null;
        }

        // black fade
        t = 0;
        while (t < blackFadeTime)
        {
            t += Time.deltaTime;
            fadeCanvas.alpha = t / blackFadeTime;
            yield return null;
        }

        endUI.SetActive(true);
        canRestart = true;
    }
}
