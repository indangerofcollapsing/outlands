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
    public class HenchmanBoneMagi : BaseHenchman
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
		public HenchmanBoneMagi() : base()
		{
            Name = "Bone Magi";

            Body = 50;
            Hue = 2117;
            BaseSoundID = 451;

            SpeechHue = 2606;

            SetStr(50);
            SetDex(50);
            SetInt(75);

            SetHits(75);
            SetMana(1000);

            SetDamage(7, 14);

            SetSkill(SkillName.Wrestling, 40);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            SetSkill(SkillName.Magery, 50);
            SetSkill(SkillName.EvalInt, 50);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 25;

			Fame = 1000;
			Karma = 1000;

            Tameable = true;
            ControlSlots = 1;
            MinTameSkill = 75;
		}

        //Animal Lore Display Info
        public override int TamedItemId { get { return 9660; } }
        public override int TamedItemHue { get { return 2501; } }
        public override int TamedItemXOffset { get { return 15; } }
        public override int TamedItemYOffset { get { return 0; } }

        //Dynamic Stats and Skills (Scale Up With Creature XP)
        public override int TamedBaseMaxHits { get { return 125; } }
        public override int TamedBaseMinDamage { get { return 4; } }
        public override int TamedBaseMaxDamage { get { return 6; } }
        public override double TamedBaseWrestling { get { return 70; } }
        public override double TamedBaseEvalInt { get { return 50; } }

        //Static Stats and Skills (Do Not Scale Up With Creature XP)
        public override int TamedBaseStr { get { return 5; } }
        public override int TamedBaseDex { get { return 50; } }
        public override int TamedBaseInt { get { return 75; } }
        public override int TamedBaseMaxMana { get { return 1000; } }
        public override double TamedBaseMagicResist { get { return 100; } }
        public override double TamedBaseMagery { get { return 50; } }
        public override double TamedBasePoisoning { get { return 0; } }
        public override double TamedBaseTactics { get { return 100; } }
        public override double TamedBaseMeditation { get { return 125; } }
        public override int TamedBaseVirtualArmor { get { return 50; } }

        public override int PoisonResistance { get { return 5; } }
        
        public override void SetUniqueAI()
        {
        }

        public override void SetTamedAI()
        {
            AISubGroup = AISubGroupType.Mage2;
            UpdateAI(false);

            DictCombatRange[CombatRange.Withdraw] = 0;

            DictCombatSpell[CombatSpell.SpellDamage3] += 1;
            DictCombatSpell[CombatSpell.SpellDamage4] += 1;
            DictCombatSpell[CombatSpell.SpellDamage5] -= 1;
            DictCombatSpell[CombatSpell.SpellDamage6] -= 1;

            SpellDelayMin *= 1.33;
            SpellDelayMax *= 1.33;
        }

        public override void OnThink()
        {
            base.OnThink();
        }
        
        public HenchmanBoneMagi(Serial serial): base(serial)
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
