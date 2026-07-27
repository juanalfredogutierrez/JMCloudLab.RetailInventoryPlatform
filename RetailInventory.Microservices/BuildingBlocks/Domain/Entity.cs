namespace BuildingBlocks.Domain;

public abstract class Entity
{
    public int Id { get; protected set; }
    public Guid Uid { get; private set; }

    protected Entity()
    {
        Uid = Guid.NewGuid();
    }
}