using System;
using System.Collections.Generic;
using Server.Custom;

namespace Server.Items
{
	/*
	Blue   - Full health replenish.
	Yellow - Able to target another zombie and heal them to full.
	Gold   - Invulnerability for 3 minutes.
	Red    - Red hue, able to double click the brain and cast Energy Bolt at 90 magery/90 EI every 30 seconds. Lasts 10 minutes.
	Black  - Transform into an undead (grey) Ogre. Perhaps Ogre Lord stats, 20 minutes?
	Silver - Double speed (mount speed), silver hued zombie. 5 minutes.
	*/

	// Zombie Brains Hues
	//	Color	Hue    
	//	Blue	6
	//	Yellow	54
	//	Gold	1177
	//	Red		37
	//  Black	1908
	//	Silver	2101

    public abstract class BaseZombieBrains : Item
    {
        public Mobile Owner { get; set; }

        public BaseZombieBrains()
            : base(0x1CF0)
        {
            Timer.DelayCall(TimeSpan.FromMinutes(20), delegate { Delete(); });
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("You cannot reach that.");
            }
            else if (!ZombieQuestRegion.IsZombieBody(from))
            {
                from.SendMessage("That looks disgusting...");
            }
            else if (ZombieQuestRegion.IsAbomination(from))
            {
                from.SendMessage("You cannot eat brains in your current form.");
            }
            else if (Owner != null && Owner == from)
            {
                from.SendMessage("You wouldn't dare eat your own brain.");
            }
            else
            {
                Eat(from);
            }
        }

        public override bool CheckLift(Mobile from, Item item, ref Network.LRReason reject)
        {
            if (!ZombieQuestRegion.IsZombieBody(from))
            {
                from.SendMessage("That looks disgusting...");
                reject = Network.LRReason.CannotLift;
                return false;
            }
            else if (Owner != null && Owner == from)
            {
                from.SendMessage("You wouldn't dare eat your own brain.");
                reject = Network.LRReason.CannotLift;
                return false;
            }
            else
            {
                return base.CheckLift(from, item, ref reject);
            }
        }

        public virtual void Eat(Mobile from)
        {
        }

        public BaseZombieBrains(Serial serial)
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

            Timer.DelayCall(TimeSpan.FromMinutes(20), delegate { Delete(); });
        }
    }
}
