using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Network;

namespace Server.Mobiles
{
	[CorpseName( "a plague beast corpse" )]
	public class PlagueBeast : BaseCreature
	{
		[Constructable]
		public PlagueBeast() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a plague beast";
			Body = 775;
            Hue = 2963;

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(300);

            SetDamage(12, 24);

            SetSkill(SkillName.Wrestling, 80);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 25;

			Fame = 13000;
			Karma = -13000;
		}

        public override void SetUniqueAI()
        {            
            UniqueCreatureDifficultyScalar = 1.25;
        }

        protected override bool OnMove(Direction d)
        {
            if (Global_AllowAbilities)
            {
                Blood blood = new Blood();
                blood.Hue = 2963;
                blood.Name = "slime";
                blood.ItemID = Utility.RandomList(4650, 4651, 4652, 4653, 4654, 4655);

                blood.MoveToWorld(Location, Map);

                Effects.PlaySound(Location, Map, Utility.RandomList(0x5D9, 0x5DB));
            }

            return base.OnMove(d);
        }
		
		public override void OnDamagedBySpell( Mobile caster )
		{
			if ( this.Map != null && caster != this && 0.25 > Utility.RandomDouble() )
			{
				BaseCreature spawn = new PlagueSpawn( this );

				spawn.Team = this.Team;
				spawn.MoveToWorld( this.Location, this.Map );
				spawn.Combatant = caster;

				Say( 1053034 ); // * The plague beast creates another beast from its flesh! *
			}

			base.OnDamagedBySpell( caster );
		}

		public override void OnGotMeleeAttack( Mobile attacker )
		{
			if ( this.Map != null && attacker != this && 0.25 > Utility.RandomDouble() )
			{
				BaseCreature spawn = new PlagueSpawn( this );

				spawn.Team = this.Team;
				spawn.MoveToWorld( this.Location, this.Map );
				spawn.Combatant = attacker;

				Say( 1053034 ); // * The plague beast creates another beast from its flesh! *
			}

			base.OnGotMeleeAttack( attacker );
		}

		public PlagueBeast( Serial serial ) : base( serial )
		{
		}

		public override int GetIdleSound()
		{
			return 0x1BF;
		}

		public override int GetAttackSound()
		{
			return 0x1C0;
		}

		public override int GetHurtSound()
		{
			return 0x1C1;
		}

		public override int GetDeathSound()
		{
			return 0x1C2;
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
