using System.Windows;

namespace Zametek.WindowsEx.PropertyPersistence.Xml
{
    public class PropertyState
        : AbstractPropertyState<State, Element, Property>
    {
        internal PropertyState(DependencyObject element)
            : base(element)
        {
        }
    }
}
