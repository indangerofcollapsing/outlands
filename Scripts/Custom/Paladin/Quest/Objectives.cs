using System;
using Server;
using Server.Mobiles;
using Server.Items;

namespace Server.Engines.Quests.Paladin
{
	public class ReturnDeceitItem : QuestObjective
	{
        private static readonly Point3D[] m_Locations = new Point3D[] { new Point3D(5139, 663, 0), new Point3D(5181, 724, 0), new Point3D(5151, 743, 0), new Point3D(5199, 655, 0), new Point3D(5211, 744, -20), new Point3D(5307, 668, 0), new Point3D(5266, 690, 0), new Point3D(5315, 749, -20) };
        private PaladinQuestItem m_PaladinSword;

		public override object Message
		{
			get
			{
				return "Travel to the depths of Deceit and find the stolen Paladin Silver Sword, before it is lost for good.";
			}
		}

        public void GenerateDeceitItem(Mobile player)
        {
            m_PaladinSword = new PaladinQuestItem(player, "Paladin Silver Sword", 5049, 2101);
            Point3D position = PaladinQuestItem.AdjustPaladinQuestItemSpawnPoint(m_Locations[Utility.Random(m_Locations.Length)]);
            m_PaladinSword.MoveToWorld(position, Map.Felucca);
            PaladinQuestItem.AllPaladinQuestItems.Add(m_PaladinSword);
        }

        public ReturnDeceitItem()
		{
		}

        public override void CheckProgress()
        {
            if (m_StartTime + TimeSpan.FromHours(5) < DateTime.UtcNow)
                System.Cancel();

            base.CheckProgress();
        }

        public override void OnCancelled()
        {
            if (m_PaladinSword != null)
                m_PaladinSword.Delete();
        }

        public override void RenderProgress(BaseQuestGump gump)
        {
            base.RenderProgress(gump);
        }

		public override void OnComplete()
		{
            System.AddConversation(new DeceitItemReturned());
		}

        public override void ChildSerialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(0);
            writer.Write(m_PaladinSword);
        }

        public override void ChildDeserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            //version 0
            m_PaladinSword = (PaladinQuestItem)reader.ReadItem();
        }
	}
    public class ReturnHythlothItem : QuestObjective
    {
        private static readonly Point3D[] m_Locations = new Point3D[] { new Point3D(6122, 218, 22), new Point3D(6122, 159, 0), new Point3D(6053, 157, 0), new Point3D(6028, 197, 22), new Point3D(6049, 227, 44), new Point3D(6083, 90, 22), new Point3D(6111, 84, 0), new Point3D(6104, 44, 22), new Point3D(6049, 45, 0) };
        private PaladinQuestItem m_PaladinShield;

        public override object Message
        {
            get
            {
                return "Travel to the depths of Hythloth and find the stolen Paladin Silver Shield, before it is lost for good.";
            }
        }

        public void GenerateHythlothItem(Mobile player)
        {
            m_PaladinShield = new PaladinQuestItem(player, "Paladin Shield", 7108, 150);
            Point3D position = PaladinQuestItem.AdjustPaladinQuestItemSpawnPoint(m_Locations[Utility.Random(m_Locations.Length)]);
            m_PaladinShield.MoveToWorld(position, Map.Felucca);
            PaladinQuestItem.AllPaladinQuestItems.Add(m_PaladinShield);
        }

        public ReturnHythlothItem()
        {
        }

        public override void CheckProgress()
        {
            if (m_StartTime + TimeSpan.FromHours(5) < DateTime.UtcNow)
                System.Cancel();

            base.CheckProgress();
        }

        public override void OnCancelled()
        {
            if (m_PaladinShield != null)
                m_PaladinShield.Delete();
        }

        public override void RenderProgress(BaseQuestGump gump)
        {
            base.RenderProgress(gump);
        }

        public override void OnComplete()
        {
            if (PaladinSiege.CanBeginSiege())
                System.AddConversation(new HythlothItemReturned());
            else
                System.AddConversation(new SiegeInProcess());
        }

        public override void ChildSerialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(0);
            writer.Write(m_PaladinShield);
        }

        public override void ChildDeserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            //version 0
            m_PaladinShield = (PaladinQuestItem)reader.ReadItem();
        }
    }
    public class WaitingForSiege : QuestObjective
    {
        public override object Message
        {
            get
            {
                return "Return to the Paladin Order Guard when the city is free from attacks.";
            }
        }

        public WaitingForSiege()
        {
        }

        public override void OnComplete()
        {
            System.AddConversation(new HythlothItemReturned());
        }

        public override void ChildSerialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(0);
        }

        public override void ChildDeserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();
        }
    }
    public class DefendTrinsic : QuestObjective
    {
        public override object Message
        {
            get
            {
                return "Defend the citizens from the daemons and undead attempting to take back the artifacts!";
            }
        }

        public DefendTrinsic()
        {
        }

        public override void OnComplete()
        {
            System.Complete();
        }

        public override void ChildSerialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(0);
        }

        public override void ChildDeserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();
        }
    }

}