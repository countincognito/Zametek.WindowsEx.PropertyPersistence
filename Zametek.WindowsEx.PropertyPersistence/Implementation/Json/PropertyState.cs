using Newtonsoft.Json;
using System.ComponentModel;
using System.Windows;

namespace Zametek.WindowsEx.PropertyPersistence.Json
{
    public class PropertyState
        : AbstractPropertyState<State, Element, Property>
    {
        internal PropertyState(DependencyObject element)
            : base(element)
        {
        }

        #region Overrides

        protected override string Serialize(DependencyProperty property, object value)
        {
            var valueType = DependencyPropertyDescriptor.FromProperty(property, Type).PropertyType;
            return JsonConvert.SerializeObject(value, valueType, null);
        }

        protected override object Deserialize(DependencyProperty property, string stringValue)
        {
            var valueType = DependencyPropertyDescriptor.FromProperty(property, Type).PropertyType;
            return JsonConvert.DeserializeObject(stringValue, valueType);
        }

        #endregion
    }
}
