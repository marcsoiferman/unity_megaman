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

public class SniperJoeController : EnemyController, IEnemy
{
    public float ModeChangeTime => 5;
    public override float BounceAmount => 3;
    public override bool HurtByJump => false;
    public override int ContactDammage => 3;
    public EnemyStance mode { get; set; }
    private float modeDeltaTime; 

    SniperJoeWeapon weapon;
    protected override void Awake()
    {
        base.Awake();
        //_collider = GetComponents<Collider2D>()[1];
        weapon = GetComponent<SniperJoeWeapon>();
        weapon.FireCooldownSeconds = 1;
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
        this.spriteRenderer.color = new Color(207, 0,4);
        //shoot
    }

    private void DefensiveAction()
    {
        weapon.IsEnabled = false;
        this.spriteRenderer.color = new Color(207, 206, 0);
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

    public enum EnemyStance
{
    Attack,
    Defend
}
}
