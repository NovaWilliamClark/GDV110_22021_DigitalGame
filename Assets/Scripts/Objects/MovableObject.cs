using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.UI;

namespace Objects
{
	public class MovableObject : InteractionPoint
	{
		[Header("Box")] 
		[SerializeField] private bool canMove;

		[SerializeField] private bool canClimb;
		private bool nearLedge = false;
		private bool isGrounded;
		private bool onBox;
		[SerializeField] private LayerMask groundLayers;
		[SerializeField] private float groundCastOffset = 0.5f;
		private float playerDirection;

		private bool ready = false;

		private Vector3 ledgeLeftCorner;
		private Vector3 leftRightCorner;

		private Rigidbody2D rigidbody;
		private BoxCollider2D boxCollider;
		private bool playerAbove;
		
		private InputAction interactInput;
		private InputAction climbInput;

		private MoveState state;

		private Vector3 positionLastFrame;
		private bool collidedWhileMoving;

		public enum MoveState
		{
			Wait,
			Moving,
			Climb,
		}
		
		protected override void Awake()
		{
			levelController = FindObjectOfType<LevelController>();
			persistentObject = GetComponent<PersistentObject>();
			InitVisuals();
			rigidbody = GetComponent<Rigidbody2D>();
			boxCollider = GetComponent<BoxCollider2D>(); 
			levelController.PlayerSpawned.AddListener(OnPlayerSpawned);
		}

		private void OnPlayerSpawned(CharacterController cc)
		{
			playerRef = cc;
			levelController.PlayerSpawned.RemoveListener(OnPlayerSpawned);
			//playerRef.Input.Player.
			interactInput = playerRef.Input.Player.Interact;
			climbInput = playerRef.Input.Player.Climb;
			
			ready = true;
		}

		private void ListenForInput()
		{
			if (!playerInRange) return;

			if (playerAbove || playerRef.IsFalling) return;
			if (playerRef.movableObject)
			{
				if (playerRef.movableObject != this)
				{
					return;
				}
			}
			
			if (interactInput.IsPressed())
			{
				if (canMove)
				{
					ChangeState(MoveState.Moving);
				}
			}

			if (climbInput.WasReleasedThisFrame())
			{
				if (canClimb)
				{
					ChangeState(MoveState.Climb);
				}
			}
		}

		private void ChangeState(MoveState changeTo)
		{
			state = changeTo;
			Debug.Log(changeTo);
		}

		private void DoMove()
		{
			// player released input - transition back to wait for input
			if (interactInput.WasReleasedThisFrame() || !isGrounded)
			{
				collidedWhileMoving = false;
				playerRef.movableObject = null;
				boxCollider.isTrigger = false;
				transform.SetParent(null);
				playerRef.isMovingObject = false;
				
				rigidbody.bodyType = RigidbodyType2D.Dynamic;
				ChangeState(MoveState.Wait);
				return;
			}
			
			rigidbody.bodyType = RigidbodyType2D.Kinematic;
			boxCollider.isTrigger = true;
			playerRef.movableObject = this;
			
			// flip player to face box
			if (playerRef.IsFacingLeft() && playerDirection == -1)
			{
				playerRef.SetIsFlipped(false);
			} else if (!playerRef.IsFacingLeft() && playerDirection == 1)
			{
				playerRef.SetIsFlipped(true);
			}

			playerRef.isMovingObject = true;
			transform.parent = playerRef.transform;
		}

		float DirectionToPlayer()
		{
			return Mathf.Sign(playerRef.transform.position.x - transform.position.x);
		}
		
		private void DoClimb()
		{
			// -1 player is left of, 1 player is right of
			if (playerDirection == -1 && playerRef.IsFacingLeft() || playerDirection == 1 && !playerRef.IsFacingLeft())
			{
				// player isn't facing box
				 ChangeState(MoveState.Wait);
				return;
			}

			if (!nearLedge)
			{
				ChangeState(MoveState.Wait);
				return;
			}

			var top = boxCollider.bounds.center;
			top.y = boxCollider.bounds.max.y;
			var playerColl = playerRef.GetComponent<CapsuleCollider2D>();
			top.y += playerColl.size.y / 2f;
			playerRef.transform.position = top;
			ChangeState(MoveState.Wait);
		}

		protected override void Update()
		{
			if (!ready) return;
			//canInteract = canMove;

			canInteract = canMove || canClimb;

			if (playerInRange)
			{
				playerDirection = Mathf.Sign(playerRef.transform.position.x - transform.position.x);
			}

			// show glow/visuals
			base.Update();

			switch (state)
			{
				case MoveState.Wait:
					ListenForInput();
					break;
				case MoveState.Moving:
					DoMove();
					break;
				case MoveState.Climb:
					DoClimb();
					break;
				default:
					Debug.Log("No state set");
					break;
			}
		}

		protected override void OnTriggerEnter2D(Collider2D other)
		{
			if (!other.CompareTag("Player")) return;
			
		}

		protected override void OnTriggerStay2D(Collider2D other)
		{
			if (!other.CompareTag("Player")) return;
			if (!canInteract) return;
			if (playerAbove)
			{
				DisablePrompt();
			}
			else if (!promptBox.IsVisible || !promptBox.isActiveAndEnabled)
			{
				string msg;
				if (canClimb && !canMove)
				{
					msg = "Climb - Space";
				}
				else
				{
					msg = promptMessage;
				}
				promptBox.gameObject.SetActive(true);
				ResetPrompt(msg);
			}
		}

		protected override void OnTriggerExit2D(Collider2D other)
		{
			if (!other.CompareTag("Player")) return;
			if (!automaticInteraction)
			{
				DisablePrompt();
            
				if (canReInteract)
				{
					canInteract = true;
				}
			}
		}

		protected override void FixedUpdate()
		{
			if (!ready) return;
			playerInRange = Physics2D.OverlapCircle(transform.position, fxRange, LayerMask.GetMask("Player"));

			var above = boxCollider.bounds.center;
			var size = boxCollider.size;
			above.y = boxCollider.bounds.max.y + size.y/4f;
			var aboveColliders = Physics2D.OverlapBoxAll(above, size / 2, 0);
			var aboveSorted = new List<Collider2D>();
			foreach (var aboveCollider in aboveColliders)
			{
				if (aboveCollider.transform == transform)
				{
					continue;
				}

				if (aboveCollider.isTrigger)
				{
					if (!aboveCollider.CompareTag("Moveable"))
						continue;
				}
				aboveSorted.Add(aboveCollider);
			}

			playerAbove = aboveSorted.Find(p => p.transform == playerRef.transform);
			canClimb = aboveSorted.Count == 0;
			
			DebugDrawBox(above, size/2,0,Color.yellow, 0);

			// ray cast ledges
			var leftCorner = boxCollider.bounds.max;
			leftCorner.x = boxCollider.bounds.min.x;
			ledgeLeftCorner = leftCorner;
			var rightCorner = boxCollider.bounds.max;
			leftRightCorner = rightCorner;
			var leftLedge = RaycastLedge(leftCorner, transform.TransformDirection(Vector3.left), 5f);
			var rightLedge = RaycastLedge(rightCorner, transform.TransformDirection(Vector3.right), 5f);

			nearLedge = leftLedge || rightLedge;

			// ground check
			var groundBoxSize = boxCollider.size;
			groundBoxSize.x += groundBoxSize.x * 0.5f;
			groundBoxSize.y /= 2f;
			
			var bottom = boxCollider.bounds.center;
			bottom.y = boxCollider.bounds.min.y - groundCastOffset - groundBoxSize.y/2f;

			var ground = Physics2D.OverlapBoxAll(bottom, groundBoxSize, 0, groundLayers);
			var groundSorted = ground.Where(g => g.transform != transform).ToList();
			isGrounded = groundSorted.Count > 0;
			DebugDrawBox(bottom,groundBoxSize,0,Color.red, 0);

			if (state == MoveState.Moving)
			{
				// player acts as collision point when moving in their direction
				// only need to check opposite side
				var bounds = boxCollider.bounds;
				var moveCastPosition = bounds.center;
				Vector3 moveCastSize = boxCollider.size / 2;
				
				// moving left
				if (!IsMovingRight())
				{
					moveCastPosition.x = bounds.min.x - (moveCastSize / 2).x;
				}
				// moving right
				else if (IsMovingRight())
				{
					moveCastPosition.x = bounds.max.x + (moveCastSize / 2).x;
				}
				
				DebugDrawBox(moveCastPosition,moveCastSize,0f,Color.yellow,0f);
				var hitColliders = Physics2D.OverlapBoxAll(moveCastPosition, moveCastSize, 0f);
				foreach (var coll in hitColliders)
				{
					if (coll.transform == transform)
						continue;
					if (coll.isTrigger)
						continue;

					float diff = 0f;
					if (IsMovingRight())
					{
						// collision while moving right ?
						if (boxCollider.bounds.max.x > coll.bounds.min.x)
						{
							diff = coll.bounds.min.x - boxCollider.bounds.max.x;
							collidedWhileMoving = true;
						}
					}
					else if (!IsMovingRight())
					{
						// collision while moving left ?
						if (boxCollider.bounds.min.x < coll.bounds.max.x)
						{
							diff = coll.bounds.max.x - boxCollider.bounds.min.x;
							collidedWhileMoving = true;
						}
					}
					transform.position = new Vector3(transform.position.x + diff, transform.position.y,
						transform.position.z);
				}
			}
			 
			// non-movable boxes should just set themselves as static
			if (state == MoveState.Wait)
			{
				if (isGrounded && !canMove)
				{
					rigidbody.bodyType = RigidbodyType2D.Static;
				}
			}

			positionLastFrame = transform.position;
		}

		private bool IsMovingRight()
		{
			var moveDirection = transform.position - positionLastFrame;
			if (moveDirection.x < 0f && playerDirection > 0f)
			{
				return false;
			}
			return moveDirection.x > 0f && playerDirection < 0f;
		}
		
		RaycastHit2D RaycastLedge(Vector3 ledgePosition, Vector3 direction, float distance)
		{
			Debug.DrawRay(ledgePosition, direction.normalized * distance, Color.magenta);
			return Physics2D.Raycast(ledgePosition, direction, distance, LayerMask.GetMask("Player"));
		}

		void DebugDrawBox( Vector2 point, Vector2 size, float angle, Color color, float duration) {

			var orientation = Quaternion.Euler(0, 0, angle);

			// Basis vectors, half the size in each direction from the center.
			Vector2 right = orientation * Vector2.right * size.x/2f;
			Vector2 up = orientation * Vector2.up * size.y/2f;

			// Four box corners.
			var topLeft = point + up - right;
			var topRight = point + up + right;
			var bottomRight = point - up + right;
			var bottomLeft = point - up - right;

			// Now we've reduced the problem to drawing lines.
			Debug.DrawLine(topLeft, topRight, color, duration);
			Debug.DrawLine(topRight, bottomRight, color, duration);
			Debug.DrawLine(bottomRight, bottomLeft, color, duration);
			Debug.DrawLine(bottomLeft, topLeft, color, duration);
		}
	}
}