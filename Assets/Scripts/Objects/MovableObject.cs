using System;
using Unity.VisualScripting;
using UnityEngine;

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


		protected override void Awake()
		{
			base.Awake();
			boxCollider = GetComponentInChildren<Collider2D>();
			var lc = FindObjectOfType<LevelController>();
			lc.PlayerSpawned.AddListener(OnPlayerLoaded);
		}

		private void OnPlayerLoaded(CharacterController controller)
		{
			playerController = GameObject.FindWithTag("Player").GetComponent<CharacterController>();
		}
		
		protected override void FixedUpdate()
		{
			MoveObject();
			base.FixedUpdate();
		}

		protected override void OnTriggerEnter2D(Collider2D other)
		{
			base.OnTriggerEnter2D(other);
			if (!other.gameObject.CompareTag("Player"))
			{
				return;
			}
			
			// TODO: SET KINEMATIC IF PLAYER IS ON TOP
			canMoveObject = true;
			GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
		}

		protected override void OnTriggerExit2D(Collider2D other)
		{
			canMoveObject = false;
		}

		private void MoveObject()
		{
			if (canMoveObject && Input.GetKey(KeyCode.Mouse1))
			{
				OnObjectMove?.Invoke(true);
				// Slow PLayer Movement
				playerController.acceleration = moveVelocity;
				playerController.isMovingObject = true;

				//Move Object   
				gameObject.transform.Translate(playerController.movementVelocity.x / 50, 0, 0);
			}
			else if (canMoveObject && Input.GetKeyUp(KeyCode.Space))
			{
				// teleport to above the box
				var top = boxCollider.bounds.center;
				top.y = boxCollider.bounds.max.y;
				top.y -= boxCollider.bounds.center.y;
				playerController.gameObject.transform.position = top;
			}
			else
			{
				OnObjectMove?.Invoke(false);
				playerController.isMovingObject = false;
				//acceleration = 30.0f;
			}
		}
	}
}