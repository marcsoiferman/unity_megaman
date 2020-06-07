using Platformer.Mechanics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Speed = 20f;
    public Rigidbody2D rigidBody;
    private float power = 1;
    public Sprite[] AnimatingSprites;
    public int LoopFrame;
    public float FrameSeconds;
    private int _mCurrentFrame;
    private SpriteRenderer _mRenderer;
    private float deltaTime;
    public GameObject Explosion;
    public GameObject MuzzleFlash;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody.velocity = transform.right * Speed;
        _mRenderer = GetComponent<SpriteRenderer>();
        _mCurrentFrame = 0;
        deltaTime = 0;
    }

    public void SetPower(float power)
    {
        this.power = power;
    }

    private void FixedUpdate()
    {
        Vector3 position = Camera.main.WorldToViewportPoint(transform.position);
        if (position.x < 0 || position.x > 1 || position.y < 0 || position.y > 1 || position.z < 0)
            Destroy(gameObject);
    }

    private void Update()
    {
        if (AnimatingSprites != null && AnimatingSprites.Length > 0)
        {
            deltaTime += Time.deltaTime;
            while (deltaTime >= FrameSeconds)
            {
                deltaTime = Math.Max(0, deltaTime - FrameSeconds);
                _mCurrentFrame++;
                if (_mCurrentFrame >= AnimatingSprites.Length)
                    _mCurrentFrame = Math.Max(0, LoopFrame);
                _mRenderer.sprite = AnimatingSprites[_mCurrentFrame];
            }
        }
    }

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
        //Level level = collision.GetComponent<Level>();
        //if (level != null)
        //{
        //    Destroy(gameObject);
        //}
    }
}
