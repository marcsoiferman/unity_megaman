﻿using Platformer.Mechanics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Speed = 20f;
    public Rigidbody2D rigidBody;
    protected float power = 1;
    public int FrameMoveStart;
    public Sprite[] AnimatingSprites;
    public int LoopFrame;
    public float FrameSeconds;
    private int _mCurrentFrame;
    private SpriteRenderer _mRenderer;
    private float deltaTime;
    public GameObject Explosion;
    private bool moving;

    // Start is called before the first frame update
    void Start()
    {
        moving = false;
        if (FrameMoveStart <= 0)
        {
            rigidBody.velocity = transform.right * Speed;
            moving = true;
        }
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
            float threshold = moving ? FrameSeconds : FrameSeconds / 2;
            deltaTime += Time.deltaTime;
            while (deltaTime >= threshold)
            {
                deltaTime = Math.Max(0, deltaTime - threshold);
                _mCurrentFrame++;
                if (_mCurrentFrame >= AnimatingSprites.Length)
                    _mCurrentFrame = Math.Max(0, LoopFrame);
                if (_mCurrentFrame >= FrameMoveStart && !moving)
                {
                    rigidBody.velocity = transform.right * Speed;
                    moving = true;
                }
                _mRenderer.sprite = AnimatingSprites[_mCurrentFrame];
            }
        }
    }
}
