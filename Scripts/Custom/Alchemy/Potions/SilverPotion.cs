using System;
using Server.Network;
using Server;
using Server.Targets;
using Server.Targeting;
using Server.Spells;
using Server.Mobiles;
using Server.Spells.Fifth;
namespace Server.Items
{            
    public class SilverPotion: BasePotion
    {
        [Constructable]
        public SilverPotion()
            : base(0xF0B, PotionEffect.Custom)
        {
            Weight = 1.0;
            Movable = true;
            Hue = 0x3CB;
            Name = "a glittering silver potion";
        }
        public SilverPotion(Serial serial)
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
            else
            {
                m.RevealingAction();
                m.Target = new ThrowTarget(this);
            }
        }

        private class ThrowTarget : Target
        {
            private SilverPotion m_Potion;

            public SilverPotion Potion
            {
                get { return m_Potion; }
            }
            public ThrowTarget(SilverPotion potion)
                : base(12, true, TargetFlags.None)
            {
                m_Potion = potion;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Potion.Deleted || m_Potion.Map == Map.Internal)
                    return;      

                if (!(targeted is ISlayer))
                {
                    from.SendMessage("Thats is an invalid target.");
                    return;
                }
                
                if (!(targeted is Item))
                {
                    if (!((Item)targeted).IsChildOf(from.Backpack))
                    {
                        from.SendMessage("That must be in your backpack.");
                        return;
                    }
                }

                ISlayer b = targeted as ISlayer;

                if (b == null)
                    return;

                Map map = from.Map;

                if (map == null)
                    return;

                from.RevealingAction();
                b.TempSlayer = SlayerName.Silver; 
                Timer.DelayCall(TimeSpan.FromSeconds(600), delegate { m_Potion.ChangeBack(b); });
                BasePotion.PlayDrinkEffect(from);
                m_Potion.Consume();
            }
        } 

        public void ChangeBack(ISlayer item)
        {
            item.TempSlayer = SlayerName.None;   
        }
    }
}