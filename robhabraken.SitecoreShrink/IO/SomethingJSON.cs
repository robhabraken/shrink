namespace robhabraken.SitecoreShrink.IO
{
    using Entities;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Runtime.Serialization.Json;

    public class SomethingJSON
    {
        public SomethingJSON()
        {

        }

        public void Serialize(MediaItemX reportItem)
        {
            var memoryStream = new MemoryStream();
            var serializer = new DataContractJsonSerializer(typeof(MediaItemX));
            serializer.WriteObject(memoryStream, reportItem);

            memoryStream.Position = 0;
            var streamReader = new StreamReader(memoryStream);

            using (var file = new System.IO.StreamWriter(@"D:\report.txt", false))
            {
                file.WriteLine(streamReader.ReadToEnd());
            }
        }
    }
}
