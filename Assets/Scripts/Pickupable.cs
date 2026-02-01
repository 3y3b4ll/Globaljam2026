using UnityEngine.Rendering;
using UnityEngine;

public class Pickupable : MonoBehaviour
{
    [HideInInspector] public Rigidbody rb;

    public VolumeProfile maskProfile;
    public GameObject overlayImage;

    [Header("Linked Collectible")]
    public GameObject linkedCollectible;

    [Header("Optional Light")]
    public Light pointLight; // assign in inspector (child light)


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void OnPickup()
    {
        if (rb) rb.isKinematic = true;

        // Hide the mask visually
        gameObject.SetActive(false);

        // Disable child point light if assigned
        if (pointLight != null)
            pointLight.enabled = false;

        if (linkedCollectible != null)
            linkedCollectible.SetActive(true);
    }



    public void OnDrop(Vector3 position)
    {
        transform.position = position;
        gameObject.SetActive(true);

        if (rb)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Enable child light
        if (pointLight != null)
            pointLight.enabled = true;

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
