using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;

namespace Server.Engines.Quests.Paladin
{
    public class PaladinQuestItem : QuestItem
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile QuestMobile { get; set; }
        public override bool Decays { get { return false; } }
        private DateTime m_Created;
        public static List<PaladinQuestItem> AllPaladinQuestItems = new List<PaladinQuestItem>();

        public PaladinQuestItem(Mobile from, string name, int itemID, int hue)
            : base(itemID)
        {
            Name = name;
            Hue = hue;
            LootType = LootType.Newbied;
            QuestMobile = from;
            m_Created = DateTime.UtcNow;
        }

        public static Point3D AdjustPaladinQuestItemSpawnPoint(Point3D position)
        {
            Point3D adjustedPosition = new Point3D(position.X, position.Y, position.Z);

            List<PaladinQuestItem> conflicts = AllPaladinQuestItems.FindAll(i => i.GetWorldLocation().X == position.X && i.GetWorldLocation().Y == position.Y);
            if (conflicts.Count > 0)
            {
                // need to get the Z of the highest stacked item
                conflicts.Sort((left, right) => left.GetWorldLocation().Z.CompareTo(right.GetWorldLocation().Z));
                adjustedPosition.Z = conflicts[conflicts.Count - 1].GetWorldLocation().Z + 1;
            }
            return adjustedPosition;
        }

        public override bool CanBeSeenBy(Mobile from)
        {
            return (from == QuestMobile || (from != null && from.AccessLevel >= AccessLevel.GameMaster));
        }

        public override bool CanDrop(PlayerMobile pm)
        {
            return false;
        }

        public PaladinQuestItem(Serial serial)
            : base(serial)
        {
        }

        public override bool OnDragLift(Mobile from)
        {
            if (from != null && from.AccessLevel >= AccessLevel.GameMaster)
                return true;

            if (from != QuestMobile)
            {
                from.SendMessage("That belongs to another questing paladin");
                return false;
            }
            else
            {
                return true;
            }
        }

        public override void OnDelete()
        {
            AllPaladinQuestItems.RemoveAll(i => i.CompareTo(this) == 0);
            base.OnDelete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(1);

            //version 1
            writer.Write(m_Created);

            //version 0
            writer.Write(QuestMobile);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();

            if (version > 0)
                m_Created = reader.ReadDateTime();
            else
                m_Created = DateTime.UtcNow;

            //version 0
            QuestMobile = reader.ReadMobile();

            if (QuestMobile == null || m_Created + TimeSpan.FromHours(5) < DateTime.UtcNow)
                Timer.DelayCall(TimeSpan.FromTicks(1), Delete);

            // keep track of items in the world
            AllPaladinQuestItems.Add(this);
        }
    }
}