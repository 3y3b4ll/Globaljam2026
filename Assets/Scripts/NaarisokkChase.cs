using UnityEngine;

public class NaarisokkChase : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Camera playerCamera;
    public SafeZoneDetector SafeZoneDetector;

    // =========================
    // SPEED
    // =========================

    [Header("Speed")]
    public float wanderSpeed = 1.2f;
    public float chaseSpeed = 5.5f; // slower than player

    // =========================
    // BEHAVIOR
    // =========================

    [Header("Behavior")]
    public float chaseRadius = 25f;
    public float maxRoamDistance = 60f;

    // =========================
    // TELEPORT SCARE SYSTEM
    // =========================

    [Header("Teleport Scare")]
    public float maxDistance = 80f;
    public float respawnDistance = 22f;
    public float teleportCooldown = 20f;
    public float teleportChance = 0.45f;

    float teleportTimer;

    // =========================
    // SAFEZONE REPEL
    // =========================

    [Header("Safezone Repel")]
    public float safezoneRepelTime = 6f;
    float safezoneTimer;

    // =========================
    // GROUND SNAP
    // =========================

    [Header("Ground Snap")]
    public float rayStartHeight = 10f;
    public float groundOffset = 0.05f;
    public LayerMask groundLayer;

    // =========================
    // HOUSE AVOIDANCE
    // =========================

    [Header("House Avoidance")]
    public Transform[] houseCenters;
    public float avoidRadius = 7f;
    public float avoidStrength = 2.5f;

    // =========================
    // WALL AVOIDANCE
    // =========================

    [Header("Bounds")]
    public float wallCheckDistance = 1.6f;
    public LayerMask wallLayer;

    // =========================
    // AUDIO
    // =========================

    [Header("Audio Sources")]
    public AudioSource pantingAudio;
    public AudioSource chaseVoiceAudio;
    public AudioSource wanderVoiceAudio;

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
    // INTERNAL
    // =========================

    bool chasing;
    Vector3 wanderDir;
    float wanderTimer;
    public float wanderSegmentTime = 3f;

    // =========================
    // INIT
    // =========================

    void Start()
    {
        teleportTimer = teleportCooldown;

        wanderVoiceTimer = Random.Range(wanderVoiceInterval.x, wanderVoiceInterval.y);
        chaseVoiceTimer = Random.Range(chaseVoiceInterval.x, chaseVoiceInterval.y);

        PickNewWanderDirection();
        wanderTimer = wanderSegmentTime;
    }

    // =========================
    // UPDATE
    // =========================

    void Update()
    {
        if (player == null) return;

        float d = Vector3.Distance(transform.position, player.position);

        teleportTimer -= Time.deltaTime;

        // --- safezone repel ---
        if (SafeZoneDetector.inSafeZone)
            safezoneTimer = safezoneRepelTime;

        if (safezoneTimer > 0f)
        {
            safezoneTimer -= Time.deltaTime;
            chasing = false;
        }
        else
        {
            chasing = d < chaseRadius;
            if (d > maxRoamDistance)
                chasing = true;
        }

        // --- scare teleport (IN CAMERA VIEW) ---
        if (d > maxDistance &&
            teleportTimer <= 0f &&
            !SafeZoneDetector.inSafeZone &&
            Random.value < teleportChance)
        {
            TeleportIntoCameraEdgeView();
            teleportTimer = teleportCooldown;
        }

        // --- behavior ---
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
    // TELEPORT — EDGE OF SCREEN
    // =========================

    void TeleportIntoCameraEdgeView()
    {
        if (playerCamera == null) return;

        float edgeX = Random.value < 0.5f ? 0.08f : 0.92f;
        float y = Random.Range(0.3f, 0.7f);

        Vector3 vp = new Vector3(edgeX, y, respawnDistance);
        Vector3 world = playerCamera.ViewportToWorldPoint(vp);

        transform.position = world;
        SnapToGround();
    }

    // =========================
    // VOICES
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
                wanderVoiceInterval.y);
        }
    }

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
                chaseVoiceInterval.y);
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
        Vector3 away =
            (transform.position - player.position).normalized;

        wanderDir = (new Vector3(r.x, 0, r.y) + away * 0.6f).normalized;
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
