using System;
using Server;
using Server.Items;
using Server.Guilds;
using Server.Multis;
using Server.Regions;
using Server.Mobiles;
using Server.Targeting;
using Server.Engines.PartySystem;
using Server.Misc;
using System.Collections.Generic;
using Server.Spells.Seventh;
using Server.Spells.Fifth;

using Server.Commands;
using Server.Network;
using Server.ArenaSystem;
using Server.Custom;

namespace Server
{
    public class DefensiveSpell
    {
        public static void Nullify(Mobile from)
        {
            if (!from.CanBeginAction(typeof(DefensiveSpell)))
                new InternalTimer(from).Start();
        }

        private class InternalTimer : Timer
        {
            private Mobile m_Mobile;

            public InternalTimer(Mobile m)
                : base(TimeSpan.FromMinutes(1.0))
            {
                m_Mobile = m;

                Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                m_Mobile.EndAction(typeof(DefensiveSpell));
            }
        }
    }
}

namespace Server.Spells
{
    public enum TravelCheckType
    {
        RecallFrom,
        RecallTo,
        GateFrom,
        GateTo,
        Mark,
        TeleportFrom,
        TeleportTo
    }

    public class SpellHelper
    {
        public static bool SPELLS_USE_IPY3_STYLE_DISRUPTS_AND_HEALS = true;

        private static TimeSpan AosDamageDelay = TimeSpan.FromSeconds(1.0);
        private static TimeSpan OldDamageDelay = TimeSpan.FromSeconds(0.5);

        private static bool DistanceDelayEnabled = false;

        public static TimeSpan SpellHealWindowDuration = TimeSpan.FromSeconds(10);
        public static double SpellHealScalarAdjustmentPerCount = .25;

        public static double HealThroughPoisonScalar = 0.5;
        
        [Usage("ToggleDistanceDelay")]
        [Description("Enables or disables the distance delay system")]
        public static void ToggleDistanceDelay_OnCommand(CommandEventArgs e)
        {
            if (DistanceDelayEnabled)
            {
                DistanceDelayEnabled = false;
                e.Mobile.SendMessage("Distance Delay has been disabled.");
            }
            else
            {
                DistanceDelayEnabled = true;
                e.Mobile.SendMessage("Distance Delay has been enabled.");
            }
        }

        public static void Initialize()
        {
            CommandSystem.Register("ToggleDistanceDelay",  AccessLevel.Developer, new CommandEventHandler(ToggleDistanceDelay_OnCommand));
        }

        public const double enhancedSummonDamageMultiplier = 1.25;
        public const double enhancedSummonHitPointsMultiplier = 1.25;
        public const double enhancedSummonDurationMultiplier = 1.25;

        public const double enhancedMultiplier = 1.25;
        public const double enhancedTamedCreatureMultiplier = 1.125;

        public const double chargedMultiplier = 1.5;
        public const double chargedTamedCreatureMultiplier = 1.25;

        public const double slayerMultiplier = 1.5;
        public const double slayerTamedMultiplier = 1.25;

        public static bool IsChargedSpell(Mobile caster, Mobile target, bool needValidTarget, bool fromScroll)
        {
            if (caster == null)
                return false;

            if (needValidTarget)
            {
                var pm_Caster = caster as PlayerMobile;
                var bc_Caster = caster as BaseCreature;

                var pm_Target = target as PlayerMobile;
                var bc_Target = target as BaseCreature;

                if (pm_Caster != null && pm_Target != null)
                    return false;

                if (bc_Target != null && bc_Target.ImmuneToChargedSpells)
                    return false;
            }

            double spiritSpeakSkill = caster.Skills[SkillName.SpiritSpeak].Value;
            double baseChance = 0.10;

            if (!(caster is PlayerMobile))
                baseChance = 0.05;

            double bonusChance = 0.15 * (spiritSpeakSkill / 100);

            double inscriptionSkill = caster.Skills[SkillName.Inscribe].Value;

            if (fromScroll)
                bonusChance += (0.5 * inscriptionSkill / 100);

            double chance = baseChance + bonusChance;

            double result = Utility.RandomDouble();

            if (result <= chance)
            {
                if (target != null)
                    target.FixedEffect(0x3779, 10, 20);

                if (caster is BaseCreature)
                    caster.PublicOverheadMessage(MessageType.Regular, 0, false, "*charges spell*");
                else
                    caster.SendMessage("You charge your spell with additional energy!");

                return true;
            }

            return false;
        }

        public static bool IsTamedTarget(Mobile caster, Mobile target)
        {
            BaseCreature bc_Creature = target as BaseCreature;

            if (bc_Creature != null)
            {
                if (bc_Creature.Controlled && bc_Creature.ControlMaster is PlayerMobile)
                    return true;
            }

            return false;
        }

        public static bool IsEnhancedSpell(Mobile caster, Mobile target, EnhancedSpellbookType enhancedSpellbookType, bool needValidTarget, bool spendCharge)
        {
            if (caster == null || enhancedSpellbookType == null)
                return false;

            if (!(caster is PlayerMobile))
                return false;

            if (needValidTarget)
            {
                var bcTarget = target as BaseCreature;

                if (target is PlayerMobile || (bcTarget != null && bcTarget.ImmuneToChargedSpells))
                    return false;
            }

            if (caster.FindItemOnLayer(Layer.OneHanded) is EnhancedSpellbook)
            {
                EnhancedSpellbook spellbook = caster.FindItemOnLayer(Layer.OneHanded) as EnhancedSpellbook;

                if (spellbook.EnhancedType == enhancedSpellbookType)
                {
                    if (spendCharge)
                        spellbook.OnSpellCast(caster);

                    return true;
                }
            }

            return false;
        }

        public static bool CheckIfOK(Map map, int x, int y, int z)
        {
            Point3D location = new Point3D(x, y, z);

            if (!Region.Find(location, map).AllowSpawn())
                return false;

            bool foundOceanStatic = false;

            IPooledEnumerable nearbyItems = map.GetItemsInRange(location, 0);

            foreach (Item item in nearbyItems)
            {
                if (item.OceanStatic)
                {
                    foundOceanStatic = true;
                    break;
                }
            }

            nearbyItems.Free();

            if (foundOceanStatic)
                return false;

            return CanFit(map, x, y, z, 16, false, true, true);
        }

        public static bool CanFit(Map m, int x, int y, int z, int height, bool checkBlocksFit, bool checkMobiles, bool requireSurface)
        {
            if (m == Map.Internal)
                return false;

            if (x < 0 || y < 0 || x >= m.Width || y >= m.Height)
                return false;

            bool hasSurface = false;

            LandTile lt = m.Tiles.GetLandTile(x, y);
            int lowZ = 0, avgZ = 0, topZ = 0;

            m.GetAverageZ(x, y, ref lowZ, ref avgZ, ref topZ);
            TileFlag landFlags = TileData.LandTable[lt.ID & TileData.MaxLandValue].Flags;

            if ((landFlags & TileFlag.Impassable) != 0 && avgZ > z && (z + height) > lowZ)
                return false;
            else if ((landFlags & TileFlag.Impassable) == 0 && z == avgZ && !lt.Ignored)
                hasSurface = true;

            StaticTile[] staticTiles = m.Tiles.GetStaticTiles(x, y, true);

            bool surface, impassable;

            for (int i = 0; i < staticTiles.Length; ++i)
            {
                ItemData id = TileData.ItemTable[staticTiles[i].ID & TileData.MaxItemValue];
                surface = id.Surface;
                impassable = id.Impassable;

                if ((surface || impassable) && (staticTiles[i].Z + id.CalcHeight) > z && (z + height) > staticTiles[i].Z)
                    return false;
                else if (surface && !impassable && z == (staticTiles[i].Z + id.CalcHeight))
                    hasSurface = true;
            }

            Sector sector = m.GetSector(x, y);
            List<Item> items = sector.Items;
            List<Mobile> mobs = sector.Mobiles;

            for (int i = 0; i < items.Count; ++i)
            {
                Item item = items[i];

                if (!(item is BaseMulti) && item.ItemID <= TileData.MaxItemValue && item.AtWorldPoint(x, y))
                {
                    ItemData id = item.ItemData;
                    surface = id.Surface;
                    impassable = id.Impassable;

                    if ((surface || impassable || (checkBlocksFit && item.BlocksFit)) && (item.Z + id.CalcHeight) > z && (z + height) > item.Z)
                        return false;
                    else if (surface && !impassable && !item.Movable && z == (item.Z + id.CalcHeight))
                        hasSurface = true;
                }
            }

            if (checkMobiles)
            {
                for (int i = 0; i < mobs.Count; ++i)
                {
                    Mobile mobile = mobs[i];

                    if (mobile.Location.X == x && mobile.Location.Y == y && mobile.Alive && (mobile.AccessLevel == AccessLevel.Player || !mobile.Hidden))
                        if ((mobile.Z + 16) > z && (z + height) > mobile.Z)
                            return false;
                }
            }

            return !requireSurface || hasSurface;
        }

        public static bool CheckMulti(Point3D p, Map map)
        {
            return CheckMulti(p, map, true, 0);
        }

        public static bool CheckMulti(Point3D p, Map map, bool houses)
        {
            return CheckMulti(p, map, houses, 0);
        }

        public static bool CheckMulti(Point3D p, Map map, bool houses, int housingrange)
        {
            if (map == null || map == Map.Internal)
                return false;

            Sector sector = map.GetSector(p.X, p.Y);

            for (int i = 0; i < sector.Multis.Count; ++i)
            {
                BaseMulti multi = sector.Multis[i];

                if (multi is BaseHouse)
                {
                    BaseHouse bh = (BaseHouse)multi;

                    if ((houses && bh.IsInside(p, 16)) || (housingrange > 0 && bh.InRange(p, housingrange)))
                        return true;
                }
                else if (multi.Contains(p))
                {
                    return true;
                }
            }

            return false;
        }

        public static void Turn(Mobile from, object to)
        {
            IPoint3D target = to as IPoint3D;

            if (target == null)
                return;

            if (target is Item)
            {
                Item item = (Item)target;

                if (item.RootParent != from)
                    from.Direction = from.GetDirectionTo(item.GetWorldLocation());
            }
            else if (from != target)
            {
                from.Direction = from.GetDirectionTo(target);
            }
        }

        //Changed to follow ipy timers
        private static TimeSpan CombatHeatDelay = TimeSpan.FromSeconds(30);
        private static bool RestrictTravelCombat = false;

        public static bool CheckCombat(Mobile m, bool forceCheck = false)
        {
            if (!m.Alive) return false;

            if (!forceCheck)
                return false;

            for (int i = 0; i < m.Aggressed.Count; ++i)
            {
                AggressorInfo info = m.Aggressed[i];

                if (info.Defender.Player && (DateTime.UtcNow - info.LastCombatTime) < CombatHeatDelay)
                    return true;
            }

            return false;
        }

        public static bool AdjustField(ref Point3D p, Map map, int height, bool mobsBlock)
        {
            if (map == null)
                return false;

            for (int offset = 0; offset < 10; ++offset)
            {
                Point3D loc = new Point3D(p.X, p.Y, p.Z - offset);

                if (map.CanFit(loc, height, true, mobsBlock))
                {
                    p = loc;
                    return true;
                }
            }

            return false;
        }

        public static bool CanRevealCaster(Mobile m)
        {
            if (m is BaseCreature)
            {
                BaseCreature c = (BaseCreature)m;

                if (!c.Controlled)
                    return true;
            }

            return false;
        }

        public static void GetSurfaceTop(ref IPoint3D p)
        {
            if (p is Item)
            {
                p = ((Item)p).GetSurfaceTop();
            }
            else if (p is StaticTarget)
            {
                StaticTarget t = (StaticTarget)p;
                int z = t.Z;

                if ((t.Flags & TileFlag.Surface) == 0)
                    z -= TileData.ItemTable[t.ItemID & TileData.MaxItemValue].CalcHeight;

                p = new Point3D(t.X, t.Y, z);
            }
        }

        public static bool AddStatOffset(Mobile m, StatType type, int offset, TimeSpan duration)
        {
            if (offset > 0)
                return AddStatBonus(m, m, type, offset, duration);
            else if (offset < 0)
                return AddStatCurse(m, m, type, -offset, duration);

            return true;
        }

        public static bool AddStatBonus(Mobile caster, Mobile target, StatType type)
        {
            TimeSpan duration;
            int offset;

            //Enhanced Spellbook: Wizard
            bool enhancedSpellcast = false;

            if (caster.FindItemOnLayer(Layer.OneHanded) is EnhancedSpellbook)
            {
                EnhancedSpellbook spellbook = caster.FindItemOnLayer(Layer.OneHanded) as EnhancedSpellbook;

                if (spellbook.EnhancedType == EnhancedSpellbookType.Wizard)
                    enhancedSpellcast = true;
            }

            if (enhancedSpellcast)
            {
                int length = (int)(5 * GetDuration(caster, target).TotalSeconds);

                duration = TimeSpan.FromSeconds(length);
                offset = GetOffset(caster, target, type, false);
            }

            else
            {
                duration = GetDuration(caster, target);
                offset = GetOffset(caster, target, type, false);
            }

            return AddStatBonus(caster, target, type, offset, duration);
        }

        public static bool AddStatBonus(Mobile caster, Mobile target, StatType type, int bonus, TimeSpan duration)
        {
            int offset = bonus;
            string name = String.Format("[Magic] {0} Offset", type);

            StatMod mod = target.GetStatMod(name);

            if (mod != null && mod.Offset < 0)
            {
                target.AddStatMod(new StatMod(type, name, mod.Offset + offset, duration));
                return true;
            }

            else if (mod == null || mod.Offset < offset)
            {
                target.AddStatMod(new StatMod(type, name, offset, duration));
                return true;
            }

            return false;
        }

        public static bool AddStatCurse(Mobile caster, Mobile target, StatType type)
        {
            TimeSpan duration;
            int offset;

            //Enhanced Spellbook: Warlock
            bool enhancedSpellcast = false;

            if (caster.FindItemOnLayer(Layer.OneHanded) is EnhancedSpellbook)
            {
                EnhancedSpellbook spellbook = caster.FindItemOnLayer(Layer.OneHanded) as EnhancedSpellbook;

                if (spellbook.EnhancedType == EnhancedSpellbookType.Warlock && target is BaseCreature)
                    enhancedSpellcast = true;
            }

            if (enhancedSpellcast)
            {
                int length = (int)(5 * GetDuration(caster, target).TotalSeconds);

                duration = TimeSpan.FromSeconds(length);
                offset = 3 * GetOffset(caster, target, type, true);
            }

            else
            {
                duration = GetDuration(caster, target);
                offset = GetOffset(caster, target, type, true);
            }

            if (caster is BaseCreature && target is PlayerMobile)
            {
                if (offset > 11)
                    offset = 11;
            }

            return AddStatCurse(caster, target, type, offset, duration);
        }

        public static bool AddStatCurse(Mobile caster, Mobile target, StatType type, int curse, TimeSpan duration)
        {
            int offset = -curse;
            string name = String.Format("[Magic] {0} Offset", type);

            StatMod mod = target.GetStatMod(name);

            if (mod != null && mod.Offset > 0)
            {
                target.AddStatMod(new StatMod(type, name, mod.Offset + offset, duration));
                //Added by IPY
                if (target.Spell != null)
                    target.Spell.OnCasterHurt();
                return true;
            }
            else if (mod == null || mod.Offset > offset)
            {
                target.AddStatMod(new StatMod(type, name, offset, duration));
                //Added by IPY
                if (target.Spell != null)
                    target.Spell.OnCasterHurt();
                return true;
            }

            return false;
        }

        public static TimeSpan GetDuration(Mobile caster, Mobile target)
        {
            //Changed to IPY calculations
            if (Core.AOS)
                return TimeSpan.FromSeconds(((caster.Skills[SkillName.EvalInt].Value / 5) + 1) * 6);

            return TimeSpan.FromSeconds(caster.Skills[SkillName.Magery].Value * 1.2);
        }

        private static bool m_DisableSkillCheck;

        public static bool DisableSkillCheck
        {
            get { return m_DisableSkillCheck; }
            set { m_DisableSkillCheck = value; }
        }

        public static double GetOffsetScalar(Mobile caster, Mobile target, bool curse)
        {
            double percent;

            //Changed to IPY Calculations
            if (curse)
                percent = 8 + ((caster.Skills[SkillName.EvalInt].Value - target.Skills[SkillName.MagicResist].Value) / 10);
            else
                percent = 1 + (caster.Skills[SkillName.EvalInt].Value / 10);

            percent *= 0.01;

            if (percent < 0)
                percent = 0;

            return percent;
        }

        public static int GetOffset(Mobile caster, Mobile target, StatType type, bool curse)
        {
            #region AOS - NOT USED
            if (Core.AOS)
            {
                if (!m_DisableSkillCheck)
                {
                    caster.CheckSkill(SkillName.EvalInt, 0.0, 120.0, 1.0);

                    if (curse)
                        target.CheckSkill(SkillName.MagicResist, 0.0, 120.0, 1.0);
                }

                double percent = GetOffsetScalar(caster, target, curse);

                switch (type)
                {
                    case StatType.Str:
                        return (int)(target.RawStr * percent);
                    case StatType.Dex:
                        return (int)(target.RawDex * percent);
                    case StatType.Int:
                        return (int)(target.RawInt * percent);
                }
            }
            #endregion

            return 1 + (int)(caster.Skills[SkillName.Magery].Value * 0.1);
        }

        public static Guild GetGuildFor(Mobile m)
        {
            Guild g = m.Guild as Guild;

            if (g == null && m is BaseCreature)
            {
                BaseCreature c = (BaseCreature)m;
                m = c.ControlMaster;

                if (m != null)
                    g = m.Guild as Guild;

                if (g == null)
                {
                    m = c.SummonMaster;

                    if (m != null)
                        g = m.Guild as Guild;
                }
            }

            return g;
        }

        public static bool ValidIndirectTarget(Mobile from, Mobile to)
        {
            return true;
        }

        private static int[] m_Offsets = new int[]
            {
                -1, -1,
                -1,  0,
                -1,  1,
                0, -1,
                0,  1,
                1, -1,
                1,  0,
                1,  1
            };

        public static void Summon(BaseCreature creature, Mobile caster, int sound, TimeSpan duration, bool scaleDuration, bool scaleStats)
        {
            Map map = caster.Map;

            if (map == null)
                return;

            double scale = 1.0 + ((caster.Skills[SkillName.Magery].Value - 100.0) / 200.0);

            //Added on IPY
            if (creature is SummonedDaemon)
                scale *= 0.5;

            if (scaleDuration)
                duration = TimeSpan.FromSeconds(duration.TotalSeconds * scale);

            if (scaleStats)
            {
                creature.RawStr = (int)(creature.RawStr * scale);
                creature.Hits = creature.HitsMax;

                creature.RawDex = (int)(creature.RawDex * scale);
                creature.Stam = creature.StamMax;

                creature.RawInt = (int)(creature.RawInt * scale);
                creature.Mana = creature.ManaMax;
            }

            /*Point3D p = new Point3D( caster );

            if( SpellHelper.FindValidSpawnLocation( map, ref p, true ) )
            {
                BaseCreature.Summon( creature, caster, p, sound, duration );
                return;
            }


            */
            //Kept IPY way
            int offset = Utility.Random(8) * 2;

            for (int i = 0; i < m_Offsets.Length; i += 2)
            {
                int x = caster.X + m_Offsets[(offset + i) % m_Offsets.Length];
                int y = caster.Y + m_Offsets[(offset + i + 1) % m_Offsets.Length];

                if (map.CanSpawnMobile(x, y, caster.Z))
                {
                    BaseCreature.Summon(creature, caster, new Point3D(x, y, caster.Z), sound, duration);
                    return;
                }
                else
                {
                    int z = map.GetAverageZ(x, y);

                    if (map.CanSpawnMobile(x, y, z))
                    {
                        BaseCreature.Summon(creature, caster, new Point3D(x, y, z), sound, duration);
                        return;
                    }
                }
            }

            creature.Delete();
            caster.SendLocalizedMessage(501942); // That location is blocked.
        }
        //Not used on IPY
        public static bool FindValidSpawnLocation(Map map, ref Point3D p, bool surroundingsOnly)
        {
            if (map == null)   //sanity
                return false;

            if (!surroundingsOnly)
            {
                if (map.CanSpawnMobile(p))   //p's fine.
                {
                    p = new Point3D(p);
                    return true;
                }

                int z = map.GetAverageZ(p.X, p.Y);

                if (map.CanSpawnMobile(p.X, p.Y, z))
                {
                    p = new Point3D(p.X, p.Y, z);
                    return true;
                }
            }

            int offset = Utility.Random(8) * 2;

            for (int i = 0; i < m_Offsets.Length; i += 2)
            {
                int x = p.X + m_Offsets[(offset + i) % m_Offsets.Length];
                int y = p.Y + m_Offsets[(offset + i + 1) % m_Offsets.Length];

                if (map.CanSpawnMobile(x, y, p.Z))
                {
                    p = new Point3D(x, y, p.Z);
                    return true;
                }
                else
                {
                    int z = map.GetAverageZ(x, y);

                    if (map.CanSpawnMobile(x, y, z))
                    {
                        p = new Point3D(x, y, z);
                        return true;
                    }
                }
            }

            return false;
        }

        private delegate bool TravelValidator(Map map, Point3D loc);

        private static TravelValidator[] m_Validators = new TravelValidator[]
            {
                new TravelValidator( IsFeluccaT2A ),
                new TravelValidator( IsIlshenar ),
                new TravelValidator( IsTrammelWind ),
                new TravelValidator( IsFeluccaWind ),
                new TravelValidator( IsFeluccaDungeon ),
                new TravelValidator( IsTrammelSolenHive ),
                new TravelValidator( IsFeluccaSolenHive ),
                new TravelValidator( IsCrystalCave ),
                new TravelValidator( IsDoomGauntlet ),
                new TravelValidator( IsDoomFerry ),
                new TravelValidator( IsFactionStronghold ),
                new TravelValidator( IsChampionSpawn ),
                new TravelValidator( IsTokunoDungeon ),
                // IPY no travel zones.
                new TravelValidator( IsIceIslandZone ),
                new TravelValidator( IsFireIslandZone ),
                new TravelValidator( IsRaidZone ),
                new TravelValidator( IsHedgeMaze ),
                new TravelValidator( IsYewJail ),
                new TravelValidator( IsFeluccaRestrictedArea )
            };

        private static bool[,] m_Rules = new bool[,]
            {
                        /*T2A(Fel)      Ilshenar        Wind(Tram), Wind(Fel),  Dungeons(Fel),  Solen(Tram),    Solen(Fel), CrystalCave(Malas), Gauntlet(Malas),    Gauntlet(Ferry),    Stronghold,     ChampionSpawn, Dungeons(Tokuno[Malas]), Ice Island Zone,   Fire Island Zone   RAID ZONE    HEDGE MAZE     YEW JAIL  RestrictedArea(Felluca)*/ 
/* Recall From */       { true,         true,           true,           false,      true,          true,           false,      false,              false,              false,              false,          false,          false,                  true,               true,         false,       false,         false,         false},
/* Recall To */         { true,         false,          false,          false,      true,          false,          false,      false,              false,              false,              false,          false,          false,                  true,               true,         false,       false,         false,         false},
/* Gate From */         { true,         false,          false,          false,      true,          false,          false,      false,              false,              false,              false,          false,          false,                  true,               true,         false,       false,         false,         false},
/* Gate To */           { true,         false,          false,          false,      true,          false,          false,      false,              false,              false,              false,          false,          false,                  true,               true,         false,       false,         false,         false},
/* Mark In */           { true,         false,          false,          false,      true,          false,          false,      false,              false,              false,              false,          false,          false,                  true,               true,         false,       false,         false,         false},
/* Tele From */         { true,         true,           true,           true,       true,          true,           true,       true,               true,               true,               true,           true,           true,                   true,               true,         false,       false,         false,         true},
/* Tele To */           { true,         true,           true,           true,       true,          true,           true,       true,               true,               true,               true,           true,           true,                   true,               true,         false,       false,         false,         true},
            };

        public static void SendInvalidMessage(Mobile caster, TravelCheckType type)
        {
            if (type == TravelCheckType.RecallTo || type == TravelCheckType.GateTo)
                caster.SendLocalizedMessage(1019004); // You are not allowed to travel there.

            else if (type == TravelCheckType.TeleportTo)
                caster.SendLocalizedMessage(501035); // You cannot teleport from here to the destination.

            else
                caster.SendLocalizedMessage(501802); // Thy spell doth not appear to work...
        }

        public static bool CheckTravel(Mobile caster, TravelCheckType type)
        {
            return CheckTravel(caster, caster.Map, caster.Location, type);
        }

        public static bool CheckTravel(Map map, Point3D loc, TravelCheckType type)
        {
            return CheckTravel(null, map, loc, type);
        }

        private static Mobile m_TravelCaster;
        private static TravelCheckType m_TravelType;

        public static bool CheckTravel(Mobile caster, Map map, Point3D loc, TravelCheckType type)
        {
            if (IsInvalid(map, loc)) // null, internal, out of bounds
            {
                if (caster != null)
                    SendInvalidMessage(caster, type);

                return false;
            }

            bool foundOceanStatic = false;

            IPooledEnumerable nearbyItems = map.GetItemsInRange(loc, 0);

            foreach (Item item in nearbyItems)
            {
                if (item.OceanStatic)
                {
                    foundOceanStatic = true;
                    break;
                }
            }

            nearbyItems.Free();

            if (foundOceanStatic)
            {
                caster.SendMessage("That is not a valid location.");
                return false;
            }

            if (caster != null && caster.AccessLevel == AccessLevel.Player && caster.Region.IsPartOf(typeof(Regions.Jail)))
            {
                caster.SendLocalizedMessage(1114345); // You'll need a better jailbreak plan than that!
                return false;
            }

            // Always allow monsters to teleport
            if (caster is BaseCreature && (type == TravelCheckType.TeleportTo || type == TravelCheckType.TeleportFrom))
            {
                BaseCreature bc = (BaseCreature)caster;

                if (!bc.Controlled && !bc.Summoned)
                    return true;
            }

            m_TravelCaster = caster;
            m_TravelType = type;

            int v = (int)type;
            bool isValid = true;

            for (int i = 0; isValid && i < m_Validators.Length; ++i)
                isValid = (m_Rules[v, i] || !m_Validators[i](map, loc));

            if (!isValid && caster != null)
                SendInvalidMessage(caster, type);

            return isValid;
        }

        // IPY - Buccs and misc other IPY locations
        private static int[] m_BuccsLocation = new int[] { 2550, 3000, 1900, 2400 };
        public static bool InBuccs(Map map, Point3D loc)
        {
            int x = loc.X;
            int y = loc.Y;

            return (map == Map.Felucca) && (x > m_BuccsLocation[0] && x < m_BuccsLocation[1] && y > m_BuccsLocation[2] && y < m_BuccsLocation[3]);
        }

        public static bool IsFireIslandZone(Map map, Point3D loc)
        {
            int x = loc.X, y = loc.Y;

            return (map == Map.Felucca) && (x >= 3712 && y >= 2912 && x < 5119 && y < 4095);
        }

        public static bool InYewCrypts(Map map, Point3D loc)
        {
            int x = loc.X;
            int y = loc.Y;

            return (map == Map.Felucca) && (x > 940 && x < 1024 && y > 677 && y < 834);
        }


        public static bool InYewOrcFort(Map map, Point3D loc)
        {
            int x = loc.X;
            int y = loc.Y;

            return (map == Map.Felucca) && (x > 591 && x < 669 && y > 1450 && y < 1518);
        }

        public static bool IsDeceitDungeon(Map map, Point3D loc)
        {
            int x = loc.X, y = loc.Y;

            return (map == Map.Felucca) &&
                ((x >= 5120 && y >= 520 && x <= 5360 && y <= 769) ||
                (x >= 5189 && y >= 1802 && x < 5275 && y < 1868) ||
                (x >= 5149 && y >= 1198 && x < 5228 && y < 1266));
        }

        public static bool IsYewJail(Map map, Point3D loc)
        {
            int x = loc.X, y = loc.Y;

            return (map == Map.Felucca) && (x >= 254 && y >= 757 && x <= 292 && y <= 782);

        }

        public static bool IsHythlothDungeon(Map map, Point3D loc)
        {
            int x = loc.X, y = loc.Y;

            return (map == Map.Felucca) && ((x >= 5900 && y >= 0 && x <= 6150 && y <= 250) || (x >= 6625 && y >= 60 && x < 6706 && y < 197));
        }

        public static bool IsRaidZone(Map map, Point3D loc)
        {
            return false;
        }

        public static bool IsIceIslandZone(Map map, Point3D loc)
        {
            int x = loc.X, y = loc.Y;

            return (map == Map.Felucca) && (x >= 3542 && y >= 0 && x < 4494 && y < 854);
        }

        public static bool IsWindLoc(Point3D loc)
        {
            int x = loc.X, y = loc.Y;

            return (x >= 5120 && y >= 0 && x < 5376 && y < 256);
        }

        //Added by IPY
        public static bool IsStarRoom(Point3D loc)
        {
            int x = loc.X, y = loc.Y;

            return (x >= 5122 && y >= 1744 && x < 5170 && y < 1790);
        }

        public static bool IsFeluccaWind(Map map, Point3D loc)
        {
            return (map == Map.Felucca && IsWindLoc(loc));
        }

        public static bool IsTrammelWind(Map map, Point3D loc)
        {
            return (map == Map.Trammel && IsWindLoc(loc));
        }

        public static bool IsIlshenar(Map map, Point3D loc)
        {
            return (map == Map.Ilshenar);
        }

        public static bool IsSolenHiveLoc(Point3D loc)
        {
            int x = loc.X, y = loc.Y;

            return (x >= 5640 && y >= 1776 && x < 5935 && y < 2039);
        }

        public static bool IsTrammelSolenHive(Map map, Point3D loc)
        {
            return (map == Map.Trammel && IsSolenHiveLoc(loc));
        }

        public static bool IsFeluccaSolenHive(Map map, Point3D loc)
        {
            return (map == Map.Felucca && IsSolenHiveLoc(loc));
        }

        public static bool IsFeluccaT2A(Map map, Point3D loc)
        {
            int x = loc.X, y = loc.Y;

            return (map == Map.Felucca && x >= 5120 && y >= 2304 && x < 6144 && y < 4096) || IsT2ADungeon(map, loc) || IsSolenHiveLoc(loc);
        }

        public static bool IsAnyT2A(Map map, Point3D loc)
        {
            int x = loc.X, y = loc.Y;

            return ((map == Map.Trammel || map == Map.Felucca) && x >= 5120 && y >= 2304 && x < 6144 && y < 4096) || IsT2ADungeon(map, loc) || IsSolenHiveLoc(loc);
        }

        public static bool IsFeluccaDungeon(Map map, Point3D loc)
        {
            Region region = Region.Find(loc, map);
            return (region.IsPartOf(typeof(DungeonRegion)) && region.Map == Map.Felucca);
        }

        private static Rectangle2D destardBoss = new Rectangle2D(new Point2D(5114, 782), new Point2D(5194, 883));
        private static Rectangle2D lythLair = new Rectangle2D(new Point2D(5128, 995), new Point2D(5214, 1042));
        private static Rectangle2D destardThird = new Rectangle2D(new Point2D(5128, 953), new Point2D(5158, 1015));
        private static Rectangle2D destardThirdOverlap = new Rectangle2D(new Point2D(5151, 977), new Point2D(5183, 1019));
        private static Rectangle2D despiseBoss = new Rectangle2D(new Point2D(5504, 774), new Point2D(5646, 940));
        private static Rectangle2D wrongBoss = new Rectangle2D(new Point2D(5676, 611), new Point2D(5729, 672));
        private static Rectangle2D covetousBoss = new Rectangle2D(new Point2D(5396, 1791), new Point2D(5488, 1834));
        private static Rectangle2D covetousFour = new Rectangle2D(new Point2D(5498, 1793), new Point2D(5556, 1821));
        private static Rectangle2D covetousThree = new Rectangle2D(new Point2D(5564, 1844), new Point2D(5601, 1870));
        private static Rectangle2D shameFour = new Rectangle2D(new Point2D(5636, 3), new Point2D(5893, 125));
        private static Rectangle2D horseLand = new Rectangle2D(new Point2D(5121, 1080), new Point2D(5210, 1155));
        private static Rectangle2D maggotLair = new Rectangle2D(new Point2D(5764, 675), new Point2D(5842, 739));
        private static Rectangle2D iceBoss = new Rectangle2D(new Point2D(5792, 313), new Point2D(5874, 394));
        private static Rectangle2D fireDungeon = new Rectangle2D(new Point2D(5637, 1280), new Point2D(5897, 1531));
        private static Rectangle2D elfDungeon = new Rectangle2D(new Point2D(6188, 307), new Point2D(6571, 724));
        private static Rectangle2D eventArea = new Rectangle2D(new Point2D(6618, 601), new Point2D(6698, 681));


        public static bool IsDungeonBossArea(Map map, Point3D loc)
        {
            var point = new Point2D(loc.X, loc.Y);

            return destardBoss.Contains(point) || despiseBoss.Contains(point)
                || wrongBoss.Contains(point) || covetousBoss.Contains(point)
                || covetousFour.Contains(point) || covetousThree.Contains(point)
                || shameFour.Contains(point) || horseLand.Contains(point)
                || maggotLair.Contains(point) || destardThird.Contains(point)
                || lythLair.Contains(point) || destardThirdOverlap.Contains(point)
                || iceBoss.Contains(point) || fireDungeon.Contains(point)
                || elfDungeon.Contains(point) || IsFeluccaT2A(map, loc)
                || eventArea.Contains(point);
        }

        public static Rectangle2D britainGY = new Rectangle2D(new Point2D(1314, 1430), new Point2D(1436, 1538));
        public static Rectangle2D coveGY = new Rectangle2D(new Point2D(2409, 1076), new Point2D(2472, 1134));
        public static Rectangle2D moonGlowGY = new Rectangle2D(new Point2D(4517, 1285), new Point2D(4565, 1346));
        public static Rectangle2D jhelomGY = new Rectangle2D(new Point2D(1264, 3703), new Point2D(1304, 3760));
        public static Rectangle2D nujelmGY = new Rectangle2D(new Point2D(3491, 1115), new Point2D(3563, 1166));
        public static Rectangle2D vesperGY = new Rectangle2D(new Point2D(2711, 825), new Point2D(2802, 910));
        public static Rectangle2D yewGY = new Rectangle2D(new Point2D(699, 1095), new Point2D(745, 1144));

        public static bool IsGraveYardArea(Map map, Point3D loc)
        {
            var point = new Point2D(loc.X, loc.Y);

            return britainGY.Contains(point) ||
                   coveGY.Contains(point) ||
                   moonGlowGY.Contains(point) ||
                   jhelomGY.Contains(point) ||
                   nujelmGY.Contains(point) ||
                   vesperGY.Contains(point) ||
                   yewGY.Contains(point);
        }

        public static bool IsFeluccaRestrictedArea(Map map, Point3D loc)
        {
            return map == Map.Felucca && (IsHythlothDungeon(map, loc) || IsDeceitDungeon(map, loc) || IsDungeonBossArea(map, loc) || IsNewbieDungeon(map, loc));
        }

        public static bool IsNewbieDungeon(Map map, Point3D loc)
        {
            Region region = Region.Find(loc, map);

            return region is NewbieDungeonRegion;
        }

        public static bool IsHedgeMaze(Map map, Point3D loc)
        {
            int x = loc.X; int y = loc.Y;

            return (map == Map.Felucca) && (x > 1022 && x < 1267 && y > 2147 && y < 2305);
        }

        public static bool IsT2ADungeon(Map map, Point3D loc)
        {
            int x = loc.X; int y = loc.Y;

            return (map == Map.Felucca) && (x > 5119 && x < 6150 && y > 1280 && y < 1780) && !IsBritainSewers(map, loc);
        }

        public static bool IsBritainSewers(Map map, Point3D loc)
        {
            int x = loc.X; int y = loc.Y;

            return (map == Map.Felucca) && (x > 6000 && x < 6150 && y > 1420 && y < 1520);
        }

        public static bool IsCrystalCave(Map map, Point3D loc)
        {
            if (map != Map.Malas)
                return false;

            int x = loc.X, y = loc.Y, z = loc.Z;

            bool r1 = (x >= 1182 && y >= 437 && x < 1211 && y < 470);
            bool r2 = (x >= 1156 && y >= 470 && x < 1211 && y < 503);
            bool r3 = (x >= 1176 && y >= 503 && x < 1208 && y < 509);
            bool r4 = (x >= 1188 && y >= 509 && x < 1201 && y < 513);

            return (z < -80 && (r1 || r2 || r3 || r4));
        }

        public static bool IsFactionStronghold(Map map, Point3D loc)
        {
            /*// Teleporting is allowed, but only for faction members
            if ( !Core.AOS && m_TravelCaster != null && (m_TravelType == TravelCheckType.TeleportTo || m_TravelType == TravelCheckType.TeleportFrom) )
            {
                if ( Factions.Faction.Find( m_TravelCaster, true, true ) != null )
                    return false;
            }*/

            return false;// (Region.Find(loc, map).IsPartOf(typeof(Factions.StrongholdRegion)));
        }

        public static bool IsChampionSpawn(Map map, Point3D loc)
        {
            return (Region.Find(loc, map).IsPartOf(typeof(Engines.CannedEvil.ChampionSpawnRegion)));
        }

        public static bool IsDoomFerry(Map map, Point3D loc)
        {
            if (map != Map.Malas)
                return false;

            int x = loc.X, y = loc.Y;

            if (x >= 426 && y >= 314 && x <= 430 && y <= 331)
                return true;

            if (x >= 406 && y >= 247 && x <= 410 && y <= 264)
                return true;

            return false;
        }

        public static bool IsTokunoDungeon(Map map, Point3D loc)
        {
            //The tokuno dungeons are really inside malas
            if (map != Map.Malas)
                return false;

            int x = loc.X, y = loc.Y, z = loc.Z;

            bool r1 = (x >= 0 && y >= 0 && x <= 128 && y <= 128);
            bool r2 = (x >= 45 && y >= 320 && x < 195 && y < 710);

            return (r1 || r2);
        }

        public static bool IsFireDungeon(Map map, Point3D loc)
        {
            if (map != Map.Felucca && map != Map.Trammel) //trammel for second instance
                return false;

            int x = loc.X, y = loc.Y;

            return (x >= 5635 && y >= 1285 && x < 5880 && y < 1520);
        }

        public static bool IsDoomGauntlet(Map map, Point3D loc)
        {
            if (map != Map.Malas)
                return false;

            int x = loc.X - 256, y = loc.Y - 304;

            return (x >= 0 && y >= 0 && x < 256 && y < 256);
        }

        public static bool IsLampRoom(Map map, Point3D loc)
        {
            if (map != Map.Malas)
                return false;

            int x = loc.X, y = loc.Y;

            return (x >= 465 && y >= 92 && x < 474 && y < 102);
        }

        public static bool IsGuardianRoom(Map map, Point3D loc)
        {
            if (map != Map.Malas)
                return false;

            int x = loc.X, y = loc.Y;

            return (x >= 356 && y >= 5 && x < 375 && y < 25);
        }

        public static bool IsHeartwood(Map map, Point3D loc)
        {
            int x = loc.X, y = loc.Y;

            return (map == Map.Trammel || map == Map.Felucca) && (x >= 6911 && y >= 254 && x < 7167 && y < 511);
        }

        public static bool IsMLDungeon(Map map, Point3D loc)
        {
            return MondainsLegacy.IsMLRegion(Region.Find(loc, map));
        }

        public static bool IsInvalid(Map map, Point3D loc)
        {
            if (map == null || map == Map.Internal)
                return true;

            int x = loc.X, y = loc.Y;

            return (x < 0 || y < 0 || x >= map.Width || y >= map.Height);
        }

        //towns
        public static bool IsTown(IPoint3D loc, Mobile caster)
        {
            if (loc is Item)
                loc = ((Item)loc).GetWorldLocation();

            return IsTown(new Point3D(loc), caster);
        }

        public static bool IsTown(Point3D loc, Mobile caster)
        {
            Map map = caster.Map;

            if (map == null)
                return false;

            #region Dueling
            Engines.ConPVP.SafeZone sz = (Engines.ConPVP.SafeZone)Region.Find(loc, map).GetRegion(typeof(Engines.ConPVP.SafeZone));

            if (sz != null)
            {
                PlayerMobile pm = (PlayerMobile)caster;

                if (pm == null || pm.DuelContext == null || !pm.DuelContext.Started || pm.DuelPlayer == null || pm.DuelPlayer.Eliminated)
                    return true;
            }
            #endregion

            GuardedRegion reg = (GuardedRegion)Region.Find(loc, map).GetRegion(typeof(GuardedRegion));

            return (reg != null && !reg.IsDisabled());
        }

        public static bool CheckTown(IPoint3D loc, Mobile caster)
        {
            if (loc is Item)
                loc = ((Item)loc).GetWorldLocation();

            return CheckTown(new Point3D(loc), caster);
        }

        public static bool CheckTown(Point3D loc, Mobile caster, bool forceAllow = false)
        {
            if (IsTown(loc, caster) && !forceAllow)
            {
                caster.SendLocalizedMessage(500946); // You cannot cast this in town!
                return false;
            }

            return true;
        }
        
        public static void CheckReflect(int circle, Mobile caster, ref Mobile target)
        {
            CheckReflect(circle, ref caster, ref target);
        }

        public static void CheckReflect(int circle, ref Mobile caster, ref Mobile target)
        {
            target.OnBeforeHarmfulSpell();

            if (target.MagicDamageAbsorb > 0)
            {
                ++circle;

                bool reflect = false;

                //Base Creature Casting and Player Has Inscription or Wizard Enhanced Spellbook
                if (target.MagicDamageAbsorb > 1 && caster is BaseCreature)
                {
                    reflect = true;

                    target.MagicDamageAbsorb -= circle;

                    if (target.MagicDamageAbsorb < 0)
                        target.MagicDamageAbsorb = 0;

                    DefensiveSpell.Nullify(target);
                }

                //Normal Reflect
                else if (target.MagicDamageAbsorb > 0)
                {
                    reflect = true;

                    target.MagicDamageAbsorb = 0;
                    DefensiveSpell.Nullify(target);
                }

                else
                {
                }

                if (target is BaseCreature)
                {
                    ((BaseCreature)target).CheckReflect(caster, ref reflect);

                    target.MagicDamageAbsorb = 0;
                    DefensiveSpell.Nullify(target);
                }

                if (reflect)
                {
                    int spellHue = PlayerEnhancementPersistance.GetSpellHueFor(caster, HueableSpell.MagicReflect);

                    //Player Enhancement Customization: Bounce
                    bool shielded = PlayerEnhancementPersistance.IsCustomizationEntryActive(target, CustomizationType.Shielded);

                    if (shielded)
                        CustomizationAbilities.Shielded(target);
                    else
                        target.FixedEffect(0x37B9, 10, 5, spellHue, 0);

                    Mobile temp = caster;

                    caster = target;
                    target = temp;
                }
            }

            else if (target is BaseCreature)
            {
                bool reflect = false;

                ((BaseCreature)target).CheckReflect(caster, ref reflect);

                if (reflect)
                {
                    target.FixedEffect(0x37B9, 10, 5);

                    Mobile temp = caster;
                    caster = target;
                    target = temp;
                }
            }
        }        

        //Rabbi Dan - 10/03/2010 - Distance Based Damage Delay
        public static TimeSpan GetDamageDelayForSpell(Spell sp, Mobile target)
        {
            if (!sp.DelayedDamage)
                return TimeSpan.Zero;

            if (SpellHelper.SPELLS_USE_IPY3_STYLE_DISRUPTS_AND_HEALS && sp is Server.Spells.Fourth.LightningSpell)
            {
                return new TimeSpan(0, 0, 0, 0, 150); // 0.15sec delay on lightning for IPY3
            }

            else
            {
                if (DistanceDelayEnabled)
                {
                    int distance = GetDistanceToTarget(sp.Caster, target);
                    return (new TimeSpan(0, 0, 0, 0, distance * 100));
                }
                else
                {
                    return OldDamageDelay;
                }
            }
        }

        public static int GetDistanceToTarget(Mobile caster, Mobile target)
        {
            int x = Math.Abs(caster.X - target.X);
            int y = Math.Abs(caster.Y - target.Y);

            return Math.Max(x, y);
        }

        public static void Damage(MagerySpell spell, Mobile target, double damage)
        {
            TimeSpan ts = GetDamageDelayForSpell(spell, target);

            Damage(spell, ts, target, spell.Caster, damage);
        }

        public static void Damage(Spell spell, Mobile target, double damage)
        {
            TimeSpan ts = GetDamageDelayForSpell(spell, target);

            Damage(ts, target, spell.Caster, damage);
        }

        public static void Damage(TimeSpan delay, Mobile target, double damage)
        {
            Damage(delay, target, null, damage);
        }

        public static void Damage(TimeSpan delay, Mobile target, Mobile from, double damage)
        {
            if (delay == TimeSpan.Zero)
            {
                var intDamage = (int)damage;
                //Add damage modifier for instant attack spell
                if (target is BaseCreature && from != null)
                    ((BaseCreature)target).AlterSpellDamageFrom(from, ref intDamage);

                target.Damage(intDamage, from);
            }
            else
                new SpellDamageTimer(null, target, from, (int)damage, delay).Start();

            if (target is BaseCreature && from != null && delay == TimeSpan.Zero)
                ((BaseCreature)target).OnDamagedBySpell(from);
        }

        public static void Damage(Spell spell, TimeSpan delay, Mobile target, Mobile from, double damage)
        {
            int iDamage = (int)damage;

            if (target is BaseCreature && from != null && delay == TimeSpan.Zero)
                ((BaseCreature)target).OnDamagedBySpell(from);

            if (delay == TimeSpan.Zero)
            {
                /*Commented by IPY 
                if( from is BaseCreature )
                    ((BaseCreature)from).AlterSpellDamageTo( target, ref iDamage );

                if( target is BaseCreature )
                    ((BaseCreature)target).AlterSpellDamageFrom( from, ref iDamage );
                */
                target.Damage(iDamage, from);
            }
            else
            {
                new SpellDamageTimer(spell, target, from, iDamage, delay).Start();
            }

            //if (target is BaseCreature && from != null && delay == TimeSpan.Zero)
            //   ((BaseCreature)target).OnDamagedBySpell(from);

        }

        public static void Damage(MagerySpell spell, Mobile target, double damage, int phys, int fire, int cold, int pois, int nrgy)
        {
            TimeSpan ts = GetDamageDelayForSpell(spell, target);

            Damage(spell, ts, target, spell.Caster, damage, phys, fire, cold, pois, nrgy, DFAlgorithm.Standard);
        }

        public static void Damage(MagerySpell spell, Mobile target, double damage, int phys, int fire, int cold, int pois, int nrgy, DFAlgorithm dfa)
        {
            TimeSpan ts = GetDamageDelayForSpell(spell, target);

            Damage(spell, ts, target, spell.Caster, damage, phys, fire, cold, pois, nrgy, dfa);
        }

        public static void Damage(Spell spell, Mobile target, double damage, int phys, int fire, int cold, int pois, int nrgy)
        {
            TimeSpan ts = GetDamageDelayForSpell(spell, target);

            Damage(spell, ts, target, spell.Caster, damage, phys, fire, cold, pois, nrgy, DFAlgorithm.Standard);
        }

        public static void Damage(TimeSpan delay, Mobile target, double damage, int phys, int fire, int cold, int pois, int nrgy)
        {
            Damage(delay, target, null, damage, phys, fire, cold, pois, nrgy);
        }

        public static void Damage(TimeSpan delay, Mobile target, Mobile from, double damage, int phys, int fire, int cold, int pois, int nrgy)
        {
            Damage(delay, target, from, damage, phys, fire, cold, pois, nrgy, DFAlgorithm.Standard);
        }

        public static void Damage(TimeSpan delay, Mobile target, Mobile from, double damage, int phys, int fire, int cold, int pois, int nrgy, DFAlgorithm dfa)
        {
            Damage(null, delay, target, from, damage, phys, fire, cold, pois, nrgy, dfa);
        }

        public static void Damage(Spell spell, TimeSpan delay, Mobile target, Mobile from, double damage, int phys, int fire, int cold, int pois, int nrgy, DFAlgorithm dfa)
        {
            if (damage == null)
                return;

            PlayerMobile pm_Caster = from as PlayerMobile;
            BaseCreature bc_Caster = from as BaseCreature;

            BaseCreature bc_Target = target as BaseCreature;
            PlayerMobile pm_Target = target as PlayerMobile;

            int iDamage = (int)damage;
            
            if (delay == TimeSpan.Zero || delay == null)
            {
                if (from is BaseCreature)
                    ((BaseCreature)from).AlterSpellDamageTo(target, ref iDamage);

                if (target is BaseCreature)
                    ((BaseCreature)target).AlterSpellDamageFrom(from, ref iDamage);
                
                WeightOverloading.DFA = dfa;

                int finalAdjustedDamage = AOS.Damage(target, from, iDamage, phys, fire, cold, pois, nrgy);

                WeightOverloading.DFA = DFAlgorithm.Standard;

                //Display Player Spell Damage
                if (pm_Caster != null)
                {
                    if (pm_Caster.m_ShowSpellDamage == DamageDisplayMode.PrivateMessage)
                        pm_Caster.SendMessage(pm_Caster.PlayerSpellDamageTextHue, "Your spell hits " + target.Name + " for " + finalAdjustedDamage.ToString() + " damage.");

                    if (pm_Caster.m_ShowSpellDamage == DamageDisplayMode.PrivateOverhead)
                        target.PrivateOverheadMessage(MessageType.Regular, pm_Caster.PlayerSpellDamageTextHue, false, "-" + finalAdjustedDamage.ToString(), pm_Caster.NetState);
                }

                //Display Follower Spell Damage
                if (bc_Caster != null)
                {
                    if (bc_Caster.Controlled && bc_Caster.ControlMaster is PlayerMobile)
                    {
                        PlayerMobile playerOwner = bc_Caster.ControlMaster as PlayerMobile;

                        if (target.GetDistanceToSqrt(playerOwner) <= 20)
                        {
                            if (playerOwner.m_ShowFollowerDamage == DamageDisplayMode.PrivateMessage)
                                playerOwner.SendMessage(playerOwner.PlayerFollowerDamageTextHue, "Follower: " + bc_Caster.Name + " casts for " + finalAdjustedDamage.ToString() + " spell damage against " + target.Name + ".");

                            if (playerOwner.m_ShowFollowerDamage == DamageDisplayMode.PrivateOverhead)
                                target.PrivateOverheadMessage(MessageType.Regular, playerOwner.PlayerFollowerDamageTextHue, false, "-" + finalAdjustedDamage.ToString(), playerOwner.NetState);
                        }
                    }
                }

                //Provoked Creature Spell Damage
                if (bc_Caster != null)
                {
                    if (bc_Caster.BardProvoked && bc_Caster.BardMaster is PlayerMobile)
                    {
                        PlayerMobile playerBard = bc_Caster.BardMaster as PlayerMobile;

                        if (target.GetDistanceToSqrt(playerBard) <= 20)
                        {
                            if (playerBard.m_ShowProvocationDamage == DamageDisplayMode.PrivateMessage)
                                playerBard.SendMessage(playerBard.PlayerProvocationDamageTextHue, "Provocation: " + bc_Caster.Name + " casts for " + finalAdjustedDamage.ToString() + " spell damage against " + target.Name + ".");

                            if (playerBard.m_ShowProvocationDamage == DamageDisplayMode.PrivateOverhead)
                                target.PrivateOverheadMessage(MessageType.Regular, playerBard.PlayerProvocationDamageTextHue, false, "-" + finalAdjustedDamage.ToString(), playerBard.NetState);
                        }
                    }
                }
            }

            else
                new SpellDamageTimerAOS(spell, target, from, iDamage, phys, fire, cold, pois, nrgy, delay, dfa).Start();

            if (target is BaseCreature && from != null && delay == TimeSpan.Zero)
            {
                BaseCreature c = (BaseCreature)target;

                c.OnHarmfulSpell(from);
                c.OnDamagedBySpell(from);
            }
        }

        public static void DamageChanceDisturb(Spell spell, Mobile target, double damage, double chance)
        {
            if (target == null || target.Deleted || !target.Alive || damage <= 0)
                return;

            Mobile from = spell.Caster;

            PlayerMobile pm_Caster = from as PlayerMobile;
            BaseCreature bc_Caster = from as BaseCreature;

            PlayerMobile pm_Target = target as PlayerMobile;
            BaseCreature bc_Target = target as BaseCreature;

            int discordancePenalty = 0;
            int adjustedDamageDisplayed = 0;

            if (Utility.RandomDouble() > chance)
            {
                TimeSpan ts = GetDamageDelayForSpell(spell, target);

                int amount = (int)damage;

                if (amount > 0)
                {
                    if (ts > TimeSpan.Zero)
                        new SpellDamageNoDisturbTimer(spell, target, from, amount, ts).Start();
                    else
                    {
                        if (from is BaseCreature)
                            ((BaseCreature)from).AlterSpellDamageTo(target, ref amount);

                        if (target is BaseCreature)
                            ((BaseCreature)target).AlterSpellDamageFrom(from, ref amount);

                        if (target is BaseCreature && from != null)
                            ((BaseCreature)target).OnDamagedBySpell(from);

                        if (!target.CanBeDamaged() || target.Deleted)
                            return;

                        if (!target.Region.OnDamage(target, ref amount))
                            return;

                        if (amount > 0)
                        {
                            int oldHits = target.Hits;
                            int newHits = oldHits - amount;

                            adjustedDamageDisplayed = amount;

                            if (bc_Target != null)
                            {
                                //Discordance
                                adjustedDamageDisplayed = (int)((double)adjustedDamageDisplayed * (1 + bc_Target.DiscordEffect));

                                //Ship Combat
                                if (BaseBoat.UseShipBasedDamageModifer(from, bc_Target))
                                    adjustedDamageDisplayed = (int)((double)adjustedDamageDisplayed * BaseBoat.shipBasedDamageToCreatureScalar);
                            }

                            if (pm_Target != null)
                            {
                                //Ship Combat
                                if (BaseBoat.UseShipBasedDamageModifer(from, pm_Target))
                                    adjustedDamageDisplayed = (int)((double)adjustedDamageDisplayed * BaseBoat.shipBasedDamageToPlayerScalar);
                            }

                            //Display Player Spell Damage
                            if (pm_Caster != null)
                            {
                                if (pm_Caster.m_ShowSpellDamage == DamageDisplayMode.PrivateMessage)
                                    pm_Caster.SendMessage(pm_Caster.PlayerSpellDamageTextHue, "Your spell hits " + target.Name + " for " + adjustedDamageDisplayed.ToString() + " damage.");

                                if (pm_Caster.m_ShowSpellDamage == DamageDisplayMode.PrivateOverhead)
                                    target.PrivateOverheadMessage(MessageType.Regular, pm_Caster.PlayerSpellDamageTextHue, false, "-" + adjustedDamageDisplayed.ToString(), pm_Caster.NetState);
                            }

                            //Display Follower Spell Damage
                            if (bc_Caster != null)
                            {
                                if (bc_Caster.Controlled && bc_Caster.ControlMaster is PlayerMobile)
                                {
                                    PlayerMobile playerOwner = bc_Caster.ControlMaster as PlayerMobile;

                                    if (target.GetDistanceToSqrt(playerOwner) <= 20)
                                    {
                                        if (playerOwner.m_ShowFollowerDamage == DamageDisplayMode.PrivateMessage)
                                            playerOwner.SendMessage(playerOwner.PlayerFollowerDamageTextHue, "Follower: " + bc_Caster.Name + " casts for " + adjustedDamageDisplayed.ToString() + " spell damage against " + target.Name + ".");

                                        if (playerOwner.m_ShowFollowerDamage == DamageDisplayMode.PrivateOverhead)
                                            target.PrivateOverheadMessage(MessageType.Regular, playerOwner.PlayerFollowerDamageTextHue, false, "-" + adjustedDamageDisplayed.ToString(), playerOwner.NetState);
                                    }
                                }
                            }

                            //Provoked Creature Spell Damage
                            if (bc_Caster != null)
                            {
                                if (bc_Caster.BardProvoked && bc_Caster.BardMaster is PlayerMobile)
                                {
                                    PlayerMobile playerBard = bc_Caster.BardMaster as PlayerMobile;

                                    if (target.GetDistanceToSqrt(playerBard) <= 20)
                                    {
                                        if (playerBard.m_ShowProvocationDamage == DamageDisplayMode.PrivateMessage)
                                            playerBard.SendMessage(playerBard.PlayerProvocationDamageTextHue, "Provocation: " + bc_Caster.Name + " casts for " + adjustedDamageDisplayed.ToString() + " spell damage against " + target.Name + ".");

                                        if (playerBard.m_ShowProvocationDamage == DamageDisplayMode.PrivateOverhead)
                                            target.PrivateOverheadMessage(MessageType.Regular, playerBard.PlayerProvocationDamageTextHue, false, "-" + adjustedDamageDisplayed.ToString(), playerBard.NetState);
                                    }
                                }
                            }

                            if (from != null)
                                target.RegisterDamage(amount, from);

                            target.Paralyzed = false;

                            target.OnDamage(amount, from, newHits < 0);

                            IMount m = target.Mount;

                            if (m != null)
                                m.OnRiderDamaged(amount, from, newHits < 0);

                            if (newHits < 0)
                            {
                                target.LastKiller = from;
                                target.Hits = 0;

                                if (oldHits >= 0)
                                    target.Kill();
                            }

                            else
                                target.Hits = newHits;
                        }
                    }
                }
            }

            else
            {
                adjustedDamageDisplayed = (int)damage;

                if (bc_Target != null)
                {
                    //Discordance
                    adjustedDamageDisplayed = (int)((double)adjustedDamageDisplayed * (1 + bc_Target.DiscordEffect));
                    
                    //Ship Combat
                    if (BaseBoat.UseShipBasedDamageModifer(from, bc_Target))
                        adjustedDamageDisplayed = (int)((double)adjustedDamageDisplayed * BaseBoat.shipBasedDamageToCreatureScalar);
                }

                if (pm_Target != null)
                {
                    //Ship Combat
                    if (BaseBoat.UseShipBasedDamageModifer(from, pm_Target))
                        adjustedDamageDisplayed = (int)((double)adjustedDamageDisplayed * BaseBoat.shipBasedDamageToPlayerScalar);
                }

                //Display Player Spell Damage
                if (pm_Caster != null)
                {
                    if (pm_Caster.m_ShowSpellDamage == DamageDisplayMode.PrivateMessage)
                        pm_Caster.SendMessage(pm_Caster.PlayerSpellDamageTextHue, "Your spell hits " + target.Name + " for " + adjustedDamageDisplayed.ToString() + " damage.");

                    if (pm_Caster.m_ShowSpellDamage == DamageDisplayMode.PrivateOverhead)
                        target.PrivateOverheadMessage(MessageType.Regular, pm_Caster.PlayerSpellDamageTextHue, false, "-" + adjustedDamageDisplayed.ToString(), pm_Caster.NetState);
                }

                //Display Follower Spell Damage
                if (bc_Caster != null)
                {
                    if (bc_Caster.Controlled && bc_Caster.ControlMaster is PlayerMobile)
                    {
                        PlayerMobile playerOwner = bc_Caster.ControlMaster as PlayerMobile;

                        if (target.GetDistanceToSqrt(playerOwner) <= 20)
                        {
                            if (playerOwner.m_ShowFollowerDamage == DamageDisplayMode.PrivateMessage)
                                playerOwner.SendMessage(playerOwner.PlayerFollowerDamageTextHue, "Follower: " + bc_Caster.Name + " casts for " + adjustedDamageDisplayed.ToString() + " spell damage against " + target.Name + ".");

                            if (playerOwner.m_ShowFollowerDamage == DamageDisplayMode.PrivateOverhead)
                                target.PrivateOverheadMessage(MessageType.Regular, playerOwner.PlayerFollowerDamageTextHue, false, "-" + adjustedDamageDisplayed.ToString(), playerOwner.NetState);
                        }
                    }
                }

                //Provoked Creature Spell Damage
                if (bc_Caster != null)
                {
                    if (bc_Caster.BardProvoked && bc_Caster.BardMaster is PlayerMobile)
                    {
                        PlayerMobile playerBard = bc_Caster.BardMaster as PlayerMobile;

                        if (target.GetDistanceToSqrt(playerBard) <= 20)
                        {
                            if (playerBard.m_ShowProvocationDamage == DamageDisplayMode.PrivateMessage)
                                playerBard.SendMessage(playerBard.PlayerProvocationDamageTextHue, "Provocation: " + bc_Caster.Name + " casts for " + adjustedDamageDisplayed.ToString() + " spell damage against " + target.Name + ".");

                            if (playerBard.m_ShowProvocationDamage == DamageDisplayMode.PrivateOverhead)
                                target.PrivateOverheadMessage(MessageType.Regular, playerBard.PlayerProvocationDamageTextHue, false, "-" + adjustedDamageDisplayed.ToString(), playerBard.NetState);
                        }
                    }
                }

                SpellHelper.Damage(spell, target, damage);
            }
        }

        public static void Heal(int amount, Mobile target, Mobile from)
        {
            Heal(amount, target, from, true);
        }

        public static void Heal(int amount, Mobile target, Mobile from, bool message)
        {
            //TODO: All Healing *spells* go through ArcaneEmpowerment
            target.Heal(amount, from, message);
        }        

        private class SpellDamageTimer : Timer
        {
            private Mobile m_Target, m_From;
            private int m_Damage;
            private Spell m_Spell;

            public SpellDamageTimer(Spell s, Mobile target, Mobile from, int damage, TimeSpan delay)
                : base(delay)
            {
                m_Target = target;
                m_From = from;
                m_Damage = damage;
                m_Spell = s;
                Priority = TimerPriority.TwentyFiveMS;
            }

            protected override void OnTick()
            {
                //Add logic to check for damage bonus on the mob
                if (m_Target is BaseCreature && m_From != null)
                    ((BaseCreature)m_Target).AlterSpellDamageFrom(m_From, ref m_Damage);

                // IPY
                m_Target.Damage(m_Damage);
            }
        }

        private class SpellDamageNoDisturbTimer : Timer
        {
            private Mobile m_Target, m_From;
            private int m_Damage;
            private Spell m_Spell;

            public SpellDamageNoDisturbTimer(Spell s, Mobile target, Mobile from, int damage, TimeSpan delay)
                : base(delay)
            {
                m_Target = target;
                m_From = from;
                m_Damage = damage;
                m_Spell = s;
                Priority = TimerPriority.TwentyFiveMS;
            }

            protected override void OnTick()
            {
                PlayerMobile pm_Caster = m_From as PlayerMobile;
                BaseCreature bc_Caster = m_From as BaseCreature;

                BaseCreature bc_Target = m_Target as BaseCreature;
                PlayerMobile pm_Target = m_Target as PlayerMobile;
                
                if (m_From is BaseCreature)
                    ((BaseCreature)m_From).AlterSpellDamageTo(m_Target, ref m_Damage);

                if (m_Target is BaseCreature)
                    ((BaseCreature)m_Target).AlterSpellDamageFrom(m_From, ref m_Damage);

                if (m_Target is BaseCreature && m_From != null)
                    ((BaseCreature)m_Target).OnDamagedBySpell(m_From);

                if (!m_Target.CanBeDamaged() || m_Target.Deleted)
                    return;

                if (!m_Target.Region.OnDamage(m_Target, ref m_Damage))
                    return;

                if (m_Damage > 0)
                {
                    int oldHits = m_Target.Hits;
                    int newHits = oldHits - m_Damage;

                    int discordancePenalty = 0;
                    int adjustedDamageDisplayed = m_Damage;

                    if (bc_Target != null)
                    {                        
                        //Discordance
                        adjustedDamageDisplayed = (int)((double)adjustedDamageDisplayed * (1 + bc_Target.DiscordEffect));

                        //Ship Combat
                        if (BaseBoat.UseShipBasedDamageModifer(m_From, bc_Target))
                            adjustedDamageDisplayed = (int)((double)adjustedDamageDisplayed * BaseBoat.shipBasedDamageToCreatureScalar);
                    }

                    if (pm_Target != null)
                    {
                        //Ship Combat
                        if (BaseBoat.UseShipBasedDamageModifer(m_From, pm_Target))
                            adjustedDamageDisplayed = (int)((double)adjustedDamageDisplayed * BaseBoat.shipBasedDamageToPlayerScalar);
                    }

                    //Display Player Spell Damage
                    if (pm_Caster != null)
                    {
                        if (pm_Caster.m_ShowSpellDamage == DamageDisplayMode.PrivateMessage)
                            pm_Caster.SendMessage(pm_Caster.PlayerSpellDamageTextHue, "Your spell hits " + m_Target.Name + " for " + adjustedDamageDisplayed.ToString() + " damage.");

                        if (pm_Caster.m_ShowSpellDamage == DamageDisplayMode.PrivateOverhead)
                            m_Target.PrivateOverheadMessage(MessageType.Regular, pm_Caster.PlayerSpellDamageTextHue, false, "-" + adjustedDamageDisplayed.ToString(), pm_Caster.NetState);
                    }

                    //Display Follower Spell Damage
                    if (bc_Caster != null)
                    {
                        if (bc_Caster.Controlled && bc_Caster.ControlMaster is PlayerMobile)
                        {
                            PlayerMobile playerOwner = bc_Caster.ControlMaster as PlayerMobile;

                            if (m_Target.GetDistanceToSqrt(playerOwner) <= 20)
                            {
                                if (playerOwner.m_ShowFollowerDamage == DamageDisplayMode.PrivateMessage)
                                    playerOwner.SendMessage(playerOwner.PlayerFollowerDamageTextHue, "Follower: " + bc_Caster.Name + " casts for " + adjustedDamageDisplayed.ToString() + " spell damage against " + m_Target.Name + ".");

                                if (playerOwner.m_ShowFollowerDamage == DamageDisplayMode.PrivateOverhead)
                                    m_Target.PrivateOverheadMessage(MessageType.Regular, playerOwner.PlayerFollowerDamageTextHue, false, "-" + adjustedDamageDisplayed.ToString(), playerOwner.NetState);
                            }
                        }
                    }

                    //Provoked Creature Spell Damage
                    if (bc_Caster != null)
                    {
                        if (bc_Caster.BardProvoked && bc_Caster.BardMaster is PlayerMobile)
                        {
                            PlayerMobile playerBard = bc_Caster.BardMaster as PlayerMobile;

                            if (m_Target.GetDistanceToSqrt(playerBard) <= 20)
                            {
                                if (playerBard.m_ShowProvocationDamage == DamageDisplayMode.PrivateMessage)
                                    playerBard.SendMessage(playerBard.PlayerProvocationDamageTextHue, "Provocation: " + bc_Caster.Name + " casts for " + adjustedDamageDisplayed.ToString() + " spell damage against " + m_Target.Name + ".");

                                if (playerBard.m_ShowProvocationDamage == DamageDisplayMode.PrivateOverhead)
                                    m_Target.PrivateOverheadMessage(MessageType.Regular, playerBard.PlayerProvocationDamageTextHue, false, "-" + adjustedDamageDisplayed.ToString(), playerBard.NetState);
                            }
                        }
                    }

                    if (m_From != null)
                        m_Target.RegisterDamage(m_Damage, m_From);

                    m_Target.Paralyzed = false;

                    m_Target.OnDamage(m_Damage, m_From, newHits < 0);

                    IMount m = m_Target.Mount;

                    if (m != null)
                        m.OnRiderDamaged(m_Damage, m_From, newHits < 0);

                    if (newHits < 0)
                    {
                        m_Target.LastKiller = m_From;

                        m_Target.Hits = 0;

                        if (oldHits >= 0)
                            m_Target.Kill();
                    }

                    else
                        m_Target.Hits = newHits;
                }
            }
        }

        private class SpellDamageTimerAOS : Timer
        {
            private Mobile m_Target, m_From;
            private int m_Damage;
            private int m_Phys, m_Fire, m_Cold, m_Pois, m_Nrgy;
            private DFAlgorithm m_DFA;
            private Spell m_Spell;

            public SpellDamageTimerAOS(Spell s, Mobile target, Mobile from, int damage, int phys, int fire, int cold, int pois, int nrgy, TimeSpan delay, DFAlgorithm dfa)
                : base(delay)
            {
                m_Target = target;
                m_From = from;
                m_Damage = damage;
                m_Phys = phys;
                m_Fire = fire;
                m_Cold = cold;
                m_Pois = pois;
                m_Nrgy = nrgy;
                m_DFA = dfa;
                m_Spell = s;

                Priority = TimerPriority.TwentyFiveMS;
            }

            protected override void OnTick()
            {
                PlayerMobile pm_Caster = m_From as PlayerMobile;
                BaseCreature bc_Caster = m_From as BaseCreature;

                PlayerMobile pm_Target = m_Target as PlayerMobile;
                BaseCreature bc_Target = m_Target as BaseCreature;
                
                if (m_From is BaseCreature && m_Target != null)
                    ((BaseCreature)m_From).AlterSpellDamageTo(m_Target, ref m_Damage);

                if (m_Target is BaseCreature && m_From != null)
                    ((BaseCreature)m_Target).AlterSpellDamageFrom(m_From, ref m_Damage);
                
                WeightOverloading.DFA = m_DFA;
                int finalAdjustedDamage = AOS.Damage(m_Target, m_From, m_Damage, m_Phys, m_Fire, m_Cold, m_Pois, m_Nrgy);
                WeightOverloading.DFA = DFAlgorithm.Standard;

                //Display Player Spell Damage
                if (pm_Caster != null)
                {
                    if (pm_Caster.m_ShowSpellDamage == DamageDisplayMode.PrivateMessage)
                        pm_Caster.SendMessage(pm_Caster.PlayerSpellDamageTextHue, "Your spell hits " + m_Target.Name + " for " + finalAdjustedDamage.ToString() + " damage.");

                    if (pm_Caster.m_ShowSpellDamage == DamageDisplayMode.PrivateOverhead)
                        m_Target.PrivateOverheadMessage(MessageType.Regular, pm_Caster.PlayerSpellDamageTextHue, false, "-" + finalAdjustedDamage.ToString(), pm_Caster.NetState);
                }

                //Display Follower Spell Damage
                if (bc_Caster != null)
                {
                    if (bc_Caster.Controlled && bc_Caster.ControlMaster is PlayerMobile)
                    {
                        PlayerMobile playerOwner = bc_Caster.ControlMaster as PlayerMobile;

                        if (m_Target.GetDistanceToSqrt(playerOwner) <= 20)
                        {
                            if (playerOwner.m_ShowFollowerDamage == DamageDisplayMode.PrivateMessage)
                                playerOwner.SendMessage(playerOwner.PlayerFollowerDamageTextHue, "Follower: " + bc_Caster.Name + " casts for " + finalAdjustedDamage.ToString() + " spell damage against " + m_Target.Name + ".");

                            if (playerOwner.m_ShowFollowerDamage == DamageDisplayMode.PrivateOverhead)
                                m_Target.PrivateOverheadMessage(MessageType.Regular, playerOwner.PlayerFollowerDamageTextHue, false, "-" + finalAdjustedDamage.ToString(), playerOwner.NetState);
                        }
                    }
                }

                //Provoked Creature Spell Damage
                if (bc_Caster != null)
                {
                    if (bc_Caster.BardProvoked && bc_Caster.BardMaster is PlayerMobile)
                    {
                        PlayerMobile playerBard = bc_Caster.BardMaster as PlayerMobile;

                        if (m_Target.GetDistanceToSqrt(playerBard) <= 20)
                        {
                            if (playerBard.m_ShowProvocationDamage == DamageDisplayMode.PrivateMessage)
                                playerBard.SendMessage(playerBard.PlayerProvocationDamageTextHue, "Provocation: " + bc_Caster.Name + " casts for " + finalAdjustedDamage.ToString() + " spell damage against " + m_Target.Name + ".");

                            if (playerBard.m_ShowProvocationDamage == DamageDisplayMode.PrivateOverhead)
                                m_Target.PrivateOverheadMessage(MessageType.Regular, playerBard.PlayerProvocationDamageTextHue, false, "-" + finalAdjustedDamage.ToString(), playerBard.NetState);
                        }
                    }
                }

                if (m_Target is BaseCreature && m_From != null)
                {
                    BaseCreature c = (BaseCreature)m_Target;

                    c.OnHarmfulSpell(m_From);
                    c.OnDamagedBySpell(m_From);
                }
            }
        }
    }

    public class TransformationSpellHelper
    {
        #region Context Stuff
        private static Dictionary<Mobile, TransformContext> m_Table = new Dictionary<Mobile, TransformContext>();

        public static void AddContext(Mobile m, TransformContext context)
        {
            m_Table[m] = context;
        }

        public static void RemoveContext(Mobile m, bool resetGraphics)
        {
            TransformContext context = GetContext(m);

            if (context != null)
                RemoveContext(m, context, resetGraphics);
        }

        public static void RemoveContext(Mobile m, TransformContext context, bool resetGraphics)
        {
            if (m_Table.ContainsKey(m))
            {
                m_Table.Remove(m);

                List<ResistanceMod> mods = context.Mods;

                for (int i = 0; i < mods.Count; ++i)
                    m.RemoveResistanceMod(mods[i]);

                if (resetGraphics)
                {
                    m.HueMod = -1;
                    m.BodyMod = 0;
                }

                context.Timer.Stop();
                context.Spell.RemoveEffect(m);
            }
        }

        public static TransformContext GetContext(Mobile m)
        {
            TransformContext context = null;

            m_Table.TryGetValue(m, out context);

            return context;
        }

        public static bool UnderTransformation(Mobile m)
        {
            return (GetContext(m) != null);
        }

        public static bool UnderTransformation(Mobile m, Type type)
        {
            TransformContext context = GetContext(m);

            return (context != null && context.Type == type);
        }
        #endregion

        public static bool CheckCast(Mobile caster, Spell spell)
        {
            if (Factions.Sigil.ExistsOn(caster))
            {
                caster.SendLocalizedMessage(1061632); // You can't do that while carrying the sigil.
                return false;
            }

            else if (!caster.CanBeginAction(typeof(PolymorphSpell)))
            {
                caster.SendLocalizedMessage(1061628); // You can't do that while polymorphed.
                return false;
            }

            return true;
        }

        public static bool OnCast(Mobile caster, Spell spell)
        {
            ITransformationSpell transformSpell = spell as ITransformationSpell;

            if (transformSpell == null)
                return false;

            if (Factions.Sigil.ExistsOn(caster))
            {
                caster.SendLocalizedMessage(1061632); // You can't do that while carrying the sigil.
            }
            else if (!caster.CanBeginAction(typeof(PolymorphSpell)))
            {
                caster.SendLocalizedMessage(1061628); // You can't do that while polymorphed.
            }
            else if (DisguiseTimers.IsDisguised(caster))
            {
                caster.SendLocalizedMessage(1061631); // You can't do that while disguised.
                return false;
            }
            else if (!caster.CanBeginAction(typeof(IncognitoSpell)) || (caster.IsBodyMod && GetContext(caster) == null))
            {
                spell.DoFizzle();
            }
            else if (spell.CheckSequence())
            {
                TransformContext context = GetContext(caster);
                Type ourType = spell.GetType();

                bool wasTransformed = (context != null);
                bool ourTransform = (wasTransformed && context.Type == ourType);

                if (wasTransformed)
                {
                    RemoveContext(caster, context, ourTransform);

                    if (ourTransform)
                    {
                        caster.PlaySound(0xFA);
                        caster.FixedParticles(0x3728, 1, 13, 5042, EffectLayer.Waist);
                    }
                }

                if (!ourTransform)
                {
                    List<ResistanceMod> mods = new List<ResistanceMod>();

                    if (transformSpell.PhysResistOffset != 0)
                        mods.Add(new ResistanceMod(ResistanceType.Physical, transformSpell.PhysResistOffset));

                    if (transformSpell.FireResistOffset != 0)
                        mods.Add(new ResistanceMod(ResistanceType.Fire, transformSpell.FireResistOffset));

                    if (transformSpell.ColdResistOffset != 0)
                        mods.Add(new ResistanceMod(ResistanceType.Cold, transformSpell.ColdResistOffset));

                    if (transformSpell.PoisResistOffset != 0)
                        mods.Add(new ResistanceMod(ResistanceType.Poison, transformSpell.PoisResistOffset));

                    if (transformSpell.NrgyResistOffset != 0)
                        mods.Add(new ResistanceMod(ResistanceType.Energy, transformSpell.NrgyResistOffset));

                    if (!((Body)transformSpell.Body).IsHuman)
                    {
                        Mobiles.IMount mt = caster.Mount;

                        if (mt != null)
                            mt.Rider = null;
                    }

                    caster.BodyMod = transformSpell.Body;
                    caster.HueMod = transformSpell.Hue;

                    for (int i = 0; i < mods.Count; ++i)
                        caster.AddResistanceMod(mods[i]);

                    transformSpell.DoEffect(caster);

                    Timer timer = new TransformTimer(caster, transformSpell);
                    timer.Start();

                    AddContext(caster, new TransformContext(timer, mods, ourType, transformSpell));
                    return true;
                }
            }

            return false;
        }
    }

    public interface ITransformationSpell
    {
        int Body { get; }
        int Hue { get; }

        int PhysResistOffset { get; }
        int FireResistOffset { get; }
        int ColdResistOffset { get; }
        int PoisResistOffset { get; }
        int NrgyResistOffset { get; }

        double TickRate { get; }
        void OnTick(Mobile m);

        void DoEffect(Mobile m);
        void RemoveEffect(Mobile m);
    }


    public class TransformContext
    {
        private Timer m_Timer;
        private List<ResistanceMod> m_Mods;
        private Type m_Type;
        private ITransformationSpell m_Spell;

        public Timer Timer { get { return m_Timer; } }
        public List<ResistanceMod> Mods { get { return m_Mods; } }
        public Type Type { get { return m_Type; } }
        public ITransformationSpell Spell { get { return m_Spell; } }

        public TransformContext(Timer timer, List<ResistanceMod> mods, Type type, ITransformationSpell spell)
        {
            m_Timer = timer;
            m_Mods = mods;
            m_Type = type;
            m_Spell = spell;
        }
    }

    public class TransformTimer : Timer
    {
        private Mobile m_Mobile;
        private ITransformationSpell m_Spell;

        public TransformTimer(Mobile from, ITransformationSpell spell)
            : base(TimeSpan.FromSeconds(spell.TickRate), TimeSpan.FromSeconds(spell.TickRate))
        {
            m_Mobile = from;
            m_Spell = spell;

            Priority = TimerPriority.TwoFiftyMS;
        }

        protected override void OnTick()
        {
            if (m_Mobile.Deleted || !m_Mobile.Alive || m_Mobile.Body != m_Spell.Body || m_Mobile.Hue != m_Spell.Hue)
            {
                TransformationSpellHelper.RemoveContext(m_Mobile, true);
                Stop();
            }
            else
            {
                m_Spell.OnTick(m_Mobile);
            }
        }
    }
}
