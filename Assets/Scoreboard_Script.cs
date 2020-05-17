using Platformer.Mechanics;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Scoreboard_Script : MonoBehaviour
{
    TextMeshProUGUI scoreboard;
    double scoreValue;

    // Start is called before the first frame update
    void Start()
    {
        scoreboard = GetComponentInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        scoreboard.text = "Score = " + scoreValue;
    }

    internal void UpdateScore(int v)
    {
        scoreValue += v;
    }

    internal void ResetScore()
    {
        scoreValue = 0;
    }
}
