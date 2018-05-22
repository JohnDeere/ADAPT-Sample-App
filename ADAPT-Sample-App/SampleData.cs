using System.IO;
using System.IO.Compression;

namespace ADAPT_Sample_App
{
    public static class SampleData
    {
        /*
         * There is sample data available at https://github.com/JohnDeere/SampleData
         * This class assumes you have cloned that repository into C:\SampleData
         * In a real application, you would get data either directly from your users or through the MyJohnDeere Files API - https://developer.deere.com/#!documentation&doc=myjohndeere%2Ffiles.htm&anchor=
         */
        private static string _sampleDataLocation = @"C:\SampleData\Display data\";

        public static string GetAdmDatacard()
        {
            var zippedAdmData = Path.Combine(_sampleDataLocation, "ADAPT Data Model - 4600 Harvest.zip");
            var unzippedDataLocation = UnzipFile(zippedAdmData);

            return unzippedDataLocation;
        }

        //Note that you will need a copy of the John Deere ADAPT plugins in order to import the 2630 and 4600 data.
        //They aren't open source, but if you have a licensed copy you can drop it in the \bin directory and this sample app will automatically work for 2630 and 4600 data.
        public static string Get2630Data()
        {
            var zipped2630Data = Path.Combine(_sampleDataLocation, "GS3 - 2630 Setup Data.zip");
            var unzippedDataLocation = UnzipFile(zipped2630Data);

            return unzippedDataLocation;
        }

        public static string Get4600Data()
        {
            var zippedGen4Data = Path.Combine(_sampleDataLocation, "Gen4 - 4600 Documentation Data.zip");
            var unzippedDataLocation = UnzipFile(zippedGen4Data);

            return unzippedDataLocation;
        }

        private static string UnzipFile(string zipped2630Data)
        {
            var unzippedDataLocation = Path.Combine(_sampleDataLocation, "unzippedData");
            if (Directory.Exists(unzippedDataLocation))
                Directory.Delete(unzippedDataLocation, true);
            Directory.CreateDirectory(unzippedDataLocation);

            ZipFile.ExtractToDirectory(zipped2630Data, unzippedDataLocation);
            return unzippedDataLocation;
        }
    }
}
