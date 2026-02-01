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
        {
            stepTimer = 0f;
            return;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        bool moving = Mathf.Abs(x) > 0.1f || Mathf.Abs(z) > 0.1f;

        if (!moving)
        {
            stepTimer = 0f;
            return;
        }

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
        if (bank.Length == 0) return;

        footstepAudio.pitch = Random.Range(0.93f, 1.07f);
        footstepAudio.PlayOneShot(bank[Random.Range(0, bank.Length)]);
    }

}
