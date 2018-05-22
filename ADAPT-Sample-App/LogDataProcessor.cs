using System.Collections.Generic;
using System.Linq;
using AgGateway.ADAPT.ApplicationDataModel.ADM;
using AgGateway.ADAPT.ApplicationDataModel.Equipment;
using AgGateway.ADAPT.ApplicationDataModel.LoggedData;
using AgGateway.ADAPT.ApplicationDataModel.Shapes;
using AgGateway.ADAPT.ISOv4Plugin.ExtensionMethods;

namespace ADAPT_Sample_App
{
    public class LogDataProcessor
    {
        /*
         * Note the use of foreach loops here. The unerlying IEnumerable's often use deferred execution to reduce memory usage.
         * This means anything that causes multiple iterations of an IEnumerable will have a severe performance impact.
         * For instance, adaptDataModel.Documents.LoggedData.Count() will be extremely slow. Additionally, loading everything into
         * memory at once using adaptDataModel.Documents.LoggedData.ToList() or similar will consume a very large amount of memory.
         */
        public void Process(ApplicationDataModel adaptDataModel)
        {
            //An ADAPT LoggedData is roughly analogous to an EIC LogStream
            foreach (var loggedData in adaptDataModel.Documents.LoggedData)
            {
                //An ADAPT OperationData is roughly analogous to an EIC LogBlock
                foreach (var operationData in loggedData.OperationData)
                {
                    ProcessEquipmentDefinitionForOperation(operationData, adaptDataModel.Catalog);
                    ProcessOperationData(operationData, adaptDataModel.Catalog);
                }

                //This is similar to EIC's stream.Release() method that releases underlying resources from memory.
                //This is the only place in ADAPT where this concept exists.
                loggedData.ReleaseSpatialData.Invoke();
            }
        }

        private void ProcessEquipmentDefinitionForOperation(OperationData operationData, Catalog catalog)
        {
            var equipmentConfigurations = catalog.EquipmentConfigurations.Where(config => 
                                                  operationData.EquipmentConfigurationIds.Contains(config.Id.ReferenceId));
            foreach (var config in equipmentConfigurations)
            {
                var connector = catalog.Connectors.Single(c => c.Id.ReferenceId == config.Connector1Id);
                var deviceElementConfig = catalog.DeviceElementConfigurations.Single(c => c.Id.ReferenceId == connector.DeviceElementConfigurationId);
                //Once we have the DeviceElementConfiguration's, we can start to store equipment offsets. 
                if (deviceElementConfig is MachineConfiguration machineConfiguration)
                {
                    //Not a typo: The X-axis is inline and the Y-axis is lateral. This is consistent with ISO offsets.
                    var gpsReceiverLateralOffset = machineConfiguration.GpsReceiverYOffset;
                    var gpsReceiverInlineOffset = machineConfiguration.GpsReceiverXOffset;
                    var gpsReceiverVerticalOffset = machineConfiguration.GpsReceiverZOffset;
                }
                if (deviceElementConfig is ImplementConfiguration implementConfiguration)
                {
                    var controlPoint = implementConfiguration.ControlPoint;
                    var implementWidth = implementConfiguration.Width;
                }
            }
        }

        private static void ProcessOperationData(OperationData operationData, Catalog catalog)
        {
            var sections = ProcessImplementSections(operationData, catalog);

            //Each SpatialRecord is equivalent to one row from an EIC LogBlock's SpatialLayer. 
            foreach (var spatialRecord in operationData.GetSpatialRecords.Invoke())
            {
                var point = spatialRecord.Geometry as Point;
                //The SpatialRecord has a value for each available meter (called WorkingData in ADAPT).
                //Note that section status is treated as a WorkingData. Each controllable section will have a meter with representation of SectionStatus.
                foreach (var section in sections)
                {
                    var metersForSection = section.GetWorkingDatas.Invoke();
                    foreach (var meter in metersForSection)
                    {
                        var meterValue = spatialRecord.GetMeterValue(meter);
                    }
                }
            }
        }

        /*
         *  Implement sections (called DeviceElementUses) are hierarchical. A section at depth 0 represents the entire width of the implement.
         *  A greater depth indicates a more granular division of the implement. Deere's plugins interpret this as:
         *   0: Master level. This represents the entire implement.
         *   1: Meter level. This usually represents one half of the implement.
         *   2: Section level. This represents one controllable section of the implement.
         *   3: Row level. This represents a single row.
         *   Not every level will be present in every datacard. For example, you may have Master level and Section level data but no Meter level data.
         */
        private static List<DeviceElementUse> ProcessImplementSections(OperationData operationData, Catalog catalog)
        {
            var maxImplementSectionDepth = operationData.MaxDepth;
            for (int i = 0; i <= maxImplementSectionDepth; ++i)
            {
                var sectionsAtThisDepth = operationData.GetDeviceElementUses.Invoke(i);
                foreach (var section in sectionsAtThisDepth)
                {
                    //The order indicates the section's relative position on the implement. 0 is the left-most section, 1 is next to it, etc. 
                    var sectionOrder = section.Order;
                    var equipmentConfiguration = catalog.DeviceElementConfigurations.Single(config => config.Id.ReferenceId == section.DeviceConfigurationId);
                    var sectionConfiguration = equipmentConfiguration as SectionConfiguration;
                    var sectionWidth = sectionConfiguration.SectionWidth;
                    var lateralOffset = sectionConfiguration.LateralOffset;
                    var inlineOffset = sectionConfiguration.InlineOffset;
                }
            }

            //If you don't care about any of this, you can get all the sections at once and ignore the hierarchy:
            return operationData.GetAllSections();
        }
    }
}
