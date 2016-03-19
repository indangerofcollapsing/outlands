using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Prompts;
using System.Collections;
using Server.Spells;

namespace Server.Items
{
    public class T2ATicket : Item
    {
        public override string DefaultName { get { return "T2A Access Ticket"; } }

        [Constructable]
        public T2ATicket()
            : base(0x2258)
        {
            LootType = LootType.Blessed; 
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
            LabelTo(from, "Use to gain access!");
        }

        public void PromptResponse(Mobile from, bool accepted)
        {
            if (!(from is PlayerMobile))
                return;

            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042553); //You must have the object in your backpack to use it.
                return;
            }

            PlayerMobile pm = from as PlayerMobile;

            if (accepted)
            {
                from.SendMessage("You have now gained access to T2A for a period of two weeks!");
				pm.T2AAccess = DateTime.UtcNow + TimeSpan.FromDays(14);
                Delete();
            }
            else
            {
                from.SendMessage("If you change your mind at a later date, please use this ticket again.");
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042553); //You must have the object in your backpack to use it.
                return;
            }

            from.Prompt = new InternalPrompt(this);
            from.SendMessage("Using this ticket will give your character access to T2A for a period of two (2) weeks. Type 'yes' if you wish to proceed.");
        }

        public T2ATicket(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }

        private class InternalPrompt : Prompt
        {
            T2ATicket m_Ticket;

            public InternalPrompt(T2ATicket ticket)
            {
                m_Ticket = ticket;
            }

            public override void OnResponse(Mobile from, string text)
            {
                if (m_Ticket != null)
                    m_Ticket.PromptResponse(from, text.ToLower() == "yes");
            }
        }

    }

    public class T2ATeleporter : Teleporter
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public bool ReplaceT2ATeleporters { get { return false; } set {
            if (value)
            {
                Queue q = new Queue();

                foreach (Item item in World.Items.Values)
                    if ((item is Teleporter) && Spells.SpellHelper.IsAnyT2A(((Teleporter)item).MapDest, ((Teleporter)item).PointDest))
                        q.Enqueue(item);

                while (q.Count > 0)
                {
                    Teleporter tp = (Teleporter)q.Dequeue();
                    T2ATeleporter t2a = new T2ATeleporter();
                    t2a.PointDest = tp.PointDest;
                    t2a.MapDest = tp.MapDest;
                    t2a.MoveToWorld(tp.Location, tp.Map);
                    tp.Delete();
                }
            }
            else
            {
                Queue q = new Queue();

                foreach (Item item in World.Items.Values)
                    if ((item is T2ATeleporter))
                        q.Enqueue(item);

                while (q.Count > 0)
                {
                    T2ATeleporter tp = (T2ATeleporter)q.Dequeue();
                    Teleporter t2a = new Teleporter();
                    t2a.PointDest = tp.PointDest;
                    t2a.MapDest = tp.MapDest;
                    t2a.MoveToWorld(tp.Location, tp.Map);
                    tp.Delete();
                }
            }
        }
        }

        [Constructable]
        public T2ATeleporter()
        {
            Name = "t2a teleporter";
        }

        public override void StartTeleport(Mobile m)
        {
            if (T2AAccess.HasAccess(m))
                base.StartTeleport(m);
            else
                m.SendMessage("T2A Access is disabled at this time.");
        }

        public T2ATeleporter(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }

    public static class T2AAccess
    {
        public static bool HasAccess(Mobile m)
        {
            if (!(m is PlayerMobile))
                return true;

			return m.AccessLevel >= AccessLevel.GameMaster || ((PlayerMobile)m).T2AAccess > DateTime.UtcNow;
        }

        public static bool IsT2A(Point3D p, Map m)
        {
            if (m == null)
                m = Map.Felucca;

            return SpellHelper.IsAnyT2A(m, p);
        }

        public static bool IsT2A(Point2D p, Map m)
        {
            if (m == null)
                m = Map.Felucca;

            return SpellHelper.IsAnyT2A(m, new Point3D(p,0));
        }
    }
}
