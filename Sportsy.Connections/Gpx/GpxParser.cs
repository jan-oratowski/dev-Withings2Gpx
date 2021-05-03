using Sportsy.Data.Models;
using Sportsy.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;

// ReSharper disable PossibleMultipleEnumeration
// ReSharper disable MemberCanBeMadeStatic.Local

namespace Sportsy.Connections.Gpx
{
    public class GpxParser : IGpxParser
    {
        public ImportedActivity Parse(Stream stream)
        {
            var gpx = new XmlDocument();
            gpx.Load(stream);

            if (gpx.DocumentElement == null)
                throw new NullReferenceException("gpx.DocumentElement");

            var importedActivity = new ImportedActivity
            {
                ImportSource = ImportSourceEnum.FileGpx,
            };

            var nodes = gpx.DocumentElement.ChildNodes.Cast<XmlNode>().ToList();

            AddMetadata(importedActivity, nodes.FirstOrDefault(node => node.Name == "metadata"));
            AddTrack(importedActivity, nodes.FirstOrDefault(node => node.Name == "trk"));

            var points = importedActivity.Points.OrderBy(p => p.Time);

            if (points.First().Time.HasValue && !importedActivity.StartTime.HasValue)
                importedActivity.StartTime = points.First().Time;

            if (points.Last().Time.HasValue && !importedActivity.EndTime.HasValue)
                importedActivity.EndTime = points.Last().Time;

            return importedActivity;
        }

        private void AddTrack(ImportedActivity importedActivity, XmlNode track)
        {
            if (track == null)
                return;

            foreach (XmlNode node in track.ChildNodes)
            {
                switch (node.Name.ToLower())
                {
                    case "src":
                        importedActivity.ImportSource = DetectSource(node.InnerText);
                        break;
                    case "type":
                        importedActivity.ActivityType = DetectActivityType(node.InnerText);
                        break;
                    // ReSharper disable once StringLiteralTypo
                    case "trkseg":
                        importedActivity.Points = AddPoints(node);
                        break;
                }
            }
        }

        private ICollection<ImportedPoint> AddPoints(XmlNode node) =>
            (from XmlNode pt in node.ChildNodes
             select MapToImportedPoint(pt)
                ).ToList();

        private ImportedPoint MapToImportedPoint(XmlNode pt)
        {
            var point = new ImportedPoint();

            var lat = GetValueFromAttribute(pt, "lat");
            if (!string.IsNullOrWhiteSpace(lat))
                point.Latitude = decimal.Parse(lat, NumberStyles.Number, CultureInfo.InvariantCulture);

            var lon = GetValueFromAttribute(pt, "lon");
            if (!string.IsNullOrWhiteSpace(lon))
                point.Longitude = decimal.Parse(lon, NumberStyles.Number, CultureInfo.InvariantCulture);

            foreach (XmlNode node in pt.ChildNodes)
            {
                switch (node.Name.ToLower())
                {
                    case "time":
                        if (DateTime.TryParse(node.InnerText, out var dt))
                            point.Time = dt;
                        break;
                    case "ele":
                        if (decimal.TryParse(node.InnerText, NumberStyles.Number, CultureInfo.InvariantCulture, out var ele))
                            point.Elevation = ele;
                        break;
                    case "extensions":
                        LoadFromExtensions(point, node);
                        break;
                }
            }

            return point;
        }

        private void LoadFromExtensions(ImportedPoint point, XmlNode node)
        {
            if (node == null || point == null || !node.HasChildNodes)
                return;

            foreach (XmlNode ext in node.ChildNodes)
                foreach (XmlNode child in ext.ChildNodes)
                    switch (child.Name.ToLower())
                    { // ReSharper disable StringLiteralTypo
                        case "gpxtpx:hr":
                            point.HeartRate = int.Parse(child.InnerText);
                            break;
                        case "gpxtpx:cad":
                            point.Cadence = int.Parse(child.InnerText);
                            break;
                        case "gpxtpx:atemp":
                            point.Temperature = decimal.Parse(child.InnerText, NumberStyles.Number, CultureInfo.InvariantCulture);
                            break;
                        case "gpxpx:PowerInWatts":
                            point.Power = int.Parse(child.InnerText);
                            break;
                    } // ReSharper restore StringLiteralTypo
        }


        private string GetValueFromAttribute(XmlNode node, string attribute) =>
            node.Attributes != null &&
            node.Attributes[attribute].Specified
                ? node.Attributes[attribute].Value
                : null;

        private void AddMetadata(ImportedActivity importedActivity, XmlNode metadata)
        {
            if (metadata == null)
                return;

            foreach (XmlNode node in metadata.ChildNodes)
            {
                switch (node.Name.ToLower())
                {
                    case "name":
                        importedActivity.Name = node.InnerText;
                        break;
                    case "link":
                        importedActivity.Link = node.Attributes?["href"].Value;
                        importedActivity.LinkText = node.FirstChild.InnerText;
                        break;
                    case "time":
                        if (DateTime.TryParse(node.InnerText, out var dateTime))
                            importedActivity.StartTime = dateTime;
                        break;
                }
            }
        }

        private ImportSourceEnum DetectSource(string source)
        {
            switch (source.ToLower())
            {
                case "withings":
                    return ImportSourceEnum.FileGpxWithings;
                default:
                    return ImportSourceEnum.FileGpx;
            }
        }

        private ActivityTypeEnum DetectActivityType(string activityType)
        {
            switch (activityType.ToLower())
            {
                case "ride":
                    return ActivityTypeEnum.BikeRide;
                default:
                    return ActivityTypeEnum.Other;
            }
        }
    }
}
