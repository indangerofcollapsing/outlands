using System;
using Server.Items;
using Server.Targeting;
using System.Collections;

namespace Server.Mobiles
{
	[CorpseName( "a drider sentinel corpse" )]
	public class DriderSentinel : BaseCreature
	{
		[Constructable]
		public DriderSentinel() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a drider sentinel";
            Body = 71;
            BaseSoundID = 589;
            Hue = 1102;

			SetStr( 50 );
			SetDex( 50 );
			SetInt( 50 );

			SetHits( 300 );
            SetMana( 1000 );

			SetDamage( 10, 20 );

            SetSkill( SkillName.Wrestling, 85);
            SetSkill( SkillName.Tactics, 100);

			SetSkill( SkillName.MagicResist, 100);

            SetSkill(SkillName.Magery, 50);
            SetSkill(SkillName.EvalInt, 50);
            SetSkill(SkillName.Meditation, 100);

            SetSkill( SkillName.Poisoning, 15);

            SetSkill(SkillName.DetectHidden, 75);

			Fame = 4000;
			Karma = -4000;

			VirtualArmor = 25;
		}        

        public override Poison HitPoison { get { return Poison.Greater; } }
        public override Poison PoisonImmune { get { return Poison.Greater; } }

        public override int Meat { get { return 2; } }

        public override void SetUniqueAI()
        {
            ResolveAcquireTargetDelay = 1.0;
            RangePerception = 12;
            
            DictGuardAction[GuardAction.None] = 1;
            DictGuardAction[GuardAction.DetectHidden] = 3;

            DictWanderAction[WanderAction.None] = 1;
            DictWanderAction[WanderAction.DetectHidden] = 1;

            DictWaypointAction[WaypointAction.None] = 0;
            DictWaypointAction[WaypointAction.DetectHidden] = 1;
        }        

        public DriderSentinel(Serial serial) : base(serial)
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
