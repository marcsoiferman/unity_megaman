using Platformer.Core;
using Platformer.Model;
using System;
using UnityEngine;

namespace Platformer.Mechanics
{
    /// <summary>
    /// This class exposes the the game model in the inspector, and ticks the
    /// simulation.
    /// </summary> 
    public class GameController : MonoBehaviour
    {
        public static GameController Instance { get; private set; }

        //This model field is public and can be therefore be modified in the 
        //inspector.
        //The reference actually comes from the InstanceRegister, and is shared
        //through the simulation and events. Unity will deserialize over this
        //shared reference when the scene loads, allowing the model to be
        //conveniently configured inside the inspector.
        public PlatformerModel model = Simulation.GetModel<PlatformerModel>();
        public EnemyManager enemyManager; 

        void OnEnable()
        {
            Instance = this;
            model.player.OnRespawn += RespawnWorld;
            enemyManager = GetComponent<EnemyManager>();
        }

        private void RespawnWorld()
        {
            TokenController tokenController = GetComponent<TokenController>();
            tokenController.Respawn();
     
            enemyManager.Respawn();
        }

        void OnDisable()
        {
            if (Instance == this) Instance = null;
        }

        void Update()
        {
            if (Instance == this) Simulation.Tick();
        }
    }
}