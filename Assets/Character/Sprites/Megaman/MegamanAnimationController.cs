using Platformer.Mechanics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegamanAnimationController : KinematicObject
{
    // Start is called before the first frame update
    public Sprite[] RunningSprites;
    public Sprite[] RunningSpritesShooting;
    public float FrameSeconds;
    private int _mCurrentFrame;
    private float deltaTime;
    private SpriteRenderer _mRenderer;
    private Rigidbody2D _mRigidBody;

    public bool IsSpawned;

    public bool Shooting;
    public bool _Shooting
    {
        get
        {
            return _shooting;
        }
        set
        {
            if (_shooting != value)
            {
                _shooting = value;
                animator.SetFloat("walkframe", animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1);
                animator.SetBool("shooting", _Shooting);
            }
        }
    }
    private bool _shooting;
    internal Animator animator;


    public int AnimationState;
    public int _AnimationState
    {
        get
        {
            return _animationState;
        }
        set
        {
            if (_animationState != value)
            {
                _animationState = value;
                animator.SetInteger("animationState", _animationState);
            }
        }
    }
    private int _animationState;


    private bool _Grounded
    {
        get
        {
            return _grounded;
        }
        set
        {
            if (_grounded != value)
            {
                _grounded = value;
                animator.SetBool("grounded", _grounded);
            }
        }
    }
    private bool _grounded;

    protected override void Start()
    {
        _mRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        _mRigidBody = GetComponent<Rigidbody2D>();
        Shooting = false;
        IsSpawned = false;

        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        _Shooting = Shooting;
        _AnimationState = AnimationState;
        _Grounded = IsGrounded;

        if (!IsSpawned)
        {
            if (animator.GetAnimatorTransitionInfo(0).IsName("MegamanSpawn -> MegamanIdle"))
            {
                IsSpawned = true;
                AnimationState = 0;
            }
        }
        //deltaTime += Time.deltaTime;
        //while (deltaTime >= FrameSeconds)
        //{
        //    deltaTime = Math.Max(0, deltaTime - FrameSeconds);
        //    _mCurrentFrame++;
        //    _mRenderer.sprite = GetSprite();
        //}
    }

    private Sprite GetSprite()
    {
        Sprite[] arr = RunningSprites;

        if (Shooting)
            arr = RunningSpritesShooting;

        return arr[_mCurrentFrame % arr.Length];
    }
}
