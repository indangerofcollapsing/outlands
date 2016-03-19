using System;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Gumps;
using Server.Custom;
using System.Collections;
using System.Collections.Generic;

namespace Server.Items
{
    public class CustomAlchemyPotion : Item
    {
        #region Properties

        private CustomAlchemy.EffectType m_PrimaryEffect = CustomAlchemy.EffectType.AccuracyIncrease;
        [CommandProperty(AccessLevel.GameMaster)]
        public CustomAlchemy.EffectType PrimaryEffect
        {
            get { return m_PrimaryEffect; }
            set { m_PrimaryEffect = value; }
        }

        private CustomAlchemy.EffectType m_SecondaryEffect = CustomAlchemy.EffectType.BleedDamage;
        [CommandProperty(AccessLevel.GameMaster)]
        public CustomAlchemy.EffectType SecondaryEffect
        {
            get { return m_SecondaryEffect; }
            set { m_SecondaryEffect = value; }
        }

        private bool m_PositiveEffect = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool PositiveEffect
        {
            get { return m_PositiveEffect; }
            set { m_PositiveEffect = value; }
        }

        private CustomAlchemy.EffectPotencyType m_EffectPotency = CustomAlchemy.EffectPotencyType.Target;
        [CommandProperty(AccessLevel.GameMaster)]
        public CustomAlchemy.EffectPotencyType EffectPotency
        {
            get { return m_EffectPotency; }
            set { m_EffectPotency = value; }
        }

        private bool m_Identified = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Identified
        {
            get { return m_Identified; }
            set { m_Identified = value; }
        }

        #endregion

        [Constructable]
        public CustomAlchemyPotion(): base(6211) //6189
        {
            Name = "custom alchemy potion";

            Hue = 0;
            Weight = 1;
        }

        public CustomAlchemyPotion(Serial serial): base(serial)
        {
        }

        public string GetPotionName()
        {
            return CustomAlchemy.GetPotionName(m_PrimaryEffect, m_SecondaryEffect, m_PositiveEffect, m_EffectPotency);
        }

        public override void OnSingleClick(Mobile from)
        {
            string name = GetPotionName().ToLower();

            if ((name == "" || !m_Identified) && from.AccessLevel == AccessLevel.Player)
                LabelTo(from, "an unknown custom alchemy potion");

            else
                LabelTo(from, name);
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("That item must be in your pack in order to use it.");
                return;
            }

            if (!Identified && from.AccessLevel == AccessLevel.Player)
            {
                from.SendMessage("This potion is of unknown composition and must be analyzed with Taste Identification before it may properly utilized.");
                return;
            }

            CustomAlchemy.UsePotion(this, from);
        }
        
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version    

            writer.Write((int)m_PrimaryEffect);
            writer.Write((int)m_SecondaryEffect);
            writer.Write(m_PositiveEffect);
            writer.Write((int)m_EffectPotency);            

            writer.Write(m_Identified);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_PrimaryEffect = (CustomAlchemy.EffectType)reader.ReadInt();
                m_SecondaryEffect = (CustomAlchemy.EffectType)reader.ReadInt();
                m_PositiveEffect = reader.ReadBool();
                m_EffectPotency = (CustomAlchemy.EffectPotencyType)reader.ReadInt();

                m_Identified = reader.ReadBool();
            }
        }
    }
}