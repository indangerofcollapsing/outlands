using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Multis;
using Server.Misc;
using Server.Network;
using Server.Mobiles;

namespace Server
{
    [CorpseName("a failed experiment corpse")]
    public class UOACZFailedExperiment : UOACZBaseUndead
	{
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

        public override BonePileType BonePile { get { return BonePileType.Small; } }

        public override int DifficultyValue { get { return 3; } }

		[Constructable]
		public UOACZFailedExperiment() : base()
		{
            Name = "a failed experiment";
            Body = 305;
            BaseSoundID = 471;

            SetStr(75);
            SetDex(25);
            SetInt(25);

            SetHits(200);

            SetDamage(9, 18);

            SetSkill(SkillName.Wrestling, 55);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

            Fame = 4000;
            Karma = -4000;                   
		}

        public override void SetUniqueAI()
        {
            base.SetUniqueAI();
            
            DictCombatTargetingWeight[CombatTargetingWeight.EasiestToHit] = 5;
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);
            
            double effectChance = .2;

            SpecialAbilities.EnrageSpecialAbility(effectChance, attacker, this, .2, 20, -1, true, "Your attack enrages the target.", "", "*becomes enraged*");            
        }

        public override int GetAngerSound() { return 0x584; }
        public override int GetIdleSound() { return 0x383; }
        public override int GetAttackSound() { return 0x382; }
        public override int GetHurtSound() { return 0x385; }
        public override int GetDeathSound() { return 0x455; }

        public UOACZFailedExperiment(Serial serial): base(serial)
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
