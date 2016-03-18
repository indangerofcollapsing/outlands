using System;
using Server;
using Server.Mobiles;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Items;

namespace Server.Custom
{
    public class DoubloonGeneratorStone : Item
    {                
        [Constructable]
        public DoubloonGeneratorStone(): base(0x0EDE)
        {
            Name = "a doubloon generator stone";
            Movable = false;
        }

        public DoubloonGeneratorStone(Serial serial): base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            int amount = 5000;

            if (Banker.DepositUniqueCurrency(from, typeof(Doubloon), amount))
            {
                Doubloon doubloonPile = new Doubloon(amount);
                from.SendSound(doubloonPile.GetDropSound());
                doubloonPile.Delete();

                from.SendMessage(amount.ToString() + " doubloons have been placed in your bank box.");
            }

            else
                from.SendMessage("There was no available space to place the doubloons in your bank box.");   
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