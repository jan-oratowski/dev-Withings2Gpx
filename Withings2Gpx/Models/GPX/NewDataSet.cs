using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Withings2Gpx.Models.GPX
{
    [GeneratedCode("xsd", "4.8.3928.0")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(AnonymousType=true, Namespace="http://www.topografix.com/GPX/1/1")]
    [XmlRoot(Namespace="http://www.topografix.com/GPX/1/1", IsNullable=false)]
    public class NewDataSet {
    
        private Gpx[] _itemsField;
    
        [XmlElement("gpx")]
        public Gpx[] Items {
            get => _itemsField;
            set => _itemsField = value;
        }
    }
}