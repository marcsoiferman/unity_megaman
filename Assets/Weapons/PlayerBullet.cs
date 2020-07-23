using Platformer.Mechanics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Weapons
{
    public class PlayerBullet : Bullet
    {
        private void OnTriggerEnter2D(Collider2D collision)
        { 
            EnemyController enemy = collision.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.Damage((int)power);
                if (Explosion != null)
                {
                    GameObject obj = Instantiate(Explosion, rigidBody.transform.position, rigidBody.transform.rotation);

                }
                Destroy(gameObject);
            }
        }
    }
}
