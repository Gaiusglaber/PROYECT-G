using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectG.Player.Controller
{
    public class PlayerController : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [Header("---FOR TEST---")]
        [SerializeField] private bool controllerEnable = false;
        [SerializeField] private LayerMask groundLayer = default;
        [Header("---MOVEMENT---")]
        [SerializeField] private float acceleration = 0;
        [SerializeField] private float deAcceleration = 0;
        [SerializeField] private float movementClamp = 0;
        [Header("---GRVITY---")]
        [SerializeField] private float fallClamp = 0;
        [SerializeField] private float minFallSpeed = 0;
        [SerializeField] private float maxFallSpeed = 0;
        [Header("---JUMP---")]
        [SerializeField] private float jumpHeight = 0;
        [SerializeField] private float jumpApexThreshold = 0;
        [SerializeField] private float coyoteTimeThreshold= 0;
        [SerializeField] private float jumpBuffer= 0;
        [SerializeField] private float jumpEarlyGravityTreshold = 4;
        [Header("---COLLISION---")]
        [SerializeField] private float timeLeftGrounded = 0;
        #endregion

        #region PRIVATE_FIELDS
        private bool onGround = false;
        private bool colLeft = false;
        private bool colRight = false;
        private bool colTop = false;

        private bool endedJumpEarly = false;
        private bool coyoteUsable = false;
        private float apexPoint;
        private float lastJumpPressed;

        private float fallSpeed = 30;

        private Vector3 lastPosition = default;
        private float horizontalSpeed = 0;
        private float verticalSpeed = 0;
        private float apexBonus = 2;
        #endregion

        #region PROPERTIES
        public Vector3 Velocity { get; private set; }
        public bool JumpingFrame { get; private set; }
        public bool LandingFrame { get; private set; }
        public Vector3 RawMovement { get; private set; }
        public bool Grounded => onGround;

        private bool CanUseCoyote
        {
            get
            {
                return (coyoteUsable && !onGround && timeLeftGrounded + coyoteTimeThreshold > Time.time);
            }
        }
        private bool BufferedJump
        {
            get
            {
                return (onGround && lastJumpPressed + jumpBuffer > Time.time);
            }
        }
        #endregion

        #region UNITY_CALLS
        private void Update()
        {
            if (!controllerEnable)
                return;

            Velocity = (transform.position - lastPosition) / Time.deltaTime;
            lastPosition = transform.position;

            CheckCollisions();

            CalculateApexJump();
            CalculteGravity();
            CalculateJump();

            CalculateHorizontalMove();

            MovePlayer();
        }
        #endregion

        #region PRIVATE_METHODS
        private void GroundCollision()
        {
            Ray2D rayDown = new Ray2D(transform.position, -transform.transform.up);
            Debug.DrawRay(rayDown.origin, rayDown.direction * .65f, Color.red);
            bool groundCheck = Physics2D.Raycast(rayDown.origin, rayDown.direction, .65f, groundLayer.value);

            if(onGround && !groundCheck)
            {
                timeLeftGrounded = Time.time;
            }
            else if(!onGround && groundCheck)
            {
                coyoteUsable = true;
                LandingFrame = true;
            }


            onGround = groundCheck;
        }
        private void CheckCollisions()
        {
            GroundCollision();

            Ray2D rayLeft = new Ray2D(transform.position, -transform.transform.right);
            Ray2D rayRight = new Ray2D(transform.position, transform.transform.right);
            Ray2D rayTop = new Ray2D(transform.position, transform.transform.up);

            Debug.DrawRay(rayLeft.origin, rayLeft.direction * .65f, Color.blue);
            Debug.DrawRay(rayRight.origin, rayRight.direction * .65f, Color.yellow);
            Debug.DrawRay(rayTop.origin, rayTop.direction * .65f, Color.green);

            colLeft = Physics2D.Raycast(rayLeft.origin, rayLeft.direction, .65f, groundLayer.value);
            colRight = Physics2D.Raycast(rayRight.origin, rayRight.direction, .65f, groundLayer.value);
            colTop = Physics2D.Raycast(rayTop.origin, rayTop.direction, .65f, groundLayer.value);
        }
        private void CalculateHorizontalMove()
        {
            if(Input.GetAxisRaw("Horizontal") != 0)
            {
                horizontalSpeed += Input.GetAxisRaw("Horizontal") * acceleration * Time.deltaTime;

                horizontalSpeed = Mathf.Clamp(horizontalSpeed,-movementClamp, movementClamp);

                float apexBonus = Mathf.Sign(Input.GetAxisRaw("Horizontal")) * this.apexBonus * apexPoint;
                horizontalSpeed += apexBonus * Time.deltaTime;
            }
            else
            {
                horizontalSpeed = Mathf.MoveTowards(horizontalSpeed, 0, deAcceleration * Time.deltaTime);
            }

            if(horizontalSpeed > 0 && colRight || horizontalSpeed < 0 && colLeft)
            {
                horizontalSpeed = 0;
            }
        }
        private void CalculteGravity()
        {
            if (onGround)
            {
                if (verticalSpeed < 0) verticalSpeed = 0;
                return;
            }

            float newFallSpeed = 0;

            if(endedJumpEarly && verticalSpeed > 0)
            {
                newFallSpeed = fallSpeed * jumpEarlyGravityTreshold;
            }
            else
            {
                newFallSpeed = fallSpeed;
            }

            verticalSpeed -= newFallSpeed * Time.deltaTime;

            if(verticalSpeed < fallClamp)
            {
                verticalSpeed = fallClamp;
            }
        }
        private void CalculateApexJump()
        {
            if(!onGround)
            {
                apexPoint = Mathf.InverseLerp(jumpApexThreshold, 0, Mathf.Abs(Velocity.y));
                fallSpeed = Mathf.Lerp(minFallSpeed, maxFallSpeed, apexPoint);
            }
            else
            {
                apexPoint = 0;
            }
        }
        private void CalculateJump()
        {
            if(Input.GetButtonDown("Jump"))
            {
                lastJumpPressed = Time.time;
            }

            if(Input.GetButtonDown("Jump") && CanUseCoyote || BufferedJump)
            {
                verticalSpeed = jumpHeight;
                endedJumpEarly = false;
                coyoteUsable = false;
                timeLeftGrounded = float.MinValue;
                JumpingFrame = true;
            }
            else
            {
                JumpingFrame = false;
            }

            if(!onGround && Input.GetButtonUp("Jump") && !endedJumpEarly && Velocity.y > 0)
            {
                endedJumpEarly = true;
            }

            if(colTop)
            {
                if (verticalSpeed > 0)
                    verticalSpeed = 0;
            }
        }
        private void MovePlayer()
        {
            RawMovement = new Vector3(horizontalSpeed, verticalSpeed);
            Vector3 movement = RawMovement * Time.deltaTime;
            transform.position += movement;
        }
        #endregion

        #region PUBLIC_METHODS

        #endregion
    }
}