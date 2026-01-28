namespace Code.Entities
{
    public interface IEntityComponent
    {
        Entity Owner { get; }

        void InitCompo(Entity entity);
    }
}
