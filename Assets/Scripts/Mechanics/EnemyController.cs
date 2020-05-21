using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
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
        public PatrolPath path;
        public AudioClip ouch;
        public bool IsAlive;

        private Vector3 startingPosition;
        private Vector2 startingVelocity;
        internal PatrolPath.Mover mover;
        internal AnimationController control;
        internal Collider2D _collider;
        internal AudioSource _audio;
        SpriteRenderer spriteRenderer;

        public Bounds Bounds => _collider.bounds;

        public int EnemyIndex { get; set; }
        public EnemyManager Manager { get; set; }

        void Awake()
        {
            control = GetComponent<AnimationController>();
            _collider = GetComponent<Collider2D>();
            _audio = GetComponent<AudioSource>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            startingPosition = this.transform.position;
            startingVelocity = this.control.velocity;
            IsAlive = true;
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            var player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                var ev = Schedule<PlayerEnemyCollision>();
                ev.player = player;
                ev.enemy = this;
            }
        }
        public void Respawn()
        {
            if (!IsAlive)
            {
                this._collider.enabled = true;
                this._collider.attachedRigidbody.bodyType = RigidbodyType2D.Kinematic;
                this.control.enabled = true;
                this.transform.position = startingPosition;
                this.control.velocity = startingVelocity;
                IsAlive = true;
            }
        }


        void Update()
        {
            if (path != null)
            {
                if (mover == null) mover = path.CreateMover(control.maxSpeed * 0.5f);
                control.move.x = Mathf.Clamp(mover.Position.x - transform.position.x, -1, 1);
            }
        }

    }
}