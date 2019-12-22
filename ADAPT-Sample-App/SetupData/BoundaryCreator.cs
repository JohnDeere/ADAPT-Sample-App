using System.Collections.Generic;
using AgGateway.ADAPT.ApplicationDataModel.ADM;
using AgGateway.ADAPT.ApplicationDataModel.FieldBoundaries;
using AgGateway.ADAPT.ApplicationDataModel.Logistics;
using AgGateway.ADAPT.ApplicationDataModel.Shapes;

namespace ADAPT_Sample_App.SetupData
{
    public class BoundaryCreator
    {
        public void AddBoundary(Field field, Catalog adaptCatalog)
        {
            var fieldBoundary = new FieldBoundary
            {
                FieldId = field.Id.ReferenceId, 
                Description = "Boundary Name", 
                SpatialData = new MultiPolygon()
            };
            // It is possible for a field boundary to contain multiple polygons (e.g., multiple exteriors).
            // Older Deere displays require a single exterior ring - however, given multiple exterior rings the Deere
            // plugins will automatically add land bridges as needed to ensure compatibility.
            var polygon = new Polygon
            {
                ExteriorRing = new LinearRing
                {
                    Points = new List<Point>
                    {
                        // Spatial coordinates go here. Coordinates must use a WGS84 spatial projection.
                    }
                },
            };
            fieldBoundary.SpatialData.Polygons.Add(polygon);
            var interior = new LinearRing
            {
                Id = 1,
                Points = new List<Point>
                {
                    // Spatial coordinates go here. Coordinates must use a WGS84 spatial projection.
                }
            };
            polygon.InteriorRings.Add(interior);
            // InteriorBoundaryAttributes provide metadata about an interior ring.
            var interiorAttributes = new InteriorBoundaryAttribute
            {
                Description = "Interior Name",
                // IsPassable indicates whether it is safe to drive through an interior. 
                IsPassable = false, 
                // This should match the Id value of the corresponding interior ring.
                ShapeIdRef = 1
            };
            fieldBoundary.InteriorBoundaryAttributes.Add(interiorAttributes);
            
            adaptCatalog.FieldBoundaries.Add(fieldBoundary);
        }
    }
}