using Assets.Scripts;
using Platformer.Mechanics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        if (enemies is null)
            FindAllEnemies();
        //Register all tokens so they can work with this controller.
        foreach(IEnemy enemy in enemies.Where(a=> !a.IsAlive))
        {
            enemy.Respawn();
        }
    }

    void Update()
    {
        foreach (IEnemy enemy in enemies.Where(a => !a.IsAlive))
        {
            Vector3 position = Camera.main.WorldToViewportPoint(enemy.transform.position);
            Vector3 startingPosition = Camera.main.WorldToViewportPoint(enemy.StartingPosition);

            if (position.x < 0 || position.x > 1 || startingPosition.y < 0 || startingPosition.y > 1)
                enemy.Respawn();
        }
    }
}
