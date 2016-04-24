using System;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.ContextMenus;
using Server.Multis;
using Server.Misc;
using Server.Network;
using Server.Mobiles;

namespace Server.Custom
{
    public class HenchmanPirateFirstMate : BaseHenchman
	{
        public override string[] recruitSpeech  { get { return new string[] {   "Har, har! We'll turn the seas red and bring them under the heels of our boots!",
                                                                                };
            }
        }

        public override string[] idleSpeech
        {
            get
            {
                return new string[] {       "Let none alive take the wind from our sails.",
                                            "Ho, ho! The sea's our mistress and she be a good lass to us.",
                                            "A pirate's takes what he wants and gives nothing back!",
                                            "Booty and plunder for us, misery and death to the rest!" 
                                            };
            }
        }

        public override string[] combatSpeech
        {
            get
            {
                return new string[] {     "Give em' what-for, men!",
                                          "Leave none alive!",
                                          "Gutless cowards, all of em!",
                                          "Har, har!" 
                                          };
            }
        }

        public override HenchmanGroupType HenchmanGroup { get { return HenchmanGroupType.Pirate ; } }

		[Constructable]
		public HenchmanPirateFirstMate() : base()
		{
            SpeechHue = Utility.RandomRedHue();

            Title = "the first mate";           

            SetStr(75);
            SetDex(50);
            SetInt(25);

            SetHits(200);

            SetDamage(6, 12);

            SetSkill(SkillName.Archery, 70);
            SetSkill(SkillName.Fencing, 70);
            SetSkill(SkillName.Swords, 70);
            SetSkill(SkillName.Macing, 70);

            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Parry, 5);

            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 25;

			Fame = 1000;
			Karma = -2000;

            Tameable = true;
            ControlSlots = 5;
            MinTameSkill = 120;
		}        

        //Animal Lore Display Info
        public override int TamedItemId { get { return 8454; } }
        public override int TamedItemHue { get { return 0; } }
        public override int TamedItemXOffset { get { return 5; } }
        public override int TamedItemYOffset { get { return 0; } }

        //Dynamic Stats and Skills (Scale Up With Creature XP)
        public override int TamedBaseMaxHits { get { return 400; } }
        public override int TamedBaseMinDamage { get { return 32; } }
        public override int TamedBaseMaxDamage { get { return 34; } }

        public override double TamedBaseWrestling { get { return 100; } }
        public override double TamedBaseArchery { get { return 100; } }
        public override double TamedBaseFencing { get { return 100; } }
        public override double TamedBaseMacing { get { return 100; } }
        public override double TamedBaseSwords { get { return 100; } }

        public override double TamedBaseEvalInt { get { return 0; } }

        //Static Stats and Skills (Do Not Scale Up With Creature XP)
        public override int TamedBaseStr { get { return 75; } }
        public override int TamedBaseDex { get { return 50; } }
        public override int TamedBaseInt { get { return 25; } }
        public override int TamedBaseMaxMana { get { return 0; } }
        public override double TamedBaseMagicResist { get { return 50; } }
        public override double TamedBaseMagery { get { return 0; } }
        public override double TamedBasePoisoning { get { return 0; } }
        public override double TamedBaseTactics { get { return 100; } }
        public override double TamedBaseMeditation { get { return 100; } }
        public override int TamedBaseVirtualArmor { get { return 100; } }

        public override void GenerateItems()
        {
            int colorTheme = Utility.RandomDyedHue();
            int customTheme = 2051;
           
            m_Items.Add(new PlateGloves() { LootType = LootType.Blessed });
            m_Items.Add(new PlateGorget() { LootType = LootType.Blessed });
            m_Items.Add(new StuddedArms() { LootType = LootType.Blessed });
            m_Items.Add(new ChainChest() { LootType = LootType.Blessed });

            m_Items.Add(new BodySash(customTheme) { LootType = LootType.Blessed });
            m_Items.Add(new Kilt(customTheme) { LootType = LootType.Blessed });          
            m_Items.Add(new PlateLegs() { Hue = customTheme, LootType = LootType.Blessed });
           
            m_Items.Add(new Bandana(colorTheme) { LootType = LootType.Blessed });

            switch (Utility.RandomMinMax(1, 6))
            {
                case 1: { m_Items.Add(new Cutlass() { LootType = LootType.Blessed }); m_Items.Add(new BronzeShield() { Hue = customTheme, LootType = LootType.Blessed }); break; }
                case 2: { m_Items.Add(new Scimitar() { LootType = LootType.Blessed }); m_Items.Add(new BronzeShield() { Hue = customTheme, LootType = LootType.Blessed }); break; }
                case 3: { m_Items.Add(new WarMace() { LootType = LootType.Blessed }); m_Items.Add(new BronzeShield() { Hue = customTheme, LootType = LootType.Blessed }); break; }
                case 4: { m_Items.Add(new WarFork() { LootType = LootType.Blessed }); m_Items.Add(new BronzeShield() { Hue = customTheme, LootType = LootType.Blessed }); break; }
                case 5: { m_Items.Add(new WarHammer() { LootType = LootType.Blessed }); break; }
                case 6: { m_Items.Add(new ExecutionersAxe() { LootType = LootType.Blessed }); break; }
            }

            HeavyCrossbow bow = new HeavyCrossbow();
            bow.LootType = LootType.Blessed;
            m_Items.Add(bow);

            Utility.AssignRandomHair(this, Utility.RandomHairHue());
        }

        public override bool CanSwitchWeapons { get { return true; } }

        public override void SetUniqueAI()
        {            
        }

        public override void SetTamedAI()
        {
            DictCombatFlee[CombatFlee.Flee10] = 0;
            DictCombatFlee[CombatFlee.Flee5] = 0;

            DictCombatAction[CombatAction.CombatSpecialAction] = 3;
            DictCombatAction[CombatAction.CombatHealSelf] = 1;

            DictCombatSpecialAction[CombatSpecialAction.ThrowShipBomb] = 1;

            DictCombatHealSelf[CombatHealSelf.PotionHealSelf50] = 1;
            DictCombatHealSelf[CombatHealSelf.PotionCureSelf] = 5;

            CombatSpecialActionMinDelay = 15;
            CombatSpecialActionMaxDelay = 25;

            CombatHealActionMinDelay = 15;
            CombatHealActionMaxDelay = 25;

            PvPMeleeDamageScalar = BaseCreature.BasePvPMeleeDamageScalar * BaseCreature.BasePvPHenchmenDamageScalar;
            PvPAbilityDamageScalar = BaseCreature.BasePvPAbilityDamageScalar * BaseCreature.BasePvPHenchmenDamageScalar;
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            double effectChance = .15;

            if (Controlled && ControlMaster != null)
            {
                if (ControlMaster is PlayerMobile)
                {
                    if (defender is PlayerMobile)
                        effectChance = .05;
                    else
                        effectChance = .15;
                }
            }

            SpecialAbilities.BleedSpecialAbility(effectChance, this, defender, DamageMax, 8.0, -1, true, "", "Their vicious attack causes you to bleed!", "-1");
        }
        
        public HenchmanPirateFirstMate(Serial serial): base(serial)
        {
        }    

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}
