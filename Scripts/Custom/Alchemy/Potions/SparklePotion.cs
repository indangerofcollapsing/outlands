using System;
using System.Collections;
using Server;
using Server.Network;
using Server.Targets;
using Server.Targeting;
using Server.Mobiles;
using Server.Spells;

namespace Server.Items
{
	public class SparklePotion : BasePotion
	{
		public override bool RequireFreeHand{ get{ return true; } }

        [Constructable]
		public SparklePotion() 
            : base( 0xF0D, PotionEffect.Custom )
		{
            Weight = 1.0;
            Movable = true;
            Hue = 2125;
            Name = "a sparkley potion";
		}

		public SparklePotion( Serial serial ) : base( serial )
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

		public override void Drink( Mobile from )
		{
            if (this != null && this.ParentEntity != from.Backpack)
            {
                from.SendMessage("The potion must be in your pack to drink it.");
            }
            else
            {
                from.RevealingAction();
                from.Target = new ThrowTarget(this);
            }
		}

		private class ThrowTarget : Target
		{
			private SparklePotion m_Potion;
            private IEntity to;
            private TimeSpan delay;

			public SparklePotion Potion
			{
				get{ return m_Potion; }
			}
			public ThrowTarget( SparklePotion potion ) : base( 12, true, TargetFlags.None )
			{
				m_Potion = potion;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( m_Potion.Deleted || m_Potion.Map == Map.Internal )
					return;

				IPoint3D p = targeted as IPoint3D;

				if ( p == null )
					return;

				Map map = from.Map;

				if ( map == null )
					return;

				SpellHelper.GetSurfaceTop( ref p );
				from.RevealingAction();  
   
                if (p is Mobile)
                {
                    to = (Mobile)p;
                    MobileDeleteTimer t = new MobileDeleteTimer((Mobile)p);
                    t.Start();
                }
                else
                {
                    to = new Entity(Serial.Zero, new Point3D(p), map);
                }
				Effects.SendMovingEffect( from, to, m_Potion.ItemID & 0x3FFF, 7, 0, false, false, m_Potion.Hue, 0 );

				if( m_Potion.Amount > 1 )
				{
					Mobile.LiftItemDupe( m_Potion, 1 );
				}
				m_Potion.Internalize();
			}   
		}

        public class MobileDeleteTimer : Timer
        {

            private Mobile mob;
            private int count;

            public MobileDeleteTimer(Mobile m) : base(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10))
            {
                mob = m;
                count = 0;

                //Priority = TimerPriority.FiveSeconds;
            }

            protected override void OnTick()
            {
                if (count < 18)
                {
                    if (mob != null || mob.Deleted)
                    {
                        int hue = Utility.Random(40);
                        if (hue < 8)
                            hue = 0x66D;
                        else if (hue < 10)
                            hue = 0x482;
                        else if (hue < 12)
                            hue = 0x47E;
                        else if (hue < 16)
                            hue = 0x480;
                        else if (hue < 20)
                            hue = 0x47F;
                        else
                            hue = 0;

                        if (Utility.RandomBool())
                            hue = Utility.RandomList(0x47E, 0x47F, 0x480, 0x482, 0x66D);

                        int renderMode = Utility.RandomList(0, 2, 3, 4, 5, 7);
                        Effects.PlaySound(mob.Location, mob.Map, Utility.Random(0x11B, 4));
                        Effects.SendLocationEffect(mob.Location, mob.Map, 0x373A + (0x10 * Utility.Random(4)), 16, 10, hue, renderMode);
                        count += 1;
                    }
                }
                else
                {
                    this.Stop();
                }
            }
        }
	}
}