using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SpawnVignetteFade : MonoBehaviour
{
    public Volume globalVolume;
    public float startIntensity = 0.75f;
    public float endIntensity = 0.25f;
    public float fadeTime = 3f;

    Vignette vignette;

    void Start()
    {
        if (globalVolume.profile.TryGet(out vignette))
        {
            vignette.intensity.value = startIntensity;
            StartCoroutine(Fade());
        }
    }

    System.Collections.IEnumerator Fade()
    {
        float t = 0;

        while (t < fadeTime)
        {
            t += Time.deltaTime;
            float v = Mathf.Lerp(startIntensity, endIntensity, t / fadeTime);
            vignette.intensity.value = v;
            yield return null;
        }
    }
}
