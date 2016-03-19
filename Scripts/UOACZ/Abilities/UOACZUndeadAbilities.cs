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
using Server.Achievements;

namespace Server
{
    public enum UOACZUndeadAbilityType
    {       
        Regeneration,
        Sacrifice,
        GiftOfCorruption,
        Creep,
        Dig,          
        Rally,

        RecruitZombie,
        LightningBolt,
        AuraOfDecay,
        BloodyMess,
        Ignite,
        Bile, 
        Combust,
        Virus,
        Plague,

        RecruitSkeleton,
        ShieldOfBones,        
        Sunder,
        Demolish,
        RestlessDead,

        CarrionSwarm,
        ConsumeCorpse,
        CorpseBreath,
        FeedingFrenzy,  
        LivingBomb,

        Shadows,
        Wail,
        GhostlyStrike,
        Dematerialize,       
        Malediction,

        Deathbolt,
        BatForm,
        Transfix,
        EnergyBolt,
        Embrace,
        Unlife,        
        ChainLightning,        

        VoidRift,
        Enervate,        
        Darkblast,        
        Engulf,

        WildHunt,
        Charge,       
        Firebreath,
        WingBuffet,
        BoneBreath
    }

    public static class UOACZUndeadAbilities
    {
        #region Regeneration Ability

        public static void RegenerationAbility(PlayerMobile player)
        {
            double directionDelay = .25;
            double initialDelay = UOACZSystem.AbilityInitialDelaySeconds;
            double totalDelay = directionDelay + initialDelay;

            SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1, totalDelay, true, 0, false, "", "");

            player.RevealingAction();

            Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
            {
                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!player.IsUOACZUndead) return;

                int animation = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimation;
                int animationFrames = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimationFrames;

                player.Animate(animation, animationFrames, 1, true, false, 0);
                player.PlaySound(player.GetAngerSound());

                Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (!player.IsUOACZUndead) return;

                    AbilitySuccessful(player, UOACZUndeadAbilityType.Regeneration);

                    double amount = .25;

                    if (player.HitsMax >= 300)
                        amount = .20;

                    if (player.HitsMax >= 400)
                        amount = .15;

                    SpecialAbilities.HardySpecialAbility(1.0, null, player, amount, 60, -1, true, "", "", "-1");

                    player.FixedParticles(0x376A, 9, 64, 5008, 2210, 0, EffectLayer.Waist);
                    player.PlaySound(0x64E);

                    player.ResetRegenTimers();

                    player.SendMessage("You wounds begin to heal and close on their own.");

                    Timer.DelayCall(TimeSpan.FromSeconds(60), delegate
                    {
                        if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                        if (!player.IsUOACZUndead) return;

                        player.SendMessage("Regeneration ability effect has expired.");
                    });

                    Timer.DelayCall(TimeSpan.FromSeconds(61), delegate
                    {
                        if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                        if (!player.IsUOACZUndead) return;

                        player.ResetRegenTimers();
                    });
                });
            });
        }

        #endregion        

        #region Sacrifice Ability

        public static void SacrificeAbility(PlayerMobile player)
        {
            double directionDelay = .25;
            double initialDelay = UOACZSystem.AbilityInitialDelaySeconds;
            double totalDelay = directionDelay + initialDelay;

            SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1, totalDelay, true, 0, false, "", "");

            player.RevealingAction();

            Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
            {
                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!player.IsUOACZUndead) return;

                int animation = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimation;
                int animationFrames = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimationFrames;

                player.Animate(animation, animationFrames, 1, true, false, 0);
                player.PlaySound(player.GetAngerSound());

                Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (!player.IsUOACZUndead) return;

                    AbilitySuccessful(player, UOACZUndeadAbilityType.Sacrifice);

                    player.PlaySound(0x5D8);

                    Queue m_Queue = new Queue();

                    foreach (BaseCreature follower in player.AllFollowers)
                    {
                        if (!UOACZSystem.IsUOACZValidMobile(follower)) continue;
                        if (Utility.GetDistance(player.Location, follower.Location) > 12) continue;
                        if (follower.Hits == follower.HitsMax) continue;

                        m_Queue.Enqueue(follower);
                    }
                    
                    while (m_Queue.Count > 0)
                    {
                        BaseCreature follower = (BaseCreature)m_Queue.Dequeue();

                        int healingAmount = (int)(Math.Round((double)follower.HitsMax * .25 * UOACZPersistance.UndeadBalanceScalar * UOACZSystem.GetUndeadAbilityScalar(player)));

                        follower.Heal(healingAmount);

                        follower.FixedParticles(0x376A, 9, 32, 5030, 0, 0, EffectLayer.Waist);
                        follower.PlaySound(0x202);
                    }

                    int damage = (int)(Math.Round((double)player.HitsMax * .10));

                    UOACZSystem.DamageCorpse(player.Location, player.Map, false);
                    
                    player.SendMessage("You offer a sacrifice in order to heal your followers.");

                    AOS.Damage(player, damage, 0, 100, 0, 0, 0);
                });
            });
        }

        #endregion

        #region Gift of Corruption Ability

        public static void GiftOfCorruptionAbility(PlayerMobile player)
        {
            player.SendMessage("Target a corrupted wildlife creature.");
            player.Target = new GiftOfCorruptionAbilityTarget();
        }

        public class GiftOfCorruptionAbilityTarget : Target
        {
            private IEntity targetLocation;

            public GiftOfCorruptionAbilityTarget(): base(25, false, TargetFlags.None, false)
            {
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile player = from as PlayerMobile;

                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!CanUseAbility(player, UOACZUndeadAbilityType.GiftOfCorruption, true)) return;

                if (target == player)
                {
                    player.SendMessage("You cannot target yourself.");
                    return;
                }

                if (!(target is UOACZBaseWildlife))
                {
                    player.SendMessage("That is not a wildlife creature.");
                    return;
                }

                UOACZBaseWildlife bc_Wildlife = target as UOACZBaseWildlife;

                if (!bc_Wildlife.Corrupted)
                {
                    player.SendMessage("That creature has not been corrupted yet.");
                    return;
                }
                
                if (Utility.GetDistance(player.Location, bc_Wildlife.Location) > 2)
                {
                    player.SendMessage("That creature is too far away.");
                    return;
                }

                if (!player.Map.InLOS(player.Location, bc_Wildlife.Location))
                {
                    player.SendMessage("That is not within in your line of sight.");
                    return;
                }

                player.RevealingAction();

                SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1.0, 1, true, 0, false, "", "");

                player.Direction = Utility.GetDirection(player.Location, bc_Wildlife.Location);

                Timer.DelayCall(TimeSpan.FromSeconds(.25), delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (!player.IsUOACZUndead) return;

                    if (!UOACZSystem.IsUOACZValidMobile(bc_Wildlife)) return;
                    if (Utility.GetDistance(player.Location, bc_Wildlife.Location) > 8) return;

                    int animation = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimation;
                    int animationFrames = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimationFrames;

                    player.Animate(animation, animationFrames, 1, true, false, 0);
                    player.PlaySound(player.GetAngerSound());

                    AbilitySuccessful(player, UOACZUndeadAbilityType.GiftOfCorruption);

                    Point3D location = bc_Wildlife.Location;
                    Map map = bc_Wildlife.Map;

                    Effects.SendLocationEffect(location, map, 0x36BD, 30, 10, 2210, 0);

                    bc_Wildlife.Kill();
                    UOACZSystem.DamageCorpse(location, map, true);

                    player.PlaySound(0x65A);
                    player.FixedParticles(0x375A, 10, 30, 5011, 2210, 0, EffectLayer.Head);

                    UOACZSystem.ChangeStat(player, UOACZSystem.UOACZStatType.UndeadScore, UOACZSystem.GiftOfCorruptionAbilityScore, true);

                    AchievementSystemImpl.Instance.TickProgressMulti(player, AchievementTriggers.Trigger_UOACZCorruptionAbilities, 1);

                    player.SendMessage(UOACZSystem.greenTextHue, "You draw power from the corruption and are rewarded for your efforts.");

                    if (player.Backpack != null)
                        player.Backpack.DropItem(new UOACZBrains());

                    if (Utility.RandomDouble() < UOACZSystem.GiftOfCorruptionAbilityCorruptionTokenChance * UOACZPersistance.UndeadBalanceScalar * UOACZSystem.GetUndeadAbilityScalar(player))
                    {
                        if (player.Backpack != null)
                        {
                            player.Backpack.DropItem(new UOACZCorruptionStone(player));
                            player.SendMessage(UOACZSystem.greenTextHue, "You have received a corruption stone.");
                        }
                    }

                    if (Utility.RandomDouble() < UOACZSystem.GiftOfCorruptionAbilityUpgradeTokenChance * UOACZPersistance.UndeadBalanceScalar * UOACZSystem.GetUndeadAbilityScalar(player))
                    {
                        if (player.Backpack != null)
                        {
                            player.Backpack.DropItem(new UOACZUndeadUpgradeToken(player));
                            player.SendMessage(UOACZSystem.greenTextHue, "You have received an upgrade token.");
                        }
                    }
                });
            }
        }

        #endregion

        #region Creep Ability

        public static void CreepAbility(PlayerMobile player)
        {
            player.SendMessage("Target a scavengeable or harvestable object.");
            player.Target = new CreepAbilityTarget();
        }

        public class CreepAbilityTarget : Target
        {
            private IEntity targetLocation;

            public CreepAbilityTarget(): base(25, true, TargetFlags.None, false)
            {
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile player = from as PlayerMobile;

                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!CanUseAbility(player, UOACZUndeadAbilityType.Creep, true)) return;

                UOACZBaseScavengeObject scavengeObject = null;

                if (target is UOACZBaseScavengeObject)
                    scavengeObject = target as UOACZBaseScavengeObject;

                if (scavengeObject == null)
                {
                    IPoint3D location = target as IPoint3D;

                    if (location == null)
                        return;

                    Map map = player.Map;

                    if (map == null)
                        return;

                    SpellHelper.GetSurfaceTop(ref location);

                    if (location is Mobile)
                        targetLocation = (Mobile)location;

                    else
                        targetLocation = new Entity(Serial.Zero, new Point3D(location), map);

                    IPooledEnumerable nearbyItems = map.GetItemsInRange(targetLocation.Location, 0);

                    foreach (Item item in nearbyItems)
                    {
                        if (item is UOACZBaseScavengeObject)
                        {
                            scavengeObject = item as UOACZBaseScavengeObject;
                            break;
                        }
                    }

                    nearbyItems.Free();
                }

                if (scavengeObject == null)
                {
                    player.SendMessage("That is not a scavengable or harvestable object.");
                    return;
                }

                if (Utility.GetDistance(player.Location, scavengeObject.Location) > 2)
                {
                    player.SendMessage("That is too far away.");
                    return;
                }

                if (!player.Map.InLOS(player.Location, scavengeObject.Location))
                {
                    player.SendMessage("That is not within in your line of sight.");
                    return;
                }

                if (scavengeObject.Corrupted)
                {
                    player.SendMessage("That is already corrupted.");
                    return;
                }

                double directionDelay = .25;
                double initialDelay = UOACZSystem.AbilityInitialDelaySeconds;
                double totalDelay = directionDelay + initialDelay;

                SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1, totalDelay, true, 0, false, "", "");

                player.RevealingAction();

                Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (!player.IsUOACZUndead) return;
                    if (scavengeObject == null) return;
                    if (scavengeObject.Deleted) return;

                    int animation = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimation;
                    int animationFrames = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimationFrames;

                    player.Animate(animation, animationFrames, 1, true, false, 0);
                    player.PlaySound(player.GetAngerSound());

                    Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
                    {
                        if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                        if (!player.IsUOACZUndead) return;
                        if (scavengeObject == null) return;
                        if (scavengeObject.Deleted) return;

                        AbilitySuccessful(player, UOACZUndeadAbilityType.Creep);

                        Effects.PlaySound(scavengeObject.Location, scavengeObject.Map, 0x5CB);

                        scavengeObject.Corrupted = true;                        

                        Static corruption = new Static();

                        corruption.ItemID = 0x376A;
                        corruption.Hue = 2051;
                        corruption.Name = "corruption";
                        corruption.MoveToWorld(scavengeObject.Location, scavengeObject.Map);

                        scavengeObject.m_CorruptionItem = corruption;

                        scavengeObject.TrapType = UOACZBaseScavengeObject.ScavengeTrapType.Undead;
                        scavengeObject.TrapResolveChance = 20;

                        Timer.DelayCall(TimeSpan.FromMinutes(30), delegate
                        {
                            if (scavengeObject != null)
                            {
                                if (!scavengeObject.Deleted)
                                {
                                    scavengeObject.Corrupted = false;
                                    scavengeObject.TrapType = UOACZBaseScavengeObject.ScavengeTrapType.None;
                                    scavengeObject.TrapResolveChance = 0;
                                }
                            }

                            if (corruption != null)
                            {
                                if (!corruption.Deleted)
                                    corruption.Delete();
                            }
                        });

                        UOACZSystem.ChangeStat(player, UOACZSystem.UOACZStatType.UndeadScore, UOACZSystem.CreepAbilityScore, true);

                        AchievementSystemImpl.Instance.TickProgressMulti(player, AchievementTriggers.Trigger_UOACZCorruptionAbilities, 1);

                        player.SendMessage(UOACZSystem.greenTextHue, "You spread corruption and are rewarded for your efforts.");

                        UOACZEvents.SpreadCorruption();

                        if (player.Backpack != null)
                        {
                            player.Backpack.DropItem(new UOACZBrains());
                            player.SendMessage(UOACZSystem.greenTextHue, "");
                        }

                        if (Utility.RandomDouble() < UOACZSystem.CreepAbilityCorruptionTokenChance * UOACZPersistance.UndeadBalanceScalar * UOACZSystem.GetUndeadAbilityScalar(player))
                        {
                            if (player.Backpack != null)
                            {
                                player.Backpack.DropItem(new UOACZCorruptionStone(player));
                                player.SendMessage(UOACZSystem.greenTextHue, "You have received a corruption stone.");
                            }
                        }

                        if (Utility.RandomDouble() < UOACZSystem.CreepAbilityUpgradeTokenChance * UOACZPersistance.UndeadBalanceScalar * UOACZSystem.GetUndeadAbilityScalar(player))
                        {
                            if (player.Backpack != null)
                            {
                                player.Backpack.DropItem(new UOACZUndeadUpgradeToken(player));
                                player.SendMessage(UOACZSystem.greenTextHue, "You have received an upgrade token.");
                            }
                        }
                    });
                });
            }
        }

        #endregion

        #region Dig Ability

        public static void DigAbility(PlayerMobile player)
        {
            if (player.LastPlayerCombatTime + UOACZSystem.TunnelDigPvPThreshold > DateTime.UtcNow)
            {
                string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, player.LastPlayerCombatTime + UOACZSystem.TunnelDigPvPThreshold, false, true, true, true, true);

                player.SendMessage("You have been in combat with another player too recently and must wait " + timeRemaining + " before you may use this ability.");
                return;
            }

            double directionDelay = .25;
            double initialDelay = UOACZSystem.AbilityInitialDelaySeconds;
            double totalDelay = directionDelay + initialDelay;

            SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1, totalDelay, true, 0, false, "", "");
            
            Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
            {
                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!player.IsUOACZUndead) return;

                int animation = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimation;
                int animationFrames = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimationFrames;

                player.Animate(animation, animationFrames, 1, true, false, 0);
                player.PlaySound(player.GetAngerSound());

                Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (!player.IsUOACZUndead) return;

                    AbilitySuccessful(player, UOACZUndeadAbilityType.Dig);

                    List<UOACZTunnel> m_Destinations = new List<UOACZTunnel>();

                    foreach (UOACZTunnel tunnel in UOACZTunnel.m_Instances)
                    {
                        if (tunnel == null) continue;
                        if (tunnel.Deleted) continue;
                        if (!UOACZRegion.ContainsItem(tunnel)) continue;
                        if (tunnel.TunnelType == UOACZTunnel.TunnelLocation.Wilderness) continue;

                        m_Destinations.Add(tunnel);
                    }

                    if (m_Destinations.Count == 0)
                        return;

                    UOACZTunnel targetTunnel = m_Destinations[Utility.RandomMinMax(0, m_Destinations.Count - 1)];

                    Point3D startingLocation = player.Location;
                    Map map = player.Map;

                    Effects.PlaySound(startingLocation, map, 0x247);
                    Effects.SendLocationEffect(startingLocation, map, 0x3728, 10, 10, 0, 0);

                    TimedStatic dirt = new TimedStatic(Utility.RandomList(7681, 7682), 5);
                    dirt.Name = "dirt";
                    dirt.MoveToWorld(startingLocation, map);
                    dirt.PublicOverheadMessage(MessageType.Regular, 0, false, "*digs tunnel*");

                    for (int b = 0; b < 8; b++)
                    {
                        dirt = new TimedStatic(Utility.RandomList(7681, 7682), 5);
                        dirt.Name = "dirt";
                        Point3D dirtLocation = new Point3D(startingLocation.X + Utility.RandomList(-2, -1, 1, 2), startingLocation.Y + Utility.RandomList(-2, -1, 1, 2), startingLocation.Z);
                        SpellHelper.AdjustField(ref dirtLocation, map, 12, false);

                        dirt.MoveToWorld(dirtLocation, map);
                    }

                    Effects.SendLocationEffect(targetTunnel.Location, targetTunnel.Map, 0x3728, 10, 10, 0, 0);
                    Effects.PlaySound(targetTunnel.Location, targetTunnel.Map, 0x247);

                    player.Location = targetTunnel.Location;

                    foreach (Mobile follower in player.AllFollowers)
                    {
                        if (UOACZSystem.IsUOACZValidMobile(follower))
                            follower.Location = targetTunnel.Location;
                    }

                    dirt = new TimedStatic(Utility.RandomList(7681, 7682), 5);
                    dirt.Name = "dirt";
                    dirt.MoveToWorld(targetTunnel.Location, map);
                    dirt.PublicOverheadMessage(MessageType.Regular, 0, false, "*appears from tunnel*");

                    for (int b = 0; b < 8; b++)
                    {
                        dirt = new TimedStatic(Utility.RandomList(7681, 7682), 5);
                        dirt.Name = "dirt";

                        Point3D dirtLocation = new Point3D(targetTunnel.Location.X + Utility.RandomList(-2, -1, 1, 2), targetTunnel.Location.Y + Utility.RandomList(-2, -1, 1, 2), targetTunnel.Location.Z);
                        SpellHelper.AdjustField(ref dirtLocation, map, 12, false);

                        dirt.MoveToWorld(dirtLocation, map);
                    }
                });
            });
        }

        #endregion

        #region Rally Ability

        public static void RallyAbility(PlayerMobile player)
        {
            if (player.LastPlayerCombatTime + TimeSpan.FromSeconds(30) >= DateTime.UtcNow)
            {
                string cooldownRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, player.LastCombatTime + TimeSpan.FromSeconds(30), false, false, false, true, true);

                player.SendMessage("You have been in combat with another player too recently to use this ability. You must wait another " + cooldownRemaining + ".");
                
                return;
            }

            double directionDelay = .25;
            double initialDelay = UOACZSystem.AbilityInitialDelaySeconds;
            double totalDelay = directionDelay + initialDelay;

            SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1, totalDelay, true, 0, false, "", "");

            player.RevealingAction();

            Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
            {
                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!player.IsUOACZUndead) return;

                int animation = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimation;
                int animationFrames = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimationFrames;

                player.Animate(animation, animationFrames, 1, true, false, 0);
                player.PlaySound(player.GetAngerSound());

                Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (!player.IsUOACZUndead) return;

                    AbilitySuccessful(player, UOACZUndeadAbilityType.Rally);

                    foreach (Mobile follower in player.AllFollowers)
                    {
                        if (UOACZSystem.IsUOACZValidMobile(follower))
                        {
                            Effects.SendLocationParticles(EffectItem.Create(follower.Location, follower.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 0, 0, 2023, 0);
                            Effects.PlaySound(follower.Location, follower.Map, 0x1FC);

                            follower.Location = player.Location;
                            follower.PlaySound(follower.IdleSound);

                            Effects.SendLocationParticles(EffectItem.Create(follower.Location, follower.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 0, 0, 2023, 0);
                            Effects.PlaySound(follower.Location, follower.Map, 0x1FC);
                        }
                    }
                });
            });
        }

        #endregion


        #region Recruit Zombie Ability

        public static void RecruitZombieAbility(PlayerMobile player)
        {
            player.SendMessage("Target a location to raise a zombie from.");
            player.Target = new RecruitZombieAbilityTarget();            
        }

        public class RecruitZombieAbilityTarget : Target
        {
            private IEntity targetLocation;

            public RecruitZombieAbilityTarget(): base(25, true, TargetFlags.None, false)
            {
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile player = from as PlayerMobile;

                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!CanUseAbility(player, UOACZUndeadAbilityType.RecruitZombie, true)) return;

                IPoint3D location = target as IPoint3D;

                if (location == null)
                    return;

                Map map = player.Map;

                if (map == null)
                    return;

                SpellHelper.GetSurfaceTop(ref location);

                if (location is Mobile)
                    targetLocation = (Mobile)location;

                else
                    targetLocation = new Entity(Serial.Zero, new Point3D(location), map);

                if (Utility.GetDistance(player.Location, targetLocation.Location) > 8)
                {
                    player.SendMessage("That target is too far away.");
                    return;
                }

                if (!player.Map.InLOS(player.Location, targetLocation.Location))
                {
                    player.SendMessage("That location is not within in your line of sight.");
                    return;
                }

                double directionDelay = .25;
                double initialDelay = UOACZSystem.AbilityInitialDelaySeconds;
                double totalDelay = directionDelay + initialDelay;

                SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1, totalDelay, true, 0, false, "", "");

                player.RevealingAction();
                
                Point3D playerLocation = player.Location;
                Map playerMap = player.Map;

                Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (!player.IsUOACZUndead) return;

                    int animation = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimation;
                    int animationFrames = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimationFrames;

                    player.Animate(animation, animationFrames, 1, true, false, 0);
                    player.PlaySound(player.GetAngerSound());

                    Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
                    {
                        if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                        if (!player.IsUOACZUndead) return;

                        AbilitySuccessful(player, UOACZUndeadAbilityType.RecruitZombie);

                        UOACZZombie creature = new UOACZZombie();
                        creature.MoveToWorld(targetLocation.Location, targetLocation.Map);

                        int controlSlotsNeeded = 1;

                        if (player.Followers + controlSlotsNeeded <= player.FollowersMax)
                        {
                            creature.TimesTamed++;
                            creature.SetControlMaster(player);
                            creature.IsBonded = false;
                            creature.OwnerAbandonTime = DateTime.UtcNow + creature.AbandonDelay;

                            UOACZSystem.AddCreatureToPlayerSwarm(player, creature);
                        }

                        else
                            player.SendMessage("One or more creatures were unable to join your swarm as they would exceed your swarm control slot limit.");

                        creature.PlaySound(creature.GetIdleSound());
                    });
                });
            }
        }

        #endregion
       
        #region Aura of Decay Ability

        public static void AuraOfDecayAbility(PlayerMobile player)
        {
            double directionDelay = .25;
            double initialDelay = UOACZSystem.AbilityInitialDelaySeconds;
            double totalDelay = directionDelay + initialDelay;

            SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1, totalDelay, true, 0, false, "", "");

            player.RevealingAction();

            Point3D playerLocation = player.Location;
            Map playerMap = player.Map;
            
            Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
            {
                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!player.IsUOACZUndead) return;

                int animation = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimation;
                int animationFrames = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimationFrames;

                player.Animate(animation, animationFrames, 1, true, false, 0);
                player.PlaySound(player.GetAngerSound());

                Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (!player.IsUOACZUndead) return;

                    AbilitySuccessful(player, UOACZUndeadAbilityType.AuraOfDecay);

                    double duration = 30;

                    Timer.DelayCall(TimeSpan.FromSeconds(duration), delegate
                    {
                        if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                        if (!player.IsUOACZUndead) return;

                        player.SendMessage("Aura of Decay ability effect has expired.");
                    });

                    for (int a = 0; a < duration; a++)
                    {
                        Timer.DelayCall(TimeSpan.FromSeconds(a * 1), delegate
                        {
                            if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                            if (!player.IsUOACZUndead) return;

                            player.PlaySound(0x5CB);
                            Effects.SendLocationParticles(EffectItem.Create(player.Location, player.Map, TimeSpan.FromSeconds(0.25)), 0x376A, 10, 20, 2199, 0, 5029, 0);

                            Queue m_Queue = new Queue();

                            IPooledEnumerable nearbyMobiles = player.Map.GetMobilesInRange(player.Location, 1);

                            foreach (Mobile mobile in nearbyMobiles)
                            {
                                if (!UOACZSystem.IsUOACZValidMobile(mobile)) continue;
                                if (mobile == player) continue;                                
                                if (mobile is UOACZBaseUndead) continue;

                                PlayerMobile playerTarget = mobile as PlayerMobile;
                                {
                                    if (playerTarget != null)
                                    {
                                        if (playerTarget.IsUOACZUndead)
                                            continue;
                                    }
                                }

                                m_Queue.Enqueue(mobile);
                            }

                            nearbyMobiles.Free();

                            while (m_Queue.Count > 0)
                            {
                                Mobile mobile = (Mobile)m_Queue.Dequeue();

                                mobile.PlaySound(0x5CB);
                                Effects.SendLocationParticles(EffectItem.Create(mobile.Location, mobile.Map, TimeSpan.FromSeconds(0.25)), 0x376A, 10, 20, 2199, 0, 5029, 0);
                                
                                int minDamage = 2;
                                int maxDamage = 4;

                                double damage = (double)Utility.RandomMinMax(minDamage, maxDamage);
                                double damageScalar = 1.0;

                                if (mobile is PlayerMobile)
                                {
                                    damageScalar *= UOACZSystem.GetFatigueScalar(player);
                                }

                                if (mobile is BaseCreature)
                                {
                                    BaseCreature bc_Creature = mobile as BaseCreature;

                                    damageScalar = 2;

                                    if (bc_Creature.Controlled)
                                        damageScalar *= UOACZSystem.GetFatigueScalar(player);                                    
                                }

                                damageScalar *= UOACZPersistance.UndeadBalanceScalar * UOACZSystem.GetUndeadAbilityScalar(player);

                                damage = (Math.Round((double)damage * damageScalar));

                                player.DoHarmful(mobile);

                                UOACZSystem.PlayerInflictDamage(player, mobile, false, (int)damage);

                                new Blood().MoveToWorld(mobile.Location, mobile.Map);
                                AOS.Damage(mobile, player, (int)damage, 0, 100, 0, 0, 0);
                            }
                        });
                    }
                });
            });
        }

        #endregion

        #region Bloody Mess Ability

        public static void BloodyMessAbility(PlayerMobile player)
        {                     
            double directionDelay = .25;
            double initialDelay = UOACZSystem.AbilityInitialDelaySeconds;
            double totalDelay = directionDelay + initialDelay;

            SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1, totalDelay, true, 0, false, "", "");

            player.RevealingAction();

            int radius = 3;

            Point3D playerLocation = player.Location;
            Map playerMap = player.Map;           

            Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
            {
                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!player.IsUOACZUndead) return;

                int animation = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimation;
                int animationFrames = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimationFrames;

                player.Animate(animation, animationFrames, 1, true, false, 0);
                player.PlaySound(player.GetAngerSound());

                Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (!player.IsUOACZUndead) return;

                    AbilitySuccessful(player, UOACZUndeadAbilityType.BloodyMess);

                    Effects.PlaySound(playerLocation, playerMap, 0x4F1);

                    int projectiles = 12;
                    int particleSpeed = 4;

                    for (int a = 0; a < projectiles; a++)
                    {
                        Point3D newLocation = new Point3D(player.X + Utility.RandomList(-5, -4, -3, -2, -1, 1, 2, 3, 4, 5), player.Y + Utility.RandomList(-5, -4, -3, -2, -1, 1, 2, 3, 4, 5), player.Z);
                        SpellHelper.AdjustField(ref newLocation, player.Map, 12, false);

                        IEntity effectStartLocation = new Entity(Serial.Zero, new Point3D(player.X, player.Y, player.Z + 5), player.Map);
                        IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + 5), player.Map);

                        Effects.SendMovingEffect(effectStartLocation, effectEndLocation, Utility.RandomList(0x1645, 0x122A, 0x122B, 0x122C, 0x122D, 0x122E, 0x122F), particleSpeed, 0, false, false, 0, 0);
                    }
                    
                    Queue m_Queue = new Queue();

                    IPooledEnumerable nearbyMobiles = playerMap.GetMobilesInRange(playerLocation, radius);

                    foreach (Mobile mobile in nearbyMobiles)
                    {
                        if (!UOACZSystem.IsUOACZValidMobile(mobile)) continue;
                        if (mobile == player) continue;
                        if (!playerMap.InLOS(playerLocation, mobile.Location)) continue;
                        if (mobile is UOACZBaseUndead) continue;

                        PlayerMobile playerTarget = mobile as PlayerMobile;
                        {
                            if (playerTarget != null)
                            {
                                if (playerTarget.IsUOACZUndead)
                                    continue;
                            }
                        }

                        m_Queue.Enqueue(mobile);
                    }

                    nearbyMobiles.Free();

                    while (m_Queue.Count > 0)
                    {
                        Mobile mobile = (Mobile)m_Queue.Dequeue();

                        mobile.PlaySound(0x4F1);

                        int minDamage = 15;
                        int maxDamage = 25;

                        double damage = (double)Utility.RandomMinMax(minDamage, maxDamage);                        

                        double damageScalar = 1.0;                        

                        if (mobile is PlayerMobile)
                        {
                            damageScalar *= UOACZSystem.GetFatigueScalar(player);
                        }

                        if (mobile is BaseCreature)
                        {
                            BaseCreature bc_Creature = mobile as BaseCreature;

                            damageScalar = 3;

                            if (bc_Creature.Controlled)
                                damageScalar *= UOACZSystem.GetFatigueScalar(player);                            
                        }

                        damageScalar *= UOACZPersistance.UndeadBalanceScalar * UOACZSystem.GetUndeadAbilityScalar(player);

                        damage = (Math.Round((double)damage * damageScalar));

                        player.DoHarmful(mobile);
                        
                        SpecialAbilities.BleedSpecialAbility(1.0, player, mobile, damage, 8, -1, true, "", "You begin to bleed!.");
                    }
                });
            });
        }

        #endregion

        #region Ignite Ability

        public static void IgniteAbility(PlayerMobile player)
        {
            double directionDelay = .25;
            double initialDelay = UOACZSystem.AbilityInitialDelaySeconds;
            double totalDelay = directionDelay + initialDelay;

            SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1, totalDelay, true, 0, false, "", "");

            player.RevealingAction();

            int radius = 3;

            Point3D playerLocation = player.Location;
            Map playerMap = player.Map;

            Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
            {
                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!player.IsUOACZUndead) return;

                int animation = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimation;
                int animationFrames = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimationFrames;

                player.Animate(animation, animationFrames, 1, true, false, 0);
                player.PlaySound(player.GetAngerSound());

                Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (!player.IsUOACZUndead) return;

                    AbilitySuccessful(player, UOACZUndeadAbilityType.Ignite);

                    player.FixedParticles(0x3709, 10, 30, 5052, EffectLayer.LeftFoot);
                    player.PlaySound(0x5CF);

                    int minRange = -1 * radius;
                    int maxRange = radius + 1;

                    for (int a = minRange; a < maxRange; a++)
                    {
                        for (int b = minRange; b < maxRange; b++)
                        {
                            if (Utility.RandomDouble() <= .15)
                            {
                                Point3D newLocation = new Point3D(playerLocation.X + a, playerLocation.Y + b, playerLocation.Z);

                                double distance = Utility.GetDistanceToSqrt(playerLocation, newLocation);
                                double distanceDelay = (distance * .05);

                                Timer.DelayCall(TimeSpan.FromSeconds(distanceDelay), delegate
                                {
                                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                                    if (!player.IsUOACZUndead) return;

                                    new UOACZFirefield(player).MoveToWorld(newLocation, playerMap);
                                });
                            }
                        }
                    }
                });
            });
        }

        #endregion

        #region Bile Ability

        public static void BileAbility(PlayerMobile player)
        {
            double directionDelay = .25;
            double initialDelay = UOACZSystem.AbilityInitialDelaySeconds;
            double totalDelay = directionDelay + initialDelay;

            SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1, totalDelay, true, 0, false, "", "");

            player.RevealingAction();

            int radius = 3;

            Point3D playerLocation = player.Location;
            Map playerMap = player.Map;

            Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
            {
                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!player.IsUOACZUndead) return;

                int animation = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimation;
                int animationFrames = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimationFrames;

                player.Animate(animation, animationFrames, 1, true, false, 0);
                player.PlaySound(player.GetAngerSound());

                Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (!player.IsUOACZUndead) return;

                    AbilitySuccessful(player, UOACZUndeadAbilityType.Bile);

                    player.FixedEffect(0x372A, 10, 30, 2208, 0);
                    player.PlaySound(0x22F);

                    int minRange = -1 * radius;
                    int maxRange = radius + 1;

                    for (int a = minRange; a < maxRange; a++)
                    {
                        for (int b = minRange; b < maxRange; b++)
                        {
                            if (Utility.RandomDouble() <= .2)
                            {
                                Point3D newLocation = new Point3D(playerLocation.X + a, playerLocation.Y + b, playerLocation.Z);

                                double distance = Utility.GetDistanceToSqrt(playerLocation, newLocation);
                                double distanceDelay = (distance * .05);

                                Timer.DelayCall(TimeSpan.FromSeconds(distanceDelay), delegate
                                {
                                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                                    if (!player.IsUOACZUndead) return;

                                    new UOACZBile(player).MoveToWorld(newLocation, playerMap);
                                });
                            }
                        }
                    }
                });
            });
        }

        #endregion

        #region Combust Ability

        public static void CombustAbility(PlayerMobile player)
        {
            player.SendMessage("Target a firefield.");
            player.Target = new CombustAbilityTarget();
        }

        public class CombustAbilityTarget : Target
        {
            private IEntity targetLocation;

            public CombustAbilityTarget(): base(25, true, TargetFlags.None, false)
            {
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile player = from as PlayerMobile;

                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!CanUseAbility(player, UOACZUndeadAbilityType.Combust, true)) return;

                UOACZFirefield firefield = null;

                if (target is UOACZFirefield)                
                    firefield = target as UOACZFirefield;

                if (firefield == null)
                {
                    IPoint3D location = target as IPoint3D;

                    if (location == null)
                        return;
                
                    Map map = player.Map;

                    if (map == null)
                        return;

                    SpellHelper.GetSurfaceTop(ref location);

                    if (location is Mobile)
                        targetLocation = (Mobile)location;

                    else
                        targetLocation = new Entity(Serial.Zero, new Point3D(location), map);

                    IPooledEnumerable nearbyItems = map.GetItemsInRange(targetLocation.Location, 0);

                    foreach (Item item in nearbyItems)
                    {
                        if (item is UOACZFirefield)
                        {
                            firefield = item as UOACZFirefield;
                            break;
                        }
                    }

                    nearbyItems.Free();
                }

                if (firefield == null)
                {
                    player.SendMessage("That is not a firefield.");
                    return;
                }

                if (Utility.GetDistance(player.Location, firefield.Location) > 8)
                {
                    player.SendMessage("That firefield is too far away.");
                    return;
                }

                if (!player.Map.InLOS(player.Location, firefield.Location))
                {
                    player.SendMessage("That firefield is not within in your line of sight.");
                    return;
                }

                double directionDelay = .25;
                double initialDelay = UOACZSystem.AbilityInitialDelaySeconds;
                double totalDelay = directionDelay + initialDelay;

                SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1, totalDelay, true, 0, false, "", "");

                player.RevealingAction();
                
                Point3D playerLocation = player.Location;
                Map playerMap = player.Map;

                Direction directionToTarget = Utility.GetDirection(playerLocation, firefield.Location);

                Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (!player.IsUOACZUndead) return;
                    if (firefield == null) return;
                    if (firefield.Deleted) return;  

                    int animation = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimation;
                    int animationFrames = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimationFrames;

                    player.Animate(animation, animationFrames, 1, true, false, 0);
                    player.PlaySound(player.GetAngerSound());

                    Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
                    {
                        if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                        if (!player.IsUOACZUndead) return;
                        if (firefield == null) return;
                        if (firefield.Deleted) return;                            
                            
                        AbilitySuccessful(player, UOACZUndeadAbilityType.Combust);

                        Effects.PlaySound(firefield.Location, firefield.Map, 0x309);

                        int radius = 2;

                        int minRange = radius * -1;
                        int maxRange = radius;

                        int effectHue = 0;

                        Point3D firefieldLocation = firefield.Location;
                        Map firefieldMap = firefield.Map;

                        firefield.Delete();

                        for (int a = minRange; a < maxRange + 1; a++)
                        {
                            for (int b = minRange; b < maxRange + 1; b++)
                            {
                                Point3D newPoint = new Point3D(firefieldLocation.X + a, firefieldLocation.Y + b, firefieldLocation.Z);
                                SpellHelper.AdjustField(ref newPoint, firefieldMap, 12, false);

                                int distance = Utility.GetDistance(firefieldLocation, newPoint);

                                Timer.DelayCall(TimeSpan.FromSeconds(distance * .20), delegate
                                {
                                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                                    if (!player.IsUOACZUndead) return;

                                    Effects.PlaySound(newPoint, firefieldMap, Utility.RandomList(0x208));
                                    Effects.SendLocationParticles(EffectItem.Create(newPoint, firefieldMap, TimeSpan.FromSeconds(0.25)), 0x3709, 10, 30, effectHue, 0, 5029, 0);

                                    IPooledEnumerable nearbyMobiles = firefieldMap.GetMobilesInRange(newPoint, 0);

                                    Queue m_Queue = new Queue();

                                    foreach (Mobile mobile in nearbyMobiles)
                                    {
                                        if (!UOACZSystem.IsUOACZValidMobile(mobile)) continue;                                       
                                        if (mobile == player) continue;
                                        if (!firefieldMap.InLOS(firefieldLocation, mobile.Location)) continue;
                                        if (mobile is UOACZBaseUndead) continue;

                                        PlayerMobile playerTarget = mobile as PlayerMobile;

                                        if (playerTarget != null)
                                        {
                                            if (playerTarget.IsUOACZUndead)
                                                continue;
                                        }

                                        m_Queue.Enqueue(mobile);
                                    }

                                    nearbyMobiles.Free();

                                    while (m_Queue.Count > 0)
                                    {
                                        Mobile mobile = (Mobile)m_Queue.Dequeue();

                                        int minDamage = 10;
                                        int maxDamage = 20;

                                        double damage = (double)Utility.RandomMinMax(minDamage, maxDamage);

                                        double damageScalar = 1.0;

                                        if (mobile is PlayerMobile)
                                        {
                                            damageScalar *= UOACZSystem.GetFatigueScalar(player);
                                        }

                                        if (mobile is BaseCreature)
                                        {
                                            BaseCreature bc_Creature = mobile as BaseCreature;

                                            damageScalar = 3;

                                            if (bc_Creature.Controlled)
                                                damageScalar *= UOACZSystem.GetFatigueScalar(player);                                            
                                        }

                                        damageScalar *= UOACZPersistance.UndeadBalanceScalar * UOACZSystem.GetUndeadAbilityScalar(player);                                        

                                        player.DoHarmful(mobile);

                                        damage = (Math.Round((double)damage * damageScalar));

                                        UOACZSystem.PlayerInflictDamage(player, mobile, false, (int)damage);

                                        new Blood().MoveToWorld(mobile.Location, mobile.Map);
                                        AOS.Damage(mobile, from, (int)damage, 0, 100, 0, 0, 0);
                                    }
                                });
                            }
                        }
                    });
                });
            }
        }

        #endregion
               
        #region Virus Ability

        public static void VirusAbility(PlayerMobile player)
        {
            player.SendMessage("Target a bile location.");
            player.Target = new VirusAbilityTarget();
        }

        public class VirusAbilityTarget : Target
        {
            private IEntity targetLocation;

            public VirusAbilityTarget(): base(25, true, TargetFlags.None, false)
            {
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile player = from as PlayerMobile;

                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!CanUseAbility(player, UOACZUndeadAbilityType.Virus, true)) return;

                UOACZBile bile = null;

                if (target is UOACZBile)
                    bile = target as UOACZBile;

                if (bile == null)
                {
                    IPoint3D location = target as IPoint3D;

                    if (location == null)
                        return;

                    Map map = player.Map;

                    if (map == null)
                        return;

                    SpellHelper.GetSurfaceTop(ref location);

                    if (location is Mobile)
                        targetLocation = (Mobile)location;

                    else
                        targetLocation = new Entity(Serial.Zero, new Point3D(location), map);

                    IPooledEnumerable nearbyItems = map.GetItemsInRange(targetLocation.Location, 0);

                    foreach (Item item in nearbyItems)
                    {
                        if (item is UOACZBile)
                        {
                            bile = item as UOACZBile;
                            break;
                        }
                    }

                    nearbyItems.Free();
                }

                if (bile == null)
                {
                    player.SendMessage("That is not a bile location.");
                    return;
                }

                if (Utility.GetDistance(player.Location, bile.Location) > 8)
                {
                    player.SendMessage("That bile location is too far away.");
                    return;
                }

                if (!player.Map.InLOS(player.Location, bile.Location))
                {
                    player.SendMessage("That bile location is not within in your line of sight.");
                    return;
                }

                double directionDelay = .25;
                double initialDelay = UOACZSystem.AbilityInitialDelaySeconds;
                double totalDelay = directionDelay + initialDelay;

                SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1, totalDelay, true, 0, false, "", "");

                player.RevealingAction();

                Point3D playerLocation = player.Location;
                Map playerMap = player.Map;

                Direction directionToTarget = Utility.GetDirection(playerLocation, bile.Location);

                Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (!player.IsUOACZUndead) return;
                    if (bile == null) return;
                    if (bile.Deleted) return;

                    int animation = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimation;
                    int animationFrames = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimationFrames;

                    player.Animate(animation, animationFrames, 1, true, false, 0);
                    player.PlaySound(player.GetAngerSound());

                    Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
                    {
                        if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                        if (!player.IsUOACZUndead) return;
                        if (bile == null) return;
                        if (bile.Deleted) return;

                        AbilitySuccessful(player, UOACZUndeadAbilityType.Virus);

                        Effects.PlaySound(bile.Location, bile.Map, 0x56C);

                        int radius = 2;

                        int minRange = radius * -1;
                        int maxRange = radius;

                        int effectHue = 2210;

                        Point3D bileLocation = bile.Location;
                        Map bileMap = bile.Map;

                        bile.Delete();

                        for (int a = minRange; a < maxRange + 1; a++)
                        {
                            for (int b = minRange; b < maxRange + 1; b++)
                            {
                                Point3D newPoint = new Point3D(bileLocation.X + a, bileLocation.Y + b, bileLocation.Z);
                                SpellHelper.AdjustField(ref newPoint, bileMap, 12, false);

                                int distance = Utility.GetDistance(bileLocation, newPoint);

                                Timer.DelayCall(TimeSpan.FromSeconds(distance * .20), delegate
                                {
                                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                                    if (!player.IsUOACZUndead) return;

                                    Effects.PlaySound(newPoint, bileMap, Utility.RandomList(0x56C));
                                    Effects.SendLocationParticles(EffectItem.Create(newPoint, bileMap, TimeSpan.FromSeconds(0.25)), 0x3709, 10, 30, effectHue, 0, 5029, 0);

                                    IPooledEnumerable nearbyMobiles = bileMap.GetMobilesInRange(newPoint, 0);

                                    Queue m_Queue = new Queue();

                                    foreach (Mobile mobile in nearbyMobiles)
                                    {
                                        if (!UOACZSystem.IsUOACZValidMobile(mobile)) continue;
                                        if (mobile == player) continue;
                                        if (!bileMap.InLOS(bileLocation, mobile.Location)) continue;
                                        if (mobile is UOACZBaseUndead) continue;

                                        PlayerMobile playerTarget = mobile as PlayerMobile;

                                        if (playerTarget != null)
                                        {
                                            if (playerTarget.IsUOACZUndead)
                                                continue;
                                        }

                                        m_Queue.Enqueue(mobile);
                                    }

                                    nearbyMobiles.Free();

                                    while (m_Queue.Count > 0)
                                    {
                                        Mobile mobile = (Mobile)m_Queue.Dequeue();

                                        int minPoisonLevel = 2;
                                        int maxPoisonLevel = 3;

                                        if (mobile is BaseCreature)
                                        {
                                            BaseCreature bc_Creature = mobile as BaseCreature;

                                            if (bc_Creature.Controlled && UOACZSystem.GetFatigueScalar(player) < 1.0)
                                            {
                                                minPoisonLevel = 0;
                                                maxPoisonLevel = 0;
                                            }
                                        }

                                        if (mobile is PlayerMobile)
                                        {
                                            minPoisonLevel--;
                                            maxPoisonLevel--;

                                            if (UOACZSystem.GetFatigueScalar(player) < 1.0)
                                            {
                                                minPoisonLevel = 0;
                                                maxPoisonLevel = 0;
                                            }
                                        }

                                        player.DoHarmful(mobile);

                                        Poison poison = Poison.GetPoison(Utility.RandomMinMax(minPoisonLevel, maxPoisonLevel));
                                        mobile.ApplyPoison(player, poison);
                                    }
                                });
                            }
                        }
                    });
                });
            }
        }

        #endregion

        #region Plague Ability

        public static void PlagueAbility(PlayerMobile player)
        {
            player.SendMessage("Target a creature or player to attack.");
            player.Target = new PlagueAbilityTarget();
        }

        public class PlagueAbilityTarget : Target
        {
            private IEntity targetLocation;

            public PlagueAbilityTarget(): base(25, false, TargetFlags.Harmful, false)
            {
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile player = from as PlayerMobile;

                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!CanUseAbility(player, UOACZUndeadAbilityType.Plague, true)) return;

                Mobile mobileTarget = null;

                if (target == player)
                {
                    player.SendMessage("You cannot target yourself.");
                    return;
                }

                if (target is Mobile)
                {
                    mobileTarget = target as Mobile;

                    if (!UOACZSystem.IsUOACZValidMobile(mobileTarget))
                    {
                        player.SendMessage("That cannot be damaged.");
                        return;                        
                    }

                    if (mobileTarget is UOACZBaseUndead)
                    {
                        player.SendMessage("They are immune to that effect.");
                        return;  
                    }

                    PlayerMobile playerTarget = mobileTarget as PlayerMobile;

                    if (playerTarget != null)
                    {
                        if (playerTarget.IsUOACZUndead)
                        {
                            player.SendMessage("They are immune to that effect.");
                            return; 
                        }
                    }
                }

                else
                {
                    player.SendMessage("Perhaps this would be better off targeted at something living.");
                    return;
                }

                Map map = player.Map;

                if (Utility.GetDistance(player.Location, mobileTarget.Location) > 8)
                {
                    player.SendMessage("That target is too far away.");
                    return;
                }

                if (!player.Map.InLOS(player.Location, mobileTarget.Location))
                {
                    player.SendMessage("That is not within in your line of sight.");
                    return;
                }

                double directionDelay = .25;
                double initialDelay = UOACZSystem.AbilityInitialDelaySeconds;
                double totalDelay = directionDelay + initialDelay;

                SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1, totalDelay, true, 0, false, "", "");

                player.RevealingAction();

                Point3D playerLocation = player.Location;
                Map playerMap = player.Map;

                Direction directionToTarget = Utility.GetDirection(playerLocation, mobileTarget.Location);

                Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (!player.IsUOACZUndead) return;
                    if (!UOACZSystem.IsUOACZValidMobile(mobileTarget)) return;
                    if (Utility.GetDistance(player.Location, mobileTarget.Location) >= 20) return;
                    if (mobileTarget.Hidden) return;

                    int animation = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimation;
                    int animationFrames = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimationFrames;

                    player.Animate(animation, animationFrames, 1, true, false, 0);
                    player.PlaySound(player.GetAngerSound());

                    Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
                    {
                        if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                        if (!player.IsUOACZUndead) return;
                        if (!UOACZSystem.IsUOACZValidMobile(mobileTarget)) return;
                        if (Utility.GetDistance(player.Location, mobileTarget.Location) >= 20) return;
                        if (mobileTarget.Hidden) return;

                        AbilitySuccessful(player, UOACZUndeadAbilityType.Plague);

                        Effects.SendLocationParticles(EffectItem.Create(mobileTarget.Location, mobileTarget.Map, TimeSpan.FromSeconds(0.2)), 0x372A, 6, 20, 2636, 0, 5029, 0);
                        Effects.PlaySound(mobileTarget.Location, mobileTarget.Map, 0x457);

                        for (int a = 0; a < 4; a++)
                        {
                            Point3D plagueLocation = new Point3D(mobileTarget.X + Utility.RandomList(-1, 1), mobileTarget.Y + Utility.RandomList(-1, 1), mobileTarget.Z);
                            SpellHelper.AdjustField(ref plagueLocation, mobileTarget.Map, 12, false);

                            TimedStatic pitResidue = new TimedStatic(Utility.RandomList(0x1645, 0x122A, 0x122B, 0x122C, 0x122D, 0x122E, 0x122F), 5);
                            pitResidue.Name = "plague";
                            pitResidue.MoveToWorld(plagueLocation, mobileTarget.Map);
                        }

                        double damage = 10;
                        double damageScalar = 1.0;

                        if (mobileTarget is PlayerMobile)
                        {
                            damageScalar *= UOACZSystem.GetFatigueScalar(player);
                        }

                        if (mobileTarget is BaseCreature)
                        {
                            BaseCreature bc_Creature = mobileTarget as BaseCreature;

                            damageScalar = 2;

                            if (bc_Creature.Controlled)
                                damageScalar *= UOACZSystem.GetFatigueScalar(player);                            
                        }

                        damageScalar *= UOACZPersistance.UndeadBalanceScalar * UOACZSystem.GetUndeadAbilityScalar(player);

                        damage *= damageScalar;

                        player.DoHarmful(mobileTarget);

                        SpecialAbilities.DiseaseSpecialAbility(1.0, player, mobileTarget, damage, 60, 0x62B, true, "", "You have been inflicted with a horrific disease!");
                    });
                });
            }
        }

        #endregion
        
        #region Recruit Skeleton Ability

        public static void RecruitSkeletonAbility(PlayerMobile player)
        {
            player.SendMessage("Target location to raise a skeleton from.");
            player.Target = new RecruitSkeletonAbilityTarget();
        }

        public class RecruitSkeletonAbilityTarget : Target
        {
            private IEntity targetLocation;

            public RecruitSkeletonAbilityTarget(): base(25, true, TargetFlags.None, false)
            {
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile player = from as PlayerMobile;

                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!CanUseAbility(player, UOACZUndeadAbilityType.RecruitSkeleton, true)) return;

                IPoint3D location = target as IPoint3D;

                if (location == null)
                    return;

                Map map = player.Map;

                if (map == null)
                    return;

                SpellHelper.GetSurfaceTop(ref location);

                if (location is Mobile)
                    targetLocation = (Mobile)location;

                else
                    targetLocation = new Entity(Serial.Zero, new Point3D(location), map);

                if (Utility.GetDistance(player.Location, targetLocation.Location) > 8)
                {
                    player.SendMessage("That target is too far away.");
                    return;
                }

                if (!player.Map.InLOS(player.Location, targetLocation.Location))
                {
                    player.SendMessage("That location is not within in your line of sight.");
                    return;
                }

                double directionDelay = .25;
                double initialDelay = UOACZSystem.AbilityInitialDelaySeconds;
                double totalDelay = directionDelay + initialDelay;

                SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1, totalDelay, true, 0, false, "", "");

                player.RevealingAction();

                Point3D playerLocation = player.Location;
                Map playerMap = player.Map;

                Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (!player.IsUOACZUndead) return;

                    int animation = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimation;
                    int animationFrames = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimationFrames;

                    player.Animate(animation, animationFrames, 1, true, false, 0);
                    player.PlaySound(player.GetAngerSound());

                    Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
                    {
                        if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                        if (!player.IsUOACZUndead) return;

                        AbilitySuccessful(player, UOACZUndeadAbilityType.RecruitSkeleton);

                        UOACZSkeleton creature = new UOACZSkeleton();
                        creature.MoveToWorld(targetLocation.Location, targetLocation.Map);

                        int controlSlotsNeeded = 1;

                        if (player.Followers + controlSlotsNeeded <= player.FollowersMax)
                        {
                            creature.TimesTamed++;
                            creature.SetControlMaster(player);
                            creature.IsBonded = false;
                            creature.OwnerAbandonTime = DateTime.UtcNow + creature.AbandonDelay;

                            UOACZSystem.AddCreatureToPlayerSwarm(player, creature);
                        }

                        else
                            player.SendMessage("One or more creatures were unable to join your swarm as they would exceed your swarm control slot limit.");

                        creature.PlaySound(creature.GetIdleSound());
                    });
                });
            }
        }

        #endregion

        #region ShieldOfBones Ability

        public static void ShieldOfBonesAbility(PlayerMobile player)
        {
            double directionDelay = .25;
            double initialDelay = UOACZSystem.AbilityInitialDelaySeconds;
            double totalDelay = directionDelay + initialDelay;

            SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1, totalDelay, true, 0, false, "", "");

            player.RevealingAction();
                
            Point3D playerLocation = player.Location;
            Map playerMap = player.Map;
            
            Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
            {
                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!player.IsUOACZUndead) return;

                int animation = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimation;
                int animationFrames = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimationFrames;

                player.Animate(animation, animationFrames, 1, true, false, 0);
                player.PlaySound(player.GetAngerSound());

                Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (!player.IsUOACZUndead) return;

                    AbilitySuccessful(player, UOACZUndeadAbilityType.ShieldOfBones);

                    double duration = 15;                    

                    Timer.DelayCall(TimeSpan.FromSeconds(duration), delegate
                    {
                        if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                        if (!player.IsUOACZUndead) return;

                        player.SendMessage("Shield of Bones ability effect has expired.");
                    });

                    double amount = .25;

                    amount *= UOACZPersistance.UndeadBalanceScalar * UOACZSystem.GetUndeadAbilityScalar(player);

                    SpecialAbilities.ShieldOfBonesSpecialAbility(1.0, null, player, amount, duration, -1, true, "", "", "*shielded*");

                    Point3D location = player.Location;
                    Map map = player.Map;

                    player.PlaySound(0x65A);

                    int projectiles = 15;
                    int particleSpeed = 4;

                    for (int a = 0; a < projectiles; a++)
                    {
                        Point3D newLocation = new Point3D(player.X + Utility.RandomList(-5, -4, -3, -2, -1, 1, 2, 3, 4, 5), player.Y + Utility.RandomList(-5, -4, -3, -2, -1, 1, 2, 3, 4, 5), player.Z);
                        SpellHelper.AdjustField(ref newLocation, player.Map, 12, false);

                        IEntity effectStartLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + 10), player.Map);
                        IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(player.X, player.Y, player.Z + 10), player.Map);

                        Effects.SendMovingEffect(effectStartLocation, effectEndLocation, Utility.RandomList(6929, 6930, 6937, 6938, 6933, 6934, 6935, 6936, 6939, 6940, 6880, 6881, 6882, 6883), particleSpeed, 0, false, false, 0, 0);
                    }

                    foreach (BaseCreature creature in player.AllFollowers)
                    {
                        if (!UOACZSystem.IsUOACZValidMobile(creature)) continue;
                        if (Utility.GetDistance(player.Location, creature.Location) > 12) continue;

                        SpecialAbilities.ShieldOfBonesSpecialAbility(1.0, null, creature, .25, duration, -1, true, "", "", "*shielded*");

                        creature.PlaySound(0x65A);

                        projectiles = 10;
                        particleSpeed = 4;

                        for (int a = 0; a < projectiles; a++)
                        {
                            Point3D newLocation = new Point3D(creature.X + Utility.RandomList(-5, -4, -3, -2, -1, 1, 2, 3, 4, 5), creature.Y + Utility.RandomList(-5, -4, -3, -2, -1, 1, 2, 3, 4, 5), creature.Z);
                            SpellHelper.AdjustField(ref newLocation, creature.Map, 12, false);

                            IEntity effectStartLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + 10), creature.Map);
                            IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(creature.X, creature.Y, creature.Z + 10), creature.Map);

                            Effects.SendMovingEffect(effectStartLocation, effectEndLocation, Utility.RandomList(6929, 6930, 6937, 6938, 6933, 6934, 6935, 6936, 6939, 6940, 6880, 6881, 6882, 6883), particleSpeed, 0, false, false, 0, 0);
                        }
                    }
                });
            });
        }

        #endregion

        #region Lightning Bolt Ability

        public static void LightningBoltAbility(PlayerMobile player)
        {
            player.SendMessage("Target a creature or player to attack.");
            player.Target = new LightningBoltAbilityTarget();
        }

        public class LightningBoltAbilityTarget : Target
        {
            private IEntity targetLocation;

            public LightningBoltAbilityTarget(): base(25, false, TargetFlags.Harmful, false)
            {
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile player = from as PlayerMobile;

                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!CanUseAbility(player, UOACZUndeadAbilityType.LightningBolt, true)) return;

                Mobile mobileTarget = null;

                if (target == player)
                {
                    player.SendMessage("You cannot target yourself.");
                    return;
                }

                if (target is Mobile)
                {
                    mobileTarget = target as Mobile;

                    if (!UOACZSystem.IsUOACZValidMobile(mobileTarget))
                    {
                        player.SendMessage("That cannot be damaged.");
                        return;
                    }
                }

                else
                {
                    player.SendMessage("Perhaps this would be better off targeted at something living.");
                    return;
                }

                Map map = player.Map;

                if (Utility.GetDistance(player.Location, mobileTarget.Location) > 8)
                {
                    player.SendMessage("That target is too far away.");
                    return;
                }

                if (!player.Map.InLOS(player.Location, mobileTarget.Location))
                {
                    player.SendMessage("That is not within in your line of sight.");
                    return;
                }

                double directionDelay = .25;
                double initialDelay = UOACZSystem.AbilityInitialDelaySeconds;
                double totalDelay = directionDelay + initialDelay;

                SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1, totalDelay, true, 0, false, "", "");

                player.RevealingAction();

                Point3D playerLocation = player.Location;
                Map playerMap = player.Map;

                Direction directionToTarget = Utility.GetDirection(playerLocation, mobileTarget.Location);
                player.Direction = directionToTarget;

                Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (!player.IsUOACZUndead) return;
                    if (!UOACZSystem.IsUOACZValidMobile(mobileTarget)) return;

                    int animation = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimation;
                    int animationFrames = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimationFrames;

                    player.Animate(animation, animationFrames, 1, true, false, 0);
                    player.PlaySound(player.GetAngerSound());

                    Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
                    {
                        if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                        if (!player.IsUOACZUndead) return;
                        if (!UOACZSystem.IsUOACZValidMobile(mobileTarget)) return;

                        AbilitySuccessful(player, UOACZUndeadAbilityType.LightningBolt);

                        mobileTarget.BoltEffect(0);
                        mobileTarget.PlaySound(0x29);

                        int minDamage = 10;
                        int maxDamage = 20;

                        double damageScalar = 1.0;

                        if (mobileTarget is PlayerMobile)
                        {
                            damageScalar *= UOACZSystem.GetFatigueScalar(player);
                        }

                        if (mobileTarget is BaseCreature)
                        {
                            BaseCreature bc_Creature = mobileTarget as BaseCreature;

                            damageScalar = 4;

                            if (bc_Creature.Controlled)
                                damageScalar *= UOACZSystem.GetFatigueScalar(player);                            
                        }

                        damageScalar *= UOACZPersistance.UndeadBalanceScalar * UOACZSystem.GetUndeadAbilityScalar(player);

                        double damage = (double)Utility.RandomMinMax(minDamage, maxDamage);

                        damage = (Math.Round((double)damage * damageScalar));

                        player.DoHarmful(mobileTarget);

                        UOACZSystem.PlayerInflictDamage(player, mobileTarget, false, (int)damage);

                        new Blood().MoveToWorld(mobileTarget.Location, mobileTarget.Map);
                        AOS.Damage(mobileTarget, player, (int)damage, 0, 100, 0, 0, 0);
                    });
                });
            }
        }

        #endregion

        #region Sunder Ability

        public static void SunderAbility(PlayerMobile player)
        {
            player.SendMessage("Target a creature or player to strike.");
            player.Target = new SunderAbilityTarget();
        }

        public class SunderAbilityTarget : Target
        {
            private IEntity targetLocation;

            public SunderAbilityTarget(): base(25, false, TargetFlags.Harmful, false)
            {
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile player = from as PlayerMobile;

                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!CanUseAbility(player, UOACZUndeadAbilityType.Sunder, true)) return;

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

                BaseWeapon weapon = player.Weapon as BaseWeapon;

                Mobile mobileTarget = target as Mobile;

                if (Utility.GetDistance(player.Location , mobileTarget.Location) > 2)
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

                SpecialAbilities.HinderSpecialAbility(1.0, null, player, 0.5, 1, true, 0, false, "", "");
                                
                player.Direction = Utility.GetDirection(player.Location, mobileTarget.Location);

                Timer.DelayCall(TimeSpan.FromSeconds(.25), delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (!player.IsUOACZUndead) return;

                    if (!UOACZSystem.IsUOACZValidMobile(mobileTarget)) return;
                    if (Utility.GetDistance(player.Location, mobileTarget.Location) > 8) return;

                    weapon = player.Weapon as BaseWeapon;

                    if (weapon == null)
                        return;

                    AbilitySuccessful(player, UOACZUndeadAbilityType.Sunder);

                    player.PlaySound(0x51D);

                    bool attackHit = weapon.CheckHit(player, mobileTarget);

                    if (!attackHit)
                    {
                        if (mobileTarget is BaseCreature)
                            attackHit = true;                            
                    }

                    double damageScalar = 1.5;
                    double pierceAmount = .33;

                    double duration = 15;

                    if (mobileTarget is PlayerMobile)
                    {
                        pierceAmount *= UOACZSystem.GetFatigueScalar(player);
                    }

                    if (mobileTarget is BaseCreature)
                    {
                        damageScalar = 3;
                        pierceAmount = .66;

                        pierceAmount *= UOACZSystem.GetFatigueScalar(player);
                    }

                    damageScalar *= UOACZPersistance.UndeadBalanceScalar * UOACZSystem.GetUndeadAbilityScalar(player);
                    pierceAmount *= UOACZPersistance.UndeadBalanceScalar * UOACZSystem.GetUndeadAbilityScalar(player);
                    
                    player.DoHarmful(mobileTarget);

                    if (attackHit)
                    {
                        SpecialAbilities.PierceSpecialAbility(1.0, player, mobileTarget, pierceAmount, duration, -1, true, "", "Their attack cuts through your armor!");
                        weapon.OnHit(player, mobileTarget, damageScalar);

                        Timer.DelayCall(TimeSpan.FromSeconds(30), delegate
                        {
                            if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                            if (!player.IsUOACZUndead) return;

                            player.SendMessage("Sunder ability effect has expired.");
                        });
                    }

                    else
                        weapon.OnMiss(player, mobileTarget);
                   
                    player.NextCombatTime = DateTime.UtcNow + weapon.GetDelay(player, false);
                });
            }
        }

        #endregion

        #region Demolish Ability

        public static void DemolishAbility(PlayerMobile player)
        {
            double directionDelay = .25;
            double initialDelay = UOACZSystem.AbilityInitialDelaySeconds;
            double totalDelay = directionDelay + initialDelay;

            SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1, totalDelay, true, 0, false, "", "");

            player.RevealingAction();

            Point3D playerLocation = player.Location;
            Map playerMap = player.Map;

            Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
            {
                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!player.IsUOACZUndead) return;

                int range = 3;

                for (int a = 0; a < 20; a++)
                {
                    TimedStatic floorCrack = new TimedStatic(Utility.RandomList(6913, 6914, 6915, 6916, 6917, 6918, 6919, 6920), 5);
                    floorCrack.Name = "floor crack";

                    Point3D newLocation = new Point3D(player.Location.X + Utility.RandomMinMax(-1 * range, range), player.Location.Y + Utility.RandomMinMax(-1 * range, range), player.Location.Z);
                    SpellHelper.AdjustField(ref newLocation, player.Map, 12, false);

                    floorCrack.MoveToWorld(newLocation, player.Map);
                }

                int animation = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimation;
                int animationFrames = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimationFrames;

                player.Animate(animation, animationFrames, 1, true, false, 0);
                player.PlaySound(player.GetAngerSound());

                Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (!player.IsUOACZUndead) return;

                    AbilitySuccessful(player, UOACZUndeadAbilityType.Demolish);                    

                    player.PlaySound(0x305); 

                    Map map = player.Map;
                    Point3D location = player.Location;

                    Queue m_Queue = new Queue();

                    IPooledEnumerable nearbyItems = map.GetItemsInRange(location, range);

                    foreach (Item item in nearbyItems)
                    {
                        if (item is UOACZBreakableStatic)
                            m_Queue.Enqueue(item);
                    }

                    nearbyItems.Free();                    

                    int damageMin = player.m_UOACZAccountEntry.UndeadProfile.DamageMin * 5;
                    int damageMax = player.m_UOACZAccountEntry.UndeadProfile.DamageMax * 5;

                    while (m_Queue.Count > 0)
                    {
                        Item item = (Item)m_Queue.Dequeue();

                        UOACZBreakableStatic breakableStatic = item as UOACZBreakableStatic;

                        if (breakableStatic.DamageState == BreakableStatic.DamageStateType.Broken)
                            continue;

                        int damage = (int)((double)Utility.RandomMinMax(damageMin, damageMax) * UOACZPersistance.UndeadBalanceScalar * UOACZSystem.GetUndeadAbilityScalar(player));

                        //Prevent Player From Receiving Many Rewards
                        if (m_Queue.Count == 0)
                            breakableStatic.ReceiveDamage(player, damage, BreakableStatic.InteractionType.Weapon);

                        else
                        {
                            AchievementSystemImpl.Instance.TickProgressMulti(player, AchievementTriggers.Trigger_UOACZDamageObjects, 1);   

                            breakableStatic.ReceiveDamage(null, damage, BreakableStatic.InteractionType.Weapon);
                        }
                    }

                    m_Queue = new Queue();

                    IPooledEnumerable nearbyMobiles = player.Map.GetMobilesInRange(player.Location, range);

                    foreach (Mobile mobile in nearbyMobiles)
                    {
                        if (mobile == player) continue;
                        if (!UOACZSystem.IsUOACZValidMobile(mobile)) continue;                        
                        if (mobile is UOACZBaseUndead) continue;

                        PlayerMobile playerTarget = mobile as PlayerMobile;

                        if (playerTarget != null)
                        {
                            if (playerTarget.IsUOACZUndead)
                                continue;
                        }

                        m_Queue.Enqueue(mobile);
                    }

                    nearbyMobiles.Free();

                    while (m_Queue.Count > 0)
                    {
                        Mobile mobile = (Mobile)m_Queue.Dequeue();

                        int minDamage = 8;
                        int maxDamage = 12;

                        double damage = (double)Utility.RandomMinMax(minDamage, maxDamage);

                        double damageScalar = 1.0;

                        if (mobile is PlayerMobile)
                        {
                            damageScalar *= UOACZSystem.GetFatigueScalar(player);
                        }

                        if (mobile is BaseCreature)
                        {
                            BaseCreature bc_Creature = mobile as BaseCreature;

                            damageScalar = 3;

                            if (bc_Creature.Controlled)
                                damageScalar *= UOACZSystem.GetFatigueScalar(player);                            
                        }

                        damageScalar *= UOACZPersistance.UndeadBalanceScalar * UOACZSystem.GetUndeadAbilityScalar(player);

                        damage = (Math.Round((double)damage * damageScalar));

                        UOACZSystem.PlayerInflictDamage(player, mobile, true, (int)damage);

                        player.DoHarmful(mobile);

                        new Blood().MoveToWorld(mobile.Location, mobile.Map);
                        AOS.Damage(mobile, player, (int)damage, 100, 0, 0, 0, 0);
                    }
                });
            });
        }        

        #endregion

        #region Restless Dead Ability

        public static void RestlessDeadAbility(PlayerMobile player)
        {
            player.RevealingAction();

            int stationarySeconds = 2;

            SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1, stationarySeconds, true, 0, false, "", "");  

            Point3D location = player.Location;
            Map map = player.Map;

            int range = 5;

            List<Point3D> m_Locations = new List<Point3D>();

            IPooledEnumerable mobilesInArea = map.GetMobilesInRange(location, range);

            foreach (Mobile mobile in mobilesInArea)
            {
                if (mobile == player) continue;
                if (!UOACZSystem.IsUOACZValidMobile(mobile)) continue;
                if (mobile is UOACZBaseUndead) continue;

                PlayerMobile playerTarget = mobile as PlayerMobile;

                if (playerTarget != null)
                {
                    if (playerTarget.IsUOACZUndead)
                        continue;
                }

                if (!mobile.InLOS(player))
                    continue;

                if (!m_Locations.Contains(mobile.Location))
                    m_Locations.Add(mobile.Location);                
            }

            mobilesInArea.Free();
            
            foreach (Point3D point in m_Locations)
            {
                int bonePileCount = 3;

                for (int a = 0; a < bonePileCount; a++)
                {
                    TimedStatic bonePile = new TimedStatic(Utility.RandomList(6922, 6923, 6924, 6925, 6926, 6927, 6928, 3786, 3787, 3788, 3789, 3790, 3791, 3792, 3793, 3794), stationarySeconds);

                    bonePile.Name = "bones";
                    bonePile.MoveToWorld(point, map);
                }
            }

            Effects.PlaySound(location, map, 0x222);

            for (int a = 0; a < stationarySeconds + 1; a++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(a * 1), delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (!player.IsUOACZUndead) return;

                    int animation = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimation;
                    int animationFrames = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimationFrames;

                    player.Animate(animation, animationFrames, 1, true, false, 0);
                    player.PlaySound(player.GetIdleSound());
                });
            }

            Timer.DelayCall(TimeSpan.FromSeconds(stationarySeconds), delegate
            {
                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!player.IsUOACZUndead) return;

                AbilitySuccessful(player, UOACZUndeadAbilityType.RestlessDead);

                foreach (Point3D point in m_Locations)
                {
                    Effects.SendLocationParticles(EffectItem.Create(point, map, TimeSpan.FromSeconds(0.25)), 8700, 10, 30, 0, 0, 5029, 0);

                    IPooledEnumerable spikePoint = map.GetMobilesInRange(point, 1);
                    
                   Queue m_Queue = new Queue();

                    foreach (Mobile mobile in spikePoint)
                    {
                        if (mobile == player) continue;
                        if (!UOACZSystem.IsUOACZValidMobile(mobile)) continue;
                        if (mobile is UOACZBaseUndead) continue;

                        PlayerMobile playerTarget = mobile as PlayerMobile;

                        if (playerTarget != null)
                        {
                            if (playerTarget.IsUOACZUndead)
                                continue;
                        }

                        m_Queue.Enqueue(mobile);
                    }

                    spikePoint.Free();                    

                    Effects.PlaySound(point, map, 0x11D);
                    
                    int projectiles = 10;
                    int particleSpeed = 8;
                    double distanceDelayInterval = .12;

                    int minRadius = 1;
                    int maxRadius = 5;

                    List<Point3D> m_ValidLocations = SpecialAbilities.GetSpawnableTiles(point, true, false, point, map, projectiles, 20, minRadius, maxRadius, false);

                    if (m_ValidLocations.Count == 0)
                        return;

                    for (int a = 0; a < projectiles; a++)
                    {
                        Point3D newLocation = m_ValidLocations[Utility.RandomMinMax(0, m_ValidLocations.Count - 1)];

                        IEntity effectStartLocation = new Entity(Serial.Zero, new Point3D(point.X, point.Y, point.Z + 2), map);
                        IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + 50), map);

                        newLocation.Z += 5;

                        Effects.SendMovingEffect(effectStartLocation, effectEndLocation, Utility.RandomList(6929, 6930, 6937, 6938, 6933, 6934, 6935, 6936, 6939, 6940, 6880, 6881, 6882, 6883), particleSpeed, 0, false, false, 0, 0);
                    }

                    IEntity locationEntity = new Entity(Serial.Zero, new Point3D(point.X, point.Y, point.Z - 1), map);
                    Effects.SendLocationParticles(locationEntity, Utility.RandomList(0x36BD, 0x36BF, 0x36CB, 0x36BC), 30, 7, 2497, 0, 5044, 0);

                    int creaturesLost = 0;

                    while (m_Queue.Count > 0)
                    {
                        Mobile mobile = (Mobile)m_Queue.Dequeue();

                        int minDamage = 10;
                        int maxDamage = 20;

                        double damage = (double)Utility.RandomMinMax(minDamage, maxDamage);
                        double damageScalar = 1.0;

                        if (mobile is PlayerMobile)
                        {
                            damageScalar *= UOACZSystem.GetFatigueScalar(player);
                        }

                        if (mobile is BaseCreature)
                        {
                            BaseCreature bc_Creature = mobile as BaseCreature;

                            damageScalar = 3;

                            if (bc_Creature.Controlled)
                                damageScalar *= UOACZSystem.GetFatigueScalar(player);                            
                        }

                        damageScalar *= UOACZPersistance.UndeadBalanceScalar * UOACZSystem.GetUndeadAbilityScalar(player);

                        Point3D mobileLocation = mobile.Location;

                        damage = (Math.Round((double)damage * damageScalar));

                        player.DoHarmful(mobile);

                        UOACZSystem.PlayerInflictDamage(player, mobile, true, (int)damage);

                        new Blood().MoveToWorld(mobile.Location, mobile.Map);
                        AOS.Damage(mobile, (int)damage, 100, 0, 0, 0, 0);

                        if (mobile == null)
                            continue;

                        if (!mobile.Alive)
                        {
                            UOACZSkeleton creature = new UOACZSkeleton();                            

                            int controlSlotsNeeded = 1;

                            if (player.Followers + controlSlotsNeeded <= player.FollowersMax)
                            {
                                creature.TimesTamed++;
                                creature.SetControlMaster(player);
                                creature.IsBonded = false;
                                creature.OwnerAbandonTime = DateTime.UtcNow + creature.AbandonDelay;

                                UOACZSystem.AddCreatureToPlayerSwarm(player, creature);
                            }

                            else
                                creaturesLost++;

                            creature.MoveToWorld(mobileLocation, map);
                            creature.PlaySound(creature.GetIdleSound());
                        }
                    }

                    if (creaturesLost > 0)
                        player.SendMessage("One or more creatures were unable to join your swarm as they would exceed your swarm control slot limit.");
                }
            });
        }

        #endregion

        #region Carrion Swarm Ability

        public static void CarrionSwarmAbility(PlayerMobile player)
        {
            player.SendMessage("Target a corpse.");
            player.Target = new CarrionSwarmAbilityTarget();
        }

        public class CarrionSwarmAbilityTarget : Target
        {
            private IEntity targetLocation;

            public CarrionSwarmAbilityTarget(): base(25, true, TargetFlags.None, false)
            {
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile player = from as PlayerMobile;

                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!CanUseAbility(player, UOACZUndeadAbilityType.CarrionSwarm, true)) return;

                Corpse corpse = null;

                if (target is Corpse)
                    corpse = target as Corpse;

                if (corpse == null)
                {
                    IPoint3D location = target as IPoint3D;

                    if (location == null)
                        return;

                    Map map = player.Map;

                    if (map == null)
                        return;

                    SpellHelper.GetSurfaceTop(ref location);

                    if (location is Mobile)
                        targetLocation = (Mobile)location;

                    else
                        targetLocation = new Entity(Serial.Zero, new Point3D(location), map);

                    IPooledEnumerable nearbyItems = map.GetItemsInRange(targetLocation.Location, 1);

                    foreach (Item item in nearbyItems)
                    {
                        if (item is Corpse)
                        {
                            corpse = item as Corpse;
                            break;
                        }
                    }

                    nearbyItems.Free();
                }

                if (corpse == null)
                {
                    player.SendMessage("That is not a corpse.");
                    return;
                }

                if (Utility.GetDistance(player.Location, corpse.Location) > 8)
                {
                    player.SendMessage("That corpse is too far away.");
                    return;
                }

                if (!player.Map.InLOS(player.Location, corpse.Location))
                {
                    player.SendMessage("That corpse is not within in your line of sight.");
                    return;
                }

                double directionDelay = .25;
                double initialDelay = UOACZSystem.AbilityInitialDelaySeconds;
                double totalDelay = directionDelay + initialDelay;

                SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1, totalDelay, true, 0, false, "", "");

                player.RevealingAction();

                Point3D playerLocation = player.Location;
                Map playerMap = player.Map;

                Direction directionToTarget = Utility.GetDirection(playerLocation, corpse.Location);
                player.Direction = directionToTarget;

                Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (!player.IsUOACZUndead) return;
                    if (corpse == null) return;
                    if (corpse.Deleted) return;

                    int animation = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimation;
                    int animationFrames = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimationFrames;

                    player.Animate(animation, animationFrames, 1, true, false, 0);
                    player.PlaySound(player.GetAngerSound());

                    Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
                    {
                        if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                        if (!player.IsUOACZUndead) return;
                        if (corpse == null) return;
                        if (corpse.Deleted) return;

                        AbilitySuccessful(player, UOACZUndeadAbilityType.CarrionSwarm);

                        Effects.PlaySound(corpse.Location, corpse.Map, 0x580);

                        UOACZSystem.DamageCorpse(corpse.Location, corpse.Map, false);

                        bool unableToAddCreature = false;

                        for (int a = 0; a < 2; a++)
                        {
                            UOACZCarrionRat creature = new UOACZCarrionRat();

                            int controlSlotsNeeded = 1;

                            if (player.Followers + controlSlotsNeeded <= player.FollowersMax)
                            {
                                creature.TimesTamed++;
                                creature.SetControlMaster(player);
                                creature.IsBonded = false;
                                creature.OwnerAbandonTime = DateTime.UtcNow + creature.AbandonDelay;
                                
                                UOACZSystem.AddCreatureToPlayerSwarm(player, creature);
                            }

                            else
                                unableToAddCreature = true;

                            creature.UseMoveToWorldHandling = false;
                            creature.MoveToWorld(corpse.Location, corpse.Map);                            
                        }

                        if (unableToAddCreature)
                            player.SendMessage("One or more creatures were unable to join your swarm as they would exceed your swarm control slot limit.");
                    });
                });
            }
        }

        #endregion

        #region Consume Corpse Ability

        public static void ConsumeCorpseAbility(PlayerMobile player)
        {
            player.SendMessage("Target a nearby corpse.");
            player.Target = new ConsumeCorpseAbilityTarget();
        }

        public class ConsumeCorpseAbilityTarget : Target
        {
            private IEntity targetLocation;

            public ConsumeCorpseAbilityTarget(): base(25, true, TargetFlags.None, false)
            {
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile player = from as PlayerMobile;

                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!CanUseAbility(player, UOACZUndeadAbilityType.ConsumeCorpse, true)) return;

                Corpse corpse = null;

                if (target is Corpse)
                    corpse = target as Corpse;

                if (corpse == null)
                {
                    IPoint3D location = target as IPoint3D;

                    if (location == null)
                        return;

                    Map map = player.Map;

                    if (map == null)
                        return;

                    SpellHelper.GetSurfaceTop(ref location);

                    if (location is Mobile)
                        targetLocation = (Mobile)location;

                    else
                        targetLocation = new Entity(Serial.Zero, new Point3D(location), map);

                    IPooledEnumerable nearbyItems = map.GetItemsInRange(targetLocation.Location, 1);

                    foreach (Item item in nearbyItems)
                    {
                        if (item is Corpse)
                        {
                            corpse = item as Corpse;
                            break;
                        }
                    }

                    nearbyItems.Free();
                }

                if (corpse == null)
                {
                    player.SendMessage("That is not a corpse.");
                    return;
                }

                if (Utility.GetDistance(player.Location, corpse.Location) > 2)
                {
                    player.SendMessage("That corpse is too far away.");
                    return;
                }

                if (!player.Map.InLOS(player.Location, corpse.Location))
                {
                    player.SendMessage("That corpse is not within in your line of sight.");
                    return;
                }

                double directionDelay = .25;
                double totalDelay = directionDelay + 2;

                SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1, totalDelay, true, 0, false, "", "");

                player.RevealingAction();

                Point3D playerLocation = player.Location;
                Map playerMap = player.Map;

                Direction directionToTarget = Utility.GetDirection(playerLocation, corpse.Location);
                player.Direction = directionToTarget;

                Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (!player.IsUOACZUndead) return;
                    if (corpse == null) return;
                    if (corpse.Deleted) return;

                    AbilitySuccessful(player, UOACZUndeadAbilityType.ConsumeCorpse);

                    for (int a = 0; a < 2; a++)
                    {
                        Timer.DelayCall(TimeSpan.FromSeconds(a * 1), delegate
                        {
                            if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                            if (!player.IsUOACZUndead) return;
                            if (corpse == null) return;
                            if (corpse.Deleted) return;

                            int specialAnimation = player.m_UOACZAccountEntry.UndeadProfile.SpecialAnimation;
                            int specialAnimationFrames = player.m_UOACZAccountEntry.UndeadProfile.SpecialAnimationFrames;

                            player.Animate(specialAnimation, specialAnimationFrames, 1, true, false, 0);

                            Effects.PlaySound(corpse.Location, corpse.Map, 0x5DA);
                            player.PlaySound(player.m_UOACZAccountEntry.UndeadProfile.AngerSound);

                            UOACZSystem.DamageCorpse(corpse.Location, corpse.Map, false);

                            from.PublicOverheadMessage(MessageType.Regular, 0, false, "*feeds*");

                            double amount = .125;

                            amount *= UOACZPersistance.UndeadBalanceScalar * UOACZSystem.GetUndeadAbilityScalar(player);

                            int hitsRegained = (int)(Math.Round((double)player.HitsMax * amount));

                            player.Heal(hitsRegained);
                        });
                    }

                    Timer.DelayCall(TimeSpan.FromSeconds(1.2), delegate
                    {
                        if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                        if (!player.IsUOACZUndead) return;
                        if (corpse == null) return;
                        if (corpse.Deleted) return;

                        UOACZSystem.DamageCorpse(corpse.Location, corpse.Map, true);
                    });                   
                });
            }
        }

        #endregion        

        #region Corpse Breath Ability

        public static void CorpseBreathAbility(PlayerMobile player)
        {
            player.SendMessage("Target a player or creature.");
            player.Target = new CorpseBreathAbilityTarget();
        }

        public class CorpseBreathAbilityTarget : Target
        {
            public CorpseBreathAbilityTarget(): base(25, false, TargetFlags.Harmful, false)
            {
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile player = from as PlayerMobile;

                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!CanUseAbility(player, UOACZUndeadAbilityType.CorpseBreath, true)) return;

                Mobile mobileTarget = null;

                if (target == player)
                {
                    player.SendMessage("You cannot target yourself.");
                    return;
                }

                if (target is Mobile)
                {
                    mobileTarget = target as Mobile;

                    if (!UOACZSystem.IsUOACZValidMobile(mobileTarget))
                    {
                        player.SendMessage("That cannot be damaged.");
                        return;
                    }
                }

                else
                {
                    player.SendMessage("Perhaps this would be better off targeted at something living.");
                    return;
                }
                
                if (Utility.GetDistance(player.Location, mobileTarget.Location) > 8)
                {
                    player.SendMessage("That target is too far away.");
                    return;
                }

                if (!player.Map.InLOS(player.Location, mobileTarget.Location))
                {
                    player.SendMessage("That is not within in your line of sight.");
                    return;
                }

                double directionDelay = .25;
                double initialDelay = UOACZSystem.AbilityInitialDelaySeconds;
                double totalDelay = directionDelay + initialDelay + 1;

                SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1, totalDelay, true, 0, false, "", "");

                player.RevealingAction();

                Point3D playerLocation = player.Location;
                Map playerMap = player.Map;

                Direction directionToTarget = Utility.GetDirection(playerLocation, mobileTarget.Location);
                player.Direction = directionToTarget;

                Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (!player.IsUOACZUndead) return;
                    if (!UOACZSystem.IsUOACZValidMobile(mobileTarget)) return;
                    if (Utility.GetDistance(player.Location, mobileTarget.Location) >= 20) return;
                    if (mobileTarget.Hidden) return;

                    int animation = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimation;
                    int animationFrames = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimationFrames;

                    player.Animate(animation, animationFrames, 1, true, false, 0);
                    player.PlaySound(player.GetAngerSound());

                    Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
                    {
                        if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                        if (!player.IsUOACZUndead) return;
                        if (!UOACZSystem.IsUOACZValidMobile(mobileTarget)) return;
                        if (Utility.GetDistance(player.Location, mobileTarget.Location) >= 20) return;
                        if (mobileTarget.Hidden) return;

                        AbilitySuccessful(player, UOACZUndeadAbilityType.CorpseBreath);

                        int effectSound = 0x246;
                        int itemID = 0x3728;
                        int itemHue = 2610;

                        int corpseParts = 25;
                        double corpseScalar = 1.0;

                        Point3D location = mobileTarget.Location;
                        Map map = mobileTarget.Map;

                        IPooledEnumerable nearbyCorpses = playerMap.GetItemsInRange(playerLocation, 12);

                        Queue m_Queue = new Queue();

                        foreach (Item item in nearbyCorpses)
                        {
                            if (item is Corpse)
                            {
                                corpseParts++;
                                m_Queue.Enqueue(item);                                
                            }
                        }

                        nearbyCorpses.Free();

                        while (m_Queue.Count > 0)
                        {
                            Corpse corpse = (Corpse)m_Queue.Dequeue();
                            new Blood().MoveToWorld(corpse.Location, corpse.Map);
                        }

                        if (corpseParts > 25)
                            corpseParts = 25;

                        corpseScalar += (.033 * (double)corpseParts);

                        for (int a = 0; a < corpseParts; a++)
                        {
                            Timer.DelayCall(TimeSpan.FromSeconds(a * .05), delegate
                            {
                                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                                if (!player.IsUOACZUndead) return;

                                Effects.PlaySound(playerLocation, playerMap, 0x4F1);

                                IEntity effectStartLocation = new Entity(Serial.Zero, playerLocation, playerMap);
                                IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(location.X, location.Y, location.Z + 2), map);

                                int itemId = Utility.RandomList
                                    (
                                    7389, 7397, 7398, 7395, 7402, 7408, 7407, 7393, 7584, 7405, 7585, 7600, 7587, 7602, 7394,
                                    7404, 7391, 7396, 7399, 7403, 7406, 7586, 7599, 7588, 7601, 7392, 7392, 7583, 7597, 7390
                                    );

                                Effects.SendMovingEffect(effectStartLocation, effectEndLocation, itemId, 5, 0, false, false, 0, 0);
                            });
                        }
                        
                        int minDamage = 10;
                        int maxDamage = 20;

                        double damage = (double)Utility.RandomMinMax(minDamage, maxDamage);

                        double damageScalar = 1.0;
                        double duration = (double)(Utility.RandomMinMax(3, 5));

                        if (mobileTarget is PlayerMobile)
                        {
                            damageScalar *= UOACZSystem.GetFatigueScalar(player);
                        }

                        if (mobileTarget is BaseCreature)
                        {
                            BaseCreature bc_Creature = mobileTarget as BaseCreature;

                            damageScalar = 3;

                            if (bc_Creature.Controlled)
                                damageScalar *= UOACZSystem.GetFatigueScalar(player);
                        }

                        damageScalar *= UOACZPersistance.UndeadBalanceScalar * UOACZSystem.GetUndeadAbilityScalar(player);
                                                                        
                        damage = (Math.Round((double)damage * damageScalar * corpseScalar));
                        duration = duration * damageScalar;

                        UOACZSystem.PlayerInflictDamage(player, mobileTarget, false, (int)damage);
                        
                        player.RevealingAction();

                        player.DoHarmful(mobileTarget);

                        SpecialAbilities.EntangleSpecialAbility(1.0, player, mobileTarget, 1.0, duration, -1, true, "", "The foul stench of the dead stuns you, leaving you unable to move.");

                        new Blood().MoveToWorld(mobileTarget.Location, mobileTarget.Map);
                        AOS.Damage(mobileTarget, player, (int)damage, 0, 100, 0, 0, 0);
                    });
                });
            }
        }

        #endregion 

        #region Feeding Frenzy Ability

        public static void FeedingFrenzyAbility(PlayerMobile player)
        {
            player.SendMessage("Target a nearby corpse.");
            player.Target = new FeedingFrenzyAbilityTarget();
        }

        public class FeedingFrenzyAbilityTarget : Target
        {
            private IEntity targetLocation;

            public FeedingFrenzyAbilityTarget(): base(25, true, TargetFlags.None, false)
            {
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile player = from as PlayerMobile;

                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!CanUseAbility(player, UOACZUndeadAbilityType.FeedingFrenzy, true)) return;

                Corpse corpse = null;

                if (target is Corpse)
                    corpse = target as Corpse;

                if (corpse == null)
                {
                    IPoint3D location = target as IPoint3D;

                    if (location == null)
                        return;

                    Map map = player.Map;

                    if (map == null)
                        return;

                    SpellHelper.GetSurfaceTop(ref location);

                    if (location is Mobile)
                        targetLocation = (Mobile)location;

                    else
                        targetLocation = new Entity(Serial.Zero, new Point3D(location), map);

                    IPooledEnumerable nearbyItems = map.GetItemsInRange(targetLocation.Location, 1);

                    foreach (Item item in nearbyItems)
                    {
                        if (item is Corpse)
                        {
                            corpse = item as Corpse;
                            break;
                        }
                    }

                    nearbyItems.Free();
                }

                if (corpse == null)
                {
                    player.SendMessage("That is not a corpse.");
                    return;
                }

                if (Utility.GetDistance(player.Location, corpse.Location) > 2)
                {
                    player.SendMessage("That corpse is too far away.");
                    return;
                }

                if (!player.Map.InLOS(player.Location, corpse.Location))
                {
                    player.SendMessage("That corpse is not within in your line of sight.");
                    return;
                }
                                
                player.RevealingAction();

                SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1.0, 2.2, true, 0, false, "", "");

                for (int a = 0; a < 2; a++)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(a * 1), delegate
                    {
                        if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                        if (!player.IsUOACZUndead) return;
                        if (corpse == null) return;
                        if (corpse.Deleted) return;

                        int specialAnimation = player.m_UOACZAccountEntry.UndeadProfile.SpecialAnimation;
                        int specialAnimationFrames = player.m_UOACZAccountEntry.UndeadProfile.SpecialAnimationFrames;

                        player.Animate(specialAnimation, specialAnimationFrames, 1, true, false, 0);

                        Effects.PlaySound(corpse.Location, corpse.Map, 0x5DA);
                        player.PlaySound(player.m_UOACZAccountEntry.UndeadProfile.AngerSound);

                        UOACZSystem.DamageCorpse(corpse.Location, corpse.Map, false);

                        from.PublicOverheadMessage(MessageType.Regular, 0, false, "*feeds*");
                    });
                }

                Timer.DelayCall(TimeSpan.FromSeconds(1.2), delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (!player.IsUOACZUndead) return;
                    if (corpse == null) return;
                    if (corpse.Deleted) return;

                    UOACZSystem.DamageCorpse(corpse.Location, corpse.Map, true);

                    AbilitySuccessful(player, UOACZUndeadAbilityType.FeedingFrenzy);

                    double duration = 15;

                    double amount = .25;

                    amount *= UOACZPersistance.UndeadBalanceScalar * UOACZSystem.GetUndeadAbilityScalar(player);

                    player.PlaySound(0x580);
                    player.FixedParticles(0x373A, 10, 15, 5036, 2116, 0, EffectLayer.Head);

                    SpecialAbilities.FrenzySpecialAbility(1.0, player, null, amount, duration, 0, true, "", "Your feeding drives you into a frenzy.", "*goes into a frenzy*");
                    SpecialAbilities.EnrageSpecialAbility(1.0, null, player, amount, duration, 0, false, "", "", "-1");

                    Queue m_Queue = new Queue();

                    foreach (BaseCreature creature in player.AllFollowers)
                    {
                        if (creature == null) continue;
                        if (creature.Deleted || !creature.Alive) continue;
                        if (!UOACZSystem.IsUOACZValidMobile(creature)) continue;
                        if (!creature.Map.InLOS(creature.Location, player.Location)) continue;
                        if (Utility.GetDistance(creature.Location, player.Location) >= 15) continue;

                        m_Queue.Enqueue(creature);
                    }

                    int projectiles = 4;
                    int particleSpeed = 4;

                    while (m_Queue.Count > 0)
                    {
                        BaseCreature bc_Creature = (BaseCreature)m_Queue.Dequeue();

                        SpecialAbilities.FrenzySpecialAbility(1.0, bc_Creature, null, amount, duration, 0, true, "", "", "*frenzies*");
                        SpecialAbilities.EnrageSpecialAbility(1.0, null, bc_Creature, amount, duration, 0, false, "", "", "-1");

                        bc_Creature.PlaySound(0x580);
                        bc_Creature.FixedParticles(0x373A, 10, 15, 5036, 2116, 0, EffectLayer.Head);
                    }                                        

                    Timer.DelayCall(TimeSpan.FromSeconds(duration), delegate
                    {
                        if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                        if (!player.IsUOACZUndead) return;

                        player.SendMessage("Feeding Frenzy ability effect has expired.");
                    });
                });
            }
        }

        #endregion                

        #region Living Bomb Ability

        public static void LivingBombAbility(PlayerMobile player)
        {
            player.SendMessage("Target a swarm follower.");
            player.Target = new LivingBombAbilityTarget();
        }

        public class LivingBombAbilityTarget : Target
        {
            private IEntity targetLocation;

            public LivingBombAbilityTarget() : base(25, true, TargetFlags.None, false)
            {
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile player = from as PlayerMobile;

                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!CanUseAbility(player, UOACZUndeadAbilityType.LivingBomb, true)) return;

                UOACZBaseUndead bc_Creature = null;

                if (target is UOACZBaseUndead)
                    bc_Creature = target as UOACZBaseUndead;

                else
                {
                    player.SendMessage("That is not a swarm follower.");
                    return;
                }

                if (!bc_Creature.Alive)
                {
                    player.SendMessage("That creature is not alive.");
                    return;
                }

                if (bc_Creature.ControlMaster != player)
                {
                    player.SendMessage("That creature is not part of your swarm.");
                    return;
                }

                if (Utility.GetDistance(player.Location, bc_Creature.Location) > 10)
                {
                    player.SendMessage("That creature is too far away.");
                    return;
                }

                if (!player.Map.InLOS(player.Location, bc_Creature.Location))
                {
                    player.SendMessage("That creature is not within in your line of sight.");
                    return;
                }

                double directionDelay = .25;
                double initialDelay = UOACZSystem.AbilityInitialDelaySeconds;
                double totalDelay = directionDelay + initialDelay;

                SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1, totalDelay, true, 0, false, "", "");

                player.RevealingAction();

                Point3D playerLocation = player.Location;
                Map playerMap = player.Map;

                Direction directionToTarget = Utility.GetDirection(playerLocation, bc_Creature.Location);
                player.Direction = directionToTarget;

                Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (!player.IsUOACZUndead) return;
                    if (bc_Creature == null) return;
                    if (bc_Creature.Deleted || !bc_Creature.Alive) return;
                    if (bc_Creature.ControlMaster != player) return;

                    int animation = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimation;
                    int animationFrames = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimationFrames;

                    player.Animate(animation, animationFrames, 1, true, false, 0);
                    player.PlaySound(player.GetAngerSound());

                    Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
                    {
                        if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                        if (!player.IsUOACZUndead) return;
                        if (bc_Creature == null) return;
                        if (bc_Creature.Deleted || !bc_Creature.Alive) return;
                        if (bc_Creature.ControlMaster != player) return;

                        AbilitySuccessful(player, UOACZUndeadAbilityType.LivingBomb);

                        player.PlaySound(player.GetAngerSound());

                        bc_Creature.Hue = 2587;
                        bc_Creature.PublicOverheadMessage(MessageType.Regular, 0, false, "*begins to pulse*");

                        bc_Creature.PlaySound(0x593);
                        bc_Creature.FixedParticles(0x3735, 1, 20, 9503, 2075, 0, EffectLayer.Waist);

                        for (int a = 1; a < 5; a++)
                        {
                            int second = a;

                            Timer.DelayCall(TimeSpan.FromSeconds(second), delegate
                            {
                                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                                if (!player.IsUOACZUndead) return;
                                if (bc_Creature == null) return;
                                if (bc_Creature.Deleted || !bc_Creature.Alive) return;

                                bc_Creature.FixedParticles(0x3735, 1, 20, 9503, 2075, 0, EffectLayer.Waist);
                                bc_Creature.PlaySound(0x666);
                                bc_Creature.PublicOverheadMessage(MessageType.Regular, 0, false, (5 - second).ToString());
                            });
                        }

                        Timer.DelayCall(TimeSpan.FromSeconds(5), delegate
                        {
                            if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                            if (!player.IsUOACZUndead) return;
                            if (bc_Creature == null) return;
                            if (bc_Creature.Deleted || !bc_Creature.Alive) return;

                            Point3D location = bc_Creature.Location;
                            Map map = bc_Creature.Map;

                            bc_Creature.Kill();

                            UOACZSystem.DamageCorpse(location, map, true);

                            Effects.PlaySound(location, map, 0x309);

                            int radius = 2;

                            int minRange = radius * -1;
                            int maxRange = radius;

                            int effectHue = 2075;

                            for (int a = minRange; a < maxRange + 1; a++)
                            {
                                for (int b = minRange; b < maxRange + 1; b++)
                                {
                                    Point3D newPoint = new Point3D(location.X + a, location.Y + b, location.Z);
                                    SpellHelper.AdjustField(ref newPoint, map, 12, false);

                                    int distance = Utility.GetDistance(location, newPoint);

                                    Timer.DelayCall(TimeSpan.FromSeconds(distance * .20), delegate
                                    {
                                        if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                                        if (!player.IsUOACZUndead) return;

                                        Effects.PlaySound(newPoint, map, Utility.RandomList(0x208));
                                        Effects.SendLocationParticles(EffectItem.Create(newPoint, map, TimeSpan.FromSeconds(0.25)), 0x3709, 10, 30, effectHue, 0, 5029, 0);

                                        IPooledEnumerable nearbyMobiles = map.GetMobilesInRange(newPoint, 0);

                                        Queue m_Queue = new Queue();

                                        foreach (Mobile mobile in nearbyMobiles)
                                        {
                                            if (!UOACZSystem.IsUOACZValidMobile(mobile)) continue;                                           
                                            if (mobile == player) continue;
                                            if (!map.InLOS(location, mobile.Location)) continue;
                                            if (mobile is UOACZBaseUndead) continue;

                                            PlayerMobile playerTarget = mobile as PlayerMobile;

                                            if (playerTarget != null)
                                            {
                                                if (playerTarget.IsUOACZUndead)
                                                    continue;
                                            }

                                            m_Queue.Enqueue(mobile);
                                        }

                                        nearbyMobiles.Free();

                                        while (m_Queue.Count > 0)
                                        {
                                            Mobile mobile = (Mobile)m_Queue.Dequeue();

                                            int minDamage = 20;
                                            int maxDamage = 40;

                                            double damage = (double)Utility.RandomMinMax(minDamage, maxDamage);

                                            double damageScalar = 1.0;

                                            if (mobile is PlayerMobile)                                            
                                                damageScalar *= UOACZSystem.GetFatigueScalar(player);                                            

                                            if (mobile is BaseCreature)
                                            {
                                                BaseCreature bc_Target = mobile as BaseCreature;

                                                damageScalar = 4;

                                                if (bc_Target.Controlled)
                                                    damageScalar *= UOACZSystem.GetFatigueScalar(player);
                                            }

                                            damageScalar *= UOACZPersistance.UndeadBalanceScalar * UOACZSystem.GetUndeadAbilityScalar(player);

                                            player.DoHarmful(mobile);

                                            damage = (Math.Round((double)damage * damageScalar));

                                            UOACZSystem.PlayerInflictDamage(player, mobile, false, (int)damage);

                                            new Blood().MoveToWorld(mobile.Location, mobile.Map);
                                            AOS.Damage(mobile, from, (int)damage, 0, 100, 0, 0, 0);
                                        }
                                    });
                                }
                            }

                        });
                    });
                });
            }
        }

        #endregion

        #region Shadows Ability

        public static void ShadowsAbility(PlayerMobile player)
        {
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

            player.Hidden = true;
            player.StealthAttackReady = true;
            player.AllowedStealthSteps = 500;

            player.Warmode = false;
            player.Combatant = null;
            player.NextCombatTime = player.NextCombatTime + TimeSpan.FromSeconds(1);

            AbilitySuccessful(player, UOACZUndeadAbilityType.Shadows);

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

            UOACZSpectre creature = new UOACZSpectre();
           
            creature.UseMoveToWorldHandling = false;
            creature.MoveToWorld(player.Location, player.Map);
            
            int controlSlotsNeeded = 1;

            if (player.Followers + controlSlotsNeeded <= player.FollowersMax)
            {
                creature.TimesTamed++;
                creature.SetControlMaster(player);
                creature.IsBonded = false;
                creature.OwnerAbandonTime = DateTime.UtcNow + creature.AbandonDelay;

                UOACZSystem.AddCreatureToPlayerSwarm(player, creature);
            }

            else
                player.SendMessage("One or more creatures were unable to join your swarm as they would exceed your swarm control slot limit.");

            Timer.DelayCall(TimeSpan.FromSeconds(3), delegate
            {
                if (!UOACZSystem.IsUOACZValidMobile(creature))
                    return;

                creature.Hidden = true;
                creature.IsStealthing = true;
                creature.StealthAttackReady = true;
            });

            creature.PlaySound(creature.GetIdleSound());            

            AchievementSystemImpl.Instance.TickProgressMulti(player, AchievementTriggers.Trigger_UOACZNightwalker, 1);

            player.SendMessage("You become one with the shadows.");
        }

        #endregion
       
        #region Wail Ability

        public static void WailAbility(PlayerMobile player)
        {
            double directionDelay = .25;
            double initialDelay = UOACZSystem.AbilityInitialDelaySeconds;
            double totalDelay = directionDelay + initialDelay;

            SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1, totalDelay, true, 0, false, "", "");

            player.RevealingAction();
                
            Point3D playerLocation = player.Location;
            Map playerMap = player.Map;

            int range = 8;

            Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
            {
                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!player.IsUOACZUndead) return;

                int animation = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimation;
                int animationFrames = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimationFrames;

                player.Animate(animation, animationFrames, 1, true, false, 0);
                player.PlaySound(player.GetAngerSound());

                Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (!player.IsUOACZUndead) return;

                    AbilitySuccessful(player, UOACZUndeadAbilityType.Wail);

                    playerLocation = player.Location;
                    playerMap = player.Map;
                    
                    Queue m_Queue = new Queue();

                    IPooledEnumerable nearbyMobiles = playerMap.GetMobilesInRange(playerLocation, range);

                    foreach (Mobile mobile in nearbyMobiles)
                    {
                        if (!UOACZSystem.IsUOACZValidMobile(mobile)) continue;
                        if (mobile == player) continue;
                        if (mobile.Hidden) continue;
                        if (mobile is UOACZBaseUndead) continue;

                        PlayerMobile playerTarget = mobile as PlayerMobile;
                        {
                            if (playerTarget != null)
                            {
                                if (playerTarget.IsUOACZUndead)
                                    continue;
                            }
                        }

                        m_Queue.Enqueue(mobile);
                    }                    

                    nearbyMobiles.Free();                    

                    while (m_Queue.Count > 0)
                    {
                        Mobile mobile = (Mobile)m_Queue.Dequeue();

                        double distance = Utility.GetDistanceToSqrt(playerLocation, mobile.Location);
                        double destinationDelay = (double)distance * .08;

                        Effects.PlaySound(playerLocation, playerMap, 0x64C);

                        Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                        {
                            if (!UOACZSystem.IsUOACZValidMobile(mobile)) return;
                            if (Utility.GetDistance(playerLocation, mobile.Location) >= 20) return;
                            if (mobile.Hidden) return;
                                                        
                            Effects.PlaySound(mobile.Location, mobile.Map, 0x5C6);
                            Effects.SendLocationParticles(EffectItem.Create(mobile.Location, mobile.Map, TimeSpan.FromSeconds(0.25)), 0x3779, 10, 50, 1153, 0, 5029, 0);

                            Direction direction = Utility.GetDirection(player.Location, mobile.Location);
                            int windId = UOACZSystem.GetWindItemId(direction, true);

                            player.MovingEffect(mobile, windId, 5, 1, false, false, 1153, 0);

                            int minDamage = 8;
                            int maxDamage = 12;

                            double damage = (double)Utility.RandomMinMax(minDamage, maxDamage);

                            double damageScalar = 1.0;

                            if (mobile is PlayerMobile)
                            {
                                damageScalar *= UOACZSystem.GetFatigueScalar(player);
                            }

                            if (mobile is BaseCreature)
                            {
                                BaseCreature bc_Creature = mobile as BaseCreature;

                                damageScalar = 4;

                                if (bc_Creature.Controlled)
                                    damageScalar *= UOACZSystem.GetFatigueScalar(player);                                
                            }

                            damageScalar *= UOACZPersistance.UndeadBalanceScalar * UOACZSystem.GetUndeadAbilityScalar(player);

                            damage = (Math.Round((double)damage * damageScalar));

                            UOACZSystem.PlayerInflictDamage(player, mobile, false, (int)damage);

                            player.DoHarmful(mobile);

                            new Blood().MoveToWorld(mobile.Location, mobile.Map);
                            AOS.Damage(mobile, player, (int)damage, 0, 100, 0, 0, 0);
                        });
                    }
                });
            });
        }

        #endregion        

        #region Ghostly Strike Ability

        public static void GhostlyStrikeAbility(PlayerMobile player)
        {
            player.SendMessage("Target a player or creature to attack.");
            player.Target = new GhostlyStrikeAbilityTarget();
        }

        public class GhostlyStrikeAbilityTarget : Target
        {
            public GhostlyStrikeAbilityTarget(): base(25, false, TargetFlags.Harmful, false)
            {
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile player = from as PlayerMobile;

                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!CanUseAbility(player, UOACZUndeadAbilityType.GhostlyStrike, true)) return;

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

                BaseWeapon weapon = player.Weapon as BaseWeapon;

                if (weapon == null)
                    return;

                Mobile mobileTarget = target as Mobile;

                if (!player.InRange(mobileTarget, weapon.MaxRange) && !player.RangeExemption(mobileTarget))
                {
                    player.SendMessage("That target is out of attack range.");
                    return;
                }

                if (!player.Map.InLOS(player.Location, mobileTarget.Location))
                {
                    player.SendMessage("That is not within in your line of sight.");
                    return;
                }

                bool wasHidden = player.Hidden;

                SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1.0, 1, true, 0, false, "", "");

                player.Direction = Utility.GetDirection(player.Location, mobileTarget.Location);

                Timer.DelayCall(TimeSpan.FromSeconds(.25), delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (!player.IsUOACZUndead) return;

                    if (!UOACZSystem.IsUOACZValidMobile(mobileTarget)) return;
                    if (Utility.GetDistance(player.Location, mobileTarget.Location) > 8) return;

                    weapon = player.Weapon as BaseWeapon;

                    if (weapon == null)
                        return;

                    AbilitySuccessful(player, UOACZUndeadAbilityType.GhostlyStrike);
                    
                    player.PlaySound(0x51D);

                    bool attackHit = weapon.CheckHit(player, mobileTarget);

                    if (!attackHit)
                    {
                        if (mobileTarget is BaseCreature)
                            attackHit = true;
                    }

                    double damageScalar = 1.33;

                    if (mobileTarget is PlayerMobile)
                    {
                        damageScalar *= UOACZSystem.GetFatigueScalar(player);

                        if (wasHidden)
                            damageScalar *= 1.33;
                    }

                    if (mobileTarget is BaseCreature)
                    {
                        BaseCreature bc_Creature = mobileTarget as BaseCreature;

                        damageScalar = 2;

                        if (bc_Creature.Controlled)
                            damageScalar *= UOACZSystem.GetFatigueScalar(player);                        

                        if (wasHidden)
                            damageScalar *= 2;
                    }

                    damageScalar *= UOACZPersistance.UndeadBalanceScalar * UOACZSystem.GetUndeadAbilityScalar(player);

                    player.RevealingAction();

                    if (attackHit)
                        weapon.OnHit(player, mobileTarget, damageScalar);

                    else
                        weapon.OnMiss(player, mobileTarget);

                    player.DoHarmful(mobileTarget);
                    player.NextCombatTime = DateTime.UtcNow + weapon.GetDelay(player, false);         
                });
            }
        }

        #endregion

        #region Dematerialize Ability

        public static void DematerializeAbility(PlayerMobile player)
        {
            player.SendMessage("Target a gate door.");
            player.Target = new DematerializeAbilityTarget();
        }

        public class DematerializeAbilityTarget : Target
        {
            private IEntity targetLocation;

            public DematerializeAbilityTarget(): base(25, false, TargetFlags.None, false)
            {
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile player = from as PlayerMobile;

                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!CanUseAbility(player, UOACZUndeadAbilityType.Dematerialize, true)) return;

                UOACZBreakableDoor door = null;

                if (target is UOACZBreakableDoor)
                    door = target as UOACZBreakableDoor;

                if (door == null)
                {
                    IPoint3D location = target as IPoint3D;

                    if (location == null)
                        return;

                    Map map = player.Map;

                    if (map == null)
                        return;

                    SpellHelper.GetSurfaceTop(ref location);

                    if (location is Mobile)
                        targetLocation = (Mobile)location;

                    else
                        targetLocation = new Entity(Serial.Zero, new Point3D(location), map);

                    IPooledEnumerable nearbyItems = map.GetItemsInRange(targetLocation.Location, 0);

                    foreach (Item item in nearbyItems)
                    {
                        if (item is UOACZBreakableDoor)
                        {
                            door = item as UOACZBreakableDoor;
                            break;
                        }
                    }

                    nearbyItems.Free();
                }

                if (door == null)
                {
                    player.SendMessage("That is not a gate door.");
                    return;
                }

                if (Utility.GetDistance(player.Location, door.Location) > 8)
                {
                    player.SendMessage("That gate door is too far away.");
                    return;
                }

                if (!player.Map.InLOS(player.Location, door.Location))
                {
                    player.SendMessage("That gate door is not within in your line of sight.");
                    return;
                }

                if (door.DamageState == BreakableStatic.DamageStateType.Broken)
                {
                    player.SendMessage("That gate door is already broken and may be freely entered.");
                    return;
                }

                Direction directionToDoor = Utility.GetDirection(door.Location, player.Location);
                Direction exitDirection = Server.Direction.Down;

                if (door.DoorFacing == UOACZBreakableDoor.DoorFacingType.EastWest)
                {
                    switch (directionToDoor)
                    {
                        case Direction.Up: exitDirection = Direction.South; break;
                        case Direction.North: exitDirection = Direction.South; break;
                        case Direction.Right: exitDirection = Direction.South; break;

                        case Direction.East: exitDirection = Direction.South; break;

                        case Direction.Left: exitDirection = Direction.North; break;
                        case Direction.South: exitDirection = Direction.North; break;
                        case Direction.Down: exitDirection = Direction.North; break;

                        case Direction.West: exitDirection = Direction.North; break;
                    }
                }

                else
                {

                    switch (directionToDoor)
                    {
                        case Direction.Left: exitDirection = Direction.East; break;
                        case Direction.West: exitDirection = Direction.East; break;
                        case Direction.Up: exitDirection = Direction.East; break;

                        case Direction.North: exitDirection = Direction.East; break;

                        case Direction.Right: exitDirection = Direction.West; break;
                        case Direction.East: exitDirection = Direction.West; break;
                        case Direction.Down: exitDirection = Direction.West; break;

                        case Direction.South: exitDirection = Direction.West; break;
                    }
                }

                Point3D newLocation = SpecialAbilities.GetPointByDirection(door.Location, exitDirection);

                Effects.PlaySound(newLocation, player.Map, 0x0FB);

                player.Location = newLocation;

                foreach (Mobile follower in player.AllFollowers)
                {
                    if (UOACZSystem.IsUOACZValidMobile(follower))
                        follower.Location = newLocation;
                }

                AbilitySuccessful(player, UOACZUndeadAbilityType.Dematerialize);
            }
        }

        #endregion

        #region Malediction Ability

        public static void MaledictionAbility(PlayerMobile player)
        {
            double directionDelay = .25;
            double initialDelay = UOACZSystem.AbilityInitialDelaySeconds;
            double totalDelay = directionDelay + initialDelay;

            SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1, totalDelay, true, 0, false, "", "");

            player.RevealingAction();

            int radius = 8;

            Point3D playerLocation = player.Location;
            Map playerMap = player.Map;

            Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
            {
                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!player.IsUOACZUndead) return;

                int animation = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimation;
                int animationFrames = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimationFrames;

                player.Animate(animation, animationFrames, 1, true, false, 0);
                player.PlaySound(player.GetAngerSound());

                Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (!player.IsUOACZUndead) return;

                    AbilitySuccessful(player, UOACZUndeadAbilityType.Malediction);

                    Effects.PlaySound(playerLocation, playerMap, 0x58F);

                    Queue m_Queue = new Queue();

                    IPooledEnumerable nearbyMobiles = playerMap.GetMobilesInRange(playerLocation, radius);

                    foreach (Mobile mobile in nearbyMobiles)
                    {
                        if (!UOACZSystem.IsUOACZValidMobile(mobile)) continue;
                        if (mobile == player) continue;
                        if (!playerMap.InLOS(playerLocation, mobile.Location)) continue;
                        if (mobile is UOACZBaseUndead) continue;

                        PlayerMobile playerTarget = mobile as PlayerMobile;
                        {
                            if (playerTarget != null)
                            {
                                if (playerTarget.IsUOACZUndead)
                                    continue;
                            }
                        }

                        m_Queue.Enqueue(mobile);
                    }

                    nearbyMobiles.Free();

                    while (m_Queue.Count > 0)
                    {
                        Mobile mobile = (Mobile)m_Queue.Dequeue();

                        mobile.PlaySound(0x0F5);                        

                        player.DoHarmful(mobile);

                        double slowdownAmount = .20;

                        double damageScalar = 1.0;

                        if (mobile is PlayerMobile)
                        {
                            damageScalar *= UOACZSystem.GetFatigueScalar(player);
                        }

                        if (mobile is BaseCreature)
                        {
                            BaseCreature bc_Creature = mobile as BaseCreature;

                            damageScalar = 2;

                            if (bc_Creature.Controlled)
                                damageScalar *= UOACZSystem.GetFatigueScalar(player);
                        }

                        damageScalar *= UOACZPersistance.UndeadBalanceScalar * UOACZSystem.GetUndeadAbilityScalar(player);

                        slowdownAmount *= damageScalar;

                        double duration = 15;

                        Effects.PlaySound(player.Location, player.Map, 0x593);
                        Effects.SendLocationParticles(EffectItem.Create(mobile.Location, mobile.Map, TimeSpan.FromSeconds(0.5)), 0x3728, 10, 30, 2610, 0, 5029, 0);

                        SpecialAbilities.CrippleSpecialAbility(1.0, player, mobile, slowdownAmount, duration, 0, false, "", "You have been struck by a malediction and your attacks have been slowed!"); 
                    }
                });
            });
        }

        #endregion

        #region Deathbolt Ability

        public static void DeathboltAbility(PlayerMobile player)
        {
            player.SendMessage("Target a creature or player to strike.");
            player.Target = new DeathboltAbilityTarget();
        }

        public class DeathboltAbilityTarget : Target
        {
            private IEntity targetLocation;

            public DeathboltAbilityTarget(): base(25, false, TargetFlags.Harmful, false)
            {
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile player = from as PlayerMobile;

                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!CanUseAbility(player, UOACZUndeadAbilityType.Deathbolt, true)) return;

                Mobile mobileTarget = null;

                if (target == player)
                {
                    player.SendMessage("You cannot target yourself.");
                    return;
                }

                if (target is Mobile)
                {
                    mobileTarget = target as Mobile;

                    if (!UOACZSystem.IsUOACZValidMobile(mobileTarget))
                    {
                        player.SendMessage("That cannot be damaged.");
                        return;
                    }
                }

                else
                {
                    player.SendMessage("Perhaps this would be better off targeted at something living. Or undead.");
                    return;
                }

                Map map = player.Map;

                if (Utility.GetDistance(player.Location, mobileTarget.Location) > 8)
                {
                    player.SendMessage("That target is too far away.");
                    return;
                }

                if (!player.Map.InLOS(player.Location, mobileTarget.Location))
                {
                    player.SendMessage("That is not within in your line of sight.");
                    return;
                }
            
                double directionDelay = .25;
                double initialDelay = UOACZSystem.AbilityInitialDelaySeconds;
                double totalDelay = directionDelay + initialDelay;

                SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1, totalDelay, true, 0, false, "", "");

                player.RevealingAction();
                
                Point3D playerLocation = player.Location;
                Map playerMap = player.Map;

                Direction directionToTarget = Utility.GetDirection(playerLocation, mobileTarget.Location);
                player.Direction = directionToTarget;

                Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (!player.IsUOACZUndead) return;
                    if (!UOACZSystem.IsUOACZValidMobile(mobileTarget)) return;
                    if (Utility.GetDistance(player.Location, mobileTarget.Location) >= 20) return;
                    if (mobileTarget.Hidden) return;

                    int animation = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimation;
                    int animationFrames = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimationFrames;

                    player.Animate(animation, animationFrames, 1, true, false, 0);
                    player.PlaySound(player.GetAngerSound());

                    Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
                    {
                        if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                        if (!player.IsUOACZUndead) return;
                        if (!UOACZSystem.IsUOACZValidMobile(mobileTarget)) return;
                        if (Utility.GetDistance(player.Location, mobileTarget.Location) >= 20) return;
                        if (mobileTarget.Hidden) return;

                        AbilitySuccessful(player, UOACZUndeadAbilityType.Deathbolt);

                        int effectSound = 0x246;
                        int itemID = 8707;
                        int itemHue = 2612;

                        Effects.PlaySound(player.Location, player.Map, effectSound);

                        IEntity startLocation = new Entity(Serial.Zero, new Point3D(player.Location.X, player.Location.Y, player.Location.Z + 8), player.Map);
                        IEntity endLocation = new Entity(Serial.Zero, new Point3D(mobileTarget.Location.X, mobileTarget.Location.Y, mobileTarget.Location.Z + 8), mobileTarget.Map);

                        Effects.SendMovingEffect(startLocation, endLocation, itemID, 5, 0, false, false, itemHue, 0);
                        
                        double distance = player.GetDistanceToSqrt(endLocation.Location);
                        double destinationDelay = (double)distance * .08;

                        Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                        {
                            if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                            if (!player.IsUOACZUndead) return;
                            if (!UOACZSystem.IsUOACZValidMobile(mobileTarget)) return;
                            if (Utility.GetDistance(player.Location, mobileTarget.Location) >= 20) return;
                            if (mobileTarget.Hidden) return;

                             int hitSound = 0x653;

                            Effects.PlaySound(mobileTarget.Location, mobileTarget.Map, hitSound);

                            Effects.SendLocationParticles(EffectItem.Create(mobileTarget.Location, map, TimeSpan.FromSeconds(0.25)), 0x3709, 10, 20, 2613, 0, 5029, 0);
                            Effects.SendLocationParticles(EffectItem.Create(mobileTarget.Location, map, TimeSpan.FromSeconds(0.25)), 0x3779, 10, 60, 2613, 0, 5029, 0);
                            
                            int minDamage = 10;
                            int maxDamage = 15;

                            double damage = (double)Utility.RandomMinMax(minDamage, maxDamage);

                            bool willKill = false;
                            bool livingTarget = false;

                            double damageScalar = 1.0;

                            if (mobileTarget is PlayerMobile)
                            {
                                damageScalar *= UOACZSystem.GetFatigueScalar(player);
                            }

                            if (mobileTarget is BaseCreature)
                            {
                                BaseCreature bc_Creature = mobileTarget as BaseCreature;

                                damageScalar = 4;

                                if (bc_Creature.Controlled)
                                    damageScalar *= UOACZSystem.GetFatigueScalar(player);
                            }

                            damageScalar *= UOACZPersistance.UndeadBalanceScalar * UOACZSystem.GetUndeadAbilityScalar(player);
                            
                            if (mobileTarget is UOACZBaseWildlife)
                                livingTarget = true;

                            if (mobileTarget is UOACZBaseHuman)
                                livingTarget = true;

                            if (mobileTarget is PlayerMobile)
                            {
                                PlayerMobile playerTarget = mobileTarget as PlayerMobile;

                                if (playerTarget.IsUOACZHuman)
                                    livingTarget = true;
                            }

                            damage = (Math.Round((double)damage * damageScalar));
                            
                            if (damage > mobileTarget.Hits)
                                willKill = true;

                            Point3D mobileLocation = mobileTarget.Location;
                            Map mobileMap = mobileTarget.Map;

                            player.RevealingAction();

                            player.DoHarmful(mobileTarget);

                            UOACZSystem.PlayerInflictDamage(player, mobileTarget, false, (int)damage);

                            new Blood().MoveToWorld(mobileTarget.Location, mobileTarget.Map);
                            AOS.Damage(mobileTarget, player, (int)damage, 0, 100, 0, 0, 0);

                            if (willKill && livingTarget)
                            {
                                Timer.DelayCall(TimeSpan.FromSeconds(1), delegate
                                {
                                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                                    if (!player.IsUOACZUndead) return;

                                    int threatLevel = UOACZPersistance.m_ThreatLevel - 90;
                                    UOACZBaseUndead creature = (UOACZBaseUndead)Activator.CreateInstance(UOACZBaseUndead.GetRandomUndeadType(0, threatLevel));

                                    if (creature != null)
                                    {
                                        int controlSlotsNeeded = 1;

                                        if (player.Followers + controlSlotsNeeded <= player.FollowersMax)
                                        {
                                            creature.TimesTamed++;
                                            creature.SetControlMaster(player);
                                            creature.IsBonded = false;
                                            creature.OwnerAbandonTime = DateTime.UtcNow + creature.AbandonDelay;

                                            UOACZSystem.AddCreatureToPlayerSwarm(player, creature);
                                        }

                                        else
                                            player.SendMessage("One or more creatures were unable to join your swarm as they would exceed your swarm control slot limit.");

                                        creature.MoveToWorld(mobileLocation, mobileMap);
                                        creature.PlaySound(creature.GetIdleSound());
                                    }
                                });
                            }
                        });
                    });
                });
            }
        }

        #endregion

        #region BatForm Ability

        public static void BatFormAbility(PlayerMobile player)
        {
            player.SendMessage("Target the location you wish to teleport to.");
            player.Target = new BatFormAbilityTarget();
        }

        public class BatFormAbilityTarget : Target
        {
            public BatFormAbilityTarget(): base(25, true, TargetFlags.None, false)
            {
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile player = from as PlayerMobile;

                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!CanUseAbility(player, UOACZUndeadAbilityType.BatForm, true)) return;

                IPoint3D location = target as IPoint3D;

                if (location == null)
                    return;

                Map map = player.Map;

                if (map == null)
                    return;

                SpellHelper.GetSurfaceTop(ref location);

                IEntity targetLocation = new Entity(Serial.Zero, new Point3D(location), map);

                if (Utility.GetDistance(player.Location, targetLocation.Location) > 12)
                {
                    player.SendMessage("That location is too far away.");
                    return;
                }

                if (!player.Map.InLOS(player.Location, targetLocation.Location))
                {
                    player.SendMessage("That is not within in your line of sight.");
                    return;
                }

                if (!player.Map.CanSpawnMobile(targetLocation.Location))
                {
                    player.SendMessage("That is not a valid location to teleport to.");
                    return;
                }

                player.PlaySound(0x657);

                Point3D oldLocation = player.Location;
                Map oldMap = player.Map;       

                AbilitySuccessful(player, UOACZUndeadAbilityType.BatForm);

                player.Location = targetLocation.Location;

                Point3D effectStep = oldLocation;
                Point3D newLocation = player.Location;

                int distance = Utility.GetDistance(oldLocation, newLocation);                

                for (int a = 0; a < distance; a++)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(a * .05), delegate
                    {
                        Direction direction = Utility.GetDirection(effectStep, newLocation);
                        effectStep = SpecialAbilities.GetPointByDirection(effectStep, direction);

                        Effects.SendLocationParticles(EffectItem.Create(effectStep, oldMap, EffectItem.DefaultDuration), 0x3728, 10, 10, 0, 0, 2023, 0);
                        Effects.PlaySound(effectStep, oldMap, 0x5C6);
                    });
                }

                UOACZGiantBat creature = new UOACZGiantBat();

                int controlSlotsNeeded = 1;

                if (player.Followers + controlSlotsNeeded <= player.FollowersMax)
                {
                    creature.TimesTamed++;
                    creature.SetControlMaster(player);
                    creature.IsBonded = false;
                    creature.OwnerAbandonTime = DateTime.UtcNow + creature.AbandonDelay;

                    UOACZSystem.AddCreatureToPlayerSwarm(player, creature);
                }

                else
                    player.SendMessage("One or more creatures were unable to join your swarm as they would exceed your swarm control slot limit.");

                creature.UseMoveToWorldHandling = false;
                creature.MoveToWorld(oldLocation, oldMap);
                creature.PlaySound(creature.GetIdleSound());

                creature.Hidden = false;
                creature.Frozen = false;
            }
        }

        #endregion

        #region Transfix Ability

        public static void TransfixAbility(PlayerMobile player)
        {
            player.SendMessage("Target a player or creature to attack.");
            player.Target = new TransfixAbilityTarget();
        }

        public class TransfixAbilityTarget : Target
        {
            public TransfixAbilityTarget(): base(25, false, TargetFlags.Harmful, false)
            {
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile player = from as PlayerMobile;

                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!CanUseAbility(player, UOACZUndeadAbilityType.Transfix, true)) return;

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

                BaseWeapon weapon = player.Weapon as BaseWeapon;

                if (weapon == null)
                    return;

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
                    if (!player.IsUOACZUndead) return;

                    if (!UOACZSystem.IsUOACZValidMobile(mobileTarget)) return;
                    if (Utility.GetDistance(player.Location, mobileTarget.Location) > 10) return;

                    AbilitySuccessful(player, UOACZUndeadAbilityType.Transfix);

                    weapon = player.Weapon as BaseWeapon;

                    if (weapon == null)
                    return;

                    player.PlaySound(0x51D);

                    bool attackHit = weapon.CheckHit(player, mobileTarget);

                    if (!attackHit)
                    {
                        if (mobileTarget is BaseCreature)
                            attackHit = true;
                    }

                    bool livingTarget = false;

                    double damageScalar = 1.25;
                    double transfixDuration = Utility.RandomMinMax(2, 4);
                    
                    if (mobileTarget is PlayerMobile)
                    {
                        transfixDuration *= UOACZSystem.GetFatigueScalar(player);
                    }

                    if (mobileTarget is BaseCreature)
                    {
                        BaseCreature bc_Creature = mobileTarget as BaseCreature;

                        damageScalar = 2;                       

                        if (bc_Creature.Controlled)
                        {
                            transfixDuration *= UOACZSystem.GetFatigueScalar(player);
                        }
                    }

                    damageScalar *= UOACZPersistance.UndeadBalanceScalar * UOACZSystem.GetUndeadAbilityScalar(player);

                    Point3D mobileLocation = mobileTarget.Location;
                    Map mobileMap = mobileTarget.Map;

                    player.DoHarmful(mobileTarget);

                    if (attackHit)
                    {
                        SpecialAbilities.PetrifySpecialAbility(1.0, player, mobileTarget, 1.0, transfixDuration, -1, true, "", "You are transfixed by their gaze!"); 

                        weapon.OnHit(player, mobileTarget, damageScalar);
                    }

                    else
                        weapon.OnMiss(player, mobileTarget);
                    
                    player.NextCombatTime = DateTime.UtcNow + weapon.GetDelay(player, false);                    
                });
            }
        }

        #endregion

        #region Unlife Ability

        public static void UnlifeAbility(PlayerMobile player)
        {
            double directionDelay = .25;
            double initialDelay = UOACZSystem.AbilityInitialDelaySeconds;
            double totalDelay = directionDelay + initialDelay;

            SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1, totalDelay, true, 0, false, "", "");

            player.RevealingAction();
                
            Point3D playerLocation = player.Location;
            Map playerMap = player.Map;

            int range = 8;

            Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
            {
                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!player.IsUOACZUndead) return;

                int animation = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimation;
                int animationFrames = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimationFrames;

                player.Animate(animation, animationFrames, 1, true, false, 0);
                player.PlaySound(player.GetAngerSound());

                Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (!player.IsUOACZUndead) return;

                    AbilitySuccessful(player, UOACZUndeadAbilityType.Unlife);

                    Queue m_Queue = new Queue();

                    IPooledEnumerable nearbyMobiles = player.Map.GetMobilesInRange(player.Location, range);

                    foreach (Mobile mobile in nearbyMobiles)
                    {
                        if (!UOACZSystem.IsUOACZValidMobile(mobile)) continue;
                        if (mobile == player) continue;                        
                        if (mobile is UOACZBaseWildlife) continue;
                        if (mobile is UOACZBaseHuman) continue;
                        if (mobile.Hidden) continue;
                        if (!playerMap.InLOS(player.Location, mobile.Location)) continue;

                        PlayerMobile playerTarget = mobile as PlayerMobile;

                        if (playerTarget != null)
                        {
                            if (playerTarget.IsUOACZHuman)
                                continue;
                        }

                        m_Queue.Enqueue(mobile);
                    }

                    nearbyMobiles.Free();

                    while (m_Queue.Count > 0)
                    {
                        Mobile mobileTarget = (Mobile)m_Queue.Dequeue();

                        if (!UOACZSystem.IsUOACZValidMobile(mobileTarget)) return;
                        if (Utility.GetDistance(player.Location, mobileTarget.Location) >= 20) return;

                        double healingAmount = 15;

                        if (mobileTarget is BaseCreature)
                            healingAmount *= 3;

                        healingAmount *= UOACZPersistance.UndeadBalanceScalar * UOACZSystem.GetUndeadAbilityScalar(player);

                        int finalAmount = (int)(Math.Round(healingAmount));

                        mobileTarget.Heal(finalAmount);

                        mobileTarget.FixedParticles(0x376A, 9, 32, 5030, 0, 0, EffectLayer.Waist);
                        mobileTarget.PlaySound(0x202);
                    }
                });
            });
        }        

        #endregion

        #region Embrace Ability

        public static void EmbraceAbility(PlayerMobile player)
        {
            player.SendMessage("Target a player or creature to attack.");
            player.Target = new EmbraceAbilityTarget();
        }

        public class EmbraceAbilityTarget : Target
        {
            public EmbraceAbilityTarget() : base(25, false, TargetFlags.Harmful, false)
            {
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile player = from as PlayerMobile;

                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!CanUseAbility(player, UOACZUndeadAbilityType.Embrace, true)) return;

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

                BaseWeapon weapon = player.Weapon as BaseWeapon;

                if (weapon == null)
                    return;

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
                    if (!player.IsUOACZUndead) return;

                    if (!UOACZSystem.IsUOACZValidMobile(mobileTarget)) return;
                    if (Utility.GetDistance(player.Location, mobileTarget.Location) > 8) return;

                    weapon = player.Weapon as BaseWeapon;

                    if (weapon == null)
                        return;

                    AbilitySuccessful(player, UOACZUndeadAbilityType.Embrace);
                   
                    player.PlaySound(0x510);

                    bool attackHit = weapon.CheckHit(player, mobileTarget);

                    if (!attackHit)
                    {
                        if (mobileTarget is BaseCreature)
                            attackHit = true;
                    }

                    bool livingTarget = false;

                    double damageScalar = 1.25;

                    if (mobileTarget is PlayerMobile)
                    {
                        damageScalar *= UOACZSystem.GetFatigueScalar(player);
                    }

                    if (mobileTarget is BaseCreature)
                    {
                        BaseCreature bc_Creature = mobileTarget as BaseCreature;

                        damageScalar = 2.5;

                        if (bc_Creature.Controlled)
                            damageScalar *= UOACZSystem.GetFatigueScalar(player);
                    }

                    damageScalar *= UOACZPersistance.UndeadBalanceScalar * UOACZSystem.GetUndeadAbilityScalar(player);

                    if (mobileTarget is UOACZBaseWildlife)
                        livingTarget = true;

                    if (mobileTarget is UOACZBaseHuman)
                        livingTarget = true;

                    if (mobileTarget is PlayerMobile)
                    {
                        PlayerMobile playerTarget = mobileTarget as PlayerMobile;

                        if (playerTarget.IsUOACZHuman)
                            livingTarget = true;
                    }

                    player.RevealingAction();

                    Point3D mobileLocation = mobileTarget.Location;
                    Map mobileMap = mobileTarget.Map;

                    if (attackHit)
                        weapon.OnHit(player, mobileTarget, damageScalar);

                    else
                        weapon.OnMiss(player, mobileTarget);

                    player.DoHarmful(mobileTarget);
                    player.NextCombatTime = DateTime.UtcNow + weapon.GetDelay(player, false);                                
                        
                    Timer.DelayCall(TimeSpan.FromSeconds(.25), delegate
                    {
                        if (!livingTarget) 
                            return;

                        bool targetKilled = false;

                        if (mobileTarget == null)
                            targetKilled = true;

                        else if (mobileTarget.Deleted || !mobileTarget.Alive)
                            targetKilled = true;

                        if (!targetKilled)
                            return;

                        if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                        if (!player.IsUOACZUndead) return;

                        UOACZSkeletalKnight creature = new UOACZSkeletalKnight();

                        int controlSlotsNeeded = creature.ControlSlots;

                        if (player.Followers + controlSlotsNeeded <= player.FollowersMax)
                        {
                            creature.TimesTamed++;
                            creature.SetControlMaster(player);
                            creature.IsBonded = false;
                            creature.OwnerAbandonTime = DateTime.UtcNow + creature.AbandonDelay;

                            UOACZSystem.AddCreatureToPlayerSwarm(player, creature);
                        }

                        else
                            player.SendMessage("One or more creatures were unable to join your swarm as they would exceed your swarm control slot limit.");

                        creature.MoveToWorld(mobileLocation, mobileMap);
                        creature.PlaySound(creature.GetIdleSound()); 
                    });                    
                });
            }
        }

        #endregion

        #region Energy Bolt Ability

        public static void EnergyBoltAbility(PlayerMobile player)
        {
            player.SendMessage("Target a creature or player to attack.");
            player.Target = new EnergyBoltAbilityTarget();
        }

        public class EnergyBoltAbilityTarget : Target
        {
            private IEntity targetLocation;

            public EnergyBoltAbilityTarget(): base(25, false, TargetFlags.Harmful, false)
            {
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile player = from as PlayerMobile;

                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!CanUseAbility(player, UOACZUndeadAbilityType.EnergyBolt, true)) return;

                Mobile mobileTarget = null;

                if (target == player)
                {
                    player.SendMessage("You cannot target yourself.");
                    return;
                }

                if (target is Mobile)
                {
                    mobileTarget = target as Mobile;

                    if (!UOACZSystem.IsUOACZValidMobile(mobileTarget))
                    {
                        player.SendMessage("That cannot be damaged.");
                        return;
                    }

                    if (mobileTarget is UOACZBaseUndead)
                    {
                        player.SendMessage("They are immune to that effect.");
                        return;
                    }

                    PlayerMobile playerTarget = mobileTarget as PlayerMobile;

                    if (playerTarget != null)
                    {
                        if (playerTarget.IsUOACZUndead)
                        {
                            player.SendMessage("They are immune to that effect.");
                            return;
                        }
                    }
                }

                else
                {
                    player.SendMessage("Perhaps this would be better off targeted at something living.");
                    return;
                }

                Map map = player.Map;

                if (Utility.GetDistance(player.Location, mobileTarget.Location) > 8)
                {
                    player.SendMessage("That target is too far away.");
                    return;
                }

                if (!player.Map.InLOS(player.Location, mobileTarget.Location))
                {
                    player.SendMessage("That is not within in your line of sight.");
                    return;
                }

                double directionDelay = .25;
                double initialDelay = UOACZSystem.AbilityInitialDelaySeconds;
                double totalDelay = directionDelay + initialDelay;

                SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1, totalDelay, true, 0, false, "", "");

                player.RevealingAction();

                Point3D playerLocation = player.Location;
                Map playerMap = player.Map;

                Direction directionToTarget = Utility.GetDirection(playerLocation, mobileTarget.Location);
                player.Direction = directionToTarget;

                Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (!player.IsUOACZUndead) return;
                    if (!UOACZSystem.IsUOACZValidMobile(mobileTarget)) return;
                    if (Utility.GetDistance(player.Location, mobileTarget.Location) >= 20) return;
                    if (mobileTarget.Hidden) return;

                    int animation = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimation;
                    int animationFrames = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimationFrames;

                    player.Animate(animation, animationFrames, 1, true, false, 0);
                    player.PlaySound(player.GetAngerSound());

                    Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
                    {
                        if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                        if (!player.IsUOACZUndead) return;
                        if (!UOACZSystem.IsUOACZValidMobile(mobileTarget)) return;
                        if (Utility.GetDistance(player.Location, mobileTarget.Location) >= 20) return;
                        if (mobileTarget.Hidden) return;

                        AbilitySuccessful(player, UOACZUndeadAbilityType.EnergyBolt);

                        player.MovingParticles(mobileTarget, 0x379F, 5, 0, false, true, 0, 0, 3043, 4043, 0x211, 0);
                        player.PlaySound(0x20A);

                        player.DoHarmful(mobileTarget);

                        double distance = Utility.GetDistanceToSqrt(playerLocation, mobileTarget.Location);
                        double destinationDelay = (double)distance * .08;

                        Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                        {
                            int minDamage = 15;
                            int maxDamage = 25;

                            double damageScalar = 1.0;

                            if (mobileTarget is PlayerMobile)
                            {
                                damageScalar *= UOACZSystem.GetFatigueScalar(player);
                            }

                            if (mobileTarget is BaseCreature)
                            {
                                BaseCreature bc_Creature = mobileTarget as BaseCreature;

                                damageScalar = 4;

                                if (bc_Creature.Controlled)
                                    damageScalar *= UOACZSystem.GetFatigueScalar(player);
                            }

                            damageScalar *= UOACZPersistance.UndeadBalanceScalar * UOACZSystem.GetUndeadAbilityScalar(player);

                            double damage = (double)Utility.RandomMinMax(minDamage, maxDamage);

                            damage = (Math.Round((double)damage * damageScalar));

                            UOACZSystem.PlayerInflictDamage(player, mobileTarget, false, (int)damage);

                            new Blood().MoveToWorld(mobileTarget.Location, mobileTarget.Map);
                            AOS.Damage(mobileTarget, player, (int)damage, 0, 100, 0, 0, 0);
                        });
                    });
                });
            }
        }

        #endregion

        #region Chain Lightning Ability

        public static void ChainLightningAbility(PlayerMobile player)
        {
            player.SendMessage("Target a location.");
            player.Target = new ChainLightningAbilityTarget();
        }

        public class ChainLightningAbilityTarget : Target
        {
            private IEntity targetLocation;

            public ChainLightningAbilityTarget(): base(25, true, TargetFlags.None, false)
            {
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile player = from as PlayerMobile;

                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!CanUseAbility(player, UOACZUndeadAbilityType.ChainLightning, true)) return;

                IPoint3D location = target as IPoint3D;

                if (location == null)
                    return;

                Map map = player.Map;

                if (map == null)
                    return;

                SpellHelper.GetSurfaceTop(ref location);

                if (location is Mobile)
                    targetLocation = (Mobile)location;

                else
                    targetLocation = new Entity(Serial.Zero, new Point3D(location), map);

                if (Utility.GetDistance(player.Location, targetLocation.Location) > 8)
                {
                    player.SendMessage("That target is too far away.");
                    return;
                }

                if (!player.Map.InLOS(player.Location, targetLocation.Location))
                {
                    player.SendMessage("That is not within in your line of sight.");
                    return;
                }
                
                double directionDelay = .25;
                double initialDelay = UOACZSystem.AbilityInitialDelaySeconds;
                double totalDelay = directionDelay + initialDelay;

                SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1, totalDelay, true, 0, false, "", "");

                player.RevealingAction();

                Point3D playerLocation = player.Location;
                Map playerMap = player.Map;

                Direction directionToTarget = Utility.GetDirection(playerLocation, targetLocation.Location);
                player.Direction = directionToTarget;

                Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (!player.IsUOACZUndead) return;                  

                    int animation = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimation;
                    int animationFrames = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimationFrames;

                    player.Animate(animation, animationFrames, 1, true, false, 0);
                    player.PlaySound(player.GetAngerSound());

                    Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
                    {
                        if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                        if (!player.IsUOACZUndead) return;

                        AbilitySuccessful(player, UOACZUndeadAbilityType.ChainLightning);

                        int radius = 3;

                        int lightningBolts = 3;
                        double delay = 1.5 / (double)lightningBolts;

                        for (int a = 0; a < lightningBolts; a++)
                        {
                            Timer.DelayCall(TimeSpan.FromSeconds(a * delay), delegate
                            {
                                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                                if (!player.IsUOACZUndead) return;

                                Queue m_Queue = new Queue();

                                IPooledEnumerable nearbyMobiles = player.Map.GetMobilesInRange(targetLocation.Location, radius);

                                foreach (Mobile mobile in nearbyMobiles)
                                {
                                    if (mobile == player) continue;
                                    if (!UOACZSystem.IsUOACZValidMobile(mobile)) continue;
                                    if (mobile is UOACZBaseUndead) continue;

                                    PlayerMobile playerTarget = mobile as PlayerMobile;

                                    if (playerTarget != null)
                                    {
                                        if (playerTarget.IsUOACZUndead)
                                            continue;
                                    }

                                    if (!map.InLOS(mobile.Location, targetLocation.Location))
                                        continue;

                                    m_Queue.Enqueue(mobile);
                                }

                                nearbyMobiles.Free();

                                while (m_Queue.Count > 0)
                                {
                                    Mobile mobileTarget = (Mobile)m_Queue.Dequeue();

                                    mobileTarget.BoltEffect(0);

                                    mobileTarget.PlaySound(0x29);

                                    int minDamage = 5;
                                    int maxDamage = 10;

                                    double damageScalar = 1.0;

                                    if (mobileTarget is PlayerMobile)
                                    {
                                        damageScalar *= UOACZSystem.GetFatigueScalar(player);
                                    }

                                    if (mobileTarget is BaseCreature)
                                    {
                                        BaseCreature bc_Creature = mobileTarget as BaseCreature;

                                        damageScalar = 4;

                                        if (bc_Creature.Controlled)
                                            damageScalar *= UOACZSystem.GetFatigueScalar(player);
                                    }

                                    damageScalar *= UOACZPersistance.UndeadBalanceScalar * UOACZSystem.GetUndeadAbilityScalar(player);

                                    double damage = (double)Utility.RandomMinMax(minDamage, maxDamage);

                                    damage = (Math.Round((double)damage * damageScalar));

                                    UOACZSystem.PlayerInflictDamage(player, mobileTarget, false, (int)damage);

                                    player.DoHarmful(mobileTarget);

                                    new Blood().MoveToWorld(mobileTarget.Location, mobileTarget.Map);
                                    AOS.Damage(mobileTarget, player, (int)damage, 0, 100, 0, 0, 0);
                                }
                            });
                        }
                    });
                });
            }
        }

        #endregion

        #region Void Rift Ability

        public static void VoidRiftAbility(PlayerMobile player)
        {
            player.SendMessage("Target a location.");
            player.Target = new VoidRiftAbilityTarget();
        }

        public class VoidRiftAbilityTarget : Target
        {
            private IEntity targetLocation;

            public VoidRiftAbilityTarget(): base(25, true, TargetFlags.None, false)
            {
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile player = from as PlayerMobile;

                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!CanUseAbility(player, UOACZUndeadAbilityType.VoidRift, true)) return;

                IPoint3D location = target as IPoint3D;

                if (location == null)
                    return;

                Map map = player.Map;

                if (map == null)
                    return;

                SpellHelper.GetSurfaceTop(ref location);

                if (location is Mobile)
                    targetLocation = (Mobile)location;

                else
                    targetLocation = new Entity(Serial.Zero, new Point3D(location), map);

                if (Utility.GetDistance(player.Location, targetLocation.Location) > 8)
                {
                    player.SendMessage("That target is too far away.");
                    return;
                }

                if (!player.Map.InLOS(player.Location, targetLocation.Location))
                {
                    player.SendMessage("That is not within in your line of sight.");
                    return;
                }

                bool foundVoidRift = false;

                IPooledEnumerable nearbyItems = map.GetItemsInRange(targetLocation.Location, 1);

                foreach (Item item in nearbyItems)
                {
                    if (item is TimedStatic)
                    {
                        TimedStatic timedStatic = item as TimedStatic;

                        if (timedStatic.Name == "void rift" || timedStatic.Name == "darkblast")
                        {
                            foundVoidRift = true;
                            break;
                        }                       
                    }
                }

                nearbyItems.Free();

                if (foundVoidRift)
                {
                    player.SendMessage("There is already a void rift or darkblast too close to that location.");
                    return;
                }

                double directionDelay = .25;
                double initialDelay = UOACZSystem.AbilityInitialDelaySeconds;
                double totalDelay = directionDelay + initialDelay;

                SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1, totalDelay, true, 0, false, "", "");

                player.RevealingAction();

                Point3D playerLocation = player.Location;
                Map playerMap = player.Map;

                Direction directionToTarget = Utility.GetDirection(playerLocation, targetLocation.Location);
                player.Direction = directionToTarget;

                Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (!player.IsUOACZUndead) return;

                    int animation = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimation;
                    int animationFrames = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimationFrames;

                    player.Animate(animation, animationFrames, 1, true, false, 0);
                    player.PlaySound(player.GetAngerSound());

                    Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
                    {
                        if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                        if (!player.IsUOACZUndead) return;

                        AbilitySuccessful(player, UOACZUndeadAbilityType.VoidRift);

                        player.PlaySound(0x5C3); 

                        Point3D riftLocation = targetLocation.Location;
                        Map riftMap = targetLocation.Map;

                        Point3D animationLocation = riftLocation;
                        animationLocation.X++;
                        animationLocation.Y++;

                        double duration = 30;

                        TimedStatic voidRift = new TimedStatic(14695, duration);
                        voidRift.Name = "void rift";
                        voidRift.Hue = 2055;
                        voidRift.MoveToWorld(animationLocation, riftMap);

                        for (int a = 1; a < (int)(duration) + 1; a++)
                        {
                            bool playsound = false;

                            if (a % 2 == 0)
                                playsound = true;

                            Timer.DelayCall(TimeSpan.FromSeconds(a), delegate
                            {
                                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                                if (!player.IsUOACZUndead) return;

                                if (playsound)
                                    Effects.PlaySound(riftLocation, riftMap, 0x5C3);

                                Queue m_Queue = new Queue();

                                IPooledEnumerable nearbyMobiles = riftMap.GetMobilesInRange(riftLocation, 1);

                                foreach (Mobile mobile in nearbyMobiles)
                                {   
                                    if (!UOACZSystem.IsUOACZValidMobile(mobile)) continue;
                                    if (mobile == player) continue;
                                    if (!riftMap.InLOS(riftLocation, mobile.Location)) continue;
                                    if (mobile is UOACZBaseUndead) continue;

                                    PlayerMobile playerTarget = mobile as PlayerMobile;

                                    if (playerTarget != null)
                                    {
                                        if (playerTarget.IsUOACZUndead)
                                            continue;
                                    }                                    

                                    m_Queue.Enqueue(mobile);
                                }

                                nearbyMobiles.Free();

                                while (m_Queue.Count > 0)
                                {
                                    Mobile mobile = (Mobile)m_Queue.Dequeue();

                                    int minDamage = 1;
                                    int maxDamage = 3;

                                    double damageScalar = 1.0;

                                    if (mobile is PlayerMobile)
                                    {
                                        damageScalar *= UOACZSystem.GetFatigueScalar(player);
                                    }

                                    if (mobile is BaseCreature)
                                    {
                                        BaseCreature bc_Creature = mobile as BaseCreature;

                                        damageScalar = 2;

                                        if (bc_Creature.Controlled)
                                            damageScalar *= UOACZSystem.GetFatigueScalar(player);
                                    }

                                    damageScalar *= UOACZPersistance.UndeadBalanceScalar * UOACZSystem.GetUndeadAbilityScalar(player);

                                    double damage = Math.Round((double)Utility.RandomMinMax(minDamage, maxDamage) * damageScalar);

                                    UOACZSystem.PlayerInflictDamage(player, mobile, false, (int)damage);

                                    new Blood().MoveToWorld(mobile.Location, mobile.Map);
                                    mobile.FixedParticles(0x374A, 10, 15, 5013, 2055, 0, EffectLayer.LeftFoot);

                                    mobile.PlaySound(0x211);                                    

                                    if (UOACZSystem.IsUOACZValidMobile(player))
                                    {
                                        player.DoHarmful(mobile);
                                        AOS.Damage(mobile, player, (int)damage, 0, 100, 0, 0, 0);
                                    }

                                    else
                                        AOS.Damage(mobile, (int)damage, 0, 100, 0, 0, 0);
                                }
                            });
                        }

                        Timer.DelayCall(TimeSpan.FromSeconds(duration), delegate
                        {
                            if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                            if (!player.IsUOACZUndead) return;
                            if (Utility.GetDistance(riftLocation, player.Location) >= 40) return;

                            player.PlaySound(0x5D8);

                            for (int a = 0; a < 8; a++)
                            {
                                TimedStatic ichor = new TimedStatic(Utility.RandomList(4650, 4651, 4652, 4653, 4654, 4655), 5);
                                ichor.Hue = 2051;
                                ichor.Name = "ichor";

                                Point3D newPoint = new Point3D(riftLocation.X + Utility.RandomList(-2, -1, 1, 2), riftLocation.Y + Utility.RandomList(-2, -1, 1, 2), riftLocation.Z);
                                SpellHelper.AdjustField(ref newPoint, player.Map, 12, false);

                                ichor.MoveToWorld(newPoint, player.Map);
                            }

                            UOACZVoidSlime creature = new UOACZVoidSlime();

                            int controlSlotsNeeded = 1;

                            if (player.Followers + controlSlotsNeeded <= player.FollowersMax)
                            {
                                creature.TimesTamed++;
                                creature.SetControlMaster(player);
                                creature.IsBonded = false;
                                creature.OwnerAbandonTime = DateTime.UtcNow + creature.AbandonDelay;

                                UOACZSystem.AddCreatureToPlayerSwarm(player, creature);
                            }

                            else
                                player.SendMessage("One or more creatures were unable to join your swarm as they would exceed your swarm control slot limit.");

                            creature.MoveToWorld(riftLocation, riftMap);
                            creature.PlaySound(creature.GetIdleSound()); 
                        });
                    });
                });
            }
        }

        #endregion

        #region Enervate Ability

        public static void EnervateAbility(PlayerMobile player)
        {
            player.SendMessage("Target a player or creature.");
            player.Target = new EnervateAbilityTarget();
        }

        public class EnervateAbilityTarget : Target
        {
            public EnervateAbilityTarget(): base(25, false, TargetFlags.Harmful, false)
            {
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile player = from as PlayerMobile;

                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!CanUseAbility(player, UOACZUndeadAbilityType.Enervate, true)) return;

                Mobile mobileTarget = null;

                if (target == player)
                {
                    player.SendMessage("You cannot target yourself.");
                    return;
                }

                if (target is Mobile)
                {
                    mobileTarget = target as Mobile;

                    if (!UOACZSystem.IsUOACZValidMobile(mobileTarget))
                    {
                        player.SendMessage("That cannot be damaged.");
                        return;
                    }
                }

                else
                {
                    player.SendMessage("Perhaps this would be better off targeted at something living.");
                    return;
                }

                Map map = player.Map;

                if (Utility.GetDistance(player.Location, mobileTarget.Location) > 8)
                {
                    player.SendMessage("That target is too far away.");
                    return;
                }

                if (!player.Map.InLOS(player.Location, mobileTarget.Location))
                {
                    player.SendMessage("That is not within in your line of sight.");
                    return;
                }
                                
                double directionDelay = .25;
                double initialDelay = UOACZSystem.AbilityInitialDelaySeconds;
                double totalDelay = directionDelay + initialDelay;

                SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1, totalDelay, true, 0, false, "", "");

                player.RevealingAction();

                Point3D playerLocation = player.Location;
                Map playerMap = player.Map;

                Direction directionToTarget = Utility.GetDirection(playerLocation, mobileTarget.Location);
                player.Direction = directionToTarget;

                Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (!player.IsUOACZUndead) return;
                    if (!UOACZSystem.IsUOACZValidMobile(mobileTarget)) return;
                    if (Utility.GetDistance(player.Location, mobileTarget.Location) >= 20) return;
                    if (mobileTarget.Hidden) return;

                    int animation = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimation;
                    int animationFrames = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimationFrames;

                    player.Animate(animation, animationFrames, 1, true, false, 0);
                    player.PlaySound(player.GetAngerSound());

                    Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
                    {
                        if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                        if (!player.IsUOACZUndead) return;
                        if (!UOACZSystem.IsUOACZValidMobile(mobileTarget)) return;
                        if (Utility.GetDistance(player.Location, mobileTarget.Location) >= 20) return;
                        if (mobileTarget.Hidden) return;

                        AbilitySuccessful(player, UOACZUndeadAbilityType.Enervate);

                        int effectSound = 0x246;
                        int itemID = 0x3728;
                        int itemHue = 2610;

                        Effects.PlaySound(player.Location, player.Map, effectSound);
                        player.MovingEffect(mobileTarget, 0x3728, 5, 1, false, false, itemHue, 0);
                        
                        bool livingTarget = false;

                        int minDamage = 10;
                        int maxDamage = 20;

                        double damage = (double)Utility.RandomMinMax(minDamage, maxDamage);

                        double damageScalar = 1.0;

                        if (mobileTarget is PlayerMobile)
                        {
                            damageScalar *= UOACZSystem.GetFatigueScalar(player);
                        }

                        if (mobileTarget is BaseCreature)
                        {
                            BaseCreature bc_Creature = mobileTarget as BaseCreature;

                            damageScalar = 4;

                            if (bc_Creature.Controlled)
                                damageScalar *= UOACZSystem.GetFatigueScalar(player);
                        }

                        damageScalar *= UOACZPersistance.UndeadBalanceScalar * UOACZSystem.GetUndeadAbilityScalar(player);

                        if (mobileTarget is UOACZBaseWildlife)
                            livingTarget = true;

                        if (mobileTarget is UOACZBaseHuman)
                            livingTarget = true;

                        if (mobileTarget is PlayerMobile)
                        {
                            PlayerMobile playerTarget = mobileTarget as PlayerMobile;

                            if (playerTarget.IsUOACZHuman)
                                livingTarget = true;
                        }

                        damage = (Math.Round((double)damage * damageScalar));

                        UOACZSystem.PlayerInflictDamage(player, mobileTarget, false, (int)damage);

                        player.RevealingAction();

                        player.DoHarmful(mobileTarget);

                        new Blood().MoveToWorld(mobileTarget.Location, mobileTarget.Map);
                        AOS.Damage(mobileTarget, player, (int)damage, 0, 100, 0, 0, 0);                       

                        Timer.DelayCall(TimeSpan.FromSeconds(.25), delegate
                        {
                            if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                            if (!player.IsUOACZUndead) return;

                            if (!livingTarget)
                                return;

                            bool targetKilled = false;

                            if (mobileTarget == null)
                                targetKilled = true;

                            else if (mobileTarget.Deleted || !mobileTarget.Alive)
                                targetKilled = true;

                            if (!targetKilled)
                                return;

                            player.FixedParticles(0x373A, 10, 15, 5036, EffectLayer.Head);
                            player.PlaySound(0x3BD);

                            foreach (UOACZUndeadAbilityEntry abilityEntry in player.m_UOACZAccountEntry.UndeadProfile.m_Abilities)
                            {
                                DateTime cooldown = abilityEntry.m_NextUsageAllowed;

                                if (cooldown > DateTime.UtcNow)
                                {
                                    double cooldownReduction = abilityEntry.m_CooldownMinutes * .20;

                                    abilityEntry.m_NextUsageAllowed = abilityEntry.m_NextUsageAllowed.Subtract(TimeSpan.FromMinutes(cooldownReduction));
                                }
                            }

                            UOACZSystem.RefreshAllGumps(player);
                        });                         
                    });
                });
            }
        }

        #endregion        
        
        #region Darkblast Ability

        public static void DarkblastAbility(PlayerMobile player)
        {
            player.SendMessage("Target a location.");
            player.Target = new DarkblastAbilityTarget();
        }

        public class DarkblastAbilityTarget : Target
        {
            private IEntity targetLocation;

            public DarkblastAbilityTarget(): base(25, true, TargetFlags.None, false)
            {
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile player = from as PlayerMobile;

                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!CanUseAbility(player, UOACZUndeadAbilityType.Darkblast, true)) return;

                IPoint3D location = target as IPoint3D;

                if (location == null)
                    return;

                Map map = player.Map;

                if (map == null)
                    return;

                SpellHelper.GetSurfaceTop(ref location);

                if (location is Mobile)
                    targetLocation = (Mobile)location;

                else
                    targetLocation = new Entity(Serial.Zero, new Point3D(location), map);

                if (Utility.GetDistance(player.Location, targetLocation.Location) > 8)
                {
                    player.SendMessage("That target is too far away.");
                    return;
                }

                if (!player.Map.InLOS(player.Location, targetLocation.Location))
                {
                    player.SendMessage("That is not within in your line of sight.");
                    return;
                }

                bool foundVoidRift = false;

                IPooledEnumerable nearbyItems = map.GetItemsInRange(targetLocation.Location, 1);

                foreach (Item item in nearbyItems)
                {
                    if (item is TimedStatic)
                    {
                        TimedStatic timedStatic = item as TimedStatic;

                        if (timedStatic.Name == "void rift" || timedStatic.Name == "darkblast")
                        {
                            foundVoidRift = true;
                            break;
                        }
                    }
                }

                nearbyItems.Free();

                if (foundVoidRift)
                {
                    player.SendMessage("There is already a darkblast or void rift too close to that location.");
                    return;
                }

                double directionDelay = .25;
                double initialDelay = UOACZSystem.AbilityInitialDelaySeconds;
                double totalDelay = directionDelay + initialDelay;

                SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1, totalDelay, true, 0, false, "", "");

                player.RevealingAction();

                Point3D playerLocation = player.Location;
                Map playerMap = player.Map;

                Direction directionToTarget = Utility.GetDirection(playerLocation, targetLocation.Location);
                player.Direction = directionToTarget;

                Point3D darkblastLocation = targetLocation.Location;
                Map darkblastMap = targetLocation.Map;

                Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (!player.IsUOACZUndead) return;

                    int animation = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimation;
                    int animationFrames = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimationFrames;

                    player.Animate(animation, animationFrames, 1, true, false, 0);
                    player.PlaySound(player.GetAngerSound());

                    Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
                    {
                        if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                        if (!player.IsUOACZUndead) return;

                        AbilitySuccessful(player, UOACZUndeadAbilityType.Darkblast);

                        player.PlaySound(0x20F);

                        double seconds = 2;
                        int effectHue = 2800;

                        Dictionary<int, Point3D> m_DarkblastComponents = new Dictionary<int, Point3D>();

                        m_DarkblastComponents.Add(0x3083, new Point3D(darkblastLocation.X - 1, darkblastLocation.Y - 1, darkblastLocation.Z));
                        m_DarkblastComponents.Add(0x3080, new Point3D(darkblastLocation.X - 1, darkblastLocation.Y, darkblastLocation.Z));
                        m_DarkblastComponents.Add(0x3082, new Point3D(darkblastLocation.X, darkblastLocation.Y - 1, darkblastLocation.Z));
                        m_DarkblastComponents.Add(0x3081, new Point3D(darkblastLocation.X + 1, darkblastLocation.Y - 1, darkblastLocation.Z));
                        m_DarkblastComponents.Add(0x307D, new Point3D(darkblastLocation.X - 1, darkblastLocation.Y + 1, darkblastLocation.Z));
                        m_DarkblastComponents.Add(0x307F, new Point3D(darkblastLocation.X, darkblastLocation.Y, darkblastLocation.Z));
                        m_DarkblastComponents.Add(0x307E, new Point3D(darkblastLocation.X + 1, darkblastLocation.Y, darkblastLocation.Z));
                        m_DarkblastComponents.Add(0x307C, new Point3D(darkblastLocation.X, darkblastLocation.Y + 1, darkblastLocation.Z));
                        m_DarkblastComponents.Add(0x307B, new Point3D(darkblastLocation.X + 1, darkblastLocation.Y + 1, darkblastLocation.Z));

                        foreach (KeyValuePair<int, Point3D> keyPairValue in m_DarkblastComponents)
                        {
                            TimedStatic darkblastComponent = new TimedStatic(keyPairValue.Key, seconds);
                            darkblastComponent.Name = "darkblast";
                            darkblastComponent.Hue = effectHue;
                            darkblastComponent.MoveToWorld(keyPairValue.Value, darkblastMap);
                        }

                        Effects.SendLocationParticles(EffectItem.Create(darkblastLocation, darkblastMap, TimeSpan.FromSeconds(0.5)), 14202, 10, 30, effectHue, 0, 5029, 0);

                        Timer.DelayCall(TimeSpan.FromSeconds(seconds), delegate
                        {
                            if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                            if (!player.IsUOACZUndead) return;

                            foreach (KeyValuePair<int, Point3D> keyPairValue in m_DarkblastComponents)
                            {
                                Point3D explosionLocation = keyPairValue.Value;

                                Effects.SendLocationParticles(EffectItem.Create(explosionLocation, darkblastMap, TimeSpan.FromSeconds(0.5)), 0x3709, 10, 30, effectHue, 0, 5029, 0);
                                Effects.PlaySound(location, map, 0x56E);
                            }

                            Queue m_Queue = new Queue();

                            IPooledEnumerable nearbyMobiles = darkblastMap.GetMobilesInRange(darkblastLocation, 1);

                            foreach (Mobile mobile in nearbyMobiles)
                            {
                                if (!UOACZSystem.IsUOACZValidMobile(mobile)) continue;
                                if (mobile == player) continue;
                                if (!darkblastMap.InLOS(darkblastLocation, mobile.Location)) continue;
                                if (mobile is UOACZBaseUndead) continue;

                                PlayerMobile playerTarget = mobile as PlayerMobile;

                                if (playerTarget != null)
                                {
                                    if (playerTarget.IsUOACZUndead)
                                        continue;
                                }

                                m_Queue.Enqueue(mobile);
                            }

                            nearbyMobiles.Free();

                            while (m_Queue.Count > 0)
                            {
                                Mobile mobile = (Mobile)m_Queue.Dequeue();

                                int minDamage = 20;
                                int maxDamage = 30;

                                double damageScalar = 1.0;

                                if (mobile is PlayerMobile)
                                {
                                    damageScalar *= UOACZSystem.GetFatigueScalar(player);
                                }

                                if (mobile is BaseCreature)
                                {
                                    BaseCreature bc_Creature = mobile as BaseCreature;

                                    damageScalar = 4;

                                    if (bc_Creature.Controlled)
                                        damageScalar *= UOACZSystem.GetFatigueScalar(player);
                                }

                                damageScalar *= UOACZPersistance.UndeadBalanceScalar * UOACZSystem.GetUndeadAbilityScalar(player);

                                double damage = (double)Utility.RandomMinMax(minDamage, maxDamage);

                                damage = (Math.Round((double)damage * damageScalar));                                

                                Effects.PlaySound(darkblastLocation, darkblastMap, 0x20A);

                                new Blood().MoveToWorld(mobile.Location, mobile.Map);

                                UOACZSystem.PlayerInflictDamage(player, mobile, false, (int)damage);

                                if (UOACZSystem.IsUOACZValidMobile(player))
                                {
                                    player.DoHarmful(mobile);
                                    AOS.Damage(mobile, player, (int)damage, 0, 100, 0, 0, 0);
                                }

                                else
                                    AOS.Damage(mobile, (int)damage, 0, 100, 0, 0, 0);
                            }
                        });
                    });
                });
            }
        }

        #endregion

        #region Engulf Ability

        public static void EngulfAbility(PlayerMobile player)
        {
            double directionDelay = .25;
            double initialDelay = UOACZSystem.AbilityInitialDelaySeconds;
            double totalDelay = directionDelay + initialDelay;

            SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1, totalDelay, true, 0, false, "", "");

            player.RevealingAction();

            Point3D playerLocation = player.Location;
            Map playerMap = player.Map;

            int range = 8;

            Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
            {
                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!player.IsUOACZUndead) return;

                int animation = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimation;
                int animationFrames = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimationFrames;

                player.Animate(animation, animationFrames, 1, true, false, 0);
                player.PlaySound(player.GetAngerSound());

                Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (!player.IsUOACZUndead) return;

                    AbilitySuccessful(player, UOACZUndeadAbilityType.Engulf);

                    player.PlaySound(0x573);

                    for (int a = 0; a < 15; a++)
                    {
                        TimedStatic ichor = new TimedStatic(Utility.RandomList(4650, 4651, 4652, 4653, 4654, 4655), 5);
                        ichor.Hue = 2051;
                        ichor.Name = "ichor";

                        Point3D newPoint = new Point3D(player.Location.X + Utility.RandomList(-2, -1, 1, 2), player.Location.Y + Utility.RandomList(-2, -1, 1, 2), player.Location.Z);
                        SpellHelper.AdjustField(ref newPoint, player.Map, 12, false);

                        ichor.MoveToWorld(newPoint, player.Map);
                    }

                    Queue m_Queue = new Queue();

                    IPooledEnumerable nearbyMobiles = player.Map.GetMobilesInRange(player.Location, range);

                    foreach (Mobile mobile in nearbyMobiles)
                    {
                        if (mobile == player) continue;
                        if (!UOACZSystem.IsUOACZValidMobile(mobile)) continue;
                        if (mobile.Hidden) continue;
                        if (!player.Map.InLOS(player.Location, mobile.Location)) continue;
                        if (mobile is UOACZBaseUndead) continue;

                        PlayerMobile playerTarget = mobile as PlayerMobile;

                        if (playerTarget != null)
                        {
                            if (playerTarget.IsUOACZUndead)
                                continue;
                        }

                        if (mobile is UOACZBaseMilitia)
                        {
                            UOACZBaseMilitia militia = mobile as UOACZBaseMilitia;

                            if (militia.Sentry)
                                continue;
                        }
                        
                        m_Queue.Enqueue(mobile);
                    }

                    nearbyMobiles.Free();

                    while (m_Queue.Count > 0)
                    {
                        Mobile mobile = (Mobile)m_Queue.Dequeue();

                        mobile.Location = player.Location;
                        SpecialAbilities.HinderSpecialAbility(1.0, null, mobile, 1.0, 1, true, 0, false, "", "You have been engulfed!");

                        Effects.PlaySound(mobile.Location, mobile.Map, 0x101);

                        int minDamage = 15;
                        int maxDamage = 25;

                        double damage = (double)Utility.RandomMinMax(minDamage, maxDamage);

                        double damageScalar = 1.0;

                       if (mobile is PlayerMobile)
                        {
                            damageScalar *= UOACZSystem.GetFatigueScalar(player);
                        }

                        if (mobile is BaseCreature)
                        {
                            BaseCreature bc_Creature = mobile as BaseCreature;

                            damageScalar = 3;

                            if (bc_Creature.Controlled)
                                damageScalar *= UOACZSystem.GetFatigueScalar(player);
                        }

                        damageScalar *= UOACZPersistance.UndeadBalanceScalar * UOACZSystem.GetUndeadAbilityScalar(player);

                        damage = (Math.Round((double)damage * damageScalar));

                        UOACZSystem.PlayerInflictDamage(player, mobile, false, (int)damage);

                        player.DoHarmful(mobile);

                        new Blood().MoveToWorld(mobile.Location, mobile.Map);
                        AOS.Damage(mobile, player, (int)damage, 0, 100, 0, 0, 0);
                    }
                });
            });
        }
        
        #endregion        

        #region Wild Hunt Ability

        public static void WildHuntAbility(PlayerMobile player)
        {
            player.SendMessage("Target a location to raise a skeletal critter at.");
            player.Target = new WildHuntAbilityTarget();
        }

        public class WildHuntAbilityTarget : Target
        {
            private IEntity targetLocation;

            public WildHuntAbilityTarget(): base(25, true, TargetFlags.None, false)
            {
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile player = from as PlayerMobile;

                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!CanUseAbility(player, UOACZUndeadAbilityType.WildHunt, true)) return;

                IPoint3D location = target as IPoint3D;

                if (location == null)
                    return;

                Map map = player.Map;

                if (map == null)
                    return;

                SpellHelper.GetSurfaceTop(ref location);

                if (location is Mobile)
                    targetLocation = (Mobile)location;

                else
                    targetLocation = new Entity(Serial.Zero, new Point3D(location), map);

                if (Utility.GetDistance(player.Location, targetLocation.Location) > 8)
                {
                    player.SendMessage("That target is too far away.");
                    return;
                }

                if (!player.Map.InLOS(player.Location, targetLocation.Location))
                {
                    player.SendMessage("That location is not within in your line of sight.");
                    return;
                }

                double directionDelay = .25;
                double initialDelay = UOACZSystem.AbilityInitialDelaySeconds;
                double totalDelay = directionDelay + initialDelay;

                SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1, totalDelay, true, 0, false, "", "");

                player.RevealingAction();

                Point3D playerLocation = player.Location;
                Map playerMap = player.Map;

                Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (!player.IsUOACZUndead) return;

                    int animation = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimation;
                    int animationFrames = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimationFrames;

                    player.Animate(animation, animationFrames, 1, true, false, 0);
                    player.PlaySound(player.GetAngerSound());

                    Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
                    {
                        if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                        if (!player.IsUOACZUndead) return;

                        AbilitySuccessful(player, UOACZUndeadAbilityType.WildHunt);

                        UOACZSkeletalCritter creature = new UOACZSkeletalCritter();
                        creature.MoveToWorld(targetLocation.Location, targetLocation.Map);

                        int controlSlotsNeeded = 1;

                        if (player.Followers + controlSlotsNeeded <= player.FollowersMax)
                        {
                            creature.TimesTamed++;
                            creature.SetControlMaster(player);
                            creature.IsBonded = false;
                            creature.OwnerAbandonTime = DateTime.UtcNow + creature.AbandonDelay;

                            UOACZSystem.AddCreatureToPlayerSwarm(player, creature);
                        }

                        else
                            player.SendMessage("One or more creatures were unable to join your swarm as they would exceed your swarm control slot limit.");

                        creature.PlaySound(creature.GetIdleSound());
                    });
                });
            }
        }

        #endregion

        #region Charge Ability

        public static void ChargeAbility(PlayerMobile player)
        {
            player.SendMessage("Target the location you wish to charge to.");
            player.Target = new ChargeAbilityTarget();
        }

        public class ChargeAbilityTarget : Target
        {
            private IEntity targetLocation;

            public ChargeAbilityTarget(): base(25, true, TargetFlags.None, false)
            {
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile player = from as PlayerMobile;

                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!CanUseAbility(player, UOACZUndeadAbilityType.Charge, true)) return;

                Mobile mobileTarget = null;

                if (target == player)
                {
                    player.SendMessage("You cannot target yourself.");
                    return;
                }

                if (target is Mobile)
                {
                    mobileTarget = target as Mobile;

                    if (!UOACZSystem.IsUOACZValidMobile(mobileTarget))
                    {
                        player.SendMessage("That cannot be damaged.");
                        return;
                    }
                }

                else
                {
                    player.SendMessage("Perhaps this would be better off targeted at something living. Or undead.");
                    return;
                }

                if (Utility.GetDistance(player.Location, mobileTarget.Location) > 12)
                {
                    player.SendMessage("That target is too far away.");
                    return;
                }

                if (!player.Map.InLOS(player.Location, mobileTarget.Location))
                {
                    player.SendMessage("That is not within in your line of sight.");
                    return;
                }

                BaseWeapon weapon = player.Weapon as BaseWeapon;

                if (weapon == null)
                    return;

                double directionDelay = .25;
                double initialDelay = UOACZSystem.AbilityInitialDelaySeconds;
                double totalDelay = directionDelay + initialDelay;

                SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1, totalDelay, true, 0, false, "", "");

                player.RevealingAction();

                Point3D playerLocation = player.Location;
                Map playerMap = player.Map;

                Direction directionToTarget = Utility.GetDirection(playerLocation, mobileTarget.Location);
                player.Direction = directionToTarget;

                Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (!player.IsUOACZUndead) return;
                    if (!UOACZSystem.IsUOACZValidMobile(mobileTarget)) return;
                    if (Utility.GetDistance(player.Location, mobileTarget.Location) >= 20) return;

                    int animation = player.m_UOACZAccountEntry.UndeadProfile.SpecialAnimation;
                    int animationFrames = player.m_UOACZAccountEntry.UndeadProfile.SpecialAnimationFrames;

                    player.Animate(animation, animationFrames, 1, true, false, 0);
                    player.PlaySound(player.GetAngerSound());

                    Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
                    {
                        if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                        if (!player.IsUOACZUndead) return;
                        if (!UOACZSystem.IsUOACZValidMobile(mobileTarget)) return;
                        if (Utility.GetDistance(player.Location, mobileTarget.Location) >= 20) return;

                        AbilitySuccessful(player, UOACZUndeadAbilityType.Charge);

                        Point3D effectStep = player.Location;
                        Point3D mobileLocation = mobileTarget.Location;
                        Map map = mobileTarget.Map;

                        int distance = Utility.GetDistance(effectStep, mobileLocation);

                        for (int a = 0; a < distance; a++)
                        {
                            Timer.DelayCall(TimeSpan.FromSeconds(a * .05), delegate
                            {
                                Direction direction = Utility.GetDirection(effectStep, mobileLocation);
                                effectStep = SpecialAbilities.GetPointByDirection(effectStep, direction);

                                Effects.SendLocationParticles(EffectItem.Create(effectStep, map, EffectItem.DefaultDuration), 0x3728, 10, 10, 0, 0, 2023, 0);
                                Effects.PlaySound(effectStep, map, 0x5C6);
                            });
                        }

                        player.Location = mobileTarget.Location;

                        player.DoHarmful(mobileTarget);

                        bool attackHit = weapon.CheckHit(player, mobileTarget);

                        if (!attackHit)
                        {
                            if (mobileTarget is BaseCreature)
                                attackHit = true;
                        }

                        if (attackHit)
                        {
                            double damageScalar = 1;

                            if (mobileTarget is PlayerMobile)
                            {
                                damageScalar *= UOACZSystem.GetFatigueScalar(player);
                            }

                            if (mobileTarget is BaseCreature)
                            {
                                BaseCreature bc_Creature = mobileTarget as BaseCreature;

                                damageScalar = 4;

                                if (bc_Creature.Controlled)
                                    damageScalar *= UOACZSystem.GetFatigueScalar(player);
                            }

                            damageScalar *= UOACZPersistance.UndeadBalanceScalar * UOACZSystem.GetUndeadAbilityScalar(player);

                            weapon.OnHit(player, mobileTarget, damageScalar);

                            Timer.DelayCall(TimeSpan.FromSeconds(.1), delegate
                            {
                                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                                if (!player.IsUOACZUndead) return;
                                if (!UOACZSystem.IsUOACZValidMobile(mobileTarget)) return;

                                double knockbackDamage = 15;

                                if (mobileTarget is BaseCreature)
                                    knockbackDamage *= 3;

                                SpecialAbilities.KnockbackSpecialAbility(1.0, mobileLocation, player, mobileTarget, knockbackDamage, 3, -1, "", "You are knocked back by their charge!");
                            });
                        }

                        else
                            weapon.OnMiss(player, mobileTarget);

                        
                        player.NextCombatTime = DateTime.UtcNow + weapon.GetDelay(player, false);
                    });
                });
            }
        }

        #endregion        

        #region Firebreath Ability

        public static void FirebreathAbility(PlayerMobile player)
        {
            player.SendMessage("Target a creature or player to attack.");
            player.Target = new FirebreathAbilityTarget();
        }

        public class FirebreathAbilityTarget : Target
        {
            public FirebreathAbilityTarget(): base(25, false, TargetFlags.Harmful, false)
            {
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile player = from as PlayerMobile;

                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!CanUseAbility(player, UOACZUndeadAbilityType.Firebreath, true)) return;

                Mobile mobileTarget = null;

                if (target == player)
                {
                    player.SendMessage("You cannot target yourself.");
                    return;
                }

                if (target is Mobile)
                {
                    mobileTarget = target as Mobile;

                    if (!UOACZSystem.IsUOACZValidMobile(mobileTarget))
                    {
                        player.SendMessage("That cannot be damaged.");
                        return;
                    }
                }

                else
                {
                    player.SendMessage("Perhaps this would be better off targeted at something living. Or undead.");
                    return;
                }

                Map map = player.Map;

                if (Utility.GetDistance(player.Location, mobileTarget.Location) > 8)
                {
                    player.SendMessage("That target is too far away.");
                    return;
                }

                if (!player.Map.InLOS(player.Location, mobileTarget.Location))
                {
                    player.SendMessage("That is not within in your line of sight.");
                    return;
                }  

                int fireballs = 12;

                double directionDelay = .25;
                double initialDelay = UOACZSystem.AbilityInitialDelaySeconds;
                double fireballDelay = .1;
                double totalDelay = 1 + directionDelay + initialDelay + ((double)fireballs * fireballDelay);

                SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1, totalDelay, true, 0, false, "", "");

                player.RevealingAction();

                Point3D playerLocation = player.Location;
                Map playerMap = player.Map;

                Direction directionToTarget = Utility.GetDirection(player.Location, mobileTarget.Location);
                player.Direction = directionToTarget;

                Point3D targetLocation = mobileTarget.Location;
                Map targetMap = mobileTarget.Map;

                Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (!player.IsUOACZUndead) return;
                    if (!UOACZSystem.IsUOACZValidMobile(mobileTarget)) return;
                    if (Utility.GetDistance(player.Location, mobileTarget.Location) >= 20) return;
                    if (mobileTarget.Hidden) return;

                    int animation = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimation;
                    int animationFrames = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimationFrames;

                    player.Animate(animation, animationFrames, 1, true, false, 0);
                    player.PlaySound(player.GetAngerSound());

                    Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
                    {
                        if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                        if (!player.IsUOACZUndead) return;
                        if (!UOACZSystem.IsUOACZValidMobile(mobileTarget)) return;
                        if (Utility.GetDistance(player.Location, mobileTarget.Location) >= 20) return;
                        if (mobileTarget.Hidden) return;

                        AbilitySuccessful(player, UOACZUndeadAbilityType.Firebreath);                        

                        for (int a = 0; a < fireballs; a++)
                        {
                            bool willHit = true;

                            if (a % 3 == 0)
                                willHit = false;

                            Timer.DelayCall(TimeSpan.FromSeconds(a * fireballDelay), delegate
                            {
                                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                                if (!player.IsUOACZUndead) return;

                                bool combatantValid = true;

                                if (mobileTarget == null)
                                    combatantValid = false;

                                else if (mobileTarget.Deleted || !mobileTarget.Alive)
                                    combatantValid = false;

                                else
                                {
                                    if (mobileTarget.Hidden || Utility.GetDistance(playerLocation, mobileTarget.Location) >= 15)
                                        combatantValid = false;
                                }

                                if (!UOACZSystem.IsUOACZValidMobile(mobileTarget))
                                    combatantValid = false;

                                if (combatantValid)
                                {
                                    targetLocation = mobileTarget.Location;
                                    targetMap = mobileTarget.Map;

                                    directionToTarget = Utility.GetDirection(player.Location, targetLocation);
                                }

                                int effectSound = 0x357;
                                int itemID = 0x36D4;
                                int itemHue = 0;

                                int impactSound = 0x226;
                                int impactHue = 0;

                                int xOffset = 0;
                                int yOffset = 0;

                                if (!willHit)
                                {
                                    if (Utility.RandomDouble() <= .5)
                                        xOffset = Utility.RandomList(-1, 1);

                                    if (Utility.RandomDouble() <= .5)
                                        yOffset = Utility.RandomList(-1, 1);
                                }

                                IEntity startLocation = new Entity(Serial.Zero, new Point3D(playerLocation.X, playerLocation.Y, playerLocation.Z + 12), map);

                                Point3D adjustedLocation = new Point3D(targetLocation.X + xOffset, targetLocation.Y + yOffset, targetLocation.Z);
                                SpellHelper.AdjustField(ref adjustedLocation, targetMap, 12, false);

                                IEntity endLocation = new Entity(Serial.Zero, new Point3D(adjustedLocation.X, adjustedLocation.Y, adjustedLocation.Z + 8), targetMap);

                                Effects.PlaySound(playerLocation, map, effectSound);

                                if (willHit && combatantValid)                                
                                    player.MovingParticles(mobileTarget, 0x36D4, 4, 0, false, true, 0, 0, 9502, 4019, 0x160, 0); 

                                else                                                                    
                                    Effects.SendMovingEffect(startLocation, endLocation, itemID, 5, 0, false, false, itemHue, 0);                                

                                double targetDistance = Utility.GetDistanceToSqrt(playerLocation, adjustedLocation);
                                double destinationDelay = (double)targetDistance * .08;

                                Direction newDirection = Utility.GetDirection(playerLocation, adjustedLocation);

                                if (player.Direction != newDirection)
                                    player.Direction = newDirection;

                                Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                                {
                                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                                    if (!player.IsUOACZUndead) return;

                                    if (willHit)
                                    {
                                        if (UOACZSystem.IsUOACZValidMobile(mobileTarget))
                                        {
                                            if (!mobileTarget.Hidden && Utility.GetDistance(playerLocation, mobileTarget.Location) <= 30)
                                            {
                                                adjustedLocation = mobileTarget.Location;
                                                targetMap = mobileTarget.Map;
                                            }                                               
                                        }
                                    }

                                    Effects.PlaySound(adjustedLocation, targetMap, impactSound);
                                    Effects.SendLocationParticles(EffectItem.Create(adjustedLocation, targetMap, EffectItem.DefaultDuration), 0x3709, 20, 20, impactHue, 0, 0, 0);

                                    Queue m_Queue = new Queue();

                                    IPooledEnumerable nearbyMobiles = targetMap.GetMobilesInRange(adjustedLocation, 0);

                                    foreach (Mobile mobile in nearbyMobiles)
                                    {
                                        if (mobile == player) continue;
                                        if (!UOACZSystem.IsUOACZValidMobile(mobile)) continue;
                                        if (mobile is UOACZBaseUndead) continue;

                                        PlayerMobile playerTarget = mobile as PlayerMobile;

                                        if (playerTarget != null)
                                        {
                                            if (playerTarget.IsUOACZUndead)
                                                continue;
                                        }

                                        m_Queue.Enqueue(mobile);
                                    }

                                    nearbyMobiles.Free();

                                    while (m_Queue.Count > 0)
                                    {
                                        Mobile mobile = (Mobile)m_Queue.Dequeue();

                                        int minDamage = 3;
                                        int maxDamage = 5;

                                        double damage = (double)Utility.RandomMinMax(minDamage, maxDamage);
                                        double damageScalar = 1;

                                        if (mobile is PlayerMobile)
                                        {
                                            damageScalar *= UOACZSystem.GetFatigueScalar(player);
                                        }

                                        if (mobile is BaseCreature)
                                        {
                                            BaseCreature bc_Creature = mobile as BaseCreature;

                                            damageScalar = 4;

                                            if (bc_Creature.Controlled)
                                                damageScalar *= UOACZSystem.GetFatigueScalar(player);
                                        }

                                        damageScalar *= UOACZPersistance.UndeadBalanceScalar * UOACZSystem.GetUndeadAbilityScalar(player);

                                        damage = (Math.Round((double)damage * damageScalar));

                                        UOACZSystem.PlayerInflictDamage(player, mobile, false, (int)damage);

                                        player.DoHarmful(mobile);

                                        new Blood().MoveToWorld(mobile.Location, mobile.Map);
                                        AOS.Damage(mobile, player, (int)(Math.Round(damage)), 0, 100, 0, 0, 0);
                                    }
                                });
                            });
                        }
                    });
                });
            }
        }

        #endregion

        #region WingBuffet Ability

        public static void WingBuffetAbility(PlayerMobile player)
        {
            int range = 15;

            AbilitySuccessful(player, UOACZUndeadAbilityType.WingBuffet);

            player.RevealingAction();

            SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1.0, 2.0, true, 0, false, "", "");

            int animation = player.m_UOACZAccountEntry.UndeadProfile.SpecialAnimation;
            int animationFrames = player.m_UOACZAccountEntry.UndeadProfile.SpecialAnimationFrames;  
            
            for (int a = 0; a < 2; a++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(a * 1), delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (!player.IsUOACZUndead) return;

                    player.PublicOverheadMessage(MessageType.Regular, 0, false, "*furiously beats wings*");

                    player.Animate(animation, animationFrames, 1, true, false, 0);
                    player.PlaySound(0x63E);
                });
            }

            Timer.DelayCall(TimeSpan.FromSeconds(2.0), delegate
            {
                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!player.IsUOACZUndead) return;

                Queue m_Queue = new Queue();

                IPooledEnumerable nearbyMobiles = player.Map.GetMobilesInRange(player.Location, range);

                foreach (Mobile mobile in nearbyMobiles)
                {
                    if (mobile == player) continue;
                    if (!UOACZSystem.IsUOACZValidMobile(mobile)) continue;
                    if (mobile is UOACZBaseUndead) continue;

                    PlayerMobile playerTarget = mobile as PlayerMobile;

                    if (playerTarget != null)
                    {
                        if (playerTarget.IsUOACZUndead)
                            continue;
                    }

                    m_Queue.Enqueue(mobile);
                }

                nearbyMobiles.Free();

                while (m_Queue.Count > 0)
                {
                    Mobile mobile = (Mobile)m_Queue.Dequeue();

                    if (!UOACZSystem.IsUOACZValidMobile(mobile)) return;
                    if (Utility.GetDistance(player.Location, mobile.Location) >= 20) return;

                    int distance = 18 - Utility.GetDistance(player.Location, mobile.Location);

                    if (distance < 1)
                        distance = 1;

                    if (distance > 10)
                        distance = 10;

                    int minImpactDamage = 10;
                    int maxImpactDamage = 20;

                    double knockbackDamage = 10;

                    double damage = (double)Utility.RandomMinMax(minImpactDamage, maxImpactDamage);
                    double damageScalar = 1;

                    if (mobile is PlayerMobile)
                    {
                        knockbackDamage *= UOACZSystem.GetFatigueScalar(player);
                        damageScalar *= UOACZSystem.GetFatigueScalar(player);
                    }

                    if (mobile is BaseCreature)
                    {
                        BaseCreature bc_Creature = mobile as BaseCreature;

                        damageScalar = 4;

                        if (bc_Creature.Controlled)
                        {
                            knockbackDamage *= UOACZSystem.GetFatigueScalar(player);
                            damageScalar *= UOACZSystem.GetFatigueScalar(player);
                        }
                    }

                    damageScalar *= UOACZPersistance.UndeadBalanceScalar * UOACZSystem.GetUndeadAbilityScalar(player);

                    damage = (Math.Round((double)damage * damageScalar));

                    UOACZSystem.PlayerInflictDamage(player, mobile, false, (int)damage);

                    knockbackDamage *= damageScalar;

                    player.DoHarmful(mobile);

                    SpecialAbilities.KnockbackSpecialAbility(1.0, player.Location, player, mobile, damage, distance, -1, "", "The beast buffets you with its wings!");

                    new Blood().MoveToWorld(mobile.Location, mobile.Map);
                    AOS.Damage(mobile, player, (int)damage, 0, 100, 0, 0, 0);
                }
            });
        }

        #endregion

        #region BoneBreath Ability

        public static void BoneBreathAbility(PlayerMobile player)
        {
            player.SendMessage("Target a location.");
            player.Target = new BoneBreathAbilityTarget();
        }

        public class BoneBreathAbilityTarget : Target
        {
            private IEntity targetLocation;

            public BoneBreathAbilityTarget(): base(25, true, TargetFlags.None, false)
            {
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile player = from as PlayerMobile;

                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!CanUseAbility(player, UOACZUndeadAbilityType.BoneBreath, true)) return;

                IPoint3D pointLocation = target as IPoint3D;

                if (pointLocation == null)
                    return;

                Map map = player.Map;

                if (map == null)
                    return;

                SpellHelper.GetSurfaceTop(ref pointLocation);

                if (pointLocation is Mobile)
                    targetLocation = (Mobile)pointLocation;

                else
                    targetLocation = new Entity(Serial.Zero, new Point3D(pointLocation), map);

                if (Utility.GetDistance(player.Location, targetLocation.Location) > 8)
                {
                    player.SendMessage("That target is too far away.");
                    return;
                }
                
                if (!player.Map.InLOS(player.Location, targetLocation.Location))
                {
                    player.SendMessage("That is not within in your line of sight.");
                    return;
                }

                double directionDelay = .25;
                double initialDelay = UOACZSystem.AbilityInitialDelaySeconds;
                double totalDelay = directionDelay + initialDelay;

                SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1, totalDelay, true, 0, false, "", "");

                player.RevealingAction();

                Point3D playerLocation = player.Location;
                Map playerMap = player.Map;

                Direction direction = Utility.GetDirection(player.Location, targetLocation.Location);
                player.Direction = direction;

                Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (!player.IsUOACZUndead) return;

                    int animation = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimation;
                    int animationFrames = player.m_UOACZAccountEntry.UndeadProfile.CastingAnimationFrames;

                    player.Animate(animation, animationFrames, 1, true, false, 0);
                    player.PlaySound(player.GetAngerSound());

                    Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
                    {
                        if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                        if (!player.IsUOACZUndead) return;

                        AbilitySuccessful(player, UOACZUndeadAbilityType.BoneBreath);

                        Point3D location = player.Location;

                        Effects.PlaySound(location, map, 0x227);

                        Dictionary<Point3D, double> m_BreathTiles = new Dictionary<Point3D, double>();
                        Dictionary<Mobile, double> m_DamagedMobiles = new Dictionary<Mobile, double>();

                        int breathSound = 0;
                        int effectSound = 0;

                        breathSound = 0x222;
                        effectSound = 0x208;

                        double tileDelay = .10;
                        int distance = 8;

                        Point3D previousPoint = location;
                        Point3D nextPoint;

                        m_BreathTiles.Add(location, 0);

                        for (int a = 0; a < distance; a++)
                        {
                            nextPoint = SpecialAbilities.GetPointByDirection(previousPoint, direction);

                            bool canFit = SpellHelper.AdjustField(ref nextPoint, map, 12, false);

                            if (canFit && map.InLOS(location, nextPoint))
                            {
                                if (!m_BreathTiles.ContainsKey(nextPoint))
                                    m_BreathTiles.Add(nextPoint, a * tileDelay);
                            }

                            List<Point3D> perpendicularPoints = SpecialAbilities.GetPerpendicularPoints(previousPoint, nextPoint, a + 1);

                            foreach (Point3D point in perpendicularPoints)
                            {
                                Point3D ppoint = new Point3D(point.X, point.Y, point.Z);

                                canFit = SpellHelper.AdjustField(ref ppoint, map, 12, false);

                                if (canFit && map.InLOS(location, ppoint))
                                {
                                    if (!m_BreathTiles.ContainsKey(ppoint))
                                        m_BreathTiles.Add(ppoint, a * tileDelay);
                                }
                            }

                            previousPoint = nextPoint;
                        }

                        foreach (KeyValuePair<Point3D, double> pair in m_BreathTiles)
                        {
                            Timer.DelayCall(TimeSpan.FromSeconds(pair.Value), delegate
                            {
                                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                                if (!player.IsUOACZUndead) return;

                                Point3D breathLocation = pair.Key;

                                int projectiles;
                                int particleSpeed;
                                double distanceDelayInterval;

                                int minRadius;
                                int maxRadius;

                                List<Point3D> m_ValidLocations = new List<Point3D>();

                                if (breathLocation != location)
                                {
                                    Effects.PlaySound(breathLocation, map, 0x11D);

                                    if (Utility.RandomDouble() <= .5)
                                    {
                                        for (int a = 0; a < 3; a++)
                                        {
                                            TimedStatic bones = new TimedStatic(Utility.RandomList(6929, 6930, 6937, 6938, 6933, 6934, 6935, 6936, 6939, 6940, 6880, 6881, 6882, 6883), 5);
                                            bones.Name = "bones";

                                            Point3D dirtLocation = new Point3D(breathLocation.X + Utility.RandomList(-1, 1), breathLocation.Y + Utility.RandomList(-1, 1), breathLocation.Z);

                                            bones.MoveToWorld(dirtLocation, map);
                                        }
                                    }

                                    projectiles = 5;
                                    particleSpeed = 8;
                                    distanceDelayInterval = .12;

                                    minRadius = 1;
                                    maxRadius = 5;

                                    m_ValidLocations = SpecialAbilities.GetSpawnableTiles(breathLocation, true, false, breathLocation, map, projectiles, 20, minRadius, maxRadius, false);

                                    if (m_ValidLocations.Count == 0)
                                        return;

                                    for (int a = 0; a < projectiles; a++)
                                    {
                                        Point3D newLocation = m_ValidLocations[Utility.RandomMinMax(0, m_ValidLocations.Count - 1)];

                                        IEntity effectStartLocation = new Entity(Serial.Zero, new Point3D(breathLocation.X, breathLocation.Y, breathLocation.Z + 2), map);
                                        IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + 50), map);

                                        newLocation.Z += 5;

                                        Effects.SendMovingEffect(effectStartLocation, effectEndLocation, Utility.RandomList(6929, 6930, 6937, 6938, 6933, 6934, 6935, 6936, 6939, 6940, 6880, 6881, 6882, 6883), particleSpeed, 0, false, false, 0, 0);
                                    }

                                    if (Utility.RandomDouble() <= .25)
                                    {
                                        IEntity locationEntity = new Entity(Serial.Zero, new Point3D(breathLocation.X, breathLocation.Y, breathLocation.Z - 1), map);
                                        Effects.SendLocationParticles(locationEntity, Utility.RandomList(0x36BD, 0x36BF, 0x36CB, 0x36BC), 30, 7, 2497, 0, 5044, 0);
                                    }
                                }

                                IPooledEnumerable mobilesOnTile = map.GetMobilesInRange(breathLocation, 0);

                                Queue m_Queue = new Queue();

                                foreach (Mobile mobile in mobilesOnTile)
                                {
                                    if (mobile == player) continue;
                                    if (!UOACZSystem.IsUOACZValidMobile(mobile)) continue;
                                    if (mobile is UOACZBaseUndead) continue;

                                    PlayerMobile playerTarget = mobile as PlayerMobile;

                                    if (playerTarget != null)
                                    {
                                        if (playerTarget.IsUOACZUndead)
                                            continue;
                                    }

                                    m_Queue.Enqueue(mobile);
                                }

                                mobilesOnTile.Free();

                                while (m_Queue.Count > 0)
                                {
                                    Mobile mobile = (Mobile)m_Queue.Dequeue();

                                    int minDamage = 15;
                                    int maxDamage = 25;

                                    double damage = (double)Utility.RandomMinMax(minDamage, maxDamage);

                                    double damageScalar = 1;

                                    if (mobile is PlayerMobile)
                                    {
                                        damageScalar *= UOACZSystem.GetFatigueScalar(player);
                                    }

                                    if (mobile is BaseCreature)
                                    {
                                        BaseCreature bc_Creature = mobile as BaseCreature;

                                        damageScalar = 4;

                                        if (bc_Creature.Controlled)
                                            damageScalar *= UOACZSystem.GetFatigueScalar(player);
                                    }

                                    damageScalar *= UOACZPersistance.UndeadBalanceScalar * UOACZSystem.GetUndeadAbilityScalar(player);

                                    damage = (Math.Round((double)damage * damageScalar));

                                    UOACZSystem.PlayerInflictDamage(player, mobile, true, (int)damage);

                                    new Blood().MoveToWorld(mobile.Location, mobile.Map);
                                    AOS.Damage(mobile, player, (int)damage, 100, 0, 0, 0, 0);
                                }
                            });
                        }
                    });
                });
            }
        }

        #endregion

        public static void AbilitySuccessful(PlayerMobile player, UOACZUndeadAbilityType abilityType)
        {
            UOACZUndeadAbilityEntry abilityEntry = UOACZUndeadAbilities.GetAbilityEntry(player.m_UOACZAccountEntry, abilityType);
            abilityEntry.m_NextUsageAllowed = DateTime.UtcNow + TimeSpan.FromMinutes(abilityEntry.m_CooldownMinutes);
            player.Mana -= UOACZSystem.AbilityManaCost;
            UOACZSystem.RefreshAllGumps(player);

            player.m_UOACZAccountEntry.UndeadProfile.NextAbilityAllowed = DateTime.UtcNow + UOACZSystem.MinimumDelayBetweenUndeadAbilities;

            player.m_UOACZAccountEntry.UndeadAbilitiesUsed++; 
        }

        public static bool CanUseAbility(PlayerMobile player, UOACZUndeadAbilityType abilityType, bool feedback)
        {
            if (player == null) return false;
            if (player.Deleted) return false;
            
            if (!player.IsUOACZUndead)
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

            if (DateTime.UtcNow < player.m_UOACZAccountEntry.UndeadProfile.NextAbilityAllowed)
            {
                if (feedback)
                {
                    string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, player.m_UOACZAccountEntry.UndeadProfile.NextAbilityAllowed, false, true, true, true, true);
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

            UOACZUndeadAbilityEntry abilityEntry = GetAbilityEntry(player.m_UOACZAccountEntry, abilityType);

            if (abilityEntry == null)            
                return false;            

            UOACZUndeadAbilityDetail abilityDetail = UOACZUndeadAbilities.GetAbilityDetail(abilityType);

            if (DateTime.UtcNow < abilityEntry.m_NextUsageAllowed)
            {
                if (feedback)
                {
                    string cooldownText = Utility.CreateTimeRemainingString(DateTime.UtcNow, abilityEntry.m_NextUsageAllowed, false, false, true, true, true);
                    player.SendMessage("That ability is not usable for " + cooldownText + ".");
                }

                return false;
            }

            UOACZUndeadUpgradeType upgradeType = player.m_UOACZAccountEntry.UndeadProfile.ActiveForm;
            UOACZUndeadUpgradeDetail upgradeDetail = UOACZUndeadUpgrades.GetUpgradeDetail(upgradeType);

            if (player.Mana < UOACZSystem.AbilityManaCost)
            {
                if (feedback)
                    player.SendMessage("You do not have enough mana to use that ability (requires 10 mana).");

                return false;
            }

            return true;
        }

        public static void ActivateAbility(PlayerMobile player, UOACZUndeadAbilityType abilityType)
        {
            if (player == null) return;
            if (player.Deleted) return;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

            if (!CanUseAbility(player, abilityType, true))
                return;

            switch (abilityType)
            {
                case UOACZUndeadAbilityType.Regeneration: RegenerationAbility(player); break;
                case UOACZUndeadAbilityType.Sacrifice: SacrificeAbility(player); break;
                case UOACZUndeadAbilityType.GiftOfCorruption: GiftOfCorruptionAbility(player); break;
                case UOACZUndeadAbilityType.Creep: CreepAbility(player); break;
                case UOACZUndeadAbilityType.Dig: DigAbility(player); break;
                case UOACZUndeadAbilityType.Rally: RallyAbility(player); break; 

                case UOACZUndeadAbilityType.RecruitZombie: RecruitZombieAbility(player); break;
                case UOACZUndeadAbilityType.LightningBolt: LightningBoltAbility(player); break;
                case UOACZUndeadAbilityType.AuraOfDecay: AuraOfDecayAbility(player); break;
                case UOACZUndeadAbilityType.BloodyMess: BloodyMessAbility(player); break;
                case UOACZUndeadAbilityType.Ignite: IgniteAbility(player); break;
                case UOACZUndeadAbilityType.Bile: BileAbility(player); break;
                case UOACZUndeadAbilityType.Combust: CombustAbility(player); break;
                case UOACZUndeadAbilityType.Virus: VirusAbility(player); break;
                case UOACZUndeadAbilityType.Plague: PlagueAbility(player); break;

                case UOACZUndeadAbilityType.RecruitSkeleton: RecruitSkeletonAbility(player); break;
                case UOACZUndeadAbilityType.ShieldOfBones: ShieldOfBonesAbility(player); break;                
                case UOACZUndeadAbilityType.Sunder: SunderAbility(player); break;
                case UOACZUndeadAbilityType.Demolish: DemolishAbility(player); break;
                case UOACZUndeadAbilityType.RestlessDead: RestlessDeadAbility(player); break;

                case UOACZUndeadAbilityType.CarrionSwarm: CarrionSwarmAbility(player); break;
                case UOACZUndeadAbilityType.ConsumeCorpse: ConsumeCorpseAbility(player); break;
                case UOACZUndeadAbilityType.CorpseBreath: CorpseBreathAbility(player); break;
                case UOACZUndeadAbilityType.FeedingFrenzy: FeedingFrenzyAbility(player); break;  
                case UOACZUndeadAbilityType.LivingBomb: LivingBombAbility(player); break;

                case UOACZUndeadAbilityType.Shadows: ShadowsAbility(player); break;
                case UOACZUndeadAbilityType.Wail: WailAbility(player); break;
                case UOACZUndeadAbilityType.GhostlyStrike: GhostlyStrikeAbility(player); break;
                case UOACZUndeadAbilityType.Dematerialize: DematerializeAbility(player); break;               
                case UOACZUndeadAbilityType.Malediction: MaledictionAbility(player); break;

                case UOACZUndeadAbilityType.Deathbolt: DeathboltAbility(player); break;
                case UOACZUndeadAbilityType.BatForm: BatFormAbility(player); break;
                case UOACZUndeadAbilityType.Transfix: TransfixAbility(player); break;
                case UOACZUndeadAbilityType.EnergyBolt: EnergyBoltAbility(player); break;
                case UOACZUndeadAbilityType.Embrace: EmbraceAbility(player); break;
                case UOACZUndeadAbilityType.Unlife: UnlifeAbility(player); break;                
                case UOACZUndeadAbilityType.ChainLightning: ChainLightningAbility(player); break;

                case UOACZUndeadAbilityType.VoidRift: VoidRiftAbility(player); break;
                case UOACZUndeadAbilityType.Enervate: EnervateAbility(player); break;                
                case UOACZUndeadAbilityType.Darkblast: DarkblastAbility(player); break;
                case UOACZUndeadAbilityType.Engulf: EngulfAbility(player); break;

                case UOACZUndeadAbilityType.WildHunt: WildHuntAbility(player); break;
                case UOACZUndeadAbilityType.Charge: ChargeAbility(player); break;               
                case UOACZUndeadAbilityType.Firebreath: FirebreathAbility(player); break;
                case UOACZUndeadAbilityType.WingBuffet: WingBuffetAbility(player); break;
                case UOACZUndeadAbilityType.BoneBreath: BoneBreathAbility(player); break;
            }
        }       

        public static UOACZUndeadAbilityDetail GetAbilityDetail(UOACZUndeadAbilityType ability)
        {
            UOACZUndeadAbilityDetail abilityDetail = new UOACZUndeadAbilityDetail();

            switch (ability)
            {
                //Starting Abilities
                case UOACZUndeadAbilityType.Regeneration:
                    abilityDetail.Name = "Regeneration";
                    abilityDetail.Description = new string[] { "Increases the player's health regen rate by 300% for the next 60 seconds"};
                    abilityDetail.CooldownMinutes = 2;
                break;

                case UOACZUndeadAbilityType.Sacrifice:
                    abilityDetail.Name = "Sacrifice";
                    abilityDetail.Description = new string[] { "Take 10% of your maximum health as damage to heal nearby followers in your swarm for 25% of their maximum health" };
                    abilityDetail.CooldownMinutes = 2;
                break;

                case UOACZUndeadAbilityType.GiftOfCorruption:
                    abilityDetail.Name = "Gift of Corruption";
                    abilityDetail.Description = new string[] { "Kill a corrupted wildlife creature to earns score and grant yourself a randomized reward" };
                    abilityDetail.CooldownMinutes = 3;
                break;

                case UOACZUndeadAbilityType.Creep:
                    abilityDetail.Name = "Creep";
                    abilityDetail.Description = new string[] { "Spread corruption by trapping a scavengeable container, debris, harvestable vegetation, cotton, ore formation, or fishing spot. Earns score and a randomized reward" };
                    abilityDetail.CooldownMinutes = 3;
                break;

                case UOACZUndeadAbilityType.Dig:
                    abilityDetail.Name = "Dig";
                    abilityDetail.Description = new string[] { "Burrow underground and reappear at a random location within the town walls" };
                    abilityDetail.CooldownMinutes = 20;
                break;

                case UOACZUndeadAbilityType.Rally:
                    abilityDetail.Name = "Rally";
                    abilityDetail.Description = new string[] { "Summon all followers in your swarm to your current location. Cannot be used if recently in combat with another player" };
                    abilityDetail.CooldownMinutes = 5;
                break;

                //Upgrade Abilities
                case UOACZUndeadAbilityType.RecruitZombie:
                    abilityDetail.Name = "Recruit Zombie";
                    abilityDetail.Description = new string[] { "Raises a zombie to join the player's swarm"};
                    abilityDetail.CooldownMinutes = 1;
                break;

                case UOACZUndeadAbilityType.LightningBolt:
                    abilityDetail.Name = "Lightning Bolt";
                    abilityDetail.Description = new string[] { "Magical ranged attack against target" };
                    abilityDetail.CooldownMinutes = .125;
                break;

                case UOACZUndeadAbilityType.AuraOfDecay:
                    abilityDetail.Name = "Aura of Decay";
                    abilityDetail.Description = new string[] { "Creates a damaging aura of decay around the player for 15 seconds"};
                    abilityDetail.CooldownMinutes = .5;
                break;

                case UOACZUndeadAbilityType.BloodyMess:
                    abilityDetail.Name = "Bloody Mess";
                    abilityDetail.Description = new string[] { "All nearby targets take bleed damage"};
                    abilityDetail.CooldownMinutes = .25;
                break;

                case UOACZUndeadAbilityType.Ignite:
                    abilityDetail.Name = "Ignite";
                    abilityDetail.Description = new string[] { "Create several firefield locations nearby"};
                    abilityDetail.CooldownMinutes = .5;
                break;

                case UOACZUndeadAbilityType.Bile:
                    abilityDetail.Name = "Bile";
                    abilityDetail.Description = new string[] { "Create several bile locations nearby"};
                    abilityDetail.CooldownMinutes = .5;
                break;

                case UOACZUndeadAbilityType.Combust:
                    abilityDetail.Name = "Combust";
                    abilityDetail.Description = new string[] { "Detonate a firefield and deal damage to nearby targets"};
                    abilityDetail.CooldownMinutes = .25;
                break;

                case UOACZUndeadAbilityType.Virus:
                    abilityDetail.Name = "Virus";
                    abilityDetail.Description = new string[] { "Detonate a bile location and inflict poison on nearby targets"};
                    abilityDetail.CooldownMinutes = .25;
                break;

                case UOACZUndeadAbilityType.Plague:
                    abilityDetail.Name = "Plague";
                    abilityDetail.Description = new string[] { "Inflicts disease on the target, causing damage over time"};
                    abilityDetail.CooldownMinutes = .5;
                break;

                case UOACZUndeadAbilityType.RecruitSkeleton:
                    abilityDetail.Name = "Recruit Skeleton";
                    abilityDetail.Description = new string[] { "Raises a skeleton that joins the player's swarm"};
                    abilityDetail.CooldownMinutes = 1;
                break;

                case UOACZUndeadAbilityType.ShieldOfBones:
                    abilityDetail.Name = "Shield of Bones";
                    abilityDetail.Description = new string[] { "All melee attack damage against the player and their followers is reduced by 25% for 15 seconds"};
                    abilityDetail.CooldownMinutes = .5;
                break;

                case UOACZUndeadAbilityType.Sunder:
                    abilityDetail.Name = "Sunder";
                    abilityDetail.Description = new string[] { "Melee attack which also reduces target's armor for 15 seconds"};
                    abilityDetail.CooldownMinutes = .25;
                break;

                case UOACZUndeadAbilityType.Demolish:
                    abilityDetail.Name = "Demolish";
                    abilityDetail.Description = new string[] { "Damages nearby creatures and heavily damages nearby breakable objects"};
                    abilityDetail.CooldownMinutes = .5;
                break;

                case UOACZUndeadAbilityType.RestlessDead:
                    abilityDetail.Name = "Restless Dead";
                    abilityDetail.Description = new string[] { "Damaging skull spikes appear under all nearby targets and any living targets that are killed will raise a skeleton in the player's swarm"};
                    abilityDetail.CooldownMinutes = .5;
                break;

                case UOACZUndeadAbilityType.CarrionSwarm:
                    abilityDetail.Name = "Carrion Swarm";
                    abilityDetail.Description = new string[] { "Target a corpse to raise 2 carrion rats that join the player's swarm" };
                    abilityDetail.CooldownMinutes = 1;
                break;

                case UOACZUndeadAbilityType.ConsumeCorpse:
                    abilityDetail.Name = "Consume Corpse";
                    abilityDetail.Description = new string[] { "Target a corpse to heal 25% of maximum hit points"};
                    abilityDetail.CooldownMinutes = .5;
                break;

                case UOACZUndeadAbilityType.CorpseBreath:
                    abilityDetail.Name = "Corpse Breath";
                    abilityDetail.Description = new string[] { "Ranged attack that entangles the target. Damage and entangle duration increased based on number of nearby corpses" };
                    abilityDetail.CooldownMinutes = .5;
                break;

                case UOACZUndeadAbilityType.FeedingFrenzy:
                    abilityDetail.Name = "Feeding Frenzy";
                    abilityDetail.Description = new string[] { "Target a corpse to increase player and nearby swarm followers attack speed and damage by 25% for 15 seconds"};
                    abilityDetail.CooldownMinutes = .5;
                break; 

                case UOACZUndeadAbilityType.LivingBomb:
                    abilityDetail.Name = "Living Bomb";
                    abilityDetail.Description = new string[] { "Target a swarm follower. If the follower is still alive after 5 seconds, it explodes dealing damage to all nearby targets" };
                    abilityDetail.CooldownMinutes = .5;
                break;

                case UOACZUndeadAbilityType.Shadows:
                    abilityDetail.Name = "Shadows";
                    abilityDetail.Description = new string[] { "Immediately hide, gain 500 infailable stealth steps, and raise a spectre which joins your swarm"};
                    abilityDetail.CooldownMinutes = 2;
                break;

                case UOACZUndeadAbilityType.Wail:
                    abilityDetail.Name = "Wail";
                    abilityDetail.Description = new string[] { "Deal damage to all nearby targets regardless of line of sight"};
                    abilityDetail.CooldownMinutes = .25;
                break;

                case UOACZUndeadAbilityType.GhostlyStrike:
                    abilityDetail.Name = "Ghostly Strike";
                    abilityDetail.Description = new string[] { "Melee attack that deals additional damage if you are hidden" };
                    abilityDetail.CooldownMinutes = .25;
                break;

                case UOACZUndeadAbilityType.Dematerialize:
                    abilityDetail.Name = "Dematerialize";
                    abilityDetail.Description = new string[] { "Target a gate door to teleport yourself and your swarm past it. Will not reveal the player or the swarm if hidden" };
                    abilityDetail.CooldownMinutes = .5;
                break;                

                case UOACZUndeadAbilityType.Malediction:
                    abilityDetail.Name = "Malediction";
                    abilityDetail.Description = new string[] { "Reduces the attack speed of nearby targets for 15 seconds"};
                    abilityDetail.CooldownMinutes = .5;
                break;

                case UOACZUndeadAbilityType.Deathbolt:
                    abilityDetail.Name = "Deathbolt";
                    abilityDetail.Description = new string[] { "Ranged attack that raises a creature that joins the player's swarm if it kills a living target"};
                    abilityDetail.CooldownMinutes = .125;
                break;

                case UOACZUndeadAbilityType.Transfix:
                    abilityDetail.Name = "Transfix";
                    abilityDetail.Description = new string[] { "Melee attack that petrifies target" };
                    abilityDetail.CooldownMinutes = .25;
                break;

                case UOACZUndeadAbilityType.BatForm:
                    abilityDetail.Name = "Bat Form";
                    abilityDetail.Description = new string[] { "Teleport to target location and raises a giant bat at the player's original location that joins their swarm"};
                    abilityDetail.CooldownMinutes = 1;
                break;

                case UOACZUndeadAbilityType.EnergyBolt:
                    abilityDetail.Name = "Energy Bolt";
                    abilityDetail.Description = new string[] { "Ranged attack against target" };
                    abilityDetail.CooldownMinutes = .25;
                break;

                case UOACZUndeadAbilityType.Embrace:
                    abilityDetail.Name = "Embrace";
                    abilityDetail.Description = new string[] { "Melee attack that adds a skeletal knight to the player's swarm if it kills a living target"};
                    abilityDetail.CooldownMinutes = .25;
                break;

                case UOACZUndeadAbilityType.Unlife:
                    abilityDetail.Name = "Unlife";
                    abilityDetail.Description = new string[] { "Restores health to all other nearby undead"};
                    abilityDetail.CooldownMinutes = .5;
                break;

                case UOACZUndeadAbilityType.ChainLightning:
                    abilityDetail.Name = "Chain Lightning";
                    abilityDetail.Description = new string[] { "Ranged attack against targets near target location"};
                    abilityDetail.CooldownMinutes = .5;
                break;

                case UOACZUndeadAbilityType.VoidRift:
                    abilityDetail.Name = "Void Rift";
                    abilityDetail.Description = new string[] { "Create a damaging void at target location, which raises a void slime into the player's swarm after 30 seconds" };
                    abilityDetail.CooldownMinutes = 1;
                break;


                case UOACZUndeadAbilityType.Enervate:
                    abilityDetail.Name = "Enervate";
                    abilityDetail.Description = new string[] { "Ranged attack that reduces all ability cooldowns by 20% if it kills a living target"};
                    abilityDetail.CooldownMinutes = .125;
                break;

                case UOACZUndeadAbilityType.Darkblast:
                    abilityDetail.Name = "Darkblast";
                    abilityDetail.Description = new string[] { "Damage targets near target location after 2 second delay"};
                    abilityDetail.CooldownMinutes = .5;
                break;

                case UOACZUndeadAbilityType.Engulf:
                    abilityDetail.Name = "Engulf";
                    abilityDetail.Description = new string[] { "Teleport all nearby targets to player's location and damages them"};
                    abilityDetail.CooldownMinutes = .5;
                break;

                case UOACZUndeadAbilityType.WildHunt:
                    abilityDetail.Name = "Wild Hunt";
                    abilityDetail.Description = new string[] { "Raises a skeletal critter which joins the player's swarm" };
                    abilityDetail.CooldownMinutes = 1;
                break;

                case UOACZUndeadAbilityType.Charge:
                    abilityDetail.Name = "Charge";
                    abilityDetail.Description = new string[] { "Teleport to target and immediately attack them, knocking them back a small distance"};
                    abilityDetail.CooldownMinutes = .25;
                break;                

                case UOACZUndeadAbilityType.Firebreath:
                    abilityDetail.Name = "Firebreath";
                    abilityDetail.Description = new string[] { "Ranged attack that unleashes a barrage of fire against the target"};
                    abilityDetail.CooldownMinutes = .25;
                break;

                case UOACZUndeadAbilityType.WingBuffet:
                    abilityDetail.Name = "Wing Buffet";
                    abilityDetail.Description = new string[] { "Damages all nearby targets and inflicts knockback on each"};
                    abilityDetail.CooldownMinutes = .5;
                break;

                case UOACZUndeadAbilityType.BoneBreath:
                    abilityDetail.Name = "Bone Breath";
                    abilityDetail.Description = new string[] { "Damages targets in a large cone in front of player"};
                    abilityDetail.CooldownMinutes = .5;
                break;
            } 

            return abilityDetail;
        }

        public static UOACZUndeadAbilityEntry GetAbilityEntry(UOACZAccountEntry playerEntry, UOACZUndeadAbilityType abilityType)
        {
            UOACZUndeadAbilityEntry abilityEntry = null;

            foreach (UOACZUndeadAbilityEntry entry in playerEntry.UndeadProfile.m_Abilities)
            {
                if (entry.m_AbilityType == abilityType)
                    return entry;
            }

            return abilityEntry;
        }
    }

    public class UOACZUndeadAbilityDetail
    {
        public string Name = "Ability Detail";
        public string[] Description = new string[] { };

        public double CooldownMinutes = 1;
    }

    public class UOACZUndeadAbilityEntry
    {
        public UOACZUndeadAbilityType m_AbilityType = UOACZUndeadAbilityType.RecruitSkeleton;      
        public double m_CooldownMinutes = 3;
        public DateTime m_NextUsageAllowed = DateTime.UtcNow;

        public UOACZUndeadAbilityEntry(UOACZUndeadAbilityType abilityType, double cooldownMinutes, DateTime nextUsageAllowed)
        {
            m_AbilityType = abilityType;          
            m_CooldownMinutes = cooldownMinutes;
            m_NextUsageAllowed = nextUsageAllowed;
        }
    }
}