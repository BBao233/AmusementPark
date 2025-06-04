using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour
{
    public Texture blackTexture;
    private float alpha = 1.0f;
    public float fadespeed = 0.2f;
    private int fadeDir = -1;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnGUI()
    {
        alpha += fadeDir * fadespeed * Time.deltaTime;
        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), blackTexture);
    }

    public float BeginFade(int direction)
    {
        fadeDir = direction;
        return 1 / fadespeed;
    }

    void OnLevelWasLoaded()
    {
        Debug.Log("≥°æ∞º”‘ÿÕÍ±œ£°");
        BeginFade(-1);
    }
}
