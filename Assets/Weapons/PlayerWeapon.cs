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
    public class PlayerWeapon: Weapon<PlayerBullet>
    {
        public GameObject bulletPrefabCharged1;
        public GameObject bulletPrefabCharged2;
        public PlayerController player;
        private bool charging = false;
        private Stopwatch chargeStopwatch = new Stopwatch();
        public ChargingState ChargeState;
        public AudioClip chargingClip;
        bool audioInUse = false;

        protected override void Update()
        {
            bool shooting = false;


            deltaTime += Time.deltaTime;
            if (Input.GetButtonDown("Fire1"))
            {
                ChargeState = ChargingState.Tier0;
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
                player.audioSource.Stop();
                audioInUse = false;
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
                shooting = true;
            }

            player.animationController.SetShooting(shooting);
        }

        private void FixedUpdate()
        {
            if (charging)
            {
                long chargedMS = chargeStopwatch.ElapsedMilliseconds;

                if (chargedMS > 200 && !player.audioSource.isPlaying)
                {
                    player.audioSource.PlayOneShot(chargingClip);
                }

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
