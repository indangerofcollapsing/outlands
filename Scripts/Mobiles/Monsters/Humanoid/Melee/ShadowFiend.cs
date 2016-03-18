using System;
using System.Collections;
using Server.Items;
using Server.Regions;

namespace Server.Mobiles
{
	public class ShadowFiend : BaseCreature
	{
		public override bool DeleteCorpseOnDeath{ get{ return true; } }
        
		[Constructable]
		public ShadowFiend() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a shadow fiend";
			Body = 0xA8;			

            SetStr(75);
            SetDex(75);
            SetInt(25);

            SetHits(350);

            SetDamage(12, 18);

            SetSkill(SkillName.Wrestling, 85);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            SetSkill(SkillName.Hiding, 100);
            SetSkill(SkillName.Stealth, 100);

			Fame = 1000;
			Karma = -1000;
		}

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Global_AllowAbilities)
            {               
                Blood blood = new Blood();
                blood.Hue = 2051;
                blood.Name = "dark essence";
                blood.ItemID = Utility.RandomList(4650, 4651, 4652, 4653, 4654, 4655);
                blood.MoveToWorld(defender.Location, Map);
            }
        }

        public override bool OnBeforeDeath()
    {
      Effects.SendLocationEffect( Location, Map, 0x376A, 10, 1 );
            var goldItem = new Gold(ModifiedGoldWorth());
            goldItem.MoveToWorld(Location, Map);

      return base.OnBeforeDeath();
    }

		public override int GetIdleSound(){return 0x37A;}
		public override int GetAngerSound(){return 0x379;}
		public override int GetDeathSound(){return 0x381;}
		public override int GetAttackSound(){return 0x37F;}
		public override int GetHurtSound(){return 0x380;}

		public override bool CanRummageCorpses{ get{ return true; } }
        
		public ShadowFiend( Serial serial ) : base( serial )
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