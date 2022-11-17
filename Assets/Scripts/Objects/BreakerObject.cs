using System.Collections;
using System.Collections.Generic;
using Character;
using Objects;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(PersistentObject))]
public class BreakerObject : InteractionPoint
{
	[Header("Breaker")] [SerializeField] private List<ItemData> fusesUsed;
	[SerializeField] private GameObject[] activeFuses;
	[SerializeField] private ItemData triangleFuse;
	[SerializeField] private ItemData squareFuse;
	[SerializeField] private ItemData circleFuse;

	private BreakerState state;
	private int usedFuseCount;

	[SerializeField] private LevelCutscene breakerCutscene;

	private Animator animator;

	protected override void Awake()
	{
		base.Awake();
		animator = GetComponent<Animator>();
		persistentObject = GetComponent<PersistentObject>();
		state = new BreakerState(persistentObject.Id);
		levelController.PlayerSpawned.AddListener(OnPlayerLoaded);
	}

	private void OnPlayerLoaded(CharacterController cc)
	{
		cc.PlayerData.breakerFixed = IsBreakerSatisfied();
	}
	
	protected override void Interact(CharacterController cc)
	{
		var fuseTruthCollection = new bool[3];

		// check for fuses in inventory & store results bool in array
		if (cc.GetInventory.HasItem(triangleFuse))
		{
			fuseTruthCollection[0] = true;
		}

		if (cc.GetInventory.HasItem(squareFuse))
		{
			fuseTruthCollection[1] = true;
		}

		if (cc.GetInventory.HasItem(circleFuse))
		{
			fuseTruthCollection[2] = true;
		}

		if (!state.opened)
		{
			animator.SetTrigger("Open");
			hasInteracted = false;
			state.opened = true;
			state.interacted = true;

			Interacted?.Invoke(this, state);

			// if no fuses - set prompt message as "No fuses"
			if (!fuseTruthCollection[0] && !fuseTruthCollection[1] && !fuseTruthCollection[2])
			{
				promptMessage = "No Fuses";
				promptBox.Show(promptMessage);
				return;
			}

			// if fuse - prompt to insert (just do all of them for now)=
			foreach (var fuseTruth in fuseTruthCollection)
			{
				if (fuseTruth == false)
				{
					continue;
				}

				promptMessage = "Insert Fuses - RMB";
				promptBox.Show(promptMessage);
			}

			return;
		}

		Debug.Log("BreakerInteraction");
		ItemData[] fuseDatas = { triangleFuse, squareFuse, circleFuse };
		for (var i = 0; i < fuseTruthCollection.Length; ++i)
		{
			if (fuseTruthCollection[i] == false)
			{
				continue;
			}

			cc.GetInventory.UseItem(fuseDatas[i]);
			Debug.Log("Fuse Has Been Used");
			activeFuses[i].SetActive(true);
			usedFuseCount++;
			if (fuseDatas[i] == triangleFuse)
			{
				state.usedFuseTriangle = true;
			}

			if (fuseDatas[i] == squareFuse)
			{
				state.usedFuseSquare = true;
			}

			if (fuseDatas[i] == circleFuse)
			{
				state.usedFuseCircle = true;
			}
		}

		if (usedFuseCount < 3)
		{
			hasInteracted = false;
			state.interacted = false;
		}
		else
		{
			hasInteracted = true;
			DisablePrompt();
		}
		cc.PlayerData.breakerFixed = IsBreakerSatisfied();
		Interacted?.Invoke(this, state);
	}

	public override void SetInteractedState(object savedState)
	{
		animator.SetTrigger("Open");
		base.SetInteractedState(state);
		state = savedState as BreakerState;
		foreach (var fuse in activeFuses)
		{
			var fs = fuse.GetComponent<Fuse>();
			if (fs._fuseType == FuseType.TRIANGLE)
			{
				fuse.SetActive(state.usedFuseTriangle);
			}

			if (fs._fuseType == FuseType.SQUARE)
			{
				fuse.SetActive(state.usedFuseSquare);
			}

			if (fs._fuseType == FuseType.CIRCLE)
			{
				fuse.SetActive(state.usedFuseCircle);
			}
		}

		canInteract = !IsBreakerSatisfied();
	}

	bool IsBreakerSatisfied()
	{
		return state.usedFuseCircle && state.usedFuseSquare && state.usedFuseTriangle;
	}
}