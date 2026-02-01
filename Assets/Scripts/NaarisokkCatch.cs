using UnityEngine;

public class NaarisokkCatch : MonoBehaviour
{
    public LoseSequence loseSequence;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            loseSequence.TriggerLose();
        }
    }
}
