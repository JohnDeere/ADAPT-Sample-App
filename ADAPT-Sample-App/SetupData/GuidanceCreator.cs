using System.Collections.Generic;
using AgGateway.ADAPT.ApplicationDataModel.ADM;
using AgGateway.ADAPT.ApplicationDataModel.Guidance;
using AgGateway.ADAPT.ApplicationDataModel.Logistics;
using AgGateway.ADAPT.ApplicationDataModel.Shapes;

namespace ADAPT_Sample_App.SetupData
{
    public class GuidanceCreator
    {
        public void AddGuidanceLines(Field field, Catalog adaptCatalog)
        {
            CreateSampleAbLine(field, adaptCatalog);
        }

        private static void CreateSampleAbLine(Field field, Catalog adaptCatalog)
        {
            var abLine = new AbLine()
            {
                Description = "AB Line Name",
                A = new Point(), // First spatial coordinate goes here
                B = new Point(), // Last spatial coordinate goes here
                Heading = null, // If no heading is specified, the plugin will calculate the spatial heading between points A and B
                // The GpsSourceType must be correct. If the original GPS signal/correction type does not match the GPS signal / correction
                // type that is in use in the field, the in-cab display should notify the operator.
                GpsSource = new GpsSource {SourceType = GpsSourceEnum.DeereSF2} 
            };
            adaptCatalog.GuidancePatterns.Add(abLine);

            // A guidance group is used to relate the guidance pattern to a specific field.
            // Deere plugins only support one guidance pattern per guidance group.
            var guidanceGroup = new GuidanceGroup();
            guidanceGroup.GuidancePatternIds.Add(abLine.Id.ReferenceId);
            adaptCatalog.GuidanceGroups.Add(guidanceGroup);

            field.GuidanceGroupIds.Add(guidanceGroup.Id.ReferenceId);
        }

        private static void CreateSampleAbCurve(Field field, Catalog adaptCatalog)
        {
            var abCurve = new AbCurve
            {
                Description = "AB Curve Name",
                NumberOfSegments = 1, // Should match the actual number of segments in the spatial layer
                // The GpsSourceType must be correct. If the original GPS signal/correction type does not match the GPS signal / correction
                // type that is in use in the field, the in-cab display should notify the operator.
                GpsSource = new GpsSource {SourceType = GpsSourceEnum.DeereSF2},
                // Note the left and right projections may not be omitted. However, projections may contain an empty LineString.
                // Empty projections will be automatically generated on the display based on the implement swath width.
                // Track 0 may not be omitted or empty.
                Shape = new List<LineString>
                {
                    new LineString()
                    {
                        Id = 0, // An id of 0 (or any value where (Id % 3) == 0) indicates a left projection
                        Points = new List<Point>()
                    },
                    new LineString
                    {
                        Id = 1, // An id of 1 (or any value where (Id % 3) == 1) indicates a right projection
                        Points = new List<Point>() 
                    },
                    new LineString
                    {
                        Id = 1, // An id of 2 (or any value where (Id % 3) == 2) indicates this is Track 0.
                                // Having multiple "Track 0" LineString entries can represent a disjoint ABCurve - i.e., a curve with a gap in it.
                        Points = new List<Point>() // Spatial coordinates go here
                    }
                }
            };
            adaptCatalog.GuidancePatterns.Add(abCurve);
            
            var guidanceGroup = new GuidanceGroup();
            guidanceGroup.GuidancePatternIds.Add(abCurve.Id.ReferenceId);
            adaptCatalog.GuidanceGroups.Add(guidanceGroup);
            
            field.GuidanceGroupIds.Add(guidanceGroup.Id.ReferenceId);
        }
    }
}