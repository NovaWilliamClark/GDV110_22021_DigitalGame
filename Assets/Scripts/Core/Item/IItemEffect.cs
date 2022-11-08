public abstract class ItemEffect
{
    public readonly ItemUseEvent itemUseEvent;

    protected ItemEffect(ItemUseEvent useEvent = null)
    {
        itemUseEvent = useEvent;
    }

    public virtual void Use()
    {
        if (itemUseEvent)
            itemUseEvent.UsedEvent?.Invoke();
    }
}