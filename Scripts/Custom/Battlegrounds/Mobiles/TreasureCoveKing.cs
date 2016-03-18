using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Items;

namespace Server.Custom.Battlegrounds.Mobiles
{
    [CorpseName("the pirate king's corpse")]
    public class TreasureCoveKing : BattlegroundKing
    {
        [Constructable]
        public TreasureCoveKing()
            : base()
        {
            Body = 400;
            Name = "Dread Captain Elop";

            EquipItem(new DreadPirateHat() { Movable = false });
            EquipItem(new FancyShirt() { Movable = false });
            EquipItem(new LeatherGloves() { Movable = false });
            EquipItem(new LongPants() { Movable = false, Hue = 1175 });
            EquipItem(new Boots() { Movable = false, Hue = 1 });
            EquipItem(new Cutlass() { Movable = false });
        }

        public TreasureCoveKing(Serial serial)
            : base(serial)
        {

        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}
