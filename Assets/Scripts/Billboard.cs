using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField] bool lockY = true;
    public float rotationSpeed = 5f;

    private Camera cam;

    void Awake()
    {
        GameObject camObj = GameObject.Find("PlayerCamera");
        if (camObj != null)
            cam = camObj.GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (cam == null) return;

        Vector3 direction = transform.position - cam.transform.position;
        direction.y = 0f;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
}

