using UnityEngine;

public class SafeZoneDetector : MonoBehaviour
{
    public bool inSafeZone;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Safezone"))
        {
            inSafeZone = true;
            Debug.Log("ENTER SAFE");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Safezone"))
        {
            inSafeZone = false;
            Debug.Log("EXIT SAFE");
        }
    }
}
