using System.Collections;
using System.Collections.Generic;
using Character;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Boogeyman : MonoBehaviour
{
    public float attackCooldown;
    public Animator animator;
    public Collider2D collider;
    
    private CharacterController player;
    private Vector3 playerPosition;
    private float deathWallPosition;
    private float attackTimer = 0;
    private bool attackIsCharging;
    private bool isInGhostForm;
    private LevelController levelController;
    private float maxMoveDistance = 30;
    private SpriteRenderer[] spriteRenderer;

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
        this.GameObject().SetActive(false);
    }

    public void OnBossFightTrigger()
    {
        this.GameObject().SetActive(true);
        
        // Set this to be the triggers position (maybe minus a certain amount)
        deathWallPosition = 900;
        
        attackTimer = 0;
        GhostFormFadeIn();
    }

    private void Update()
    {
        playerPosition = player.transform.position;
        
        if (playerPosition.x >= deathWallPosition)
        {
            // Play animation for instakill move
            player.GetComponent<CharacterSanity>().Instakill();
        }
        else if (attackTimer >= attackCooldown && attackIsCharging == false)
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

        attackTimer += 1 * Time.deltaTime;
    }

    private void Movement()
    {
        Vector3 currentPosition = transform.position;
        
        if (Mathf.Abs(playerPosition.x - currentPosition.x) > 20)
        {
            // Set bool for animator isMoving
            Debug.Log("The Boogeyman is moving");
            currentPosition = Vector3.MoveTowards(currentPosition, playerPosition, maxMoveDistance * Time.deltaTime);
            transform.position = currentPosition;
        }
    }

    IEnumerator ChargeAttack()
    {
        yield return new WaitForSeconds(2);
        
        // Play charge up animation
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
        // Set animation trigger for melee attack
        isInGhostForm = true;
        attackTimer = 0;
        GhostFormFadeIn();
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
}
