using System;
using Server;
using Server.Multis;
using Server.Network;
using Server.Gumps;
using Server.ContextMenus;

namespace Server.Items
{
    public class TillerMan : Item
    {
        private BaseBoat m_Boat;
        public BaseBoat Boat { get { return m_Boat; } }

        public TillerMan(BaseBoat boat): base(0x3E4E)
        {
            m_Boat = boat;
            Movable = false;
        }

        public TillerMan(Serial serial): base(serial)
        {
        }

        public void SetFacing(Direction dir)
        {
            switch (dir)
            {
                case Direction.South: ItemID = 0x3E4B; break;
                case Direction.North: ItemID = m_Boat is GalleonBoat || m_Boat is CarrackBoat ? 0x3855 : 0x3E4E; break; //
                case Direction.West: ItemID = 0x3E50; break;
                case Direction.East: ItemID = 0x3E53; break;
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
        }

        public void Say(string text)
        {
            PublicOverheadMessage(MessageType.Regular, 0x3B2, false, text);
        }

        public void Say(int number)
        {
            PublicOverheadMessage(MessageType.Regular, 0x3B2, number);
        }

        public void Say(int number, string args)
        {
            PublicOverheadMessage(MessageType.Regular, 0x3B2, number, args);
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            if (m_Boat != null && m_Boat.ShipName != null)
                list.Add(1042884, m_Boat.ShipName); // the tiller man of the ~1_SHIP_NAME~
            else
                base.AddNameProperty(list);
        }

        public override void OnSingleClick(Mobile from)
        {
            int notoriety = BaseBoat.GetBoatNotoriety(from, m_Boat);
            int labelHue = Notoriety.Hues[notoriety];
            string shipLabel = m_Boat.Name;

            if (!(m_Boat.ShipName == "" || m_Boat.ShipName == null))
                shipLabel += ": " + m_Boat.ShipName;

            //from.PrivateOverheadMessage(MessageType.Label, labelHue, false, shipLabel, from.NetState);

            LabelTo(from, "Hull: {0} / {1}", m_Boat.HitPoints, m_Boat.MaxHitPoints);
            LabelTo(from, "Sails: {0} / {1}", m_Boat.SailPoints, m_Boat.MaxSailPoints);
            LabelTo(from, "Guns: {0} / {1}", m_Boat.GunPoints, m_Boat.MaxGunPoints);
        }

        public override void OnDoubleClickDead(Mobile from)
        {
            OnDoubleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (m_Boat == null || from == null)
                return;

            //Ship Gump  
            if (m_Boat.Owner != null)
            {
                if (!m_Boat.m_ScuttleInProgress)
                    from.SendGump(new BoatGump(from, m_Boat));
            }
        }

        public override void OnAfterDelete()
        {
            if (m_Boat != null)
                m_Boat.Delete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);//version

            writer.Write(m_Boat);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        m_Boat = reader.ReadItem() as BaseBoat;

                        if (m_Boat == null)
                            Delete();

                        break;
                    }
            }
        }
    }
}