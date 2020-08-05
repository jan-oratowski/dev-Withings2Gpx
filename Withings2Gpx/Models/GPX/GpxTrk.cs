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
    public class GpxTrk {
    
        private string _srcField;
    
        private string _typeField;
    
        private GpxTrkTrksegTrkpt[][] _trksegField;
    
        public string Src {
            get => _srcField;
            set => _srcField = value;
        }
    
        public string Type {
            get => _typeField;
            set => _typeField = value;
        }
    
        [XmlArrayItem("trkpt", typeof(GpxTrkTrksegTrkpt), IsNullable=false)]
        public GpxTrkTrksegTrkpt[][] Trkseg {
            get => _trksegField;
            set => _trksegField = value;
        }
    }
}