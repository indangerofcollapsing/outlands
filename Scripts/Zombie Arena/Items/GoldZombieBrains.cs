using System;
using System.Collections.Generic;
using Server.Commands;
using Server.Mobiles;
using Server.Targeting;


namespace Server.Items
{
    public class GoldZombieBrains : BaseZombieBrains
    {
        public override string DefaultName { get { return "a gold brain"; } }

        [Constructable]
        public GoldZombieBrains()
            : base()
        {
            Hue = 1177;
        }

        public override void Eat(Mobile from)
        {
			if (from.BeginAction(typeof(GoldZombieBrains)))
			{
				from.FixedParticles(0x376A, 9, 32, 5007, EffectLayer.Waist);
				from.PlaySound(0x03C);
				from.SendMessage("You devour the golden brains and begin to feel your skin harden.");
				from.Blessed = true;
				Timer.DelayCall(TimeSpan.FromMinutes(3), new TimerStateCallback(ReleaseGoldZombieBrainsLock), from);
				Delete();
			}
			else
			{
				from.SendMessage("You can't eat another one of these so soon!");
			}
        }

        public GoldZombieBrains(Serial serial)
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

		private static void ReleaseGoldZombieBrainsLock(object state)
		{
			((Mobile)state).Blessed = false;
			((Mobile)state).EndAction(typeof(GoldZombieBrains));
		}
    }
}
