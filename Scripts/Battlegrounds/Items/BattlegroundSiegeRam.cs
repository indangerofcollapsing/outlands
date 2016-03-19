using Server.Custom.Battlegrounds.Regions;
using Server.Engines.XmlSpawner2;
using Server.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom.Battlegrounds.Items
{
    public class BattlegroundSiegeRam : SiegeRam, ISiegeable
    {

        private SiegeBattleground m_Battleground;

        public override double RangeReductionWhenDamaged { get { return 1; } }
        public override double DamageReductionWhenDamaged { get { return 1; } }
        public override double WeaponRangeFactor { get { return 1; } } // ammo range * rangefactor for distance
        public override double WeaponDamageFactor { get { return Math.Min(1.5 - (m_Battleground.PlayerCount / 2) * 0.10, 1.0); } }
        public override int MinTargetRange { get { return 0; } }



        public override bool HasFiringAngle(IPoint3D t)
        {
            return true;
        }

        [Constructable]
        public BattlegroundSiegeRam()
            : base()
        {
            IsPackable = false;
            SiegeAttachment.ResistPhysical = 0;
            SiegeAttachment.ResistFire = 0;
            SiegeAttachment.HitsMax = 250;
            SiegeAttachment.Hits = 250;
            SiegeAttachment.DestroyedItemID = 0;

            FreeConsume = true;
            Projectile = new BattlegroundSiegeLog();
        }

        public BattlegroundSiegeRam(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
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

            m_Battleground.SiegeItems.Add(this);
        }
        public void RepairOrDelete()
        {
            this.Delete();
        }
    }
}
