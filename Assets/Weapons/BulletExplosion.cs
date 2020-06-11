using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletExplosion : MonoBehaviour
{
    public Sprite[] AnimatingSprites;
    private SpriteRenderer _mRenderer;
    public float FrameSeconds;
    private int _mCurrentFrame;
    private float deltaTime;

    // Start is called before the first frame update
    void Start()
    {
        _mRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (AnimatingSprites != null && AnimatingSprites.Length > 0)
        {
            deltaTime += Time.deltaTime;
            while (deltaTime >= FrameSeconds)
            {
                deltaTime = Math.Max(0, deltaTime - FrameSeconds);
                _mCurrentFrame++;
                if (_mCurrentFrame >= AnimatingSprites.Length)
                {
                    Destroy(gameObject);
                    return;
                }
                _mRenderer.sprite = AnimatingSprites[_mCurrentFrame];
            }
        }
    }
}
