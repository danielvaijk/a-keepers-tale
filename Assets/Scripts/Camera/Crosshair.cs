using UnityEngine;
using System.Collections;

public class Crosshair : MonoBehaviour
{
    public Texture2D crosshairTexture;

    private void Start ()
    {
        Cursor.visible = false;
    }

    private void OnGUI ()
    {
        Rect crosshairRect = new Rect(Screen.width / 2 - 2.5f, Screen.height / 2 - 2.5f, 5, 5);

        GUI.DrawTexture(crosshairRect, crosshairTexture, ScaleMode.StretchToFill);
    }
}