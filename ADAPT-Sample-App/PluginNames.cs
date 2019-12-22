namespace ADAPT_Sample_App
{
    public class PluginNames
    {
        // Note the Gen4 plugin can read data, but will not export setup data. 
        // This is because the Gen4 does not have its own setup model. Instead, use the Deere2630 plugin to send setup data to a Gen4.
        public static string DeereGen4 = "Deere-GS4_4600";
        
        // The 2630 and 2600 plugins can import setup and documentation data, and can export setup data.
        public static string Deere2630 = "Deere-GS3_2630";
        public static string Deere2600 = "Deere-GS2_2600";
        
        // The 1800 and Gen2 Command Center displays do not support documentation. The associated plugins can import or export setup data.
        public static string Deere1800 = "Deere-GS2_1800";
        public static string DeereGen2_CommandCenter = "Deere-GS2_CommandCenter";

        // The ISO and ADM plugins are open source projects managed by AgGateway - https://github.com/ADAPT/
        public static string AgGatewayIsoXml = "ISO v4 Plugin";
        public static string AgGatewayApplicationDataModel = "ADM";
    }
}