namespace robhabraken.SitecoreShrink.IO
{
    using Entities;
    using System.IO;
    using System.Runtime.Serialization.Json;

    public class JsonStorage
    {
        private string jsonFilePath;

        public JsonStorage(string path)
        {
            this.jsonFilePath = path;
        }

        public void Serialize(MediaItemReport mediaItemReport)
        {
            var memoryStream = new MemoryStream();
            var serializer = new DataContractJsonSerializer(typeof(MediaItemReport));
            serializer.WriteObject(memoryStream, mediaItemReport);

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

            return (MediaItemReport)serializer.ReadObject(streamReader.BaseStream);
        }
    }
}
