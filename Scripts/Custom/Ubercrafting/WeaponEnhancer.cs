using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Server;
using Server.Items;
using Server.Gumps;
using Server.Network;
using Server.Targeting;
using Server.Targets;

namespace Server.Custom.Ubercrafting
{
    public static class WeaponEnhancement
    {
        public enum EWeaponEnhancement
        {
            None = 0,
            Overwhelming = 1,	// +dmg vs non player
            Conquering = 2,		// +dmg vs non player
            Eradication = 3,	// +dmg vs non player
        }

        public static string AsString(EWeaponEnhancement e)
        {
            switch (e)
            {
                case EWeaponEnhancement.Overwhelming:
                    return "+10% Monster Damage";
                    break;

                case EWeaponEnhancement.Conquering:
                    return "+20% Monster Damage";

                    break;
                case EWeaponEnhancement.Eradication:
                    return "+30% Monster Damage";
                    break;

                default:
                    return String.Empty;
                    break;
            }
        }
    }

    abstract class WeaponEnhancer : EnhanceItemBase
    {
        public WeaponEnhancer(int id) : base(id) { }
        public WeaponEnhancer(Serial serial) : base(serial) { }

        public override SkillName RequiredSkill(Item i)
        {
            if (i is BaseStaff || i is Club)
                return SkillName.Carpentry;
            else if (i is BaseBashing)
                return SkillName.Blacksmith;
            else if (i is BaseAxe)
                return SkillName.Blacksmith;
            else if (i is BaseKnife)
                return SkillName.Blacksmith;
            else if (i is BasePoleArm)
                return SkillName.Blacksmith;
            else if (i is BaseSword)
                return SkillName.Blacksmith;
            else if (i is BaseRanged)
                return SkillName.Fletching;
            else if (i is BaseSpear)
                return SkillName.Blacksmith;
            else
                return SkillName.Ninjitsu; // ...
        }

        public override double RequiredSkillLevel(Item i)
        {
            return 100.0;
        }

        public override bool ValidTarget(Item i)
        {
            return i is BaseMeleeWeapon && !(i is Fists) && i.LootType != Server.LootType.Blessed && i.LootType != Server.LootType.Newbied;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }

    class WeaponDamageEnhancer : WeaponEnhancer
    {
        public override string DefaultName
        {
            get
            {
                return "a weapon enhancer (bonus damage against creatures)";
            }
        }

        [Constructable]
        public WeaponDamageEnhancer() : base(0x186C) { }
        public WeaponDamageEnhancer(Serial serial) : base(serial) { }

        public override bool Enhance(Item i, Mobile enhancer)
        {
            // check for existing enhancements.
            BaseWeapon weapon = i as BaseWeapon;

            if (weapon == null)
                return false;

            if (weapon.Quality != WeaponQuality.Exceptional || weapon.Slayer != SlayerName.None || weapon.Slayer2 != SlayerName.None || weapon.DamageLevel != WeaponDamageLevel.Regular || weapon.AccuracyLevel != WeaponAccuracyLevel.Regular)
            {
                enhancer.SendMessage("Only non-magical, non-slayer weapons crafted by a grandmaster may be enhanced.");

                return false;
            }

            switch (weapon.UOACWeaponAttribute)
            {
                case WeaponEnhancement.EWeaponEnhancement.Eradication:
                    enhancer.SendMessage("That item cannot be further enhanced.");
                    return false;
                break;
                case WeaponEnhancement.EWeaponEnhancement.None:
                    // to overwhelming
                    weapon.UOACWeaponAttribute = WeaponEnhancement.EWeaponEnhancement.Overwhelming;
                    return true;
                break;
                case WeaponEnhancement.EWeaponEnhancement.Overwhelming:
                    // to Conquering
                    weapon.UOACWeaponAttribute = WeaponEnhancement.EWeaponEnhancement.Conquering;
                    return true;
                break;
                case WeaponEnhancement.EWeaponEnhancement.Conquering:
                    // to Eradication
                    weapon.UOACWeaponAttribute = WeaponEnhancement.EWeaponEnhancement.Eradication;
                    return true;
                break;
                default:
                    // Item has another enhancement, can't overwrite.
                    enhancer.SendMessage("That item can not be further enhanced");
                    return false;
                break;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }
}
