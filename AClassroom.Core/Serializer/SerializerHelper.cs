﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AClassroom.Core
{
    /// <summary>
    /// 序列化，json ,xml,binary 读写类
    /// </summary>
    public class SerializerHelper
    {
        public static void XmlWrite<T>(T t, string fileName)
        {
            using (Stream stream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                XmlSerializer xmlFormat = new XmlSerializer(typeof(T));
                xmlFormat.Serialize(stream, t);
            }
        }

        public static T XmlReader<T>(string fileName)
        {
            if (!File.Exists(fileName)) return default(T);
            using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                XmlSerializer xmlFormat = new XmlSerializer(typeof(T));
                stream.Position = 0;
                return (T)xmlFormat.Deserialize(stream);
            }
        }

        public static void JsonWrite<T>(T t, string fileName)
        {
            using (StreamWriter streamWriter = File.CreateText(fileName))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(streamWriter, t);
            }
        }

        public static T JsonReader<T>(string fileName)
        {
            if (!File.Exists(fileName)) return default(T);
            using (StreamReader streamReader = File.OpenText(fileName))
            {
                JsonSerializer serializer = new JsonSerializer();
                return (T)serializer.Deserialize(streamReader, typeof(T));
            }
        }

        public static void BinaryWrite<T>(T t, string fileName)
        {
            using (var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite))
            {
                BinaryFormatter serializer = new BinaryFormatter();
                serializer.Serialize(fileStream, t);
            }
        }

        public static T BinaryReader<T>(string fileName)
        {
            if (!File.Exists(fileName)) return default(T);
            using (var streamReader = File.OpenRead(fileName))
            {
                BinaryFormatter serializer = new BinaryFormatter();
                return (T)serializer.Deserialize(streamReader);
            }
        }


    }

}
