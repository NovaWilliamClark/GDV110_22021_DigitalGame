using System;
using System.Collections;
using Audio;
using Character;
using Cinemachine;
using Core;
using Unity.VisualScripting;
using UnityEngine;

namespace AI
{
	/*
	 * Stalker will see the player from a far distance and screech reducing the players sanity
	 * The Stalker will then proceed to chase the player until the it grabs the player
	 * while chasing if the stalker has a light on it, it will slow down the stalker
	 */
	public class Stalker : MonoBehaviour, ILightResponder
	{
		[Header("Movement")]
		[SerializeField] private float chargeVelocity = 20;
		[SerializeField] private float stopDistance = 15f;
		[SerializeField] private float targetDistanceThreshold = 5f;
		private GameObject targetPlayer;
		private CharacterSanity playerSanity;
		private Vector2 targetPosition;
		private Vector2 direction = new (-1, 1);
		private float distanceToTarget;
		private bool isChasing;
		private bool isDisabled;
		private bool isDead;
		
		[Header("LightInteraction")]
		[SerializeField] private float timeBetweenLightStacks = 0.05f;
		[SerializeField] private float slowdownPerStack = 2f;
		private bool isLightLockActive;
		private int lightDebuffCounter;

		[Header("Scream Attack")]
		[SerializeField] private float screamTime = 3f;
		[SerializeField] private float screamDamage;
		[SerializeField] private AudioClip screamSound;
		private bool isScreaming;
		private bool hasScreamed;
		private int screamCounter;


		[Header("Melee Attack")]
		[SerializeField] private float attackCooldownTime = 2;
		private StalkerAttackCollider attackCollider;
		private bool isAttacking;
		private bool isInAttackCooldown;

		[Header("Components")]
		[SerializeField] public Animator animator;
		[SerializeField] private Transform puppet;
		private SanityVisual sanityMeter;
		private CinemachineVirtualCamera virtualCamera;
		private readonly Vector3 flippedScale = new(-1, 1, 1);

		// TODO: When Screaming the camera should flick over to the stalker for a moment

		// Start is called before the first frame update

		private void Awake()
		{
			virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
			sanityMeter = GameObject.Find("SanityMeter_Alt").GetComponent<SanityVisual>();
			attackCollider = GetComponentInChildren<StalkerAttackCollider>();
		}

		// Update is called once per frame
		void Update()
		{
			if (!isDisabled)
			{
				// TODO: Callback Function after everything in loaded
				targetPlayer = GameObject.FindWithTag("Player");
				playerSanity = targetPlayer.GetComponent<CharacterSanity>();
				if (targetPlayer)
				{
					var tPosition = targetPlayer.transform.position;
					targetPosition = new Vector2(tPosition.x, transform.position.y);
					distanceToTarget = Vector2.Distance(targetPosition, transform.position);
				}

				if (!isChasing && !isScreaming && !isAttacking && distanceToTarget <= targetDistanceThreshold &&
					distanceToTarget > stopDistance)
				{
					// TODO: Switch to Scream Animation
					// TODO: Play Scream VFX

					if (!hasScreamed)
					{
						AudioManager.Instance.PlaySound(screamSound);
						screamCounter++;
						StartCoroutine(CameraShift());
						StartCoroutine(ScreamTime());
					}
				}
			}
		}

		private void FixedUpdate()
		{
			if (isChasing)
			{
				Move();
				UpdateDirection();
			}
		}

		private void Move()
		{
			if (!isDisabled)
			{
				transform.position =
					Vector2.MoveTowards(transform.position, targetPosition, chargeVelocity * Time.deltaTime);
				if (distanceToTarget <= stopDistance)
				{
					isChasing = false;

					if (!isInAttackCooldown)
					{
						Attack();
					}
				}
			}
			else if(isDead)
			{
				// Sink Into The Ground
				transform.position = Vector2.MoveTowards(transform.position,
					new Vector2(transform.position.x, transform.position.y - 10), chargeVelocity * Time.deltaTime);
			}
		}
		
		private void UpdateDirection()
		{
			direction = (targetPosition - (Vector2)transform.position).normalized;
			if (direction.x >= 0)
			{
				if (puppet)
				{
					puppet.localScale = flippedScale;
					return;
				}
			}
            
			if (direction.x <= 0)
			{
				if (puppet)
				{
					puppet.localScale = Vector2.one;
				}
			} 
		}

		private void Attack()
		{
			isAttacking = true;
			animator.SetTrigger("Melee");
			isInAttackCooldown = true;
		}
		
		public IEnumerator AttackCooldownReset()
		{
			yield return new WaitForSeconds(attackCooldownTime);
			attackCollider.DeactivateCollisionBox();
			isInAttackCooldown = false;
			isAttacking = false;
			isChasing = true;
		}


		private IEnumerator CameraShift()
		{
			sanityMeter.ToggleVisibility(true, 0.5f);
			virtualCamera.enabled = true;
			targetPlayer.GetComponent<CharacterController>().ToggleActive(false);
			yield return new WaitForSeconds(60f / 100f * screamTime + 1.6f);
			virtualCamera.enabled = false;
			targetPlayer.GetComponent<CharacterController>().ToggleActive(true);
			sanityMeter.ToggleVisibility(false, 0.5f);
		}

		private IEnumerator ScreamTime()
		{
			hasScreamed = true;
			isScreaming = true;
			yield return new WaitForSeconds(screamTime);
			isScreaming = false;
			isChasing = true;
			targetPlayer.GetComponent<CharacterSanity>().DecreaseSanity(screamDamage, true);
		}

		private void OnDrawGizmos()
		{
			Matrix4x4 oldMatrix = Gizmos.matrix;
			Gizmos.color = Color.red;
			Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, new Vector3(1f, 1f, 0.01f));
			Gizmos.DrawWireSphere(Vector3.zero, targetDistanceThreshold);
			Gizmos.matrix = oldMatrix;
		}

		private void ApplyDebuffs()
		{
			if (lightDebuffCounter >= 15)
			{
				isDisabled = true;
				StartCoroutine(OnDeathScream());
				return;
			}

			chargeVelocity -= slowdownPerStack;
		}

		private IEnumerator ApplyLightCounter()
		{
			isLightLockActive = true;
			yield return new WaitForSeconds(timeBetweenLightStacks);
			lightDebuffCounter++;
			isLightLockActive = false;
			ApplyDebuffs();
		}

		private IEnumerator OnDeathScream()
		{
			yield return new WaitForSeconds(1.2f);
			if (screamCounter <= 1)
			{
				isDead = true;
				AudioManager.Instance.PlaySound(screamSound);
				targetPlayer.GetComponent<CharacterSanity>().DecreaseSanity(screamDamage, true);
				screamCounter++;
			}

			yield return new WaitForSeconds(8f);
			Destroy(gameObject);
		}

		public void OnLightEntered(float intensity)
		{
		}

		public void OnLightExited(float intensity)
		{
		}

		public void OnLightStay(float intensity)
		{
			if (!isLightLockActive)
			{
				StartCoroutine(ApplyLightCounter());
			}
		}
	}
}