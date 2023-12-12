using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float runSpeed = 7f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float groundRaycastDistance = 0.3f;
    [SerializeField] private float gravity = 10f;
    [SerializeField] private float grabRange = 2f;
    [SerializeField] private float kickForce = 2f;
    [SerializeField] private float ballDistance = 2f;
    [SerializeField] private float aimGuideSpeed = 0.5f;
    [SerializeField] private float aimGuideDistance = 4f;
    [SerializeField] private float aimGuideXMax = 4f;
    [SerializeField] private float aimGuideYMax = 4f;

    private PlayerAction playerInput;
    private Vector2 moveInput;
    private bool running;
    private Camera mainCamera;
    private Rigidbody rb;
    private LayerMask groundLayer;
    private GameObject grabbedBall;
    private GameObject aimingGuide;
    private LineRenderer AimLine;
    private Animator animator;
    public bool isHoldingBall;
    public bool isMoving = false;
    private Vector3 lastGrounded;

    private void Awake()
    {
        playerInput = new PlayerAction();
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        groundLayer = LayerMask.GetMask("Ground");
        isHoldingBall = false;

        rb.freezeRotation = true;

        animator = GetComponent<Animator>();
        aimingGuide = transform.Find("AimGuide").gameObject;
        AimLine = GameObject.Find("Aim Line Renderer").GetComponent<LineRenderer>();

        Vector3 aimingGuidePosition = transform.position + transform.forward * aimGuideDistance + Vector3.up;
        aimingGuide.transform.position = aimingGuidePosition;

        lastGrounded = transform.position;
    }

    private void OnEnable()
    {
        playerInput.Enable();
        playerInput.Player.walk.performed += OnMovementPerformed;
        playerInput.Player.walk.canceled += OnMovementCanceled;
        playerInput.Player.run.performed += OnRunPerformed;
        playerInput.Player.run.canceled += OnRunCanceled;
        playerInput.Player.grab.started += OnGrabPerformed;
        playerInput.Player.grab.canceled += OnGrabCanceled;
    }

    private void OnDisable()
    {
        playerInput.Disable();
        playerInput.Player.walk.performed -= OnMovementPerformed;
        playerInput.Player.walk.canceled -= OnMovementCanceled;
        playerInput.Player.run.performed -= OnRunPerformed;
        playerInput.Player.run.canceled -= OnRunCanceled;
        playerInput.Player.grab.started -= OnGrabPerformed;
        playerInput.Player.grab.canceled -= OnGrabCanceled;
    }

    void Update()
    {
        Movement();
        UpdateDottedLine();
        if (isHoldingBall)
        {
            AimGuidePosition();
        }

        animator.SetBool("Walking", moveInput != Vector2.zero);
        animator.SetBool("Running", running && moveInput != Vector2.zero);
        animator.SetBool("PickUp", isHoldingBall);
    }

    private void OnMovementPerformed(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnMovementCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
    }

    private void OnRunPerformed(InputAction.CallbackContext context)
    {
        running = true;
    }

    private void OnRunCanceled(InputAction.CallbackContext context)
    {
        running = false;
    }

    private void OnGrabPerformed(InputAction.CallbackContext context)
    {
        if (grabbedBall == null)
        {

            LayerMask throwableLayerMask = LayerMask.GetMask("Throwable");

            Collider[] colliders = Physics.OverlapSphere(transform.position, grabRange, throwableLayerMask);
            foreach (var collider in colliders)
            {
                grabbedBall = collider.gameObject;
                isHoldingBall = true;
                Rigidbody ballRb = grabbedBall.GetComponent<Rigidbody>();
                ballRb.useGravity = false;
                ballRb.isKinematic = true;

                Vector3 toBall = grabbedBall.transform.position - transform.position;
                toBall.y = 0;
                if (toBall != Vector3.zero)
                {
                    Quaternion newRotation = Quaternion.LookRotation(toBall);
                    transform.rotation = newRotation;
                }

                Vector3 grabPosition = transform.position + transform.forward * ballDistance + Vector3.up;
                grabbedBall.transform.position = grabPosition;

                grabbedBall.transform.parent = transform;
                break;
            }
        }

    }

    private void OnGrabCanceled(InputAction.CallbackContext context)
    {
        if (isHoldingBall)
        {
            GameManager.instance.kickCount++;
            grabbedBall.transform.parent = null;
            isHoldingBall = false;
            Rigidbody ballRb = grabbedBall.GetComponent<Rigidbody>();
            ballRb.useGravity = true;
            ballRb.isKinematic = false;

            Vector3 forceDirection = (aimingGuide.transform.position - grabbedBall.transform.position).normalized;
            ballRb.AddForce(forceDirection * kickForce, ForceMode.Impulse);

            Vector3 aimingGuidePosition = transform.position + transform.forward * aimGuideDistance + Vector3.up;
            aimingGuide.transform.position = aimingGuidePosition;

            grabbedBall = null;

            
        }
    }

    private void Movement()
    {
        if (isHoldingBall)
        {
            rb.velocity = Vector3.zero;
        }
        
        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;
        moveDirection = mainCamera.transform.TransformDirection(moveDirection);
        moveDirection.y = 0;
        float currentSpeed = running ? runSpeed : walkSpeed;
        RaycastHit hit;
        bool isGrounded = Physics.Raycast(rb.position, Vector3.down, out hit, groundRaycastDistance, groundLayer);

        if (isGrounded)
        {
            lastGrounded = transform.position;
            if (!isHoldingBall)
            {
                isMoving = true;
                Vector3 moveOnGround = Vector3.ProjectOnPlane(moveDirection, hit.normal);
                Vector3 desiredPosition = hit.point + Vector3.up * 0.1f;

                rb.MovePosition(desiredPosition);
                rb.velocity = moveOnGround * currentSpeed;
                rb.angularVelocity = Vector3.zero;

                Vector3 lockYRotation = transform.rotation.eulerAngles;
                lockYRotation.x = 0f;
                transform.rotation = Quaternion.Euler(lockYRotation);

                if (moveDirection != Vector3.zero)
                {
                    Quaternion newRotation = Quaternion.LookRotation(moveOnGround);
                    transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
                }

            }
        }
        else
        {
            transform.position = lastGrounded;
            rb.AddForce(Vector3.down * gravity);
        }
    }

    private void AimGuidePosition()
    {
        Vector2 moveInput = playerInput.Player.walk.ReadValue<Vector2>();
        Vector3 moveDirection = new Vector3(moveInput.x, moveInput.y, 0).normalized;

        moveDirection = transform.TransformDirection(moveDirection);

        Vector3 newPosition = aimingGuide.transform.position + moveDirection * aimGuideSpeed * Time.deltaTime;


        float xMin = transform.position.x - aimGuideXMax;
        float xMax = transform.position.x + aimGuideXMax;
        float yMin = transform.position.y - aimGuideYMax;
        float yMax = transform.position.y + aimGuideYMax;

        Vector3 localPosition = transform.InverseTransformPoint(newPosition);

        localPosition.x = Mathf.Clamp(localPosition.x, -aimGuideXMax, aimGuideXMax);
        localPosition.y = Mathf.Clamp(localPosition.y, -aimGuideYMax, aimGuideYMax);

        newPosition = transform.TransformPoint(localPosition);

        aimingGuide.transform.position = newPosition;
    }

    private void UpdateDottedLine()
    {
        if (grabbedBall != null && isHoldingBall)
        {
            int numPoints = 100;
            float timeStep = 0.1f;
            Vector3[] points = new Vector3[numPoints];
            Rigidbody ballRigidbody = grabbedBall.GetComponent<Rigidbody>();
            float ballMass = ballRigidbody.mass;

            Vector3 initialPosition = grabbedBall.transform.position;
            Vector3 initialVelocityVector = (aimingGuide.transform.position - initialPosition).normalized * kickForce;

            Vector3 acceleration = Vector3.down * Physics.gravity.magnitude * ballMass;

            for (int i = 0; i < numPoints; i++)
            {
                float time = i * timeStep;
                Vector3 position = initialPosition + (initialVelocityVector * time) + (acceleration * time * time / 2f);
                points[i] = position;
            }

            AimLine.positionCount = numPoints;
            AimLine.SetPositions(points);
            AimLine.enabled = true;
        }
        else
        {
            AimLine.enabled = false;
        }
    }
}
