using Server.Custom.Battlegrounds.Regions;
using Server.Engines.XmlSpawner2;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom.Battlegrounds.Items
{
    public class WoodenSiegeDoor : SiegeDoor
    {
        [Constructable]
        public WoodenSiegeDoor(DoorFacing facing)
            : base(0x675 + (2 * (int)facing), 0x6B5 + (2 * (int)facing), 0xEA, 0xF1, BaseDoor.GetOffset(facing))
        {

        }

        public WoodenSiegeDoor(Serial serial)
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
        }
    }

    public class BarredMetalSiegeDoor : SiegeDoor
    {
        [Constructable]
        public BarredMetalSiegeDoor( DoorFacing facing ) : base( 0x685 + (2 * (int)facing), 0x686 + (2 * (int)facing), 0xEC, 0xF3, BaseDoor.GetOffset( facing ) )
        {
        }

        public BarredMetalSiegeDoor(Serial serial)
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
        }
    }

    public class MetalSiegeDoor : SiegeDoor
    {
        [Constructable]
        public MetalSiegeDoor(DoorFacing facing)
            : base(0x675 + (2 * (int)facing), 0x676 + (2 * (int)facing), 0xEC, 0xF3, BaseDoor.GetOffset(facing))
        {

        }

        public MetalSiegeDoor(Serial serial)
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
        }

    }

    public class SiegeDoor : BaseDoor, ISiegeable
    {
        protected XmlSiege Attachment { get { return (XmlSiege)XmlAttach.FindAttachment(this, typeof(XmlSiege)); } }

        public int HitsMax { get { return 1000; } }

        private SiegeBattleground m_Battleground;

        public SiegeDoor(int closedId, int openId, int openSoundId, int closeSoundId, Point3D offset)
            : base(closedId, openId, openSoundId, closeSoundId, offset)
        {
            var siege = new XmlSiege();
            siege.HitsMax = HitsMax;
            siege.Hits = HitsMax;
            siege.ResistFire = 0;
            siege.ResistPhysical = 0;
            XmlAttach.AttachTo(this, siege);
        }

        public SiegeDoor(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);

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
            LabelTo(from, string.Format("Defense Door: [{0}/{1}]", Hits, HitsMax));
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

        public override void Use(Mobile from)
        {
            if (m_Battleground.Active && from is PlayerMobile)
            {
                var player = from as PlayerMobile;
                if (m_Battleground.Offense.Contains(player))
                    from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 502503); // That is locked.
                else if (m_Battleground.Defense.Contains(player))
                {
                    from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 501282); // You quickly unlock, open, and relock the door
                }
            }
            else
            {
                base.Use(from);
            }
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

                a.BeginAttackTarget(from, this, Location);
            }
        }
    }
}
