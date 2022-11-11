using System;
using Character;
using UnityEngine;

namespace AI
{
    public class StalkerAttackCollider : MonoBehaviour
	{
		[SerializeField] private float attackDamage;
		[SerializeField] private Vector2 collisionAttackBoxSize;
		
		private Stalker StalkerInst;
		[HideInInspector] public BoxCollider2D collisonBox;

		private void Awake()
		{
			StalkerInst = GetComponentInParent<Stalker>();
			collisonBox = GetComponent<BoxCollider2D>();
		}
		private void Start()
		{
			collisonBox.size = new Vector2(0, 0);
		}

		public void ActivateCollisionBox()
		{
			collisonBox.size = new Vector2(15, 20);
			StartCoroutine(StalkerInst.AttackCooldownReset());
		}

		public void DeactivateCollisionBox()
		{
			collisonBox.size = new Vector2(0, 0);
		}
		
		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (collision.gameObject.CompareTag("Player"))
			{
				HandleCollision(collision.GetComponent<CharacterController>());
			}
		}

		private void HandleCollision(CharacterController obj)
		{
			obj.GetComponent<CharacterSanity>().DecreaseSanity(attackDamage, false);
		}
	}
}
