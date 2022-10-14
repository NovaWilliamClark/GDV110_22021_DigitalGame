/*******************************************************************************************
*
*    File: EnemyBehaviour.cs
*    Purpose: Character Controller Movement and Interaction
*    Author: Joshua Stephens
*    Date: 11/10/2022
*
**********************************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using AI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyBehaviour : MonoBehaviour
{
    public Transform rayCast;
    public LayerMask raycastMask;
    [SerializeField] public float rayCastLength;
    [SerializeField] public float attackDistance;
    [SerializeField] public float moveSpeed;
    [SerializeField] public float atkCooldownTime;
    [SerializeField] public Animator animator = null;

    private RaycastHit2D hit;
    private GameObject target;
    private float distance;
    private bool attackMode;
    private bool inRange;
    private bool inCooldown;
    [HideInInspector] public Animator weaponAnimator;


    void Start()
    {
        weaponAnimator = GetComponentInChildren<EnemyWeapon>().GetComponent<Animator>();
    }
    
    // Update is called once per frame
    void Update()
    {
        if (inRange)
        {
            hit = Physics2D.Raycast(rayCast.position, Vector2.left, rayCastLength, raycastMask);
            //RaycastDebugger();
        }

        if (hit.collider != null)
        {
            EnemyLogic();
        }
        else if (hit.collider == null)
        {
            inRange = false;
        }
    }

    void OnTriggerEnter2D(Collider2D trig)
    {
        if (trig.gameObject.tag == "Player")
        {
            target = trig.gameObject;
            inRange = true;
        }
    }

    void EnemyLogic()
    {
        distance = Vector2.Distance(transform.position, target.transform.position);

        if (distance > attackDistance && !attackMode)
        {
            Move();
        }
        else if (attackDistance >= distance && inCooldown == false)
        {
            Attack();
        }
        
    }

    void Move()
    { 
        Vector2 targetPosition = new Vector2(target.transform.position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
      
    }

    void Attack()
    {
        inCooldown = true;
        attackMode = true;
        animator.SetTrigger("Melee");
    }

    // void RaycastDebugger()
   // {
   //     if (distance > attackDistance)
   //     {
   //         Debug.DrawRay(rayCast.position, Vector2.left * rayCastLength, Color.red);
   //     }
   //     else if (attackDistance > distance)
   //     {
   //         Debug.DrawRay(rayCast.position, Vector2.left * rayCastLength, Color.green);
   //     }
   // }

    public IEnumerator AttackCooldownReset()
    {
        yield return new WaitForSeconds(atkCooldownTime);
        inCooldown = false;
        attackMode = false;
    }
}
