using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using System.Collections;
using System.Collections.Generic;

namespace Server.Custom
{
    public class PetBattleRewardTotem : Item
    {
        public Mobile m_Owner;
        public Type m_CreatureType;
        public string m_CreatureName;

        public int m_CreatureAngerSound;
        public int m_CreatureIdleSound;
        public int m_CreatureAttackSound;

        public PetBattleRewardTotemCreature m_RewardTotem;

        [Constructable]
        public PetBattleRewardTotem(Mobile owner, Type creatureType, string creatureName): base(0x1223)
        {
            Name = "a pet battle reward totem";

            Weight = 1;

            m_Owner = owner;
            m_CreatureType = creatureType;
            m_CreatureName = creatureName;
        }

        public PetBattleRewardTotem(Serial serial): base(serial)
        {
        }

        public override bool OnDragLift(Mobile from)
        {
            if (m_RewardTotem != null)
            {
                if (!m_RewardTotem.Deleted)
                    m_RewardTotem.Delete();
            }

            return true;
        }

        public override void OnSingleClick(Mobile from)
        {
            bool buildNewCreature = false;

            if (m_RewardTotem == null)
                buildNewCreature = true;

            else
            {
                if (m_RewardTotem.Deleted)
                    buildNewCreature = true;
            }

            NetState ns = from.NetState;

            if (ns != null)
            {
                ns.Send(new UnicodeMessage(Serial, ItemID, MessageType.Label, 0, 3, "ENU", "", m_Owner.RawName));
                ns.Send(new UnicodeMessage(Serial, ItemID, MessageType.Label, 0, 3, "ENU", "", m_CreatureName + " Mastery"));
            }

            if (buildNewCreature)
                from.SendMessage("Place this statue on the ground and double click it to activate it.");
        }

        public override void OnDoubleClick(Mobile from)
        {
            bool buildNewCreature = false;

            if (m_RewardTotem == null)
                buildNewCreature = true;

            else
            {
                if (m_RewardTotem.Deleted)
                    buildNewCreature = true;
            }

            if (!buildNewCreature || m_CreatureType == null)
                return;

            if (RootParentEntity != null)
            {
                from.SendMessage("This item must be placed on the ground to be activated.");
                return;
            }

            BaseCreature creature = (BaseCreature)Activator.CreateInstance(m_CreatureType);

            PetBattleRewardTotemCreature newCreatureTotem = new PetBattleRewardTotemCreature();

            newCreatureTotem.Visible = false;

            newCreatureTotem.ItemID = creature.PetBattleItemId;
            newCreatureTotem.Hue = creature.PetBattleItemHue;
            m_CreatureName = creature.PetBattleTitle;

            m_CreatureAngerSound = creature.GetAngerSound();
            m_CreatureIdleSound = creature.GetIdleSound();
            m_CreatureAttackSound = creature.GetAttackSound();

            newCreatureTotem.MoveToWorld(Location, Map);
            newCreatureTotem.Z = this.Z + creature.PetBattleStatueOffsetZ - 10;

            newCreatureTotem.Visible = true;

            m_RewardTotem = newCreatureTotem;
            newCreatureTotem.m_RewardTotem = this;

            Effects.PlaySound(Location, Map, Utility.RandomList(m_CreatureAngerSound));
        }

        public override void OnDelete()
        {
            if (m_RewardTotem != null)
            {
                if (!m_RewardTotem.Deleted)
                    m_RewardTotem.Delete();
            }

            base.OnDelete();            
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version  

            writer.Write(m_Owner);
            writer.Write(m_CreatureType.ToString());
            writer.Write(m_CreatureName);

            writer.Write((Item)m_RewardTotem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            
             string strCreatureType = "";

            if (version >= 0)
            {
                m_Owner = reader.ReadMobile();
                strCreatureType = reader.ReadString();
                m_CreatureName = reader.ReadString();

                m_RewardTotem = (PetBattleRewardTotemCreature)reader.ReadItem();                
            }

            Type type = Type.GetType(strCreatureType);

            if (type != null)            
                m_CreatureType = type;            
        }
    }
    
    public class PetBattleRewardTotemCreature : Item
    {
        public PetBattleRewardTotem m_RewardTotem;

        [Constructable]
        public PetBattleRewardTotemCreature(): base(9743)
        {          
        }

        public PetBattleRewardTotemCreature(Serial serial): base(serial)
        {
        }

        public override bool OnDragLift(Mobile from)
        {
            this.Delete();

            return false;
        }

        public override void OnSingleClick(Mobile from)
        {
            if (m_RewardTotem != null)
            {
                if (!m_RewardTotem.Deleted)
                {
                    NetState ns = from.NetState;

                    if (ns != null)
                    {
                        ns.Send(new UnicodeMessage(Serial, ItemID, MessageType.Label, 0, 3, "ENU", "", m_RewardTotem.m_Owner.RawName));
                        ns.Send(new UnicodeMessage(Serial, ItemID, MessageType.Label, 0, 3, "ENU", "", m_RewardTotem.m_CreatureName + " Mastery"));
                    }
                }
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (m_RewardTotem != null)
            {
                if (!m_RewardTotem.Deleted)
                    Effects.PlaySound(Location, Map, Utility.RandomList(m_RewardTotem.m_CreatureAngerSound, m_RewardTotem.m_CreatureIdleSound, m_RewardTotem.m_CreatureAttackSound));
            }
        }

        public override void OnDelete()
        {
            if (m_RewardTotem != null)            
                m_RewardTotem.m_RewardTotem = null;            

            base.OnDelete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version 

            writer.Write(m_RewardTotem);
        }       

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_RewardTotem = (PetBattleRewardTotem)reader.ReadItem();
        }
    }    
}