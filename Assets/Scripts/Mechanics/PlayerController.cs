using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Gameplay;
using static Platformer.Core.Simulation;
using Platformer.Model;
using Platformer.Core;

namespace Platformer.Mechanics
{
    /// <summary>
    /// This is the main class used to implement control of the player.
    /// It is a superset of the AnimationController class, but is inlined to allow for any kind of customisation.
    /// </summary>
    public class PlayerController : KinematicObject
    {
        public AudioClip jumpAudio;
        public AudioClip respawnAudio;
        public AudioClip ouchAudio;

        /// <summary>
        /// Max horizontal speed of the player.
        /// </summary>
        public float maxSpeed = 15;
        /// <summary>
        /// Initial jump velocity at the start of a jump.
        /// </summary>
        public float jumpTakeOffSpeed = 7;
        public float dashTakeOffSpeed = 3;


        public JumpState jumpState = JumpState.Grounded;
        public DashState dashState = DashState.Grounded;

        private bool stopJump;
        private bool stopDash;
        /*internal new*/ public Collider2D collider2d;
        /*internal new*/ public AudioSource audioSource;
        public Health health;
        public bool controlEnabled = true;

        bool jump;
        bool dash;

        int jumpDashLimit = 1;
        int accJumpDash = 0;
       
        Vector2 move;
        SpriteRenderer spriteRenderer;
        internal Animator animator;
        readonly PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public Bounds Bounds => collider2d.bounds;

        void Awake()
        {
            health = GetComponent<Health>();
            audioSource = GetComponent<AudioSource>();
            collider2d = GetComponent<Collider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
        }

        protected override void Update()
        {
            if (controlEnabled)
            {
                move.x = Input.GetAxis("Horizontal");
                
                if (jumpState == JumpState.Grounded && Input.GetButtonDown("Jump"))
                    jumpState = JumpState.PrepareToJump;
                else if (Input.GetButtonUp("Jump"))
                {
                    stopJump = true;
                    Schedule<PlayerStopJump>().player = this;
                }

                if (dashState == DashState.Grounded && Input.GetButtonDown("Dash"))
                {
                    dashState = DashState.Dashing;
                    dash = true;
                }
                else if (Input.GetButtonUp("Dash"))
                {
                    stopDash = true;
                }
            }
            else
            {
                move.x = 0;
            }

            UpdateJumpState();
            base.Update();
        }

        void UpdateJumpState()
        {
            jump = false;
            switch (jumpState)
            {
                case JumpState.PrepareToJump:
                    jumpState = JumpState.Jumping;
                    jump = true;
                    stopJump = false;
                    break;
                case JumpState.Jumping:
                    if (!IsGrounded)
                    {
                        Schedule<PlayerJumped>().player = this;
                        jumpState = JumpState.InFlight;
                    }
                    break;
                case JumpState.InFlight:
                    if (IsGrounded)
                    {
                        Schedule<PlayerLanded>().player = this;
                        jumpState = JumpState.Landed;
                    }
                    break;
                case JumpState.Landed:
                    jumpState = JumpState.Grounded;
                    break;
            }
        }

        float dashSpeed = 0;
        protected override void ComputeVelocity()
        {
             if (jump && IsGrounded)
            {
                accJumpDash = jumpDashLimit;
                velocity.y = jumpTakeOffSpeed * model.jumpModifier;
                jump = false;
            }
            else if (stopJump)
            {
                stopJump = false;
                if (velocity.y > 0)
                {
                    velocity.y = velocity.y * model.jumpDeceleration;
                }
            }

            if (dash)
            {
                if(jumpState == JumpState.Grounded || accJumpDash > 0)
                    dashSpeed = dashTakeOffSpeed;

                accJumpDash--;

                dash = false;
            }
            else if (dashSpeed > 0)
            {
                dashSpeed--;
            }
            else if (dashSpeed ==0)
            {
                dashState = DashState.Grounded;
            }

            float movementX =0;
            if (move.x > 0.01f)
                spriteRenderer.flipX = false;

            else if (move.x < -0.01f)
                spriteRenderer.flipX = true;


            if(!spriteRenderer.flipX)
            {
                velocity.x += dashSpeed;
                movementX = System.Math.Max(move.x, dashSpeed);
            }
            else
            {
                dashSpeed *= -1;
                velocity.x += dashSpeed;
                movementX = System.Math.Min(move.x, dashSpeed);
                dashSpeed *= -1;
            }

            animator.SetBool("grounded", !dash && IsGrounded);
            animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);

            Vector2 finalMove = new Vector2(movementX,  move.y);
            targetVelocity = finalMove * maxSpeed;
        }

        public enum JumpState
        {
            Grounded,
            PrepareToJump,
            Jumping,
            InFlight,
            Landed
        }

        public enum DashState
        {
            Grounded,
            PrepareToDash,
            Dashing,
            DoneDashing
        }
    }
}