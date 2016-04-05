using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Misc;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using Server.Items;

namespace Server
{
    public enum TownIDValue
    {
        Inactive,

        Cambria,
        Prevalia,
    }    
    
    public static class Towns
    {
        private static Type[] ActiveTownList = new Type[]
	    {
		    typeof( Cambria ),
            typeof( Prevalia )
	    };

        public static TownsPersistanceItem PersistanceItem;

        public static List<Town> TownList = new List<Town>();

        public static void Initialize()
        {
            Timer.DelayCall(TimeSpan.FromTicks(1), delegate
            {
                if (PersistanceItem == null)
                    PersistanceItem = new TownsPersistanceItem();

                CreateAuditTownInstances();
            });
        }

        public static void CreateAuditTownInstances()
        {
            for (int a = 0; a < ActiveTownList.Length; a++)
            {
                Type townType = ActiveTownList[a];

                bool foundTownInstance = false;

                foreach (Town instance in TownList)
                {
                    if (instance.GetType() == townType)
                    {
                        foundTownInstance = true;
                        break;
                    } 
                }

                if (!foundTownInstance)
                {
                    Town townInstance = (Town)Activator.CreateInstance(townType);

                    if (townInstance == null)
                    {
                        Console.Write("Unable to create Town: " + townType.ToString() + "\n");
                        continue;
                    }

                    townInstance.CreateTown();
                }
            } 
        }

        public static void CreateTown(TownIDValue town)
        {
            Town townInstance = new Town(town, Map.Felucca);

            townInstance.CreateTown();
        }

        public static Town GetTown(IndexedRegionName indexedRegionName)
        {
            Town townInstance = null;

            foreach (Town instance in TownList)
            {
                if (instance == null)
                    continue;

                if (instance.RegionName == indexedRegionName)
                    return instance;
            }

            return townInstance;
        }

        public static Town GetTown(TownIDValue townValue)
        {
            Town townInstance = null;

            foreach (Town instance in TownList)
            {
                if (instance == null)
                    continue;

                if (instance.TownID == townValue)
                    return instance;
            }

            return townInstance;
        }

        public static Town FromRegion(Region region)
        {
            foreach (Town town in TownList)
            {
                if (region.IsPartOf(town.region))
                    return town;
            }

            return null;
        }

        public static Town FromLocation(Point3D location, Map map)
        {
            Region reg = Region.Find(location, map);

            return FromRegion(reg);
        }

        public static void Serialize(GenericWriter writer)
        {
            writer.Write((int)0); //Version
        }

        public static void Deserialize(GenericReader reader)
        {
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
            }
        }
    }

    public class TownsPersistanceItem : Item
    {
        public override string DefaultName { get { return "TownPersistance"; } }

        public TownsPersistanceItem(): base(0x0)
        {
            Movable = false;
        }

        public TownsPersistanceItem(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version

            Towns.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Movable = false;

            Towns.PersistanceItem = this;
            Towns.Deserialize(reader);
        }
    } 
}