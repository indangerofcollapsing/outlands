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
    public class HenchmanKnight : BaseHenchman
	{
        public override string[] recruitSpeech
        {
            get
            {
                return new string[] {   "I hope to serve thee with courage and honor.",
                                                                                };
            }
        }

        public override string[] idleSpeech
        {
            get
            {
                return new string[] {       "By your command, I shall serve thee faithfully.",
                                                                                "Protect the weak. Shepherd the innocent.",
                                                                                "*whistles happily*",
                                                                                "*inspects armor*" 
                                                                                };
            }
        }

        public override string[] combatSpeech
        {
            get
            {
                return new string[] {     "Courage amongst all.",
                                                                                "In battle, we prove our worth.",
                                                                                "Loyalty until death. I serve with honor.",
                                                                                "You are no match for my skill." 
                                                                                };
            }
        }

        public override HenchmanGroupType HenchmanGroup { get { return HenchmanGroupType.Honorable; } }

		[Constructable]
		public HenchmanKnight() : base()
		{
            SpeechHue = Utility.RandomPinkHue();

            Title = "the knight";                

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

            Tameable = true;
            ControlSlots = 2;
            MinTameSkill = 100;
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
        public override int TamedBaseVirtualArmor { get { return 100; } }

        public override void GenerateItems()
        {
            int colorTheme = Utility.RandomDyedHue();
            int customTheme = Utility.RandomMetalHue();

            switch (Utility.RandomMinMax(1, 5))
            {
                case 1: m_Items.Add(new ChainCoif() { LootType = LootType.Blessed }); break;
                case 2: m_Items.Add(new CloseHelm() { LootType = LootType.Blessed }); break;
                case 3: m_Items.Add(new NorseHelm() { LootType = LootType.Blessed }); break;
                case 4: m_Items.Add(new RingmailCoif() { LootType = LootType.Blessed }); break;
            }

            m_Items.Add(new Boots() { LootType = LootType.Blessed });
            m_Items.Add(new ChainChest() { LootType = LootType.Blessed });
            m_Items.Add(new RingmailGloves() { LootType = LootType.Blessed });
            m_Items.Add(new RingmailArms() { LootType = LootType.Blessed });
            
            m_Items.Add(new Kilt(colorTheme) { LootType = LootType.Blessed });

            switch (Utility.RandomMinMax(1, 5))
            {
                case 1: { m_Items.Add(new VikingSword() { LootType = LootType.Blessed }); m_Items.Add(new MetalKiteShield() {Hue = colorTheme, LootType = LootType.Blessed }); break; }
                case 2: { m_Items.Add(new Broadsword() { LootType = LootType.Blessed }); m_Items.Add(new MetalKiteShield() { Hue = colorTheme, LootType = LootType.Blessed }); break; }
                case 3: { m_Items.Add(new Maul() { LootType = LootType.Blessed }); m_Items.Add(new MetalKiteShield() { Hue = colorTheme, LootType = LootType.Blessed }); break; }
                case 4: { m_Items.Add(new Axe() { LootType = LootType.Blessed }); break; }
                case 5: { m_Items.Add(new WarHammer() { LootType = LootType.Blessed }); break; }
            }

            Utility.AssignRandomHair(this, Utility.RandomHairHue());
        }

        public override void SetUniqueAI()
        {
            PvPMeleeDamageScalar = BaseCreature.BasePvPMeleeDamageScalar * BaseCreature.BasePvPHenchmenDamageScalar;
            PvPAbilityDamageScalar = BaseCreature.BasePvPAbilityDamageScalar * BaseCreature.BasePvPHenchmenDamageScalar;
        }

        public override void SetTamedAI()
        {
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            double effectChance = .25;

            if (Controlled && ControlMaster != null)
            {
                if (ControlMaster is PlayerMobile)
                {
                    if (defender is PlayerMobile)
                        effectChance = .10;
                    else
                        effectChance = .25;
                }
            }

            SpecialAbilities.CourageSpecialAbility(effectChance, this, defender, .05, 10, -1, true, "", "", "draws upon courage*");
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);

            double effectChance = .25;

            if (Controlled && ControlMaster != null)
            {
                if (ControlMaster is PlayerMobile)
                {
                    if (attacker is PlayerMobile)
                        effectChance = .10;
                    else
                        effectChance = .25;
                }
            }

            SpecialAbilities.FortitudeSpecialAbility(effectChance, attacker, this, 50, 10, -1, true, "They draw upon fortitude from your attack.", "", "draws upon fortitude*"); 
        }
        
        public HenchmanKnight(Serial serial): base(serial)
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
