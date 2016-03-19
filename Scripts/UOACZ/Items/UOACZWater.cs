using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Spells;
using Server.Mobiles;

namespace Server.Items
{
    public class UOACZWater : Item
    {
        private int m_Charges;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges
        {
            get { return m_Charges; }
            set 
            { 
                m_Charges = value;

                if (m_Charges == 0)
                    Delete();

                if (m_Charges == 1)
                    ItemID = 4652;

                if (m_Charges == 2)
                    ItemID = 4654;

                if (m_Charges == 3)
                    ItemID = 4651;

                if (m_Charges == 4)
                    ItemID = 4653;

                if (m_Charges == 5)
                    ItemID = 4650;
            }
        }

        public UOACZWaterSpawner m_Spawner;

        [Constructable]
        public UOACZWater() : base(4651)
        {
            Name = "fresh ground water";

            Hue = 2591;

            switch (Utility.RandomMinMax(1, 10))
            {
                case 1: Charges = 1; break;
                case 2: Charges = 2; break;
                case 3: Charges = 2; break;
                case 4: Charges = 2; break;
                case 5: Charges = 3; break;
                case 6: Charges = 3; break;
                case 7: Charges = 3; break;
                case 8: Charges = 4; break;
                case 9: Charges = 4; break;
                case 10: Charges = 5; break;
            }

            Movable = false;
        }

        public UOACZWater(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            LabelTo(from, "(double click to gather)");
        }
        
        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            PlayerMobile player = from as PlayerMobile;

            if (!UOACZSystem.IsUOACZValidMobile(player)) return;
            if (!(player.IsUOACZHuman)) return;

            if (Utility.GetDistance(player.Location, Location) > 2)
            {
                from.SendMessage("You are too far away to use that.");
                return;
            }

            if (!Map.InLOS(player.Location, Location))
            {
                from.SendMessage("That is not within your line of sight.");
                return;
            }

            if (player.Backpack == null)
                return;

            bool foundContainer = false;

            Item[] items = player.Backpack.FindItemsByType(typeof(UOACZWaterTub));

            if (!foundContainer)
            {
                foreach (Item item in items)
                {
                    UOACZWaterTub waterContainer = item as UOACZWaterTub;

                    if (waterContainer.Charges < waterContainer.MaxCharges)
                    {
                        waterContainer.Charges++;

                        foundContainer = true;
                        player.SendMessage("You gather the water and place it into a water tub.");

                        break;
                    }
                }
            }

            if (!foundContainer)
            {
                items = player.Backpack.FindItemsByType(typeof(UOACZTub));

                UOACZTub waterContainer = null;

                foreach (Item item in items)
                {
                    waterContainer = item as UOACZTub;
                    foundContainer = true;
                    break;
                }

                if (waterContainer != null)
                {
                    int oldX = waterContainer.X;
                    int oldY = waterContainer.Y;

                    if (waterContainer != null)
                    {
                        waterContainer.Delete();

                        UOACZWaterTub newWaterTub = new UOACZWaterTub();
                        newWaterTub.Charges = 1;

                        player.Backpack.DropItem(newWaterTub);

                        newWaterTub.X = oldX;
                        newWaterTub.Y = oldY;

                        player.SendMessage("You gather the water and place it into an empty tub.");
                    }
                }
            }

            if (!foundContainer)
            {
                items = player.Backpack.FindItemsByType(typeof(UOACZBottleOfWater));

                foreach (Item item in items)
                {
                    UOACZBottleOfWater waterContainer = item as UOACZBottleOfWater;

                    if (waterContainer.Charges < waterContainer.MaxCharges)
                    {
                        waterContainer.Charges++;

                        foundContainer = true;
                        player.SendMessage("You gather the water and add it to an existing bottle of water.");

                        break;
                    }
                }
            }            

            if (!foundContainer)
            {
                Item item = player.Backpack.FindItemByType(typeof(Bottle));

                if (item != null)
                {
                    Bottle bottle = item as Bottle;

                    if (bottle.Amount == 1)
                        bottle.Delete();

                    else
                        bottle.Amount--;

                    UOACZBottleOfWater newBottleOfWater = new UOACZBottleOfWater();

                    player.Backpack.DropItem(newBottleOfWater);

                    foundContainer = true;
                    player.SendMessage("You gather the water and place it into an empty bottle.");
                }                
            }

            if (!foundContainer)
            {
                items = player.Backpack.FindItemsByType(typeof(UOACZGlass));

                UOACZGlass waterContainer = null;

                foreach (Item item in items)
                {
                    waterContainer = item as UOACZGlass;

                    foundContainer = true;

                    break;
                }

                if (waterContainer != null)
                {
                    int oldX = waterContainer.X;
                    int oldY = waterContainer.Y;

                    if (waterContainer != null)
                    {
                        waterContainer.Delete();

                        UOACZGlassOfWater newWaterGlass = new UOACZGlassOfWater();
                        newWaterGlass.Charges = 1;

                        player.Backpack.DropItem(newWaterGlass);

                        newWaterGlass.X = oldX;
                        newWaterGlass.Y = oldY;

                        player.SendMessage("You gather the water and place it into an empty glass.");
                    }
                }
            }

            if (foundContainer)
            {
                player.Animate(32, 5, 1, true, false, 0);
                player.PlaySound(0x4d1);

                Charges--;

                UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);
                player.m_UOACZAccountEntry.ScavengeableWaterItems++;
            }

            else            
                player.SendMessage("You must have a empty bottle, empty glass, or water tub available in order to gather water.");
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (m_Spawner != null)
                m_Spawner.m_Items.Remove(this);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version  
        
            //Version 0
            writer.Write(m_Charges);
            writer.Write(m_Spawner);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_Charges = reader.ReadInt();
                m_Spawner = (UOACZWaterSpawner)reader.ReadItem();
            }
        }
    }
}