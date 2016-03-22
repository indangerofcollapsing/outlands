using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
    [FlipableAttribute(0x1441, 0x1440)]
    public class UOACZSurvivalMachete : BaseSword
    {
        public override int BaseMinDamage { get { return 5; } }
        public override int BaseMaxDamage { get { return 10; } }
        public override int BaseSpeed { get { return 50; } }

        public override int InitMinHits { get { return 500; } }
        public override int InitMaxHits { get { return 500; } }

        public override int BaseHitSound { get { return 0x23B; } }
        public override int BaseMissSound { get { return 0x23A; } }

        [Constructable]
        public UOACZSurvivalMachete(): base(0x1441)
        {
            Name = "a survival machete";
                        
            Hue = 2575;

            Weight = 2.0;

            LootType = LootType.Blessed;
        }

        public UOACZSurvivalMachete(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}