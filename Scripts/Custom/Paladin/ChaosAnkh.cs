/***************************************************************************
 *                               ChaosAnkh.cs
 *                            ------------------
 *   begin                : July 2010
 *   author               : Sean Stavropoulos
 *   email                : sean.stavro@gmail.com
 *
 *
 ***************************************************************************/
using System;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Multis;
using Server.Gumps;
using System.Collections;
using System.Collections.Generic;
using Server.ContextMenus;

namespace Server.Items
{
    public class BaseChaosAnkh : AddonComponent
    {
        private Mobile m_Owner;

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Owner { get { return m_Owner; } set { m_Owner = value; InvalidateProperties(); } }

        [Constructable]
		public BaseChaosAnkh( int itemID ) : base( itemID )
		{

            Movable = false;
            Name = "Chaos Ankh";
            LootType = LootType.Blessed;
		}

        public BaseChaosAnkh(Serial serial)
            : base(serial)
		{
		}

        public static void MurdererRessurected()
        {
            Rectangle2D redzone = PaladinEvents.MurdererSafeZone;
            int randomRange = Utility.RandomMinMax(-5,15);
            redzone = new Rectangle2D(new Point2D(redzone.Start.X-randomRange,redzone.Start.Y-randomRange),new Point2D(redzone.End.X+randomRange,redzone.End.Y+randomRange));
            IPooledEnumerable eable = Map.Felucca.GetMobilesInBounds(redzone);

            Queue q = new Queue();

            foreach (Mobile m in eable)
            {
                if (m.Player && ((PlayerMobile)m).Paladin && m.Alive)
                {
                    m.Emote("You feel the ground trembling.");
                    m.SendMessage("The nearby Chaos Ankh unleashes its power on those pure of heart.");
                    m.PlaySound( 0x2F3 );
                    q.Enqueue(m);
                }
            }
            eable.Free();

            Timer.DelayCall(TimeSpan.FromSeconds(2), delegate
            {
                while (q.Count > 0)
                {
                    Mobile m = (Mobile)q.Dequeue();

                    if (m != null)
                    {
                        m.LastKiller = null;
                        m.Kill();
                    }
                }

            });
            
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version

            writer.Write((Mobile)m_Owner);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();

            m_Owner = reader.ReadMobile();
        }

        #region custom methods
        public bool IsHomeOwner(Mobile from)
        {
            if (from.AccessLevel >= AccessLevel.GameMaster)
                return true;

            BaseHouse house = BaseHouse.FindHouseAt(this);
            return (house != null && house.IsOwner(from));
        }
        #endregion

        #region overrided Methods
        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            from.SendMessage("This is not a container!");
            return false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            //if (!from.InRange(this.GetWorldLocation(), 2))
            //{
            //    from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            //}
            //else
            //{
                //bool isHomeOwner = IsHomeOwner(from);

                //Console.WriteLine("OnDoubleClick");
                // set owner if not already set -- this is only done the first time.
                //if (m_Owner == null && isHomeOwner)
                //{
                    //m_Owner = from;
                    //this.Name = m_Owner.Name.ToString() + "'s Chaos Ankh";
                    //this.Name = m_Owner.Name.ToString() + "'s Shrine";
                    //from.SendMessage("Ownership of this ankh has been bestowed upon you.");
                //}
                //else
                //{
                    //if (m_Owner != from)
                    //{
                    //    from.SendMessage("This is not your's to use.");
                    //    return;
                    //}
                    //else if (!isHomeOwner)
                    //{
                    //    from.SendMessage("This is not your home!");
                    //}
                //}
            //}
        }

        public override bool HandlesOnMovement { get { return true; } } // Tell the core that we implement OnMovement

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (Parent == null && Utility.InRange(Location, m.Location, 1) && !Utility.InRange(Location, oldLocation, 1))
            {
                if (m.Alive)
                    return;

                if (!m.InRange(this.GetWorldLocation(), 2)) //2 = Resurrect Range
                    m.SendLocalizedMessage(500446); // That is too far away.
                else if (m is PlayerMobile && (((PlayerMobile)m).RestitutionFee > 0 || ((PlayerMobile)m).MurdererDeathGumpNeeded))
                {
                    m.SendMessage("You have not paid sufficiently for your crimes and shall not be ressurected.");
                    return;
                }
                else if (m.Map != null && m.Map.CanFit(m.Location, 16, false, false))
                {
                    m.CloseGump(typeof(ResurrectGump));
                    m.SendGump(new ResurrectGump(m, ResurrectMessage.ChaosShrine));
                }
                else
                    m.SendLocalizedMessage(502391); // Thou can not be resurrected there!
            }
              
        }
        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);
            Ankhs.GetContextMenuEntries(from, this, list);
        }
        public override void OnDoubleClickDead(Mobile m)
        {
            if (m is PlayerMobile && (((PlayerMobile)m).RestitutionFee > 0 || ((PlayerMobile)m).MurdererDeathGumpNeeded))
            {
                m.SendMessage("You have not paid sufficiently for your crimes and shall not be ressurected.");
                return;
            }

            Ankhs.Resurrect(m, this);
        }
        #endregion
    }

    #region ChaosResAnkh West
    public class ChaosResAnkhWestPartA : BaseChaosAnkh
    {
        [Constructable]
        public ChaosResAnkhWestPartA()
            : base(0x2)
        {
        }

        public ChaosResAnkhWestPartA(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version
        }
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }

    public class ChaosResAnkhWestPartB : BaseChaosAnkh
    {
        [Constructable]
        public ChaosResAnkhWestPartB()
            : base(0x3)
        {
        }

        public ChaosResAnkhWestPartB(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version
        }
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }
    #endregion

    #region ChaosResAnkh North
    public class ChaosResAnkhNorthPartA : BaseChaosAnkh
    {
        [Constructable]
        public ChaosResAnkhNorthPartA()
            : base(0x5)
        {
        }

        public ChaosResAnkhNorthPartA(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version
        }
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }

    public class ChaosResAnkhNorthPartB : BaseChaosAnkh
    {
        [Constructable]
        public ChaosResAnkhNorthPartB()
            : base(0x4)
        {
        }

        public ChaosResAnkhNorthPartB(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version
        }
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }
    #endregion

    #region class ChaosResAnkh
    public class ChaosResAnkh : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new ChaosResAnkhDeed(); } }

        private AddonComponent GetComponent(BaseChaosAnkh component, int hue)
        {
            BaseChaosAnkh ac = new BaseChaosAnkh(component.ItemID);

            ac.Hue = hue;
            ac.Name = "chaos shrine ankh";

            return ac;
        }

        public ChaosResAnkh(bool east)
        {
            int hue = Utility.RandomList(2970);

            if (east)
            {
                AddComponent(GetComponent(new ChaosResAnkhWestPartA(), hue), 0, 0, 0);
                AddComponent(GetComponent(new ChaosResAnkhWestPartB(), hue), 0, -1, 0);
            }
            else
            {
                AddComponent(GetComponent(new ChaosResAnkhNorthPartA(), hue), 0, 0, 0);
                AddComponent(GetComponent(new ChaosResAnkhNorthPartB(), hue), -1, 0, 0);
            }
        }

        public ChaosResAnkh(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
    #endregion

    #region class ChaosResAnkhDeed
    public class ChaosResAnkhDeed : BaseAddonDeed
    {
        private bool m_East;
        public override BaseAddon Addon { get { return new ChaosResAnkh(m_East); } }

        [Constructable]
        public ChaosResAnkhDeed()
        {
            Name = "a deed for a chaos shrine ankh";
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(InternalGump));
                from.SendGump(new InternalGump(this));
            }
            else
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
        }

        private void SendTarget(Mobile m)
        {
            base.OnDoubleClick(m);
        }

        private class InternalGump : Gump
        {
            private ChaosResAnkhDeed m_Deed;

            public InternalGump(ChaosResAnkhDeed deed)
                : base(150, 50)
            {
                m_Deed = deed;

                AddBackground(0, 0, 350, 200, 0xA28);

                AddItem(100, 35, 0x4);
                AddItem(122, 35, 0x5);
                AddButton(70, 35, 0x868, 0x869, 1, GumpButtonType.Reply, 0); // South

                AddItem(205, 35, 0x2);
                AddItem(227, 35, 0x3);
                AddButton(185, 35, 0x868, 0x869, 2, GumpButtonType.Reply, 0); // East
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (m_Deed.Deleted || info.ButtonID == 0)
                    return;

                try
                {
                    m_Deed.m_East = (info.ButtonID != 1);
                    m_Deed.SendTarget(sender.Mobile);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message + "InternalGump OnResponse try/catch");
                }
            }
        }

        public ChaosResAnkhDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }
    #endregion
}