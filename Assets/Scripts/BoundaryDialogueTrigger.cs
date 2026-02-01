using UnityEngine;

public class BoundaryDialogueTrigger : MonoBehaviour
{
    public DialoguePopup dialogue;
    [TextArea]
    public string message = "I shouldn’t go any further…";

    public float cooldown = 5f;
    float timer;

    void Update()
    {
        if (timer > 0)
            timer -= Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && timer <= 0f)
        {
            dialogue.Show(message);
            timer = cooldown;
        }
    }
}
