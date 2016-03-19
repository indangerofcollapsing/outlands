using System;
using Server.Network;
using Server;
using Server.Targets;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Items
{
    public abstract class BaseRepelPotion : BasePotion
    {
        public abstract double Delay { get; }
        public override bool RequireFreeHand { get { return true; } }
        public abstract SlayerName RepelSlayerName { get; }

        public BaseRepelPotion( PotionEffect effect ) : base(0xF0B, PotionEffect.Custom)
        {
        }

        public BaseRepelPotion(Serial serial)
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

        private void ChangeBack(PlayerMobile from)
        {
            from.RepelGroupEntry = null;
        }

        private static void ReleaseExploLock(object state)
        {
            ((Mobile)state).EndAction(typeof(BaseRepelPotion));
        }

        public override void Drink(Mobile from)
        {
            if (this != null && this.ParentEntity != from.Backpack)
            {
                from.SendMessage("The potion must be in your pack to drink it.");
            }
            else if (!from.BeginAction(typeof(BaseRepelPotion)))
            {
                from.PrivateOverheadMessage(MessageType.Regular, 0x22, false, "You must wait a while before you drink another.", from.NetState);
                return;
            }
            else
            {
                if (RequireFreeHand)
                {
                    if (from is PlayerMobile)
                    {
                        PlayerMobile p = from as PlayerMobile;
                        Timer.DelayCall(TimeSpan.FromSeconds(180), new TimerStateCallback(ReleaseExploLock), from);
                        Timer.DelayCall(TimeSpan.FromSeconds(60), delegate { ChangeBack(p); });
                        p.RepelGroupEntry = SlayerGroup.GetEntryByName(RepelSlayerName);
                        this.Consume();
                        if (p.Female == false)
                        {
                            from.SendMessage("Hail to the king baby.");
                        }
                        else
                        {
                            from.SendMessage("Hail to the queen baby.");
                        }
                        int hue = 0x43D;
                        int renderMode = 2;
                        Effects.SendLocationEffect(from.Location, from.Map, 0x373A + (0x10 * 2), 16, 10, hue, renderMode);
                        BasePotion.PlayDrinkEffect(from);
                        from.PlaySound(0x1E7);
                    }
                }
                else
                {
                    from.SendMessage("You need 1 hand available to drink that.");
                }
            }
        }
	}
}