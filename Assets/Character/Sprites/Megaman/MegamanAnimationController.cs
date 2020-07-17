using Platformer.Mechanics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class MegamanAnimationController : MonoBehaviour
{
    public enum MotionState
    {
        Idle,
        Running,
        Dashing,
        Jumping,
        WallSliding,
        Spawning,
        Unspawning
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
    public Sprite[] UnspawningSprites;

    public Sprite[] WallSlidingSprites;
    public Sprite[] WallSlidingSpritesShooting;

    public int DashPauseFrame;
    public int JumpPauseFrame;
    public int UnspawnPauseFrame;

    public int FrameCount;

    public float FrameSeconds;
    private int _mCurrentFrame;
    private float deltaTime;
    private SpriteRenderer _mRenderer;

    public float StopShootingLagTime;
    private float _StopShootingTime;

    public bool IsSpawned
    {
        get => _IsSpawned;
        set
        {
            if (value != _IsSpawned)
            {
                _IsSpawned = value;
                _mCurrentFrame = 0;
            }
        }
    }
    private bool _IsSpawned;

    public bool IsUnspawning
    {
        get => _IsUnspawning;
        set
        {
            if (value != _IsUnspawning)
            {
                _IsUnspawning = value;
                _mCurrentFrame = 0;
            }
        }
    }
    private bool _IsUnspawning;

    private bool _StopShootingTriggered;
    private bool IsShooting;
    public bool IsGrounded;
    public bool IsWallSliding;
    public MotionState AnimationState;
    private MotionState LastAnimationState;

    public float YVelocity;
    public float XVelocity;
    public bool Jumping;
    public bool Dashing;

    private Queue<MotionState> queuedStates;

    private void Start()
    {
        queuedStates = new Queue<MotionState>();
        _mRenderer = GetComponent<SpriteRenderer>();
        LastAnimationState = MotionState.Idle;
        IsShooting = false;
        IsSpawned = false;
        IsWallSliding = false;
        IsGrounded = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!IsSpawned && !IsGrounded)
        {
            _mRenderer.sprite = SpawningSprites[0];
            return;
        }

        if (_StopShootingTriggered)
        {
            _StopShootingTime += Time.deltaTime;
            if (_StopShootingTime >= StopShootingLagTime)
            {
                IsShooting = false;
                _StopShootingTriggered = false;
            }
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
        else
        {
            MotionState state = DetermineState();
            if (LastAnimationState == MotionState.Jumping && state != MotionState.WallSliding)
                QueueAnimation(state);
            else
                AnimationState = state;
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
            if (!IsShooting)
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
                        if (YVelocity != 0)
                            _mCurrentFrame = JumpPauseFrame;
                        break;
                    case MotionState.Running:
                        arr = RunningSprites;
                        break;
                    case MotionState.WallSliding:
                        arr = WallSlidingSprites;
                        break;
                    case MotionState.Unspawning:
                        arr = UnspawningSprites;
                        if (_mCurrentFrame > UnspawnPauseFrame)
                            _mCurrentFrame = UnspawnPauseFrame;
                        break;
                }
            }

            if (IsShooting)
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
                        if (YVelocity != 0)
                            _mCurrentFrame = JumpPauseFrame;
                        break;
                    case MotionState.Running:
                        arr = RunningSpritesShooting;
                        break;
                    case MotionState.WallSliding:
                        arr = WallSlidingSpritesShooting;
                        break;
                }
            }
        }

        if (_mCurrentFrame != 0 && _mCurrentFrame % arr.Length == 0)
        {
            if (queuedStates.Count > 0)
            {
                AnimationState = queuedStates.Dequeue();
                return GetSprite();
            }
        }

        return arr[_mCurrentFrame % arr.Length];
    }

    public void QueueAnimation(MotionState state)
    {
        if (!queuedStates.Contains(state))
            queuedStates.Enqueue(state);
    }

    public MotionState DetermineState()
    {
        if (IsUnspawning)
            return MotionState.Unspawning;

        if (IsWallSliding && !IsGrounded)
            return MotionState.WallSliding;

        if (!IsGrounded)
            return MotionState.Jumping;

        if (Dashing)
            return MotionState.Dashing;

        if (XVelocity != 0)
            return MotionState.Running;

        return MotionState.Idle;
    }

    public void SetShooting(bool shooting)
    {
        if (shooting)
        {
            IsShooting = true;
            _StopShootingTriggered = false;
        }
        else
        {
            if (!_StopShootingTriggered)
                _StopShootingTime = 0;
            _StopShootingTriggered = true;
        }
    }

    public bool IsUnspawnAnimationComplete()
    {
        return (IsUnspawning && _mCurrentFrame >= UnspawnPauseFrame);
    }
}
