using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Character;
using Core;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;

public class Boogeyman : MonoBehaviour, ILightResponder
{
    public float attackCooldown;
    public Animator animator;
    public Collider2D collider;
    [SerializeField] private Transform puppet = null;
    
    private CharacterController player;
    private Vector3 playerPosition;
    private float deathWallPosition = 1200;
    private float attackTimer = 0;
    private bool attackIsCharging;
    private bool isInGhostForm;
    private LevelController levelController;
    private float maxMoveDistance = 30;
    private SpriteRenderer[] spriteRenderer;
    private int currentPhase = 0;
    public Vector3[] patrolPoints;
    private int currentPatrolPoint;
    private int health = 100;
    private int lightHits = 0;

    private void Awake()
    {
        levelController = FindObjectOfType<LevelController>();
        levelController.PlayerSpawned.AddListener(OnPlayerSpawn);
        spriteRenderer = GetComponentsInChildren<SpriteRenderer>();
        isInGhostForm = true;
        attackIsCharging = false;
    }

    private void OnPlayerSpawn(CharacterController cc)
    {
        player = cc;
        gameObject.SetActive(false);
    }

    public void OnBossFightTrigger()
    {
        gameObject.SetActive(true);
        
        // Reset boss animation to idle
        
        deathWallPosition -= 300;
        currentPhase++;

        if (currentPhase == 2)
        {
            this.transform.position = patrolPoints[3];
            maxMoveDistance = 20;
        }
        
        attackTimer = 0;
        GhostFormFadeIn();
    }

    private void Update()
    {
        playerPosition = player.transform.position;

        if (health <= 0)
        {
            Destroy(gameObject);
        }
        
        if (playerPosition.x >= deathWallPosition)
        {
            // Play animation for instakill move
            player.GetComponent<CharacterSanity>().Instakill();
        }
        else if (attackTimer >= attackCooldown && attackIsCharging == false && currentPhase == 1)
        {
            attackIsCharging = true;
            isInGhostForm = false;
            GhostFormFadeOut();
            Debug.Log("Charging attack");
            StartCoroutine(ChargeAttack());
        }
        else if (isInGhostForm)
        {
            Movement();
        }
        else if (!isInGhostForm && !attackIsCharging && currentPhase == 2)
        {
            attackIsCharging = true;
            Debug.Log("Charging attack");
            StartCoroutine(ChargeAttack());
        }

        attackTimer += 1 * Time.deltaTime;
    }

    private void Movement()
    {
        Vector3 currentPosition = transform.position;
        lightHits = 0;
        
        if (Mathf.Abs(playerPosition.x - currentPosition.x) > 20 && currentPhase == 1)
        {
            // Set bool for animator isMoving
            currentPosition = Vector3.MoveTowards(currentPosition, playerPosition, maxMoveDistance * Time.deltaTime);
            transform.position = currentPosition;
        }
        else if (currentPhase == 2)
        {
            // Set bool for animator isMoving
            currentPosition = Vector3.MoveTowards(currentPosition, patrolPoints[currentPatrolPoint], maxMoveDistance * Time.deltaTime);
            transform.position = currentPosition;

            if (transform.position == patrolPoints[currentPatrolPoint])
            {
                if (playerPosition.x > currentPosition.x)
                {
                    puppet.localScale = Vector3.one;
                }
                else
                {
                    puppet.localScale = new Vector3(-1, 1, 1);
                }
                
                isInGhostForm = false;
                GhostFormFadeOut();
            }
        }
    }

    IEnumerator ChargeAttack()
    {
        animator.Play("Attack");
        yield return new WaitForSeconds(2);
        
        Attack();
    }

    public void OnBossFightComplete()
    {
        // Fade out
        this.GameObject().SetActive(false);
    }

    private void Attack()
    {
        Debug.Log("The Boogeyman has attacked");
        attackIsCharging = false;
        isInGhostForm = true;
        attackTimer = 0;
        GhostFormFadeIn();

        if (currentPhase == 2 && currentPatrolPoint < 3)
        {
            currentPatrolPoint++;
        }
        else if (currentPhase == 2 && currentPatrolPoint == 3)
        {
            currentPatrolPoint = 0;
        }
    }

    private void GhostFormFadeIn()
    {
        foreach (var sr in spriteRenderer)
        {
            sr.DOFade(0.5f, 0.5f);
        }
        
        Physics2D.IgnoreCollision(collider, player.GetComponent<Collider2D>());
    }

    private void GhostFormFadeOut()
    {
        foreach (var sr in spriteRenderer)
        {
            sr.DOFade(1f, 0.5f);
        }
        
        Physics2D.IgnoreCollision(collider, player.GetComponent<Collider2D>(), false);
    }
    
    public void OnLightEntered(float intensity)
    {
    }

    public void OnLightExited(float intensity)
    {
    }

    public void OnLightStay(float intensity)
    {
        if (!isInGhostForm && currentPhase == 2)
        {
            lightHits++;
            
            if (lightHits >= 3)
            {
                health -= 10;
                isInGhostForm = true;
                GhostFormFadeIn();
            
                if (currentPhase == 2 && currentPatrolPoint < 3)
                {
                    currentPatrolPoint++;
                }
                else if (currentPhase == 2 && currentPatrolPoint == 3)
                {
                    currentPatrolPoint = 0;
                }
            }
        }
    }
}
