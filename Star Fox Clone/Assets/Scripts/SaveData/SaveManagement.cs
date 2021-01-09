using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveManagement : MonoBehaviour
{
    public static void Save(object objectToSave, string fileName)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = new FileStream(fileName, FileMode.Create);
        bf.Serialize(stream, objectToSave);
        stream.Close();
    }

    public static object Load(string fileName)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = new FileStream(fileName, FileMode.Open);
        object result = bf.Deserialize(stream);
        stream.Close();

        return result;
    }
}
