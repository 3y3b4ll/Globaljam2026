using UnityEngine.Rendering;
using UnityEngine;

public class Pickupable : MonoBehaviour
{
    [HideInInspector] public Rigidbody rb;

    public VolumeProfile maskProfile;
    public GameObject overlayImage;

    [Header("Linked Collectible")]
    public GameObject linkedCollectible;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void OnPickup()
    {
        if (rb) rb.isKinematic = true;

        // hide visuals instead of disabling object
        GetComponent<Collider>().enabled = false;
        GetComponent<Renderer>().enabled = false;

        if (linkedCollectible != null)
            linkedCollectible.SetActive(true);
    }


    public void OnDrop(Vector3 position)
    {
        transform.position = position;

        GetComponent<Collider>().enabled = true;
        GetComponent<Renderer>().enabled = true;

        if (rb)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (linkedCollectible != null)
            linkedCollectible.SetActive(false);
    }

    public void IgnorePlayerCollision(Collider playerCol, float time)
    {
        Collider myCol = GetComponent<Collider>();
        if (!myCol || !playerCol) return;

        Physics.IgnoreCollision(myCol, playerCol, true);
        StartCoroutine(RestoreCollision(myCol, playerCol, time));
    }

    System.Collections.IEnumerator RestoreCollision(Collider a, Collider b, float t)
    {
        yield return new WaitForSeconds(t);
        if (a && b)
            Physics.IgnoreCollision(a, b, false);
    }


    public void PermanentlyRemove()
    {
        Destroy(gameObject);
    }
}
