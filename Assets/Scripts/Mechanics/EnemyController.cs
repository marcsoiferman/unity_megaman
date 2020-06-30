using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Assets.Scripts;
using Assets.Scripts.Gameplay;
using Platformer.Gameplay;
using UnityEngine;
using static Platformer.Core.Simulation;

namespace Platformer.Mechanics
{
    /// <summary>
    /// A simple controller for enemies. Provides movement control over a patrol path.
    /// </summary>
    [RequireComponent(typeof(AnimationController), typeof(Collider2D))]
    public class EnemyController : MonoBehaviour, IEnemy
    {
        protected bool facingRight = false;

        public Component ComponentFacing;
        public float ContactDammageCoolDownSeconds => 2;
        public virtual float BounceAmount => 7;
        public virtual bool HurtByJump => true;
        public virtual int ContactDammage => 2;
        protected float contactDeltaTime;
        protected PlayerController _inContactPlayer;

        public PatrolPath path;
        public AudioClip ouch { get; protected set; }
        public bool IsAlive => health.IsAlive;

        public Vector3 StartingPosition { get; set; }
        protected Vector2 startingVelocity;
        protected Bounds startingBounds;

        internal PatrolPath.Mover mover;
        public AnimationController control { get; protected set; }
        public Collider2D _collider { get; protected set; }
        public  AudioSource _audio { get; protected set; }
        public Health health { get; protected set; }
        protected SpriteRenderer spriteRenderer;

        public Bounds Bounds => _collider.bounds;

        public int EnemyIndex { get; set; }
        public EnemyManager Manager { get; set; }

        protected virtual void Awake()
        {
            control = GetComponent<AnimationController>();
            _collider = GetComponent<Collider2D>();
            _audio = GetComponent<AudioSource>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            StartingPosition = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z); 
            startingVelocity = new Vector2(this.control.velocity.x,0);
            health = GetComponent<Health>();
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            //Checks if enemy hit player    
            var player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                SetPlayerCollision(player);
            }
        }

        void OnTriggerExit2d(Collision2D collision)
        {
            RemovePlayerCollision();
        }

        private void SchedulePlayerCollision()
        {
            if (contactDeltaTime <= ContactDammageCoolDownSeconds)
                return;

            var ev = Schedule<PlayerEnemyCollision>();
            ev.player = _inContactPlayer;
            ev.enemy = this;

            contactDeltaTime = 0;
        }

        public void Respawn()
        {
            this._collider.enabled = true;
            this.control.Teleport(StartingPosition);
            this.control.enabled = true;
            health.Respawn(); 
        }
        protected virtual void Update()
        {
            contactDeltaTime += Time.deltaTime;

            if (path != null)
            {
                if (mover == null) mover = path.CreateMover(control.maxSpeed * 0.5f);
                control.move.x = Mathf.Clamp(mover.Position.x - transform.position.x, -1, 1);
            }

            if (_inContactPlayer != null)
                SchedulePlayerCollision();

            CheckPlayerDirection();
        }

        private void CheckPlayerDirection()
        {
            if (ComponentFacing is null) return;

            bool playerToTheRight = this.transform.position.x < ComponentFacing.transform.position.x;
            bool flipToRight = playerToTheRight && !facingRight;
            bool flipToLeft = !playerToTheRight && facingRight;

            if (flipToLeft || flipToRight)
                Flip();
        }

        public virtual void Damage(int amount = 1)
        {
            health.Decrement(amount);
            if (!health.IsAlive)
            {
                PlayerController player = UnityEngine.Object.FindObjectOfType<PlayerController>();
                player.UpdateScore(ScoreHelper.SLIME_ENEMY_POINTS);
                Schedule<EnemyDeath>().enemy = this;
                RemovePlayerCollision();
            }
        }

        internal void SetPlayerCollision(PlayerController p)
        {
            if (p is null)
            {
                RemovePlayerCollision();
                return;
            }

            _inContactPlayer = p;
            p.HasLeftEnemyContact += RemovePlayerCollision;
        }

        private void RemovePlayerCollision()
        {
            if (_inContactPlayer is null)
                return;

            _inContactPlayer.HasLeftEnemyContact -= RemovePlayerCollision;
            _inContactPlayer = null;
        }

        private void Flip()
        {
            facingRight = !facingRight;
            transform.Rotate(0f, 180f, 0f);
        }
    }
}