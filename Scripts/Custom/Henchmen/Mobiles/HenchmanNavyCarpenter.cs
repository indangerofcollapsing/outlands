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
    public class HenchmanNavyCarpenter : BaseHenchman
	{
        public override string[] recruitSpeech  { get { return new string[] {   "Yar! Sign me up fer yer crew!",
                                                                                };
            }
        }

        public override string[] idleSpeech
        {
            get
            {
                return new string[] {       "*puts nail between teeth*",
                                                                                "*inspects board*",
                                                                                "*twirls hammer*",
                                                                                "*wipes sweat off brow*"
                                                                                };
            }
        }

        public override string[] combatSpeech
        {
            get
            {
                return new string[] {     "Ey! Watch the woodwork!",
                                                                                "Drat! I just fixed that!",
                                                                                "We're going to need more nails!",
                                                                                "I'm paid to hammer boards, not heads!" 
                                                                                };
            }
        }

        public override HenchmanGroupType HenchmanGroup { get { return HenchmanGroupType.Navy; } }

		[Constructable]
		public HenchmanNavyCarpenter() : base()
		{
            SpeechHue = Utility.RandomBlueHue();

            Title = "the carpenter";             

            SetStr(50);
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
            MinTameSkill = 120;
		}        

        //Animal Lore Display Info
        public override int TamedItemId { get { return 8454; } }
        public override int TamedItemHue { get { return 0; } }
        public override int TamedItemXOffset { get { return 5; } }
        public override int TamedItemYOffset { get { return 0; } }

        //Dynamic Stats and Skills (Scale Up With Creature XP)
        public override int TamedBaseMaxHits { get { return 250; } }
        public override int TamedBaseMinDamage { get { return 16; } }
        public override int TamedBaseMaxDamage { get { return 18; } }

        public override double TamedBaseWrestling { get { return 90; } }
        public override double TamedBaseArchery { get { return 90; } }
        public override double TamedBaseFencing { get { return 90; } }
        public override double TamedBaseMacing { get { return 90; } }
        public override double TamedBaseSwords { get { return 90; } }

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
        public override int TamedBaseVirtualArmor { get { return 50; } }

        public override void GenerateItems()
        {
            int colorTheme = Utility.RandomDyedHue();
            int customTheme = 0;

            m_Items.Add(new Shoes() { LootType = LootType.Blessed });            
            m_Items.Add(new LeatherGloves() { LootType = LootType.Blessed });
            m_Items.Add(new LeatherArms() { LootType = LootType.Blessed });

            m_Items.Add(new HalfApron(Utility.RandomNeutralHue()) { LootType = LootType.Blessed });
            m_Items.Add(new ShortPants(Utility.RandomNeutralHue()) { LootType = LootType.Blessed });
            m_Items.Add(new SkullCap(colorTheme) { LootType = LootType.Blessed });
            
            m_Items.Add(new Hatchet() { LootType = LootType.Blessed });

            Bow bow = new Bow();
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

            CombatSpecialActionMinDelay = 40;
            CombatSpecialActionMaxDelay = 60;

            CombatHealActionMinDelay = 40;
            CombatHealActionMaxDelay = 60;

            PvPMeleeDamageScalar = BaseCreature.BasePvPMeleeDamageScalar * BaseCreature.BasePvPHenchmenDamageScalar;
            PvPAbilityDamageScalar = BaseCreature.BasePvPAbilityDamageScalar * BaseCreature.BasePvPHenchmenDamageScalar;
        }

        public void AssistRepair()
        {
            if (!Alive)
                return;            

            Effects.PlaySound(Location, Map, 0x23D);
            Animate(12, 5, 1, true, false, 0);

            double repairInterval = 3.5;

            AIObject.NextMove = DateTime.UtcNow + TimeSpan.FromSeconds(repairInterval);
            NextCombatTime = DateTime.UtcNow + TimeSpan.FromSeconds(repairInterval);

            NextSpellTime = NextSpellTime + TimeSpan.FromSeconds(repairInterval);
            NextCombatHealActionAllowed = NextCombatHealActionAllowed + TimeSpan.FromSeconds(repairInterval);
            NextCombatSpecialActionAllowed = NextCombatSpecialActionAllowed + TimeSpan.FromSeconds(repairInterval);
            NextCombatEpicActionAllowed = NextCombatEpicActionAllowed + TimeSpan.FromSeconds(repairInterval);
        }
        
        public HenchmanNavyCarpenter(Serial serial): base(serial)
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
