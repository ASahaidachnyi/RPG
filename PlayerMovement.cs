using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 4f;
    public float sprintSpeed = 7f;
    public float rotationSpeed = 10f;
    public float jumpForce = 6f;
    public float groundCheckDistance = 0.2f;

    [Header("References")]
    public Transform cameraTransform;
    public LayerMask groundMask;

    private Rigidbody rb;
    private PlayerInputActions inputActions;
    private Vector2 moveInput;
    private bool jumpInput;
    private bool sprintInput;
    private bool isGrounded;

    private Vector3 currentVelocity;
    private Animator animator;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        inputActions = new PlayerInputActions();

        // Input bindings
        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        inputActions.Player.Jump.performed += ctx => jumpInput = true;

        inputActions.Player.Sprint.performed += ctx => sprintInput = true;
        inputActions.Player.Sprint.canceled += ctx => sprintInput = false;
    }

    void OnEnable() => inputActions.Player.Enable();
    void OnDisable() => inputActions.Player.Disable();

    void FixedUpdate()
    {
        GroundCheck();

        Vector3 inputDir = new Vector3(moveInput.x, 0f, moveInput.y);

        float targetSpeed = sprintInput ? sprintSpeed : walkSpeed;

        if (inputDir.sqrMagnitude > 0.01f)
        {
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            Vector3 moveDir = camForward * inputDir.z + camRight * inputDir.x;
            moveDir.Normalize();

            Vector3 targetVelocity = moveDir * targetSpeed;
            rb.linearVelocity = Vector3.SmoothDamp(rb.linearVelocity, new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z), ref currentVelocity, 0.05f);

            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
        else
        {
            rb.linearVelocity = Vector3.SmoothDamp(rb.linearVelocity, new Vector3(0f, rb.linearVelocity.y, 0f), ref currentVelocity, 0.1f);
        }

        if (jumpInput && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpInput = false;
            isGrounded = false;

            if (animator != null) animator.SetTrigger("Jump");
        }

        if (animator != null)
        {
            float horizontalSpeed = new Vector2(rb.linearVelocity.x, rb.linearVelocity.z).magnitude;
            float speedPercent = horizontalSpeed / sprintSpeed;
            animator.SetFloat("Speed", speedPercent, 0.1f, Time.fixedDeltaTime);
            animator.SetBool("IsGrounded", isGrounded);
        }
    }

    void GroundCheck()
    {
        isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, groundCheckDistance + 0.1f, groundMask);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position + Vector3.up * 0.1f, transform.position + Vector3.up * 0.1f + Vector3.down * groundCheckDistance);
    }
#endif
}
