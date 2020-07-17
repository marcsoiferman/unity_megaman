using Platformer.Mechanics;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Character.Sprites.Enemies.RedBlob
{
    public class RedSlimeController : EnemyController
    {
        public AnimationController control { get; protected set; }

        protected override void Awake()
        {
            control.GetComponent<AnimationController>();
            base.Awake();
        }

        public override void Respawn()
        {
            this.control.Teleport(StartingPosition);
            this.control.enabled = true;
            base.Respawn();
        }
    }
}
