namespace robhabraken.SitecoreShrink.IO
{
    using Entities;
    using System.IO;
    using System.Runtime.Serialization.Json;
    using System.Web;

    /// <summary>
    /// Generic class to store serializable objects as a JSON file on disk.
    /// </summary>
    public class JsonStorage
    {
        private string jsonFilePath;

        /// <summary>
        /// Constructs a new JSON storage object referring to the given file path.
        /// </summary>
        /// <param name="path">The file path indicating where to store the object on disk, relative to the webroot.</param>
        public JsonStorage(string path)
        {
            this.jsonFilePath = HttpRuntime.AppDomainAppPath + path;
        }
        
        /// <summary>
        /// Serializes the given object to disk in a JSON file format.
        /// </summary>
        /// <typeparam name="T">The type of the object to store.</typeparam>
        /// <param name="objectToStore">The object to store.</param>
        public void Serialize<T>(T objectToStore)
        {
            var memoryStream = new MemoryStream();
            var serializer = new DataContractJsonSerializer(typeof(T));
            serializer.WriteObject(memoryStream, objectToStore);

            memoryStream.Position = 0;
            var streamReader = new StreamReader(memoryStream);

            using (var file = new StreamWriter(this.jsonFilePath, false))
            {
                file.WriteLine(streamReader.ReadToEnd());
            }
        }

        /// <summary>
        /// Deserializes an object from disk, being stored in a JSON file format.
        /// </summary>
        /// <typeparam name="T">The type of the object to retrieve.</typeparam>
        /// <returns>The deserialized object of the given type.</returns>
        public T Deserialize<T>()
        {
            var streamReader = new StreamReader(this.jsonFilePath);            
            var serializer = new DataContractJsonSerializer(typeof(MediaItemReport));

            return (T)serializer.ReadObject(streamReader.BaseStream);
        }
    }
}
