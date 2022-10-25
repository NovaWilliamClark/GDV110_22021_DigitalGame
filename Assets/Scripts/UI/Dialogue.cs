/*******************************************************************************************
*
*    File: Dialogue.cs
*    Purpose: Supplies the dialogue
*    Author: Joshua Stephens
*    Date: 25/10/2022
*
**********************************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[Serializable]
public class Dialogue
{
    public string name;
    
    [TextArea(3, 10)]
    public string[] sentences;
}
