using Sportsy.Data.Models;
using System;
using System.Collections.Generic;
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

            return importedActivity;
        }

        private void AddTrack(ImportedActivity importedActivity, XmlNode track)
        {
            if (track == null)
                return;

            foreach (XmlNode node in track.ChildNodes)
            {
                switch (node.Name)
                {
                    case "src":
                        importedActivity.ImportSource = DetectSource(node.InnerText);
                        break;
                    case "type":
                        importedActivity.ActivityType = DetectActivityType(node.InnerText);
                        break;
                    case "trkseg":
                        importedActivity.Points = AddPoints(node);
                        break;

                }
            }

        }

        private ICollection<ImportedPoint> AddPoints(XmlNode node)
        {
            var points = new List<ImportedPoint>();

            foreach (XmlNode pt in node.ChildNodes)
                points.Add(MapToImportedPoint(pt));

            return points;
        }

        private ImportedPoint MapToImportedPoint(XmlNode pt)
        {
            var point = new ImportedPoint();

            var lat = GetValueFromAttribute(pt, "lat");
            if (!string.IsNullOrWhiteSpace(lat))
                point.Latitude = decimal.Parse(lat);

            var lon = GetValueFromAttribute(pt, "lon");
            if (!string.IsNullOrWhiteSpace(lon))
                point.Longitude = decimal.Parse(lon);

            foreach (XmlNode node in pt.ChildNodes)
            {
                switch (node.Name)
                {
                    case "time":
                        if (DateTime.TryParse(node.Value, out var dt))
                            point.Time = dt;
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
            if (node == null || point == null)
                return;


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
                switch (node.Name)
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
    internal static class GpxParserExtensions
    {
    }
}
