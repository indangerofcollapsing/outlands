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
    public class SlimeBomb : BasePotion
    {
        public override bool RequireFreeHand { get { return true; } }

        [Constructable]
        public SlimeBomb()
            : base(0xF0D, PotionEffect.Custom)
        {
            Weight = 1.0;
            Movable = true;
            Hue = 0x1D4;
            Name = "a swirling green potion";
        }

        public SlimeBomb(Serial serial)
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

        public override void Drink(Mobile from)
        {
            if (this != null && this.ParentEntity != from.Backpack)
            {
                from.SendMessage("The potion must be in your pack to drink it.");
            }
            else 
            {
                if (from.CanBeginAction(typeof(SlimeBomb)))
                {
                    if (!RequireFreeHand || HasFreeHand(from))
                    {
                        from.RevealingAction();
                        from.Target = new ThrowTarget(this);
                    }
                    else
                    {
                        from.SendLocalizedMessage(502172); // You must have a free hand to drink a potion.
                    }
                }
                else
                {
                    from.SendMessage("You cannot use that right now."); 
                }
            }		  
        }

        private class ThrowTarget : Target
        {
            private SlimeBomb m_Potion;
            private IEntity to;

            public SlimeBomb Potion
            {
                get { return m_Potion; }
            }
            public ThrowTarget(SlimeBomb potion)
                : base(12, true, TargetFlags.None)
            {
                m_Potion = potion;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Potion.Deleted || m_Potion.Map == Map.Internal)
                    return;

                IPoint3D p = targeted as IPoint3D;

                if (p == null)
                    return;

                Map map = from.Map;

                if (map == null)
                    return;

                SpellHelper.GetSurfaceTop(ref p);

                if (p is Mobile)
                {
                    to = (Mobile)p;
                }
                else
                {
                    to = new Entity(Serial.Zero, new Point3D(p), map);
                }

                if (!map.CanSpawnMobile(to.Location))
                {
                    from.SendLocalizedMessage(501942);// That location is blocked.
                    return;
                }

                from.BeginAction(typeof(SlimeBomb));
                Timer.DelayCall(TimeSpan.FromMinutes(10), delegate { from.EndAction(typeof(SlimeBomb)); });
                from.RevealingAction();
                Effects.SendMovingEffect(from, to, m_Potion.ItemID & 0x3FFF, 7, 0, false, false, m_Potion.Hue, 0);
                Effects.PlaySound(from.Location, map, 0x1CC);
                Timer.DelayCall(TimeSpan.FromSeconds(1.0), delegate { m_Potion.ShowEffect(to.Location, map); });            
                m_Potion.Consume();
            }
        }

        private void ShowEffect(Point3D p, Map map)
        {
            Effects.SendLocationParticles(EffectItem.Create(p, map, EffectItem.DefaultDuration), 0x3728, 10, 10, 5023);
            int z = Utility.Random(3) + 3;
            for (int x = 0; x < z; x++)
            {
                BaseCreature slime = new Slime();
                slime.Hue = Utility.RandomSlimeHue();
                slime.MoveToWorld(p, map);
            }
        }
    }
}