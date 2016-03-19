using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
    [FlipableAttribute(0x1441, 0x1440)]
    public class UOACZSurvivalMachete : BaseSword
    {
        public override WeaponAbility PrimaryAbility { get { return WeaponAbility.BleedAttack; } }
        public override WeaponAbility SecondaryAbility { get { return WeaponAbility.ShadowStrike; } }

        public override int AosStrengthReq { get { return 25; } }
        public override int AosMinDamage { get { return 11; } }
        public override int AosMaxDamage { get { return 13; } }
        public override int AosSpeed { get { return 44; } }
        public override float MlSpeed { get { return 2.50f; } }

        public override int OldStrengthReq { get { return 10; } }
        public override int OldMinDamage { get { return 5; } }
        public override int OldMaxDamage { get { return 10; } }
        public override int OldSpeed { get { return 54; } }

        public override int DefHitSound { get { return 0x23B; } }
        public override int DefMissSound { get { return 0x23A; } }

        public override int InitMinHits { get { return 500; } }
        public override int InitMaxHits { get { return 500; } }

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