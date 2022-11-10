using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Objects
{
	public class MovableObject : InteractionPoint
	{
		public bool isMoving = false;
		[SerializeField] public float moveVelocity;
		protected override void Interact(CharacterController cc)
		{
		}
		
		
	}
}

