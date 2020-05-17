using Platformer.Mechanics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Speed = 20f;
    public Rigidbody2D rigidBody;
    private int Lifetime = 150;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody.velocity = transform.right * Speed;
    }

    private void FixedUpdate()
    {
        Lifetime--;
        if (Lifetime <= 0)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyController enemy = collision.GetComponent<EnemyController>();
        if (enemy != null)
        {
            enemy.Damage(1);
            Destroy(gameObject);
        }
        Level level = collision.GetComponent<Level>();
        if (level != null)
        {
            Destroy(gameObject);
        }
    }
}
