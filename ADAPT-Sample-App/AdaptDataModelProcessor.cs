using System;
using System.Collections.Generic;
using System.Linq;
using AgGateway.ADAPT.ApplicationDataModel.ADM;
using AgGateway.ADAPT.ApplicationDataModel.Common;
using AgGateway.ADAPT.ApplicationDataModel.Products;

namespace ADAPT_Sample_App
{
    public class AdaptDataModelProcessor
    {
        public void Process(IList<ApplicationDataModel> adaptDataModels)
        {
            foreach (var adaptDataModel in adaptDataModels)
            {
                /*
                 * Unlike EIC, the ADAPT data model does not clearly delineate between pure setup data (aka, an EIC setup stream),
                 * and setup data that describes a field operation (aka, setup information on an ILogBlock in EIC). All setup information
                 * is located on the Catalog. Documentation data that describes a field operation will reference setup data from the Catalog.
                 */
                var setupData = adaptDataModel.Catalog;

                ReadFieldLevelInformation(setupData);
                ReadProductInformation(setupData);
                new LogDataProcessor().Process(adaptDataModel);
            }
        }

        private void ReadFieldLevelInformation(Catalog setupData)
        {
            foreach (var field in setupData.Fields)
            {
                var fieldId = FindJohnDeereGuid(field.Id);

                //Unlike the 2630 and earlier displays, Deere's Gen4 displays allow clients, farms, and fields to be nullable. 
                var associatedFarm = setupData.Farms.SingleOrDefault(farm => farm.Id.ReferenceId == field.FarmId);
                var associatedClient = setupData.Growers.SingleOrDefault(grower => grower.Id.ReferenceId == field.GrowerId);

                var fieldBoundaries = setupData.FieldBoundaries.Where(boundary => boundary.FieldId == field.Id.ReferenceId);
                if(field.ActiveBoundaryId.HasValue)
                {
                    var activeFieldBoundary = setupData.FieldBoundaries.Single(boundary => boundary.Id.ReferenceId == field.ActiveBoundaryId);
                }

                var guidanceGroupsForField = setupData.GuidanceGroups.Where(group => field.GuidanceGroupIds.Contains(group.Id.ReferenceId));
                foreach (var guidanceGroup in guidanceGroupsForField)
                {
                    var guidanceLines = setupData.GuidancePatterns.Where(pattern => guidanceGroup.GuidancePatternIds.Contains(pattern.Id.ReferenceId));
                    foreach (var line in guidanceLines)
                    {
                        var guidanceLineType = line.GuidancePatternType; 
                    }
                }
            }
        }

        private void ReadProductInformation(Catalog setupData)
        {
            foreach (var product in setupData.Products)
            {
                if (product.ProductType == ProductTypeEnum.Variety)
                {
                    var cropVariety = product as CropVarietyProduct;
                }
                if (product.ProductType == ProductTypeEnum.Mix)
                {
                    var tankMix = product as MixProduct;
                }
                if (product.ProductType == ProductTypeEnum.Chemical)
                {
                    var chemical = product as CropProtectionProduct;
                }
                if (product.ProductType == ProductTypeEnum.Fertilizer)
                {
                    var fertilizer = product as CropNutritionProduct;
                }
                var productId = FindJohnDeereGuid(product.Id);
            }
        }

        private Guid FindJohnDeereGuid(CompoundIdentifier compoundId)
        {
            //For (much) more detail on id's in ADAPT, see the data model documentat at https://aggateway.atlassian.net/wiki/spaces/ADM/pages/62849080/ADAPT+-+Compound+Identifier+Discussion
            //or the brief seminar at https://www.youtube.com/watch?v=6jscstS6xPs 
            var deereSourcedId = compoundId.UniqueIds.FirstOrDefault(id => id.Source == "http://www.deere.com");
            if (deereSourcedId != null)
                return Guid.Parse(deereSourcedId.Id);

            //If this data did not come from a Deere plugin, there still might be a valid Guid that we can use.
            var otherId = compoundId.UniqueIds.FirstOrDefault(id => id.IdType == IdTypeEnum.UUID);
            if (otherId != null)
                return Guid.Parse(otherId.Id);

            //There may still be an integer id or a URI that we can use. What to do here is up to you; for this sample, we'll just make up an id.
            return Guid.NewGuid();
        }
    }
}
