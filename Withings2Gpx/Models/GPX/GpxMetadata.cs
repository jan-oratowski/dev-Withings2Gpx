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
    public class GpxMetadata {
    
        private string _nameField;
    
        private string _timeField;
    
        private GpxMetadataLink[] _linkField;
    
        public string Name {
            get => _nameField;
            set => _nameField = value;
        }
    
        public string Time {
            get => _timeField;
            set => _timeField = value;
        }
    
        [XmlElement("link")]
        public GpxMetadataLink[] Link {
            get => _linkField;
            set => _linkField = value;
        }
    }
}