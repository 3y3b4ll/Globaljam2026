using UnityEngine;

public class FPSController : MonoBehaviour
{
    [Header("Head Bob")]
    public float bobFrequency = 8f;
    public float bobAmplitude = 0.05f;
    public float bobSmoothing = 8f;

    [Header("Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 1.5f;
    public float gravity = -9.81f;
    public float mouseSensitivity = 2f;
    public float mouseSmoothTime = 0.05f;

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation = 0f;
    private float currentMouseX;
    private float currentMouseY;
    private float bobTimer;
    private Vector3 cameraDefaultPos;


    public Transform cameraTransform;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cameraDefaultPos = cameraTransform.localPosition;
    }

    void Update()
    {
        Move();
        Look();
        HeadBob();
    }

    void Move()
    {
        bool isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        float x = Input.GetAxis("Horizontal"); // A/D
        float z = Input.GetAxis("Vertical");   // W/S

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * moveSpeed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void Look()
    {
        float targetMouseX = Input.GetAxis("Mouse X") * mouseSensitivity * 100f * Time.deltaTime;
        float targetMouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * 100f * Time.deltaTime;

        currentMouseX = Mathf.Lerp(currentMouseX, targetMouseX, 1f / mouseSmoothTime * Time.deltaTime);
        currentMouseY = Mathf.Lerp(currentMouseY, targetMouseY, 1f / mouseSmoothTime * Time.deltaTime);

        xRotation -= currentMouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * currentMouseX);
    }
    void HeadBob()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        bool isMoving = (Mathf.Abs(x) > 0.1f || Mathf.Abs(z) > 0.1f) && controller.isGrounded;

        if (isMoving)
        {
            bobTimer += Time.deltaTime * bobFrequency;
            float bobOffset = Mathf.Sin(bobTimer) * bobAmplitude;

            Vector3 targetPos = cameraDefaultPos + Vector3.up * bobOffset;

            cameraTransform.localPosition = Vector3.Lerp(
                cameraTransform.localPosition,
                targetPos,
                Time.deltaTime * bobSmoothing
            );
        }
        else
        {
            bobTimer = 0f;

            cameraTransform.localPosition = Vector3.Lerp(
                cameraTransform.localPosition,
                cameraDefaultPos,
                Time.deltaTime * bobSmoothing
            );
        }
    }



}
