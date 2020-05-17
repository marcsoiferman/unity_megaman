using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Gameplay;
using static Platformer.Core.Simulation;
using Platformer.Model;
using Platformer.Core;
using System;
using System.Diagnostics;

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
        public float maxSpeed = 7;
        /// <summary>
        /// Initial jump velocity at the start of a jump.
        /// </summary>
        public float jumpTakeOffSpeed = 7;

        /// <summary>
        /// Dash speed boost.
        /// </summary>
        public float dashBoost = 2f;

        /// <summary>
        /// Dash speed boost in the air (doesn't decay).
        /// </summary>
        public float dashAirBoost = 2f;

        /// <summary>
        /// Dash Duration (# of game ticks).
        /// </summary>
        public int dashDuration = 15;

        /// <summary>
        /// Dash decay (% remaining per game tick).
        /// </summary>
        public float dashDecay = 0.98f;

        /// <summary>
        /// % of decay that needs to happen for dash to end.
        /// </summary>
        public float dashFloor = 0.7f;

        private float currentDashVelocity = 0;
        public int currentDashDuration = 0;

        public JumpState jumpState = JumpState.Grounded;
        public DashState dashState = DashState.NotDashing;
        private bool stopDash;
        private bool stopJump;
        /*internal new*/ public Collider2D collider2d;
        /*internal new*/ public AudioSource audioSource;
        public Health health;
        public bool controlEnabled = true;

        public bool dash;
        bool jump;
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

                if (Input.GetButtonDown("Dash") && this.IsGrounded && dashState == DashState.NotDashing)
                {
                    dashState = DashState.PrepareToDash;
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
            UpdateDashState();
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
                        dashState = DashState.NotDashing;
                        currentDashDuration = 0;
                        currentDashVelocity = 0;
                    }
                    break;
                case JumpState.Landed:
                    jumpState = JumpState.Grounded;
                    break;
            }
        }

        void UpdateDashState()
        {
            dash = false;
            switch (dashState)
            {
                case DashState.PrepareToDash:
                    dashState = DashState.Dashing;
                    dash = true;
                    stopDash = false;
                    break;
                case DashState.Dashing:
                    //Schedule<PlayerJumped>().player = this;
                    dashState = DashState.InDash;
                    break;
                case DashState.InDash:
                    if (currentDashVelocity == 0)
                    {
                        //Schedule<PlayerLanded>().player = this;
                        dashState = DashState.DoneDashing;
                    }
                    break;
                case DashState.DoneDashing:
                    dashState = DashState.NotDashing;
                    break;
            }
        }

        protected override void ComputeVelocity()
        {
            if (jump && IsGrounded)
            {
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
                currentDashVelocity = dashBoost;
                currentDashDuration = dashDuration;
                dash = false;
            }
            else if (stopDash)
            {
                currentDashVelocity = 0;
            }

            float tempDashVelocity = currentDashVelocity;

            if (move.x > 0.01f)
                spriteRenderer.flipX = false;
            else if (move.x < -0.01f)
                spriteRenderer.flipX = true;

            if (currentDashDuration != 0)
            {
                if (IsGrounded)
                {
                    if (currentDashDuration < 0.5 * dashDuration)
                        currentDashVelocity *= dashDecay;
                    currentDashDuration--;
                    if (currentDashDuration <= 0)
                        currentDashVelocity = 0;
                }
                else
                {
                    tempDashVelocity = Mathf.Abs(move.x * dashAirBoost);
                }
                if (spriteRenderer.flipX)
                    tempDashVelocity = -tempDashVelocity;
            }

            velocity.x += currentDashVelocity;

            animator.SetBool("grounded", IsGrounded);
            animator.SetBool("dashing", dashState != DashState.NotDashing);
            animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);

            Vector2 finalMove = new Vector2(tempDashVelocity != 0 ? tempDashVelocity : move.x, move.y);
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
            NotDashing,
            PrepareToDash,
            Dashing,
            InDash,
            DoneDashing
        }
    }
}