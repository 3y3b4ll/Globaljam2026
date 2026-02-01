using UnityEngine;
using System.Collections;

public class SokkDirector : MonoBehaviour
{
    [Header("References")]
    public GameObject sokk;
    public NaarisokkChase sokkChase;
    public AudioSource sokkChaseAudio;

    public FPSController playerController;
    public Transform sokkLookTarget;

    public DialoguePopup dialogue;

    [Header("Timing")]
    public float spawnDelay = 1f;
    public float turnTime = 0.7f;
    public float chaseDelay = 1f;

    bool triggered = false;

    public void TriggerFirstPickup()
    {
        if (triggered) return;
        triggered = true;
        StartCoroutine(Sequence());
    }

    IEnumerator Sequence()
    {
        // spawn sokk
        sokk.SetActive(true);
        sokkChase.enabled = false;

        yield return new WaitForSeconds(spawnDelay);

        // lock controls
        playerController.locked = true;

        // smooth look at sokk
        yield return StartCoroutine(SmoothLookAt());

        // dialogue
        if (dialogue)
            dialogue.Show("Papa, what's wrong?");

        // chase audio
        if (sokkChaseAudio && !sokkChaseAudio.isPlaying)
            sokkChaseAudio.Play();

        yield return new WaitForSeconds(chaseDelay);

        // start chase
        sokkChase.enabled = true;

        // unlock
        playerController.locked = false;
    }

    IEnumerator SmoothLookAt()
    {
        Vector3 startDir = playerController.cameraTransform.forward;
        Vector3 targetDir =
            (sokkLookTarget.position - playerController.cameraTransform.position).normalized;

        float t = 0;

        while (t < turnTime)
        {
            t += Time.deltaTime;
            Vector3 dir = Vector3.Slerp(startDir, targetDir, t / turnTime);

            // controller-safe rotation
            playerController.ForceLookAt(
                playerController.cameraTransform.position + dir * 10f
            );

            yield return null;
        }

        // final snap exact
        playerController.ForceLookAt(sokkLookTarget.position);
    }
}
