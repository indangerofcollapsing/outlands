using System;
using System.Collections.Generic;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Items
{
    public class YellowZombieBrains : BaseZombieBrains
    {
        public override string DefaultName { get { return "a yellow brain"; } }

        [Constructable]
        public YellowZombieBrains()
            : base()
        {
            Hue = 54;
        }

        public override void Eat(Mobile from)
        {
			if (from.CanBeginAction(typeof(YellowZombieBrains)))
			{
                from.SendMessage("Which zombie would you like to throw this at?");
				from.Target = new ZombieTarget(from, this);
			}
			else
			{
				from.SendMessage("You can't throw these again so soon!");
			}
        }

        public YellowZombieBrains(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

		private class ZombieTarget : Target
		{
			private Mobile m_Mobile;
            private YellowZombieBrains m_Brains;

			public ZombieTarget(Mobile m, YellowZombieBrains brains)
				: base(12, false, TargetFlags.None)
			{
				m_Mobile = m;
                m_Brains = brains;
			}

			protected override void OnTarget(Mobile from, object targeted)
			{
                if (m_Brains == null || m_Brains.Deleted)
                {
                    from.SendMessage("You lost sight of the brain.");
                }
				else if (!(targeted is Mobile))
				{
					from.SendMessage("You cannot target that.");
				}
                else if (targeted == from)
                {
                    from.SendMessage("You must use this on another zombie.");
                }
                else
                {
                    from.SendMessage("You heave the zombie brains at your fellow zombie to heal them.");

                    from.BeginAction(typeof(YellowZombieBrains));
                    Timer.DelayCall(TimeSpan.FromMinutes(1), new TimerStateCallback(ReleaseYellowZombieBrainsLock), from);
                    Effects.SendMovingEffect(from, targeted as IEntity, m_Brains.ItemID & 0x3FFF, 7, 0, false, false, m_Brains.Hue, 0);
                    m_Brains.Delete();

                    Timer.DelayCall(TimeSpan.FromSeconds(1.0), delegate
                    {
                        Mobile m = (Mobile)targeted;
                        m.Hits = m.HitsMax;
                        m.SendMessage("{0} has healed you!", from.Name);
                        m.FixedParticles(0x376A, 9, 32, 5005, EffectLayer.Waist);
                        m.PlaySound(0x1F2);
                        from.PlaySound(0x1F2);
                    });
                }
			}
		}

		private static void ReleaseYellowZombieBrainsLock( object state )
		{
			((Mobile)state).EndAction(typeof(YellowZombieBrains));
		}
    }
}