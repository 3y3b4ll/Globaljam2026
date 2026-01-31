using UnityEngine;

public class AutoHideUI : MonoBehaviour
{
    public CanvasGroup panel;   // StartDialoguePanel
    public float visibleTime = 3f;
    public float fadeTime = 1.5f;

    void Start()
    {
        StartCoroutine(HideRoutine());
    }

    System.Collections.IEnumerator HideRoutine()
    {
        // wait before fading
        yield return new WaitForSeconds(visibleTime);

        float t = 0f;

        while (t < fadeTime)
        {
            t += Time.deltaTime;
            panel.alpha = Mathf.Lerp(1f, 0f, t / fadeTime);
            yield return null;
        }

        panel.alpha = 0f;
        panel.gameObject.SetActive(false);
    }
}
