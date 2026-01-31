using UnityEngine;

public class NaarisokkChase : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float stopDistance = 3f;

    [Header("Activation")]
    public float activationDistance = 25f;
    public bool requireLineOfSight = false;

    [Header("Grounding")]
    public float groundCheckHeight = 2f;
    public float groundOffset = 0.05f;
    public LayerMask groundLayer;

    private bool isActive = false;

    void Update()
    {
        if (player == null)
        {
            Debug.Log("Player is NULL");
            return;
        }

        Vector3 dir = player.position - transform.position;
        dir.y = 0f;

        transform.position += dir.normalized * moveSpeed * Time.deltaTime;
    }

    void MoveTowardsPlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        transform.position += direction.normalized * moveSpeed * Time.deltaTime;
    }

    bool HasLineOfSight()
    {
        if (!requireLineOfSight) return true;

        Vector3 origin = transform.position + Vector3.up * 1.5f;
        Vector3 target = player.position + Vector3.up * 1.5f;

        return !Physics.Raycast(origin, target - origin, Vector3.Distance(origin, target));
    }

    void SnapToGround()
    {
        Ray ray = new Ray(transform.position + Vector3.up * groundCheckHeight, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, groundCheckHeight * 2f, groundLayer))
        {
            Vector3 pos = transform.position;
            pos.y = hit.point.y + groundOffset;
            transform.position = pos;
        }

        Debug.DrawRay(
        transform.position + Vector3.up * groundCheckHeight,
        Vector3.down * groundCheckHeight * 2f,
        Color.red
        );
    }
}
