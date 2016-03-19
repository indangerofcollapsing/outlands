using System;
using Server.Network;
using Server;
using Server.Targets;
using Server.Targeting;
using Server.Spells;
using Server.Mobiles;
using Server.Spells.Fifth;
using System.Collections;
using System.Collections.Generic;
    
namespace Server.Items
{            
    public class InvisPotion: BasePotion
    {
        public override bool RequireFreeHand { get { return true; } }
        private static Dictionary<Mobile, InternalTimer> _table = new Dictionary<Mobile, InternalTimer>();

        [Constructable]
        public InvisPotion()
            : base(0xF0B, PotionEffect.Custom)
        {
            Weight = 1.0;
            Movable = true;
            Hue = 0x189;
            Name = "a translucent bottle of goo";
        }
        public InvisPotion(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override void Drink(Mobile m)
        {
            if (this != null && this.ParentEntity != m.Backpack)
            {
                m.SendMessage("The potion must be in your pack to drink it.");
            }
            else if (_table.ContainsKey(m))
            {
                m.SendMessage("You cannot drink that potion again so soon.");
            }
            else
            {
                m.Animate(34, 5, 1, true, false, 0);
                BasePotion.PlayDrinkEffect(m);
                m.PlaySound(0x3C4);
                m.Hidden = true;
                this.Consume();
                InternalTimer t = new InternalTimer(m, TimeSpan.FromSeconds(60));
                _table.Add(m, t);
            }
        }

        public static void RemoveTimer(Mobile m)
        {
            InternalTimer t;
            _table.TryGetValue(m, out t);

            if (t != null)
            {
                t.Stop();
                _table.Remove(m);
            }
        }

        private class InternalTimer : Timer
        {
            private Mobile m_Mobile;

            public InternalTimer(Mobile m, TimeSpan duration)
                : base(duration)
            {
                m_Mobile = m;
            }

            protected override void OnTick()
            {
                m_Mobile.RevealingAction();
                RemoveTimer(m_Mobile);
            }
        }
    }
}