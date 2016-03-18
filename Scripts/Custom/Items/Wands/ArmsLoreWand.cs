using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;

namespace Server.Items
{
    public class ArmsLoreWand : BaseWand
    {
        public override TimeSpan GetUseDelay { get { return TimeSpan.Zero; } }

        [Constructable]
        public ArmsLoreWand()
            : base(WandEffect.ArmsLore, 25, 175)
        {
            Charges = 50;
            Identified = true;
            Name = "an arms lore magic wand";
        }

        public ArmsLoreWand(Serial serial)
            : base(serial)
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

        public override bool OnWandTarget(Mobile from, object o)
        {
            if (o is BaseWeapon)
            {

                BaseWeapon weap = (BaseWeapon)o;

                if (weap.MaxHitPoints != 0)
                {
                    int hp = (int)((weap.HitPoints / (double)weap.MaxHitPoints) * 10);

                    if (hp < 0)
                        hp = 0;
                    else if (hp > 9)
                        hp = 9;

                    from.SendLocalizedMessage(1038285 + hp);
                }

                int damage = (weap.MaxDamage + weap.MinDamage) / 2;
                int hand = (weap.Layer == Layer.OneHanded ? 0 : 1);

                if (damage < 3)
                    damage = 0;
                else
                    damage = (int)Math.Ceiling(Math.Min(damage, 30) / 5.0);

                WeaponType type = weap.Type;

                if (type == WeaponType.Ranged)
                    from.SendLocalizedMessage(1038224 + (damage * 9));
                else if (type == WeaponType.Piercing)
                    from.SendLocalizedMessage(1038218 + hand + (damage * 9));
                else if (type == WeaponType.Slashing)
                    from.SendLocalizedMessage(1038220 + hand + (damage * 9));
                else if (type == WeaponType.Bashing)
                    from.SendLocalizedMessage(1038222 + hand + (damage * 9));
                else
                    from.SendLocalizedMessage(1038216 + hand + (damage * 9));

                if (weap.Poison != null && weap.PoisonCharges > 0)
                    from.SendLocalizedMessage(1038284); // It appears to have poison smeared on it.

            }
            else if (o is BaseArmor)
            {
                BaseArmor arm = (BaseArmor)o;

                if (arm.MaxHitPoints != 0)
                {
                    int hp = (int)((arm.HitPoints / (double)arm.MaxHitPoints) * 10);

                    if (hp < 0)
                        hp = 0;
                    else if (hp > 9)
                        hp = 9;

                    from.SendLocalizedMessage(1038285 + hp);
                }

                //For Slayer armor, also show the value of the armor HP
                if (o is BaseDungeonArmor)
                {
                    from.SendMessage("You notice an inscription on the armor suddenly glowing brighter.");
                    string text = String.Format("* {0}/{1} *", arm.HitPoints, arm.MaxHitPoints);
                    from.PublicOverheadMessage(MessageType.Emote, 0x0, false, text);
                }

                from.SendLocalizedMessage(1038295 + (int)Math.Ceiling(Math.Min(arm.ArmorRating, 35) / 5.0));

            }
            else if (o is SwampDragon && ((SwampDragon)o).HasBarding)
            {
                SwampDragon pet = (SwampDragon)o;

                if (from.CheckTargetSkill(SkillName.ArmsLore, o, 0, 100))
                {
                    int perc = (4 * pet.BardingHP) / pet.BardingMaxHP;

                    if (perc < 0)
                        perc = 0;
                    else if (perc > 4)
                        perc = 4;

                    pet.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1053021 - perc, from.NetState);
                }
                else
                {
                    from.SendLocalizedMessage(500353); // You are not certain...
                }
            }
            else
            {
                from.SendLocalizedMessage(500352); // This is neither weapon nor armor.
            }

            return (o is Item);
        }
    }
}