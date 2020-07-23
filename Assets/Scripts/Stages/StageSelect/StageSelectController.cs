using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectController : MonoBehaviour
{
    public Color DefaultColor;
    public Color SelectedColor;

    public Image[] Images;

    public int SelectedCell;
    private float deltaTime;

    // Start is called before the first frame update
    void Start()
    {
        SelectImage(0);
        deltaTime = 0;
    }

    void SelectImage(int image)
    {
        if (image >= 0 && image < Images.Length)
        {
            if (SelectedCell >= 0 && SelectedCell < Images.Length)
                Images[SelectedCell].color = DefaultColor;
            Images[image].color = SelectedColor;
            SelectedCell = image;
        }
    }

    // Update is called once per frame
    void Update()
    {
        deltaTime += Time.deltaTime;

        Vector2 move = new Vector2();
        move.x = Input.GetAxis("Horizontal");
        move.y = Input.GetAxis("Vertical");

        if (move.magnitude == 0)
            deltaTime = 0;

        if (Input.GetButtonDown("Jump"))
        {
            if (SelectedCell == 0)
                EditorSceneManager.LoadSceneAsync("SparkMan");
            else
                EditorSceneManager.LoadSceneAsync("SampleScene");
        }

        if (deltaTime < 0.2)
            return;

        if (move.x != 0)
            SelectImage(SelectedCell + Math.Sign(move.x));
        else if (move.y != 0)
            SelectImage(SelectedCell - 3 * Math.Sign(move.y));

        deltaTime = 0;
    }
}
