using System;
using Server.Network;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.Targeting;
using Server.Spells;
using Server.Gumps;

namespace Server.Custom
{
    public class ResearchMaterials : Item
    {
        private bool m_Researched = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Researched
        {
            get { return m_Researched; }
            set 
            {
                m_Researched = value;

                if (value == true)
                    Hue = 2550;
                else
                    Hue = 0;
            }
        }  

        [Constructable]
        public ResearchMaterials(): base(7187)
        {
            Name = "research materials";
            Hue = 0;

            Weight = 1.0;
        }

        public ResearchMaterials(Serial serial): base(serial)
        {
        }
        
        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            if (m_Researched)
                LabelTo(from, "(researched)");
        }        

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
                from.SendMessage("This must be in your pack in order to use it.");
            else
            {
                if (m_Researched)
                {
                    from.SendMessage("These research materials may now be applied towards an ancient mystery scroll.");
                    return;
                }

                else
                    AttemptResearch(from);
            }
        }

        public void AttemptResearch(Mobile from)
        {
            if (m_Researched)
            {
                from.SendMessage("You have already researched this.");
                return;
            }

            if (from.Skills.Forensics.Value < 95)
            {
                from.SendMessage("You do not have enough forensics evaluation skill to make an appropriate research attempt on these materials.");
                return;
            }

            if (from.CheckTargetSkill(SkillName.Forensics, this, 95, 110.0, 1.0))      
            {
                Researched = true;            

                from.SendSound(0x652);
                from.SendMessage("You learn many a great secret whilst prying through the words held within. You may now apply this research to an ancient mystery scroll.");

                return;
            }

            else
            {
                if (Utility.RandomDouble() <= .25)
                {
                    from.SendSound(0x5AE);
                    from.SendMessage("The ancient parchments crumble to dust before your very eyes and are lost!");

                    Delete();
                }

                else
                {
                    from.SendSound(0x055);
                    from.SendMessage("You diligently sift through the research materials, but are unable to glean any secrets held within.");
                }
                
                return;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version

            //Version 0
            writer.Write(m_Researched);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_Researched = reader.ReadBool();
            }
        }
    }    
}
