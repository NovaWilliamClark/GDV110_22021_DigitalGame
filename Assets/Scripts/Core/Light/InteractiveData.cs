using UnityEngine;

[CreateAssetMenu(menuName = "Create InteractiveData", fileName = "InteractiveData", order = 0)]
public class InteractiveData : ScriptableObject
{
    public enum InteractionState
    {
        ACTIVE,
        INACTIVE
    }

    public InteractionState state;

    private void OnEnable()
    {
        state = InteractionState.ACTIVE;
    }
}