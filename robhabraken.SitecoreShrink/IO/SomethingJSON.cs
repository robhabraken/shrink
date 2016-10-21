namespace robhabraken.SitecoreShrink.IO
{
    using Entities;
    using System.IO;
    using System.Runtime.Serialization.Json;

    public class SomethingJSON
    {
        private string jsonFilePath;

        public SomethingJSON(string path)
        {
            this.jsonFilePath = path;
        }

        public void Serialize(MediaItemReport reportItem)
        {
            var memoryStream = new MemoryStream();
            var serializer = new DataContractJsonSerializer(typeof(MediaItemReport));
            serializer.WriteObject(memoryStream, reportItem);

            memoryStream.Position = 0;
            var streamReader = new StreamReader(memoryStream);

            using (var file = new StreamWriter(this.jsonFilePath, false))
            {
                file.WriteLine(streamReader.ReadToEnd());
            }
        }

        public MediaItemReport Deserialize()
        {
            var streamReader = new StreamReader(this.jsonFilePath);            
            var serializer = new DataContractJsonSerializer(typeof(MediaItemReport));

            var reportItem = (MediaItemReport)serializer.ReadObject(streamReader.BaseStream);

            return reportItem;
        }
    }
}
