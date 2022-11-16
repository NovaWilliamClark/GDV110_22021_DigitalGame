using System.Collections;
using System.Collections.Generic;
using Objects;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(PersistentObject))]
public class BreakerObject : InteractionPoint
{
    [Header("Breaker")]
    [SerializeField] private List<ItemData> fusesUsed;
    [SerializeField] private ItemData triangleFuse;
    [SerializeField] private ItemData squareFuse;
    [SerializeField] private ItemData circleFuse;

    private BreakerState state;
    
    [SerializeField] private LevelCutscene breakerCutscene;

    private Animator animator;
    
    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
        persistentObject = GetComponent<PersistentObject>();
        state = new BreakerState(persistentObject.Id);
    }

    protected override void Interact(CharacterController cc)
    {
        base.Interact(cc);

        if (!state.opened)
        {
            animator.SetTrigger("Open");
            hasInteracted = false;
            state.opened = true;
            state.interacted = true;
            Interacted?.Invoke(this, state);
        }
        
        cc.PlayerData.breakerFixed = true;
    }

    public override void SetInteractedState(object savedState)
    {
        base.SetInteractedState(state);
        state = savedState as BreakerState;
        playerRef.PlayerData.breakerFixed = IsBreakerSatisfied();
        if (IsBreakerSatisfied())
        {
            hasInteracted = true;
            canInteract = false;
        }
    }

    bool IsBreakerSatisfied()
    {
        return state.usedFuseCircle && state.usedFuseSquare && state.usedFuseTriangle;
    }
}
