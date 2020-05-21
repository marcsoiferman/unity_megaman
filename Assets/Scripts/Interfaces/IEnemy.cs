using Platformer.Mechanics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public interface IEnemy
    {
        int EnemyIndex { get; set; }
        EnemyManager Manager {get; set; }
        void Respawn();

    }
}
