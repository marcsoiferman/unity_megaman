using Platformer.Mechanics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefabNormal;

    public float FireCooldownSeconds;
    public int MaxBullets;
    protected float deltaTime;

    private void Start()
    {
        deltaTime = FireCooldownSeconds; //let you shoot immediately
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    protected void Shoot(float power)
    {
        if (deltaTime < FireCooldownSeconds)
            return;

        GameObject prefab = GetPrefabBullet(power);

        GameObject obj = Instantiate(prefab, firePoint.position, firePoint.rotation);
        Bullet bullet = obj.GetComponent<Bullet>();
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
