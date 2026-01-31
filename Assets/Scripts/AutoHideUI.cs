using UnityEngine;
using TMPro;

public class AutoHideUI : MonoBehaviour
{
    public float visibleTime = 3f;
    public float fadeTime = 1.5f;

    TextMeshProUGUI txt;

    void Start()
    {
        txt = GetComponent<TextMeshProUGUI>();
        Invoke(nameof(StartFade), visibleTime);
    }

    void StartFade()
    {
        StartCoroutine(Fade());
    }

    System.Collections.IEnumerator Fade()
    {
        float t = 0;

        Color c = txt.color;

        while (t < fadeTime)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(1, 0, t / fadeTime);
            txt.color = c;
            yield return null;
        }

        gameObject.SetActive(false);
    }
}
