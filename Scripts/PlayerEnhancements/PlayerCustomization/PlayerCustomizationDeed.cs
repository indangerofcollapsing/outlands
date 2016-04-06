using System;
using Server;
using Server.Regions;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;
using Server.Multis;
using System.Collections;
using System.Collections.Generic;
using Server.Items;

namespace Server.Custom
{
    public class PlayerCustomizationDeed : Item
    {
        private CustomizationType m_Customization = CustomizationType.Artisan;
        [CommandProperty(AccessLevel.Administrator)]
        public CustomizationType Customization
        {
            get { return m_Customization; }
            set { m_Customization = value; }
        }

        [Constructable]
        public PlayerCustomizationDeed(): base(0x14F0)
        {
            Name = "a player customization deed";

            Hue = 2615;

            m_Customization = PlayerCustomization.GetRandomCustomizationType();
        }

        [Constructable]
        public PlayerCustomizationDeed(CustomizationType customization): this()
        {
            Name = "a player customization deed";
            Weight = 0.1;

            m_Customization = customization;
        }

        public PlayerCustomizationDeed(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            PlayerCustomizationDetail details = PlayerCustomization.GetCustomizationDetail(m_Customization);

            LabelTo(from, "a player customization deed: " + details.m_Name);
        }

        public override void OnDoubleClick(Mobile from)
        {
            PlayerMobile pm_From = from as PlayerMobile;

            if (pm_From == null)
                return;

            if (!pm_From.Alive)
            {
                pm_From.SendMessage("You must be alive to use that.");
                return;
            }

            if (!IsChildOf(pm_From.Backpack))
            {
                pm_From.SendMessage("that must be in your backpack for you to use it.");
                return;
            }

            PlayerEnhancementPersistance.CheckAndCreatePlayerEnhancementAccountEntry(pm_From);

            Activate(pm_From);
        }

        public virtual void Activate(PlayerMobile player)
        {
            PlayerCustomizationEntry entry = PlayerEnhancementPersistance.GetCustomizationEntry(player, m_Customization);

            if (entry == null)
                return;

            PlayerCustomizationDetail details = PlayerCustomization.GetCustomizationDetail(m_Customization);

            if (details == null)
                return;

            if (entry.m_Unlocked)
            {
                player.SendMessage("You have already unlocked the player customization: " + details.m_Name + ".");
                return;
            }

            else
            {
                entry.m_Unlocked = true;
                entry.m_Active = true;

                player.SendMessage("You unlock the player customization for: " + details.m_Name + ".");

                PlayerCustomization.OnUnlockCustomization(player, m_Customization);

                player.PlaySound(0x0F7);
                player.FixedParticles(0x373A, 10, 15, 5012, 2587, 0, EffectLayer.Waist);

                player.CloseGump(typeof(PlayerCustomizationGump));
                player.CloseGump(typeof(PlayerCustomizationConfirmationGump));                

                Delete();

                return;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            writer.Write((int)m_Customization);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version >= 0)
            {
                m_Customization = (CustomizationType)reader.ReadInt();
            }
        }
    }
}