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
        public bool IsAlive { get; set; }

        public Vector3 StartingPosition { get; set; }
        private Vector2 startingVelocity;
        private Bounds startingBounds;

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
            StartingPosition = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z); 
            startingVelocity = new Vector2(this.control.velocity.x,0);

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
            this._collider.enabled = true;
            this.transform.position = StartingPosition;
            this.control.velocity = startingVelocity;
            this.control.ResetGrounding();
            this.control.enabled = true;
            IsAlive = true;
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