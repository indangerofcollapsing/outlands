using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom.Items
{
    class AstralLeash : Item
    {
        private int m_Charges;
        [CommandProperty(AccessLevel.Counselor)]
        public int Charges { get { return m_Charges; } set { m_Charges = value; } }

        [Constructable]
        public AstralLeash()
            : base(0x1ea0)
        {
            m_Charges = 1;
            Hue = 17;
            Stackable = true;
        }

        public override bool StackWith(Mobile from, Item dropped, bool playSound)
        {
            if (dropped is AstralLeash)
            {
                var droppedLeash = dropped as AstralLeash;
                if (dropped.Movable && (Movable || (!Movable && IsLockedDown)) && dropped.Stackable && Stackable && dropped.GetType() == GetType() && dropped.ItemID == ItemID && dropped.Hue == Hue &&  (droppedLeash.Charges + Charges) <= MaxStack && dropped != this && !dropped.Nontransferable && !Nontransferable)
                {
                    if (LootType != dropped.LootType)
                        LootType = LootType.Regular;

                    Charges += droppedLeash.Charges;
                    dropped.Delete();

                    if (playSound && from != null)
                    {
                        int soundID = GetDropSound();

                        if (soundID == -1)
                            soundID = 0x42;

                        from.SendSound(soundID, GetWorldLocation());
                    }

                    return true;
                }

                return false;
            }

            return false;
        }

        public override int MaxStack
        {
            get { return 5; }
        }

        public override string DefaultName { get { return string.Format("astral leash - {0} charges remaining.", m_Charges); } }

        public bool Consume()
        {
            if (CanConsume())
            {
                m_Charges -= 1;
                if (m_Charges <= 0)
                    Delete();
                return true;
            }

            return false;
        }

        public bool CanConsume()
        {
            return m_Charges > 0;
        }

        public AstralLeash(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(m_Charges);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Charges = reader.ReadInt();
            Stackable = true;
        }
    }
}
