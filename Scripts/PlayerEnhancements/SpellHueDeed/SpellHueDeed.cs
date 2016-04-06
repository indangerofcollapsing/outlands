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
    public class SpellHueDeed : Item
    {       
        private HueableSpell m_HueableSpell = HueableSpell.MagicArrow;
        [CommandProperty(AccessLevel.Administrator)]
        public HueableSpell HueableSpell
        {
            get { return m_HueableSpell; }
            set { m_HueableSpell = value; }
        }

        private SpellHueType m_HueType = SpellHueType.Icy;
        [CommandProperty(AccessLevel.Administrator)]
        public SpellHueType HueType
        {
            get { return m_HueType; }
            set { m_HueType = value; }
        }

        [Constructable]
        public SpellHueDeed(): base(0x14F0)
        {
            Name = "a spell hue deed";
            Weight = 0.1;

            Hue = 2620;

            m_HueableSpell = SpellHue.GetRandomHueableSpell();
            m_HueType = SpellHue.GetRandomSpellHue();
        }

        [Constructable]
        public SpellHueDeed(HueableSpell hueableSpell): this()
        {
            Name = "a spell hue deed";
            Weight = 0.1;

            Hue = 2620;

            m_HueableSpell = hueableSpell;
            m_HueType = SpellHue.GetRandomSpellHue();
        }

        [Constructable]
        public SpellHueDeed(SpellHueType spellHueType): this()
        {
            Name = "a spell hue deed";
            Weight = 0.1;

            Hue = 2620;

            m_HueableSpell = SpellHue.GetRandomHueableSpell();
            m_HueType = spellHueType;
        }

        [Constructable]
        public SpellHueDeed(HueableSpell hueableSpell, SpellHueType spellHueType): this()
        {
            Name = "a spell hue deed:";
            Weight = 0.1;

            Hue = 2620;

            m_HueableSpell = hueableSpell;
            m_HueType = spellHueType;
        }

        public SpellHueDeed(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            HueableSpellDetail hueableSpellDetails = SpellHue.GetHueableSpellDetail(m_HueableSpell);
            SpellHueTypeDetail spellHueTypeDetails = SpellHue.GetSpellHueTypeDetail(m_HueType);

            string text = "(" + hueableSpellDetails.m_SpellName + ": " + spellHueTypeDetails.m_Name + ")";

            base.OnSingleClick(from);

            LabelTo(from, text);
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
                pm_From.SendMessage("That must be in your backpack for you to use it.");
                return;
            }
            
            PlayerEnhancementPersistance.CheckAndCreatePlayerEnhancementAccountEntry(pm_From);

            Activate(pm_From);
        }

        public virtual void Activate(PlayerMobile player)
        {
            SpellHueEntry entry = PlayerEnhancementPersistance.GetSpellHueEntry(player, m_HueableSpell);

            if (entry == null)
                return;

            HueableSpellDetail hueableSpellDetails = SpellHue.GetHueableSpellDetail(m_HueableSpell);
            SpellHueTypeDetail spellHueTypeDetails = SpellHue.GetSpellHueTypeDetail(m_HueType);

            if (hueableSpellDetails == null || spellHueTypeDetails == null)
                return;

            string displayName = hueableSpellDetails.m_SpellName + ": " + spellHueTypeDetails.m_Name;

            if (entry.m_UnlockedHues.Contains(m_HueType))
            {
                player.SendMessage("You have already unlocked the spell hue for " + displayName + ".");
                return;
            }

            else
            {
                entry.m_UnlockedHues.Add(m_HueType);
                entry.m_ActiveHue = m_HueType;

                player.SendMessage("You unlock the spell hue for " + displayName + ".");

                player.PlaySound(0x0F5);
                player.FixedParticles(0x375A, 10, 15, 5012, 2587, 0, EffectLayer.Waist);

                player.CloseGump(typeof(PlayerSpellHuesGump));

                Delete();                

                return;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            writer.Write((int)m_HueableSpell);
            writer.Write((int)m_HueType);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version >= 0)
            {
                m_HueableSpell = (HueableSpell)reader.ReadInt();
                m_HueType = (SpellHueType)reader.ReadInt();
            }
        }
    }
}