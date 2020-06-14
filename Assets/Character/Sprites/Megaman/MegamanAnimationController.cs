using Platformer.Mechanics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegamanAnimationController : KinematicObject
{
    public enum MotionState
    {
        Idle,
        Running,
        Dashing,
        Jumping
    }

    // Start is called before the first frame update
    public Sprite[] RunningSprites;
    public Sprite[] RunningSpritesShooting;

    public Sprite[] IdleSprites;
    public Sprite[] IdleSpritesShooting;

    public Sprite[] DashingSprites;
    public Sprite[] DashingSpritesShooting;

    public Sprite[] JumpingSprites;
    public Sprite[] JumpingSpritesShooting;

    public Sprite[] SpawningSprites;

    public int DashPauseFrame;
    public int JumpPauseFrame;

    public int FrameCount;

    public float FrameSeconds;
    private int _mCurrentFrame;
    private float deltaTime;
    private SpriteRenderer _mRenderer;

    public bool IsSpawned;
    public bool Shooting;
    public MotionState AnimationState;
    private MotionState LastAnimationState;

    protected override void Start()
    {
        _mRenderer = GetComponent<SpriteRenderer>();
        Shooting = false;
        IsSpawned = false;

        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (!IsSpawned && !IsGrounded)
        {
            _mRenderer.sprite = SpawningSprites[0];
            return;
        }

        deltaTime += Time.deltaTime;
        while (deltaTime >= FrameSeconds)
        {
            deltaTime = Math.Max(0, deltaTime - FrameSeconds);
            _mCurrentFrame++;
            _mRenderer.sprite = GetSprite();
        }

        if (!IsSpawned)
        {
            if (_mCurrentFrame >= SpawningSprites.Length - 1)
            {
                IsSpawned = true;
                AnimationState = MotionState.Idle;
            }
        }
    }

    private Sprite GetSprite()
    {
        if (LastAnimationState != AnimationState)
            _mCurrentFrame = 0;

        Sprite[] arr = RunningSprites;

        if (!IsSpawned)
        {
            arr = SpawningSprites;
        }
        else
        {
            LastAnimationState = AnimationState;
            if (!Shooting)
            {
                switch (AnimationState)
                {
                    case MotionState.Dashing:
                        arr = DashingSprites;
                        if (_mCurrentFrame > DashPauseFrame)
                            _mCurrentFrame = DashPauseFrame;
                        break;
                    case MotionState.Idle:
                        arr = IdleSprites;
                        break;
                    case MotionState.Jumping:
                        arr = JumpingSprites;
                        break;
                    case MotionState.Running:
                        arr = RunningSprites;
                        break;
                }
            }

            if (Shooting)
            {
                switch (AnimationState)
                {
                    case MotionState.Dashing:
                        arr = DashingSpritesShooting;
                        if (_mCurrentFrame > DashPauseFrame)
                            _mCurrentFrame = DashPauseFrame;
                        break;
                    case MotionState.Idle:
                        arr = IdleSpritesShooting;
                        break;
                    case MotionState.Jumping:
                        arr = JumpingSpritesShooting;
                        break;
                    case MotionState.Running:
                        arr = RunningSpritesShooting;
                        break;
                }
            }
        }
        return arr[_mCurrentFrame % arr.Length];
    }
}
