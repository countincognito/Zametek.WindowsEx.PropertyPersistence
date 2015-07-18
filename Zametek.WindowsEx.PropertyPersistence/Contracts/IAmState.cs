using System.Collections.Generic;

namespace Zametek.WindowsEx.PropertyPersistence
{
    public interface IAmState<TElement>
    {
        List<TElement> Elements
        {
            get;
        }
    }
}
