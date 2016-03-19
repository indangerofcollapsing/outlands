using System;

using Server.Network;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Items
{
    public class EnhancedSpellbookTome : Item
	{
        private EnhancedSpellbookType m_EnhancedSpellbookType;
        private SlayerName m_Slayer;        
        
        [CommandProperty(AccessLevel.GameMaster)]
        public EnhancedSpellbookType EnhancedType
        {
            get { return m_EnhancedSpellbookType; }
            set { m_EnhancedSpellbookType = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public SlayerName Slayer
        {
            get { return m_Slayer; }
            set { m_Slayer = value;}
        }
        
        [Constructable]
        public EnhancedSpellbookTome(EnhancedSpellbookType tomeType, SlayerName slayerType): base(0x0FBD)
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
                m_Slayer = slayerType;
            }

            if (m_Slayer == null || m_Slayer == SlayerName.None)            
                Delete();
            
            else            
                m_Slayer = slayerType;            
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
            {
                SlayerName slayer = (SlayerName)BaseRunicTool.GetRandomSlayer();

                if (slayer != null && slayer != SlayerName.None)                
                    m_Slayer = slayer;                

                else                
                    Delete();                
            }
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
            {
                SlayerName slayer = (SlayerName)BaseRunicTool.GetRandomSlayer();           

                if (slayer != null && slayer != SlayerName.None)                
                    m_Slayer = slayer;                

                else                
                    Delete();                
            }
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
            PlayerMobile pm = from as PlayerMobile;
            Spellbook spellbook = item as Spellbook;

            //Player or Item is Invalid
            if (pm == null || item == null)            
                return;            

            //Target Non-Enhanced Spellbook
            if (spellbook != null && !(item is EnhancedSpellbook))
            {
            }

            else
            {
                pm.SendMessage("You must target a basic spellbook.");
                return;
            }

            //Spellbook Isn't In Inventory or Equipped
            if (!spellbook.IsChildOf(pm))
            {
                pm.SendMessage("You must target a spellbook that you have equipped or that is in your backpack.");
                return;
            }

            //Spellbook Has Spells In It
            if (spellbook.SpellCount > 0)
            {
                pm.SendMessage("You must target an empty spellbook.");
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
            if (pm.Skills[SkillName.Inscribe].Value < inscriptionSkillRequired)
            {
                pm.SendMessage("You lack the neccessary Inscription skill to use this tome");
                return;
            }

            EnhancedSpellbook enhancedSpellbook;

            if (m_EnhancedSpellbookType == EnhancedSpellbookType.Slayer)            
                enhancedSpellbook = new EnhancedSpellbook(EnhancedSpellbookType.Slayer, m_Slayer);            

            else            
                enhancedSpellbook = new EnhancedSpellbook(m_EnhancedSpellbookType);            

            enhancedSpellbook.Crafter = pm;
            enhancedSpellbook.AddEnhancedScrolls();
                      
            pm.Backpack.AddItem(enhancedSpellbook);

            pm.SendMessage("You craft an enhanced spellbook");
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
                    return EnhancedSpellbookTomeTypeSlayerAsString(this.m_Slayer);                

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

        public static string EnhancedSpellbookTomeTypeSlayerAsString(SlayerName name)
        {
            switch(name.ToString())
            {
                case "":
                    return "A Slayer Tome";
                
                case "Silver":
                    return "A Silver Tome";

                case "OrcSlaying":
                    return "An Orc Slaying Tome";

                case "TrollSlaughter":
                    return "A Troll Slaughter Tome";

                case "OgreTrashing":
                    return "An Ogre Trashing Tome";

                case "Repond":
                    return "A Repond Tome";

                case "DragonSlaying":
                    return "A Dragon Slaying Tome";

                case "Terathan":
                    return "A Terathan Tome";

                case "SnakesBane":
                    return "A Snakes Bane Tome";

                case "LizardmanSlaughter":
                    return "A Lizardman Slaughter Tome";

                case "ReptilianDeath":
                    return "A Reptilian Death Tome";

                case "DaemonDismissal":
                    return "A Daemon Dismissal Tome";

                case "GargoylesFoe":
                    return "A Gargoyles Foe Tome";

                case "BalronDamnation":
                    return "A Balron Damnation Tome";

                case "Ophidian":
                    return "An Ophidian Tome";

                case "Exorcism":
                    return "An Exorcism Tome";

                case "SpidersDeath":
                    return "A Spiders Death Tome";

                case "ScorpionsBane":
                    return "A Scorpions Bane Tome";

                case "ArachnidDoom":
                    return "An Arachnid Doom Tome";

                case "FlameDousing":
                    return "A Flame Dousing Tome";

                case "WaterDissipation":
                    return "A Water Dissipation Tome";

                case "Vacuum":
                    return "A Vacuum Tome";

                case "ElementalHealth":
                    return "An Elemental Health Tome";

                case "EarthShatter":
                    return "An Earth Shatter Tome";

                case "BloodDrinking":
                    return "A Blood Drinking Tome";

                case "SummerWind":
                    return "A Summer Wind Tome";

                case "ElementalBan":
                    return "An Elemental Ban Tome";

                case "Fey":
                    return "A Fey Tome";
                
                default: return "A Slayer Tome";
            }            
        }
        #endregion

        public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version

            writer.Write((byte)m_Slayer);
            writer.Write((byte)m_EnhancedSpellbookType);
		}

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            //Version 0
            m_Slayer = (SlayerName)reader.ReadByte();
            m_EnhancedSpellbookType = (EnhancedSpellbookType)reader.ReadByte();           
 
            //----------

            Hue = 101;
        }
    }
}