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

public class SniperJoeController : EnemyController, IEnemy
{
    public override float BounceAmount => 3;
    public override bool HurtByJump => false;
    public override int ContactDammage => 3;

    Weapon weapon;
    protected override void Awake()
    {
        base.Awake();
        //_collider = GetComponents<Collider2D>()[1];
        weapon = GetComponent<Weapon>();
    }
    // Start is called before the first frame update
    void Start() { }
}
