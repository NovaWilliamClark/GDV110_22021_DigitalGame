/*******************************************************************************************
*
*    File: EnemyBehaviour.cs
*    Purpose: Enemy movement and interaction
*    Author: Joshua Stephens
*    Date: 11/10/2022
*
**********************************************************************************************/

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
    public float rayCastLength;
    public float attackDistance;
    public float moveSpeed;
    public float timer;
    
    private RaycastHit2D hit;
    private GameObject target;
    private Animator weaponAnimator;
    private float distance;
    private bool attackMode;
    private bool inRange;
    private bool isInCooldown;
    private float intTimer;

    private bool CanAttack { get; set; }

    void Start()
    {
        weaponAnimator = GetComponentInChildren<EnemyWeapon>().GetComponent<Animator>();
    }
    
    void Awake()
    {
        intTimer = timer;
    }

    // Update is called once per frame
    void Update()
    {
        if (inRange)
        {
            hit = Physics2D.Raycast(rayCast.position, Vector2.left, rayCastLength, raycastMask);
        }

        if (hit.collider != null)
        {
            EnemyLogic();
        }
        else if (hit.collider == null)
        {
            inRange = false;
        }

        if (inRange == false)
        {
            StopAttack();
        }
    }

    void OnTriggerEnter2D(Collider2D trig)
    {
        if (trig.gameObject.CompareTag("Player"))
        {
            target = trig.gameObject;
            inRange = true;
        }
    }

    void EnemyLogic()
    {
        distance = Vector2.Distance(transform.position, target.transform.position);

        if (distance > attackDistance)
        {
            Move();
            StopAttack();
        }
        else if (attackDistance >= distance && isInCooldown == false)
        {
            Attack();
        }

        if (isInCooldown)
        {
            Cooldown();
        }
    }

    void Move()
    {
        Vector2 targetPosition = new Vector2(target.transform.position.x, transform.position.y);

        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    void Attack()
    {
        timer = intTimer;
        attackMode = true;
        isInCooldown = true;
        weaponAnimator.SetTrigger("MeleeSwipe");
    }

    void Cooldown()
    {
        timer -= Time.deltaTime;

        if (timer <= 0 && isInCooldown)
        {
            isInCooldown = false;
            timer = intTimer;
        }
    }
    
    void StopAttack()
    {
        attackMode = false;
    }
}
