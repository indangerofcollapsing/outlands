using System;
using Server;
using Server.Regions;
using Server.Targeting;
using Server.Engines.CannedEvil;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;
using Server.Multis;
using System.Collections;
using System.Collections.Generic;

namespace Server.Custom
{    
    public class HenchmanCreationDeed : Item
    {
        public enum CreatureType
        {
            //Undead
            BoneMagi,
            Lich,
            Mummy,
            RottingCorpse,
            SkeletalKnight,
            Skeleton,
            Spectre,
            VampireCountess,
            VampireThrall,
            Zombie
        }

        private CreatureType m_Creature;
        [CommandProperty(AccessLevel.GameMaster)]
        public CreatureType Creature
        {
            get { return m_Creature; }
            set { m_Creature = value; }
        }

        private int m_Experience;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Experience
        {
            get { return m_Experience; }
            set { m_Experience = value; }
        }

        public static TimeSpan CombatDelay = TimeSpan.FromSeconds(60);
        public static TimeSpan CombatPvPDelay = TimeSpan.FromSeconds(300);
      
        [Constructable]
        public HenchmanCreationDeed(): base(0x14F0)
		{
            Name = "a henchman creation deed";

            Weight = 0.1;
            Hue = 2500;

            SetRandomCreature();
            m_Experience = Utility.Random(1, 250);
		}

        [Constructable]
        public HenchmanCreationDeed(CreatureType creatureType): base(0x14F0)
        {
            Name = "a henchman creation deed";

            Weight = 0.1;
            Hue = 2500;

            m_Creature = creatureType;

            m_Experience = Utility.Random(1, 250);
        }

        public HenchmanCreationDeed(Serial serial): base(serial)
		{
		}

        public void SetRandomCreature()
        {
            CreatureType creatureType = CreatureType.Skeleton;

            Dictionary<CreatureType, int> DictItemTypes = new Dictionary<CreatureType, int>();

            DictItemTypes.Add(CreatureType.Skeleton, 5);
            DictItemTypes.Add(CreatureType.Zombie, 5);

            DictItemTypes.Add(CreatureType.BoneMagi, 4);
            DictItemTypes.Add(CreatureType.Spectre, 4);

            DictItemTypes.Add(CreatureType.Lich, 3);
            DictItemTypes.Add(CreatureType.SkeletalKnight, 3);

            DictItemTypes.Add(CreatureType.RottingCorpse, 2);
            DictItemTypes.Add(CreatureType.VampireThrall, 2);

            DictItemTypes.Add(CreatureType.VampireCountess, 1);
            DictItemTypes.Add(CreatureType.Mummy, 1);

            int TotalValues = 0;

            foreach (KeyValuePair<CreatureType, int> pair in DictItemTypes)
            {
                TotalValues += pair.Value;
            }

            double ActionCheck = Utility.RandomDouble();
            double CumulativeAmount = 0.0;
            double AdditionalAmount = 0.0;

            bool foundDirection = true;

            foreach (KeyValuePair<CreatureType, int> pair in DictItemTypes)
            {
                AdditionalAmount = (double)pair.Value / (double)TotalValues;

                if (ActionCheck >= CumulativeAmount && ActionCheck < (CumulativeAmount + AdditionalAmount))
                {
                    m_Creature = pair.Key;

                    return;
                }

                CumulativeAmount += AdditionalAmount;
            }
        }

        public string GetCreatureName(CreatureType creatureType)
        {
            string name = "";

            switch (creatureType)
            {
                //Undead
                case CreatureType.BoneMagi: name = "bone magi"; break;
                case CreatureType.Lich: name = "lich"; break;
                case CreatureType.Mummy: name = "mummy"; break;
                case CreatureType.RottingCorpse: name = "rotting corpse"; break;
                case CreatureType.SkeletalKnight: name = "skeletal knight"; break;
                case CreatureType.Skeleton: name = "skeleton"; break;
                case CreatureType.Spectre: name = "spectre"; break;
                case CreatureType.VampireCountess: name = "vampire countess"; break;
                case CreatureType.VampireThrall: name = "vampire thrall"; break;
                case CreatureType.Zombie: name = "zombie"; break;
            }

            return name;
        }

        public Type GetCreatureType(CreatureType creatureType)
        {
            Type type = null;

            switch (creatureType)
            {
                //Undead
                case CreatureType.BoneMagi: type = typeof(HenchmanBoneMagi); break;
                case CreatureType.Lich: type = typeof(HenchmanLich); break;
                case CreatureType.Mummy: type = typeof(HenchmanMummy); break;
                case CreatureType.RottingCorpse: type = typeof(HenchmanRottingCorpse); break;
                case CreatureType.SkeletalKnight: type = typeof(HenchmanSkeletalKnight); break;
                case CreatureType.Skeleton: type = typeof(HenchmanSkeleton); break;
                case CreatureType.Spectre: type = typeof(HenchmanSpectre); break;
                case CreatureType.VampireCountess: type = typeof(HenchmanVampireCountess); break;
                case CreatureType.VampireThrall: type = typeof(HenchmanVampireThrall); break;
                case CreatureType.Zombie: type = typeof(HenchmanZombie); break;
            }

            return type;
        }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, Name);
            LabelTo(from, "(" + GetCreatureName(m_Creature) + ")");
            LabelTo(from, "[bonus exp: " + m_Experience.ToString() + "]");            
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

            if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("That must be in your backpack for you to use it.");
                return;
            }

            Type creatureType = GetCreatureType(m_Creature);

            if (creatureType == null)
                return;

            BaseHenchman henchman = (BaseHenchman)Activator.CreateInstance(creatureType);

            if (henchman == null)
                return;

            if (DateTime.UtcNow < player.LastCombatTime + CombatDelay)
            {
                DateTime cooldown = player.LastCombatTime + CombatDelay;

                string nextActivationAllowed = Utility.CreateTimeRemainingString(DateTime.UtcNow, cooldown, false, false, false, true, true);

                player.SendMessage("You have been in combat too recently and must wait another " + nextActivationAllowed + " before you may use this.");
                henchman.Delete();

                return;
            }

            if (DateTime.UtcNow < player.LastPlayerCombatTime + CombatPvPDelay)
            {
                DateTime cooldown = player.LastPlayerCombatTime + CombatPvPDelay;

                string nextActivationAllowed = Utility.CreateTimeRemainingString(DateTime.UtcNow, cooldown, false, false, false, true, true);

                player.SendMessage("You have been in combat with another player too recently and must wait another " + nextActivationAllowed + " before you may use this.");
                henchman.Delete();

                return;
            }

            if (player.Skills.Begging.Value < henchman.MinTameSkill)
            {
                player.SendMessage("You do not have the neccessary Begging skill required to create and command that creature.");
                henchman.Delete();
                
                return;
            }

            if (player.Skills.Camping.Value < henchman.MinTameSkill)
            {
                player.SendMessage("You do not have the neccessary Camping skill required to create and command that creature.");
                henchman.Delete();

                return;
            }

            if (player.Followers + henchman.ControlSlots > player.FollowersMax)
            {
                player.SendMessage("You must have " + henchman.ControlSlots.ToString() + " free control slots in order to create and control that creature.");
                henchman.Delete();
                
                return;
            }

            henchman.MoveToWorld(player.Location, player.Map);

            Timer.DelayCall(TimeSpan.FromSeconds(.1), delegate
            {
                if (player == null) return;
                if (player.Deleted) return;
                if (henchman == null) return;
                if (henchman.Deleted) return;

                henchman.Owners.Add(player);
                henchman.TimesTamed++;
                henchman.SetControlMaster(player);
                henchman.IsBonded = true;
                henchman.OwnerAbandonTime = DateTime.UtcNow + henchman.AbandonDelay;
                henchman.ResurrectionsRemaining = 2;

                henchman.Experience = Experience;

                if (henchman.recruitSpeech.Length > 0)
                    henchman.Say(henchman.recruitSpeech[Utility.Random(henchman.recruitSpeech.Length - 1)]);

                henchman.Hits = henchman.HitsMax;
                henchman.Stam = henchman.StamMax;
                henchman.Mana = henchman.ManaMax;

                henchman.PlaySound(henchman.GetIdleSound());

                player.SendMessage("You recruit the creature and they bond to you. They may be resurrected " + henchman.ResurrectionsRemaining.ToString() + " times before they fade from creation.");
            });
            
            Delete();
        }
        
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version

            writer.Write((int)m_Creature);
            writer.Write(m_Experience);           
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

            if (version >= 0)
            {
                m_Creature = (CreatureType)reader.ReadInt();
                m_Experience = reader.ReadInt();                
            }
		}
    }
}