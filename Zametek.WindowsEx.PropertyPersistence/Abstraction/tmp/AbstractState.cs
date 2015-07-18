using System.Collections.Generic;

namespace Zametek.WindowsEx.PropertyPersistence
{
    public abstract class AbstractState<TElement>
        where TElement : AbstractElement<IAmProperty>
        //where TProperty : new()//AbstractProperty
    {
        #region Ctors

        public AbstractState()
        {
            Elements = new List<TElement>();
        }

        #endregion

        #region Properties

        public List<TElement> Elements
        {
            get;
            private set;
        }

        #endregion
    }
}
