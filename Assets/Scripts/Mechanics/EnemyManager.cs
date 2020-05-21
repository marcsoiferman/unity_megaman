using Assets.Scripts;
using Platformer.Mechanics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Tooltip("Instances of enemies which are animated. If empty, token instances are found and loaded at runtime.")]
    public IEnemy[] enemies;

    float nextFrameTime = 0;

    [ContextMenu("Find All Enemies")]
    void FindAllEnemies()
    {
        enemies = UnityEngine.Object.FindObjectsOfType<EnemyController>();
    }


    void Awake()
    {

        FindAllEnemies();
        //Register all tokens so they can work with this controller.
        for (var i = 0; i < enemies.Length; i++)
        {
            enemies[i].EnemyIndex = i;
            enemies[i].Manager = this;
        }
    }

    public void Respawn()
    {
        if (enemies.Length == 0)
            FindAllEnemies();
        //Register all tokens so they can work with this controller.
        for (var i = 0; i < enemies.Length; i++)
        {
            enemies[i].Respawn();

        }
    }

    void Update()
    {
        
    }
}
