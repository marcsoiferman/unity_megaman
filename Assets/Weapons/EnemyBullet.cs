using Platformer.Mechanics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Weapons
{
    class EnemyBullet : Bullet
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.Damage((int)power);
                if (Explosion != null)
                {
                    GameObject obj = Instantiate(Explosion, rigidBody.transform.position, rigidBody.transform.rotation);

                }
                Destroy(gameObject);
            }
        }
    }
}
