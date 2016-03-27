using System;
using Server.Network;
using Server.Items;
using Server.Targeting;
using Server.Mobiles;
using Server.Engines.CannedEvil;

namespace Server.Items
{
	[FlipableAttribute( 0xE81, 0xE82 )]
	public class ShepherdsCrook : BaseStaff
	{
        public override int BaseMinDamage { get { return 1; } }
        public override int BaseMaxDamage { get { return 2; } }
        public override int BaseSpeed { get { return 50; } }

        public override int InitMinHits { get { return 40; } }
        public override int InitMaxHits { get { return 60; } }

        public override int IconItemId { get { return 5108; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return -28; } }
        public override int IconOffsetY { get { return -10; } }

		[Constructable]
		public ShepherdsCrook() : base( 0xE81 )
		{
			Weight = 4.0;
		}

		public ShepherdsCrook( Serial serial ) : base( serial )
		{
		}

		public override WeaponAnimation GetAnimation()
		{
			WeaponAnimation animation = WeaponAnimation.Slash1H;

			Mobile attacker = this.Parent as Mobile;

			if (attacker != null)
			{
				if (attacker.FindItemOnLayer(Layer.TwoHanded) != null)
				{
					switch (Utility.RandomMinMax(1,4))
					{
						case 1: animation = WeaponAnimation.Bash2H;break;
						case 2: animation = WeaponAnimation.Bash2H;break;
						case 3: animation = WeaponAnimation.Slash2H;break;
						case 4: animation = WeaponAnimation.Pierce2H;break;
					}
				}
			}

			return animation;
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

		public override void OnDoubleClick( Mobile from )
		{
			from.SendLocalizedMessage( 502464 ); // Target the animal you wish to herd.
			from.Target = new HerdingTarget();
		}

		private class HerdingTarget : Target
		{
			public HerdingTarget() : base( 10, false, TargetFlags.None )
			{
			}

			protected override void OnTarget( Mobile from, object targ )
			{
				if ( targ is BaseCreature )
				{
					BaseCreature bc = (BaseCreature)targ;

					if ( IsHerdable( bc ) )
					{
						if ( bc.Controlled )						
							bc.PrivateOverheadMessage( MessageType.Regular, 0x3B2, 502467, from.NetState ); // That animal looks tame already.
						
						else 
						{
							from.SendLocalizedMessage( 502475 ); // Click where you wish the animal to go.
							from.Target = new InternalTarget( bc );
						}
					}

					else					
						from.SendLocalizedMessage( 502468 ); // That is not a herdable animal.					
				}

				else				
					from.SendLocalizedMessage( 502472 ); // You don't seem to be able to persuade that to move.				
			}
            
			private bool IsHerdable( BaseCreature bc )
			{
				if ( bc.IsParagon )
					return false;

				if ( bc.Tameable )
					return true;

				Map map = bc.Map;

				return false;
			}

			private class InternalTarget : Target
			{
				private BaseCreature m_Creature;

				public InternalTarget( BaseCreature c ) : base( 10, true, TargetFlags.None )
				{
					m_Creature = c;
				}

				protected override void OnTarget( Mobile from, object targ )
				{
					if ( targ is IPoint2D )
					{
						if ( from.CheckTargetSkill( SkillName.Herding, m_Creature, 0, 120, 1.0 ) )
						{
							m_Creature.TargetLocation = new Point2D( (IPoint2D)targ );
							from.SendLocalizedMessage( 502479 ); // The animal walks where it was instructed to.

                            from.NextSkillTime = Core.TickCount + (int)(SkillCooldown.HerdingSuccessCooldown * 1000);
						}

						else
						{
							from.SendLocalizedMessage( 502472 ); // You don't seem to be able to persuade that to move.
                            from.NextSkillTime = Core.TickCount + (int)(SkillCooldown.HerdingFailureCooldown * 1000);
						}
					}
				}
			}
		}
	}
}
