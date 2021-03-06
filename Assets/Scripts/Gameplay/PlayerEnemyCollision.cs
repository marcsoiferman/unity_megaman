using Assets.Scripts;
using Assets.Scripts.Gameplay;
using Platformer.Core;
using Platformer.Mechanics;
using Platformer.Model;
using UnityEngine;
using static Platformer.Core.Simulation;

namespace Platformer.Gameplay
{

    /// <summary>
    /// Fired when a Player collides with an Enemy.
    /// </summary>
    /// <typeparam name="EnemyCollision"></typeparam>
    public class PlayerEnemyCollision : Simulation.Event<PlayerEnemyCollision>
    {
        public IEnemy enemy;
        public PlayerController player;

        PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public override void Execute()
        {
            var willHurtEnemy = (player.Bounds.center.y >= enemy.Bounds.max.y) && enemy.HurtByJump;
            float killBounce = 2;

            if (willHurtEnemy)
            {
                enemy.Damage(3);
                if (!enemy.IsAlive)
                {
                    player.Bounce(killBounce);
                }
                else
                {
                    player.Bounce(enemy.BounceAmount);
                }
            }
            else
            {
                player.PlayDamageAnimation();
                player.Damage(enemy.ContactDamage);
            }
        }
    }
}