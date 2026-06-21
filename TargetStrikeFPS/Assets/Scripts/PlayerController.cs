using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 9f;
    public float crouchSpeed = 2.5f;
    public float jumpHeight = 1.4f;
    public float gravity = -20f;
    public float smoothTime = 0.05f;

    [Header("Crouch")]
    public float standHeight = 1.8f;
    public float crouchHeight = 0.9f;
    public float crouchTransitionSpeed = 8f;

    [Header("Camera Bob")]
    public Transform cameraRoot;
    public float bobFrequency = 10f;
    public float bobAmplitude = 0.06f;
    private float _bobTimer;
    private Vector3 _camDefaultPos;

    [Header("Mouse Look")]
    public float mouseSensitivity = 180f;
    public float maxLookAngle = 85f;
    public Transform cameraTransform;

    private CharacterController _cc;
    private Vector3 _velocity;
    private float _xRotation;
    private Vector2 _currentInput;
    private Vector2 _smoothInputVelocity;
    private bool _isCrouching;
    private bool _controlsEnabled = true;

    void Awake() => _cc = GetComponent<CharacterController>();

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (cameraRoot) _camDefaultPos = cameraRoot.localPosition;
    }

    void Update()
    {
        if (!_controlsEnabled || !GameManager.Instance.IsGameRunning) return;
        HandleMouseLook();
        HandleMovement();
        HandleCrouch();
        HandleCameraBob();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;
        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -maxLookAngle, maxLookAngle);
        cameraTransform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleMovement()
    {
        bool isGrounded = _cc.isGrounded;
        if (isGrounded && _velocity.y < 0f) _velocity.y = -2f;

        Vector2 rawInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        _currentInput = Vector2.SmoothDamp(_currentInput, rawInput, ref _smoothInputVelocity, smoothTime);

        float speed = _isCrouching ? crouchSpeed : Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
        Vector3 move = transform.right * _currentInput.x + transform.forward * _currentInput.y;
        _cc.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded && !_isCrouching)
            _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        _velocity.y += gravity * Time.deltaTime;
        _cc.Move(_velocity * Time.deltaTime);
    }

    void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.C))
            _isCrouching = !_isCrouching;

        float targetHeight = _isCrouching ? crouchHeight : standHeight;
        _cc.height = Mathf.Lerp(_cc.height, targetHeight, crouchTransitionSpeed * Time.deltaTime);
    }

    void HandleCameraBob()
    {
        if (!cameraRoot) return;
        bool isMoving = _currentInput.magnitude > 0.1f && _cc.isGrounded;
        _bobTimer = isMoving ? _bobTimer + Time.deltaTime * bobFrequency : 0f;
        Vector3 targetBob = isMoving
            ? _camDefaultPos + new Vector3(0, Mathf.Sin(_bobTimer) * bobAmplitude, 0)
            : _camDefaultPos;
        cameraRoot.localPosition = Vector3.Lerp(cameraRoot.localPosition, targetBob, 8f * Time.deltaTime);
    }

    public void SetControlsEnabled(bool value)
    {
        _controlsEnabled = value;
        Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !value;
    }
}
