using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Items;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
    public class ArcaneDust : Item
    {
        public override string DefaultName
        {
            get
            {
                return "a pile of arcane dust";
            }
        }

        [Constructable]
        public ArcaneDust()
            : this(1)
        {
        }

        [Constructable]
        public ArcaneDust(int amount)
            : base(0x26B8)
        {
            Stackable = true;
            Weight = 0.1;
            Amount = amount;
            Hue = 2410;
        }



        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(this.GetWorldLocation(), 2) && IsAccessibleTo(from) && !IsLockedDown)
            {
                from.Target = new InternalTarget(this);
            }
            else
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            }
        }

        private class InternalTarget : Target
        {
            private ArcaneDust m_Powder;

            public InternalTarget(ArcaneDust powder)
                : base(-1, false, TargetFlags.None)
            {
                m_Powder = powder;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Powder.Deleted)
                    return;

                if (!from.InRange(m_Powder.GetWorldLocation(), 2))
                {
                    from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
                    return;
                }

                if (targeted is Item)
                {
                    Item item = targeted as Item;

                    if (item.IsArcaneDustRechargable)
                    {
                        if (item.ArcaneDustBasedChargesRemaining < item.ArcaneDustBasedChargesMaximumCharges)
                        {
                            item.ArcaneDustBasedChargesRemaining += item.ArcaneDustBasedChargesRegainedPerArcaneDust;

                            if (item.ArcaneDustBasedChargesRemaining > item.ArcaneDustBasedChargesMaximumCharges)
                                item.ArcaneDustBasedChargesRemaining = item.ArcaneDustBasedChargesMaximumCharges;

                            from.SendMessage("You use the arcane dust to recharge the item.");
                            from.SendSound(0x64B);

                            if (m_Powder.Amount > 1)
                                m_Powder.Amount -= 1;

                            else
                                m_Powder.Delete();

                            return;
                        }

                        else
                        {
                            from.SendMessage("That item is already at full charge capacity.");
                            return;
                        }
                    }
                }
                
                if (targeted is BaseDungeonArmor)
                {
                    BaseDungeonArmor DungeonArmorPiece = targeted as BaseDungeonArmor;

                    if (DungeonArmorPiece.BlessedCharges >= DungeonArmorPiece.MaxBlessedCharges)
                        MessageHelper.SendMessageTo(m_Powder, from, "This item cannot absorb any more charges.", 0x59);                    

                    else
                    {                     
                        DungeonArmorPiece.BlessedCharges = DungeonArmorPiece.MaxBlessedCharges;
                                              
                        if (m_Powder.Amount > 1)
                            m_Powder.Amount -= 1;

                        else
                            m_Powder.Delete();
                    }
                }

                else if (targeted is BaseDungeonShield)
                {
                    BaseDungeonShield DungeonShield = targeted as BaseDungeonShield;

                    if (DungeonShield.BlessedCharges >= DungeonShield.MaxBlessedCharges)
                        MessageHelper.SendMessageTo(m_Powder, from, "This item cannot absorb any more charges.", 0x59);

                    else
                    {
                        DungeonShield.BlessedCharges = DungeonShield.MaxBlessedCharges;

                        if (m_Powder.Amount > 1)
                            m_Powder.Amount -= 1;

                        else
                            m_Powder.Delete();
                    }
                }

                else
                    MessageHelper.SendMessageTo(m_Powder, from, "There is no effect to this item.", 0x59);                
            }
        }

        public ArcaneDust(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}
