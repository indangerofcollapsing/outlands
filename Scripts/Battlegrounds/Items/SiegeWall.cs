using Server.Custom.Battlegrounds.Regions;
using Server.Engines.XmlSpawner2;
using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom.Battlegrounds.Items
{
    public class SiegeWall : Item, ISiegeable
    {
        private SiegeBattleground m_Battleground;

        protected XmlSiege Attachment { get { return (XmlSiege)XmlAttach.FindAttachment(this, typeof(XmlSiege)); } }

        public int HitsMax { get { return 1000; } }

        [Constructable]
        public SiegeWall(int itemID)
            : base(itemID)
        {
            this.Movable = false;
            var siege = new XmlSiege();
            siege.HitsMax = HitsMax;
            siege.Hits = HitsMax;
            siege.ResistFire = 0;
            siege.ResistPhysical = 0;
            XmlAttach.AttachTo(this, siege);
        }

        public SiegeWall(Serial serial) 
            : base(serial) 
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1); //version

            //version 1 to update hp

        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            OnMapChange();
        }

        public int Hits { get { return Attachment.Hits; } }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, string.Format("Defense Wall: [{0}/{1}]", Hits, HitsMax));
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
            Attachment.HitsMax = HitsMax;
            Attachment.Hits = HitsMax;
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);
            if (from.Warmode && !m_Battleground.Defense.Contains(from as PlayerMobile))
            {
                Item weapon = from.FindItemOnLayer(Layer.OneHanded);
                if (weapon == null)
                    weapon = from.FindItemOnLayer(Layer.TwoHanded);

                if (weapon == null)
                {
                    from.SendMessage("You must equip a weapon to attack this.");
                    return;
                }

                HandSiegeAttack a = (HandSiegeAttack)XmlAttach.FindAttachment(weapon, typeof(HandSiegeAttack));

                if (a == null || a.Deleted)
                {
                    a = new HandSiegeAttack();
                    XmlAttach.AttachTo(weapon, a);
                }

                a.BeginAttackTarget(from, this, this.Location);
            }
        }
    }
}
