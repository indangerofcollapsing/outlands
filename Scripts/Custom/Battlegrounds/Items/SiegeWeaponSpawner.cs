using Server.Custom.Battlegrounds.Regions;
using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom.Battlegrounds.Items
{
    public class SiegeWeaponSpawner : XmlSpawner
    {
        private SiegeBattleground m_Battleground;
        private TimeSpan DefaultMinDelay { get { return TimeSpan.FromMinutes(3); } }
        private TimeSpan DefaultMaxDelay { get { return TimeSpan.FromMinutes(6); } }

        [Constructable]
        public SiegeWeaponSpawner()
            : base()
        {
            SpawnRange = 0;
            MinDelay = DefaultMinDelay;
            MaxDelay = DefaultMaxDelay;
            SmartSpawning = false;
            Name = "Battleground weapon spawner - Do not delete.";
        }

        public SiegeWeaponSpawner(Serial serial)
            : base(serial)
        {

        }

        public override void OnDelete()
        {
            if (m_Battleground != null && m_Battleground.SiegeSpawners.Contains(this))
                m_Battleground.SiegeSpawners.Remove(this);
            base.OnDelete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            // version 1 timer updates
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version < 1)
            {
                MinDelay = DefaultMinDelay;
                MaxDelay = DefaultMaxDelay;
            }

            OnMapChange();
        }

        public override void OnMapChange()
        {
            base.OnMapChange();
            if (Location == Point3D.Zero) return;

            var region = Region.Find(Location, Map);
            if (!(region is BattlegroundRegion))
            {
                Delete();
                return;
            }

            m_Battleground = ((BattlegroundRegion)region).Battleground as SiegeBattleground;

            if (m_Battleground == null)
            {
                Delete();
                return;
            }

            m_Battleground.SiegeSpawners.Add(this);
        }
    }
}
