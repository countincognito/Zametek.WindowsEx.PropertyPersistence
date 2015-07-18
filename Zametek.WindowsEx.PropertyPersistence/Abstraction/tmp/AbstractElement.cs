using System.Collections.Generic;

namespace Zametek.WindowsEx.PropertyPersistence
{
    public abstract class AbstractElement<TProperty>
        where TProperty : IAmProperty
    {
        #region Ctors

        public AbstractElement()
        {
            Properties = new List<TProperty>();
        }

        #endregion

        #region Properties

        public List<TProperty> Properties
        {
            get;
            private set;
        }

        public string Uid
        {
            get;
            set;
        }

        #endregion
    }
}
