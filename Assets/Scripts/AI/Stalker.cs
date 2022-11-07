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
		[SerializeField] private float chargeVelocity = 20;
		[SerializeField] private float stopDistance = 15f;
		[SerializeField] private float screamTime = 3f;
		[SerializeField] private float targetDistanceThreshold = 5f;
		[SerializeField] private float screamDamage;
		[SerializeField] private AudioClip screamSound;

		private bool isChasing;
		private bool isScreaming;
		private bool hasScreamed;

		private bool hasCachedPlayer;
		private bool hasCachedPlayerCameraRef;
		
		private GameObject targetPlayer;
		private Vector2 targetPosition;
		private float distanceToTarget;

		private SanityVisual sanityMeter;

		private CinemachineVirtualCamera virtualCamera;

		// TODO: When Screaming the camera should flick over to the stalker for a moment

		// Start is called before the first frame update

		private void Awake()
		{
			virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
			sanityMeter =  GameObject.Find("SanityMeter_Alt").GetComponent<SanityVisual>();
		}

		void Start()
        {
			
		}

        // Update is called once per frame
		void Update()
		{
			// TODO: Callback Function after everything in loaded
			targetPlayer = GameObject.FindWithTag("Player");
			if (targetPlayer)
			{
				var tPosition = targetPlayer.transform.position;
				targetPosition = new Vector2(tPosition.x, transform.position.y);
				distanceToTarget = Vector2.Distance(targetPosition, transform.position);
			}

			if (!isChasing && !isScreaming && distanceToTarget <= targetDistanceThreshold && distanceToTarget > stopDistance)
			{
				// TODO: Switch to Scream Animation
				// TODO: Play Scream VFX

				if (!hasScreamed)
				{
					AudioManager.Instance.PlaySound(screamSound);
					StartCoroutine(CameraShift());
				}
				StartCoroutine(ScreamTime());
			}
		}

		private void FixedUpdate()
		{
			if (isChasing)
			{
				Move();
			}
		}

		private void Move()
		{
			transform.position = Vector2.MoveTowards(transform.position, targetPosition, chargeVelocity * Time.deltaTime);
			if (distanceToTarget <= stopDistance)
			{
				isChasing = false;
				Debug.Log("STALKER ATTACKING");
			}
		}

		private IEnumerator CameraShift()
		{
			sanityMeter.ToggleVisibility(true, 0.5f);
			virtualCamera.enabled = true;
			yield return new WaitForSeconds(60f / 100f * screamTime);
			virtualCamera.enabled = false;
			sanityMeter.ToggleVisibility(false, 0.5f);
		}

		private IEnumerator ScreamTime()
		{
			hasScreamed = true;
			isScreaming = true;
			yield return new WaitForSeconds(screamTime);
			isScreaming = false;
			isChasing = true;
			targetPlayer.GetComponent<CharacterSanity>().DecreaseSanity(screamDamage, false);
		}

		private void OnDrawGizmos()
		{
			Matrix4x4 oldMatrix = Gizmos.matrix;
			Gizmos.color = Color.red;
			Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, new Vector3(1f, 1f, 0.01f));
			Gizmos.DrawWireSphere(Vector3.zero, targetDistanceThreshold);
			Gizmos.matrix = oldMatrix;
		}

		public void OnLightEntered(float intensity)
		{ }
		public void OnLightExited(float intensity)
		{ }

		public void OnLightStay(float intensity)
		{
			
		}
	}
}
