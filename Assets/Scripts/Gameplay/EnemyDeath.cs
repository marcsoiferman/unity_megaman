using Assets.Scripts;
using Platformer.Core;
using Platformer.Mechanics;
using System.ComponentModel;
using UnityEngine;

namespace Platformer.Gameplay
{
    /// <summary>
    /// Fired when the health component on an enemy has a hitpoint value of  0.
    /// </summary>
    /// <typeparam name="EnemyDeath"></typeparam>
    public class EnemyDeath : Simulation.Event<EnemyDeath>
    {
        public EnemyController enemy;
        public GameObject DeathObject;

        public override void Execute()
        {
            enemy.SetCollision(false); 
            enemy.health.Die();
            if (enemy._audio && enemy.ouch)
                enemy._audio.PlayOneShot(enemy.ouch);
                        
                if (DeathObject != null)
                {
                    Rigidbody2D rigidBody = enemy.GetComponent<Rigidbody2D>();
                    DeathObject.GetComponent<Rigidbody2D>().gravityScale = 0;
                    GameObject obj = GameObject.Instantiate(DeathObject, rigidBody.transform.position, rigidBody.transform.rotation);
                }
        }
    }
}