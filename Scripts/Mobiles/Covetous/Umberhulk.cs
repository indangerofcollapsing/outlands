using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.ContextMenus;

namespace Server.Mobiles
{
	[CorpseName( "an umberhulk corpse" )]
	public class Umberhulk : BaseCreature
	{
		[Constructable]
		public Umberhulk() : base( AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
            Name = "an umberhulk";
            Body = 244;
            Hue = 0;
            
            SetStr(100);
            SetDex(50);
            SetInt(25);

            SetHits(500);

            SetDamage(15, 25);

            SetSkill(SkillName.Wrestling, 80);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 75);

            VirtualArmor = 75;

			Fame = 2000;
			Karma = -2000;
		}

        public override void SetUniqueAI()
        {           
            UniqueCreatureDifficultyScalar = 1.05;           
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            SpecialAbilities.StunSpecialAbility(.10, this, defender, .10, 10, -1, true, "", "Their attack stuns you!");
        }

		public override int GetAngerSound(){return 0x4E7;}
        public override int GetAttackSound(){return 0x63A;}
        public override int GetHurtSound(){return 0x4E9;}
        public override int GetDeathSound(){return 0x4E6;}
		public override int GetIdleSound(){return 0x4E7;}

        public Umberhulk(Serial serial): base(serial)
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