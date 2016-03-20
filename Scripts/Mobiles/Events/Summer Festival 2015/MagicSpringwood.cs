using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server;
using Server.Mobiles;
using Server.Misc;
using Server.Network;
using Server.Engines.Plants;
using Server.Targeting;

namespace Server.Items
{
    public class MagicSpringwood : Item
    {
        public bool inUse = false;

        [Constructable]
        public MagicSpringwood() : base(0x1B9B)
        {
            Name = "magic springwood";
            Hue = 2542;

            Weight = 1;            
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from == null)
                return;

            if (inUse)
                return;

            if (!from.Alive)
            {
                from.SendMessage("You must be alive to use that.");
                return;
            }           

            if (!IsChildOf(from.Backpack))
                from.SendMessage("That item must be in your backpack in order to use it.");
            else
            {
                from.SendMessage("Target a mystical green fire to release the reward hidden within.");
                from.Target = new MagicSpringwoodTarget(this);
            }
        }

        public class MagicSpringwoodTarget : Target
        {
            private MagicSpringwood m_MagicSpringwood;

            public MagicSpringwoodTarget(MagicSpringwood magicSpringwood): base(3, true, TargetFlags.None)
            {
                m_MagicSpringwood = magicSpringwood;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                if (m_MagicSpringwood.Deleted)
                    return;

                PlayerMobile pm = from as PlayerMobile;

                if (pm == null)
                    return;

                MysticGreenFire mysticGreenFire = target as MysticGreenFire;

                if (mysticGreenFire != null)
                {
                    if (m_MagicSpringwood.inUse)
                        return;

                    mysticGreenFire.ReceiveReward(from, m_MagicSpringwood);
                }

                else
                {
                    from.SendMessage("You must target a mystical green fire to use this item.");
                    return;
                }
            }
        }        

        public MagicSpringwood(Serial serial) : base(serial) 
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version 

            writer.Write(inUse);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version >= 0)
            {
                inUse = reader.ReadBool();
            }
        }
    }
}