using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.IO;


namespace TMC
{
    public class Json
    {
        // Create a User object and serialize it to a JSON stream.  
        public static byte[] WriteClient(Gamers user)
        {
            try
            {
                // Create a stream to serialize the object to.  
                var ms = new MemoryStream();

                // Serializer the User object to the stream.  
                var ser = new DataContractJsonSerializer(typeof(Gamers));
                ser.WriteObject(ms, user);
                byte[] json = ms.ToArray();
                ms.Close();

                if (json.Length < 7000)
                {

                    byte[] jsonSize = Encoding.UTF8.GetBytes(json.Length.ToString());

                    byte[] jsonLen = new byte[6 + json.Length];

                    Array.Copy(jsonSize, 0, jsonLen, 0, jsonSize.Length);
                    Array.Copy(json, 0, jsonLen, 6, json.Length);

                    return jsonLen;
                }
                else
                    return null;

            }
            catch
            {
                return null;
            }
        }

        // Deserialize a JSON stream to a User object.  
        public static Gamers ReadClient(byte[] json)
        {
            try
            {
                string sizeJson = Encoding.UTF8.GetString(json, 0, 6);

                // Payload size
                int size = 0;
                Int32.TryParse(sizeJson, out size);

                // Create an array the size of a useful package
                byte[] jsonAfter = new byte[size];

                // Copy from the initial array to the final only the useful part
                Array.Copy(json, 6, jsonAfter, 0, size);

                string returnData = Encoding.UTF8.GetString(jsonAfter, 0, jsonAfter.Length);
                var deserializedUser = new Gamers();
                var ms = new MemoryStream(Encoding.UTF8.GetBytes(returnData));
                var ser = new DataContractJsonSerializer(deserializedUser.GetType());
                deserializedUser = ser.ReadObject(ms) as Gamers;
                ms.Close();
                return deserializedUser;
            }
            catch
            {
                return null;
            }
        }

        // Create a User object and serialize it to a JSON stream.  
        public static byte[] WriteHost(GamerList user)
        {
            try
            {
                // Create a stream to serialize the object to.  
                var ms = new MemoryStream();

                // Serializer the User object to the stream.  
                var ser = new DataContractJsonSerializer(typeof(GamerList));
                ser.WriteObject(ms, user);
                byte[] json = ms.ToArray();
                ms.Close();

                if (json.Length < 7000)
                {
                    byte[] jsonSize = Encoding.UTF8.GetBytes(json.Length.ToString());

                    byte[] jsonLen = new byte[6 + json.Length];

                    Array.Copy(jsonSize, 0, jsonLen, 0, jsonSize.Length);
                    Array.Copy(json, 0, jsonLen, 6, json.Length);

                    return jsonLen;
                }
                else

                    return null;

            }
            catch
            {
                return null;
            }
        }

        // Deserialize a JSON stream to a User object.  
        public static GamerList ReadHost(byte[] json)
        {
            try
            {

                string sizeJson = Encoding.UTF8.GetString(json, 0, 6);
                // Payload size
                int size = 0;
                Int32.TryParse(sizeJson, out size);

                // Create an array the size of a useful package
                byte[] jsonAfter = new byte[size];

                // Copy from the initial array to the final only the useful part
                Array.Copy(json, 6, jsonAfter, 0, size);

                string returnData = Encoding.UTF8.GetString(jsonAfter, 0, jsonAfter.Length);
                var deserializedUser = new GamerList();
                var ms = new MemoryStream(Encoding.UTF8.GetBytes(returnData));
                var ser = new DataContractJsonSerializer(deserializedUser.GetType());
                deserializedUser = ser.ReadObject(ms) as GamerList;
                ms.Close();
                return deserializedUser;
            }
            catch
            {
                return null;
            }
        }

    }
}
