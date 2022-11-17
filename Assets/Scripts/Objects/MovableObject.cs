using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.UI;

namespace Objects
{
	public class MovableObject : InteractionPoint
	{
		public bool isMoving = false;
		[SerializeField] public float moveVelocity;
		
		[Header("Push & Pull")] [SerializeField]
		private bool canMoveObject;
		private float movementAcceleration;
		private RaycastHit2D boxCastHit;
		private CharacterController playerController;
		private Collider2D boxCollider;
		public static event Action<bool> OnObjectMove;
		private CharacterController cc;
		[SerializeField] private float interactionDistance;
		private bool isOnBox = false;

		protected override void Awake()
		{
			base.Awake();
			boxCollider = GetComponentInChildren<Collider2D>();
			var lc = FindObjectOfType<LevelController>();
			lc.PlayerSpawned.AddListener(OnPlayerLoaded);
			cc = FindObjectOfType<CharacterController>();
		}

		private void OnPlayerLoaded(CharacterController controller)
		{
			playerController = GameObject.FindWithTag("Player").GetComponent<CharacterController>();
		}

		protected override void Update()
		{
			base.Update();
			MoveObject();
		}

		protected override void OnTriggerEnter2D(Collider2D other)
		{
			base.OnTriggerEnter2D(other);
			if (other.GetComponent<CharacterController>())
			{
				canMoveObject = true;
				
			}
			
		}

		protected override void OnTriggerExit2D(Collider2D other)
		{
			if (other.GetComponent<CharacterController>())
			{
				canMoveObject = false;
			}
			base.OnTriggerExit2D(other);
			
		}

		private void MoveObject()
		{
			if (canMoveObject && Input.GetKey(KeyCode.Mouse1))
			{
				OnObjectMove?.Invoke(true);
				gameObject.transform.SetParent(playerController.transform);
				// Slow PLayer Movement
				playerController.acceleration = moveVelocity;
				playerController.isMovingObject = true;
				
			}
			else if (canMoveObject && Input.GetKeyDown(KeyCode.Space))
			{
				// teleport to above the box
				
				var top = boxCollider.bounds.center;
				top.y = boxCollider.bounds.max.y;
				top.y -= boxCollider.bounds.center.y;
				playerController.gameObject.transform.position = top;
				gameObject.transform.SetParent(null);
				playerController.isMovingObject = false;
				playerController.acceleration = 40.0f;
				
			}
			else
			{
				OnObjectMove?.Invoke(false);
				
				gameObject.transform.SetParent(null);
				playerController.isMovingObject = false;
				playerController.acceleration = 40.0f;
			}
		}
	}
}