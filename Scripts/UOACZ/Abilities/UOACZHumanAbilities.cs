using System;
using Server;
using Server.Regions;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;
using Server.Multis;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Accounting;
using Server.Spells;
using Server.Custom;


namespace Server
{
    public enum UOACZHumanAbilityType
    {
        Escape,
        Toolkit,
        Inspiration,
        Scientist,
        Technician,
        Provider,
        SecretStash,
        EmergencyRepairs,
        Searcher,
        Camel,
        Longshot,
        MarkTarget,
        HeartpierceArrow,
        Snare,
        Hamstring,
        ShieldWall,
        Phalanx,
        Spellbreaking,
        Hardy,
        Throw,
        Shadowstrike,
        Expertise,
        Knockback,
        RapidTreatment,
        IronFists,
        SuperiorHealing,
        FirstAid,
        Evasion,
        Flee,
        Cleave,
        Overpower
    }

    public static class UOACZHumanAbilities
    {
        #region Escape Ability

        public static void EscapeAbility(PlayerMobile player)
        {
            if (player.m_UOACZAccountEntry.HumanProfile.NextHideAllowed > DateTime.UtcNow)
            {
                player.SendMessage("You must wait another " + Utility.CreateTimeRemainingString(DateTime.UtcNow, player.m_UOACZAccountEntry.HumanProfile.NextHideAllowed, false, true, true, true, true) + " before you can use another hiding ability.");
                return;
            }

            player.PlaySound(0x657);

            int projectiles = 6;
            int particleSpeed = 4;

            for (int a = 0; a < projectiles; a++)
            {
                Point3D newLocation = new Point3D(player.X + Utility.RandomList(-5, -4, -3, -2, -1, 1, 2, 3, 4, 5), player.Y + Utility.RandomList(-5, -4, -3, -2, -1, 1, 2, 3, 4, 5), player.Z);
                SpellHelper.AdjustField(ref newLocation, player.Map, 12, false);

                IEntity effectStartLocation = new Entity(Serial.Zero, new Point3D(player.X, player.Y, player.Z + 5), player.Map);
                IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + 5), player.Map);

                Effects.SendMovingEffect(effectStartLocation, effectEndLocation, Utility.RandomList(0x3728), particleSpeed, 0, false, false, 0, 0);
            }

            AbilitySuccessful(player, UOACZHumanAbilityType.Escape);

            player.Hidden = true;
            player.StealthAttackReady = true;
            player.AllowedStealthSteps = 30;

            player.Warmode = false;
            player.Combatant = null;
            player.NextCombatTime = player.NextCombatTime + TimeSpan.FromSeconds(1);

            player.m_UOACZAccountEntry.HumanProfile.NextHideAllowed = DateTime.UtcNow + UOACZSystem.HideCooldown;

            IPooledEnumerable m_MobilesNearby = player.Map.GetMobilesInRange(player.Location, 30);

            foreach (Mobile mobile in m_MobilesNearby)
            {
                if (mobile.Combatant == player)
                    mobile.Combatant = null;

                if (mobile is BaseCreature)
                {
                    mobile.RemoveAggressor(player);
                    mobile.RemoveAggressed(player);
                }
            }

            m_MobilesNearby.Free();

            player.SendMessage("You make a hasty escape.");

            Timer.DelayCall(TimeSpan.FromSeconds(30), delegate
            {
                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!player.IsUOACZHuman) return;

                if (player.Hidden)
                {
                    player.RevealingAction();
                    player.SendMessage("You can no longer hide in the shadows.");
                }
            });
        }

        #endregion

        #region Phalanx Ability

        public static void PhalanxAbility(PlayerMobile player)
        {
            SpecialAbilities.PhalanxSpecialAbility(1.0, null, player, 1.0, 60, -1, true, "", "", "-1");

            player.RevealingAction();

            player.FixedParticles(0x376A, 9, 64, 5008, 2526, 0, EffectLayer.Waist);
            player.PlaySound(0x64E);

            AbilitySuccessful(player, UOACZHumanAbilityType.Phalanx);

            player.SendMessage("You stand ready to defend your fortified position.");

            Timer.DelayCall(TimeSpan.FromSeconds(60), delegate
            {
                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!player.IsUOACZHuman) return;

                player.SendMessage("Phalanx ability effect has expired.");
            });
        }

        #endregion

        #region Toolkit Ability

        public static void ToolkitAbility(PlayerMobile player)
        {
            player.SendMessage("Target a tool to restore to full charges.");
            player.Target = new ToolkitAbilityTarget();
        }

        public class ToolkitAbilityTarget : Target
        {
            public ToolkitAbilityTarget()
                : base(25, false, TargetFlags.None, false)
            {
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile player = from as PlayerMobile;

                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!CanUseAbility(player, UOACZHumanAbilityType.Toolkit, true)) return;

                Item item = null;

                bool validItem = false;

                if (target is Item)
                {
                    if (target is BaseTool || target is UOACZRepairHammer || target is UOACZLockpickKit ||
                        target is UOACZFishingPole)
                    {
                        item = target as Item;
                        validItem = true;
                    }
                }

                if (!validItem)
                {
                    from.SendMessage("That is not a valid item to repair.");
                    return;
                }

                if (!item.IsChildOf(from.Backpack))
                {
                    from.SendMessage("That item must be in your pack in order target it.");
                    return;
                }

                if (target is BaseTool)
                {
                    BaseTool tool = target as BaseTool;

                    if (tool.UsesRemaining == tool.MaxUses)
                    {
                        player.SendMessage("That tool is already at its maximum number of uses remaining.");
                        return;
                    }

                    tool.UsesRemaining = tool.MaxUses;
                }

                else
                {
                    if (target is UOACZRepairHammer)
                    {
                        UOACZRepairHammer tool = target as UOACZRepairHammer;

                        if (tool.Charges == UOACZRepairHammer.MaxCharges)
                        {
                            player.SendMessage("That tool is already at its maximum number of uses remaining.");
                            return;
                        }

                        tool.Charges = UOACZRepairHammer.MaxCharges;
                    }

                    if (target is UOACZLockpickKit)
                    {
                        UOACZLockpickKit tool = target as UOACZLockpickKit;

                        if (tool.Charges == UOACZLockpickKit.MaxCharges)
                        {
                            player.SendMessage("That tool is already at its maximum number of uses remaining.");
                            return;
                        }

                        tool.Charges = UOACZLockpickKit.MaxCharges;
                    }

                    if (target is UOACZFishingPole)
                    {
                        UOACZFishingPole tool = target as UOACZFishingPole;

                        if (tool.Charges == UOACZFishingPole.MaxCharges)
                        {
                            player.SendMessage("That tool is already at its maximum number of uses remaining.");
                            return;
                        }

                        tool.Charges = UOACZFishingPole.MaxCharges;
                    }

                    if (target is UOACZPickaxe)
                    {
                        UOACZPickaxe tool = target as UOACZPickaxe;

                        if (tool.UsesRemaining == UOACZPickaxe.MaxUses)
                        {
                            player.SendMessage("That tool is already at its maximum number of uses remaining.");
                            return;
                        }

                        tool.UsesRemaining = UOACZPickaxe.MaxUses;
                    }
                }

                player.SendMessage("You restore the tool to its maximum number of uses remaining.");

                player.RevealingAction();

                player.SendSound(0x64A);
                Effects.SendLocationParticles(EffectItem.Create(new Point3D(player.X, player.Y, player.Z + 5), player.Map, EffectItem.DefaultDuration), 0x377A, 10, 10, 9502);

                AbilitySuccessful(player, UOACZHumanAbilityType.Toolkit);
            }
        }

        #endregion

        #region Inspiration Ability

        public static void InspirationAbility(PlayerMobile player)
        {
            SpecialAbilities.InspirationSpecialAbility(1.0, null, player, .1, 300, -1, true, "", "", "-1");

            player.RevealingAction();

            AbilitySuccessful(player, UOACZHumanAbilityType.Inspiration);

            player.SendMessage("You feel inspired to craft exceptional items.");

            Timer.DelayCall(TimeSpan.FromSeconds(300), delegate
            {
                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!player.IsUOACZHuman) return;

                player.SendMessage("Inspiration ability effect has expired.");
            });
        }

        #endregion

        #region Scientist Ability

        public static void ScientistAbility(PlayerMobile player)
        {
            SpecialAbilities.ScientistSpecialAbility(1.0, null, player, .1, 300, -1, true, "", "", "-1");

            player.RevealingAction();

            player.FixedParticles(0x376A, 9, 64, 5008, 2526, 0, EffectLayer.Waist);
            player.PlaySound(0x64E);

            AbilitySuccessful(player, UOACZHumanAbilityType.Scientist);

            player.SendMessage("You feel motivated to create a multitude of alchemical items.");

            Timer.DelayCall(TimeSpan.FromSeconds(300), delegate
            {
                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!player.IsUOACZHuman) return;

                player.SendMessage("Scientist ability effect has expired.");
            });
        }

        #endregion

        #region Technician Ability

        public static void TechnicianAbility(PlayerMobile player)
        {
            SpecialAbilities.TechnicianSpecialAbility(1.0, null, player, .1, 300, -1, true, "", "", "-1");

            player.RevealingAction();

            player.FixedParticles(0x376A, 9, 64, 5008, 2526, 0, EffectLayer.Waist);
            player.PlaySound(0x64E);

            AbilitySuccessful(player, UOACZHumanAbilityType.Technician);

            player.SendMessage("You feel motivated to create a multitude of tinkering items.");

            Timer.DelayCall(TimeSpan.FromSeconds(300), delegate
            {
                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!player.IsUOACZHuman) return;

                player.SendMessage("Technician ability effect has expired.");
            });
        }

        #endregion

        #region Provider Ability

        public static void ProviderAbility(PlayerMobile player)
        {
            SpecialAbilities.ProviderSpecialAbility(1.0, null, player, .1, 300, -1, true, "", "", "-1");

            player.RevealingAction();

            player.FixedParticles(0x376A, 9, 64, 5008, 2526, 0, EffectLayer.Waist);
            player.PlaySound(0x64E);

            AbilitySuccessful(player, UOACZHumanAbilityType.Provider);

            player.SendMessage("You feel motivated to create a multitude of culinary items.");

            Timer.DelayCall(TimeSpan.FromSeconds(300), delegate
            {
                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!player.IsUOACZHuman) return;

                player.SendMessage("Provider ability effect has expired.");
            });
        }

        #endregion

        #region Secret Stash Ability

        public static void SecretStashAbility(PlayerMobile player)
        {
            player.m_UOACZAccountEntry.HumanProfile.Stockpile.MaxItems += 5;

            player.RevealingAction();

            player.FixedParticles(0x373A, 10, 15, 5036, 2615, 0, EffectLayer.Head);
            player.PlaySound(0x1E2);

            UOACZSystem.RefreshAllGumps(player);

            player.SendMessage("You uncover secret areas to store additional items in the town stockpile.");

            AbilitySuccessful(player, UOACZHumanAbilityType.SecretStash);
        }

        #endregion

        #region Emergency Repairs Ability

        public static void EmergencyRepairsAbility(PlayerMobile player)
        {
            SpecialAbilities.EmergencyRepairsSpecialAbility(1.0, null, player, 1.0, 300, -1, true, "", "", "-1");

            player.RevealingAction();

            player.FixedParticles(0x376A, 9, 64, 5008, 2615, 0, EffectLayer.Waist);
            player.PlaySound(0x64E);

            AbilitySuccessful(player, UOACZHumanAbilityType.EmergencyRepairs);

            player.SendMessage("You become of flurry of reconstruction.");

            Timer.DelayCall(TimeSpan.FromSeconds(300), delegate
            {
                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!player.IsUOACZHuman) return;

                player.SendMessage("Emergency Repairs ability effect has expired.");
            });
        }

        #endregion

        #region Searcher Ability

        public static void SearcherAbility(PlayerMobile player)
        {
            SpecialAbilities.SearcherSpecialAbility(1.0, null, player, .1, 300, -1, true, "", "", "-1");

            player.RevealingAction();

            player.FixedParticles(0x376A, 9, 64, 5008, 2526, 0, EffectLayer.Waist);
            player.PlaySound(0x64E);

            AbilitySuccessful(player, UOACZHumanAbilityType.Searcher);

            player.SendMessage("You prepare to give extra effort to your scavenging attempts.");

            Timer.DelayCall(TimeSpan.FromSeconds(300), delegate
            {
                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!player.IsUOACZHuman) return;

                player.SendMessage("Searcher ability effect has expired.");
            });
        }

        #endregion

        #region Camel Ability

        public static void CamelAbility(PlayerMobile player)
        {
            player.m_UOACZAccountEntry.HumanProfile.MaxHungerPoints += 5;
            player.m_UOACZAccountEntry.HumanProfile.MaxThirstPoints += 5;

            player.RevealingAction();

            player.FixedParticles(0x373A, 10, 15, 5036, 2615, 0, EffectLayer.Head);
            player.PlaySound(0x1E2);

            UOACZSystem.RefreshAllGumps(player);

            player.SendMessage("You feel much more capable of withstanding hunger and thirst.");

            AbilitySuccessful(player, UOACZHumanAbilityType.Camel);
        }

        #endregion

        #region Longshot Ability

        public static void LongshotAbility(PlayerMobile player)
        {
            BaseWeapon weapon = player.Weapon as BaseWeapon;

            if (!(weapon is BaseRanged))
            {
                player.SendMessage("This ability requires a ranged weapon.");
                return;
            }

            player.SendMessage("Target a creature or player to hit with this ability.");
            player.Target = new LongShotAbilityTarget();
        }

        public class LongShotAbilityTarget : Target
        {
            public LongShotAbilityTarget(): base(25, false, TargetFlags.Harmful, false)
            {
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile player = from as PlayerMobile;

                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!CanUseAbility(player, UOACZHumanAbilityType.Longshot, true)) return;

                BaseWeapon weapon = player.Weapon as BaseWeapon;

                if (!(weapon is BaseRanged))
                {
                    player.SendMessage("This ability requires a ranged weapon.");
                    return;
                }

                Mobile mobileTarget = null;

                if (target == player)
                {
                    player.SendMessage("You cannot target yourself.");
                    return;
                }

                if (!(target is Mobile))
                {
                    player.SendMessage("That is not a valid target.");
                    return;
                }

                mobileTarget = target as Mobile;

                if (!UOACZSystem.IsUOACZValidMobile(mobileTarget))
                {
                    player.SendMessage("That is not a valid target.");
                    return;
                }

                double rangeScalar = 2.0;

                if (mobileTarget is PlayerMobile)
                    rangeScalar = 1.5;

                int range = (int)(Math.Round((double)weapon.MaxRange * rangeScalar));

                if (Utility.GetDistance(player.Location, mobileTarget.Location) > range)
                {
                    player.SendMessage("That target is too far away.");
                    return;
                }

                if (!player.Map.InLOS(player.Location, mobileTarget.Location))
                {
                    player.SendMessage("That is not within in your line of sight.");
                    return;
                }

                player.RevealingAction();

                SpecialAbilities.HinderSpecialAbility(1.0, null, player, 2, 1, true, 0, false, "", "");
                player.SendMessage("You take aim...");

                player.Direction = Utility.GetDirection(player.Location, mobileTarget.Location);

                Timer.DelayCall(TimeSpan.FromSeconds(1.5), delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (!player.IsUOACZHuman) return;

                    if (!UOACZSystem.IsUOACZValidMobile(mobileTarget)) return;
                    if (Utility.GetDistance(player.Location, mobileTarget.Location) > range + 10) return;

                    weapon = player.Weapon as BaseWeapon;

                    if (!(weapon is BaseRanged))
                    {
                        player.SendMessage("This ability requires a ranged weapon.");
                        return;
                    }

                    weapon.PlaySwingAnimation(player);
                    player.PlaySound(0x522);

                    bool attackHit = weapon.CheckHit(player, mobileTarget);

                    if (!attackHit)
                    {
                        if (mobileTarget is BaseCreature)
                            attackHit = true;
                    }

                    Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate
                    {
                        if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                        if (!player.IsUOACZHuman) return;

                        if (!UOACZSystem.IsUOACZValidMobile(mobileTarget)) return;
                        if (Utility.GetDistance(player.Location, mobileTarget.Location) > range + 15) return;

                        if (!(weapon is BaseRanged))
                        {
                            player.SendMessage("This ability requires a ranged weapon.");
                            return;
                        }

                        int itemID = 3906;

                        if (!(weapon is Bow))
                            itemID = 7166;

                        AbilitySuccessful(player, UOACZHumanAbilityType.Longshot);

                        player.SendMessage("And fire!");

                        player.MovingEffect(mobileTarget, itemID, 18, 1, false, false);

                        Effects.SendLocationParticles(EffectItem.Create(player.Location, player.Map, TimeSpan.FromSeconds(0.25)), 0x3779, 10, 15, 2209, 0, 5029, 0);

                        double damageScalar = 1.33;  

                        if (mobileTarget is BaseCreature)
                            damageScalar = 3.0;                      

                        damageScalar *= UOACZPersistance.HumanBalanceScalar;

                        player.DoHarmful(mobileTarget);
                        player.NextCombatTime = DateTime.UtcNow + weapon.GetDelay(player, false);

                        if (attackHit)
                            weapon.OnHit(player, mobileTarget, damageScalar);

                        else
                            player.PlaySound(0x238);
                    });
                });
            }
        }

        #endregion

        #region Mark Target Ability

        public static void MarkTargetAbility(PlayerMobile player)
        {
            player.SendMessage("Target a creature or player to mark with this ability.");
            player.Target = new MarkTargetAbilityTarget();
        }

        public class MarkTargetAbilityTarget : Target
        {
            public MarkTargetAbilityTarget(): base(25, false, TargetFlags.Harmful, false)
            {
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile player = from as PlayerMobile;

                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!CanUseAbility(player, UOACZHumanAbilityType.MarkTarget, true)) return;

                Mobile mobileTarget = null;

                if (target == player)
                {
                    player.SendMessage("You cannot target yourself.");
                    return;
                }

                if (!(target is Mobile))
                {
                    player.SendMessage("That is not a valid target.");
                    return;
                }

                mobileTarget = target as Mobile;

                if (!UOACZSystem.IsUOACZValidMobile(mobileTarget))
                {
                    player.SendMessage("That is not a valid target.");
                    return;
                }

                if (Utility.GetDistance(player.Location, mobileTarget.Location) > 25)
                {
                    player.SendMessage("That target is too far away.");
                    return;
                }

                if (!player.Map.InLOS(player.Location, mobileTarget.Location))
                {
                    player.SendMessage("That is not within in your line of sight.");
                    return;
                }

                player.RevealingAction();

                AbilitySuccessful(player, UOACZHumanAbilityType.MarkTarget);

                player.DoHarmful(mobileTarget);

                player.SendMessage("You mark your target and increase the accuracy of all attacks against them.");

                mobileTarget.FixedParticles(0x373A, 10, 15, 5036, 2116, 0, EffectLayer.Head);
                mobileTarget.PlaySound(0x1E1);

                double reductionAmount = -.10;

                if (mobileTarget is BaseCreature)
                {
                    reductionAmount = -.15;

                    BaseCreature bc_Target = mobileTarget as BaseCreature;

                    if (bc_Target.Controlled)
                        reductionAmount *= UOACZSystem.GetFatigueScalar(player);
                }

                PlayerMobile playerTarget = mobileTarget as PlayerMobile;

                if (playerTarget != null)
                {
                    if (playerTarget.IsUOACZHuman)
                        reductionAmount = -.05;

                    if (playerTarget.IsUOACZUndead)
                        reductionAmount = -.10;

                    reductionAmount *= UOACZSystem.GetFatigueScalar(player);
                }

                reductionAmount *= UOACZPersistance.HumanBalanceScalar;

                SpecialAbilities.EvasionSpecialAbility(1.0, null, mobileTarget, reductionAmount, 60, 0, false, "", "", "-1");

                Timer.DelayCall(TimeSpan.FromSeconds(60), delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (!player.IsUOACZHuman) return;

                    player.SendMessage("Mark Target ability effect has expired.");
                });
            }
        }

        #endregion

        #region Heartpierce Arrow Ability

        public static void HeartpierceArrowAbility(PlayerMobile player)
        {
            BaseWeapon weapon = player.Weapon as BaseWeapon;

            if (!(weapon is BaseRanged))
            {
                player.SendMessage("This ability requires a ranged weapon.");
                return;
            }

            player.SendMessage("Target a creature or player to hit with this ability.");
            player.Target = new HeartpierceArrowAbilityTarget();
        }

        public class HeartpierceArrowAbilityTarget : Target
        {
            public HeartpierceArrowAbilityTarget(): base(25, false, TargetFlags.Harmful, false)
            {
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile player = from as PlayerMobile;

                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!CanUseAbility(player, UOACZHumanAbilityType.HeartpierceArrow, true)) return;

                BaseWeapon weapon = player.Weapon as BaseWeapon;

                if (!(weapon is BaseRanged))
                {
                    player.SendMessage("This ability requires a ranged weapon.");
                    return;
                }

                int range = weapon.MaxRange;

                Mobile mobileTarget = null;

                if (target == player)
                {
                    player.SendMessage("You cannot target yourself.");
                    return;
                }

                if (!(target is Mobile))
                {
                    player.SendMessage("That is not a valid target.");
                    return;
                }

                mobileTarget = target as Mobile;

                if (!UOACZSystem.IsUOACZValidMobile(mobileTarget))
                {
                    player.SendMessage("That is not a valid target.");
                    return;
                }

                if (Utility.GetDistance(player.Location, mobileTarget.Location) > range)
                {
                    player.SendMessage("That target is too far away.");
                    return;
                }

                if (!player.Map.InLOS(player.Location, mobileTarget.Location))
                {
                    player.SendMessage("That is not within in your line of sight.");
                    return;
                }

                player.RevealingAction();

                SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1, 2, true, 0, false, "", "");

                player.SendMessage("You aim for it's heart...");

                player.Direction = Utility.GetDirection(player.Location, mobileTarget.Location);

                Timer.DelayCall(TimeSpan.FromSeconds(1.5), delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (!player.IsUOACZHuman) return;

                    if (!UOACZSystem.IsUOACZValidMobile(mobileTarget)) return;
                    if (Utility.GetDistance(player.Location, mobileTarget.Location) > range + 10) return;

                    weapon = player.Weapon as BaseWeapon;

                    if (!(weapon is BaseRanged))
                    {
                        player.SendMessage("This ability requires a ranged weapon.");
                        return;
                    }

                    weapon.PlaySwingAnimation(player);
                    player.PlaySound(0x522);

                    bool attackHit = weapon.CheckHit(player, mobileTarget);

                    if (!attackHit)
                    {
                        if (mobileTarget is BaseCreature)
                            attackHit = true;
                    }

                    Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate
                    {
                        if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                        if (!player.IsUOACZHuman) return;

                        if (!UOACZSystem.IsUOACZValidMobile(mobileTarget)) return;
                        if (Utility.GetDistance(player.Location, mobileTarget.Location) > range + 10) return;

                        weapon = player.Weapon as BaseWeapon;

                        if (!(weapon is BaseRanged))
                        {
                            player.SendMessage("This ability requires a ranged weapon.");
                            return;
                        }

                        AbilitySuccessful(player, UOACZHumanAbilityType.HeartpierceArrow);

                        player.SendMessage("And fire!");

                        int itemID = 3906;

                        if (!(weapon is Bow))
                            itemID = 7166;

                        player.MovingEffect(mobileTarget, itemID, 18, 1, false, false);

                        Effects.SendLocationParticles(EffectItem.Create(player.Location, player.Map, TimeSpan.FromSeconds(0.25)), 0x3779, 10, 15, 2209, 0, 5029, 0);

                        double bleedAmount = 120;

                        if (mobileTarget is BaseCreature)
                        {
                            bleedAmount = 120;

                            BaseCreature bc_Target = mobileTarget as BaseCreature;

                            if (bc_Target.Controlled)
                                bleedAmount *= UOACZSystem.GetFatigueScalar(player);
                        }

                        PlayerMobile playerTarget = mobileTarget as PlayerMobile;

                        if (playerTarget != null)
                        {
                            bleedAmount = 40;
                            bleedAmount *= UOACZSystem.GetFatigueScalar(player);
                        }

                        bleedAmount *= UOACZPersistance.HumanBalanceScalar;

                        if (attackHit)
                        {
                            new Blood().MoveToWorld(mobileTarget.Location, mobileTarget.Map);
                            SpecialAbilities.BleedSpecialAbility(1.0, player, mobileTarget, bleedAmount, 8, -1, true, "", "You have been hit with a heartpierce arrow and begin to bleed.");

                            weapon.OnHit(player, mobileTarget);
                        }

                        else
                            player.PlaySound(0x238);

                        player.DoHarmful(mobileTarget);
                        player.NextCombatTime = DateTime.UtcNow + weapon.GetDelay(player, false);
                    });
                });
            }
        }

        #endregion

        #region Hamstring Ability

        public static void HamstringAbility(PlayerMobile player)
        {
            BaseWeapon weapon = player.Weapon as BaseWeapon;

            if (!(weapon is BaseRanged))
            {
                player.SendMessage("This ability requires a ranged weapon.");
                return;
            }

            player.SendMessage("Target a creature or player to hit with this ability.");
            player.Target = new HamstringAbilityTarget();
        }

        public class HamstringAbilityTarget : Target
        {
            public HamstringAbilityTarget(): base(25, false, TargetFlags.Harmful, false)
            {
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile player = from as PlayerMobile;

                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!CanUseAbility(player, UOACZHumanAbilityType.Hamstring, true)) return;

                BaseWeapon weapon = player.Weapon as BaseWeapon;

                if (!(weapon is BaseRanged))
                {
                    player.SendMessage("This ability requires a ranged weapon.");
                    return;
                }

                int range = weapon.MaxRange;

                Mobile mobileTarget = null;

                if (target == player)
                {
                    player.SendMessage("You cannot target yourself.");
                    return;
                }

                if (!(target is Mobile))
                {
                    player.SendMessage("That is not a valid target.");
                    return;
                }

                mobileTarget = target as Mobile;

                if (!UOACZSystem.IsUOACZValidMobile(mobileTarget))
                {
                    player.SendMessage("That is not a valid target.");
                    return;
                }

                if (Utility.GetDistance(player.Location, mobileTarget.Location) > range)
                {
                    player.SendMessage("That target is too far away.");
                    return;
                }

                if (!player.Map.InLOS(player.Location, mobileTarget.Location))
                {
                    player.SendMessage("That is not within in your line of sight.");
                    return;
                }

                player.RevealingAction();

                SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1, 2, true, 0, false, "", "");

                player.SendMessage("You aim for it's legs...");

                player.Direction = Utility.GetDirection(player.Location, mobileTarget.Location);

                Timer.DelayCall(TimeSpan.FromSeconds(1.5), delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (!player.IsUOACZHuman) return;

                    if (!UOACZSystem.IsUOACZValidMobile(mobileTarget)) return;
                    if (Utility.GetDistance(player.Location, mobileTarget.Location) > range + 10) return;

                    weapon = player.Weapon as BaseWeapon;

                    if (!(weapon is BaseRanged))
                    {
                        player.SendMessage("This ability requires a ranged weapon.");
                        return;
                    }

                    weapon.PlaySwingAnimation(player);
                    player.PlaySound(0x522);

                    bool attackHit = weapon.CheckHit(player, mobileTarget);

                    if (!attackHit)
                    {
                        if (mobileTarget is BaseCreature)
                            attackHit = true;
                    }

                    Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate
                    {
                        if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                        if (!player.IsUOACZHuman) return;

                        if (!UOACZSystem.IsUOACZValidMobile(mobileTarget)) return;
                        if (Utility.GetDistance(player.Location, mobileTarget.Location) > range + 10) return;

                        weapon = player.Weapon as BaseWeapon;

                        if (!(weapon is BaseRanged))
                        {
                            player.SendMessage("This ability requires a ranged weapon.");
                            return;
                        }

                        AbilitySuccessful(player, UOACZHumanAbilityType.Hamstring);

                        player.SendMessage("And fire!");

                        int itemID = 3906;

                        if (!(weapon is Bow))
                            itemID = 7166;

                        player.MovingEffect(mobileTarget, itemID, 18, 1, false, false);

                        Effects.SendLocationParticles(EffectItem.Create(player.Location, player.Map, TimeSpan.FromSeconds(0.25)), 0x3779, 10, 15, 2209, 0, 5029, 0);

                        double damageScalar = 1.0;
                        double entangleDuration = 10;

                        if (mobileTarget is BaseCreature)
                        {
                            entangleDuration = Utility.RandomMinMax(8, 16);
                            damageScalar = 3;

                            BaseCreature bc_Target = mobileTarget as BaseCreature;

                            if (bc_Target.Controlled)
                                entangleDuration *= UOACZSystem.GetFatigueScalar(player);
                        }

                        PlayerMobile playerTarget = mobileTarget as PlayerMobile;

                        if (playerTarget != null)
                        {
                            if (playerTarget.IsUOACZUndead)
                            {
                                entangleDuration = Utility.RandomMinMax(4, 8);
                                damageScalar = 1.25;
                            }

                            if (playerTarget.IsUOACZHuman)
                            {
                                entangleDuration = Utility.RandomMinMax(2, 4);
                                damageScalar = 1.0;
                            }

                            entangleDuration *= UOACZSystem.GetFatigueScalar(player);
                        }

                        entangleDuration *= UOACZPersistance.HumanBalanceScalar;

                        player.DoHarmful(mobileTarget);
                        player.NextCombatTime = DateTime.UtcNow + weapon.GetDelay(player, false);

                        if (attackHit)
                        {
                            new Blood().MoveToWorld(mobileTarget.Location, mobileTarget.Map);
                            SpecialAbilities.EntangleSpecialAbility(1.0, player, mobileTarget, 1.0, entangleDuration, -1, true, "", "You have been hamstrung and can't move!");

                            weapon.OnHit(player, mobileTarget, damageScalar);
                        }

                        else
                            player.PlaySound(0x238);                        
                    });
                });
            }
        }

        #endregion

        #region Snare Ability

        public static void SnareAbility(PlayerMobile player)
        {
            if (player.Backpack == null)
                return;

            player.RevealingAction();

            player.FixedParticles(0x373A, 10, 15, 5036, 2615, 0, EffectLayer.Head);
            player.PlaySound(0x1E2);

            /*
            player.SendMessage("You fashion a hunting snare.");

            HuntingSnare huntingSnare = new HuntingSnare();

            if (!player.Backpack.TryDropItem(player, huntingSnare, false))
            {
                player.SendMessage("You do not have enough room in your backpack so the item is placed at your feet.");
                huntingSnare.MoveToWorld(player.Location, player.Map);
            }
            */

            AbilitySuccessful(player, UOACZHumanAbilityType.Snare);
        }

        #endregion

        #region Shield Wall Ability

        public static void ShieldWallAbility(PlayerMobile player)
        {
            player.FixedParticles(0x375A, 9, 40, 5027, 2402, 0, EffectLayer.Waist);
            player.PlaySound(0x1F7);

            double absorbAmount = 25;

            absorbAmount *= UOACZSystem.GetFatigueScalar(player);
            absorbAmount *= UOACZPersistance.HumanBalanceScalar;

            int finalAmount = (int)Math.Round(absorbAmount);

            if (finalAmount < 1)
                finalAmount = 1;

            player.MeleeDamageAbsorb = finalAmount;            

            player.RevealingAction();

            AbilitySuccessful(player, UOACZHumanAbilityType.ShieldWall);

            player.SendMessage("You ready your defenses.");
        }

        #endregion        

        #region Spellbreaking Ability

        public static void SpellbreakingAbility(PlayerMobile player)
        {
            player.FixedParticles(0x375A, 10, 30, 5037, 0, 0, EffectLayer.Waist);
            player.PlaySound(0x1E9);

            double absorbAmount = 6;

            absorbAmount *= UOACZSystem.GetFatigueScalar(player);
            absorbAmount *= UOACZPersistance.HumanBalanceScalar;

            int finalAmount = (int)Math.Round(absorbAmount);

            if (finalAmount < 1)
                finalAmount = 1;

            player.MagicDamageAbsorb = finalAmount;

            player.RevealingAction();

            AbilitySuccessful(player, UOACZHumanAbilityType.Spellbreaking);

            player.SendMessage("You create a magical barrier around yourself.");
        }

        #endregion

        #region Hardy Ability

        public static void HardyAbility(PlayerMobile player)
        {
            SpecialAbilities.HardySpecialAbility(1.0, null, player, .25, 60, -1, true, "", "", "-1");

            player.RevealingAction();

            player.FixedParticles(0x376A, 9, 64, 5008, 2526, 0, EffectLayer.Waist);
            player.PlaySound(0x64E);

            AbilitySuccessful(player, UOACZHumanAbilityType.Hardy);

            player.ResetRegenTimers();

            Timer.DelayCall(TimeSpan.FromSeconds(61), delegate
            {
                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!player.IsUOACZHuman) return;

                player.ResetRegenTimers();
            });

            player.SendMessage("You begin to recuperate and recover from your wounds faster.");

            Timer.DelayCall(TimeSpan.FromSeconds(60), delegate
            {
                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!player.IsUOACZHuman) return;

                player.SendMessage("Hardy ability effect has expired.");
            });
        }

        #endregion

        #region Throw Ability

        public static void ThrowAbility(PlayerMobile player)
        {
            player.SendMessage("Target a player or creature to attack.");
            player.Target = new ThrowAbilityTarget();
        }

        public class ThrowAbilityTarget : Target
        {
            public ThrowAbilityTarget(): base(25, false, TargetFlags.Harmful, false)
            {
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile player = from as PlayerMobile;

                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!CanUseAbility(player, UOACZHumanAbilityType.Throw, true)) return;

                if (target == player)
                {
                    player.SendMessage("You cannot target yourself.");
                    return;
                }

                if (!(target is Mobile))
                {
                    player.SendMessage("That is not a valid target.");
                    return;
                }

                Mobile mobileTarget = target as Mobile;

                if (!UOACZSystem.IsUOACZValidMobile(mobileTarget))
                {
                    player.SendMessage("That is not a valid target.");
                    return;
                }

                if (Utility.GetDistance(player.Location, mobileTarget.Location) > 10)
                {
                    player.SendMessage("That location is too far away.");
                    return;
                }

                if (!player.Map.InLOS(player.Location, mobileTarget.Location))
                {
                    player.SendMessage("That is not within in your line of sight.");
                    return;
                }

                player.RevealingAction();

                SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1.0, 1, true, 0, false, "", "");

                int throwSound = 0x5D3;

                int itemID = 3921;
                int itemHue = 0;
                int hitSound = 0x23B;

                int animation = 9;
                int animationFrames = 7;

                switch (Utility.RandomMinMax(1, 4))
                {
                    //Dagger
                    case 1:
                        itemID = 3921;
                        itemHue = 0;
                        hitSound = 0x3BA;

                        animation = 9;
                        animationFrames = 7;
                        break;

                    //Hatchet
                    case 2:
                        itemID = 3907;
                        itemHue = 0;
                        hitSound = 0x237;

                        animation = 9;
                        animationFrames = 7;
                        break;

                    //Scimitar
                    case 3:
                        itemID = 5045;
                        itemHue = 0;
                        hitSound = 0x237;

                        animation = 9;
                        animationFrames = 7;
                        break;

                    //Two-Handed Axe
                    case 4:
                        itemID = 5114;
                        itemHue = 0;
                        hitSound = 0x237;

                        animation = 12;
                        animationFrames = 7;
                        break;
                }

                player.Animate(animation, animationFrames, 1, true, false, 0);

                Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (!player.IsUOACZHuman) return;

                    if (!UOACZSystem.IsUOACZValidMobile(mobileTarget)) return;
                    if (Utility.GetDistance(player.Location, mobileTarget.Location) >= 15) return;

                    AbilitySuccessful(player, UOACZHumanAbilityType.Throw);

                    Effects.PlaySound(player.Location, player.Map, throwSound);

                    IEntity startLocation = new Entity(Serial.Zero, new Point3D(player.Location.X, player.Location.Y, player.Location.Z + 5), player.Map);
                    IEntity endLocation = new Entity(Serial.Zero, new Point3D(mobileTarget.Location.X, mobileTarget.Location.Y, mobileTarget.Location.Z + 5), mobileTarget.Map);

                    Effects.SendMovingEffect(startLocation, endLocation, itemID, 15, 0, false, false, itemHue, 0);

                    double distance = player.GetDistanceToSqrt(endLocation.Location);
                    double destinationDelay = (double)distance * .04;

                    Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                    {
                        if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                        if (!player.IsUOACZHuman) return;

                        if (!UOACZSystem.IsUOACZValidMobile(mobileTarget)) return;
                        if (Utility.GetDistance(player.Location, mobileTarget.Location) >= 20) return;
                        if (mobileTarget.Hidden) return;

                        Effects.PlaySound(mobileTarget.Location, mobileTarget.Map, hitSound);

                        int minDamage = 15;
                        int maxDamage = 25;

                        double damageScalar = 3;

                        if (mobileTarget is BaseCreature)
                        {
                            damageScalar = 4;

                            BaseCreature bc_Target = mobileTarget as BaseCreature;

                            if (bc_Target.Controlled)
                                damageScalar *= UOACZSystem.GetFatigueScalar(player);
                        }

                        PlayerMobile playerTarget = mobileTarget as PlayerMobile;

                        if (playerTarget != null)
                        {
                            if (playerTarget.IsUOACZHuman)
                                damageScalar = .75;

                            if (playerTarget.IsUOACZUndead)
                                damageScalar = 1.0;

                            damageScalar *= UOACZSystem.GetFatigueScalar(player);
                        }

                        damageScalar *= UOACZPersistance.HumanBalanceScalar;

                        int damage = (int)(Math.Round(((double)Utility.RandomMinMax(minDamage, maxDamage)) * damageScalar));

                        UOACZSystem.PlayerInflictDamage(player, mobileTarget, true, damage);

                        player.DoHarmful(mobileTarget);

                        new Blood().MoveToWorld(mobileTarget.Location, mobileTarget.Map);
                        AOS.Damage(mobileTarget, player, damage, 100, 0, 0, 0, 0);
                    });
                });
            }
        }

        #endregion

        #region Shadowstrike Ability

        public static void ShadowstrikeAbility(PlayerMobile player)
        {
            if (player.m_UOACZAccountEntry.HumanProfile.NextHideAllowed > DateTime.UtcNow)
            {
                player.SendMessage("You must wait another " + Utility.CreateTimeRemainingString(DateTime.UtcNow, player.m_UOACZAccountEntry.HumanProfile.NextHideAllowed, false, true, true, true, true) + " before you can use another hiding ability.");
                return;
            }

            BaseWeapon weapon = player.Weapon as BaseWeapon;

            if (weapon is BaseRanged)
            {
                player.SendMessage("This ability cannot be used with a ranged weapon.");
                return;
            }

            player.SendMessage("Target a player or creature to attack.");
            player.Target = new ShadowstrikeAbilityTarget();
        }

        public class ShadowstrikeAbilityTarget : Target
        {
            public ShadowstrikeAbilityTarget(): base(25, false, TargetFlags.Harmful, false)
            {
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile player = from as PlayerMobile;

                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!CanUseAbility(player, UOACZHumanAbilityType.Shadowstrike, true)) return;

                BaseWeapon weapon = player.Weapon as BaseWeapon;

                if (weapon is BaseRanged)
                {
                    player.SendMessage("This ability cannot be used with a ranged weapon.");
                    return;
                }

                if (player.m_UOACZAccountEntry.HumanProfile.NextHideAllowed > DateTime.UtcNow)
                {
                    player.SendMessage("You must wait another " + Utility.CreateTimeRemainingString(DateTime.UtcNow, player.m_UOACZAccountEntry.HumanProfile.NextHideAllowed, false, true, true, true, true) + " before you can use another hiding ability.");
                    return;
                }

                if (target == player)
                {
                    player.SendMessage("You cannot target yourself.");
                    return;
                }

                if (!(target is Mobile))
                {
                    player.SendMessage("That is not a valid target.");
                    return;
                }

                Mobile mobileTarget = target as Mobile;

                if (Utility.GetDistance(player.Location, mobileTarget.Location) > 2)
                {
                    player.SendMessage("That target is out of attack range.");
                    return;
                }

                if (!player.Map.InLOS(player.Location, mobileTarget.Location))
                {
                    player.SendMessage("That is not within in your line of sight.");
                    return;
                }

                player.RevealingAction();

                SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1.0, 1, true, 0, false, "", "");

                player.Direction = Utility.GetDirection(player.Location, mobileTarget.Location);

                Timer.DelayCall(TimeSpan.FromSeconds(.25), delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (!player.IsUOACZHuman) return;

                    if (!UOACZSystem.IsUOACZValidMobile(mobileTarget)) return;
                    if (Utility.GetDistance(player.Location, mobileTarget.Location) > 15) return;

                    weapon = player.Weapon as BaseWeapon;

                    if (weapon is BaseRanged)
                    {
                        player.SendMessage("This ability cannot be used with a ranged weapon.");
                        return;
                    }

                    player.PlaySound(0x51D);

                    bool attackHit = weapon.CheckHit(player, mobileTarget);

                    if (!attackHit)
                    {
                        if (mobileTarget is BaseCreature)
                            attackHit = true;
                    }

                    player.DoHarmful(mobileTarget);

                    double damageScalar = 1.25;

                    if (mobileTarget is BaseCreature)
                        damageScalar = 2.0;

                    damageScalar *= UOACZPersistance.HumanBalanceScalar;

                    if (attackHit)
                        weapon.OnHit(player, mobileTarget, damageScalar);

                    else
                        weapon.OnMiss(player, mobileTarget);

                    AbilitySuccessful(player, UOACZHumanAbilityType.Shadowstrike);
                    
                    player.DoHarmful(mobileTarget);
                    player.NextCombatTime = DateTime.UtcNow + weapon.GetDelay(player, false);

                    int projectiles = 6;
                    int particleSpeed = 4;

                    for (int a = 0; a < projectiles; a++)
                    {
                        Point3D newLocation = new Point3D(player.X + Utility.RandomList(-5, -4, -3, -2, -1, 1, 2, 3, 4, 5), player.Y + Utility.RandomList(-5, -4, -3, -2, -1, 1, 2, 3, 4, 5), player.Z);
                        SpellHelper.AdjustField(ref newLocation, player.Map, 12, false);

                        IEntity effectStartLocation = new Entity(Serial.Zero, new Point3D(player.X, player.Y, player.Z + 5), player.Map);
                        IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + 5), player.Map);

                        Effects.SendMovingEffect(effectStartLocation, effectEndLocation, Utility.RandomList(0x3728), particleSpeed, 0, false, false, 0, 0);
                    }

                    Timer.DelayCall(TimeSpan.FromSeconds(.25), delegate
                    {
                        if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                        if (!player.IsUOACZHuman) return;

                        player.Hidden = true;
                        player.StealthAttackReady = true;
                        player.AllowedStealthSteps = 10;

                        player.Warmode = false;
                        player.Combatant = null;
                        player.NextCombatTime = player.NextCombatTime + TimeSpan.FromSeconds(1);

                        player.SendMessage("You attack your target and dart into the shadows.");

                        player.m_UOACZAccountEntry.HumanProfile.NextHideAllowed = DateTime.UtcNow + UOACZSystem.HideCooldown;

                        IPooledEnumerable m_MobilesNearby = player.Map.GetMobilesInRange(player.Location, 30);

                        foreach (Mobile mobile in m_MobilesNearby)
                        {
                            if (mobile.Combatant == player)
                                mobile.Combatant = null;

                            if (mobile is BaseCreature)
                            {
                                mobile.RemoveAggressor(player);
                                mobile.RemoveAggressed(player);
                            }
                        }

                        m_MobilesNearby.Free();

                        Timer.DelayCall(TimeSpan.FromSeconds(10), delegate
                        {
                            if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                            if (!player.IsUOACZHuman) return;

                            if (player.Hidden)
                            {
                                player.RevealingAction();
                                player.SendMessage("You can no longer hide in the shadows.");
                            }
                        });
                    });
                });
            }
        }

        #endregion

        #region Expertise Ability

        public static void ExpertiseAbility(PlayerMobile player)
        {
            double amount = .15;

            amount *= UOACZPersistance.HumanBalanceScalar;

            SpecialAbilities.ExpertiseSpecialAbility(1.0, player, null, amount, 60, -1, true, "", "", "-1");

            player.RevealingAction();

            AbilitySuccessful(player, UOACZHumanAbilityType.Expertise);

            player.SendMessage("You begin to demonstrate deadly expertise with your weapon.");

            Timer.DelayCall(TimeSpan.FromSeconds(60), delegate
            {
                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!player.IsUOACZHuman) return;

                player.SendMessage("Expertise ability effect has expired.");
            });
        }

        #endregion

        #region Knockback Ability

        public static void KnockbackAbility(PlayerMobile player)
        {
            player.SendMessage("Target a player or creature to attack.");
            player.Target = new KnockbackAbilityTarget();
        }

        public class KnockbackAbilityTarget : Target
        {
            public KnockbackAbilityTarget(): base(25, false, TargetFlags.Harmful, false)
            {
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile player = from as PlayerMobile;

                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!CanUseAbility(player, UOACZHumanAbilityType.Knockback, true)) return;

                if (target == player)
                {
                    player.SendMessage("You cannot target yourself.");
                    return;
                }

                if (!(target is Mobile))
                {
                    player.SendMessage("That is not a valid target.");
                    return;
                }

                Mobile mobileTarget = target as Mobile;

                if (!UOACZSystem.IsUOACZValidMobile(mobileTarget))
                {
                    player.SendMessage("That is not a valid target.");
                    return;
                }

                BaseWeapon weapon = player.Weapon as BaseWeapon;

                if (Utility.GetDistance(player.Location, mobileTarget.Location) > 2)
                {
                    player.SendMessage("That target is out of attack range.");
                    return;
                }

                if (!player.Map.InLOS(player.Location, mobileTarget.Location))
                {
                    player.SendMessage("That is not within in your line of sight.");
                    return;
                }

                player.RevealingAction();

                SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1.0, 1, true, 0, false, "", "");

                player.Direction = Utility.GetDirection(player.Location, mobileTarget.Location);

                Timer.DelayCall(TimeSpan.FromSeconds(.25), delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (!player.IsUOACZHuman) return;

                    if (!UOACZSystem.IsUOACZValidMobile(mobileTarget)) return;
                    if (Utility.GetDistance(player.Location, mobileTarget.Location) > 8) return;

                    weapon = player.Weapon as BaseWeapon;

                    player.PlaySound(0x50F);

                    bool attackHit = weapon.CheckHit(player, mobileTarget);

                    if (!attackHit)
                    {
                        if (mobileTarget is BaseCreature)
                            attackHit = true;
                    }

                    double damageScalar = 2.0;
                    int distance = 12;
                    int knockbackDamage = 80;

                    if (mobileTarget is BaseCreature)
                    {
                        damageScalar = 3.0;
                        distance = 12;
                        knockbackDamage = 80;

                        BaseCreature bc_Target = mobileTarget as BaseCreature;

                        if (bc_Target.Controlled)
                            knockbackDamage = (int)(Math.Round((double)knockbackDamage * UOACZSystem.GetFatigueScalar(player)));
                    }

                    if (mobileTarget is UOACZBaseUndead)
                    {
                        damageScalar = 3.0;
                        distance = 12;
                        knockbackDamage = 80;

                        BaseCreature bc_Target = mobileTarget as BaseCreature;

                        if (bc_Target.InitialDifficulty > BaseCreature.MediumDifficultyThreshold)
                            distance = 8;

                        if (bc_Target.InitialDifficulty > BaseCreature.HighDifficultyThreshold)
                            distance = 4;
                    }

                    PlayerMobile playerTarget = mobileTarget as PlayerMobile;

                    if (playerTarget != null)
                    {
                        if (playerTarget.IsUOACZUndead)
                        {
                            damageScalar = 1.5;
                            distance = 8;
                            knockbackDamage = 30;
                        }

                        if (playerTarget.IsUOACZHuman)
                        {
                            damageScalar = 1.25;
                            distance = 8;
                            knockbackDamage = 20;
                        }

                        knockbackDamage = (int)(Math.Round((double)knockbackDamage * UOACZSystem.GetFatigueScalar(player)));
                    }

                    damageScalar *= UOACZPersistance.HumanBalanceScalar;
                    knockbackDamage = (int)(Math.Round((double)knockbackDamage * UOACZPersistance.HumanBalanceScalar));

                    AbilitySuccessful(player, UOACZHumanAbilityType.Knockback);

                    player.DoHarmful(mobileTarget);

                    if (attackHit)
                    {
                        weapon.OnHit(player, mobileTarget, damageScalar);
                        SpecialAbilities.KnockbackSpecialAbility(1.0, player.Location, player, mobileTarget, knockbackDamage, distance, 0, "", "Their swing knocks you off your feet!");
                    }

                    else
                        weapon.OnMiss(player, mobileTarget);

                    player.DoHarmful(mobileTarget);
                    player.NextCombatTime = DateTime.UtcNow + weapon.GetDelay(player, false);
                });
            }
        }

        #endregion

        #region Rapid Treatment Ability

        public static void RapidTreatmentAbility(PlayerMobile player)
        {
            double amount = .33;

            amount *= UOACZPersistance.HumanBalanceScalar;

            SpecialAbilities.RapidTreatmentSpecialAbility(1.0, null, player, amount, 60, -1, true, "", "", "-1");

            player.RevealingAction();

            player.FixedParticles(0x376A, 9, 64, 5008, 2526, 0, EffectLayer.Waist);
            player.PlaySound(0x64E);

            AbilitySuccessful(player, UOACZHumanAbilityType.RapidTreatment);

            player.SendMessage("You prepare to administer bandages much more quickly.");

            Timer.DelayCall(TimeSpan.FromSeconds(60), delegate
            {
                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!player.IsUOACZHuman) return;

                player.SendMessage("Rapid Treatment ability effect has expired.");
            });
        }

        #endregion

        #region Iron Fists Ability

        public static void IronFistsAbility(PlayerMobile player)
        {
            double scalar = 2.0;

            scalar *= UOACZPersistance.HumanBalanceScalar;

            SpecialAbilities.IronFistsSpecialAbility(1.0, player, null, scalar, 60, -1, true, "", "", "-1");

            player.RevealingAction();

            AbilitySuccessful(player, UOACZHumanAbilityType.IronFists);

            player.SendMessage("Your fists become living weapons.");

            Timer.DelayCall(TimeSpan.FromSeconds(60), delegate
            {
                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!player.IsUOACZHuman) return;

                player.SendMessage("Iron Fists ability effect has expired.");
            });
        }

        #endregion

        #region Superior Healing Ability

        public static void SuperiorHealingAbility(PlayerMobile player)
        {
            double amount = 1.33;

            amount *= UOACZPersistance.HumanBalanceScalar;

            SpecialAbilities.SuperiorHealingSpecialAbility(1.0, null, player, amount, 60, -1, true, "", "", "-1");

            player.RevealingAction();

            player.FixedParticles(0x376A, 9, 64, 5008, 2526, 0, EffectLayer.Waist);
            player.PlaySound(0x64E);

            AbilitySuccessful(player, UOACZHumanAbilityType.SuperiorHealing);

            player.SendMessage("You concentrate and become more adept with your healing efforts.");

            Timer.DelayCall(TimeSpan.FromSeconds(60), delegate
            {
                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!player.IsUOACZHuman) return;

                player.SendMessage("Superior Healing ability effect has expired.");
            });
        }

        #endregion

        #region First Aid Ability

        public static void FirstAidAbility(PlayerMobile player)
        {
            player.SendMessage("Target a player or creature to administer first aid to.");
            player.Target = new FirstAidAbilityTarget();
        }

        public class FirstAidAbilityTarget : Target
        {
            public FirstAidAbilityTarget()
                : base(25, false, TargetFlags.Beneficial, false)
            {
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile player = from as PlayerMobile;

                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!CanUseAbility(player, UOACZHumanAbilityType.FirstAid, true)) return;

                Mobile mobileTarget;

                if (target is Mobile)
                    mobileTarget = target as Mobile;

                else
                {
                    player.SendMessage("That is not a valid target.");
                    return;
                }

                if (!UOACZSystem.IsUOACZValidMobile(mobileTarget))
                {
                    player.SendMessage("That is not a valid target.");
                    return;
                }

                PlayerMobile pm_Target = mobileTarget as PlayerMobile;
                BaseCreature bc_Target = mobileTarget as BaseCreature;

                bool validType = true;

                if (pm_Target != null)
                {
                    if (pm_Target.IsUOACZUndead)
                        validType = false;
                }

                if (bc_Target is UOACZBaseWildlife)
                    validType = false;

                if (!validType)
                {
                    player.SendMessage("That cannot be healed.");
                    return;
                }

                if (Utility.GetDistance(player.Location, mobileTarget.Location) > 1)
                {
                    player.SendMessage("That location is too far away.");
                    return;
                }

                if (!player.Map.InLOS(player.Location, mobileTarget.Location))
                {
                    player.SendMessage("That is not within in your line of sight.");
                    return;
                }

                if (mobileTarget.Hits == mobileTarget.HitsMax)
                {
                    player.SendMessage("That is not damaged.");
                    return;
                }

                player.Direction = Utility.GetDirection(player.Location, mobileTarget.Location);

                SpecialAbilities.HinderSpecialAbility(1.0, null, player, .75, 1, true, 0, false, "", "");

                player.RevealingAction();

                Timer.DelayCall(TimeSpan.FromSeconds(.25), delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (!player.IsUOACZHuman) return;

                    if (!UOACZSystem.IsUOACZValidMobile(mobileTarget)) return;
                    if (Utility.GetDistance(player.Location, mobileTarget.Location) > 3) return;

                    player.RevealingAction();

                    player.DoBeneficial(mobileTarget);

                    player.Animate(16, 7, 1, true, false, 0);

                    double healingAmount = 25;

                    if (mobileTarget is BaseCreature)
                        healingAmount *= 3;

                    healingAmount *= UOACZPersistance.HumanBalanceScalar;

                    int finalAmount = (int)(Math.Round(healingAmount));

                    mobileTarget.Heal(finalAmount);

                    player.SendMessage("You heal your target for " + finalAmount.ToString() + " hit points.");

                    mobileTarget.FixedParticles(0x376A, 9, 32, 5030, 0, 0, EffectLayer.Waist);
                    mobileTarget.PlaySound(0x202);

                    AbilitySuccessful(player, UOACZHumanAbilityType.FirstAid);
                });
            }
        }

        #endregion

        #region Evasion Ability

        public static void EvasionAbility(PlayerMobile player)
        {
            double amount = .20;

            amount *= UOACZPersistance.HumanBalanceScalar;

            SpecialAbilities.EvasionSpecialAbility(1.0, null, player, amount, 60, -1, true, "", "", "-1");

            player.RevealingAction();

            AbilitySuccessful(player, UOACZHumanAbilityType.Evasion);

            player.SendMessage("You begin to evade incoming attacks.");

            Timer.DelayCall(TimeSpan.FromSeconds(60), delegate
            {
                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!player.IsUOACZHuman) return;

                player.SendMessage("Evasion ability effect has expired.");
            });
        }

        #endregion

        #region Flee Ability

        public static void FleeAbility(PlayerMobile player)
        {
            player.SendMessage("Target the location you wish to flee to.");
            player.Target = new FleeAbilityTarget();
        }

        public class FleeAbilityTarget : Target
        {
            public FleeAbilityTarget(): base(25, true, TargetFlags.None, false)
            {
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile player = from as PlayerMobile;

                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!CanUseAbility(player, UOACZHumanAbilityType.Flee, true)) return;

                IPoint3D location = target as IPoint3D;

                if (location == null)
                    return;

                Map map = player.Map;

                if (map == null)
                    return;

                SpellHelper.GetSurfaceTop(ref location);

                IEntity targetLocation = new Entity(Serial.Zero, new Point3D(location), map);

                if (Utility.GetDistance(player.Location, targetLocation.Location) > 8)
                {
                    player.SendMessage("That location is too far away.");
                    return;
                }

                if (!player.Map.InLOS(player.Location, targetLocation.Location))
                {
                    player.SendMessage("That is not within in your line of sight.");
                    return;
                }

                player.PlaySound(0x657);

                int projectiles = 6;
                int particleSpeed = 4;

                Map oldMap = player.Map;
                Point3D oldLocation = player.Location;

                for (int a = 0; a < projectiles; a++)
                {
                    Point3D newLocation = new Point3D(oldLocation.X + Utility.RandomList(-5, -4, -3, -2, -1, 1, 2, 3, 4, 5), oldLocation.Y + Utility.RandomList(-5, -4, -3, -2, -1, 1, 2, 3, 4, 5), oldLocation.Z);
                    SpellHelper.AdjustField(ref newLocation, oldMap, 12, false);

                    IEntity effectStartLocation = new Entity(Serial.Zero, new Point3D(oldLocation.X, oldLocation.Y, oldLocation.Z + 5), oldMap);
                    IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + 5), oldMap);

                    Effects.SendMovingEffect(effectStartLocation, effectEndLocation, Utility.RandomList(0x3728), particleSpeed, 0, false, false, 0, 0);
                }

                AbilitySuccessful(player, UOACZHumanAbilityType.Flee);

                player.Location = targetLocation.Location;

                player.Hidden = true;
                player.StealthAttackReady = true;
                player.AllowedStealthSteps = 5;

                player.Warmode = false;
                player.Combatant = null;
                player.NextCombatTime = player.NextCombatTime + TimeSpan.FromSeconds(1);

                player.m_UOACZAccountEntry.HumanProfile.NextHideAllowed = DateTime.UtcNow + UOACZSystem.HideCooldown;

                IPooledEnumerable m_MobilesNearby = player.Map.GetMobilesInRange(player.Location, 30);

                foreach (Mobile mobile in m_MobilesNearby)
                {
                    if (mobile.Combatant == player)
                        mobile.Combatant = null;

                    if (mobile is BaseCreature)
                    {
                        mobile.RemoveAggressor(player);
                        mobile.RemoveAggressed(player);
                    }
                }

                m_MobilesNearby.Free();

                player.SendMessage("You flee.");

                Timer.DelayCall(TimeSpan.FromSeconds(5), delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (!player.IsUOACZHuman) return;

                    if (player.Hidden)
                    {
                        player.RevealingAction();
                        player.SendMessage("You can no longer hide in the shadows.");
                    }
                });
            }
        }

        #endregion

        #region Cleave Ability

        public static void CleaveAbility(PlayerMobile player)
        {
            BaseWeapon weapon = player.Weapon as BaseWeapon;

            if (player == null || weapon == null)
                return;

            if (weapon is BaseRanged)
            {
                player.SendMessage("This ability cannot be used with a ranged weapon.");
                return;
            }

            List<Mobile> m_MobilesToAttack = new List<Mobile>();

            int meleeRange = 2;

            IPooledEnumerable nearbyMobiles = player.Map.GetMobilesInRange(player.Location, meleeRange);
            {
                foreach (Mobile mobile in nearbyMobiles)
                {
                    if (!UOACZSystem.IsUOACZValidMobile(mobile)) continue;
                    if (mobile == player) continue;
                    if (!player.InRange(mobile, weapon.MaxRange) && !player.RangeExemption(mobile)) continue;

                    if (mobile is UOACZBaseUndead || mobile is UOACZBaseWildlife)
                    {
                        m_MobilesToAttack.Add(mobile);
                        continue;
                    }

                    PlayerMobile playerTarget = mobile as PlayerMobile;

                    if (playerTarget != null)
                    {
                        if (playerTarget.IsUOACZUndead)
                        {
                            m_MobilesToAttack.Add(playerTarget);
                            continue;
                        }
                    }

                    bool validTarget = false;

                    if (mobile.Combatant != null && mobile.Combatant == player)
                        validTarget = true;

                    if (player.Combatant != null && player.Combatant == mobile)
                        validTarget = true;

                    foreach (AggressorInfo aggressor in mobile.Aggressors)
                    {
                        if (aggressor.Attacker == player || aggressor.Defender == player)
                            validTarget = true;
                    }

                    foreach (AggressorInfo aggressed in mobile.Aggressed)
                    {
                        if (aggressed.Attacker == player || aggressed.Defender == player)
                            validTarget = true;
                    }

                    if (validTarget)
                    {
                        m_MobilesToAttack.Add(mobile);
                        continue;
                    }
                }
            }

            nearbyMobiles.Free();

            player.RevealingAction();

            SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1.0, 1, true, 0, false, "", "");

            Timer.DelayCall(TimeSpan.FromSeconds(.25), delegate
            {
                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!player.IsUOACZHuman) return;

                weapon = player.Weapon as BaseWeapon;

                if (weapon == null)
                    return;

                if (weapon is BaseRanged)
                {
                    player.SendMessage("This ability cannot be used with a ranged weapon.");
                    return;
                }

                AbilitySuccessful(player, UOACZHumanAbilityType.Cleave);

                player.PlaySound(0x51F);

                Queue m_Queue = new Queue();

                foreach (Mobile mobile in m_MobilesToAttack)
                {
                    m_Queue.Enqueue(mobile);
                }

                while (m_Queue.Count > 0)
                {
                    Mobile mobile = (Mobile)m_Queue.Dequeue();

                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (!UOACZSystem.IsUOACZValidMobile(mobile)) continue;

                    double damageScalar = 1.25;

                    bool attackHit = weapon.CheckHit(player, mobile);

                    if (mobile is BaseCreature)
                    {
                        if (!attackHit)
                            attackHit = true;

                        damageScalar = 2;
                    }

                    damageScalar *= UOACZPersistance.HumanBalanceScalar;

                    if (attackHit)
                        weapon.OnHit(player, mobile, damageScalar);

                    else
                        weapon.OnMiss(player, mobile);

                    player.DoHarmful(mobile);
                    player.NextCombatTime = DateTime.UtcNow + weapon.GetDelay(player, false);
                }
            });
        }

        #endregion

        #region Overpower Ability

        public static void OverpowerAbility(PlayerMobile player)
        {
            BaseWeapon weapon = player.Weapon as BaseWeapon;

            if (weapon is BaseRanged)
            {
                player.SendMessage("This ability cannot be used with a ranged weapon.");
                return;
            }

            player.SendMessage("Target a player or creature to attack.");
            player.Target = new OverpowerAbilityTarget();
        }

        public class OverpowerAbilityTarget : Target
        {
            public OverpowerAbilityTarget(): base(25, false, TargetFlags.Harmful, false)
            {
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile player = from as PlayerMobile;

                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!CanUseAbility(player, UOACZHumanAbilityType.Overpower, true)) return;

                BaseWeapon weapon = player.Weapon as BaseWeapon;

                if (weapon is BaseRanged)
                {
                    player.SendMessage("This ability cannot be used with a ranged weapon.");
                    return;
                }
                
                if (target == player)
                {
                    player.SendMessage("You cannot target yourself.");
                    return;
                }

                if (!(target is Mobile))
                {
                    player.SendMessage("That is not a valid target.");
                    return;
                }

                Mobile mobileTarget = target as Mobile;

                if (Utility.GetDistance(player.Location, mobileTarget.Location) > 2)
                {
                    player.SendMessage("That target is out of attack range.");
                    return;
                }

                if (!player.Map.InLOS(player.Location, mobileTarget.Location))
                {
                    player.SendMessage("That is not within in your line of sight.");
                    return;
                }

                player.RevealingAction();

                SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1.0, 1, true, 0, false, "", "");

                player.Direction = Utility.GetDirection(player.Location, mobileTarget.Location);

                Timer.DelayCall(TimeSpan.FromSeconds(.25), delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (!player.IsUOACZHuman) return;

                    if (!UOACZSystem.IsUOACZValidMobile(mobileTarget)) return;
                    if (Utility.GetDistance(player.Location, mobileTarget.Location) > 15) return;

                    weapon = player.Weapon as BaseWeapon;

                    if (weapon is BaseRanged)
                    {
                        player.SendMessage("This ability cannot be used with a ranged weapon.");
                        return;
                    }

                    player.PlaySound(0x58F); //0x525

                    bool attackHit = weapon.CheckHit(player, mobileTarget);

                    if (!attackHit)
                    {
                        if (mobileTarget is BaseCreature)
                            attackHit = true;
                    }

                    player.DoHarmful(mobileTarget);

                    double damageScalar = 1.66;

                    if (mobileTarget is BaseCreature)
                        damageScalar = 6.0;

                    damageScalar *= UOACZPersistance.HumanBalanceScalar;

                    if (attackHit)
                    {
                        int range = 2;

                        for (int a = 0; a < 20; a++)
                        {
                            TimedStatic floorCrack = new TimedStatic(Utility.RandomList(6913, 6914, 6915, 6916, 6917, 6918, 6919, 6920), 5);
                            floorCrack.Name = "floor crack";

                            Point3D newLocation = new Point3D(mobileTarget.Location.X + Utility.RandomMinMax(-1 * range, range), mobileTarget.Location.Y + Utility.RandomMinMax(-1 * range, range), mobileTarget.Location.Z);
                            SpellHelper.AdjustField(ref newLocation, mobileTarget.Map, 12, false);

                            floorCrack.MoveToWorld(newLocation, mobileTarget.Map);

                            if (Utility.RandomDouble() <= .2)
                                new Blood().MoveToWorld(newLocation, mobileTarget.Map);                            
                        }

                        mobileTarget.FixedEffect(0x5683, 10, 20);

                        
                        weapon.OnHit(player, mobileTarget, damageScalar);
                    }

                    else
                        weapon.OnMiss(player, mobileTarget);

                    AbilitySuccessful(player, UOACZHumanAbilityType.Overpower);
                    
                    player.DoHarmful(mobileTarget);
                    player.NextCombatTime = DateTime.UtcNow + weapon.GetDelay(player, false);
                });
            }
        }

        #endregion

        public static void AbilitySuccessful(PlayerMobile player, UOACZHumanAbilityType abilityType)
        {
            UOACZHumanAbilityEntry abilityEntry = UOACZHumanAbilities.GetAbilityEntry(player.m_UOACZAccountEntry, abilityType);
            abilityEntry.m_NextUsageAllowed = DateTime.UtcNow + TimeSpan.FromMinutes(abilityEntry.m_CooldownMinutes);
            player.Mana -= UOACZSystem.AbilityManaCost;
            UOACZSystem.RefreshAllGumps(player);

            player.m_UOACZAccountEntry.HumanProfile.NextAbilityAllowed = DateTime.UtcNow + UOACZSystem.MinimumDelayBetweenHumanAbilities;

            player.m_UOACZAccountEntry.HumanAbilitiesUsed++;
        }

        public static bool CanUseAbility(PlayerMobile player, UOACZHumanAbilityType abilityType, bool feedback)
        {
            if (player == null) return false;
            if (player.Deleted) return false;

            if (!player.IsUOACZHuman)
            {
                if (feedback)
                    player.SendMessage("That ability cannot be activated in your current state.");

                return false;
            }

            if (!(player.Region is UOACZRegion) || !UOACZPersistance.Active)
            {
                if (feedback)
                    player.SendMessage("That ability cannot be activated in your current state.");

                return false;
            }

            if (!player.Alive)
            {
                if (feedback)
                    player.SendMessage("You must be alive in order to use that ability.");

                return false;
            }

            if (DateTime.UtcNow < player.m_UOACZAccountEntry.HumanProfile.NextAbilityAllowed)
            {
                if (feedback)
                {
                    string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, player.m_UOACZAccountEntry.HumanProfile.NextAbilityAllowed, false, true, true, true, true);
                    player.SendMessage("You must wait another " + timeRemaining + " before using another ability.");
                }

                return false;
            }

            if (player.Frozen)
            {
                if (feedback)
                    player.SendMessage("You are frozen and cannot use that ability at the moment.");

                return false;
            }

            UOACZHumanAbilityEntry abilityEntry = GetAbilityEntry(player.m_UOACZAccountEntry, abilityType);

            if (abilityEntry == null)
                return false;

            UOACZHumanAbilityDetail abilityDetail = UOACZHumanAbilities.GetAbilityDetail(abilityType);

            if (DateTime.UtcNow < abilityEntry.m_NextUsageAllowed)
            {
                if (feedback)
                {
                    string cooldownText = Utility.CreateTimeRemainingString(DateTime.UtcNow, abilityEntry.m_NextUsageAllowed, false, false, true, true, true);
                    player.SendMessage("That ability is not usable for " + cooldownText + ".");
                }

                return false;
            }

            if (player.Mana < UOACZSystem.AbilityManaCost)
            {
                if (feedback)
                    player.SendMessage("You do not have enough mana to use that ability (requires 10 mana).");

                return false;
            }

            return true;
        }

        public static void ActivateAbility(PlayerMobile player, UOACZHumanAbilityType abilityType)
        {
            if (player == null) return;
            if (player.Deleted) return;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

            if (!CanUseAbility(player, abilityType, true))
                return;

            switch (abilityType)
            {
                case UOACZHumanAbilityType.Escape: EscapeAbility(player); break;
                case UOACZHumanAbilityType.Phalanx: PhalanxAbility(player); break;
                case UOACZHumanAbilityType.Toolkit: ToolkitAbility(player); break;
                case UOACZHumanAbilityType.Inspiration: InspirationAbility(player); break;
                case UOACZHumanAbilityType.Scientist: ScientistAbility(player); break;
                case UOACZHumanAbilityType.Technician: TechnicianAbility(player); break;
                case UOACZHumanAbilityType.Provider: ProviderAbility(player); break;
                case UOACZHumanAbilityType.SecretStash: SecretStashAbility(player); break;
                case UOACZHumanAbilityType.EmergencyRepairs: EmergencyRepairsAbility(player); break;
                case UOACZHumanAbilityType.Searcher: SearcherAbility(player); break;
                case UOACZHumanAbilityType.Camel: CamelAbility(player); break;
                case UOACZHumanAbilityType.Longshot: LongshotAbility(player); break;
                case UOACZHumanAbilityType.MarkTarget: MarkTargetAbility(player); break;
                case UOACZHumanAbilityType.HeartpierceArrow: HeartpierceArrowAbility(player); break;
                case UOACZHumanAbilityType.Hamstring: HamstringAbility(player); break;
                case UOACZHumanAbilityType.Snare: SnareAbility(player); break;
                case UOACZHumanAbilityType.ShieldWall: ShieldWallAbility(player); break;                
                case UOACZHumanAbilityType.Spellbreaking: SpellbreakingAbility(player); break;
                case UOACZHumanAbilityType.Hardy: HardyAbility(player); break;
                case UOACZHumanAbilityType.Throw: ThrowAbility(player); break;
                case UOACZHumanAbilityType.Shadowstrike: ShadowstrikeAbility(player); break;
                case UOACZHumanAbilityType.Expertise: ExpertiseAbility(player); break;
                case UOACZHumanAbilityType.Knockback: KnockbackAbility(player); break;
                case UOACZHumanAbilityType.RapidTreatment: RapidTreatmentAbility(player); break;
                case UOACZHumanAbilityType.IronFists: IronFistsAbility(player); break;
                case UOACZHumanAbilityType.SuperiorHealing: SuperiorHealingAbility(player); break;
                case UOACZHumanAbilityType.FirstAid: FirstAidAbility(player); break;
                case UOACZHumanAbilityType.Evasion: EvasionAbility(player); break;
                case UOACZHumanAbilityType.Flee: FleeAbility(player); break;
                case UOACZHumanAbilityType.Cleave: CleaveAbility(player); break;
                case UOACZHumanAbilityType.Overpower: OverpowerAbility(player); break;
            }
        }

        public static UOACZHumanAbilityDetail GetAbilityDetail(UOACZHumanAbilityType ability)
        {
            UOACZHumanAbilityDetail abilityDetail = new UOACZHumanAbilityDetail();

            switch (ability)
            {
                case UOACZHumanAbilityType.Escape:
                    abilityDetail.Name = "Escape";
                    abilityDetail.Description = new string[] { "Immediately hide for 30 seconds and gain 30 unfailable stealth steps" };
                    abilityDetail.CooldownMinutes = 30;
                    abilityDetail.CooldownMinutesDecreasePerTimesAcquired = .3;
                    abilityDetail.CooldownMinimumMinutes = 5;
                    break;

                case UOACZHumanAbilityType.Toolkit:
                    abilityDetail.Name = "Toolkit";
                    abilityDetail.Description = new string[] { "Restore a crafting or harvesting tool to full charges" };
                    abilityDetail.CooldownMinutes = 60;
                    abilityDetail.CooldownMinutesDecreasePerTimesAcquired = 6;
                    abilityDetail.CooldownMinimumMinutes = 10;
                    break;

                case UOACZHumanAbilityType.Inspiration:
                    abilityDetail.Name = "Inspiration";
                    abilityDetail.Description = new string[] { "Gain an additional 10% chance to craft exceptional items for 5 minutes" };
                    abilityDetail.CooldownMinutes = 120;
                    abilityDetail.CooldownMinutesDecreasePerTimesAcquired = 12;
                    abilityDetail.CooldownMinimumMinutes = 20;
                    break;

                case UOACZHumanAbilityType.Scientist:
                    abilityDetail.Name = "Scientist";
                    abilityDetail.Description = new string[] { "Gain an (Alchemy Skill / 10%) chance to craft a duplicate alchemy item for the next 5 minutes" };
                    abilityDetail.CooldownMinutes = 120;
                    abilityDetail.CooldownMinutesDecreasePerTimesAcquired = 12;
                    abilityDetail.CooldownMinimumMinutes = 20;
                    break;

                case UOACZHumanAbilityType.Technician:
                    abilityDetail.Name = "Technician";
                    abilityDetail.Description = new string[] { "Gain a (Tinkering Skill / 10%) chance to craft a duplicate tinkering item for the next 5 minutes" };
                    abilityDetail.CooldownMinutes = 120;
                    abilityDetail.CooldownMinutesDecreasePerTimesAcquired = 12;
                    abilityDetail.CooldownMinimumMinutes = 20;
                    break;

                case UOACZHumanAbilityType.Provider:
                    abilityDetail.Name = "Provider";
                    abilityDetail.Description = new string[] { "Gain a (Cooking Skill / 10%) chance to craft a duplicate cooking item for the next 5 minutes" };
                    abilityDetail.CooldownMinutes = 120;
                    abilityDetail.CooldownMinutesDecreasePerTimesAcquired = 12;
                    abilityDetail.CooldownMinimumMinutes = 20;
                    break;

                case UOACZHumanAbilityType.SecretStash:
                    abilityDetail.Name = "Secret Stash";
                    abilityDetail.Description = new string[] { "Increase your stockpile's item limit by 5" };
                    abilityDetail.CooldownMinutes = 120;
                    abilityDetail.CooldownMinutesDecreasePerTimesAcquired = 12;
                    abilityDetail.CooldownMinimumMinutes = 20;
                    break;

                case UOACZHumanAbilityType.EmergencyRepairs:
                    abilityDetail.Name = "Emergency Repairs";
                    abilityDetail.Description = new string[] { "Increase the durability restored when repairing walls and constructable objects by 100% for the next 60 seconds" };
                    abilityDetail.CooldownMinutes = 6;
                    abilityDetail.CooldownMinutesDecreasePerTimesAcquired = .5;
                    abilityDetail.CooldownMinimumMinutes = 1;
                    break;

                case UOACZHumanAbilityType.Searcher:
                    abilityDetail.Name = "Searcher";
                    abilityDetail.Description = new string[] { "For the next minute, scavenge actions have a 10% chance to allow an extra scavenge attempt to be made against the object" };
                    abilityDetail.CooldownMinutes = 6;
                    abilityDetail.CooldownMinutesDecreasePerTimesAcquired = .5;
                    abilityDetail.CooldownMinimumMinutes = 1;
                    break;

                case UOACZHumanAbilityType.Camel:
                    abilityDetail.Name = "Camel";
                    abilityDetail.Description = new string[] { "Increase your maximum Hunger and maximum Thirst values by 5" };
                    abilityDetail.CooldownMinutes = 240;
                    abilityDetail.CooldownMinutesDecreasePerTimesAcquired = 24;
                    abilityDetail.CooldownMinimumMinutes = 40;
                    break;

                case UOACZHumanAbilityType.Longshot:
                    abilityDetail.Name = "Longshot";
                    abilityDetail.Description = new string[] { "Make a ranged attack at a target at +100% range that inflicts a large amount of damage" };
                    abilityDetail.CooldownMinutes = 3;
                    abilityDetail.CooldownMinutesDecreasePerTimesAcquired = .25;
                    abilityDetail.CooldownMinimumMinutes = .5;
                    break;

                case UOACZHumanAbilityType.MarkTarget:
                    abilityDetail.Name = "Mark Target";
                    abilityDetail.Description = new string[] { "Increase the chance for the target to be hit by melee/ranged attacks by 15% for the next 60 seconds, with a reduced effect on players" };
                    abilityDetail.CooldownMinutes = 3;
                    abilityDetail.CooldownMinutesDecreasePerTimesAcquired = .25;
                    abilityDetail.CooldownMinimumMinutes = .5;
                    break;

                case UOACZHumanAbilityType.HeartpierceArrow:
                    abilityDetail.Name = "Heartpierce Arrow";
                    abilityDetail.Description = new string[] { "Make a ranged attack that inflicts a large bleed effect on the target, with a reduced effect on players" };
                    abilityDetail.CooldownMinutes = 3;
                    abilityDetail.CooldownMinutesDecreasePerTimesAcquired = .25;
                    abilityDetail.CooldownMinimumMinutes = .5;
                    break;

                case UOACZHumanAbilityType.Hamstring:
                    abilityDetail.Name = "Hamstring";
                    abilityDetail.Description = new string[] { "Make a ranged attack that entangles your target, with a reduced effect on players" };
                    abilityDetail.CooldownMinutes = 3;
                    abilityDetail.CooldownMinutesDecreasePerTimesAcquired = .25;
                    abilityDetail.CooldownMinimumMinutes = .5;
                break;

                case UOACZHumanAbilityType.Snare:
                    abilityDetail.Name = "Snare";
                    abilityDetail.Description = new string[] { "Creates a hunting snare in the player's pack" };
                    abilityDetail.CooldownMinutes = 15;
                    abilityDetail.CooldownMinutesDecreasePerTimesAcquired = 1.25;
                    abilityDetail.CooldownMinimumMinutes = 2.5;
                    break;

                case UOACZHumanAbilityType.ShieldWall:
                    abilityDetail.Name = "Shield Wall";
                    abilityDetail.Description = new string[] { "Applies Reactive Armor to the player" };
                    abilityDetail.CooldownMinutes = 3;
                    abilityDetail.CooldownMinutesDecreasePerTimesAcquired = .25;
                    abilityDetail.CooldownMinimumMinutes = .5;
                    break;

                case UOACZHumanAbilityType.Phalanx:
                    abilityDetail.Name = "Phalanx";
                    abilityDetail.Description = new string[] { "Allows the player to attack targets on the other side of medium-height defensive walls for the next 60 seconds" };
                    abilityDetail.CooldownMinutes = 3;
                    abilityDetail.CooldownMinutesDecreasePerTimesAcquired = .25;
                    abilityDetail.CooldownMinimumMinutes = .5;
                    break;

                case UOACZHumanAbilityType.Spellbreaking:
                    abilityDetail.Name = "Spellbreaking";
                    abilityDetail.Description = new string[] { "Applies Magic Reflect to the player with 6 levels of reflection" };
                    abilityDetail.CooldownMinutes = 3;
                    abilityDetail.CooldownMinutesDecreasePerTimesAcquired = .25;
                    abilityDetail.CooldownMinimumMinutes = .5;
                    break;

                case UOACZHumanAbilityType.Hardy:
                    abilityDetail.Name = "Hardy";
                    abilityDetail.Description = new string[] { "Increases the player's health regen rate by 300% for the next 60 seconds" };
                    abilityDetail.CooldownMinutes = 3;
                    abilityDetail.CooldownMinutesDecreasePerTimesAcquired = .25;
                    abilityDetail.CooldownMinimumMinutes = .5;
                    break;

                case UOACZHumanAbilityType.Throw:
                    abilityDetail.Name = "Throw";
                    abilityDetail.Description = new string[] { "Throw a hidden weapon at a target for a large amount of damage" };
                    abilityDetail.CooldownMinutes = 3;
                    abilityDetail.CooldownMinutesDecreasePerTimesAcquired = .25;
                    abilityDetail.CooldownMinimumMinutes = .5;
                    break;

                case UOACZHumanAbilityType.Shadowstrike:
                    abilityDetail.Name = "Shadowstrike";
                    abilityDetail.Description = new string[] { "Make a melee attack against target and then immediately hide for 10 seconds, receiving 10 unfailable stealth steps" };
                    abilityDetail.CooldownMinutes = 6;
                    abilityDetail.CooldownMinutesDecreasePerTimesAcquired = .5;
                    abilityDetail.CooldownMinimumMinutes = 1;
                    break;

                case UOACZHumanAbilityType.Expertise:
                    abilityDetail.Name = "Expertise";
                    abilityDetail.Description = new string[] { "Gain +15% chance to inflict weapon special attacks for the next 60 seconds" };
                    abilityDetail.CooldownMinutes = 3;
                    abilityDetail.CooldownMinutesDecreasePerTimesAcquired = .25;
                    abilityDetail.CooldownMinimumMinutes = .5;
                    break;

                case UOACZHumanAbilityType.Knockback:
                    abilityDetail.Name = "Knockback";
                    abilityDetail.Description = new string[] { "Make a melee attack against target, dealing increased damage and knocking them backwards" };
                    abilityDetail.CooldownMinutes = 3;
                    abilityDetail.CooldownMinutesDecreasePerTimesAcquired = .25;
                    abilityDetail.CooldownMinimumMinutes = .5;
                    break;

                case UOACZHumanAbilityType.RapidTreatment:
                    abilityDetail.Name = "Rapid Treatment";
                    abilityDetail.Description = new string[] { "Length of time needed to apply bandages reduced by 33% for the next 60 seconds" };
                    abilityDetail.CooldownMinutes = 3;
                    abilityDetail.CooldownMinutesDecreasePerTimesAcquired = .25;
                    abilityDetail.CooldownMinimumMinutes = .5;
                    break;

                case UOACZHumanAbilityType.IronFists:
                    abilityDetail.Name = "Iron Fists";
                    abilityDetail.Description = new string[] { "Damage dealt while unarmed is increased by 200% for the next 60 seconds" };
                    abilityDetail.CooldownMinutes = 3;
                    abilityDetail.CooldownMinutesDecreasePerTimesAcquired = .25;
                    abilityDetail.CooldownMinimumMinutes = .5;
                    break;

                case UOACZHumanAbilityType.SuperiorHealing:
                    abilityDetail.Name = "Superior Healing";
                    abilityDetail.Description = new string[] { "Health restored when applying bandages increased by 33% for the next 60 seconds" };
                    abilityDetail.CooldownMinutes = 3;
                    abilityDetail.CooldownMinutesDecreasePerTimesAcquired = .25;
                    abilityDetail.CooldownMinimumMinutes = .5;
                    break;

                case UOACZHumanAbilityType.FirstAid:
                    abilityDetail.Name = "First Aid";
                    abilityDetail.Description = new string[] { "Immediately restore a moderate amount of health to an adjacent target" };
                    abilityDetail.CooldownMinutes = 6;
                    abilityDetail.CooldownMinutesDecreasePerTimesAcquired = .5;
                    abilityDetail.CooldownMinimumMinutes = 1;
                    break;

                case UOACZHumanAbilityType.Evasion:
                    abilityDetail.Name = "Evasion";
                    abilityDetail.Description = new string[] { "Reduce the chance to be hit by melee and ranged attacks by 20% for the next 60 seconds" };
                    abilityDetail.CooldownMinutes = 3;
                    abilityDetail.CooldownMinutesDecreasePerTimesAcquired = .25;
                    abilityDetail.CooldownMinimumMinutes = .5;
                    break;

                case UOACZHumanAbilityType.Flee:
                    abilityDetail.Name = "Flee";
                    abilityDetail.Description = new string[] { "Move to the target location, immediately hide for the next 5 seconds, and gain 5 unfailable stealth steps" };
                    abilityDetail.CooldownMinutes = 6;
                    abilityDetail.CooldownMinutesDecreasePerTimesAcquired = .5;
                    abilityDetail.CooldownMinimumMinutes = 1;
                    break;

                case UOACZHumanAbilityType.Cleave:
                    abilityDetail.Name = "Cleave";
                    abilityDetail.Description = new string[] { "Make a melee attack against all adjacent targets" };
                    abilityDetail.CooldownMinutes = 3;
                    abilityDetail.CooldownMinutesDecreasePerTimesAcquired = .25;
                    abilityDetail.CooldownMinimumMinutes = .5;
                break;

                case UOACZHumanAbilityType.Overpower:
                    abilityDetail.Name = "Overpower";
                    abilityDetail.Description = new string[] { "Make a melee attack inflicting massive damage" };
                    abilityDetail.CooldownMinutes = 3;
                    abilityDetail.CooldownMinutesDecreasePerTimesAcquired = .25;
                    abilityDetail.CooldownMinimumMinutes = .5;
                break;
            }

            return abilityDetail;
        }

        public static UOACZHumanAbilityEntry GetAbilityEntry(UOACZAccountEntry playerEntry, UOACZHumanAbilityType abilityType)
        {
            UOACZHumanAbilityEntry abilityEntry = null;

            foreach (UOACZHumanAbilityEntry entry in playerEntry.HumanProfile.m_Abilities)
            {
                if (entry.m_AbilityType == abilityType)
                    return entry;
            }

            return abilityEntry;
        }
    }

    public class UOACZHumanAbilityDetail
    {
        public string Name = "Ability Detail";
        public string[] Description = new string[] { };

        public double CooldownMinutes = 3;
        public double CooldownMinutesDecreasePerTimesAcquired = .25;
        public double CooldownMinimumMinutes = .5;
    }

    public class UOACZHumanAbilityEntry
    {
        public UOACZHumanAbilityType m_AbilityType = UOACZHumanAbilityType.Throw;
        public int m_TimesAcquired = 0;
        public double m_CooldownMinutes = 3;
        public DateTime m_NextUsageAllowed = DateTime.UtcNow;

        public UOACZHumanAbilityEntry(UOACZHumanAbilityType abilityType, int timesAcquired, double cooldownMinutes, DateTime nextUsageAllowed)
        {
            m_AbilityType = abilityType;
            m_TimesAcquired = timesAcquired;
            m_CooldownMinutes = cooldownMinutes;
            m_NextUsageAllowed = nextUsageAllowed;
        }
    }
}