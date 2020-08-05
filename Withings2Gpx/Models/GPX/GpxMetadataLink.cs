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
    public class GpxMetadataLink {
    
        private string _textField;
    
        private string _hrefField;
    
        public string Text {
            get => _textField;
            set => _textField = value;
        }

        [XmlAttribute]
        public string Href {
            get => _hrefField;
            set => _hrefField = value;
        }
    }
}