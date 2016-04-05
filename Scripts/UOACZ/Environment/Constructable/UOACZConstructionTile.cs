using System;
using Server;
using Server.Mobiles;
using System.Collections;
using Server.Custom;


namespace Server.Items
{
    public class UOACZConstructionTile : Item
    {
        public enum ConstructionObjectType
        {
            Any,
            Fortification,
            Wall            
        }

        private ConstructionObjectType m_ConstructionTypeAllowed = ConstructionObjectType.Fortification;
        [CommandProperty(AccessLevel.GameMaster)]
        public ConstructionObjectType ConstructionTypeAllowed
        {
            get { return m_ConstructionTypeAllowed; }
            set
            {
                m_ConstructionTypeAllowed = value;

                switch (m_ConstructionTypeAllowed)
                {
                    case ConstructionObjectType.Any: Hue = 2500; break;
                    case ConstructionObjectType.Fortification: Hue = 1107; break;
                    case ConstructionObjectType.Wall: Hue = 2405; break;
                }
            }
        }

        private UOACZConstructable m_Constructable = null;
        [CommandProperty(AccessLevel.GameMaster)]
        public UOACZConstructable Constructable
        {
            get { return m_Constructable; }
            set 
            {
                m_Constructable = value;

                if (m_Constructable == null)
                    Visible = true;

                else if (m_Constructable.Deleted)
                    Visible = true;

                else
                    Visible = false;
            }
        }        

        private Direction m_ObjectFacing = Direction.North;
        [CommandProperty(AccessLevel.GameMaster)]
        public Direction ObjectFacing
        {
            get { return m_ObjectFacing; }
            set { m_ObjectFacing = value; }
        }

        [Constructable]
        public UOACZConstructionTile(): base(16778)		
		{
            Name = "construction location";

            Hue = 2405;

            Visible = true;
            Movable = false;     
		}

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, Name);

            switch (m_ConstructionTypeAllowed)
            {
                case ConstructionObjectType.Fortification:
                    LabelTo(from, "(fortification)");
                break;

                case ConstructionObjectType.Wall:
                    LabelTo(from, "(wall)");
                break;

                case ConstructionObjectType.Any:
                    LabelTo(from, "(any type)");
                break;
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            switch (m_ConstructionTypeAllowed)
            {
                case ConstructionObjectType.Fortification:
                    from.SendMessage("Target this location with a Fortification deed to begin construction.");
                break;

                case ConstructionObjectType.Wall:
                    from.SendMessage("Target this location with a Wall deed to begin construction.");
                break;

                case ConstructionObjectType.Any:
                    from.SendMessage("Target this location with a construction deed to begin construction.");
                break;
            }
        }

        public void PlaceObject(Mobile from, UOACZBaseConstructionDeed constructionDeed)
        {
            if (from == null || constructionDeed == null)
                return;

            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

            if (m_ConstructionTypeAllowed != ConstructionObjectType.Any)
            {
                if (constructionDeed.ConstructionType != m_ConstructionTypeAllowed)
                {
                    string objectType = "";

                    switch(m_ConstructionTypeAllowed)
                    {
                        case ConstructionObjectType.Fortification: objectType = "fortification"; break;
                        case ConstructionObjectType.Wall: objectType = "wall"; break;
                    }

                    player.SendMessage("Only " + objectType + " type construction objects may be placed at that location.");
                    return;
                }
            }            

            Type type = constructionDeed.ConstructableObject;

            UOACZConstructable itemToBuild = (UOACZConstructable)Activator.CreateInstance(type);

            if (itemToBuild == null)
                return;

            Constructable = itemToBuild;
            Constructable.ConstructionTile = this;

            Constructable.MoveToWorld(Location, Map);
            Constructable.Facing = m_ObjectFacing;
            Constructable.UpdateOverrides();
            Constructable.UpdateDamagedState();  

            player.PlaySound(0x3E5);
            player.SendMessage("You place the object in the construction slot. Use a repair hammer and restore it to full durability to activate it.");

            if (player.Backpack != null && constructionDeed.CraftedBy == player)
            {                               
                UOACZSystem.ChangeStat(player, UOACZSystem.UOACZStatType.HumanScore, UOACZSystem.HumanConstructableScorePoints, true);

                if (Utility.RandomDouble() <= UOACZSystem.HumanConstructableSurvivalStoneChance)
                {
                    player.Backpack.AddItem(new UOACZSurvivalStone(player));
                    player.SendMessage("You have earned a survival stone for your construction efforts!");
                }

                if (Utility.RandomDouble() <= UOACZSystem.HumanConstructableUpgradeTokenChance)
                {
                    player.Backpack.AddItem(new UOACZHumanUpgradeToken(player));
                    player.SendMessage("You have earned an upgrade token for your construction efforts!");
                }
            }

            constructionDeed.Delete();
        }

        public UOACZConstructionTile(Serial serial): base(serial)
        {   
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version

            writer.Write(m_Constructable);
            writer.Write((int)m_ObjectFacing);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Constructable = (UOACZConstructable)reader.ReadItem();
            m_ObjectFacing = (Direction)reader.ReadInt();
        }
    }
}