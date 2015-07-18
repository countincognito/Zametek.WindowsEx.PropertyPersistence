using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Zametek.WindowsEx.PropertyPersistence
{
    public static class GenericPropertyStateHelper<TState, TElement, TProperty>
        where TState : IAmState<TElement>, new()
        where TElement : IAmElement<TProperty>, new()
        where TProperty : IAmProperty, new()
    {
        #region Fields

        private static readonly Dictionary<string, AbstractPropertyState<TState, TElement, TProperty>> s_PropertyStates =
            new Dictionary<string, AbstractPropertyState<TState, TElement, TProperty>>();

        private static readonly DependencyProperty s_LoadedHandlersProperty =
           DependencyProperty.RegisterAttached("LoadedHandlers", typeof(Dictionary<string, RoutedEventHandler>), typeof(AbstractPropertyState<TState, TElement, TProperty>));

        private static Func<DependencyObject, AbstractPropertyState<TState, TElement, TProperty>> s_AddPropertyState;

        #endregion

        #region Public Static Methods

        public static void Load(
            IAccessStateResource<TState> stateResourceAccess,
            Func<DependencyObject, AbstractPropertyState<TState, TElement, TProperty>> addPropertyState)
        {
            if (stateResourceAccess == null)
            {
                throw new ArgumentNullException("stateResourceAccess");
            }
            if (addPropertyState == null)
            {
                throw new ArgumentNullException("addPropertyState");
            }
            AbstractPropertyState<TState, TElement, TProperty>.Persistence.Load(stateResourceAccess);
            s_AddPropertyState = addPropertyState;
        }

        public static void Save(IAccessStateResource<TState> stateResourceAccess)
        {
            if (stateResourceAccess == null)
            {
                throw new ArgumentNullException("stateResourceAccess");
            }
            AbstractPropertyState<TState, TElement, TProperty>.Persistence.Save(stateResourceAccess);
        }

        #endregion

        #region Internal Static Methods

        internal static object ProvideValue(DependencyObject target, DependencyProperty property, string defaultString, BindingBase xamlBinding)
        {
            if (target == null
                || property == null)
            {
                return null;
            }
            if (string.IsNullOrEmpty(defaultString))
            {
                throw new InvalidOperationException(string.Format("No default value provided for property {0}.{1}", target, property.Name));
            }
            var element = target as FrameworkElement;
            FrameworkElement visualAnchor = AbstractPropertyState<TState, TElement, TProperty>.GetVisualAnchor(target);
            if (element == null || visualAnchor != null)
            {
                element = visualAnchor;
            }

            object defaultValue = AbstractPropertyState<TState, TElement, TProperty>.ConvertFromString(target.GetType(), property, defaultString);
            var propertyMultiValueConverter = new PropertyMultiValueConverter()
            {
                Target = element,
                Property = property,
            };
            RoutedEventHandler handler = null;
            handler = (s, e) =>
            {
                if (element != null)
                {
                    element.Loaded -= handler;
                }

                if (xamlBinding != null
                   && !IsPropertyPersisted(target, property))
                {
                    propertyMultiValueConverter.ReadStateFirst = false;
                }
                if (!HasPropertyValue(target, property))
                {
                    AddPropertyValue(target, property, defaultValue);
                }

                var multiBinding = new MultiBinding()
                {
                    Converter = propertyMultiValueConverter,
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                };
                multiBinding.Bindings.Add(CreateStateBinding(target, property));
                if (xamlBinding != null)
                {
                    multiBinding.Bindings.Add(xamlBinding);
                }
                BindingOperations.SetBinding(target, property, multiBinding);

                if (element.IsLoaded)
                {
                    propertyMultiValueConverter.ReadStateFirst = false;
                }
            };

            if (target is Window)
            {
                handler(null, null);
            }
            AddPropertyLoadedHandler(target, property, handler);
            if (element != null)
            {
                element.Loaded += handler;
            }
            if (HasPropertyValue(target, property))
            {
                return GetPropertyValue(target, property);
            }
            return defaultValue;
        }

        internal static string GetUidWithNamespace(DependencyObject element)
        {
            return AbstractPropertyState<TState, TElement, TProperty>.GetNamespace(element)
                + AbstractPropertyState<TState, TElement, TProperty>.GetUid(element);
        }

        #endregion

        #region Private Static Methods

        private static bool HasPropertyValue(DependencyObject element, DependencyProperty property)
        {
            string uidWithNamespace = GetUidWithNamespace(element);
            AbstractPropertyState<TState, TElement, TProperty> state;
            if (string.IsNullOrEmpty(uidWithNamespace)
               || !s_PropertyStates.TryGetValue(uidWithNamespace, out state))
            {
                return false;
            }
            return state.HasValue(property);
        }

        private static object GetPropertyValue(DependencyObject element, DependencyProperty property)
        {
            string uidWithNamespace = GetUidWithNamespace(element);
            AbstractPropertyState<TState, TElement, TProperty> state;
            if (string.IsNullOrEmpty(uidWithNamespace)
               || !s_PropertyStates.TryGetValue(uidWithNamespace, out state))
            {
                throw new InvalidOperationException(string.Format("The property {0}.{1} is not in state", element, property.Name));
            }
            else if (!state.HasValue(property))
            {
                throw new InvalidOperationException(string.Format("The property {0}.{1} has no value", element, property.Name));
            }
            return state.GetValue(property);
        }

        private static object AddPropertyValue(DependencyObject element, DependencyProperty property, object value)
        {
            string uidWithNamespace = GetUidWithNamespace(element);
            if (string.IsNullOrEmpty(uidWithNamespace))
            {
                return null;
            }
            AbstractPropertyState<TState, TElement, TProperty> state;
            if (!s_PropertyStates.TryGetValue(uidWithNamespace, out state))
            {
                state = s_AddPropertyState(element);
                s_PropertyStates.Add(uidWithNamespace, state);
            }
            return state.AddValue(property, value);
        }

        private static void UpdatePropertyValue(DependencyObject element, DependencyProperty property, object value)
        {
            string uidWithNamespace = GetUidWithNamespace(element);
            AbstractPropertyState<TState, TElement, TProperty> state;
            if (string.IsNullOrEmpty(uidWithNamespace)
               || !s_PropertyStates.TryGetValue(uidWithNamespace, out state))
            {
                return;
            }
            state.UpdateValue(property, value);
        }

        private static void AddPropertyLoadedHandler(DependencyObject element, DependencyProperty property, RoutedEventHandler value)
        {
            Dictionary<string, RoutedEventHandler> dictionary = GetPrivateLoadedHandlers(element);
            if (dictionary == null)
            {
                dictionary = new Dictionary<string, RoutedEventHandler>();
                SetPrivateLoadedHandlers(element, dictionary);
            }
            dictionary.Add(property.Name, value);
        }

        private static bool IsPropertyPersisted(DependencyObject element, DependencyProperty property)
        {
            string uidWithNamespace = GetUidWithNamespace(element);
            return AbstractPropertyState<TState, TElement, TProperty>.Persistence.Contains(uidWithNamespace, property.Name);
        }

        private static void SetPrivateLoadedHandlers(DependencyObject element, Dictionary<string, RoutedEventHandler> value)
        {
            element.SetValue(s_LoadedHandlersProperty, value);
        }

        private static Dictionary<string, RoutedEventHandler> GetPrivateLoadedHandlers(DependencyObject element)
        {
            return (Dictionary<string, RoutedEventHandler>)element.GetValue(s_LoadedHandlersProperty);
        }

        private static BindingBase CreateStateBinding(DependencyObject element, DependencyProperty property)
        {
            string uidWithNamespace = GetUidWithNamespace(element);
            var output = new Binding()
            {
                Mode = BindingMode.TwoWay,
                Converter = new PropertyValueConverter()
                {
                    Target = element,
                    Property = property,
                },
                Source = s_PropertyStates,                                         // Not strictly necessary.
                Path = new PropertyPath(string.Format("[{0}]", uidWithNamespace)), // Not strictly necessary.
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            };
            return output;
        }

        #endregion

        #region Private Classes

        private class PropertyValueConverter
           : IValueConverter
        {
            public DependencyObject Target
            {
                get;
                set;
            }

            public DependencyProperty Property
            {
                get;
                set;
            }

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (HasPropertyValue(Target, Property))
                {
                    return GetPropertyValue(Target, Property);
                }
                BindingOperations.ClearBinding(Target, Property);
                return Binding.DoNothing;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                UpdatePropertyValue(Target, Property, value);
                return Binding.DoNothing;
            }
        }

        private class PropertyMultiValueConverter
           : IMultiValueConverter
        {
            public PropertyMultiValueConverter()
            {
                ReadStateFirst = true;
            }

            public bool ReadStateFirst
            {
                get;
                set;
            }

            public DependencyObject Target
            {
                get;
                set;
            }

            public DependencyProperty Property
            {
                get;
                set;
            }

            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                if (values.Length == 1)
                {
                    return values[0];
                }
                if (values.Length == 2)
                {
                    if (ReadStateFirst)
                    {
                        return values[0];
                    }
                    UpdatePropertyValue(Target, Property, values[1]);
                    return values[1];
                }
                throw new InvalidOperationException();
            }

            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            {
                return new object[] { value, value };
            }
        }

        #endregion
    }
}
