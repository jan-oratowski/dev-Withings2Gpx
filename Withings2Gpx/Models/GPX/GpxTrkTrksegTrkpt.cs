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
    public class GpxTrkTrksegTrkpt {
    
        private string _eleField;
    
        private string _timeField;
    
        private GpxTrkTrksegTrkptExtensions[] _extensionsField;
    
        private string _latField;
    
        private string _lonField;
    
        public string Ele {
            get => _eleField;
            set => _eleField = value;
        }
    
        public string Time {
            get => _timeField;
            set => _timeField = value;
        }
    
        [XmlElement("extensions")]
        public GpxTrkTrksegTrkptExtensions[] Extensions {
            get => _extensionsField;
            set => _extensionsField = value;
        }
    
        [XmlAttribute]
        public string Lat {
            get => _latField;
            set => _latField = value;
        }
    
        [XmlAttribute]
        public string Lon {
            get => _lonField;
            set => _lonField = value;
        }
    }
}