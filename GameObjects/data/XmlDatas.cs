using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Net;
using GameObjects.data;

public class XmlDatas //NOTE: The new xml system MAY cause memory leaks or longer waiting times because each file needs to be called instead of the old system where only a few lines were needed to call the dat files
{                     //NOTE: Just looked at it and the new system sucks up more memory at the cost of making your server run smoother.
    const int XML_COUNT = 59; //This number needs to be EXACT of how many xml files are in data

    public static bool behaviors = true;

    static string ds = Path.DirectorySeparatorChar.ToString();
    
    static XmlDatas()
    {
        ReadXmls();
        Mods.InitMods(behaviors);
    }

    public static void DoSomething()
    { }

    public static void ReadXmls()
    {
        TypeToId = new Dictionary<short, string>();
        IdToType = new Dictionary<string, short>();
        IdToDungeon = new Dictionary<short, string>();
        KeyPrices = new Dictionary<short, int>();
        TypeToElement = new Dictionary<short, XElement>();
        TileDescs = new Dictionary<short, TileDesc>();
        ItemDescs = new Dictionary<short, Item>();
        ObjectDescs = new Dictionary<short, ObjectDesc>();
        PortalDescs = new Dictionary<short, PortalDesc>();
        DungeonDescs = new Dictionary<string, DungeonDesc>();

        ModTextures = new Dictionary<string, byte[]>();
        ItemIds = new Dictionary<string,short>();
        UsedIds = new List<short>();

        ItemPrices = new Dictionary<short, int>();
        ItemShops = new Dictionary<int, string>();

        Keys = new List<short>();

        Stream stream; //The 5 lines below are part of the old system that read files from the dats. As you can see, reading the dats took few lines of code and was very simple, but finding the right file was tedious and I hate having to Ctrl+F all solution to find something >_> Either way, it's your choice if you want to keep the new system or release this old beast...
        //for (int i = 0; i < XML_COUNT; i++) //part of the dat**.xml system 
        //{
        //    stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.dat" + i + ".xml");
        //    ProcessXml(stream);
        //}
        //NOTE: If you're going to use the old system, delete all these lines unless you want to keep a few as a storage or backup. Be sure to remove the actual xml files from the source too.
        stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.addition2.xml"); //1
        ProcessXml(stream);
        stream.Position = 0;
        using (StreamReader rdr = new StreamReader(stream))
            ExtraXml.Add(rdr.ReadToEnd());

        stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.addition.xml"); //2
        ProcessXml(stream);
        stream.Position = 0;
        using (StreamReader rdr = new StreamReader(stream))
            ExtraXml.Add(rdr.ReadToEnd());

        stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.AbyssOfDemons.xml"); //3
         ProcessXml(stream);
         stream.Position = 0;
         using (StreamReader rdr = new StreamReader(stream))
            ExtraXml.Add(rdr.ReadToEnd());

         stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.Belladonna.xml"); //4
         ProcessXml(stream);
         stream.Position = 0;
         using (StreamReader rdr = new StreamReader(stream))
             ExtraXml.Add(rdr.ReadToEnd());

         stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.CandyLand.xml"); //5
        ProcessXml(stream);
        stream.Position = 0;
        using (StreamReader rdr = new StreamReader(stream))
            ExtraXml.Add(rdr.ReadToEnd());

        //stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.CaveOfAThousandTreasures.xml"); //6
        //ProcessXml(stream);
        //stream.Position = 0;
        //using (StreamReader rdr = new StreamReader(stream))
        //    ExtraXml.Add(rdr.ReadToEnd());

        stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.Dyes.xml"); //7
        ProcessXml(stream);
        stream.Position = 0;
        using (StreamReader rdr = new StreamReader(stream))
            ExtraXml.Add(rdr.ReadToEnd());

        stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.Encounters.xml"); //8
        ProcessXml(stream);
        stream.Position = 0;
        using (StreamReader rdr = new StreamReader(stream))
            ExtraXml.Add(rdr.ReadToEnd());

        //stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.EpicForestMaze.xml"); //9
        //ProcessXml(stream);
        //stream.Position = 0;
        //using (StreamReader rdr = new StreamReader(stream))
        //    ExtraXml.Add(rdr.ReadToEnd());

        //stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.EpicPirateCave.xml"); //10
        //ProcessXml(stream);
        //stream.Position = 0;
        //using (StreamReader rdr = new StreamReader(stream))
        //    ExtraXml.Add(rdr.ReadToEnd());

        //stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.EpicSpiderDen.xml"); //11
        //ProcessXml(stream);
        //stream.Position = 0;
        //using (StreamReader rdr = new StreamReader(stream))
        //    ExtraXml.Add(rdr.ReadToEnd());

        stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.Equip.xml"); //12
        ProcessXml(stream);
        stream.Position = 0;
        using (StreamReader rdr = new StreamReader(stream))
            ExtraXml.Add(rdr.ReadToEnd());

        //stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.EquipmentSets.xml"); //was commented //13
        //ProcessXml(stream);
        //stream.Position = 0;
        //using (StreamReader rdr = new StreamReader(stream))
        //    ExtraXml.Add(rdr.ReadToEnd());

        stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.ForbiddenJungle.xml"); //14
        ProcessXml(stream);
        stream.Position = 0;
        using (StreamReader rdr = new StreamReader(stream))
            ExtraXml.Add(rdr.ReadToEnd());

        //stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.ForestMaze.xml"); //15
        //ProcessXml(stream);
        //stream.Position = 0;
        //using (StreamReader rdr = new StreamReader(stream))
        //    ExtraXml.Add(rdr.ReadToEnd());

        stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.GhostShip.xml"); //16
        ProcessXml(stream);
        stream.Position = 0;
        using (StreamReader rdr = new StreamReader(stream))
            ExtraXml.Add(rdr.ReadToEnd());

        stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.Ground.xml"); //17
        ProcessXml(stream);
        stream.Position = 0;
        using (StreamReader rdr = new StreamReader(stream))
            ExtraXml.Add(rdr.ReadToEnd());

        //stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.HauntedCemetery.xml"); //18
        //ProcessXml(stream);
        //stream.Position = 0;
        //using (StreamReader rdr = new StreamReader(stream))
        //    ExtraXml.Add(rdr.ReadToEnd());

        stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.High.xml"); //19
        ProcessXml(stream);
        stream.Position = 0;
        using (StreamReader rdr = new StreamReader(stream))
            ExtraXml.Add(rdr.ReadToEnd());

        stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.LairOfDraconis.xml"); //20
        ProcessXml(stream);
        stream.Position = 0;
        using (StreamReader rdr = new StreamReader(stream))
            ExtraXml.Add(rdr.ReadToEnd());

        //stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.LairOfShaitan.xml"); //21
        //ProcessXml(stream);
        //stream.Position = 0;
        //using (StreamReader rdr = new StreamReader(stream))
        //    ExtraXml.Add(rdr.ReadToEnd());

        stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.Low.xml"); //22
        ProcessXml(stream);
        stream.Position = 0;
        using (StreamReader rdr = new StreamReader(stream))
            ExtraXml.Add(rdr.ReadToEnd());

        //  stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.MTesting.xml"); //was commented //23
        //ProcessXml(stream);
        //stream.Position = 0;
        //using (StreamReader rdr = new StreamReader(stream))
        //    ExtraXml.Add(rdr.ReadToEnd());

        stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.MadLab.xml"); //24
        ProcessXml(stream);
        stream.Position = 0;
        using (StreamReader rdr = new StreamReader(stream))
            ExtraXml.Add(rdr.ReadToEnd());

        stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.ManorOfTheImmortals.xml"); //25
        ProcessXml(stream);
        stream.Position = 0;
        using (StreamReader rdr = new StreamReader(stream))
            ExtraXml.Add(rdr.ReadToEnd());

        stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.Mid.xml"); //26
        ProcessXml(stream);
        stream.Position = 0;
        using (StreamReader rdr = new StreamReader(stream))
            ExtraXml.Add(rdr.ReadToEnd());

        stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.Monsters.xml"); //27
        ProcessXml(stream);
        stream.Position = 0;
        using (StreamReader rdr = new StreamReader(stream))
            ExtraXml.Add(rdr.ReadToEnd());

        stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.Mountains.xml"); //28
        ProcessXml(stream);
        stream.Position = 0;
        using (StreamReader rdr = new StreamReader(stream))
            ExtraXml.Add(rdr.ReadToEnd());

        //stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.NexusDestroyed.xml"); //29
        //ProcessXml(stream);
        //stream.Position = 0;
        //using (StreamReader rdr = new StreamReader(stream))
        //    ExtraXml.Add(rdr.ReadToEnd());

        stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.Objects.xml"); //30
        ProcessXml(stream);
        stream.Position = 0;
        using (StreamReader rdr = new StreamReader(stream))
            ExtraXml.Add(rdr.ReadToEnd());

        stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.OceanTrench.xml"); //31
        ProcessXml(stream);
        stream.Position = 0;
        using (StreamReader rdr = new StreamReader(stream))
            ExtraXml.Add(rdr.ReadToEnd());

        stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.OryxCastle.xml"); //32
        ProcessXml(stream);
        stream.Position = 0;
        using (StreamReader rdr = new StreamReader(stream))
            ExtraXml.Add(rdr.ReadToEnd());

        stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.OryxChamber.xml"); //33
        ProcessXml(stream);
        stream.Position = 0;
        using (StreamReader rdr = new StreamReader(stream))
            ExtraXml.Add(rdr.ReadToEnd());

        stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.OryxWineCellar.xml"); //34
        ProcessXml(stream);
        stream.Position = 0;
        using (StreamReader rdr = new StreamReader(stream))
            ExtraXml.Add(rdr.ReadToEnd());

        //stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.Permapets.xml"); //35
        //ProcessXml(stream);
        //stream.Position = 0;
        //using (StreamReader rdr = new StreamReader(stream))
        //    ExtraXml.Add(rdr.ReadToEnd());

        //stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.Pets.xml"); //36
        //ProcessXml(stream);
        //stream.Position = 0;
        //using (StreamReader rdr = new StreamReader(stream))
        //    ExtraXml.Add(rdr.ReadToEnd());

        stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.PirateCave.xml"); //37
        ProcessXml(stream);
        stream.Position = 0;
        using (StreamReader rdr = new StreamReader(stream))
            ExtraXml.Add(rdr.ReadToEnd());

        stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.Players.xml"); //38
        ProcessXml(stream);
        stream.Position = 0;
        using (StreamReader rdr = new StreamReader(stream))
            ExtraXml.Add(rdr.ReadToEnd());

        stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.Projectiles.xml"); //39
        ProcessXml(stream);
        stream.Position = 0;
        using (StreamReader rdr = new StreamReader(stream))
            ExtraXml.Add(rdr.ReadToEnd());

        //stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.PuppetMaster.xml"); //40
        //ProcessXml(stream);
        //stream.Position = 0;
        //using (StreamReader rdr = new StreamReader(stream))
        //    ExtraXml.Add(rdr.ReadToEnd());

        //stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.Regions.xml"); //was commented //41
        //ProcessXml(stream);
        //stream.Position = 0;
        //using (StreamReader rdr = new StreamReader(stream))
        //    ExtraXml.Add(rdr.ReadToEnd());

        //stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.STesting.xml"); //42
        //ProcessXml(stream);
        //stream.Position = 0;
        //using (StreamReader rdr = new StreamReader(stream))
        //    ExtraXml.Add(rdr.ReadToEnd());

        //stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.Shatters.xml"); //43
        //ProcessXml(stream);
        //stream.Position = 0;
        //using (StreamReader rdr = new StreamReader(stream))
        //    ExtraXml.Add(rdr.ReadToEnd());

        stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.Shore.xml"); //44
        ProcessXml(stream);
        stream.Position = 0;
        using (StreamReader rdr = new StreamReader(stream))
            ExtraXml.Add(rdr.ReadToEnd());

        //stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.Skins.xml"); //45
        //ProcessXml(stream);
        //stream.Position = 0;
        //using (StreamReader rdr = new StreamReader(stream))
        //    ExtraXml.Add(rdr.ReadToEnd());

        stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.SnakePit.xml"); //46
        ProcessXml(stream);
        stream.Position = 0;
        using (StreamReader rdr = new StreamReader(stream))
            ExtraXml.Add(rdr.ReadToEnd());

        stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.SpriteWorld.xml"); //47
        ProcessXml(stream);
        stream.Position = 0;
        using (StreamReader rdr = new StreamReader(stream))
            ExtraXml.Add(rdr.ReadToEnd());

        stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.StaticObjects.xml"); //48 
        ProcessXml(stream);
        stream.Position = 0;
        using (StreamReader rdr = new StreamReader(stream))
            ExtraXml.Add(rdr.ReadToEnd());

        //stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.TTesting.xml"); //49
        //ProcessXml(stream);
        //stream.Position = 0;
        //using (StreamReader rdr = new StreamReader(stream))
        //    ExtraXml.Add(rdr.ReadToEnd());

        stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.TempObjects.xml"); //50
        ProcessXml(stream);
        stream.Position = 0;
        using (StreamReader rdr = new StreamReader(stream))
            ExtraXml.Add(rdr.ReadToEnd());

        stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.TestingObjects.xml"); //51
        ProcessXml(stream);
        stream.Position = 0;
        using (StreamReader rdr = new StreamReader(stream))
            ExtraXml.Add(rdr.ReadToEnd());

        stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.Textiles.xml"); //52
        ProcessXml(stream);
        stream.Position = 0;
        using (StreamReader rdr = new StreamReader(stream))
            ExtraXml.Add(rdr.ReadToEnd());

        stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.TombOfTheAncients.xml"); //53
        ProcessXml(stream);
        stream.Position = 0;
        using (StreamReader rdr = new StreamReader(stream))
            ExtraXml.Add(rdr.ReadToEnd());

        stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.TutorialObjects.xml"); //54
        ProcessXml(stream);
        stream.Position = 0;
        using (StreamReader rdr = new StreamReader(stream))
            ExtraXml.Add(rdr.ReadToEnd());

        //stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.TutorialScript.xml"); //55
        //ProcessXml(stream);
        //stream.Position = 0;
        //using (StreamReader rdr = new StreamReader(stream))
        //    ExtraXml.Add(rdr.ReadToEnd());

        stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.UndeadLair.xml"); //56
        ProcessXml(stream);
        stream.Position = 0;
        using (StreamReader rdr = new StreamReader(stream))
            ExtraXml.Add(rdr.ReadToEnd());

        //stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.WillemTesting.xml"); //57
        //ProcessXml(stream);
        //stream.Position = 0;
        //using (StreamReader rdr = new StreamReader(stream))
        //    ExtraXml.Add(rdr.ReadToEnd());

        stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.Obstacles.xml"); //58
        ProcessXml(stream);
        stream.Position = 0;
        using (StreamReader rdr = new StreamReader(stream))
            ExtraXml.Add(rdr.ReadToEnd());

        //stream = typeof(XmlDatas).Assembly.GetManifestResourceStream("GameObjects.data.Particles.xml"); //was commented //59
        //ProcessXml(stream);
        //stream.Position = 0;
        //using (StreamReader rdr = new StreamReader(stream))
        //    ExtraXml.Add(rdr.ReadToEnd());
        
    }
    //XML Named System by Zeroeh (Kushala Daora)
    public static string ProcessModXml(Stream stream, string dir)
    {
        XElement root = XElement.Load(stream);
        foreach (var elem in root.Elements("Ground"))
        {
            short type = 0x3000;
            string id = elem.Attribute("id").Value;

            if (!ItemIds.ContainsKey(id))
                for (var i = 0x3001; i < 0xffff; i++)
                    if (!UsedIds.Contains((short)i))
                    {
                        ItemIds.Add(id, (short)i);
                        UsedIds.Add((short)i);
                    }
            type = ItemIds[id];

            elem.SetAttributeValue("type", type);

            TypeToId[type] = id;
            IdToType[id] = type;
            TypeToElement[type] = elem;

            TileDescs[type] = new TileDesc(elem);
        }
        foreach (var elem in root.Elements("Object"))
        {
            if (elem.Element("Class") == null) continue;
            string cls = elem.Element("Class").Value;
            short type = 0x4000;
            string id = elem.Attribute("id").Value;

            if (!ItemIds.ContainsKey(id))
                for (var i = 0x4001; i < 0xffff; i++)
                    if (!UsedIds.Contains((short)i))
                    {
                        ItemIds.Add(id, (short)i);
                        UsedIds.Add((short)i);
                        break;
                    }
            type = ItemIds[id];

            Console.Out.WriteLine("(" + new DirectoryInfo(dir).Name + ") Adding mod object: " + id + " (" + type.ToString() + ")");
            if (File.Exists(dir + ds + id + ".png"))
            {
                Console.Out.WriteLine("(" + new DirectoryInfo(dir).Name + ") Adding mod texture: " + id);

                if (elem.Element("RemoteTexture") != null)
                    elem.Element("RemoteTexture").Remove();
                if (elem.Element("Texture") != null)
                    elem.Element("Texture").Remove();

                XElement texElem = new XElement("RemoteTexture",
                    new XElement("Instance",
                        new XText("production")
                    ),
                    new XElement("Id",
                        new XText("mod:" + id)
                    )
                );

                elem.Add(texElem);

                try
                {
                    ModTextures.Add(id, File.ReadAllBytes(dir + ds + id + ".png"));
                }
                catch { Console.Out.WriteLine("(" + new DirectoryInfo(dir).Name + ") Error adding texture: " + id); }
            }

            elem.SetAttributeValue("type", type);

            TypeToId[type] = id;
            IdToType[id] = type;
            TypeToElement[type] = elem;

            if (cls == "Equipment" || cls == "Dye" || cls == "Pet")
            {
                ItemDescs[type] = new Item(elem);
                if (elem.Element("Shop") != null)
                {
                    XElement shop = elem.Element("Shop");
                    ItemShops[type] = shop.Element("Name").Value;
                    ItemPrices[type] = Utils.FromString(shop.Element("Price").Value);
                }
            }
            if (cls == "Character" || cls == "GameObject" || cls == "Wall" ||
                cls == "ConnectedWall" || cls == "CaveWall" || cls == "Portal")
                ObjectDescs[type] = new ObjectDesc(elem);
            if (cls == "Portal")
            {
                try
                {
                    PortalDescs[type] = new PortalDesc(elem);
                }
                catch //(Exception e) //memory leak / never used
                { 
                    Console.WriteLine("Error for portal: " + type + " id: " + id);
                    /*3392,1792,1795,1796,1805,1806,1810,1825 -- no location, assume nexus?* 
*  Tomb Portal of Cowardice,  Dungeon Portal,  Portal of Cowardice,  Realm Portal,  Glowing Portal of Cowardice,  Glowing Realm Portal,  Nexus Portal,  Locked Wine Cellar Portal*/
                }
            }

            XElement key = elem.Element("Key");
            if (key != null)
            {
                Keys.Add(type);
                KeyPrices[type] = Utils.FromString(key.Value);
            }
        }
        foreach (var elem in root.Elements("Dungeon"))
        {
            string name = elem.Attribute("name").Value;
            short portalid = (short)Utils.FromString(elem.Attribute("type").Value);

            IdToDungeon[portalid] = name;
            DungeonDescs[name] = new DungeonDesc(elem);
        }
        using (StringWriter sw = new StringWriter())
        {
            root.Save(sw);
            return sw.ToString();
        }
    }

    static void ProcessXml(Stream stream)
    {
        XElement root = XElement.Load(stream); //bug catch
        foreach (var elem in root.Elements("Ground"))
        {
            short type = (short)Utils.FromString(elem.Attribute("type").Value);
            string id = elem.Attribute("id").Value;

            UsedIds.Add(type);

            TypeToId[type] = id;
            IdToType[id] = type;
            TypeToElement[type] = elem;

            TileDescs[type] = new TileDesc(elem);
        }
        foreach (var elem in root.Elements("Object"))
        {
            if (elem.Element("Class") == null) continue;
            string cls = elem.Element("Class").Value;
            short type = (short)Utils.FromString(elem.Attribute("type").Value);
            string id = elem.Attribute("id").Value;

            UsedIds.Add(type);

            TypeToId[type] = id;
            IdToType[id] = type;
            TypeToElement[type] = elem;

            if (cls == "Equipment" || cls == "Dye" || cls == "Pet")
            {
                ItemDescs[type] = new Item(elem);
                if (elem.Element("Shop") != null)
                {
                    XElement shop = elem.Element("Shop");
                    ItemShops[type] = shop.Element("Name").Value;
                    ItemPrices[type] = Utils.FromString(shop.Element("Price").Value);
                }
            }
            if (cls == "Character" || cls == "GameObject" || cls == "Wall" ||
                cls == "ConnectedWall" || cls == "CaveWall" || cls == "Portal")
                ObjectDescs[type] = new ObjectDesc(elem);
            if (cls == "Portal")
            {
                try
                {
                    PortalDescs[type] = new PortalDesc(elem);
                }
                catch //(Exception e) //memory leak / never used
                {
                    Console.WriteLine("Error for portal: " + type + " id: " + id);
                    /*3392,1792,1795,1796,1805,1806,1810,1825 -- no location, assume nexus?* 
*  Tomb Portal of Cowardice,  Dungeon Portal,  Portal of Cowardice,  Realm Portal,  Glowing Portal of Cowardice,  Glowing Realm Portal,  Nexus Portal,  Locked Wine Cellar Portal*/
                }
            }

            XElement key = elem.Element("Key");
            if (key != null)
            {
                Keys.Add(type);
                KeyPrices[type] = Utils.FromString(key.Value);
            }
        }
        foreach (var elem in root.Elements("Dungeon"))
        {
            string name = elem.Attribute("name").Value;
            short portalid = (short)Utils.FromString(elem.Attribute("type").Value);

            IdToDungeon[portalid] = name;
            DungeonDescs[name] = new DungeonDesc(elem);
        }
    }
    public static Dictionary<short, string> TypeToId { get; private set; }
    public static Dictionary<string, short> IdToType { get; private set; }
    public static Dictionary<short, string> IdToDungeon { get; private set; }
    public static Dictionary<short, int> KeyPrices { get; private set; }
    public static Dictionary<short, XElement> TypeToElement { get; private set; }
    public static Dictionary<short, TileDesc> TileDescs { get; private set; }
    public static Dictionary<short, Item> ItemDescs { get; private set; }
    public static Dictionary<short, ObjectDesc> ObjectDescs { get; private set; }
    public static Dictionary<short, PortalDesc> PortalDescs { get; private set; }
    public static Dictionary<string, DungeonDesc> DungeonDescs { get; private set; }
    public static Dictionary<string, byte[]> ModTextures { get; private set; }
    public static List<short> UsedIds { get; private set; }
    public static Dictionary<string, short> ItemIds { get; private set; }
    public static Dictionary<short, int> ItemPrices;
    public static Dictionary<int, string> ItemShops;
    public static List<short> Keys;
    //XElement addition; //memory leak / never used
    public static List<string> ExtraXml = new List<string>();
}