using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FPSController : MonoBehaviour
{
    public bool CanMove { get; private set; } = true;
    private bool isSprinting => canSprint && Input.GetKey(sprintKey); //add lambda operator in property for sprint check
    private bool shouldJump => Input.GetKeyDown(jumpKey) && characterController.isGrounded;
    private bool shouldDoubleJump => Input.GetKeyDown(jumpKey) && !characterController.isGrounded && hasAlreadyJumped;
    private bool shouldCrouch => Input.GetKeyDown(crouchKey) && !duringCrouchAnimation && characterController.isGrounded;

    [Header("Functional Operators")]
    [SerializeField] private bool canSprint = true;
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool canDoubleJump = true;
    [SerializeField] private bool canCrouch = true;
    [SerializeField] private bool canUseHeadbob = true;
    [SerializeField] private bool WillSlideOnSlopes = true;
    [SerializeField] private bool canInteract = true;

    [Header("Controls")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;
    [SerializeField] private KeyCode interactKey = KeyCode.Mouse0;

    [Header("Movement Parameters")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintSpeed = 6.0f;
    [SerializeField] private float crouchSpeed = 1.5f;
    [SerializeField] private float slopeSpeed = 8f;

    [Header("Look Parameters")]
    [SerializeField, Range(1, 10)] private float lookSpeedX = 2.0f;
    [SerializeField, Range(1, 10)] private float lookSpeedY = 2.0f;
    [SerializeField, Range(1, 180)] private float upperLookLimit = 80.0f;
    [SerializeField, Range(1, 180)] private float lowerLookLimit = 80.0f;

    [Header("Jumping Parameters")]
    [SerializeField] private float jumpForce = 8.0f;
    [SerializeField] private float doubleJumpForce = 16.0f;
    [SerializeField] private float gravity = 30.0f;
    private bool hasAlreadyJumped = false;

    [Header("Crouch Parameters")]
    [SerializeField] private float crouchHeight = 0.5f;
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private float timeToCrouch = 0.25f;
    [SerializeField] private Vector3 crouchingCenter = new Vector3(0,0.5f,0);
    [SerializeField] private Vector3 standingCenter = new Vector3(0,0,0);
    private bool isCrouching;
    private bool duringCrouchAnimation;

    [Header("Headbob Parameters")]
    [SerializeField] private float walkBobSpeed = 14f;
    [SerializeField] private float walkBobAmount = 0.05f;
    [SerializeField] private float sprintBobSpeed = 18f;
    [SerializeField] private float sprintBobAmount = 0.11f;
    [SerializeField] private float crouchBobSpeed = 8f;
    [SerializeField] private float crouchBobAmount = 0.025f;
    private float defaultYPos = 0;
    private float timer;
    //SLIDING PARAMETERS
    private Vector3 hitPointNormal;

    private bool IsSliding
    {
        get
        {
            //Debug.DrawRay(transform.position, Vector3.down, Color.red);
            if(characterController.isGrounded && Physics.Raycast(transform.position, Vector3.down, out RaycastHit slopeHit, 2f))
            {
                hitPointNormal = slopeHit.normal; //get angle value of floor being stood on
                return Vector3.Angle(hitPointNormal, Vector3.up) > characterController.slopeLimit;
            }
            else
            {
                return false;
            }
        }
    }

    [Header("Interaction")]
    [SerializeField] private Vector3 interactionRayPoint = default;
    [SerializeField] private float interactionDistance = default;
    [SerializeField] private LayerMask interactionLayer = default;
    private Interactable currentInteractable;

    private Camera pcam;
    private CharacterController characterController;
    private Vector3 moveDirection;
    private Vector2 currentInput;
    private float rotationX = 0;
    
    void Awake()
    {
        pcam = GetComponentInChildren<Camera>();
        characterController = GetComponent<CharacterController>();
        defaultYPos = pcam.transform.localPosition.y; //returns back to default position when not moving
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        characterController.slopeLimit = 80;
    }

    void Update()
    {
        if(CanMove)
        {
            HandleMovementInput();
            HandleMouseLook();

            if(canJump)
            {
                 HandleJump();
            }

            if(canDoubleJump)
            {
                HandleDoubleJump();
            }

            if(canCrouch)
            {
                HandleCrouch();
            }

            if(canUseHeadbob)
            {
                HandleHeadbob();
            }

            if(canInteract)
            {
                HandleInteractionCheck();
                HandleInteractionInput();
            }

            AddFinalMovements();
        }
    }

    private void HandleMovementInput()//add basic input for player
    {
        currentInput = new Vector2((isCrouching ? crouchSpeed: isSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Vertical"), (isCrouching ? crouchSpeed: isSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Horizontal")); //add ternary operator to check for sprinting or walking

        float moveDirectionY = moveDirection.y;
        moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) + (transform.TransformDirection(Vector3.right) * currentInput.y); //move along directional angles
        moveDirection.y = moveDirectionY;
    }

    private void HandleMouseLook() //handle mouse movement for camera
    {
        rotationX -= Input.GetAxis("Mouse Y") * lookSpeedY;
        rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit); //look into 80 degrees up and down
        pcam.transform.localRotation = Quaternion.Euler(rotationX, 0, 0); 
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeedX, 0); //pass rotation to parent gameobject
    }

    private void HandleJump()
    {
        if(shouldJump)
        {
            moveDirection.y = jumpForce;
            hasAlreadyJumped = true;
        }
    }

    private void HandleDoubleJump()
    {
        if(shouldDoubleJump)
        {
            moveDirection.y = doubleJumpForce;
            hasAlreadyJumped = false;
        }
    }

    private void HandleCrouch()
    {
        if(shouldCrouch)
        {
            StartCoroutine(CrouchStand());
        }
    }

    private void HandleHeadbob()
    {
        if(!characterController.isGrounded)
            return;

        if(Mathf.Abs(moveDirection.x) > 0.1f || Mathf.Abs(moveDirection.z) > 0.1f) //check for headbob
        {
            timer += Time.deltaTime * (isCrouching ? crouchBobSpeed : isSprinting ? sprintBobSpeed : walkBobSpeed);
            pcam.transform.localPosition = new Vector3(
                pcam.transform.localPosition.x, defaultYPos + Mathf.Sin(timer) * (isCrouching ? crouchBobAmount : isSprinting ? sprintBobAmount : walkBobAmount), 
                pcam.transform.localPosition.z
            );
        }
    }
    private void HandleInteractionCheck()
    {
        if(Physics.Raycast(pcam.ViewportPointToRay(interactionRayPoint), out RaycastHit hit, interactionDistance)) //looking at a gameobject with given layer
        {
            if(hit.collider.gameObject.layer == 7 && (currentInteractable == null || hit.collider.gameObject.GetInstanceID() != currentInteractable.GetInstanceID())) //make sure object is not the same one being looked at
            {
                hit.collider.TryGetComponent(out currentInteractable);

                if(currentInteractable)
                {
                    currentInteractable.OnFocus();
                }
            }
        }
        else if (currentInteractable)
        {
            currentInteractable.OnLoseFocus();
            currentInteractable = null;
        }
    }

    private void HandleInteractionInput()
    {
        if(Input.GetKeyDown(interactKey) && currentInteractable != null && Physics.Raycast(pcam.ViewportPointToRay(interactionRayPoint), out RaycastHit hit, interactionDistance, interactionLayer)) //check distance from camera 
        {
            currentInteractable.OnInteract();
        }
    }

    private void AddFinalMovements() //add gravity and simple movement
    {
        if(!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        if(WillSlideOnSlopes && IsSliding) 
        {
            moveDirection += new Vector3(hitPointNormal.x, -hitPointNormal.y, hitPointNormal.z) * slopeSpeed; //add smooth downward slide momentum
        }

        characterController.Move(moveDirection * Time.deltaTime);
    }

    private IEnumerator CrouchStand()
    {
        if(isCrouching && Physics.Raycast(pcam.transform.position, Vector3.up, 1f)) //check to not allow standing if crouching under a ceiling
            yield break;

        duringCrouchAnimation = true;

        float timeElapsed = 0;
        float targetHeight = isCrouching ? standingHeight : crouchHeight;
        float currentHeight = characterController.height;
        Vector3 targetCenter = isCrouching ? standingCenter : crouchingCenter;
        Vector3 currentCenter = characterController.center;

        while(timeElapsed < timeToCrouch)
        {
            characterController.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / timeToCrouch); //go from stand to crouch and vice versa
            characterController.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / timeToCrouch);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        characterController.height = targetHeight;
        characterController.center = targetCenter;

        isCrouching = !isCrouching; //check if standing or crouching

        duringCrouchAnimation = false;
    }
}
