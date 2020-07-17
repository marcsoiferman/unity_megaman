using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Character.Sprites.Enemies.SniperJoe
{
    public class SniperJoeAnimationController : MonoBehaviour
    {
        public enum ActionState
        {
            Defensive,
            Offensive
        }

        public Sprite[] IdleSprites;
        public Sprite PreShootingSprite;
        public Sprite ShootingSprite;

        public int FrameCount;
        public float FrameSeconds;
        private int _mCurrentFrame;
        private float deltaTime;

        private SpriteRenderer _mRenderer;
        public ActionState AnimationState;

        bool _stateChange;
        private Sprite currentSprite;

        private void Start()
        {
            _mRenderer = GetComponent<SpriteRenderer>();
            _stateChange = false;
        }

        // Update is called once per frame
        private void Update()
        {
            if (_stateChange)
            {
                _mRenderer.sprite = PreShootingSprite;
                _stateChange = false;
                return;
            }

            deltaTime += Time.deltaTime;

            while (deltaTime >= FrameSeconds)
            {
                deltaTime = Math.Max(0, deltaTime - FrameSeconds);
                _mCurrentFrame++;
                _mRenderer.sprite = GetSprite();
            }
        }

        private Sprite GetSprite()
        {
            if (AnimationState == ActionState.Defensive)
            {
                if (currentSprite == IdleSprites[0])
                    return IdleSprites[1];
                else if (currentSprite == IdleSprites[1])
                    return IdleSprites[0];
            }
            else if (AnimationState == ActionState.Offensive)
            {
                return ShootingSprite;
            }

            return IdleSprites[0];
        }

        public void SetState(ActionState state)
        {
            if (AnimationState != state)
            {
                AnimationState = state;
                _stateChange = true;
            }
        }
    }
}
