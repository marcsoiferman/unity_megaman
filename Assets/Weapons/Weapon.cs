using Platformer.Mechanics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;
    public PlayerController player;
    private bool charging = false;
    private Stopwatch chargeStopwatch = new Stopwatch();
    public ChargingState ChargeState;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            ChargeState = ChargingState.Tier0;
            Shoot(1);
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
        }
    }

    private void FixedUpdate()
    {
        if (charging)
        {
            long chargedMS = chargeStopwatch.ElapsedMilliseconds;
            if (chargedMS > 2500)
                ChargeState = ChargingState.Tier2;
            else if (chargedMS > 1000)
                ChargeState = ChargingState.Tier1;
        }
        else
        {
            ChargeState = ChargingState.Tier0;
        }
    }

    private void Shoot(float power)
    {
        GameObject obj = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet bullet = obj.GetComponent<Bullet>();
        if (bullet != null)
        {
            obj.transform.localScale *= power;
            bullet.SetPower(power);
        }
    }

    public enum ChargingState
    {
        Tier0,
        Tier1,
        Tier2
    }
}
