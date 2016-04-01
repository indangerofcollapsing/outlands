using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a kuo-toa corpse" )]
	public class KuoToa : BaseCreature
	{
		[Constructable]
		public KuoToa() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
            Name = "a kuo-toa";

            Body = 240;
            BaseSoundID = 417;			

			SetStr( 50 );
			SetDex( 50 );
			SetInt( 25 );

			SetHits( 200 );

			SetDamage( 9, 18 );

            SetSkill( SkillName.Wrestling, 75);
			SetSkill( SkillName.Tactics, 100 );			

            SetSkill(SkillName.MagicResist, 25);

            SetSkill(SkillName.Healing, 75);
            SetSkill(SkillName.Veterinary, 75);

			Fame = 1500;
			Karma = -1500;

			VirtualArmor = 25;
		}        

        public override void SetUniqueAI()
        {
            CombatHealActionMinDelay = 10;
            CombatHealActionMaxDelay = 20;
        }
        
        public override bool OnBeforeDeath()
        {
            return base.OnBeforeDeath();
        }

        public override int GetAngerSound() { return 0x50B;}
        public override int GetAttackSound() { return 0x2AA;}
        public override int GetHurtSound(){ return 0x509;}
        public override int GetDeathSound(){return 0x508;}
        public override int GetIdleSound() { return 0x50A;}

		public override bool CanRummageCorpses{ get{ return true; } }

        public KuoToa(Serial serial): base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}
