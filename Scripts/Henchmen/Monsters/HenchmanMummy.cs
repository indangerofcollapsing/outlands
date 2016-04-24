using System;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.ContextMenus;
using Server.Multis;
using Server.Misc;
using Server.Network;
using Server.Mobiles;
using Server.Spells;

namespace Server.Custom
{
    public class HenchmanMummy : BaseHenchman
	{
        public override string[] recruitSpeech  { get { return new string[] {   "",
                                                                                };
            }
        }

        public override string[] idleSpeech { get { return new string[] {       "",
                                                                                "",
                                                                                "",
                                                                                "" 
                                                                                };}}

        public override string[] combatSpeech { get { return new string[] {     "",
                                                                                "",
                                                                                "",
                                                                                "" 
                                                                                };}}

        public override HenchmanGroupType HenchmanGroup { get { return HenchmanGroupType.Undead; } }
        public override bool HenchmanHumanoid { get { return false; } }
        public override bool Recruitable { get { return false; } }

		[Constructable]
		public HenchmanMummy() : base()
		{
            Name = "Mummy";

            Body = 154;
            BaseSoundID = 0x27A;

            SpeechHue = 2606;

            SetStr(100);
            SetDex(25);
            SetInt(25);

            SetHits(400);

            SetDamage(15, 25);

            SetSkill(SkillName.Wrestling, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

            Fame = 600;
            Karma = -600;

			Fame = 1000;
			Karma = -2000;

            Tameable = true;
            ControlSlots = 3;
            MinTameSkill = 120;
		}

        //Dynamic Stats and Skills (Scale Up With Creature XP)
        public override int TamedItemId { get { return 9639; } }
        public override int TamedItemHue { get { return 0; } }
        public override int TamedItemXOffset { get { return 10; } }
        public override int TamedItemYOffset { get { return 0; } }

        public override int TamedBaseMaxHits { get { return 400; } }
        public override int TamedBaseMinDamage { get { return 24; } }
        public override int TamedBaseMaxDamage { get { return 26; } }
        public override double TamedBaseWrestling { get { return 100; } }
        public override double TamedBaseEvalInt { get { return 0; } }

        //Static Stats and Skills (Do Not Scale Up With Creature XP)
        public override int TamedBaseStr { get { return 5; } }
        public override int TamedBaseDex { get { return 25; } }
        public override int TamedBaseInt { get { return 5; } }
        public override int TamedBaseMaxMana { get { return 0; } }
        public override double TamedBaseMagicResist { get { return 75; } }
        public override double TamedBaseMagery { get { return 0; } }
        public override double TamedBasePoisoning { get { return 50; } }
        public override double TamedBaseTactics { get { return 100; } }
        public override double TamedBaseMeditation { get { return 0; } }
        public override int TamedBaseVirtualArmor { get { return 75; } }

        public override int PoisonResistance { get { return 5; } }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            double effectChance = .20;

            if (Controlled && ControlMaster != null)
            {
                if (ControlMaster is PlayerMobile)
                {
                    if (defender is PlayerMobile)
                        effectChance = .01;
                    else
                        effectChance = .80;
                }
            }

            BaseCreature bc_Defender = defender as BaseCreature;
            PlayerMobile pm_Defender = defender as PlayerMobile;

            double diseaseValue = defender.GetSpecialAbilityEntryValue(SpecialAbilityEffect.Disease);

            if (diseaseValue > 0)
                return;

            if (Utility.RandomDouble() <= effectChance)
            {
                for (int a = 0; a < 3; a++)
                {
                    Blood disease = new Blood();
                    disease.Hue = 2052;
                    disease.Name = "disease";

                    Point3D diseaseLocation = new Point3D(X + Utility.RandomList(-1, 1), Y + Utility.RandomList(-1, 1), Z);
                    SpellHelper.AdjustField(ref diseaseLocation, Map, 12, false);

                    disease.MoveToWorld(diseaseLocation, defender.Map);
                }

                SpecialAbilities.DiseaseSpecialAbility(1.0, this, defender, 20, 60, 0x62B, true, "", "They has infected you with a horrific disease!");
            }
        }
        
        public override void SetUniqueAI()
        {
        }

        public override void SetTamedAI()
        {   
        }

        public override void OnThink()
        {
            base.OnThink();
        }
        
        public HenchmanMummy(Serial serial): base(serial)
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
