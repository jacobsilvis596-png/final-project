using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation;
using UnityEngine.XR.Interaction.Toolkit;

namespace MissionBit
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private XRDeviceSimulator desktopMove;
        [SerializeField] private ActionBasedContinuousMoveProvider xrMove;

        #region
        /*
        [SerializeField]
        private float speed = 5f;
        [SerializeField]
        private float gravity = -15f;

        private Vector3 velocity;

        private bool isGrounded;
        */

        /*
         public CharacterController _controller;
         public float horizontalInput;
         public float verticalInput;

         public float MAX_SPEED = 10f;
         public float incrementRate = 0.15f;
         private float playerSpeed;
         public float turnspeed = 100f;
        */

        #endregion

        public float BASE_SPEED_XR = 3f;
        public float BASE_SPEED_DESKTOP = 15f;

        private bool wasInObstacle;
        private bool isMoving;
        private bool isSprinting;

        public bool wasInteracting = false;

        private Vector2 inputMove;
        private Vector2 lastMove;
        private Transform cameraTransform;

        public static PlayerMovement Instance;

        private void Awake()
        {
            Instance = this;
            inputMove = Vector2.zero;
            lastMove = Vector2.zero;
            ResolveCameraTransform();
        }

        private void Start()
        {
            if (xrMove != null && cameraTransform != null)
            {
                xrMove.forwardSource = cameraTransform;
            }
        }

        private void ResolveCameraTransform()
        {
            if (cameraTransform != null)
                return;

            if (Camera.main != null)
            {
                cameraTransform = Camera.main.transform;
                return;
            }

            cameraTransform = GetComponentInChildren<Camera>()?.transform ?? transform;
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            inputMove = context.ReadValue<Vector2>();
            isMoving = inputMove.sqrMagnitude > 0.01f;
        }

        private Vector2 GetMovementInput()
        {
            if (inputMove.sqrMagnitude > 0.01f)
                return inputMove;

            if (Keyboard.current == null)
                return Vector2.zero;

            var keyboardInput = Vector2.zero;

            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
                keyboardInput.y += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
                keyboardInput.y -= 1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                keyboardInput.x -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                keyboardInput.x += 1f;

            return keyboardInput;
        }

        public void OnInteract(InputAction.CallbackContext context)
        {


            if (context.action.triggered)
            {

                wasInteracting = true;



            }
            else
            {
                wasInteracting = false;
            }

        }

        public void OnSprintAction(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                isSprinting = true;
            }
            else if (context.canceled)
            {
                isSprinting = false;
            }
        }

        // Update is called once per frame
        void Update()
        {
            #region
            /*
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 move = transform.right * x + transform.forward * z;

            controller.Move(move * speed * Time.deltaTime);

            velocity.y += gravity * Time.deltaTime;

            controller.Move(velocity * Time.deltaTime);
            */


            /*
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");

            transform.Rotate(Vector3.up * Time.deltaTime * turnspeed * horizontalInput);
            transform.Translate(Vector3.forward * Time.deltaTime * speed * verticalInput);
            */

            //MovePlayer();
            #endregion
            ResolveCameraTransform();
            if (xrMove != null && cameraTransform != null)
            {
                xrMove.forwardSource = cameraTransform;
            }

            CheckSprinting();

            var movementInput = GetMovementInput();
            var hasDesktopInput = movementInput.sqrMagnitude > 0.01f;
            var hasXRInput = inputMove.sqrMagnitude > 0.01f;

            if (hasDesktopInput)
            {
                if (desktopMove != null)
                {
                    desktopMove.keyboardBodyTranslateMultiplier = 0f;
                }

                if (cameraTransform != null)
                {
                    var movementDirection = (cameraTransform.forward * movementInput.y + cameraTransform.right * movementInput.x).normalized;
                    movementDirection.y = 0f;
                    var speed = isSprinting ? BASE_SPEED_DESKTOP * 2f : BASE_SPEED_DESKTOP;
                    transform.position += movementDirection * speed * Time.deltaTime;
                }

                isMoving = true;
            }
            else if (!hasXRInput)
            {
                isMoving = false;
                if (desktopMove != null)
                {
                    desktopMove.keyboardBodyTranslateMultiplier = 0f;
                }

                if (xrMove != null)
                {
                    xrMove.moveSpeed = 0f;
                }
            }
            else
            {
                isMoving = true;
            }

            if (hasXRInput && xrMove != null)
            {
                xrMove.moveSpeed = isSprinting ? BASE_SPEED_XR * 2f : BASE_SPEED_XR;
            }
        }

        public void CheckSprinting()
        {
            if (desktopMove != null)
            {
                desktopMove.keyboardBodyTranslateMultiplier = 0f;
            }

            if (xrMove != null)
            {
                xrMove.moveSpeed = isSprinting ? BASE_SPEED_XR * 2f : BASE_SPEED_XR;
            }
        }


        private void OnTriggerEnter(Collider other)
        {


        }
    }

}
