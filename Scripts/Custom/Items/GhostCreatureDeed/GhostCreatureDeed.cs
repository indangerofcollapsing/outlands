using System;
using Server;
using Server.Regions;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;
using Server.Multis;
using System.Collections;
using System.Collections.Generic;

namespace Server.Custom
{    
    public class GhostCreatureDeed : Item
    {
        private Type m_CreatureType;
        [CommandProperty(AccessLevel.GameMaster)]
        public Type CreatureType
        {
            get { return m_CreatureType; }
            set { m_CreatureType = value; }
        }

        private double m_TamingSkill = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public double TamingSkill
        {
            get { return m_TamingSkill; }
            set { m_TamingSkill = value; }
        }

        private int m_ControlSlots = 1;
        [CommandProperty(AccessLevel.GameMaster)]
        public int ControlSlots
        {
            get { return m_ControlSlots; }
            set { m_ControlSlots = value; }
        }

        private int m_Experience;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Experience
        {
            get { return m_Experience; }
            set { m_Experience = value; }
        }

        public TimeSpan MinLastCombatDelay = TimeSpan.FromMinutes(10); //Last Time Being in Combat

        [Constructable]
        public GhostCreatureDeed(): base(0x14F0)
		{
            Name = "a ghost creature deed";

            Weight = 0.1;
            Hue = 2500;

            m_Experience = Utility.Random(1, 250);

            m_CreatureType = GetRandomType();
            ConfigureCreatureInfo();
		}

        public GhostCreatureDeed(Serial serial): base(serial)
		{
		}

        public Type GetRandomType()
        {
            Type type = typeof(Dog);

            List<Type> m_ValidTamedCreatureTypes = new List<Type>();

            m_ValidTamedCreatureTypes.Add(typeof(Alligator));
            m_ValidTamedCreatureTypes.Add(typeof(AncientWinterWyrm));
            m_ValidTamedCreatureTypes.Add(typeof(AncientWyrm));
            m_ValidTamedCreatureTypes.Add(typeof(ArcaneDragon));
            m_ValidTamedCreatureTypes.Add(typeof(ArcaneDrake));
            m_ValidTamedCreatureTypes.Add(typeof(ArmoredCrab));
            m_ValidTamedCreatureTypes.Add(typeof(ArmoredPackBear));
            m_ValidTamedCreatureTypes.Add(typeof(Basilisk));
            m_ValidTamedCreatureTypes.Add(typeof(BayingHound));
            m_ValidTamedCreatureTypes.Add(typeof(Bird));
            m_ValidTamedCreatureTypes.Add(typeof(Bison));
            m_ValidTamedCreatureTypes.Add(typeof(BlackBear));
            m_ValidTamedCreatureTypes.Add(typeof(Bloodcat));
            m_ValidTamedCreatureTypes.Add(typeof(Boar));
            m_ValidTamedCreatureTypes.Add(typeof(BrownBear));
            m_ValidTamedCreatureTypes.Add(typeof(Bull));
            m_ValidTamedCreatureTypes.Add(typeof(BullFrog));
            m_ValidTamedCreatureTypes.Add(typeof(Bullvore));
            m_ValidTamedCreatureTypes.Add(typeof(BurrowBeetle));
            m_ValidTamedCreatureTypes.Add(typeof(Caribou));
            m_ValidTamedCreatureTypes.Add(typeof(Cat));
            m_ValidTamedCreatureTypes.Add(typeof(Chicken));
            m_ValidTamedCreatureTypes.Add(typeof(ChromaticCrawler));
            m_ValidTamedCreatureTypes.Add(typeof(ChromaticDragon));
            m_ValidTamedCreatureTypes.Add(typeof(Cockatrice));
            m_ValidTamedCreatureTypes.Add(typeof(CoralSnake));
            m_ValidTamedCreatureTypes.Add(typeof(CorrosiveSlime));
            m_ValidTamedCreatureTypes.Add(typeof(Cougar));
            m_ValidTamedCreatureTypes.Add(typeof(Cow));
            m_ValidTamedCreatureTypes.Add(typeof(Crab));
            m_ValidTamedCreatureTypes.Add(typeof(DarkWisp));
            m_ValidTamedCreatureTypes.Add(typeof(Deepstalker));
            m_ValidTamedCreatureTypes.Add(typeof(DesertOstard));
            m_ValidTamedCreatureTypes.Add(typeof(DireWolf));
            m_ValidTamedCreatureTypes.Add(typeof(Dog));
            m_ValidTamedCreatureTypes.Add(typeof(Dragon));
            m_ValidTamedCreatureTypes.Add(typeof(DragonWhelp));
            m_ValidTamedCreatureTypes.Add(typeof(Drake));
            m_ValidTamedCreatureTypes.Add(typeof(DrakeWhelp));
            m_ValidTamedCreatureTypes.Add(typeof(Eagle));
            m_ValidTamedCreatureTypes.Add(typeof(Ferret));
            m_ValidTamedCreatureTypes.Add(typeof(FireBeetle));
            m_ValidTamedCreatureTypes.Add(typeof(ForestOstard));
            m_ValidTamedCreatureTypes.Add(typeof(FrostOoze));
            m_ValidTamedCreatureTypes.Add(typeof(GiantBat));
            m_ValidTamedCreatureTypes.Add(typeof(GiantCoralSnake));
            m_ValidTamedCreatureTypes.Add(typeof(GiantRat));
            m_ValidTamedCreatureTypes.Add(typeof(GiantSerpent));
            m_ValidTamedCreatureTypes.Add(typeof(GiantSpider));
            m_ValidTamedCreatureTypes.Add(typeof(GiantToad));
            m_ValidTamedCreatureTypes.Add(typeof(Goat));
            m_ValidTamedCreatureTypes.Add(typeof(Gorilla));
            m_ValidTamedCreatureTypes.Add(typeof(GreaterLizard));
            m_ValidTamedCreatureTypes.Add(typeof(GreatHart));
            m_ValidTamedCreatureTypes.Add(typeof(GreyWolf));
            m_ValidTamedCreatureTypes.Add(typeof(GrizzlyBear));
            m_ValidTamedCreatureTypes.Add(typeof(Guar));
            m_ValidTamedCreatureTypes.Add(typeof(HellHound));
            m_ValidTamedCreatureTypes.Add(typeof(Hind));
            m_ValidTamedCreatureTypes.Add(typeof(Horse));
            m_ValidTamedCreatureTypes.Add(typeof(IceLizard));
            m_ValidTamedCreatureTypes.Add(typeof(IceSerpent));
            m_ValidTamedCreatureTypes.Add(typeof(IceSkitter));
            m_ValidTamedCreatureTypes.Add(typeof(Imp));
            m_ValidTamedCreatureTypes.Add(typeof(JackRabbit));
            m_ValidTamedCreatureTypes.Add(typeof(LavaLizard));
            m_ValidTamedCreatureTypes.Add(typeof(Llama));
            m_ValidTamedCreatureTypes.Add(typeof(Locust));
            m_ValidTamedCreatureTypes.Add(typeof(Mongbat));
            m_ValidTamedCreatureTypes.Add(typeof(MountainGoat));
            m_ValidTamedCreatureTypes.Add(typeof(Nightmare));
            m_ValidTamedCreatureTypes.Add(typeof(PackHorse));
            m_ValidTamedCreatureTypes.Add(typeof(PackLlama));
            m_ValidTamedCreatureTypes.Add(typeof(Panther));
            m_ValidTamedCreatureTypes.Add(typeof(Parrot));
            m_ValidTamedCreatureTypes.Add(typeof(Pig));
            m_ValidTamedCreatureTypes.Add(typeof(PlagueRat));
            m_ValidTamedCreatureTypes.Add(typeof(PolarBear));
            m_ValidTamedCreatureTypes.Add(typeof(Rabbit));
            m_ValidTamedCreatureTypes.Add(typeof(Rat));
            m_ValidTamedCreatureTypes.Add(typeof(RockSpider));
            m_ValidTamedCreatureTypes.Add(typeof(Salamander));
            m_ValidTamedCreatureTypes.Add(typeof(Scorpion));
            m_ValidTamedCreatureTypes.Add(typeof(ScorpionHatchling));
            m_ValidTamedCreatureTypes.Add(typeof(Sewerrat));
            m_ValidTamedCreatureTypes.Add(typeof(ShadowDragon));
            m_ValidTamedCreatureTypes.Add(typeof(ShadowDrake));
            m_ValidTamedCreatureTypes.Add(typeof(Sheep));
            m_ValidTamedCreatureTypes.Add(typeof(SilverSerpent));
            m_ValidTamedCreatureTypes.Add(typeof(SkeletalDrake));
            m_ValidTamedCreatureTypes.Add(typeof(Skitter));
            m_ValidTamedCreatureTypes.Add(typeof(SkitteringHopper));
            m_ValidTamedCreatureTypes.Add(typeof(Slime));
            m_ValidTamedCreatureTypes.Add(typeof(Snake));
            m_ValidTamedCreatureTypes.Add(typeof(SnowLeopard));
            m_ValidTamedCreatureTypes.Add(typeof(Sphinx));
            m_ValidTamedCreatureTypes.Add(typeof(SwampCrawler));
            m_ValidTamedCreatureTypes.Add(typeof(TimberWolf));
            m_ValidTamedCreatureTypes.Add(typeof(VampireBat));
            m_ValidTamedCreatureTypes.Add(typeof(VoidSlime));
            m_ValidTamedCreatureTypes.Add(typeof(Walrus));
            m_ValidTamedCreatureTypes.Add(typeof(WhiteDrake));
            m_ValidTamedCreatureTypes.Add(typeof(WhiteWolf));
            m_ValidTamedCreatureTypes.Add(typeof(WhiteWyrm));
            m_ValidTamedCreatureTypes.Add(typeof(Wisp));
            m_ValidTamedCreatureTypes.Add(typeof(Wyvern));
            m_ValidTamedCreatureTypes.Add(typeof(WyvernHatchling));

            return m_ValidTamedCreatureTypes[Utility.RandomMinMax(0, m_ValidTamedCreatureTypes.Count - 1)];
        }

        public void ConfigureCreatureInfo()
        {
            if (m_CreatureType == null)
                Delete();

            BaseCreature bc_Creature = (BaseCreature)Activator.CreateInstance(m_CreatureType);

            if (bc_Creature == null)
                Delete();

            m_TamingSkill = bc_Creature.MinTameSkill;
            m_ControlSlots = bc_Creature.ControlSlots;

            Name = "a ghost creature deed for " + bc_Creature.RawName + "";

            bc_Creature.Delete();
        }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, Name);
            LabelTo(from, "(bonus exp: " + m_Experience.ToString() + ")");
            LabelTo(from, "[taming: " + m_TamingSkill.ToString() + " slots: " + m_ControlSlots.ToString() + "]");
        }

        public override void OnDoubleClick(Mobile from)
        {
            PlayerMobile pm_From = from as PlayerMobile;

            if (pm_From == null)
                return;

            if (!pm_From.Alive)
            {
                pm_From.SendMessage("You must be alive to use that.");
                return;
            }

            if (!IsChildOf(pm_From.Backpack))
            {
                pm_From.SendMessage("that must be in your backpack for you to use it.");
                return;
            }

            if (pm_From.Skills.AnimalTaming.Value < m_TamingSkill || pm_From.Skills.AnimalLore.Value < m_TamingSkill)
            {
                pm_From.SendMessage("You do not have the neccesary Animal Taming and Animal Lore neccesary to control that.");
                return;
            }

            if (pm_From.Followers + m_ControlSlots > pm_From.FollowersMax)
            {
                pm_From.SendMessage("You do not have enough control slots available to control that.");
                return;
            }

            if (pm_From.LastCombatTime + MinLastCombatDelay > DateTime.UtcNow)
            {
                pm_From.SendMessage("You have been in combat too recently to activate that. You must wait another " + Utility.CreateTimeRemainingString(DateTime.UtcNow, pm_From.LastCombatTime + MinLastCombatDelay, false, false, false, true, true) + ".");
                return;
            }            

            BaseCreature bc_Creature = (BaseCreature)Activator.CreateInstance(m_CreatureType);

            if (bc_Creature == null)
                return;

            bc_Creature.Owners.Add(pm_From);
            bc_Creature.TimesTamed++;
            bc_Creature.SetControlMaster(pm_From);
            bc_Creature.IsBonded = true;
            bc_Creature.BondingBegin = DateTime.MinValue;
            bc_Creature.OwnerAbandonTime = DateTime.UtcNow + bc_Creature.AbandonDelay;
            bc_Creature.ResurrectionsRemaining = 2;

            bc_Creature.Hue = 25000;

            bc_Creature.GenerateTamedScalars();
            bc_Creature.Experience = m_Experience;

            bc_Creature.Hits = bc_Creature.HitsMax;
            bc_Creature.Stam = bc_Creature.StamMax;
            bc_Creature.Mana = bc_Creature.ManaMax;

            bc_Creature.MoveToWorld(pm_From.Location, pm_From.Map);
            bc_Creature.PlaySound(bc_Creature.GetIdleSound());

            pm_From.SendMessage("You summon the creature and it immediately bonds with you. It may be resurrected " + bc_Creature.ResurrectionsRemaining.ToString() + " times before it fades from creation.");

            Delete();
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); //version

            writer.Write(m_CreatureType.ToString());
            writer.Write(m_TamingSkill);
            writer.Write(m_ControlSlots);
            writer.Write(m_Experience);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

            if (version >= 0)
            {
                string sCreatureType = reader.ReadString();
                m_TamingSkill = reader.ReadDouble();
                m_ControlSlots = reader.ReadInt();
                m_Experience = reader.ReadInt();

                Type type = Type.GetType(sCreatureType);

                if (type != null)
                {
                    m_CreatureType = type;
                    ConfigureCreatureInfo();
                }
            }
		}
    }
}