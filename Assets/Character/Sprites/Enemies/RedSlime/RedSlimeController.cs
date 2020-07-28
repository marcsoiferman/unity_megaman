using Platformer.Mechanics;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Character.Sprites.Enemies.RedBlob
{
    public class RedSlimeController : EnemyController
    {
        public AnimationController control { get; protected set; }

        protected override void Awake()
        {
            control = GetComponent<AnimationController>();
            DeathType = DeathType.Fall;
            base.Awake();
        }

        public override void Respawn()
        {
            this.control.Teleport(StartingPosition);
            this.control.enabled = true;
            base.Respawn();
        }

        protected override void Update()
        {
            if (path != null)
            {
                if (mover == null) mover = path.CreateMover(control.maxSpeed * 0.5f);
                control.move.x = Mathf.Clamp(mover.Position.x - transform.position.x, -1, 1);
            }
            base.Update();
        }

        public override void SetCollision(bool collision)
        {
            _collider.enabled = false;
            control.enabled = false;
        }
        public override void DisableEnemy()
        {
            this.enabled = false;
        }
    }
}
