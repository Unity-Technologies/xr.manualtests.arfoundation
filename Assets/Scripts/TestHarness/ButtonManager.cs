using System;
using UnityEngine;

public static class ButtonManager
{
    const int k_ButtonWidth = 300;

    const int k_ButtonHeight = 100;

    static int m_X;

    static int m_Y;

    static GUIStyle s_ButtonStyle;

    static GUIStyle s_LabelStyle;

    public static GUIStyle buttonStyle
    {
        get
        {
            EnsureButtonStyleExists();
            return s_ButtonStyle;
        }
    }

    public static GUIStyle labelStyle
    {
        get
        {
            if (s_LabelStyle == null)
            {
                s_LabelStyle = new GUIStyle(GUI.skin.label);
                s_LabelStyle.fontSize = 30;
            }

            return s_LabelStyle;
        }
    }

    static int m_LastFrame;

    public static void AddButton(string text, Action onClick)
    {
        if (m_LastFrame != Time.frameCount)
            OnGUI();

        if (GUI.Button(new Rect(m_X, m_Y, k_ButtonWidth, k_ButtonHeight), text, s_ButtonStyle))
            onClick();

        m_X += k_ButtonWidth;
        if (m_X + k_ButtonWidth > Screen.width)
            NextLine();
    }

    public static void NextLine()
    {
        m_X = 0;
        m_Y += k_ButtonHeight;
    }

    static void EnsureButtonStyleExists()
    {
        if (s_ButtonStyle == null)
        {
            s_ButtonStyle = new GUIStyle(GUI.skin.button);
            s_ButtonStyle.fontSize = 24;
        }
    }

    public static void OnGUI()
    {
        m_X = 0;
        m_Y = 100;
        m_LastFrame = Time.frameCount;
        EnsureButtonStyleExists();
    }
}
