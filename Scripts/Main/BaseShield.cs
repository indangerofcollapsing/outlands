using System;
using System.Collections;
using Server;
using Server.Network;
using Server.Mobiles;

namespace Server.Items
{
    public class BaseShield : BaseArmor
    {
        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Plate; } }

        public static double ShieldParrySkillScalar = .005;
        public static double ShieldParryDamageScalar = .25;
        public static double DurabilityLossChance = .1;

        public BaseShield(int itemID): base(itemID)
        {
        }

        public BaseShield(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        public override double ArmorRating
        {
            get
            {
                Mobile mobile = Parent as Mobile;

                double baseAR = base.ArmorRating;

                if (ProtectionLevel != ArmorProtectionLevel.Regular)
                    baseAR += (3 * (int)ProtectionLevel);

                switch (Resource)
                {
                    case CraftResource.DullCopper: baseAR += 1; break;
                    case CraftResource.ShadowIron: baseAR += 2; break;
                    case CraftResource.Copper: baseAR += 3; break;
                    case CraftResource.Bronze: baseAR += 4; break;
                    case CraftResource.Gold: baseAR += 5; break;
                    case CraftResource.Agapite: baseAR += 6; break;
                    case CraftResource.Verite: baseAR += 7; break;
                    case CraftResource.Valorite: baseAR += 8; break;
                    case CraftResource.Lunite: baseAR += 9; break;
                }

                baseAR += -6 + (6 * (int)Quality);

                double finalAR = (baseAR * .5) + ((baseAR * .5) * (mobile.Skills[SkillName.Parry].Value / 100));

                if (finalAR < 1)
                    finalAR = 1;

                if (mobile != null)
                    return finalAR;
                else
                    return baseAR;
            }
        }

        public override int OnHit(BaseWeapon weapon, int damage)
        {
            return OnHit(weapon, damage, null);
        }

        public int OnHit(BaseWeapon weapon, int damage, Mobile attacker)
        {
            Mobile owner = this.Parent as Mobile;

            if (owner == null)
                return damage;

            if (DecorativeEquipment)
                return damage;

            double successChance = owner.Skills[SkillName.Parry].Value * ShieldParrySkillScalar;

            if (owner.CheckSkill(SkillName.Parry, successChance, 1.0))
            {
                damage = (int)(Math.Round((double)damage * ShieldParryDamageScalar));

                if (damage < 1)
                    damage = 1;

                owner.FixedEffect(0x37B9, 10, 16);

                if (Utility.RandomDouble() <= DurabilityLossChance && LootType != LootType.Blessed && MaxHitPoints > 0)
                {
                    if (HitPoints > 1)
                    {
                        HitPoints--;

                        if (HitPoints == 5)
                        {
                            if (Parent is Mobile)
                                ((Mobile)Parent).LocalOverheadMessage(MessageType.Regular, 0x3B2, 1061121); // Your equipment is severely damaged.
                        }
                    }

                    else
                        Delete();
                }  
            }

            return damage;
        }        
    }
}
