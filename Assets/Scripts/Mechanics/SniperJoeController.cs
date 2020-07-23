using Platformer.Gameplay;
using Platformer.Mechanics;
using System.Collections;
using System.Collections.Generic;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine;
using static Platformer.Core.Simulation;
using Assets.Scripts;
using Assets.Scripts.Gameplay;
using Platformer.Gameplay;
using System;
using System.Runtime.CompilerServices;
using Assets.Weapons;
using static Assets.Character.Sprites.Enemies.SniperJoe.SniperJoeAnimationController;
using Assets.Character.Sprites.Enemies.SniperJoe;

public class SniperJoeController : EnemyController
{
    public float ModeChangeTime => 5;
    public override float BounceAmount => 3;
    public override bool HurtByJump => false;
    public override int ContactDamage => 3;
    public SniperJoeAnimationController animationController;
    public Rigidbody2D body;

    public EnemyStance mode { get; set; }
    private float modeDeltaTime; 

    private SniperJoeWeapon weapon;
    protected override void Awake()
    {
        base.Awake();
        weapon = GetComponent<SniperJoeWeapon>();
        weapon.FireCooldownSeconds = 1;
        animationController = GetComponent<SniperJoeAnimationController>();
        body = GetComponent<Rigidbody2D>();
    }
    // Start is called before the first frame update
    void Start() 
    {
        
    }
    protected override void Update()
    {
        UpdateStanceMode();
        PerformAction();
        base.Update();
    }

    private void PerformAction()
    {
        if (mode == EnemyStance.Defend)
            DefensiveAction();
        else if (mode == EnemyStance.Attack)
            AttackAction();
    }

    private void AttackAction()
    {
        weapon.IsEnabled = true;
        animationController.SetState(ActionState.Offensive);
        //shoot
    }

    private void DefensiveAction()
    {
        weapon.IsEnabled = false;
        animationController.SetState(ActionState.Defensive);
        //do nothing
    }

    private void UpdateStanceMode()
    {
        modeDeltaTime += Time.deltaTime;
        if(modeDeltaTime > ModeChangeTime)
        {
            modeDeltaTime = 0;
            FlipMode();
        }
    }
    public override void Damage(int amount = 1)
    {
        if (this.mode == EnemyStance.Defend)
            return;

        base.Damage(amount);
    }

    private void FlipMode()
    {
        switch(mode)
        {
            case EnemyStance.Attack:
                mode = EnemyStance.Defend;
                break;
            case EnemyStance.Defend:
                mode = EnemyStance.Attack;
                break;
        }
    }

    public override void Respawn()
    {
        body.position = StartingPosition;
        body.velocity *= 0;
        base.Respawn();
    }

    public enum EnemyStance
{
    Attack,
    Defend
}
}
