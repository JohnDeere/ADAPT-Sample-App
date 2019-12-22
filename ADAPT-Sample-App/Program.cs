using System;
using System.IO;
using AgGateway.ADAPT.PluginManager;

namespace ADAPT_Sample_App
{
    public class Program
    {
        private static Guid _applicationId = Guid.Empty;

        public static void Main(string[] args)
        {
            //This returns the \bin directory. 
            //ADAPT plugins do not need to be located in the application directory.
            //If you place the plugins in a separate folder, this variable should contain the path to that folder.
            var pluginLocation = AppDomain.CurrentDomain.BaseDirectory;
            var pluginManager = new PluginFactory(pluginLocation);

            //When you license the John Deere ADAPT plugins, you will receive a license file and an application id.
            //Initializing the plugin with your application id activates the license.
            //Keep the application id in a secure place. It is associated with your company.
            foreach (var pluginName in pluginManager.AvailablePlugins)
            {
                var plugin = pluginManager.GetPlugin(pluginName);
                plugin.Initialize(_applicationId.ToString());
            }

            //If you want to process documentation data (ie, sensor data that was collected by a machine in the field):
            LoadDocumentationDataFromSomeDatacard(pluginManager);

            //If you want to create a setup file that can be sent to a machine:
            CreateSetupFile(pluginManager);
        }

        private static void LoadDocumentationDataFromSomeDatacard(PluginFactory pluginManager)
        {
            var datacardLocation = SampleData.GetAdmDatacard();

            //The plugin factory automatically detects which plugin is able to load data from the given directory.
            //If the directory contains data in multiple formats (for example, ISOXml and 2630 data), this will return both plugins.
            //In that case, the ISO plugin would read the ISO data and the 2630 plugin would read the 2630 data.
            var supportedPlugins = pluginManager.GetSupportedPlugins(datacardLocation);
            foreach (var plugin in supportedPlugins)
            {
                var adaptDataModels = plugin.Import(datacardLocation);
                new AdaptDataModelProcessor().Process(adaptDataModels);
            }
        }

        private static void CreateSetupFile(PluginFactory pluginManager)
        {
            var setupDataModel = new SetupDataCreator().PopulateDataModel();
            
            //You can export setup data through any plugin other than the Deere Gen4 plugin.
            //Currently the Gen4 displays use a 2630 setup model - use the 2630 plugin to send setup data to a Gen4 display.
            var plugin = pluginManager.GetPlugin(PluginNames.Deere2630);
            if (plugin == null)
            {
                //In this case, the desired plugin failed to load. 
                //If you have not yet licensed and received a copy of the Deere plugins, that could be why - they are not distributed with this sample code.
            }
            else
            {
                var exportDirectory = Path.Combine("C:", "temp", "ADAPT_Export_Directory");
                plugin.Export(setupDataModel, exportDirectory.ToString());
            }
        }
    }
}
