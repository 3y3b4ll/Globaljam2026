using UnityEngine;

public class NaarisokkChase : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public SafeZoneDetector SafeZoneDetector;
    AudioSource audioSrc;

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

    [Header("Audio")]
    public AudioSource chaseAudio;
    public AudioSource voiceAudio;
    public AudioClip[] wanderVoiceClips;
    public Vector2 voiceInterval = new Vector2(8f, 18f);

    [Header("Leash")]
    public float maxDistance = 60f;
    public float respawnDistance = 18f;
    public Renderer sokkRenderer;

    float voiceTimer;

    // --- internal ---
    bool chasing;
    Vector3 wanderDir;
    float wanderTimer;

    void Start()
    {
        audioSrc = GetComponent<AudioSource>();
        voiceTimer = Random.Range(voiceInterval.x, voiceInterval.y);
        PickNewWanderDirection();
        wanderTimer = wanderSegmentTime;
    }

    void Update()
    {
        float d = Vector3.Distance(transform.position, player.position);

        if (d > maxDistance && !sokkRenderer.isVisible)
        {
            RepositionNearPlayer();
        }

        if (player == null) return;

        // --- chase decision ---
        chasing = d < chaseRadius && !SafeZoneDetector.inSafeZone;

        // leash — don’t disappear forever
        if (d > maxRoamDistance)
            chasing = true;

        if (chasing)
        {
            MoveChase();
            
            if (!chaseAudio.isPlaying)
                chaseAudio.Play();
        }
        else
        {
            MoveWander();
            
            if (chaseAudio.isPlaying)
                chaseAudio.Stop();

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

        Vector3 newPos = player.position + new Vector3(r.x, 0, r.y);

        transform.position = newPos;
    }

    // =========================
    // WANDER VOICES
    // =========================

    void HandleWanderVoices()
    {
        if (wanderVoiceClips.Length == 0 || voiceAudio == null)
            return;

        voiceTimer -= Time.deltaTime;

        if (voiceTimer <= 0f)
        {
            var clip = wanderVoiceClips[Random.Range(0, wanderVoiceClips.Length)];
            voiceAudio.PlayOneShot(clip);

            voiceTimer = Random.Range(voiceInterval.x, voiceInterval.y);
        }
    }

    // =========================
    // CHASE
    // =========================

    void MoveChase()
    {
        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0f;

        Vector3 dir = ApplyHouseAvoidance(toPlayer.normalized);
        dir = ApplyWallAvoidance(dir);

        transform.position += dir * chaseSpeed * Time.deltaTime;
    }

    // =========================
    // WANDER
    // =========================

    void MoveWander()
    {
        wanderTimer -= Time.deltaTime;

        if (wanderTimer <= 0f)
        {
            PickNewWanderDirection();
            wanderTimer = wanderSegmentTime;
        }

        Vector3 dir = wanderDir;
        dir = ApplyHouseAvoidance(dir);
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
    // HOUSE AVOIDANCE
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

    // =========================
    // WALL / MAP BOUNDS AVOID
    // =========================

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

        if (Physics.Raycast(ray, out RaycastHit hit, rayStartHeight * 2f, groundLayer, QueryTriggerInteraction.Ignore))
        {
            Vector3 pos = transform.position;
            pos.y = hit.point.y + groundOffset;
            transform.position = pos;
        }
    }
}
