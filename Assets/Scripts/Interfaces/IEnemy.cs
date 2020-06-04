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
        int EnemyIndex { get; set; }
        bool IsAlive { get; set; }
        Vector3 StartingPosition { get; set; }
        EnemyManager Manager {get; set; }
        Transform transform { get; }
        void Respawn();
    }
}
