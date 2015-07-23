using System.Collections.Generic;

namespace Zametek.WindowsEx.PropertyPersistence.Json
{
    public class State
        : IAmState<Element>
    {
        public State()
        {
            Elements = new List<Element>();
        }

        public List<Element> Elements
        {
            get;
            private set;
        }
    }
}
