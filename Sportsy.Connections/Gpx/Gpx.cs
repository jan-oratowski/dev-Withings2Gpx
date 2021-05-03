using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Sportsy.Connections.Gpx
{
    [XmlRoot("gpx")]
    public class Gpx
    {
        [XmlAttribute("creator")]
        public string Creator { get; set; }
        
        [XmlElement("metadata")]
        public Metadata Metadata { get; set; }
        
        [XmlElement("trk")]
        public Track Track { get; set; }
    }

    public class Metadata
    {
        [XmlElement("name")]
        public string Name { get; set; }
        [XmlElement("time")]
        public DateTime? Time { get; set; }
    }

    public class Track
    {
        [XmlElement("src")]
        public string Source { get; set; }

        [XmlElement("type")]
        public int Type { get; set; }

        [XmlArray]
        [XmlArrayItem(ElementName = "trkpt")]
        //[XmlElement("trkseg")]
        public List<TrackPoint> TrackPoints { get; set; }
    }

    public class TrackPoint
    {
        [XmlAttribute("lon")]
        public decimal? Longitude { get; set; }

        [XmlAttribute("lat")]
        public decimal? Latitude { get; set; }

        [XmlElement("ele")]
        public decimal? Elevation { get; set; }
        
        [XmlElement("time")]
        public DateTime Time { get; set; }

        [XmlElement("extensions")]
        public Extension Extensions { get; set; }
    }

    public class Extension
    {
        [XmlElement("gpxtpx:TrackPointExtension")]
        public TrackPointExtension TrackPointExtension { get; set; }
    }

    public class TrackPointExtension
    {
        public decimal? Temperature { get; set; }
        public int? HeartRate { get; set; }
        public int? Cadence { get; set; }
        public int? Power { get; set; }
    }

}
