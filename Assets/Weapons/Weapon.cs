using Assets.Weapons;
using Platformer.Mechanics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public abstract class Weapon<T> :MonoBehaviour where T :Bullet
{
    public Transform firePoint;
    public GameObject bulletPrefabNormal;

    public float FireCooldownSeconds;
    public int MaxBullets;
    protected float deltaTime;

    public bool IsEnabled { get; set; }

    private void Start()
    {
        IsEnabled = true;
        deltaTime = FireCooldownSeconds; //let you shoot immediately
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (IsEnabled)
        bool shooting = false;
        deltaTime += Time.deltaTime;
        if (Input.GetButtonDown("Fire1"))
        {
            deltaTime += Time.deltaTime;
            Shoot(1);
            shooting = true;
        }
        if (Input.GetButton("Fire1") && !charging)
        {
            charging = true;
            chargeStopwatch.Restart();
        }
        if (Input.GetButtonUp("Fire1") && charging)
        {
            charging = false;
            chargeStopwatch.Stop();
            switch(ChargeState)
            {
                case ChargingState.Tier0:
                    Shoot(1);
                    break;
                case ChargingState.Tier1:
                    Shoot(2);
                    break;
                case ChargingState.Tier2:
                    Shoot(3);
                    break;
            }
            shooting = true;
        }

        player.animationController.SetShooting(shooting);
    }

    protected void Shoot(float power)
    {
        if (deltaTime < FireCooldownSeconds)
            return;

        GameObject prefab = GetPrefabBullet(power);

        GameObject obj = Instantiate(prefab, firePoint.position, firePoint.rotation);
        T bullet = obj.GetComponent<T>();
        if (bullet != null)
        {
            bullet.SetPower(power);
        }
        deltaTime = power <= 1 ? 0 : FireCooldownSeconds;
    }

    protected virtual GameObject GetPrefabBullet(float power)
    {
        GameObject prefab = bulletPrefabNormal;
        return prefab;
    }
}
