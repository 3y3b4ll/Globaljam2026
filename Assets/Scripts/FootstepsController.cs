using UnityEngine;

public class FootstepController : MonoBehaviour
{
    public CharacterController controller;
    public SafeZoneDetector safeZone;

    public AudioSource footstepAudio;

    public AudioClip[] snowClips;
    public AudioClip[] floorClips;

    public float stepInterval = 0.5f;
    float stepTimer;

    void Update()
    {
        if (!controller.isGrounded)
            return;

        if (controller.velocity.magnitude < 0.2f)
            return;

        stepTimer -= Time.deltaTime;

        if (stepTimer <= 0f)
        {
            PlayStep();
            stepTimer = stepInterval;
        }
    }

    void PlayStep()
    {
        AudioClip[] bank = safeZone.inSafeZone ? floorClips : snowClips;

        if (bank.Length == 0)
            return;

        var clip = bank[Random.Range(0, bank.Length)];
        footstepAudio.PlayOneShot(clip);
    }
}
