using System;
using System.Collections.Generic;
using AgGateway.ADAPT.ApplicationDataModel.Common;
using AgGateway.ADAPT.ApplicationDataModel.Logistics;
using AgGateway.ADAPT.ApplicationDataModel.ReferenceLayers;
using AgGateway.ADAPT.ApplicationDataModel.Shapes;

namespace ADAPT_Sample_App.SetupData
{
	public class FlagCreator
	{
		public List<ShapeReferenceLayer> CreateFlags(Field field)
		{
			return new List<ShapeReferenceLayer>
			{
				CreatePointFlag(field),
				CreateLineFlag(field),
				CreateAreaFlag(field),
			};
		}

		private ShapeReferenceLayer CreatePointFlag(Field field)
		{
			return new ShapeReferenceLayer
			{
				Description = "Point Flag",
				FieldIds = new List<int> {field.Id.ReferenceId},
				ContextItems = new List<ContextItem>
				{
					new ContextItem {Code = "NamedFlagId", Value = Guid.NewGuid().ToString()},
					new ContextItem {Code = "NamedFlagName", Value = "Sample Rock"},
					new ContextItem {Code = "FlagType", Value = "FlagType.Point"},
				},
				ShapeLookups = new List<ShapeLookup>
				{
					new ShapeLookup
					{
						Shape = new MultiPoint
						{
							Points = new List<Point>()
						}
					}
				}
			};
		}

		private ShapeReferenceLayer CreateLineFlag(Field field)
		{
			return new ShapeReferenceLayer
			{
				Description = "Line Flag",
				FieldIds = new List<int> { field.Id.ReferenceId },
				ContextItems = new List<ContextItem>
				{
					new ContextItem {Code = "NamedFlagId", Value = Guid.NewGuid().ToString()},
					new ContextItem {Code = "NamedFlagName", Value = "Sample Waterline Marker"},
					new ContextItem {Code = "FlagType", Value = "FlagType.Line"},
				},
				ShapeLookups = new List<ShapeLookup>
				{
					new ShapeLookup
					{
						Shape = new MultiPoint
						{
							Points = new List<Point>()
						}
					}
				}
			};
		}

		private ShapeReferenceLayer CreateAreaFlag(Field field)
		{
			return new ShapeReferenceLayer
			{
				Description = "Area Flag",
				FieldIds = new List<int> { field.Id.ReferenceId },
				ContextItems = new List<ContextItem>
				{
					new ContextItem {Code = "NamedFlagId", Value = Guid.NewGuid().ToString()},
					new ContextItem {Code = "NamedFlagName", Value = "SampleDitch"},
					new ContextItem {Code = "FlagType", Value = "FlagType.Area"},
				},
				BoundingPolygon = new Polygon
				{
					ExteriorRing = new LinearRing
					{
						Points = new List<Point>
						{
							new Point{}
						}
					}
				},
				ShapeLookups = new List<ShapeLookup>
				{
					new ShapeLookup
					{
						Shape = new MultiPolygon
						{
							Polygons = new List<Polygon>
							{
								new Polygon
								{
									ExteriorRing = new LinearRing
									{

									}
								}
							}
						}
					}
				}
			};
		}
	}
}
