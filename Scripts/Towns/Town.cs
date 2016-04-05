using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Misc;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using Server.Regions;

namespace Server.Items
{
    public class Town : Item
    {
        public virtual string TownName { get { return "Town Name"; } }
        public virtual TownIDValue TownID { get { return TownIDValue.Inactive; } }   
        public virtual IndexedRegionName RegionName { get { return IndexedRegionName.NotIndexed; } }

        public virtual int TownHue { get { return 2500; } }

        public virtual int TownIconItemId { get { return 0x13B9; } }
        public virtual int TownIconHue { get { return 0; } }

        public Region region;

        [Constructable]
        public Town(TownIDValue town, Map map): base(0x0)
        {
            Visible = false;
            Movable = false;

            //-----

            region = Region.GetRegionByIndexedRegionName(RegionName);

            Towns.TownList.Add(this);
        }

        public Town(Serial serial): base(serial)
        {
        }

        public virtual void CreateTown()
        {
            CreateVendors();
        }

        public virtual void CreateVendors()
        {
        }

        public void CreateVendor(TownIDValue town, Point3D location, Map map, TownVendorType vendorType, int vendorCount, int spawnRange, int homeRange)
        {
            XmlSpawner xmlSpawner = null;

            switch (vendorType)
            {
                case TownVendorType.Banker: xmlSpawner = new XmlSpawner(new List<string> { "Banker" }, new List<int> { vendorCount }); break;
            }

            if (xmlSpawner == null)
                return;

            xmlSpawner.MoveToWorld(location, map);
            xmlSpawner.SpawnRange = spawnRange;
            xmlSpawner.HomeRange = homeRange;
            xmlSpawner.MaxCount = vendorCount;

            xmlSpawner.SmartRespawn();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {               
            }

            //-----

            region = Region.GetRegionByIndexedRegionName(RegionName);

            Towns.TownList.Add(this);
        }
    }

    public enum TownVendorType
    {
        Banker
    }
}