using System;
using Server.Items;
using Server.ContextMenus;
using Server.Multis;
using Server.Misc;
using Server.Network;
using Server.Custom;
using System.Collections;
using System.Collections.Generic;

namespace Server.Mobiles
{
	public class UOACZBaseCivilian : UOACZBaseHuman
	{
        public static List<UOACZBaseCivilian> m_Instances = new List<UOACZBaseCivilian>();

        public UOACZCivilianSpawner m_Spawner;

        public override string[] wildernessIdleSpeech { get { return new string[0]; } }
        public override string[] idleSpeech { get { return new string[0]; } }
        public override string[] undeadCombatSpeech { get { return new string[0]; } }
        public override string[] humanCombatSpeech { get { return new string[0]; } }

        public override UOACZSystem.StockpileContributionType StockpileContributionType { get { return UOACZSystem.StockpileContributionType.None; } }
        public override double StockpileContributionScalar { get { return 1.0; } }

        public override int DifficultyValue { get { return 2; } }

        public enum CivilianType
        {
            None,
            Any,
            Alchemist,
            Weaver,
            Farmer,
            Fisherman,
            Skinner,
            Baker,
            Butcher,
            Bowyer,
            Provisioner,
            Brewmaster,
            Carpenter,
            Miner,
            Healer,
            Tinker,
            Blacksmith,
            Merchant,
            Noble
        }

        [Constructable]
		public UOACZBaseCivilian() : base()
		{
            int hairHue = Utility.RandomHairHue();

            if (Female)
                Utility.AssignRandomHair(this, hairHue);
            else
            {
                if (Utility.RandomDouble() <= .90)
                    Utility.AssignRandomHair(this, hairHue);
            }

            if (!Female && Utility.RandomDouble() <= .5)
                Utility.AssignRandomFacialHair(this, hairHue);

            m_Instances.Add(this);
		}

        public override void SetUniqueAI()
        {
            base.SetUniqueAI();

            DictCombatAction[CombatAction.AttackOnly] = 15;
            DictCombatAction[CombatAction.CombatHealSelf] = 1;

            //DictCombatHealSelf[CombatHealSelf.PotionHealSelf50] = 1;
            DictCombatHealSelf[CombatHealSelf.PotionCureSelf] = 1;

            DictWanderAction[WanderAction.None] = 2;
            //DictWanderAction[WanderAction.BandageHealSelf100] = 1;
            DictWanderAction[WanderAction.PotionCureSelf] = 1;

            SetSkill(SkillName.Healing, 40);
        }

        public void CreateRandomOutfit()
        {
            //Shirt            
            switch (Utility.RandomMinMax(1, 5))
            {
                case 1: AddItem(new Shirt(Utility.RandomDyedHue()) { Movable = false }); break;
                case 2: AddItem(new FancyShirt(Utility.RandomDyedHue()) { Movable = false }); break;
                case 3: AddItem(new Doublet(Utility.RandomDyedHue()) { Movable = false }); break;
                case 4: AddItem(new Surcoat(Utility.RandomDyedHue()) { Movable = false }); break;
                case 5: AddItem(new Tunic(Utility.RandomDyedHue()) { Movable = false }); break;
            }

            //Pants
            if (Female)
            {
                switch (Utility.RandomMinMax(1, 3))
                {
                    case 1: AddItem(new Skirt(Utility.RandomNeutralHue()) { Movable = false }); break;
                    case 2: AddItem(new Kilt(Utility.RandomNeutralHue()) { Movable = false }); break;
                    case 3: AddItem(new ShortPants(Utility.RandomNeutralHue()) { Movable = false }); break;
                }
            }

            else
            {
                switch (Utility.RandomMinMax(1, 2))
                {
                    case 1: AddItem(new ShortPants(Utility.RandomNeutralHue()) { Movable = false }); break;
                    case 2: AddItem(new LongPants(Utility.RandomNeutralHue()) { Movable = false }); break;
                }
            }

            //Shoes            
            switch (Utility.RandomMinMax(1, 4))
            {
                case 1: AddItem(new Shoes(Utility.RandomNeutralHue()) { Movable = false }); break;
                case 2: AddItem(new Boots(Utility.RandomNeutralHue()) { Movable = false }); break;
                case 3: AddItem(new ThighBoots(Utility.RandomNeutralHue()) { Movable = false }); break;
                case 4: AddItem(new Sandals(Utility.RandomNeutralHue()) { Movable = false }); break;
            }
        }

        public static List<UOACZBaseCivilian> GetActiveInstances()
        {
            List<UOACZBaseCivilian> m_ActiveInstances = new List<UOACZBaseCivilian>();

            for (int a = 0; a < m_Instances.Count; a++)
            {
                UOACZBaseCivilian instance = m_Instances[a];

                if (instance.Deleted)
                    continue;

                if (UOACZRegion.ContainsMobile(instance))
                    m_ActiveInstances.Add(instance);
            }

            return m_ActiveInstances;
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (m_Instances.Contains(this))
                m_Instances.Remove(this);

            if (m_Spawner != null)
            {
                if (m_Spawner.m_Mobiles.Contains(this))
                    m_Spawner.m_Mobiles.Remove(this);
            }

            UOACZEvents.CivilianKilled();
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (m_Instances.Contains(this))
                m_Instances.Remove(this);

            if (m_Spawner != null)
            {
                if (m_Spawner.m_Mobiles.Contains(this))
                    m_Spawner.m_Mobiles.Remove(this);
            }
        }

        public UOACZBaseCivilian(Serial serial): base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version

            //Version 0
            writer.Write(m_Spawner);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

            m_Spawner = (UOACZCivilianSpawner)reader.ReadItem();

            //-----------

            m_Instances.Add(this);
		}
	}
}
