using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Objects
{
	public class MovableObject : MonoBehaviour
	{
		public bool isMoving = false;
		[SerializeField] public float moveVelocity;
	}
}

