using UnityEngine;

public class SafeZoneDetector : MonoBehaviour
{
    public bool inSafeZone;

    [Header("Ambience")]
    public AudioSource outsideAmbience;
    public AudioSource houseAmbience;

    [Header("Safezone Check")]
    public LayerMask safezoneLayer;
    public float checkRadius = 0.6f;

    void Start()
    {
        // detect if we spawn inside safe zone
        inSafeZone = Physics.CheckSphere(
            transform.position,
            checkRadius,
            safezoneLayer,
            QueryTriggerInteraction.Collide
        );

        SetAmbience(inSafeZone);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Safezone"))
        {
            inSafeZone = true;
            SetAmbience(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Safezone"))
        {
            inSafeZone = false;
            SetAmbience(false);
        }
    }

    void SetAmbience(bool inside)
    {
        if (outsideAmbience) outsideAmbience.mute = inside;
        if (houseAmbience) houseAmbience.mute = !inside;
    }
}
