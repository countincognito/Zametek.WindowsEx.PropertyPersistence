﻿using System.Xml.Serialization;

namespace Zametek.WindowsEx.PropertyPersistence
{
    [XmlType("property")]
    public class Property
        : IAmProperty
    {
        [XmlAttribute("name")]
        public string Name
        {
            get;
            set;
        }

        [XmlAttribute("value")]
        public string Value
        {
            get;
            set;
        }
    }
}
