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
		
		protected override void FixedUpdate()
		{
			base.FixedUpdate();
			MoveObject();
		}

		protected override void OnTriggerEnter2D(Collider2D other)
		{
			if (other.GetComponent<CharacterController>())
			{
				canMoveObject = true;
				
			}
			base.OnTriggerEnter2D(other);
		}

		protected override void OnTriggerExit2D(Collider2D other)
		{
			if (other.GetComponent<CharacterController>())
			{
				canMoveObject = false;
			}
			
			//*/
		}

		private void MoveObject()
		{
			if (canMoveObject && Input.GetKey(KeyCode.Mouse1))
			{
				OnObjectMove?.Invoke(true);
				// Slow PLayer Movement
				playerController.acceleration = moveVelocity;
				playerController.isMovingObject = true;
				GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
				gameObject.transform.SetParent(playerController.transform);
				//Move Object   
				//gameObject.transform.Translate(playerController.movementVelocity.x / 50, 0, 0);
			}
			else if (canMoveObject && Input.GetKeyDown(KeyCode.Space))
			{
				// teleport to above the box
				var top = boxCollider.bounds.center;
				top.y = boxCollider.bounds.max.y;
				top.y -= boxCollider.bounds.center.y;
				playerController.gameObject.transform.position = top;
				GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
				gameObject.transform.SetParent(null);
				playerController.acceleration = 40.0f;
			}
			else
			{
				OnObjectMove?.Invoke(false);
				playerController.isMovingObject = false;
				gameObject.transform.SetParent(null);
				GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
				playerController.acceleration = 40.0f;
			}
		}
	}
}