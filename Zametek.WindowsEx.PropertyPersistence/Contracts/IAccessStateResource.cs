
namespace Zametek.WindowsEx.PropertyPersistence
{
    public interface IAccessStateResource<TState>
    {
        TState Load();
        void Save(TState state);
    }
}
