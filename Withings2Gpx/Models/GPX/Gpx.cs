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
    public class Gpx {
    
        private GpxMetadata[] _metadataField;
    
        private GpxTrk[] _trkField;
    
        private string _versionField;
    
        private string _creatorField;
    
        [XmlElement("metadata")]
        public GpxMetadata[] Metadata {
            get => _metadataField;
            set => _metadataField = value;
        }
    
        [XmlElement("trk")]
        public GpxTrk[] Trk {
            get => _trkField;
            set => _trkField = value;
        }
    
        [XmlAttribute]
        public string Version {
            get => _versionField;
            set => _versionField = value;
        }
    
        [XmlAttribute]
        public string Creator {
            get => _creatorField;
            set => _creatorField = value;
        }
    }
}