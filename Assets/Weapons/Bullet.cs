using Platformer.Mechanics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Speed = 20f;
    public Rigidbody2D rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody.velocity = transform.right * Speed;
    }

    private void FixedUpdate()
    {
        Vector3 position = Camera.main.WorldToViewportPoint(transform.position);
        if (position.x < 0 || position.x > 1 || position.y < 0 || position.y > 1 || position.z < 0)
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
