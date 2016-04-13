using System;

using Server.Network;
using Server.Spells;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Items
{
    public enum EnhancedSpellbookType
    {
        Wizard,
        Warlock,
        Fire,
        Energy,
        Summoner,
        Slayer
    }    
    
    public class EnhancedSpellbook : Spellbook
	{
        private EnhancedSpellbookType m_EnhancedType;       
        
        [CommandProperty(AccessLevel.GameMaster)]
        public EnhancedSpellbookType EnhancedType
        {
            get { return m_EnhancedType; }
            set { m_EnhancedType = value; InvalidateProperties(); }
        }

        private int m_ChargesRemaining = 200;
        [CommandProperty(AccessLevel.GameMaster)]
        public int ChargesRemaining
        {
            get { return m_ChargesRemaining; }
            set { m_ChargesRemaining = value; }
        }

        [Constructable]
        public EnhancedSpellbook(EnhancedSpellbookType type, SlayerGroupType slayerGroupType) : this((ulong)0)
        {
            Hue = 101;

            LootType = Server.LootType.Regular;
            Layer = Layer.OneHanded;

            if (type == null)
                return;

            EnhancedType = type;

            m_ChargesRemaining = Utility.RandomMinMax(200, 300);

            if (EnhancedType != null && EnhancedType == EnhancedSpellbookType.Slayer)
                SlayerGroup = slayerGroupType;   
        }

        [Constructable]
        public EnhancedSpellbook(EnhancedSpellbookType type) : this((ulong)0)
        {
            Hue = 101;

            LootType = Server.LootType.Regular;
            Layer = Layer.OneHanded;

            if (type == null)
                return;

            m_ChargesRemaining = Utility.RandomMinMax(200, 300);

            EnhancedType = type;
        }
        
        [Constructable]
		public EnhancedSpellbook() : this( (ulong)0 )
		{
            Hue = 101;

            LootType = Server.LootType.Regular;
            Layer = Layer.OneHanded;

            m_ChargesRemaining = Utility.RandomMinMax(200, 300);
		}

		[Constructable]
        public EnhancedSpellbook(ulong content) : base(content, 0x0E3B)
		{	            
            Hue = 101;

            LootType = Server.LootType.Regular;
            Layer = Layer.OneHanded;

            m_ChargesRemaining = Utility.RandomMinMax(200, 300);
		}

        public void OnSpellCast(Mobile from)
        {
            m_ChargesRemaining--;

            if (m_ChargesRemaining == 0)
            {
                from.SendMessage("The spellbook runs out of charges and crumbles to dust in your hands.");
                Delete();
            }
        }       

        public override void OnSingleClick(Mobile from)
        {
            if (m_EnhancedType == EnhancedSpellbookType.Slayer)            
                LabelTo(from, SlayerGroup.ToString());   

            else            
                LabelTo(from, EnhancedSpellbookTypeAsString(m_EnhancedType));

            LabelTo(from, "charges: " + m_ChargesRemaining.ToString());   
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (dropped is SpellScroll)
            {
                from.SendMessage("You may not add scrolls to an Enhanced Spellbook");
                return false;
            }

            return base.OnDragDrop(from, dropped);
        }

        #region Name Conversions

        public static string EnhancedSpellbookTypeAsString(EnhancedSpellbookType type)
        {
            switch (type)
            {
                case EnhancedSpellbookType.Wizard:
                    return "Wizard Spellbook";

                case EnhancedSpellbookType.Warlock:
                    return "Warlock's Spellbook";

                case EnhancedSpellbookType.Fire:
                    return "Fire Spellbook";

                case EnhancedSpellbookType.Energy:
                    return "Energy Spellbook";

                case EnhancedSpellbookType.Summoner:
                    return "Summoner Spellbook";

                case EnhancedSpellbookType.Slayer:
                    return "Slayer Spellbook";

                default: return "Enhanced Spellbook";
            }
        }

        #endregion

        public EnhancedSpellbook(Serial serial): base(serial)
        {
        }

        public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( 0 ); // version  
         
            //Version 0
            writer.Write((int)m_EnhancedType); 
            writer.Write(m_ChargesRemaining);            
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();

            if (version >= 0)
            {
                m_EnhancedType = (EnhancedSpellbookType)reader.ReadInt();
                m_ChargesRemaining = reader.ReadInt();
            }
		}
	}
}