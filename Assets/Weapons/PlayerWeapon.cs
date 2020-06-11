using Platformer.Mechanics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Weapons
{
    public class PlayerWeapon: Weapon
    {
        public GameObject bulletPrefabCharged1;
        public GameObject bulletPrefabCharged2;
        public PlayerController player { get; }
        private bool charging = false;
        private Stopwatch chargeStopwatch = new Stopwatch();
        public ChargingState ChargeState;

        protected override void Update()
        {
            deltaTime += Time.deltaTime;
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
                switch (ChargeState)
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

        protected override GameObject GetPrefabBullet(float power)
        {
            GameObject prefab = bulletPrefabNormal;
            if (power == 2)
                prefab = bulletPrefabCharged1;
            else if (power == 3)
                prefab = bulletPrefabCharged2;

            return prefab;
        }
        public enum ChargingState
        {
            Tier0,
            Tier1,
            Tier2
        }
    }
}
