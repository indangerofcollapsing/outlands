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

        public BaseShield(int itemID)
            : base(itemID)
        {
        }

        public BaseShield(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version < 1)
            {
                if (this is Aegis)
                    return;

                // The 15 bonus points to resistances are not applied to shields on OSI.
                PhysicalBonus = 0;
                FireBonus = 0;
                ColdBonus = 0;
                PoisonBonus = 0;
                EnergyBonus = 0;
            }
        }

        public override double ArmorRating
        {
            get
            {
                Mobile m = this.Parent as Mobile;

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

                double parryingSkill = m.Skills[SkillName.Parry].Value;
                double finalAR = (baseAR / 2) + ((baseAR / 2) * (m.Skills[SkillName.Parry].Value / 100));

                if (finalAR < 1)
                    finalAR = 1;

                if (m != null)
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

            double parrySkill = owner.Skills[SkillName.Parry].Value;
            double baseChance = (parrySkill / 100.0) / 2;
            double chanceBonus = .167 * (owner.Skills[SkillName.ArmsLore].Value / 100);

            if (attacker is PlayerMobile)
                chanceBonus = 0;

            double chance = baseChance + chanceBonus;

            if (chance < 0.01)
                chance = 0.01;            

            //Successful Parry Check
            if (owner.CheckSkill(SkillName.Parry, chance))
            {
                double reduction = 0.40; // 50% base reduction

                // reduce 5% further for each mod
                if (Quality == ArmorQuality.Exceptional)
                    reduction -= 0.06;

                else
                    reduction -= (int)ProtectionLevel * 0.06;

                damage = (int)(damage * reduction);

                if (owner.AccessLevel > AccessLevel.Player)
                    owner.PrivateOverheadMessage(Network.MessageType.Regular, 0x22, false, String.Format("Parry Damage Modifier: {0:0.00}", 1-reduction), owner.NetState);

                //Parry Visual Effect
                owner.FixedEffect(0x37B9, 10, 16);

                //(25% Chance)
                if (25 > Utility.Random(100) && LootType != Server.LootType.Blessed)
                {
                    int wear = Utility.Random(2);
                    wear = GetGuildReducedDamage(wear);

                    //66% Chance of Possible Durability Damage
                    if (wear > 0 && MaxHitPoints > 0)
                    {
                        //Durability Damage
                        if (HitPoints >= wear)
                        {
                            HitPoints -= wear;
                            wear = 0;
                        }

                        else
                        {
                            wear -= HitPoints;
                            HitPoints = 0;
                        }

                        if (wear > 0)
                        {
                            //Shield Almost Broken
                            if (MaxHitPoints > wear)
                            {
                                MaxHitPoints -= wear;

                                if (Parent is Mobile)
                                    ((Mobile)Parent).LocalOverheadMessage(MessageType.Regular, 0x3B2, 1061121); // Your equipment is severely damaged.
                            }

                            //Shield Breaks
                            else
                            {
                                Delete();
                            }
                        }
                    }
                }
            }

            return damage;
        }        
    }
}
