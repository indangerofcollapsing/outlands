/***************************************************************************
 *                               FishermanKnife.cs
 *                            -------------------
 *   begin                : May 23, 2014
 *   author               : Patt Rojanasthien
 *   email                : frostshoxx@gmail.com
 *
 *
 ***************************************************************************/
using Server.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Items
{
    public class FishermanKnife : Cleaver, IUsesRemaining
    {
        private int m_UsesRemaining;
        private bool m_ShowUsesRemaining;

        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining
        {
            get { return m_UsesRemaining; }
            set { m_UsesRemaining = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool ShowUsesRemaining
        {
            get { return m_ShowUsesRemaining; }
            set { m_ShowUsesRemaining = value; InvalidateProperties(); }
        }

        [Constructable]
        public FishermanKnife()
            : base()
        {
            Hue = 1247;
            Name = "fisherman's knife";
            //LootType = Server.LootType.Blessed; //
            UsesRemaining = 50;
            ShowUsesRemaining = true;
        }

        public FishermanKnife(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write((bool)m_ShowUsesRemaining);

            writer.Write((int)m_UsesRemaining);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            //Add the serialization versioning just in case we expand this after it goes live.
            switch (version)
            {
                case 0:
                    {
                        m_ShowUsesRemaining = reader.ReadBool();
                        m_UsesRemaining = reader.ReadInt();

                        if (m_UsesRemaining < 1)
                            m_UsesRemaining = 1;
                        break;
                    }
            }
        }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, "Durability : {0}", this.UsesRemaining);
            LabelTo(from, "fisherman's knife");
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.SendLocalizedMessage(1010018); // What do you want to use this item on?

            from.Target = new DisposeSwimmableCorpseTarget(this);
        }
    }
}
