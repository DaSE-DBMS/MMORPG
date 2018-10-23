using System;
using System.IO;
using System.Xml.Serialization;
using System.Threading;
using Common;
using Backend.Network;

class Program
{
    static public int Main(String[] args)
    {
        if (args.Length != 1)
        {
            System.Console.WriteLine("backend.exe {Configure File Path}");
            return 0;
        }

        string confPath = args[0];
        XmlSerializer serializer = new XmlSerializer(typeof(BackendConf));
        StreamReader reader = new StreamReader(confPath);
        BackendConf conf = (BackendConf)serializer.Deserialize(reader);
        GameServer gs = new GameServer(conf);
        gs.StartUp();

        return 0;
    }
}
