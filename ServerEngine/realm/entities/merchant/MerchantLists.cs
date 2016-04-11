using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerEngine.realm.entities
{
    class MerchantLists
    {

        public static Dictionary<int, Tuple<int, CurrencyType>> prices = new Dictionary<int, Tuple<int, CurrencyType>>(){  //////////////PRICES ONLY\\\\\\\\\\\\\
                                  
/*//UT Abilities
{0x0427, new Tuple<int,CurrencyType>(4500, CurrencyType.Fame)}, //Tablet
{0xc1e, new Tuple<int,CurrencyType>(2500, CurrencyType.Fame)}, //Prot 
{0xc07, new Tuple<int,CurrencyType>(3500, CurrencyType.Fame)}, //QoT
{0xa5a, new Tuple<int,CurrencyType>(2500, CurrencyType.Fame)}, //Planewalker 
{0xc08, new Tuple<int,CurrencyType>(4500, CurrencyType.Fame)}, //Jugg
{0xc0f, new Tuple<int,CurrencyType>(3500, CurrencyType.Fame)}, //Ogmur
{0xc06, new Tuple<int,CurrencyType>(3500, CurrencyType.Fame)}, //Oreo
{0x0444, new Tuple<int,CurrencyType>(2500, CurrencyType.Fame)}, //Plague
{0xc0e, new Tuple<int,CurrencyType>(3500, CurrencyType.Fame)}, //ETorment
{0xc1c, new Tuple<int,CurrencyType>(2500, CurrencyType.Fame)}, //Cvenom 
{0xc0b, new Tuple<int,CurrencyType>(3000, CurrencyType.Fame)}, //Conflict
{0xc2a, new Tuple<int,CurrencyType>(3500, CurrencyType.Fame)}, //Ghostly
{0xc30, new Tuple<int,CurrencyType>(2500, CurrencyType.Fame)}, //Fulmi 
{0x916, new Tuple<int,CurrencyType>(3500, CurrencyType.Fame)}, //Midnight
//Draconis Armor and Shatters Rings
{0xc82, new Tuple<int,CurrencyType>(5000, CurrencyType.Fame)}, //Leaf Leather 
{0xc84, new Tuple<int,CurrencyType>(5000, CurrencyType.Fame)}, //Fire Armor
{0xc83, new Tuple<int,CurrencyType>(5000, CurrencyType.Fame)}, //Water Robe
{0x0435, new Tuple<int,CurrencyType>(5000, CurrencyType.Fame)}, //Bracer
{0x0436, new Tuple<int,CurrencyType>(5000, CurrencyType.Fame)}, //Gemstone
{0x0437, new Tuple<int,CurrencyType>(5000, CurrencyType.Fame)}, //Crown                        
//UT Weapons
{0xc02, new Tuple<int,CurrencyType>(3000, CurrencyType.Fame)}, //Doom Bow
{0xc03, new Tuple<int,CurrencyType>(2000, CurrencyType.Fame)}, //EP
{0xc05, new Tuple<int,CurrencyType>(3000, CurrencyType.Fame)}, //ASS 
{0xa03, new Tuple<int,CurrencyType>(2000, CurrencyType.Fame)}, //Csword
{0xc01, new Tuple<int,CurrencyType>(2000, CurrencyType.Fame)}, //Dblade
{0xb3f, new Tuple<int,CurrencyType>(2000, CurrencyType.Fame)}, //Cwand 
{0xc0a, new Tuple<int,CurrencyType>(3000, CurrencyType.Fame)}, //Cronus
{0xc29, new Tuple<int,CurrencyType>(2000, CurrencyType.Fame)}, //Spirit
{0xc10, new Tuple<int,CurrencyType>(2000, CurrencyType.Fame)}, //Cbow
{0xc33, new Tuple<int,CurrencyType>(2000, CurrencyType.Fame)}, //Conduct
{0x0451, new Tuple<int,CurrencyType>(5000, CurrencyType.Fame)}, //Stone Staff*/
// Effusions/Drinkables 
{0xff7, new Tuple<int,CurrencyType>(5, CurrencyType.Gold)}, // Oryx Potion
//randum stuff
{0xb3e, new Tuple<int,CurrencyType>(11500, CurrencyType.Fame)}, //Amulet of Resurrection
{0xb14, new Tuple<int,CurrencyType>(300, CurrencyType.Fame)}, //Elixir of Health 7 Dose
{0xb18, new Tuple<int,CurrencyType>(300, CurrencyType.Fame)}, //Elixir of Magic 7 Dose
{0x0432, new Tuple<int,CurrencyType>(1000, CurrencyType.Fame)}, //Pollen Powder Plus
{0x0427, new Tuple<int,CurrencyType>(1925, CurrencyType.Gold)}, //UT of the Week. Currently [Tablet of the King's Avatar]
// Clothing
{0x1007, new Tuple<int,CurrencyType>(20, CurrencyType.Fame)}, //Black
{0x1009, new Tuple<int,CurrencyType>(20, CurrencyType.Fame)}, //Blue
{0x100b, new Tuple<int,CurrencyType>(20, CurrencyType.Fame)}, //Brown
{0x1010, new Tuple<int,CurrencyType>(20, CurrencyType.Fame)}, //Coral
{0x1012, new Tuple<int,CurrencyType>(20, CurrencyType.Fame)}, //Cornsilk
{0x1015, new Tuple<int,CurrencyType>(20, CurrencyType.Fame)}, //Dark Blue
{0x101f, new Tuple<int,CurrencyType>(20, CurrencyType.Fame)}, //Dark Red
{0x1002, new Tuple<int,CurrencyType>(20, CurrencyType.Fame)}, //Aqua
{0x1004, new Tuple<int,CurrencyType>(20, CurrencyType.Fame)}, //Azure
{0x1033, new Tuple<int,CurrencyType>(20, CurrencyType.Fame)}, //Green
{0x102f, new Tuple<int,CurrencyType>(20, CurrencyType.Fame)}, //Ghost White
{0x1079, new Tuple<int,CurrencyType>(20, CurrencyType.Fame)}, //Sienna
{0x1030, new Tuple<int,CurrencyType>(20, CurrencyType.Fame)}, //Gold
//Accessory
{0x1107, new Tuple<int,CurrencyType>(20, CurrencyType.Fame)}, //Black
{0x1109, new Tuple<int,CurrencyType>(20, CurrencyType.Fame)}, //Blue
{0x110b, new Tuple<int,CurrencyType>(20, CurrencyType.Fame)}, //Brown
{0x1110, new Tuple<int,CurrencyType>(20, CurrencyType.Fame)}, //Coral
{0x1112, new Tuple<int,CurrencyType>(20, CurrencyType.Fame)}, //Cornsilk
{0x1115, new Tuple<int,CurrencyType>(20, CurrencyType.Fame)}, //Dark Blue
{0x111f, new Tuple<int,CurrencyType>(20, CurrencyType.Fame)}, //Dark Red
{0x1102, new Tuple<int,CurrencyType>(20, CurrencyType.Fame)}, //Aqua
{0x1104, new Tuple<int,CurrencyType>(20, CurrencyType.Fame)}, //Azure
{0x1133, new Tuple<int,CurrencyType>(20, CurrencyType.Fame)}, //Green
{0x112f, new Tuple<int,CurrencyType>(20, CurrencyType.Fame)}, //Ghost White
{0x1179, new Tuple<int,CurrencyType>(20, CurrencyType.Fame)}, //Sienna
{0x1130, new Tuple<int,CurrencyType>(20, CurrencyType.Fame)}, //Gold
//Top Weapons
{0xb08, new Tuple<int,CurrencyType>(900, CurrencyType.Gold)}, //Staff of the Cosmic Whole
{0xb0b, new Tuple<int,CurrencyType>(900, CurrencyType.Gold)}, //Sword of Acclaim
{0xaf6, new Tuple<int,CurrencyType>(550, CurrencyType.Gold)}, //Wand of Recompense
{0xb02, new Tuple<int,CurrencyType>(600, CurrencyType.Gold)}, //Bow of Covert Havens
{0xaff, new Tuple<int,CurrencyType>(650, CurrencyType.Gold)}, //Dagger of Foul Malevolence
{0x2587, new Tuple<int,CurrencyType>(700, CurrencyType.Gold)}, //Masamune
//Top Abilities
{0xb24, new Tuple<int,CurrencyType>(400, CurrencyType.Gold)}, //Elemental Detonation Spell
{0xb22, new Tuple<int,CurrencyType>(360, CurrencyType.Gold)}, //Colossus Shield
{0xb25, new Tuple<int,CurrencyType>(400, CurrencyType.Gold)}, //Tome of Holy Guidance
{0xb28, new Tuple<int,CurrencyType>(400, CurrencyType.Gold)}, //Quiver of Elvish Mastery
{0xb27, new Tuple<int,CurrencyType>(400, CurrencyType.Gold)}, //Cloak of Ghostly Concealment
{0xc59, new Tuple<int,CurrencyType>(360, CurrencyType.Gold)}, //Doom Circle
{0xb2c, new Tuple<int,CurrencyType>(400, CurrencyType.Gold)}, //Giantcatcher Trap
{0xb29, new Tuple<int,CurrencyType>(360, CurrencyType.Gold)}, //Helm of the Great General
{0xb26, new Tuple<int,CurrencyType>(400, CurrencyType.Gold)}, //Seal of the Blessed Champion
{0xb33, new Tuple<int,CurrencyType>(360, CurrencyType.Gold)}, //Scepter of Storms
{0xb23, new Tuple<int,CurrencyType>(360, CurrencyType.Gold)}, //Prism of Apparitions
{0xb2d, new Tuple<int,CurrencyType>(360, CurrencyType.Gold)}, //Planefetter Orb
{0xb2a, new Tuple<int,CurrencyType>(400, CurrencyType.Gold)}, //Baneserpent Poison
{0xb2b, new Tuple<int,CurrencyType>(360, CurrencyType.Gold)}, //Bloodsucker Skull
//Top Armor
{0xafc, new Tuple<int,CurrencyType>(850, CurrencyType.Gold)}, //Acropolis Armor
{0xaf9, new Tuple<int,CurrencyType>(800, CurrencyType.Gold)}, //Hydra Skin Leather
{0xb05, new Tuple<int,CurrencyType>(850, CurrencyType.Gold)}, //Robe of the Grand Sorcerer
//Top Rings
{0xacd, new Tuple<int,CurrencyType>(360, CurrencyType.Gold)}, //Ring of Exalted Health
{0xace, new Tuple<int,CurrencyType>(360, CurrencyType.Gold)}, //Ring of Exalted Magic
{0xac7, new Tuple<int,CurrencyType>(200, CurrencyType.Gold)}, //Ring of Exalted Attack
{0xac8, new Tuple<int,CurrencyType>(360, CurrencyType.Gold)}, //Ring of Exalted Defense
{0xac9, new Tuple<int,CurrencyType>(180, CurrencyType.Gold)}, //Ring of Exalted Speed
{0xacc, new Tuple<int,CurrencyType>(200, CurrencyType.Gold)}, //Ring of Exalted Dexterity
{0xaca, new Tuple<int,CurrencyType>(180, CurrencyType.Gold)}, //Ring of Exalted Vitality
{0xacb, new Tuple<int,CurrencyType>(180, CurrencyType.Gold)}, //Ring of Exalted Wisdom
        };
        public static int[] store1List = {  }; //Keys
        public static int[] store2List = {  }; //Everything in here sells for 2000 fame/gold???
        public static int[] store3List = { 0xb08, 0xb0b, 0xaf6, 0xb02, 0xaff, 0x2587 }; //Top Weapons
        public static int[] store4List = { 0xafc, 0xaf9, 0xb05 }; //Top Armors
        public static int[] store5List = { 0xb24, 0xb22, 0xb25, 0xb28, 0xb27, 0xc59, 0xb2c, 0xb29, 0xb26, 0xb33, 0xb23, 0xb2d, 0xb2a, 0xb2b }; //Top Abilities
        public static int[] store6List = { 0xacd, 0xace, 0xac7, 0xac8, 0xac9, 0xacc, 0xaca, 0xacb }; //Top Rings
        public static int[] store7List = { 0x0432, 0xb18, 0xb14, 0xff7, 0xb3e };//Misc.
        public static int[] store8List = {  };// 
        public static int[] store9List = { 0x0427 };//UT of the week. Currently [Tablet of the King's Avatar]
        /*public static int[] store1List = { }; // Keys (?) added to the map
        public static int[] store2List = { }; // WARNING: Everything in this shop will sell for 0 fame or gold!
        public static int[] store3List = { 0xc02, 0xc03, 0xc05, 0xa03, 0xc01, 0xb3f, 0xc0a, 0xc29, 0xc10, 0xc33, 0x0451 }; //Top Weapons
        public static int[] store4List = { 0xc82, 0xc84, 0xc83, 0x0435, 0x0436, 0x0437 }; //Top Armors
        public static int[] store5List = { 0xb10, 0xb11, 0xb12, 0xb13, 0xaeb, 0xaec, 0xff7, 0xb3e }; //Consumables
        public static int[] store6List = { 0x0427, 0xc1e, 0xc07, 0xa5a, 0xc08, 0xc06, 0xc0f, 0x0444, 0xc0e, 0xc1c, 0xc0b, 0xc2a, 0xc30, 0x916 }; //Top Abilities
        public static int[] store7List = { 0x161f }; // Empty Possibly Valentines???
        public static int[] store8List = { 0x1007, 0x1009, 0x100b, 0x1010, 0x1012, 0x1015, 0x101f, 0x1002, 0x1004, 0x1033, 0x102f, 0x1079, 0x1030 }; // Large Dyes
        public static int[] store9List = { 0x1107, 0x1109, 0x110b, 0x1110, 0x1112, 0x1115, 0x111f, 0x1102, 0x1104, 0x1133, 0x112f, 0x1179, 0x1130, }; // Small Dyes*/




        public static Dictionary<string, int[]> shopLists = new Dictionary<string, int[]>() {           
            
        };

        /*public static void AddAdminShop()
        {
            List<int> AdminShop = new List<int>();
            for(var i = 0x1500; i < 0x1541; i++)
            {
                AdminShop.Add((int)i);
            }
            shopLists.Add("admin", AdminShop.ToArray());
            AdminShop.Shuffle();
        }
        */
        public static void GetKeys()
        {
            List<int> nkeys = new List<int>();
            foreach (var i in XmlDatas.Keys)
            {
                prices[(int)i] = new Tuple<int, CurrencyType>(XmlDatas.KeyPrices[i], CurrencyType.Fame);
                nkeys.Add((int)i);
            }
            shopLists["keys"] = nkeys.ToArray();
        }


        public static void AddCustomShops()
        {
            foreach (var i in XmlDatas.ItemPrices)
            {
                prices.Add(i.Key, new Tuple<int, CurrencyType>(i.Value, CurrencyType.Gold)); //this defines /sell prices in Prices.cs
            }
            foreach (var i in XmlDatas.ItemShops)
            {
                if (shopLists.ContainsKey(i.Value))
                {
                    List<int> ls = shopLists[i.Value].ToList();
                    ls.Add(i.Key);
                    shopLists[i.Value] = ls.ToArray();
                }
                else
                {
                    shopLists.Add(i.Value, new int[] { i.Key });
                }
            }
        }
    }
}