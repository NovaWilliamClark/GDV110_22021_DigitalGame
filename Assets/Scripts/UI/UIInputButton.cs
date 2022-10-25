using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInputButton : MonoBehaviour, IUIButton
{
    public event EventHandler Clicked;
    public event EventHandler InputPerformed;
    
    public void OnInput()
    {
    }

    public void OnClick()
    {
    }
}
