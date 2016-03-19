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
    public class HenchmanVampireCountess : BaseHenchman
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

        public override int AttackAnimation { get { return 6; } }
        public override int AttackFrames { get { return 6; } }

		[Constructable]
		public HenchmanVampireCountess() : base()
		{
            Name = "Vampire Countess";

            Body = 258;
            Hue = 2500;
            BaseSoundID = 0x4B0;

            SpeechHue = 2606;

            SetStr(100);
            SetDex(50);
            SetInt(100);

            SetHits(750);
            SetMana(2000);

            SetDamage(25, 35);

            SetSkill(SkillName.Wrestling, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 150);

            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 25;

			Fame = 1000;
			Karma = -2000;

            Tamable = true;
            ControlSlots = 4;
            MinTameSkill = 120;
		}

        //Animal Lore Display Info
        public override int TamedItemId { get { return 11652; } }
        public override int TamedItemHue { get { return 0; } }
        public override int TamedItemXOffset { get { return 5; } }
        public override int TamedItemYOffset { get { return -5; } }

        //Dynamic Stats and Skills (Scale Up With Creature XP)
        public override int TamedBaseMaxHits { get { return 400; } }
        public override int TamedBaseMinDamage { get { return 24; } }
        public override int TamedBaseMaxDamage { get { return 26; } }
        public override double TamedBaseWrestling { get { return 95; } }
        public override double TamedBaseEvalInt { get { return 75; } }

        //Static Stats and Skills (Do Not Scale Up With Creature XP)
        public override int TamedBaseStr { get { return 5; } }
        public override int TamedBaseDex { get { return 50; } }
        public override int TamedBaseInt { get { return 100; } }
        public override int TamedBaseMaxMana { get { return 1500; } }
        public override double TamedBaseMagicResist { get { return 150; } }
        public override double TamedBaseMagery { get { return 75; } }
        public override double TamedBasePoisoning { get { return 0; } }
        public override double TamedBaseTactics { get { return 100; } }
        public override double TamedBaseMeditation { get { return 150; } }
        public override int TamedBaseVirtualArmor { get { return 75; } }

        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        
        public override void SetUniqueAI()
        {
        }

        public override void SetTamedAI()
        {
            SetSubGroup(AISubgroup.MeleeMage3);
            UpdateAI(false);

            DictCombatSpell[CombatSpell.SpellDamage4] += 2;
            DictCombatSpell[CombatSpell.SpellDamage5] += 2;
            DictCombatSpell[CombatSpell.SpellDamage6] -= 2;
            DictCombatSpell[CombatSpell.SpellDamage7] -= 1; 
        }

        public override void OnThink()
        {
            base.OnThink();
        }

        public override int GetAngerSound() { return 0x370; }
        public override int GetIdleSound() { return 0x57D; }
        public override int GetAttackSound() { return 0x374; }
        public override int GetHurtSound() { return 0x375; }
        public override int GetDeathSound() { return 0x376; }
        
        public HenchmanVampireCountess(Serial serial): base(serial)
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
