using System;
using System.Collections.Generic;
using Server.Spells;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
    public class RedZombieBrains : BaseZombieBrains
    {
        public override string DefaultName { get { return "a red brain"; } }

        //This is added so players don't share brains to benefit the group
        public DateTime m_NextCast;

        [Constructable]
        public RedZombieBrains()
            : base()
        {
            Hue = 37;
        }

        public override void Eat(Mobile from)
        {
            if (DateTime.UtcNow < m_NextCast)
            {
                from.SendMessage("You must wait before consuming more of these brains.");
            }
            else if (from.CanBeginAction(typeof(RedZombieBrains)))
            {
                from.SendMessage("You eat the reddish brains and feel a surge of energy.");
                from.Target = new ZombieTarget(from, this);

                // Delay for Red Zombie Brain deletion
                Timer.DelayCall(TimeSpan.FromMinutes(10), delegate
                {
                    Delete();
                    from.SendMessage("The brains have spoiled and disolve in front of you.");
                });
            }
            else
            {
                from.SendMessage("You must wait before consuming more of these brains.");
            }
        }

        public RedZombieBrains(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        private class ZombieTarget : Target
        {
            private Mobile m_Mobile;
            private RedZombieBrains m_Brains;

            public ZombieTarget(Mobile m, RedZombieBrains brains)
                : base(12, false, TargetFlags.Harmful)
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
                else
                {
                    from.BeginAction(typeof(RedZombieBrains));
                    Timer.DelayCall(TimeSpan.FromSeconds(35), delegate { from.EndAction(typeof(RedZombieBrains)); });

                    m_Brains.m_NextCast = DateTime.UtcNow + TimeSpan.FromSeconds(30);

                    Mobile m = (Mobile)targeted;

                    from.SendMessage("You nibble on the brains some more and again feel an energy discharge!");

                    // Perform the E-Bolt Code
                    #region E-Bolt

                    // Face Target
                    SpellHelper.Turn(from, m);

                    // Check for reflection
                    SpellHelper.CheckReflect(6, ref from, ref m);

                    int damage = Utility.Random(24, 18);

                    // Do the effects
                    from.MovingParticles(m, 0x379F, 7, 0, false, true, 3043, 4043, 0x211);
                    m.PlaySound(0x20A);

                    // Deal the damage
                    m.Damage(damage, from);
                    #endregion
                }
            }
        }

        private static void ReleaseRedZombieBrainsLock(object state)
        {
            ((Mobile)state).EndAction(typeof(RedZombieBrains));
        }
    }
}