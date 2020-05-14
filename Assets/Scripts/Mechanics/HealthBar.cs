using Platformer.Mechanics;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public Health health;

    private TextMeshProUGUI healthBar;

    // Start is called before the first frame update
    void Start()
    {
        healthBar = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        string s = "";
        for (int i = 0; i < health.currentHP; i++)
            s += "l";

        healthBar.text = s;
    }
}
