using System;
using Server;
using Server.Mobiles;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Items;

namespace Server.Custom
{
    public class UOACZTomeGenerator : Item
    {                
        [Constructable]
        public UOACZTomeGenerator(): base(0x0EDE)
        {
            Name = "a UOACZ tome generator stone";

            Hue = 2405;
            Movable = false;
        }

        public UOACZTomeGenerator(Serial serial): base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from == null) return;
            if (from.Backpack == null) return;

            Item[] survivalTomes = from.Backpack.FindItemsByType(typeof(UOACZSurvivalTome));
            Item[] corruptionTomes = from.Backpack.FindItemsByType(typeof(UOACZCorruptionTome));

            if (survivalTomes.Length < 1)
            {
                from.Backpack.DropItem(new UOACZSurvivalTome());
                from.SendMessage("You receive a UOACZ Survival Tome.");
            }

            else
                from.SendMessage("You already have a Survival Tome in your backpack.");

            if (corruptionTomes.Length < 1)
            {
                from.Backpack.DropItem(new UOACZCorruptionTome());
                from.SendMessage("You receive a UOACZ Corruption Tome.");
            }
            else
                from.SendMessage("You already have a Corruption Tome in your backpack.");
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version          
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}