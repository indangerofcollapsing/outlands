using Server.Custom.Battlegrounds.Regions;
using Server.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom.Battlegrounds.Items
{
    class StationarySiegeCatapult : SiegeCatapult, ISiegeable
    {
        private SiegeBattleground m_Battleground;

        public override double RangeReductionWhenDamaged { get { return 1; } }
        public override double DamageReductionWhenDamaged { get { return 1; } }
        public override double WeaponRangeFactor { get { return 2; } }
        public virtual int MinTargetRange { get { return 6; } }

        public int HitsMax { get { return 125; } }

        public override bool HasFiringAngle(IPoint3D t)
        {
            return true;
        }

        [Constructable]
        public StationarySiegeCatapult()
            : base()
        {
            Movable = false;
            IsDraggable = false;
            IsPackable = false;
            SiegeAttachment.ResistPhysical = 0;
            SiegeAttachment.ResistFire = 0;
            SiegeAttachment.HitsMax = HitsMax;
            SiegeAttachment.Hits = HitsMax;

            FreeConsume = true;
            Projectile = new BattlegroundSiegeCatapultShot();
        }

        public StationarySiegeCatapult(Serial serial) 
            : base(serial) 
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); //version
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
            SiegeAttachment.HitsMax = HitsMax;
            SiegeAttachment.Hits = HitsMax;
            Projectile = new BattlegroundSiegeCatapultShot();
        }

    }
}
