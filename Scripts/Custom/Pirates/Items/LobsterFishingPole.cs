/***************************************************************************
 *                            NavalCrossbow.cs
 *                            ------------------
 *   begin                : August 2010
 *   author               : Sean Stavropoulos
 *   email                : sean.stavro@gmail.com
 *
 *
 ***************************************************************************/
using System;
using System.Collections;
using Server.Network;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Items;
using Server.Custom.Pirates;
using Server.Multis;

namespace Server.Items
{
    public class LobsterFishingPole : FishingPole
    {
        public override string DefaultName { get { return "a lobster fishing pole"; } }

        private int m_Bonus;
        private SkillMod m_SkillMod;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Bonus
        {
            get
            {
                return m_Bonus;
            }
            set
            {
                m_Bonus = value;
                InvalidateProperties();

                if (m_Bonus == 0)
                {
                    if (m_SkillMod != null)
                        m_SkillMod.Remove();

                    m_SkillMod = null;
                }
                else if (m_SkillMod == null && Parent is Mobile)
                {
                    m_SkillMod = new DefaultSkillMod(SkillName.Fishing, true, m_Bonus);
                    ((Mobile)Parent).AddSkillMod(m_SkillMod);
                }
                else if (m_SkillMod != null)
                {
                    m_SkillMod.Value = m_Bonus;
                }
            }
        }

        public override void OnAdded(object parent)
        {
            base.OnAdded(parent);

            if (m_Bonus != 0 && parent is Mobile)
            {
                if (m_SkillMod != null)
                    m_SkillMod.Remove();

                m_SkillMod = new DefaultSkillMod(SkillName.Fishing, true, m_Bonus);
                ((Mobile)parent).AddSkillMod(m_SkillMod);
            }
        }

        public override void OnRemoved(object parent)
        {
            base.OnRemoved(parent);

            if (m_SkillMod != null)
                m_SkillMod.Remove();

            m_SkillMod = null;
        }

        [Constructable]
        public LobsterFishingPole()
            : base()
        {
            m_Bonus = 5;
            Hue = 1194;
        }

        public LobsterFishingPole(Serial serial)
            : base(serial)
        {

        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            writer.Write((int)m_Bonus); 
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        m_Bonus = reader.ReadInt();
                        break;
                    }
            }

            if (m_Bonus != 0 && Parent is Mobile)
            {
                if (m_SkillMod != null)
                    m_SkillMod.Remove();

                m_SkillMod = new DefaultSkillMod(SkillName.Fishing, true, m_Bonus);
                ((Mobile)Parent).AddSkillMod(m_SkillMod);
            }
        }
    }
}