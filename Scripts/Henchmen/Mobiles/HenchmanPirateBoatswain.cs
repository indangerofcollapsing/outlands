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
    public class HenchmanPirateBoatswain : BaseHenchman
	{
        public override string[] recruitSpeech  { get { return new string[] {   "Aye, you'll be needin' me to do yer dirty work, I imagin'.",
                                                                                };
            }
        }

        public override string[] idleSpeech { get { return new string[] {       "I'll keelhaul any man er woman I see dozin' off!",
                                                                                "Get off yer' bones ye lazy maggots and get to work!",
                                                                                "*adjusts hook*",
                                                                                "*sneers*" 
                                                                                };}}

        public override string[] combatSpeech { get { return new string[] {     "Any man that runs get's a blade 'imself!",
                                                                                "Give'em hell and take all they 'ave!",
                                                                                "Don't just stand there, stab somethin'!",
                                                                                "I'll be damned if I'll die here today among ye sorry lot!" 
                                                                                };}}

        public override HenchmanGroupType HenchmanGroup { get { return HenchmanGroupType.Pirate; } }

		[Constructable]
		public HenchmanPirateBoatswain() : base()
		{
            SpeechHue = Utility.RandomRedHue();

            Title = "the boatswain";

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

            Tamable = true;
            ControlSlots = 3;
            MinTameSkill = 100;
		}        

        //Animal Lore Display Info
        public override int TamedItemId { get { return 8454; } }
        public override int TamedItemHue { get { return 0; } }
        public override int TamedItemXOffset { get { return 5; } }
        public override int TamedItemYOffset { get { return 0; } }

        //Dynamic Stats and Skills (Scale Up With Creature XP)
        public override int TamedBaseMaxHits { get { return 300; } }
        public override int TamedBaseMinDamage { get { return 24; } }
        public override int TamedBaseMaxDamage { get { return 26; } }

        public override double TamedBaseWrestling { get { return 95; } }
        public override double TamedBaseArchery { get { return 95; } }
        public override double TamedBaseFencing { get { return 95; } }
        public override double TamedBaseMacing { get { return 95; } }
        public override double TamedBaseSwords { get { return 95; } }

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
               
            m_Items.Add(new ThighBoots() { LootType = LootType.Blessed });
            m_Items.Add(new StuddedLegs() { LootType = LootType.Blessed });
            m_Items.Add(new PlateGorget() { LootType = LootType.Blessed });
            m_Items.Add(new StuddedArms() { LootType = LootType.Blessed });

            m_Items.Add(new PlateGloves() { Hue = customTheme, LootType = LootType.Blessed });
            m_Items.Add(new Doublet(colorTheme) { LootType = LootType.Blessed });
            m_Items.Add(new Bandana(colorTheme) { LootType = LootType.Blessed });

            switch (Utility.RandomMinMax(1, 6))
            {
                case 1: { m_Items.Add(new Cutlass() { LootType = LootType.Blessed }); m_Items.Add(new MetalShield() { LootType = LootType.Blessed }); break; }
                case 2: { m_Items.Add(new Scimitar() { LootType = LootType.Blessed }); m_Items.Add(new MetalShield() { LootType = LootType.Blessed }); break; }
                case 3: { m_Items.Add(new Maul() { LootType = LootType.Blessed }); m_Items.Add(new MetalShield() { LootType = LootType.Blessed }); break; }
                case 4: { m_Items.Add(new WarFork() { LootType = LootType.Blessed }); m_Items.Add(new MetalShield() { LootType = LootType.Blessed }); break; }
                case 5: { m_Items.Add(new BlackStaff() { LootType = LootType.Blessed }); break; }
                case 6: { m_Items.Add(new DoubleAxe() { LootType = LootType.Blessed }); break; }
            }

            Crossbow bow = new Crossbow();
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

            CombatSpecialActionMinDelay = 30;
            CombatSpecialActionMaxDelay = 50;

            CombatHealActionMinDelay = 30;
            CombatHealActionMaxDelay = 50;

            PvPMeleeDamageScalar = BaseCreature.BasePvPMeleeDamageScalar * BaseCreature.BasePvPHenchmenDamageScalar;
            PvPAbilityDamageScalar = BaseCreature.BasePvPAbilityDamageScalar * BaseCreature.BasePvPHenchmenDamageScalar;
        }
        
        public HenchmanPirateBoatswain(Serial serial): base(serial)
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
