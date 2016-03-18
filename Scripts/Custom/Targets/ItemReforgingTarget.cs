using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Targets
{
    public class ItemReforgingTarget : Target
    {
        //Move this to feature list in the future
        public static readonly double SuccessChanceModifier = 1;
        public static readonly double ArcaneDustChanceModifier = 1.5;

        private Item m_Item;

        public ItemReforgingTarget(Item item)
            : base(2, false, TargetFlags.None)
        {
            m_Item = item;
        }

        protected override void OnTargetOutOfRange(Mobile from, object targeted)
        {
            base.OnTargetOutOfRange(from, targeted);
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            //Make sure tool still exists
            if (m_Item.Deleted)
                return;

       

            var arcaneHammer = m_Item as ArcaneHammer;
            if (arcaneHammer == null)
            {
                from.SendMessage("You are required to use an arcane hammer for this!");
                return;
            }

            var targetItem = targeted as Item;

            if (targetItem != null && targetItem.IsLockedDown)
            {
                from.SendMessage("You cannot reforge a locked down item!");
                return;
            }

            if ((targeted is Item) && (targetItem.LootType == LootType.Blessed || targetItem.LootType == LootType.Newbied || targetItem.Acquisition == Item.AcquisitionType.Crafted || targetItem.Acquisition == Item.AcquisitionType.VendorBought))
            {
                from.SendMessage("The item cannot be reforged!");
                return;
            }

            if (targeted is BaseWand)
            {
                from.SendMessage("The wand emits a magical barrier, which prevent you from reforging it!");
                return;
            }

            if (targeted is BaseDungeonArmor)
            {
                from.SendMessage("The dungeon armor piece cannot be reforged!");
                return;
            }

            //The success chance is no longer skill based
            const double successChance = 0.1;
            var isToolUsageConsumed = false;
            if (Utility.RandomDouble() > (successChance * SuccessChanceModifier) && (targeted is BaseArmor || targeted is BaseWeapon))
            {
                var item = targeted as Item;
                if (Utility.Random(0, 99) > item.BreakChanceFromReforge)
                {
                    // Fail to reforge, but the item does not break
                    from.SendMessage("You have failed to reforge the item.");
                    item.BreakChanceFromReforge += 5;
                    from.PlaySound(0x2A);
                }
                else
                {
                    //Check if arcanse dust can be add
                    double percentChance = 0;
                    if (targeted is BaseArmor)
                    {
                        var armor = targeted as BaseArmor;
                        percentChance = (double)((int)armor.ProtectionLevel + (int)armor.Durability) / 100;
                    }
                    else //if (targeted is BaseWeapon) <--- This one is always true
                    {
                        var weapon = targeted as BaseWeapon;
                        percentChance = (double)((int)weapon.DamageLevel + (int)weapon.DurabilityLevel + (int)weapon.AccuracyLevel) / 100;
                    }

                    if (Utility.RandomDouble() < (percentChance * ArcaneDustChanceModifier))
                    {
                        var arcaneDust = new ArcaneDust();
                        from.AddToBackpack(arcaneDust);
                        from.SendMessage("The item you tried to reforge disintegrated into {0}", arcaneDust.DefaultName);
                        from.FixedParticles(0x374A, 10, 15, 5028, EffectLayer.Waist);
                        from.PlaySound(0x2A);
                    }
                    else
                    {
                        from.SendMessage("The item you tried to reforge disintegrated into thin air.");
                        from.FixedParticles(0x374A, 10, 15, 5028, EffectLayer.Waist);
                        from.PlaySound(0x2A);
                    }
                    item.Delete();
                }
                isToolUsageConsumed = true;
            }
            //Check if target is an armor
            else if (targeted is BaseArmor)
            {
                var maxDurability = Enum.GetValues(typeof(ArmorDurabilityLevel)).Cast<ArmorDurabilityLevel>().Max();
                var maxProtectionLevel = Enum.GetValues(typeof(ArmorProtectionLevel)).Cast<ArmorProtectionLevel>().Max();
                var armor = targeted as BaseArmor;

                if ((int)armor.ProtectionLevel + (int)armor.Durability == 0)
                {
                    from.SendMessage("The item does not have any magical properites that can be reforged!");
                    return;
                }
                //Check if the armor is at highest magic level for all properties
                if (armor.Durability == maxDurability && armor.ProtectionLevel == maxProtectionLevel)
                {
                    from.SendMessage("You attempted to reforge the armor, but it does not seem to improve anymore.");
                }
                else
                {
                    var magicProperty = Utility.RandomMinMax(1, 2);
                    var success = false;
                    switch (magicProperty)
                    {
                        case 1:
                            if (armor.Durability < maxDurability)
                            {
                                armor.Durability += 1;
                                from.SendMessage("You have improved the armor durability via reforging!");
                                success = true;
                            }
                            break;
                        case 2:
                            if (armor.ProtectionLevel < maxProtectionLevel)
                            {
                                armor.ProtectionLevel += 1;
                                from.SendMessage("You have improved the armor protection via reforging!");
                                success = true;
                            }
                            break;
                    }
                    if (success)
                    {
                        from.BoltEffect(2707);
                        from.PlaySound(0x2A);
                    }
                    else
                    {
                        // Fail to reforge, but the item does not break
                        from.SendMessage("You have failed to reforge the item.");
                        from.PlaySound(0x2A);
                    }
                }
                isToolUsageConsumed = true;

            }
            else if (targeted is BaseWeapon)
            {
                var maxDamageLevel = Enum.GetValues(typeof(WeaponDamageLevel)).Cast<WeaponDamageLevel>().Max();
                var maxAccuracyLevel = Enum.GetValues(typeof(WeaponAccuracyLevel)).Cast<WeaponAccuracyLevel>().Max();
                var maxDurabilityLevel = Enum.GetValues(typeof(WeaponDurabilityLevel)).Cast<WeaponDurabilityLevel>().Max();
                var weapon = targeted as BaseWeapon;

                if ((int)weapon.DamageLevel + (int)weapon.DurabilityLevel + (int)weapon.AccuracyLevel == 0)
                {
                    from.SendMessage("The item does not have any magical properites that can be reforged!");
                    return;
                }

                if (weapon.DamageLevel == maxDamageLevel && weapon.AccuracyLevel == maxAccuracyLevel &&
                    weapon.DurabilityLevel == maxDurabilityLevel)
                {
                    from.SendMessage("You attempted to reforge the weapon, but it does not seem to improve anymore.");
                    from.BoltEffect(2707);
                    from.PlaySound(0x2A);
                }
                else
                {
                    var magicProperty = Utility.RandomMinMax(1, 2);
                    var success = false;
                    switch (magicProperty)
                    {
                        case 1:
                            if (weapon.DamageLevel < maxDamageLevel)
                            {
                                weapon.DamageLevel += 1;
                                from.SendMessage("You have improved the weapon damage via reforging!");
                                success = true;
                            }
                            break;
                        case 2:
                            if (weapon.AccuracyLevel < maxAccuracyLevel)
                            {
                                weapon.AccuracyLevel += 1;
                                from.SendMessage("You have improved the weapon accuracy via reforging!");
                                success = true;
                            }
                            break;
                    }
                    if (success)
                    {

                        from.BoltEffect(2707);
                        from.PlaySound(0x2A);
                    }
                    else
                    {
                        // Fail to reforge, but the item does not break
                        from.SendMessage("You have failed to reforge the item.");
                        from.PlaySound(0x2A);
                    }
                }

                isToolUsageConsumed = true;

            }
            else
            {
                from.SendMessage("You cannot do that!");
            }

            if (isToolUsageConsumed)
            {
                //Remove the durability for each successful use
                var hammer = (IUsesRemaining)m_Item;
                hammer.UsesRemaining--;

                //Destroy the tool if the durability is less than 1
                if (hammer.UsesRemaining < 1)
                {
                    m_Item.Delete();
                    from.SendMessage("Your tool falls apart after you have reforged the item.");
                    from.PlaySound(0x13E);
                }
            }
        }
    }
}
