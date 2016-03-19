using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Misc;
using Server.Spells;
using Server.Spells.Seventh;
using Server.Spells.Sixth;
using Server.Spells.Third;
using Server.Achievements;
namespace Server.Mobiles
{
	[CorpseName( "a bloodworm corpse" )]
	public class Bloodworm : BaseCreature
	{
		[Constructable]
		public Bloodworm() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
            Name = "a bloodworm";
			Body = 287;

			SetStr( 100 );
			SetDex( 50 );
			SetInt( 25 );

			SetHits( 5000 );
            SetStam( 2500 );

			SetDamage( 20, 30 );

            SetSkill(SkillName.Wrestling, 80);
            SetSkill(SkillName.Tactics, 100);

			SetSkill( SkillName.MagicResist, 50 );

			Fame = 10000;
			Karma = -10000;

			VirtualArmor = 25;
		}

        public override bool IsHighSeasBodyType { get { return true; } }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            SpecialAbilities.BleedSpecialAbility(.15, this, defender, DamageMax, 8.0, -1, true, "", "Their attack causes you to bleed!");
        }

        protected override bool OnMove(Direction d)
        {
            if (Utility.RandomDouble() < .5)
            {
                Blood blood = new Blood();
                blood.ItemID = Utility.RandomList(4651, 4652, 4653, 4654);                
                blood.Hue = 2638;
                blood.Name = "bloody discharge";

                blood.MoveToWorld(Location, Map);

                Effects.PlaySound(Location, Map, Utility.RandomList(0x5D9, 0x5DB));
            }

            return base.OnMove(d);
        }

        public override bool OnBeforeDeath()
        {
            Effects.PlaySound(Location, Map, Utility.RandomList(0x4F1));

            int corpseItems = 8;

            for (int a = 0; a < corpseItems; a++)
            {
                Point3D point = new Point3D(Location.X + Utility.RandomList(-2, -1, 1, 2), Location.Y + Utility.RandomList(-2, -1, 1, 2), Location.Z);

                Blood blood = new Blood();
                blood.ItemID = Utility.RandomList(4651, 4652, 4653, 4654);
                blood.Hue = 2638;
                blood.Name = "bloody discharge";

                blood.MoveToWorld(point, Map);                
            }

            return base.OnBeforeDeath();
        }

        public override int GetAngerSound() { return 0x581; }
        public override int GetIdleSound() { return 0x582; }
        public override int GetAttackSound() { return 0x580; }
        public override int GetHurtSound() { return 0x5DA; }
        public override int GetDeathSound() { return 0x5Dc; }
        
        public Bloodworm(Serial serial): base(serial)
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
