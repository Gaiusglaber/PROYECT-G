using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProyectG.Player.Attack;
using DragonBones;
using ProyectG.Common.Modules.Audio.Channels.Sound;

namespace ProyectG.Player.Controller
{
    [Serializable]
    public enum LastDirection
    {
        Right,
        Left
    }

    public struct RayRange
    {
        public readonly Vector2 Start;
        public readonly Vector2 End;
        public readonly Vector2 Dir;

        public RayRange(float x1, float y1, float x2, float y2, Vector2 direction)
        {
            Start = new Vector2(x1, y1);
            End = new Vector2(x2, y2);
            Dir = direction;
        }
    }

    public class PlayerController : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [Header("---FOR TEST---")]
        [SerializeField] private bool controllerEnable = false;
        [SerializeField] private float timeUntilActivatePlayer = 0;
        [SerializeField] private LayerMask groundLayer = default;
        [Header("---ATTACK---")]
        [SerializeField] private PlayerAttack playerAttack = null;
        [Header("---MOVEMENT---")]
        [SerializeField] private float acceleration = 0;
        [SerializeField] private float deAcceleration = 0;
        [SerializeField] private float movementClamp = 0;
        [Header("---GRVITY---")]
        [SerializeField] private float fallClamp = 0;
        [SerializeField] private float minFallSpeed = 0;
        [SerializeField] private float maxFallSpeed = 0;
        [Header("---JUMP---")]
        [SerializeField] private bool hollowKnightJump = false;
        [SerializeField] private float jumpHeight = 0;
        [SerializeField] private float jumpApexThreshold = 0;
        [SerializeField] private float coyoteTimeThreshold = 0;
        [SerializeField] private float jumpBuffer = 0;
        [SerializeField] private float jumpEarlyGravityTreshold = 4;
        [Header("---COLLISION---")]
        [SerializeField] private Bounds characterBounds = default;
        [SerializeField] private float timeLeftGrounded = 0;
        [SerializeField] private int detectorCount = 3;
        [SerializeField] private float rayLenght = 0.1f;
        [SerializeField, Range(0.1f, 0.3f)] private float rayBuffer = 0.1f;
        [Header("---ANIMATIONS---")]
        [SerializeField] private UnityArmatureComponent customAnimator = null;
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
        private int freeColliderInteractions = 10;

        private RayRange upRay = default;
        private RayRange downRay = default;
        private RayRange leftRay = default;
        private RayRange rightRay = default;

        private string lastAnimationExecuted = string.Empty;

        private float initialAceleration = 0.0f;
        private float initialDeAceleration = 0.0f;
        private float initialMovementClamp = 0.0f;

        private LastDirection lastDirection = default;
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

        public bool IsControllerEnable { get { return controllerEnable; } }

        public UnityArmatureComponent CustomAnimator { get { return customAnimator; } }
        #endregion

        #region UNITY_CALLS
        public void Init(SoundHandlerChannel soundHandlerChannel)
        {
            controllerEnable = true;

            playerAttack.Init(soundHandlerChannel);
        }

        public void Update()
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

            initialAceleration = acceleration;
            initialDeAceleration = deAcceleration;
            initialMovementClamp= movementClamp;
        }
        #endregion

        #region PRIVATE_METHODS
        private void CalculateRayRanged()
        {
            Bounds pjBounds = new Bounds(transform.position + characterBounds.center, characterBounds.size);

            downRay = new RayRange(pjBounds.min.x + rayBuffer, pjBounds.min.y, pjBounds.max.x - rayBuffer, pjBounds.min.y, Vector2.down);
            upRay = new RayRange(pjBounds.min.x + rayBuffer, pjBounds.max.y, pjBounds.max.x - rayBuffer, pjBounds.max.y, Vector2.up);
            leftRay = new RayRange(pjBounds.min.x, pjBounds.min.y + rayBuffer, pjBounds.min.x, pjBounds.max.y - rayBuffer, Vector2.left);
            rightRay = new RayRange(pjBounds.max.x, pjBounds.min.y + rayBuffer, pjBounds.max.x, pjBounds.max.y - rayBuffer, Vector2.right);
        }
        private void CheckCollisions()
        {
            CalculateRayRanged();

            bool groundCheck = RunDetection(downRay);
            if (onGround && !groundCheck)
            {
                timeLeftGrounded = Time.time;
            }
            else if (!onGround && groundCheck)
            {
                coyoteUsable = true;
                LandingFrame = true;
            }

            onGround = groundCheck;

            colTop = RunDetection(upRay);
            colRight = RunDetection(rightRay);
            colLeft = RunDetection(leftRay);

            bool RunDetection(RayRange range)
            {
                return EvaluateRayPositions(range).Any(point => Physics2D.Raycast(point, range.Dir, rayLenght, groundLayer));
            }
        }

        private IEnumerable<Vector2> EvaluateRayPositions(RayRange range)
        {
            for (int i = 0; i < detectorCount; i++)
            {
                float t = (float)i / (detectorCount - 1);
                yield return Vector2.Lerp(range.Start, range.End, t);
            }
        }

        private void CalculateHorizontalMove()
        {
            if (Input.GetAxisRaw("Horizontal") != 0)
            {
                horizontalSpeed += Input.GetAxisRaw("Horizontal") * acceleration * Time.deltaTime;

                horizontalSpeed = Mathf.Clamp(horizontalSpeed, -movementClamp, movementClamp);

                float apexBonus = Mathf.Sign(Input.GetAxisRaw("Horizontal")) * this.apexBonus * apexPoint;
                horizontalSpeed += apexBonus * Time.deltaTime;

                if (!playerAttack.PlayerHasDoAttack)
                {
                    if (onGround)
                    {
                        SetAnimation("movilidad");
                    }
                    else
                    {
                        SetAnimation("Salto", 1);
                    }
                }

                if(horizontalSpeed > 0)
                {
                    lastDirection = LastDirection.Right;
                }
                else
                {
                    lastDirection = LastDirection.Left;
                }
            }
            else
            {
                horizontalSpeed = Mathf.MoveTowards(horizontalSpeed, 0, (deAcceleration / 6f) * Time.deltaTime);
                
                if(!playerAttack.PlayerHasDoAttack)
                {
                    if(!onGround)
                    {
                        SetAnimation("Salto", 1);
                    }
                    else
                    {
                        SetAnimation("idle");
                    }
                }
            }

            if (horizontalSpeed > 0 && colRight || horizontalSpeed < 0 && colLeft)
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
            if(hollowKnightJump)
            {
                if (endedJumpEarly && verticalSpeed > 0)
                {
                    newFallSpeed = fallSpeed * jumpEarlyGravityTreshold;
                }
                else
                {
                    newFallSpeed = fallSpeed;
                }
            }
            else
            {
                newFallSpeed = fallSpeed;
            }

            verticalSpeed -= newFallSpeed * Time.deltaTime;

            if (verticalSpeed < fallClamp)
            {
                verticalSpeed = fallClamp;
            }
        }
        private void CalculateApexJump()
        {
            if (!onGround)
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
            if (Input.GetButtonDown("Jump"))
            {
                lastJumpPressed = Time.time;                
            }

            if (Input.GetButtonDown("Jump") && CanUseCoyote || BufferedJump)
            {
                //Jump instantaneo
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

            //Chequea si el jump corto antes para aplicar down acceleration
            if (!onGround && Input.GetButtonUp("Jump") && !endedJumpEarly && Velocity.y > 0)
            {
                endedJumpEarly = true;
            }

            if (colTop)
            {
                if (verticalSpeed > 0)
                    verticalSpeed = 0;
            }
        }
        private void MovePlayer()
        {
            SetDirectionArmature();

            Vector3 position = transform.position + characterBounds.center;
            RawMovement = new Vector3(horizontalSpeed, verticalSpeed);
            Vector3 movement = RawMovement * Time.deltaTime;
            Vector3 furthestPoint = position + movement;

            transform.position += movement;

            //Chequeo mas adelante si hay alguna cosa a que colisionar
            /*Collider2D hit = Physics2D.OverlapBox(furthestPoint, characterBounds.size, 0, groundLayer);
            if (hit == null)
            {
                transform.position += movement;
                return;
            }*/

            /*Vector2 posToMove = transform.position;

            //Ta feo pero esto hace que si chocas en la esquina de algun lado se suba auto al lugar del hit en esa direccion.
            for (int i = 1; i < freeColliderInteractions; i++)
            {
                float t = (float)i / freeColliderInteractions;
                Vector2 posToTry = Vector2.Lerp(position, furthestPoint, t);

                if (Physics2D.OverlapBox(posToTry, characterBounds.size, 0, groundLayer))
                {
                    transform.position = posToMove;

                    if (i == 1)
                    {
                        if (verticalSpeed < 0)
                        {
                            verticalSpeed = 0;
                        }
                        Vector3 direction = transform.position - hit.transform.position;
                        transform.position += direction.normalized * movement.magnitude;
                    }

                    return;
                }

                posToMove = posToTry;
            }*/
        }
        #endregion

        #region ANIMATION
        private void SetAnimation(string idAnimation, int playTimes = -1)
        {
            if(!customAnimator.animation.isPlaying)
            {
                customAnimator.animation.Play(idAnimation, playTimes);
                lastAnimationExecuted = idAnimation;
            }
            else
            {
                if(lastAnimationExecuted != idAnimation)
                {
                    customAnimator.animation.Stop();
                }
            }
        }
        private void SetDirectionArmature()
        {
            customAnimator._armature.flipX = lastDirection == LastDirection.Left ? true : false;
        }
        #endregion

        #region PUBLIC_METHODS
        public void ChangeSpeed(bool lowBatteryMode)
        {
            acceleration = lowBatteryMode ? 1 : initialAceleration;
            deAcceleration = lowBatteryMode ? 1 : initialDeAceleration;
            movementClamp = lowBatteryMode ? 1 : initialMovementClamp;
        }

        public void ToggleController(bool state)
        {
            controllerEnable = state;
        }
        #endregion

        #region CORUTINES

        #endregion

        #region ON_GIZMOS
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position + characterBounds.center, characterBounds.size);
        }
        #endregion
    }
}