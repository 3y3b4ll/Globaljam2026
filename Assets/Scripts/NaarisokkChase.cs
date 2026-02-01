using UnityEngine;

public class NaarisokkChase : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public SafeZoneDetector SafeZoneDetector;

    [Header("Speeds")]
    public float wanderSpeed = 1.1f;
    public float chaseSpeed = 2.6f;

    [Header("Behavior")]
    public float chaseRadius = 18f;
    public float maxRoamDistance = 45f;

    [Header("Ground Snap")]
    public float rayStartHeight = 10f;
    public float groundOffset = 0.05f;
    public LayerMask groundLayer;

    [Header("House Avoidance")]
    public Transform[] houseCenters;
    public float avoidRadius = 7f;
    public float avoidStrength = 2.5f;

    [Header("Wander Control")]
    public float wanderSegmentTime = 3f;

    [Header("Bounds")]
    public float wallCheckDistance = 1.6f;
    public LayerMask wallLayer;

    // =========================
    // AUDIO SOURCES
    // =========================

    [Header("Audio Sources")]
    public AudioSource pantingAudio;       // loop breathing
    public AudioSource chaseVoiceAudio;    // loud shouts
    public AudioSource wanderVoiceAudio;   // quiet distant voices

    [Header("Wander Voices")]
    public AudioClip[] wanderVoiceClips;
    public Vector2 wanderVoiceInterval = new Vector2(8f, 18f);

    [Header("Chase Voices")]
    public AudioClip[] chaseVoiceClips;
    public Vector2 chaseVoiceInterval = new Vector2(4f, 9f);

    float wanderVoiceTimer;
    float chaseVoiceTimer;
    int lastWanderVoice = -1;
    int lastChaseVoice = -1;

    // =========================
    // LEASH
    // =========================

    [Header("Leash")]
    public float maxDistance = 60f;
    public float respawnDistance = 18f;
    public Renderer sokkRenderer;

    // =========================
    // INTERNAL
    // =========================

    bool chasing;
    Vector3 wanderDir;
    float wanderTimer;

    void Start()
    {
        wanderVoiceTimer = Random.Range(wanderVoiceInterval.x, wanderVoiceInterval.y);
        chaseVoiceTimer = Random.Range(chaseVoiceInterval.x, chaseVoiceInterval.y);

        PickNewWanderDirection();
        wanderTimer = wanderSegmentTime;
    }

    void Update()
    {
        if (player == null) return;

        float d = Vector3.Distance(transform.position, player.position);

        if (d > maxDistance && sokkRenderer != null && !sokkRenderer.isVisible)
            RepositionNearPlayer();

        chasing = d < chaseRadius && !SafeZoneDetector.inSafeZone;

        if (d > maxRoamDistance)
            chasing = true;

        if (chasing)
        {
            MoveChase();

            if (pantingAudio && !pantingAudio.isPlaying)
                pantingAudio.Play();

            HandleChaseVoices();
        }
        else
        {
            MoveWander();

            if (pantingAudio && pantingAudio.isPlaying)
                pantingAudio.Stop();

            HandleWanderVoices();
        }

        SnapToGround();
    }

    // =========================
    // TELEPORT
    // =========================

    void RepositionNearPlayer()
    {
        Vector2 r = Random.insideUnitCircle.normalized * respawnDistance;
        transform.position = player.position + new Vector3(r.x, 0, r.y);
    }

    // =========================
    // WANDER VOICES (QUIET)
    // =========================

    void HandleWanderVoices()
    {
        if (wanderVoiceClips.Length == 0 || wanderVoiceAudio == null)
            return;

        wanderVoiceTimer -= Time.deltaTime;

        if (wanderVoiceTimer <= 0f)
        {
            int i = Random.Range(0, wanderVoiceClips.Length);

            if (wanderVoiceClips.Length > 1 && i == lastWanderVoice)
                i = (i + 1) % wanderVoiceClips.Length;

            lastWanderVoice = i;

            wanderVoiceAudio.pitch = Random.Range(0.92f, 1.08f);
            wanderVoiceAudio.PlayOneShot(wanderVoiceClips[i]);

            wanderVoiceTimer = Random.Range(
                wanderVoiceInterval.x,
                wanderVoiceInterval.y
            );
        }
    }

    // =========================
    // CHASE VOICES (LOUD)
    // =========================

    void HandleChaseVoices()
    {
        if (chaseVoiceClips.Length == 0 || chaseVoiceAudio == null)
            return;

        chaseVoiceTimer -= Time.deltaTime;

        if (chaseVoiceTimer <= 0f)
        {
            int i = Random.Range(0, chaseVoiceClips.Length);

            if (chaseVoiceClips.Length > 1 && i == lastChaseVoice)
                i = (i + 1) % chaseVoiceClips.Length;

            lastChaseVoice = i;

            chaseVoiceAudio.pitch = Random.Range(0.92f, 1.08f);
            chaseVoiceAudio.PlayOneShot(chaseVoiceClips[i]);

            chaseVoiceTimer = Random.Range(
                chaseVoiceInterval.x,
                chaseVoiceInterval.y
            );
        }
    }

    // =========================
    // MOVEMENT
    // =========================

    void MoveChase()
    {
        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0f;

        Vector3 dir = ApplyHouseAvoidance(toPlayer.normalized);
        dir = ApplyWallAvoidance(dir);

        transform.position += dir * chaseSpeed * Time.deltaTime;
    }

    void MoveWander()
    {
        wanderTimer -= Time.deltaTime;

        if (wanderTimer <= 0f)
        {
            PickNewWanderDirection();
            wanderTimer = wanderSegmentTime;
        }

        Vector3 dir = ApplyHouseAvoidance(wanderDir);
        dir = ApplyWallAvoidance(dir);

        transform.position += dir * wanderSpeed * Time.deltaTime;
    }

    void PickNewWanderDirection()
    {
        Vector2 r = Random.insideUnitCircle.normalized;
        Vector3 toPlayer = (player.position - transform.position).normalized;
        wanderDir = (new Vector3(r.x, 0, r.y) + toPlayer * 0.15f).normalized;
    }

    // =========================
    // AVOIDANCE
    // =========================

    Vector3 ApplyHouseAvoidance(Vector3 baseDir)
    {
        Vector3 avoid = Vector3.zero;

        foreach (var h in houseCenters)
        {
            if (h == null) continue;

            Vector3 away = transform.position - h.position;
            away.y = 0f;

            float dist = away.magnitude;

            if (dist < avoidRadius)
            {
                float force = 1f - (dist / avoidRadius);
                avoid += away.normalized * force * avoidStrength;
            }
        }

        return (baseDir + avoid).normalized;
    }

    Vector3 ApplyWallAvoidance(Vector3 dir)
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.5f, dir);

        if (Physics.Raycast(ray, wallCheckDistance, wallLayer))
        {
            dir = Vector3.Reflect(dir, ray.direction);
            PickNewWanderDirection();
        }

        return dir.normalized;
    }

    // =========================
    // GROUND SNAP
    // =========================

    void SnapToGround()
    {
        Ray ray = new Ray(transform.position + Vector3.up * rayStartHeight, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit,
            rayStartHeight * 2f,
            groundLayer,
            QueryTriggerInteraction.Ignore))
        {
            Vector3 pos = transform.position;
            pos.y = hit.point.y + groundOffset;
            transform.position = pos;
        }
    }
}
