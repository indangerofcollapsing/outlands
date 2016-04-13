using System;

using Server.Network;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Items
{
    public class EnhancedSpellbookTome : Item
	{
        private EnhancedSpellbookType m_EnhancedSpellbookType;
        [CommandProperty(AccessLevel.GameMaster)]
        public EnhancedSpellbookType EnhancedType
        {
            get { return m_EnhancedSpellbookType; }
            set { m_EnhancedSpellbookType = value; }
        }

        private SlayerGroupType m_SlayerGroup;  
        [CommandProperty(AccessLevel.GameMaster)]
        public SlayerGroupType SlayerGroup
        {
            get { return m_SlayerGroup; }
            set { m_SlayerGroup = value; }
        }
        
        [Constructable]
        public EnhancedSpellbookTome(EnhancedSpellbookType tomeType, SlayerGroupType slayerGroupType): base(0x0FBD)
        {
            Hue = 101;

            if (tomeType == null)
            {
                Delete();

                return;
            }

            else
            {
                m_EnhancedSpellbookType = tomeType;
                m_SlayerGroup = slayerGroupType;
            }

            if (m_SlayerGroup == null || m_SlayerGroup == SlayerGroupType.None)            
                Delete();
            
            else
                m_SlayerGroup = slayerGroupType;            
        }

        [Constructable]
        public EnhancedSpellbookTome(EnhancedSpellbookType tomeType): base(0x0FBD)
        {
            Hue = 101;

            if (tomeType == null)
            {
                Delete();

                return;
            }

            m_EnhancedSpellbookType = tomeType;
            
            if (m_EnhancedSpellbookType == EnhancedSpellbookType.Slayer)            
                m_SlayerGroup = BaseCreature.GetRandomSlayerType();   
        }

        [Constructable]
        public EnhancedSpellbookTome(): base(0x0FBD)
		{
            Hue = 101;

            string[] types = Enum.GetNames(typeof(EnhancedSpellbookType));
            int iType = Utility.Random(types.Length);
            string e = types[iType];

            EnhancedSpellbookType type = (EnhancedSpellbookType)Enum.Parse(typeof(EnhancedSpellbookType), e, true);

            if (type == null)
            {
                Delete();

                return;
            }

            else            
                m_EnhancedSpellbookType = type;           

            if (type == EnhancedSpellbookType.Slayer)            
                m_SlayerGroup = BaseCreature.GetRandomSlayerType();  
		}
                
        public EnhancedSpellbookTome(Serial serial): base(serial)
		{
		}   

        public override void OnDoubleClick(Mobile from)
        {
            PlayerMobile pm = from as PlayerMobile;

            if (pm != null)            
                CraftEnhancedSpellbook(pm);            
        }

        private void CraftEnhancedSpellbook(PlayerMobile pm)
        {
            pm.SendMessage("Select an empty spellbook to enhance with this tome");
            pm.BeginTarget(-1, false, TargetFlags.None, new TargetCallback(CraftEnhancedSpellbook_Callback));
        }

        private void CraftEnhancedSpellbook_Callback(Mobile from, object obj)
        {
            CraftEnhancedSpellbook_Callback(from, obj as Item, true);
        }

        private void CraftEnhancedSpellbook_Callback(Mobile from, Item item, bool target)
        {
            PlayerMobile player = from as PlayerMobile;
            Spellbook spellbook = item as Spellbook;

            //Player or Item is Invalid
            if (player == null || item == null)            
                return;            

            //Target Non-Enhanced Spellbook
            if (spellbook == null || !(item is EnhancedSpellbook))
            {
                player.SendMessage("You must target a basic spellbook.");
                return;
            }

            //Spellbook Isn't In Inventory or Equipped
            if (!spellbook.IsChildOf(player))
            {
                player.SendMessage("You must target a spellbook that you have equipped or that is in your backpack.");
                return;
            }

            //Spellbook Has Spells In It
            if (spellbook.SpellCount > 0)
            {
                player.SendMessage("You must target an empty spellbook.");
                return;
            }

            double inscriptionSkillRequired = 100;

            switch (m_EnhancedSpellbookType)
            {
                case EnhancedSpellbookType.Wizard: inscriptionSkillRequired = 80; break;
                case EnhancedSpellbookType.Warlock: inscriptionSkillRequired = 80; break;
                case EnhancedSpellbookType.Summoner: inscriptionSkillRequired = 80; break;

                case EnhancedSpellbookType.Fire: inscriptionSkillRequired = 90; break;
                case EnhancedSpellbookType.Energy: inscriptionSkillRequired = 90; break;

                case EnhancedSpellbookType.Slayer: inscriptionSkillRequired = 100; break;
            }

            //Player Has Insufficient Inscription
            if (player.Skills[SkillName.Inscribe].Value < inscriptionSkillRequired)
            {
                player.SendMessage("You lack the neccessary Inscription skill to use this tome");
                return;
            }

            EnhancedSpellbook enhancedSpellbook;

            if (m_EnhancedSpellbookType == EnhancedSpellbookType.Slayer)            
                enhancedSpellbook = new EnhancedSpellbook(EnhancedSpellbookType.Slayer, m_SlayerGroup);            

            else            
                enhancedSpellbook = new EnhancedSpellbook(m_EnhancedSpellbookType);            

            enhancedSpellbook.CraftedBy = player;
                      
            player.Backpack.AddItem(enhancedSpellbook);

            player.SendMessage("You craft an enhanced spellbook");
            from.PlaySound(0x249);

            spellbook.Delete();
            Delete();
        }

        public override string DefaultName
        {
            get
            {
                if (this.m_EnhancedSpellbookType == null)                
                    return "A Blank Tome";                

                else if (this.m_EnhancedSpellbookType == EnhancedSpellbookType.Slayer)                
                    return SlayerGroup.ToString();                

                else                
                    return EnhancedSpellbookTomeTypeAsString(this.m_EnhancedSpellbookType);                
            }
        }

        #region Name Conversions
        public static string EnhancedSpellbookTomeTypeAsString(EnhancedSpellbookType type)
        {
            switch (type)
            {
                case EnhancedSpellbookType.Wizard:
                    return "A Wizard Tome";

                case EnhancedSpellbookType.Warlock:
                    return "A Warlock Tome";

                case EnhancedSpellbookType.Fire:
                    return "A Fire Tome";

                case EnhancedSpellbookType.Energy:
                    return "An Energy Tome";

                case EnhancedSpellbookType.Summoner:
                    return "A Summoner Tome";

                default: return "A Blank Tome";
            }
        }

        #endregion

        public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version

            //Version 0
            writer.Write((int)m_EnhancedSpellbookType);
            writer.Write((int)m_SlayerGroup);            
		}

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();

            //Version 0
            if (version >= 0)
            {
                m_EnhancedSpellbookType = (EnhancedSpellbookType)reader.ReadInt();
                m_SlayerGroup = (SlayerGroupType)reader.ReadInt();
            }
        }
    }
}