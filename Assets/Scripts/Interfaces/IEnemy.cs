using Platformer.Mechanics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public interface IEnemy
    {
        float BounceAmount { get; }
        bool HurtByJump {get;}
        int ContactDammage { get; }
        int EnemyIndex { get; set; }
        bool IsAlive { get; }
        Vector3 StartingPosition { get; set; }
        EnemyManager Manager { get; set; }
        Transform transform { get; }
        void Respawn();
        Bounds Bounds { get; }
        void Damage(int amount);
        Collider2D _collider { get; }
        AnimationController control { get; }
        Health health { get; }
        AudioSource _audio { get; }
        AudioClip ouch { get;}
    }
}
