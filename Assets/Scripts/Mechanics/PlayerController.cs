using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Gameplay;
using static Platformer.Core.Simulation;
using Platformer.Model;
using Platformer.Core;
using UnityEngine.UI;
using TMPro;
using System;
using System.Diagnostics;
using System.Linq;

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

        public Color DefaultColor;
        public Color Stage1ChargingColor;
        public Color Stage2ChargingColor;
        public int ChargeFrames = 10;
        private int currentChargeFrame = 0;

        public Scoreboard_Script _scoreboard;

        #region score
        internal void UpdateScore(int v)
        {
            _scoreboard.UpdateScore(v);
        }

        internal void ResetScore()
        {
            _scoreboard.ResetScore();
        }

        #endregion

        /// <summary>
        /// Max horizontal speed of the player.
        /// </summary>
        public float maxSpeed = 7;

        internal void NotifyRespawn()
        {
            OnRespawn?.Invoke();
        }

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
        private float tempDashVelocity = 0;
        public int currentDashDuration = 0;

        public JumpState jumpState = JumpState.Grounded;
        public DashState dashState = DashState.NotDashing;
        private bool stopDash;
        private bool stopJump;
        /*internal new*/ public Collider2D collider2d;
        /*internal new*/ public AudioSource audioSource;
        /*internal new*/ public Collider2D wallCollider2d;
        public Health health;
        public MegamanAnimationController animationController;
        Weapon weapon;
        public bool controlEnabled = true;

        public bool dash;
        bool jump;
        Vector2 move;
        SpriteRenderer spriteRenderer;
        //internal Animator animator;
        readonly PlatformerModel model = Simulation.GetModel<PlatformerModel>();
        private bool againstWall => hitWallLeft || hitWallRight;

        private bool hitWallLeft = false;
        private bool hitWallRight = false;
        private bool pressingAgainstWall = false;

        public Bounds Bounds => collider2d.bounds;

        public event Action OnRespawn;
            
        void Awake()
        {
            health = GetComponent<Health>();
            audioSource = GetComponent<AudioSource>();
            collider2d = GetComponent<Collider2D>();
            wallCollider2d = GetComponents<Collider2D>()[1];
            spriteRenderer = GetComponent<SpriteRenderer>();
            //animator = GetComponent<Animator>();
            animationController = GetComponent<MegamanAnimationController>();
            weapon = GetComponent<Weapon>();
        }
        private bool lastState = false;
        protected override void Update()
        {
            pressingAgainstWall = false;
            if (controlEnabled && animationController.IsSpawned)
            {
                move.x = Input.GetAxis("Horizontal");
                if (move.x > 0 && hitWallRight)
                    pressingAgainstWall = true;
                if (move.x < 0 && hitWallLeft)
                    pressingAgainstWall = true;

                if ((jumpState == JumpState.Grounded || againstWall) && Input.GetButtonDown("Jump"))
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

            //if (lastState != pressingAgainstWall)
            //{
            //    UnityEngine.Debug.Log($"Switched! {pressingAgainstWall}");
            //    lastState = pressingAgainstWall;
            //}

            FixYVelocity = false;
            if (velocity.y < 0 && pressingAgainstWall)
            {
                FixYVelocity = true;
                FixedYVelocity = -1;
            }

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

        public void PlayDamageAnimation()
        {
            //animator.SetTrigger("hurt");
            audioSource.PlayOneShot(ouchAudio);
        }

        public void Damage(int amount = 1)
        {
            health.Decrement(amount);
            if (!health.IsAlive)
            {
                var ev = Schedule<HealthIsZero>();
                ev.health = health;
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
                    dashState = DashState.InDash;
                    break;
                case DashState.InDash:
                    if (currentDashVelocity == 0)
                    {
                        dashState = DashState.DoneDashing;
                    }
                    break;
                case DashState.DoneDashing:
                    dashState = DashState.NotDashing;
                    break;
            }
        }

        protected override void FixedUpdate()
        {
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

            tempDashVelocity = currentDashVelocity;

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
            }
            if (!facingRight)
                tempDashVelocity = -tempDashVelocity;

            UpdateDashState();
            UpdateChargingState();
            base.FixedUpdate();

            animationController.IsWallSliding = velocity.y < 0 && pressingAgainstWall;
            animationController.IsGrounded = IsGrounded;
            animationController.YVelocity = velocity.y;
            animationController.XVelocity = velocity.x;
            animationController.Jumping = jumpState != JumpState.Grounded;
            animationController.Dashing = dashState != DashState.NotDashing;
        }

        protected void UpdateChargingState()
        {
            currentChargeFrame = (currentChargeFrame + 1) % ChargeFrames;
            if (currentChargeFrame == 0)
            {
                switch (weapon.ChargeState)
                {
                    case Weapon.ChargingState.Tier0:
                        spriteRenderer.color = DefaultColor;
                        break;
                    case Weapon.ChargingState.Tier1:
                        if (spriteRenderer.color != Stage1ChargingColor)
                            spriteRenderer.color = Stage1ChargingColor;
                        else
                            spriteRenderer.color = DefaultColor;
                        break;
                    case Weapon.ChargingState.Tier2:
                        if (spriteRenderer.color != Stage2ChargingColor)
                            spriteRenderer.color = Stage2ChargingColor;
                        else
                            spriteRenderer.color = Stage1ChargingColor;
                        break;
                }
            }
        }

        protected override void ComputeVelocity()
        {
            if (jump)//&& IsGrounded)
            {
                wallJumping = againstWall && !IsGrounded;
                if (wallJumping)
                    wallBoostDuration = wallJumpDuration;
                FixYVelocity = false;
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

            velocity.x += currentDashVelocity;

            if (move.x != 0)
                UpdateFacingRight(move.x > 0f);

            Vector2 finalMove = new Vector2(tempDashVelocity != 0 ? tempDashVelocity : move.x, move.y);
            if (pressingAgainstWall)
                finalMove.x = 0;

            targetVelocity = finalMove * maxSpeed;
        }

        private void UpdateFacingRight(bool newValue)
        {
            if (facingRight == newValue)
                return;

            Flip();
        }

        private void Flip()
        {
            facingRight = !facingRight;
            transform.Rotate(0f, 180f, 0f);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Level level = collision.GetComponent<Level>();
            if (level != null)
            {
                wallJumping = false;
                List<ContactPoint2D> points = new List<ContactPoint2D>();
                int count = collision.GetContacts(points);
                if (count > 0)
                {
                    bool hitLeft = IsHitFromLeft();
                    bool hitRight = IsHitFromRight();

                    if (hitLeft || hitRight)
                    {
                        hitWallLeft = hitLeft;
                        hitWallRight = hitRight;

                        wallJumping = false;
                    }
                }
            }
        }

        private bool IsHitFromLeft()
        {
            Vector2 posCharTop = new Vector2(wallCollider2d.bounds.center.x, wallCollider2d.bounds.center.y - wallCollider2d.bounds.extents.y);
            Vector2 posCharMid = new Vector2(wallCollider2d.bounds.center.x, wallCollider2d.bounds.center.y);
            Vector2 posCharBottom = new Vector2(wallCollider2d.bounds.center.x, wallCollider2d.bounds.center.y + wallCollider2d.bounds.extents.y);
            Vector2[] positions = { posCharTop, posCharMid, posCharBottom };

            foreach(Vector2 pos in positions)
            {
                RaycastHit2D hit = Physics2D.Linecast(pos, pos - new Vector2(0.5f, 0), 1 << LayerMask.NameToLayer("Level"));
                if (hit.collider != null)
                    return true;
            }

            return false;
        }

        private bool IsHitFromRight()
        {
            Vector2 posCharTop = new Vector2(wallCollider2d.bounds.center.x, wallCollider2d.bounds.center.y - wallCollider2d.bounds.extents.y);
            Vector2 posCharMid = new Vector2(wallCollider2d.bounds.center.x, wallCollider2d.bounds.center.y);
            Vector2 posCharBottom = new Vector2(wallCollider2d.bounds.center.x, wallCollider2d.bounds.center.y + wallCollider2d.bounds.extents.y);
            Vector2[] positions = { posCharTop, posCharMid, posCharBottom };

            foreach (Vector2 pos in positions)
            {
                RaycastHit2D hit = Physics2D.Linecast(pos, pos + new Vector2(0.5f, 0), 1 << LayerMask.NameToLayer("Level"));
                if (hit.collider != null)
                    return true;
            }

            return false;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            Level level = collision.GetComponent<Level>();
            if (level != null)
            {
                hitWallLeft = false;
                hitWallRight = false;
                pressingAgainstWall = false;

                animationController.IsWallSliding = false;
            }
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