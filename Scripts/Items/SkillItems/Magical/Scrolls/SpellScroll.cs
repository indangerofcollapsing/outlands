using System;
using System.Collections;
using System.Collections.Generic;
using Server.Spells;
using Server.ContextMenus;
using Server.Engines.Craft;

namespace Server.Items
{
	public class SpellScroll : Item, ICommodity
    {
        private int m_SpellID;
        private int m_MasterStatus; //0 = Regular Scroll, 1 = Master Scroll (Unused), 2 = Master Scroll (Used)
        private int m_UsesRemaining = 0;

        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining
        {
            get { return m_UsesRemaining; }
            set { m_UsesRemaining = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MasterStatus
        {
            get
            {
                return m_MasterStatus;
            }
            set
            {
                m_MasterStatus = value;
                Hue = value > 0 ? 1340 : 0;
                UsesRemaining = Utility.RandomMinMax(40, 80);
            }
        }

        public int SpellID
        {
            get
            {
                return m_SpellID;
            }
        }

		int ICommodity.DescriptionNumber { get { return LabelNumber; } }
		bool ICommodity.IsDeedable { get { return (Core.ML); } }

        public static SpellScroll MakeMaster(SpellScroll scroll)
        {
            if (scroll == null)
                return null;

            scroll.MasterStatus = 1;

            return scroll;
        }

        public SpellScroll(Serial serial)
            : base(serial)
        {
        }

        [Constructable]
		public SpellScroll( int spellID, int itemID ) : this( spellID, itemID, 1 )
        {
        }

        [Constructable]
		public SpellScroll( int spellID, int itemID, int amount ) : base( itemID )
        {
            Stackable = true;
            Weight = 0.1;
            Amount = amount;

            m_SpellID = spellID;
            m_MasterStatus = 0;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)3); // version

            //version 1
            writer.Write((int)m_MasterStatus);

            if (m_MasterStatus > 0) //version 2
                writer.Write(UsesRemaining);

            writer.Write((int)m_SpellID);

            
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 3:
                    {
                        goto case 2;
                    }
                case 2:
                    {
                        goto case 1;
                    }
                case 1:
                    {
                        m_MasterStatus = reader.ReadInt();

                        if (version >= 2 && m_MasterStatus > 0)
                            UsesRemaining = reader.ReadInt();

                        goto case 0;
                    }
                case 0:
                    {
                        m_SpellID = reader.ReadInt();

                        break;
                    }
            }

            if (version < 3 && m_MasterStatus > 0)
                UsesRemaining = Utility.RandomMinMax(40,80);

            //Override weight to 0.1
            Weight = 0.1;
        }


        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (from.Alive && this.Movable && m_MasterStatus == 0)
                list.Add(new ContextMenus.AddToSpellbookEntry());
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            if (MasterStatus > 0)
            {
                LabelTo(from, "Master Scroll{0}",m_MasterStatus == 1? " [+1]" : "");
                LabelTo(from, "Charges: {0}", UsesRemaining);
            }
        }


        public override void OnDoubleClick(Mobile from)
        {
            if (!Multis.DesignContext.Check(from))
                return; // They are customizing

            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                return;
            }

            if (m_MasterStatus == 1) //Unused MasterScroll
            {
                double valInscription = from.Skills[SkillName.Inscribe].Value;

                if ((m_SpellID >= 48) && (m_SpellID <= 56)) //Circle 7 Scroll

                    if (valInscription >= 60.7) //Inscribe skillcheck
                        UseMasterScroll(from);
                    else
                    {
                        from.SendMessage("You are not experienced enough to be reading this material");
                    }

                else if ((m_SpellID >= 56) && (m_SpellID <= 63)) //Circle 8 Scroll

                    if (valInscription >= 75.0) //Inscribe skillcheck
                        UseMasterScroll(from);
                    else
                    {
                        from.SendMessage("You are not experienced enough to be reading this material");
                    }
            }
            else if (m_MasterStatus == 2) //Used MasterScroll
                from.SendMessage("This scroll must be in your pack while attempting to inscribe.");
            else
                CastScroll(from);
        }

        public override bool StackWith(Mobile from, Item dropped, bool playSound)
        {
            return m_MasterStatus == 0 && base.StackWith(from, dropped, playSound);
        }

        private void CastScroll(Mobile from)
        {
            Spell spell = SpellRegistry.NewSpell(m_SpellID, from, this);

            if (spell != null)
                spell.Cast();
            else
                from.SendLocalizedMessage(502345); // This spell has been temporarily disabled.
        }

        private void UseMasterScroll(Mobile from)
        {
            BlankScroll bScroll = from.Backpack.FindItemByType(typeof(BlankScroll)) as BlankScroll;
            if (bScroll != null)
            {
                if (m_SpellID > Loot.RegularScrollTypes.Length || m_SpellID < 0)
                    return;

                bScroll.Consume();
                m_MasterStatus = 2;
                from.SendMessage("You have made the initial copy of the Master Scroll!");
                from.PlaySound(0x249);
                Item item = Activator.CreateInstance(Loot.RegularScrollTypes[m_SpellID]) as Item;
                from.AddToBackpack(item);
            }
            else
                from.SendMessage("You must have a blank scroll in your possession to use this.");
            
        }

    }
}