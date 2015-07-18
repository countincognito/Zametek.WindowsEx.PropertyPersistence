using System.Collections.Generic;

namespace Zametek.WindowsEx.PropertyPersistence
{
    public interface IAmElement<TProperty>
    {
        List<TProperty> Properties
        {
            get;
        }

        string Uid
        {
            get;
            set;
        }
    }
}
