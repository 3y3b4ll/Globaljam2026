using UnityEngine;
using TMPro;

public class DialoguePopup : MonoBehaviour
{
    public GameObject dialogueRoot;
    public TextMeshProUGUI dialogueText;
    public float visibleTime = 3f;

    bool isShowing;

    public void Show(string text)
    {
        if (isShowing) return;

        dialogueText.text = text;
        dialogueRoot.SetActive(true);
        isShowing = true;

        CancelInvoke();
        Invoke(nameof(Hide), visibleTime);
    }

    void Hide()
    {
        dialogueRoot.SetActive(false);
        isShowing = false;
    }
}
