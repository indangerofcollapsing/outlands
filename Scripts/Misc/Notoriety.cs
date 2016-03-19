using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Guilds;
using Server.Multis;
using Server.Mobiles;
using Server.Engines.PartySystem;
using Server.Spells.Necromancy;
using Server.Spells.Ninjitsu;
using Server.Spells;
using Server.Bounty;
using Server.ArenaSystem;

using Server.Custom.Battlegrounds;
using Server.Custom.Battlegrounds.Regions;
using Server.Network;

namespace Server.Misc
{
    public class NotorietyHandlers
    {
        public static void Initialize()
        {
            Notoriety.Hues[Notoriety.Innocent] = 0x59;
            Notoriety.Hues[Notoriety.Ally] = 0x3F;
            Notoriety.Hues[Notoriety.CanBeAttacked] = 0x3B2;
            Notoriety.Hues[Notoriety.Criminal] = 0x3B2;
            Notoriety.Hues[Notoriety.Enemy] = 0x90;
            Notoriety.Hues[Notoriety.Murderer] = 0x22;
            Notoriety.Hues[Notoriety.Invulnerable] = 0x35;

            Notoriety.Handler = new NotorietyHandler(MobileNotoriety);

            Mobile.AllowBeneficialHandler = new AllowBeneficialHandler(Mobile_AllowBeneficial);
            Mobile.AllowHarmfulHandler = new AllowHarmfulHandler(Mobile_AllowHarmful);
        }

        private enum GuildStatus { None, Peaceful, Waring }

        private static GuildStatus GetGuildStatus(Mobile m)
        {
            if (m.Guild == null)
                return GuildStatus.None;
            else if (((Guild)m.Guild).Enemies.Count == 0 && m.Guild.Type == GuildType.Regular)
                return GuildStatus.Peaceful;

            return GuildStatus.Waring;
        }

        private static bool CheckBeneficialStatus(GuildStatus from, GuildStatus target)
        {
            if (from == GuildStatus.Waring || target == GuildStatus.Waring)
                return false;

            return true;
        }

        public static bool Mobile_AllowBeneficial(Mobile from, Mobile target)
        {
            if (from == null || target == null || from.AccessLevel > AccessLevel.Player || target.AccessLevel > AccessLevel.Player)
                return true;

            PlayerMobile pm_From = from as PlayerMobile;
            PlayerMobile pm_Target = target as PlayerMobile;
            BaseCreature bc_Target = target as BaseCreature;

            Map map = from.Map;

            #region UOACZ

            if (from.Region is UOACZRegion && target.Region is UOACZRegion)
                return true;

            #endregion

            #region ConPVP / Arenas / Battlegrounds
            PlayerMobile pmFrom = from as PlayerMobile;
            PlayerMobile pmTarg = target as PlayerMobile;

            if (pmFrom == null && from is BaseCreature)
            {
                BaseCreature bcFrom = (BaseCreature)from;

                if (bcFrom.Summoned)
                    pmFrom = bcFrom.SummonMaster as PlayerMobile;
            }

            if (pmTarg == null && target is BaseCreature)
            {
                BaseCreature bcTarg = (BaseCreature)target;

                if (bcTarg.Summoned)
                    pmTarg = bcTarg.SummonMaster as PlayerMobile;
            }

            if (pmFrom != null && pmTarg != null)
            {
                if (pmFrom.Region is BattlegroundRegion)
                    return Battleground.AllowBeneficial(pmFrom, pmTarg);

                if (pmFrom.IsInArenaFight && pmTarg.IsInArenaFight)
                    return ArenaSystem.ArenaSystem.AllowBeneficial(pmFrom, pmTarg);

                if (pmFrom.DuelContext != pmTarg.DuelContext && ((pmFrom.DuelContext != null && pmFrom.DuelContext.Started) || (pmTarg.DuelContext != null && pmTarg.DuelContext.Started)))
                    return false;

                if (pmFrom.DuelContext != null && pmFrom.DuelContext == pmTarg.DuelContext && ((pmFrom.DuelContext.StartedReadyCountdown && !pmFrom.DuelContext.Started) || pmFrom.DuelContext.Tied || pmFrom.DuelPlayer.Eliminated || pmTarg.DuelPlayer.Eliminated))
                    return false;

                if (pmFrom.DuelPlayer != null && !pmFrom.DuelPlayer.Eliminated && pmFrom.DuelContext != null && pmFrom.DuelContext.IsSuddenDeath)
                    return false;

                if (pmFrom.DuelContext != null && pmFrom.DuelContext == pmTarg.DuelContext && pmFrom.DuelContext.m_Tournament != null && pmFrom.DuelContext.m_Tournament.IsNotoRestricted && pmFrom.DuelPlayer != null && pmTarg.DuelPlayer != null && pmFrom.DuelPlayer.Participant != pmTarg.DuelPlayer.Participant)
                    return false;

                if (pmFrom.DuelContext != null && pmFrom.DuelContext == pmTarg.DuelContext && pmFrom.DuelContext.Started)
                    return true;
            }

            if ((pmFrom != null && pmFrom.DuelContext != null && pmFrom.DuelContext.Started) || (pmTarg != null && pmTarg.DuelContext != null && pmTarg.DuelContext.Started))
                return false;

            Engines.ConPVP.SafeZone sz = from.Region.GetRegion(typeof(Engines.ConPVP.SafeZone)) as Engines.ConPVP.SafeZone;

            if (sz != null /*&& sz.IsDisabled()*/ )
                return false;

            sz = target.Region.GetRegion(typeof(Engines.ConPVP.SafeZone)) as Engines.ConPVP.SafeZone;

            if (sz != null /*&& sz.IsDisabled()*/ )
                return false;
            #endregion
            
            //Young Player Handling
            bool fromYoung = from is PlayerMobile && ((PlayerMobile)from).Young;

            if (fromYoung)
            {
                bool self = target == from;
                bool targetIsYoung = target is PlayerMobile && ((PlayerMobile)target).Young;
                bool youngPet = target is BaseCreature &&
                                ((BaseCreature)target).ControlMaster is PlayerMobile &&
                                ((PlayerMobile)((BaseCreature)target).ControlMaster).Young;

                if (self || youngPet || targetIsYoung)
                    return true;

                else
                    return false; // Young players cannot perform beneficial actions towards older players
            }            

            if (map != null && (map.Rules & MapRules.BeneficialRestrictions) == 0)
                return true; // In felucca, anything goes

            if (!from.Player)
                return true; // NPCs have no restrictions

            if (target is BaseCreature && !((BaseCreature)target).Controlled)
                return false; // Players cannot heal uncontrolled mobiles

            Guild fromGuild = from.Guild as Guild;
            Guild targetGuild = target.Guild as Guild;

            if (fromGuild != null && targetGuild != null && (targetGuild == fromGuild || fromGuild.IsAlly(targetGuild)))
                return true; // Guild members can be beneficial

            return CheckBeneficialStatus(GetGuildStatus(from), GetGuildStatus(target));
        }

        public static bool Mobile_AllowHarmful(Mobile from, Mobile target)
        {
            if (from == null || target == null || from.AccessLevel > AccessLevel.Player || target.AccessLevel > AccessLevel.Player)
                return true;

            PlayerMobile pm_From = from as PlayerMobile;
            PlayerMobile pm_Target = target as PlayerMobile;
            BaseCreature bc_Target = target as BaseCreature;

            Map map = from.Map;

            #region Youngs inFelucca
            if (from.Player && target.Player && (((PlayerMobile)target).Young || ((PlayerMobile)from).Young) && !(target.Criminal || from.Criminal))
                return false;   // Old players cannot attack youngs and vice versa unless young is crim
            #endregion

            if (map != null && ((map.Rules & MapRules.BeneficialRestrictions) == 0) || Server.Spells.SpellHelper.IsFireDungeon(target.Map, target.Location))
                return true; // In felucca, anything goes. Special case fire dungeon as players can access it in fel or trammel under fel rules

            BaseCreature bc = from as BaseCreature;

            if (!from.Player && !(bc != null && bc.GetMaster() != null && bc.GetMaster().AccessLevel == AccessLevel.Player))
            {
                if (!CheckAggressor(from.Aggressors, target) && !CheckAggressed(from.Aggressed, target) && target is PlayerMobile && ((PlayerMobile)target).CheckYoungProtection(from))
                    return false;

                return true; // Uncontrolled NPCs are only restricted by the young system
            }

            Guild fromGuild = GetGuildFor(from.Guild as Guild, from);
            Guild targetGuild = GetGuildFor(target.Guild as Guild, target);

            if (fromGuild != null && targetGuild != null && (fromGuild == targetGuild || fromGuild.IsAlly(targetGuild) || fromGuild.IsEnemy(targetGuild)))
                return true; // Guild allies or enemies can be harmful                      

            if (target is BaseCreature && (((BaseCreature)target).Controlled || (((BaseCreature)target).Summoned && from != ((BaseCreature)target).SummonMaster)))
                return false; // Cannot harm other controlled mobiles

            if (target.Player)
                return false; // Cannot harm other players

            if (!(target is BaseCreature && ((BaseCreature)target).InitialInnocent))
            {
                if (Notoriety.Compute(from, target) == Notoriety.Innocent)
                    return false; // Cannot harm innocent mobiles
            }

            return true;
        }

        public static Guild GetGuildFor(Guild def, Mobile m)
        {
            Guild g = def;

            BaseCreature c = m as BaseCreature;

            if (c != null && c.Controlled && c.ControlMaster != null)
            {
                c.DisplayGuildTitle = false;

                if (c.Map != Map.Internal && (Core.AOS || Guild.NewGuildSystem || c.ControlOrder == OrderType.Attack || c.ControlOrder == OrderType.Guard))
                    g = (Guild)(c.Guild = c.ControlMaster.Guild);
                else if (c.Map == Map.Internal || c.ControlMaster.Guild == null)
                    g = (Guild)(c.Guild = null);
            }

            return g;
        }

        public static int CorpseNotoriety(Mobile source, Corpse target)
        {
            if (target.AccessLevel > AccessLevel.Player)
                return Notoriety.CanBeAttacked;

            #region UOACZ

            if (UOACZRegion.ContainsItem(target))
            {
                PlayerMobile pm_Owner = target.Owner as PlayerMobile;
                BaseCreature bc_Owner = target.Owner as BaseCreature;                

                if (pm_Owner != null)
                {
                    UOACZPersistance.CheckAndCreateUOACZAccountEntry(pm_Owner);

                    if (pm_Owner.IsUOACZUndead)
                        return Notoriety.Murderer;

                    if (pm_Owner.m_UOACZAccountEntry.ActiveProfile == UOACZAccountEntry.ActiveProfileType.Human)
                    {
                        if (pm_Owner.m_UOACZAccountEntry.HumanProfile.HonorPoints <= UOACZSystem.HonorAggressionThreshold)
                            return Notoriety.Enemy;

                        if (pm_Owner.Criminal)
                            return Notoriety.CanBeAttacked;

                        return Notoriety.Innocent;
                    }  
                }

                if (bc_Owner != null)
                {
                    if (bc_Owner is UOACZBaseUndead)
                    {
                        if (bc_Owner.ControlMaster == source)
                            return Notoriety.Ally;

                        return Notoriety.CanBeAttacked;
                    }

                    if (bc_Owner is UOACZBaseWildlife)
                        return Notoriety.CanBeAttacked;

                    if (bc_Owner is UOACZBaseHuman)
                        return Notoriety.Innocent;
                }

                return Notoriety.CanBeAttacked;
            }

            #endregion

            Body body = (Body)target.Amount;

            BaseCreature cretOwner = target.Owner as BaseCreature;

            if (cretOwner != null)
            {
                Guild sourceGuild = GetGuildFor(source.Guild as Guild, source);
                Guild targetGuild = GetGuildFor(target.Guild as Guild, target.Owner);

                if (sourceGuild != null && targetGuild != null)
                {
                    if (sourceGuild == targetGuild || sourceGuild.IsAlly(targetGuild))
                        return Notoriety.Ally;

                    else if (sourceGuild.IsEnemy(targetGuild) && !IsPaladin(source, null))
                        return Notoriety.Enemy;
                }

                if (cretOwner.IsLoHBoss() || cretOwner.FreelyLootable)
                    return Notoriety.CanBeAttacked;

                if (CheckHouseFlag(source, target.Owner, target.Location, target.Map))
                    return Notoriety.CanBeAttacked;

                int actual = Notoriety.CanBeAttacked;

                if (target.Kills >= 5 || (body.IsMonster && IsSummoned(target.Owner as BaseCreature)) || (target.Owner is BaseCreature && (((BaseCreature)target.Owner).IsMurderer() || ((BaseCreature)target.Owner).IsAnimatedDead)))
                    actual = Notoriety.Murderer;

                if (DateTime.UtcNow >= (target.TimeOfDeath + Corpse.MonsterLootRightSacrifice))
                    return actual;

                Party sourceParty = Party.Get(source);

                List<Mobile> list = target.Aggressors;

                for (int i = 0; i < list.Count; ++i)
                {
                    if (list[i] == source || (sourceParty != null && Party.Get(list[i]) == sourceParty))
                        return actual;
                }

                return Notoriety.Innocent;
            }

            else
            {
                if (target.Kills >= 5 || (body.IsMonster && IsSummoned(target.Owner as BaseCreature)) || (target.Owner is BaseCreature && (((BaseCreature)target.Owner).IsMurderer() || ((BaseCreature)target.Owner).IsAnimatedDead)))
                    return Notoriety.Murderer;

                if (target.Criminal)
                    return Notoriety.Criminal;
                
                Guild sourceGuild = GetGuildFor(source.Guild as Guild, source);
                Guild targetGuild = GetGuildFor(target.Guild as Guild, target.Owner);

                if (sourceGuild != null && targetGuild != null)
                {
                    if (sourceGuild == targetGuild || sourceGuild.IsAlly(targetGuild))
                        return Notoriety.Ally;
                    else if (sourceGuild.IsEnemy(targetGuild) && !IsPaladin(source, target.Owner))
                        return Notoriety.Enemy;
                }

                if (target.Owner != null && target.Owner is BaseCreature && ((BaseCreature)target.Owner).AlwaysAttackable)
                    return Notoriety.CanBeAttacked;

                if (CheckHouseFlag(source, target.Owner, target.Location, target.Map))
                    return Notoriety.CanBeAttacked;

                if (!(target.Owner is PlayerMobile) && !IsPet(target.Owner as BaseCreature))
                    return Notoriety.CanBeAttacked;

                List<Mobile> list = target.Aggressors;

                for (int i = 0; i < list.Count; ++i)
                {
                    if (list[i] == source)
                        return Notoriety.CanBeAttacked;
                }

                if (SpellHelper.InBuccs(target.Map, target.Location) || SpellHelper.InYewOrcFort(target.Map, target.Location) || SpellHelper.InYewCrypts(target.Map, target.Location))
                    return Notoriety.CanBeAttacked;

                if (GreyZoneTotem.InGreyZoneTotemArea(target.Location, target.Map))
                    return Notoriety.CanBeAttacked;

                //Hotspot Nearby
                if (Custom.Hotspot.InHotspotArea(target.Location, target.Map, true))
                    return Notoriety.CanBeAttacked;

                if (target.m_IsBones)
                    return Notoriety.CanBeAttacked;

                return Notoriety.Innocent;
            }
        }

        public static int MobileNotoriety(Mobile source, Mobile target)
        {
            return DetermineMobileNotoriety(source, target, true);
        }

        public static int DetermineMobileNotoriety(Mobile source, Mobile target, bool useVengeance)
        {  
            BaseCreature bc_Source = source as BaseCreature;
            PlayerMobile pm_Source = source as PlayerMobile;

            Mobile m_SourceController = null;
            BaseCreature bc_SourceController = null;
            PlayerMobile pm_SourceController = null;

            BaseCreature bc_Target = target as BaseCreature;
            PlayerMobile pm_Target = target as PlayerMobile;

            Mobile m_TargetController = null;
            BaseCreature bc_TargetController = null;
            PlayerMobile pm_TargetController = null;

            #region UOACZ

            if (UOACZRegion.ContainsMobile(target))
            {
                if (pm_Target != null)
                {
                    UOACZPersistance.CheckAndCreateUOACZAccountEntry(pm_Target);

                    if (pm_Target.IsUOACZUndead)
                    {
                        if (pm_Source != null)
                        {
                            if (pm_Source.IsUOACZHuman)
                                return Notoriety.Enemy;

                            if (pm_Source.IsUOACZUndead)
                            {
                                if (pm_Target.Criminal)
                                    return Notoriety.CanBeAttacked;

                                return Notoriety.Innocent;
                            }
                        }

                        return Notoriety.CanBeAttacked;
                    }

                    if (pm_Target.IsUOACZHuman)
                    {
                        if (pm_Source != null)
                        {
                            if (pm_Source.IsUOACZUndead)
                                return Notoriety.Enemy;
                        }

                        if (pm_Target.m_UOACZAccountEntry.HumanProfile.HonorPoints <= UOACZSystem.HonorAggressionThreshold)
                            return Notoriety.Enemy;

                        if (pm_Target.Criminal)
                            return Notoriety.CanBeAttacked;

                        if (pm_Source != null)                        
                            return Notoriety.Innocent;                        

                        return Notoriety.CanBeAttacked;
                    }                    
                }

                if (bc_Target != null)
                {
                    if (bc_Target is UOACZBaseUndead)
                    {
                        UOACZBaseUndead bc_Undead = bc_Target as UOACZBaseUndead;

                        if (bc_Undead.ControlMaster == source)
                            return Notoriety.Ally;

                        if (pm_Source != null && (bc_Undead == UOACZPersistance.UndeadChampion || bc_Undead == UOACZPersistance.UndeadBoss))
                        {
                            if (pm_Source.IsUOACZHuman)
                                return Notoriety.Murderer;
                        }

                        return Notoriety.CanBeAttacked;
                    }

                    if (bc_Target is UOACZBaseWildlife)
                        return Notoriety.CanBeAttacked;

                    if (bc_Target is UOACZBaseHuman)
                    {
                        UOACZBaseHuman bc_Human = bc_Target as UOACZBaseHuman;

                        if (source is UOACZBaseWildlife || source is UOACZBaseUndead)
                            return Notoriety.CanBeAttacked;

                        if (pm_Source != null)
                        {
                            if (pm_Source.IsUOACZUndead)
                            {
                                if (bc_Human == UOACZPersistance.HumanChampion || bc_Human == UOACZPersistance.HumanBoss)
                                    return Notoriety.Murderer;

                                return Notoriety.CanBeAttacked;
                            }
                        }
                        
                        return Notoriety.Innocent;
                    }
                }

                return Notoriety.Innocent;
            }

            #endregion

            if (bc_Source != null)
            {
                m_SourceController = bc_Source.ControlMaster as Mobile;
                bc_SourceController = bc_Source.ControlMaster as BaseCreature;
                pm_SourceController = bc_Source.ControlMaster as PlayerMobile;
            }

            if (bc_Target != null)
            {
                m_TargetController = bc_Target.ControlMaster as Mobile;
                bc_TargetController = bc_Target.ControlMaster as BaseCreature;
                pm_TargetController = bc_Target.ControlMaster as PlayerMobile;
            }           

            //Berserk Creatures
            if (bc_Source != null && (source is BladeSpirits || source is EnergyVortex))
            {
                if (bc_Source.ControlMaster != null && pm_Target != null)
                {
                    //Blade Spirits + Energy Vortexes Can Freely Attack Their Control Master Without Causing Criminal Action
                    if (bc_Source.ControlMaster == pm_Target)
                        return Notoriety.CanBeAttacked;
                }

                if (bc_Source.ControlMaster != null && bc_Target != null)
                {
                    //Blade Spirits + Energy Vortexes Can Freely Attack Other Followers Of Their Control Master Without Causing Criminal Action
                    if (bc_Source.ControlMaster == bc_Target.ControlMaster) 
                        return Notoriety.CanBeAttacked;
                }                
            }
                        
            if (target is BladeSpirits || target is EnergyVortex)
                return Notoriety.Murderer;

            //Arena
            int result = ArenaSystem.ArenaSystem.GetNotoriety(source, target);

            if (result == Notoriety.Enemy || result == Notoriety.Ally)
                return result;

            //Staff Members Always Attackable
            if (target.AccessLevel > AccessLevel.Player)
                return Notoriety.CanBeAttacked;

            if (m_TargetController != null)
            {
                //Creature Controlled By Staff Member
                if (m_TargetController.AccessLevel > AccessLevel.Player)
                    return Notoriety.CanBeAttacked;
            }

            //Consensual PvP
            if (pm_Source != null && pm_Target != null)
            {
                //Duel
                if (pm_Source.DuelContext != null && pm_Source.DuelContext.StartedBeginCountdown && !pm_Source.DuelContext.Finished && pm_Source.DuelContext == pm_Target.DuelContext)
                    return pm_Source.DuelContext.IsAlly(pm_Source, pm_Target) ? Notoriety.Ally : Notoriety.Enemy;

                //Battlegrounds
                if (pm_Source.Region is BattlegroundRegion)
                    return Battleground.AllowBeneficial(pm_Source, pm_Target) ? Notoriety.Ally : Notoriety.Enemy;
            }

            //Enemy of One
            if (pm_Source != null && bc_Target != null)
            {
                if (!bc_Target.Summoned && !bc_Target.Controlled && pm_Source.EnemyOfOneType == target.GetType())
                    return Notoriety.Enemy;
            }
            
            //Justice Free Zone
            if (SpellHelper.InBuccs(target.Map, target.Location) || SpellHelper.InYewOrcFort(target.Map, target.Location) || SpellHelper.InYewCrypts(target.Map, target.Location))
                return Notoriety.CanBeAttacked;

            //Grey Zone Totem Nearby
            if (GreyZoneTotem.InGreyZoneTotemArea(target.Location, target.Map))
                    return Notoriety.CanBeAttacked;

            //Hotspot Nearby
            if (Custom.Hotspot.InHotspotArea(target.Location, target.Map, true))
                return Notoriety.CanBeAttacked;

            //Player Notoriety
            if (pm_Target != null)
            {
                //Friendly
                if (pm_SourceController != null)
                {
                    if (pm_SourceController == pm_Target)                    
                        return Notoriety.Ally;                    
                }

                //Murderer
                if (pm_Target.Murderer && !pm_Target.HideMurdererStatus)                
                    return Notoriety.Murderer;                

                //Criminal
                if (pm_Target.Criminal)                
                    return Notoriety.Criminal;                

                //Perma-Grey
                if (SkillHandlers.Stealing.ClassicMode && pm_Target.PermaFlags.Contains(source))                
                    return Notoriety.CanBeAttacked;                

                if (pm_SourceController != null)
                {
                    //Target is Perma-Grey to Source Creature's Controller
                    if (SkillHandlers.Stealing.ClassicMode && pm_Target.PermaFlags.Contains(pm_SourceController))                    
                        return Notoriety.CanBeAttacked;                    
                }

                //Paladin (Murderers Can Freely Attack Paladins)
                if (pm_Target.Paladin)
                {
                    if (pm_Source != null)
                    {
                        if (pm_Source.Murderer)                        
                            return Notoriety.CanBeAttacked;                        
                    }

                    if (pm_SourceController != null)
                    {
                        //Source Creature's Controller is Murderer
                        if (pm_SourceController.Murderer)                        
                            return Notoriety.CanBeAttacked;                        
                    }
                }
            }

            //Guilds
            Guild sourceGuild = GetGuildFor(source.Guild as Guild, source);
            Guild targetGuild = GetGuildFor(target.Guild as Guild, target);

            if (sourceGuild != null && targetGuild != null)
            {
                if (sourceGuild == targetGuild || sourceGuild.IsAlly(targetGuild))
                    return Notoriety.Ally;

                else if (sourceGuild.IsEnemy(targetGuild) && !IsPaladin(source, target))
                    return Notoriety.Enemy;
            }

            //Creature Notoriety
            if (bc_Target != null)
            {
                //Friendly
                if (m_TargetController != null)
                {
                    //Target is Source's Controller
                    if (source == m_TargetController)                    
                        return Notoriety.Ally;                    
                }

                if (m_SourceController != null)
                {
                    //Source is Target's Controller
                    if (m_SourceController == bc_Target)                    
                        return Notoriety.Ally;                    
                }

                //Murderer
                if (bc_Target.IsMurderer())                
                    return Notoriety.Murderer;                

                if (pm_TargetController != null)
                {
                    if (pm_TargetController.Murderer)                    
                        return Notoriety.Murderer;                    
                }

                if (bc_TargetController != null)
                {
                    if (bc_TargetController.IsMurderer())                    
                        return Notoriety.Murderer;                    
                }

                //Criminal
                if (bc_Target.Criminal)                
                    return Notoriety.Criminal;                

                if (pm_TargetController != null)
                {
                    if (pm_TargetController.Criminal)                    
                        return Notoriety.Criminal;                    
                }

                if (bc_TargetController != null)
                {
                    if (bc_TargetController.Criminal)                    
                        return Notoriety.Criminal;                    
                }

                //Perma-Grey
                if (pm_TargetController != null)
                {
                    if (SkillHandlers.Stealing.ClassicMode && pm_TargetController.PermaFlags.Contains(source))                    
                        return Notoriety.CanBeAttacked;                    

                    if (pm_SourceController != null)
                    {
                        //Target is Perma-Grey to Source Creature's Controller
                        if (SkillHandlers.Stealing.ClassicMode && pm_TargetController.PermaFlags.Contains(pm_SourceController))                        
                            return Notoriety.CanBeAttacked;                        
                    }
                }

                //Paladin (Murderers Can Freely Attack Paladins)
                if (pm_TargetController != null)
                {
                    if (pm_TargetController.Paladin)
                    {
                        if (pm_Source != null)
                        {
                            if (pm_Source.Murderer)                            
                                return Notoriety.CanBeAttacked;                            
                        }

                        if (pm_SourceController != null)
                        {
                            //Source Creature's Controller is Murderer
                            if (pm_SourceController.Murderer)                            
                                return Notoriety.CanBeAttacked;                            
                        }
                    }
                }                
            }

            //Housing
            if (CheckHouseFlag(source, target, target.Location, target.Map))
                return Notoriety.CanBeAttacked;

            //Aggressor: Source to Target
            if (CheckAggressor(source.Aggressors, target))
                return Notoriety.CanBeAttacked;

            if (CheckAggressed(source.Aggressed, target) && useVengeance)
                return Notoriety.CanBeAttacked;

            //Aggressor: Source Controller to Target
            if (m_SourceController != null)
            {
                if (CheckAggressor(m_SourceController.Aggressors, target))
                    return Notoriety.CanBeAttacked;

                if (CheckAggressed(m_SourceController.Aggressed, target) && useVengeance)
                    return Notoriety.CanBeAttacked;
            }

            //Aggressor: Source to Target's Controller
            if (m_TargetController != null)
            {
                if (CheckAggressor(source.Aggressors, m_TargetController))
                    return Notoriety.CanBeAttacked;

                if (CheckAggressed(source.Aggressed, m_TargetController) && useVengeance)
                    return Notoriety.CanBeAttacked;
            }

            //Aggressor: Source Controller to Target's Controller
            if (m_SourceController != null && m_TargetController != null)
            {
                if (CheckAggressor(m_SourceController.Aggressors, m_TargetController))
                    return Notoriety.CanBeAttacked;

                if (CheckAggressed(m_SourceController.Aggressed, m_TargetController) && useVengeance)
                    return Notoriety.CanBeAttacked;
            }

            //Player Followers: If A Player or Any of Their Followers Have been Aggressed or Barded, the Player and All Other Followers Can Attack the Aggressor
            PlayerMobile pm_Player = null;

            if (pm_Source != null)
                pm_Player = pm_Source;

            if (pm_SourceController != null)
                pm_Player = pm_SourceController;

            if (pm_Player != null)
            {
                if (pm_Player.AllFollowers.Count > 0)
                {
                    //Any of the Player's Other Followers
                    foreach (Mobile follower in pm_Player.AllFollowers)
                    {
                        BaseCreature bc_Follower = follower as BaseCreature;

                        if (bc_Follower == null)
                            continue;

                        //Follower Has Been Aggressed/Aggresor to Target
                        if (CheckAggressor(bc_Follower.Aggressors, target))
                            return Notoriety.CanBeAttacked;

                        if (CheckAggressed(bc_Follower.Aggressed, target) && useVengeance)
                            return Notoriety.CanBeAttacked;

                        //Follower Has Been Aggressed/Aggresor by/to Target's Controller
                        if (m_TargetController != null)
                        {
                            if (CheckAggressor(bc_Follower.Aggressors, m_TargetController))
                                return Notoriety.CanBeAttacked;

                            if (CheckAggressed(bc_Follower.Aggressed, m_TargetController) && useVengeance)
                                return Notoriety.CanBeAttacked;
                        }
                    }
                }
            }

            //Boats: Players and Creatures Friendly to a Boat Can Freely Attack Non-Friendly Mobiles on their Boat
            BaseBoat sourceBoat = null;

            if (bc_Source != null)
            {
                if (bc_Source.BoatOccupied != null)
                    sourceBoat = bc_Source.BoatOccupied;
            }

            if (pm_Source != null)
            {
                if (pm_Source.BoatOccupied != null)
                    sourceBoat = pm_Source.BoatOccupied;
            }

            if (sourceBoat != null)
            {
                BaseBoat targetBoat = null;

                if (bc_Target != null)
                {
                    if (bc_Target.BoatOccupied != null)
                        targetBoat = bc_Target.BoatOccupied;
                }

                if (pm_Target != null)
                {
                    if (pm_Target.BoatOccupied != null)
                        targetBoat = pm_Target.BoatOccupied;
                }

                //On Same Boat
                if (sourceBoat != null && targetBoat != null && !sourceBoat.Deleted && !targetBoat.Deleted && sourceBoat == targetBoat)
                {
                    bool sourceBelongs = false;
                    bool targetBelongs = false;

                    //Source Belongs n the Boat
                    if (sourceBoat.Crew.Contains(source) || sourceBoat.IsFriend(source) || sourceBoat.IsCoOwner(source) || sourceBoat.IsOwner(source))
                        sourceBelongs = true;

                    //Source's Owner Belongs on the Boat                    
                    else if (bc_Source != null)
                    {
                        if (m_SourceController != null)
                        {
                            if (sourceBoat.Crew.Contains(m_SourceController) || sourceBoat.IsFriend(m_SourceController) || sourceBoat.IsCoOwner(m_SourceController) || sourceBoat.IsOwner(m_SourceController))
                                sourceBelongs = true;
                        }
                    }

                    //Target Belongs On The Boat
                    if (sourceBoat.Crew.Contains(target) || sourceBoat.IsFriend(target) || sourceBoat.IsCoOwner(target) || sourceBoat.IsOwner(target))
                        targetBelongs = true;

                    //Target's Owner Belongs On the Boat
                    else if (bc_Target != null)
                    {
                        if (m_TargetController != null)
                        {
                            if (sourceBoat.Crew.Contains(m_TargetController) || sourceBoat.IsFriend(m_TargetController) || sourceBoat.IsCoOwner(m_TargetController) || sourceBoat.IsOwner(m_TargetController))
                                targetBelongs = true;
                        }
                    }

                    //Target May Be Freely Attacked on Boat
                    if (sourceBelongs && !targetBelongs)
                        return Notoriety.CanBeAttacked;
                }
            }

            //Polymorph or Body Transformation
            if (!(bc_Target != null && bc_Target.InitialInnocent))
            {
                if (target.Player && target.BodyMod > 0)
                {
                }

                else if (!target.Body.IsHuman && !target.Body.IsGhost && !IsPet(bc_Target) && !TransformationSpellHelper.UnderTransformation(target) && !AnimalForm.UnderTransformation(target))
                    return Notoriety.CanBeAttacked;
            }

            //If somehow a player is attacking us with their tamed creatures, and their creatures are flagged to us but the player isn't
            //if (pm_Source != null && pm_Target != null)
            //{
            //    if (pm_Target.AllFollowers.Count > 0)
            //    {
            //        //Any of the Player's Other Followers
            //        foreach (Mobile follower in pm_Target.AllFollowers)
            //        {
            //            int notorietyResult = Notoriety.Compute(source, follower);

            //            //Enemy Tamer Adopts Notoriety of Their Creature (Anything other than Innocent)
            //            if (notorietyResult != 1)
            //            {
            //                foreach(var aggressor in source.Aggressors)
            //                {
            //                    if (aggressor.Attacker == follower)
            //                        return notorietyResult;
            //                }
            //            }
            //        } 
            //    }
            //}
            
            return Notoriety.Innocent;
        }

        public static void PushNotoriety(Mobile from, Mobile to, bool aggressor)
        {
            BaseCreature bc_From = from as BaseCreature;
            PlayerMobile pm_From = from as PlayerMobile;

            BaseCreature bc_To = to as BaseCreature;
            PlayerMobile pm_To = to as PlayerMobile;

            PlayerMobile pm_First = null;
            PlayerMobile pm_Second = null;

            if (from == null || to == null)
                return;

            if (pm_From != null)
                pm_First = pm_From;

            if (bc_From != null)
            {
                if (bc_From.Controlled && bc_From.ControlMaster is PlayerMobile)
                    pm_First = bc_From.ControlMaster as PlayerMobile;
            }

            if (pm_To != null)
                pm_Second = pm_To;

            if (bc_To != null)
            {
                if (bc_To.Controlled && bc_To.ControlMaster is PlayerMobile)
                    pm_Second = bc_To.ControlMaster as PlayerMobile;
            }

            //First Player is Online
            if (pm_First != null)
            {
                if (pm_First.NetState != null)
                {
                    List<Mobile> m_Viewables = new List<Mobile>();

                    if (pm_First.AllFollowers.Count > 0)
                    {
                        foreach (Mobile follower in pm_First.AllFollowers)
                        {
                            if (follower != null)
                                m_Viewables.Add(follower);
                        }
                    }

                    if (pm_Second != null)
                    {
                        m_Viewables.Add(pm_Second);

                        if (pm_Second.AllFollowers.Count > 0)
                        {
                            foreach (Mobile follower in pm_Second.AllFollowers)
                            {
                                if (follower != null)
                                    m_Viewables.Add(follower);
                            }
                        }
                    }

                    if (bc_To != null)
                        m_Viewables.Add(bc_To);

                    //Update Data for All Things Viewable By This Player
                    foreach (Mobile mobile in m_Viewables)
                    {
                        if (mobile != null)
                        {
                            if (pm_First.CanSee(mobile))
                                pm_First.NetState.Send(MobileIncoming.Create(pm_First.NetState, pm_First, mobile));
                        }
                    }
                }
            }

            //Second Player is Online: 
            if (pm_Second != null && pm_Second != pm_First)
            {
                if (pm_Second.NetState != null)
                {
                    List<Mobile> m_Viewables = new List<Mobile>();

                    if (pm_Second.AllFollowers.Count > 0)
                    {
                        foreach (Mobile follower in pm_Second.AllFollowers)
                        {
                            if (follower != null)
                                m_Viewables.Add(follower);
                        }
                    }

                    if (pm_First != null)
                    {
                        m_Viewables.Add(pm_First);

                        if (pm_First.AllFollowers.Count > 0)
                        {
                            foreach (Mobile follower in pm_First.AllFollowers)
                            {
                                if (follower != null)
                                    m_Viewables.Add(follower);
                            }
                        }
                    }

                    if (bc_From != null)
                        m_Viewables.Add(bc_From);

                    //Update Data for All Things Viewable By This Player
                    foreach (Mobile mobile in m_Viewables)
                    {
                        if (mobile != null)
                        {
                            if (pm_Second.CanSee(mobile))
                                pm_Second.NetState.Send(MobileIncoming.Create(pm_Second.NetState, pm_Second, mobile));
                        }
                    }
                }
            }
        }

        public static bool IsPaladin(Mobile source, Mobile target)
        {
            return (source is PlayerMobile && ((PlayerMobile)source).Paladin) || (target is PlayerMobile && ((PlayerMobile)target).Paladin);
        }

        public static bool CheckHouseFlag(Mobile from, Mobile m, Point3D p, Map map)
        {
            BaseHouse house = BaseHouse.FindHouseAt(p, map, 16);

            if (house == null || house.Public || !house.IsFriend(from))
                return false;

            if (m != null && house.IsFriend(m))
                return false;

            if (house.IsInGuardedRegion())
                return false;

            BaseCreature c = m as BaseCreature;

            if (c != null && !c.Deleted && c.Controlled && c.ControlMaster != null)
                return !house.IsFriend(c.ControlMaster);

            return true;
        }

        public static bool IsPet(BaseCreature c)
        {
            return (c != null && c.Controlled);
        }

        public static bool IsSummoned(BaseCreature c)
        {
            return (c != null && /*c.Controlled &&*/ c.Summoned);
        }

        public static bool CheckAggressor(List<AggressorInfo> list, Mobile target)
        {
            for (int i = 0; i < list.Count; ++i)
                if (list[i].Attacker == target)
                    return true;

            return false;
        }

        public static bool CheckAggressed(List<AggressorInfo> list, Mobile target)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                AggressorInfo info = list[i];

                if (!info.CriminalAggression && info.Defender == target)
                    return true;
            }

            return false;
        }
    }
}