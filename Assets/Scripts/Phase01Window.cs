using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phase01Window : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Open()
    {
        animator.SetTrigger("Open");
    }
}
