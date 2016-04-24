using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a blood troll corpse" )]
	public class BloodTroll : BaseCreature
	{
		[Constructable]
		public BloodTroll () : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a blood troll";
			Body = Utility.RandomList( 53, 54 );
			BaseSoundID = 461;
			Hue = 2117;

            SetStr(100);
            SetDex(50);
            SetInt(25);

            SetHits(400);

            SetDamage(15, 30);

            SetSkill(SkillName.Wrestling, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 125);

            VirtualArmor = 25;

			Fame = 12500;
			Karma = -12500;			
		}

        public override void SetUniqueAI()
        {
            UniqueCreatureDifficultyScalar = 1.15;
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);
            
            int bloodItems = Utility.RandomMinMax(3, 6);

            for (int a = 0; a < bloodItems; a++)
            {
                Blood blood = new Blood();
                blood.ItemID = Utility.RandomList(4650, 4651, 4652, 4653, 4654, 4655);
                blood.MoveToWorld(new Point3D(defender.X + Utility.RandomMinMax(-1, 1), defender.Y + Utility.RandomMinMax(-1, 1), defender.Z), Map);
            }

            SpecialAbilities.BleedSpecialAbility(0.25, this, defender, DamageMax, 8.0, Utility.RandomList(0x5D9, 0x5DB), true, "", "Their attack causes you to bleed!", "-1");
        }

        public override bool CanRummageCorpses { get { return true; } }
        
		public BloodTroll( Serial serial ) : base( serial )
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
