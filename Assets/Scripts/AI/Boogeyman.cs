using System.Collections;
using System.Collections.Generic;
using Character;
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
    private float attackTimer;
    private bool attackIsCharging;
    private bool isInGhostForm;
    private LevelController levelController;
    private float maxMoveDistance = 40;

    private void Awake()
    {
        levelController = FindObjectOfType<LevelController>();
        levelController.PlayerSpawned.AddListener(OnPlayerSpawn);
    }

    private void OnPlayerSpawn(CharacterController cc)
    {
        player = cc;
        playerPosition = player.transform.position;
        this.GameObject().SetActive(false);
    }

    public void OnBossFightTrigger()
    {
        this.GameObject().SetActive(true);
    }

    private void Update()
    {
        if (playerPosition.x >= deathWallPosition)
        {
            player.GetComponent<CharacterSanity>().Instakill();
        }
        else if (isInGhostForm)
        {
            Physics2D.IgnoreCollision(collider, player.GetComponent<Collider2D>());
            Movement();
        }
        else if (attackTimer >= attackCooldown)
        {
            ChargeAttack();
            attackIsCharging = true;
            isInGhostForm = false;
            GhostFormFadeOut();
        }
    }

    private void Movement()
    {
        Vector3 currentPosition = transform.position;
        
        if (Mathf.Abs(playerPosition.x - currentPosition.x) > 50)
        {
            animator.SetBool("IsMoving", true);
            currentPosition = Vector3.MoveTowards(currentPosition, playerPosition, maxMoveDistance * Time.deltaTime);
            transform.position = currentPosition;
        }
    }

    private void ChargeAttack()
    {
        // Play charge up animation
        StartCoroutine(Attack());
    }

    public void OnBossFightComplete()
    {
        // Fade out
        this.GameObject().SetActive(false);
    }

    IEnumerator Attack()
    {
        yield return new WaitForSeconds(3);

        attackIsCharging = false;
        animator.SetTrigger("Melee");
        isInGhostForm = true;
        GhostFormFadeIn();
    }

    private void GhostFormFadeIn()
    {
        // Fade in
    }

    private void GhostFormFadeOut()
    {
        // Fade out
    }
}
