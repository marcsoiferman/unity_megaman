using Platformer.Mechanics;
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
    protected AudioSource _audio;
    public AudioClip DeflectSound;
    public AudioClip ShootSound;

    // Start is called before the first frame update
    protected void Start()
    {
        moving = false;
        if (FrameMoveStart <= 0)
        {
            rigidBody.velocity = transform.right * Speed;
            moving = true;
        }
        _mRenderer = GetComponent<SpriteRenderer>();
        _audio = GetComponent<AudioSource>();
        _mCurrentFrame = 0;
        deltaTime = 0;
        PlaySound(BulletSound.Shoot);
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

    public void Deflect(double angle, bool disableCollistion = true)
    {
        PlaySound(BulletSound.Deflect);
        _mRenderer.flipX = !_mRenderer.flipX;

        angle = 145;

        double currentX = rigidBody.velocity.x;
        double currentY = rigidBody.velocity.y;

        float initialRad;

        if (currentY == 0)
        {
            if (currentX > 0)
                initialRad = 0;
            else if (currentX < 0)
                initialRad = (float)Math.PI;
            else
                initialRad = 0;
        }
        else if( currentX ==0)
        {
            if (currentY > 0)
                initialRad = (1 / 2) * (float) Math.PI;
            else
                initialRad = (3 / 2) * (float)Math.PI;
        }
        else
        {
            initialRad = (float)Math.Atan(currentY / currentX);
        }


        double newRad = angle * Math.PI / 180;
        double finalRad = (initialRad) + (Math.Sign(currentX) * newRad);

        float x = (float)Math.Cos(finalRad);
        float y = (float)Math.Sin(finalRad);

        rigidBody.velocity = new Vector2(x, y) * Speed;

        if (disableCollistion)
            DisableCollision();
    }

    protected virtual void PlaySound(BulletSound sound)
    {
        switch(sound)
        {
            case BulletSound.Deflect:
                _audio.PlayOneShot(DeflectSound);
                break;
            case BulletSound.Shoot:
                _audio.PlayOneShot(ShootSound);
                break;
        }
    }

    private void DisableCollision()
    {
        CapsuleCollider2D collider = this.GetComponent<CapsuleCollider2D>();
        collider.enabled = false;
    }

    public enum BulletSound
    {
        Deflect, Shoot
    }
}
