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

namespace Server.Custom
{
    //Warning: Do Not Remove Any Entries From Here
    public enum HueableSpell
    {
        MagicArrow,
        Clumsy,
        Feeblemind,
        Weaken,
        ReactiveArmor,

        Harm,
        Cure,
        Protection,

        Fireball,
        Bless,       
        WallOfStone,
        Teleport,

        ArchCure,
        ArchProtection,
        Curse,

        BladeSpirits,
        MagicReflect,
        MindBlast,
        SummonCreature,

        Dispel,
        EnergyBolt,
        Explosion,
        MassCurse,
        ParalyzeField,

        EnergyField,
        Flamestrike,     
        MassDispel,
        MeteorSwarm,
        
        AirElemental,
        EarthElemental,
        FireElemental,
        WaterElemental,
        SummonDaemon,
        EnergyVortex,
    }

    public enum SpellHueType
    {
        Basic,
        Icy, 
        Fiery,
        Earthy,
        Mystical,
        Ghostly        
    }  

    public static class SpellHue
    {
        public static HueableSpell GetRandomHueableSpell()
        {
            int spellCount = Enum.GetNames(typeof(HueableSpell)).Length;

            HueableSpell hueableSpell = (HueableSpell)Utility.RandomMinMax(0, spellCount - 1);

            return hueableSpell;
        }

        public static SpellHueType GetRandomSpellHue()
        {
            SpellHueType spellHue = SpellHueType.Basic;

            int randomSpellHue = Utility.RandomMinMax(1, 5);

            switch (randomSpellHue)
            {
                case 1: spellHue = SpellHueType.Icy; break;
                case 2: spellHue = SpellHueType.Fiery; break;
                case 3: spellHue = SpellHueType.Earthy; break;
                case 4: spellHue = SpellHueType.Mystical; break;
                case 5: spellHue = SpellHueType.Ghostly; break;
            }

            return spellHue;
        }

        public static SpellHueTypeDetail GetSpellHueTypeDetail(SpellHueType spellHueType)
        {
            SpellHueTypeDetail spellHueTypeDetail = new SpellHueTypeDetail();

            switch (spellHueType)
            {
                case SpellHueType.Basic:
                    spellHueTypeDetail.m_Name = "Basic";
                    spellHueTypeDetail.m_AnimationHue = 0;
                    spellHueTypeDetail.m_DisplayHue = 0;
                    break;

                case SpellHueType.Fiery:
                    spellHueTypeDetail.m_Name = "Fiery";
                    spellHueTypeDetail.m_AnimationHue = 2116;
                    spellHueTypeDetail.m_DisplayHue = 2117;
                    break;

                case SpellHueType.Icy:
                    spellHueTypeDetail.m_Name = "Icy";
                    spellHueTypeDetail.m_AnimationHue = 2578;
                    spellHueTypeDetail.m_DisplayHue = 2579;
                    break;

                case SpellHueType.Earthy:
                    spellHueTypeDetail.m_Name = "Earthy";
                    spellHueTypeDetail.m_AnimationHue = 2552;
                    spellHueTypeDetail.m_DisplayHue = 2553;
                    break;

                case SpellHueType.Mystical:
                    spellHueTypeDetail.m_Name = "Mystical";
                    spellHueTypeDetail.m_AnimationHue = 2641;
                    spellHueTypeDetail.m_DisplayHue = 2642;
                    break;

                case SpellHueType.Ghostly:
                    spellHueTypeDetail.m_Name = "Ghostly";
                    spellHueTypeDetail.m_AnimationHue = 2598;
                    spellHueTypeDetail.m_DisplayHue = 2599;
                    break;
            }

            return spellHueTypeDetail;
        }

        public static HueableSpellDetail GetHueableSpellDetail(HueableSpell hueableSpell)
        {
            HueableSpellDetail hueableSpellDetail = new HueableSpellDetail();

            switch (hueableSpell)
            {
                case HueableSpell.MagicArrow:
                hueableSpellDetail.m_SpellName = "Magic Arrow";
                hueableSpellDetail.m_IconItemId = 8324;
                break;

                /*
                case HueableSpell.Heal:
                hueableSpellDetail.m_SpellName = "Heal";
                hueableSpellDetail.m_IconItemId = 8323;
                break;
                */

                case HueableSpell.Clumsy:
                hueableSpellDetail.m_SpellName = "Clumsy";
                hueableSpellDetail.m_IconItemId = 8320;
                break;

                case HueableSpell.Feeblemind:
                hueableSpellDetail.m_SpellName = "Feeblemind";
                hueableSpellDetail.m_IconItemId = 8322;
                break;

                case HueableSpell.Weaken:
                hueableSpellDetail.m_SpellName = "Weaken";
                hueableSpellDetail.m_IconItemId = 8327;
                break;

                case HueableSpell.ReactiveArmor:
                hueableSpellDetail.m_SpellName = "Reactive Armor";
                hueableSpellDetail.m_IconItemId = 8326;
                break;

                case HueableSpell.Harm:
                hueableSpellDetail.m_SpellName = "Harm";
                hueableSpellDetail.m_IconItemId = 8331;
                break;

                case HueableSpell.Cure:
                hueableSpellDetail.m_SpellName = "Cure";
                hueableSpellDetail.m_IconItemId = 8330;
                break;

                /*
                case HueableSpell.MagicTrap:
                hueableSpellDetail.m_SpellName = "Magic Trap";
                hueableSpellDetail.m_IconItemId = 8332;
                break;
                */

                case HueableSpell.Protection:
                hueableSpellDetail.m_SpellName = "Protection";
                hueableSpellDetail.m_IconItemId = 8334;
                break;

                case HueableSpell.Fireball:
                hueableSpellDetail.m_SpellName = "Fireball";
                hueableSpellDetail.m_IconItemId = 8337;
                break;

                case HueableSpell.Bless:
                hueableSpellDetail.m_SpellName = "Bless";
                hueableSpellDetail.m_IconItemId = 8336;
                break;

                case HueableSpell.WallOfStone:
                hueableSpellDetail.m_SpellName = "Wall of Stone";
                hueableSpellDetail.m_IconItemId = 8343;
                break;

                case HueableSpell.Teleport:
                hueableSpellDetail.m_SpellName = "Teleport";
                hueableSpellDetail.m_IconItemId = 8341;
                break;

                case HueableSpell.ArchCure:
                hueableSpellDetail.m_SpellName = "Arch Cure";
                hueableSpellDetail.m_IconItemId = 8344;
                break;

                case HueableSpell.ArchProtection:
                hueableSpellDetail.m_SpellName = "Arch Protection";
                hueableSpellDetail.m_IconItemId = 8345;
                break;

                case HueableSpell.Curse:
                hueableSpellDetail.m_SpellName = "Curse";
                hueableSpellDetail.m_IconItemId = 8346;
                break;

                /*
                case HueableSpell.GreaterHeal:
                hueableSpellDetail.m_SpellName = "Greater Heal";
                hueableSpellDetail.m_IconItemId = 8348;
                break;
                */

                case HueableSpell.BladeSpirits:
                hueableSpellDetail.m_SpellName = "Blade Spirits";
                hueableSpellDetail.m_IconItemId = 8352;
                break;

                case HueableSpell.MagicReflect:
                hueableSpellDetail.m_SpellName = "Magic Reflect";
                hueableSpellDetail.m_IconItemId = 8355;
                break;

                case HueableSpell.MindBlast:
                hueableSpellDetail.m_SpellName = "Mindblast";
                hueableSpellDetail.m_IconItemId = 8356;
                break;

                /*
                case HueableSpell.Paralyze:
                hueableSpellDetail.m_SpellName = "Paralyze";
                hueableSpellDetail.m_IconItemId = 8357;
                break;
                */

                case HueableSpell.SummonCreature:
                hueableSpellDetail.m_SpellName = "Summon Creature";
                hueableSpellDetail.m_IconItemId = 8359;
                break;

                case HueableSpell.Dispel:
                hueableSpellDetail.m_SpellName = "Dispel";
                hueableSpellDetail.m_IconItemId = 8360;
                break;

                case HueableSpell.EnergyBolt:
                hueableSpellDetail.m_SpellName = "Energy Bolt";
                hueableSpellDetail.m_IconItemId = 8361;
                break;

                case HueableSpell.Explosion:
                hueableSpellDetail.m_SpellName = "Explosion";
                hueableSpellDetail.m_IconItemId = 8362;
                break;

                /*
                case HueableSpell.Invisibility:
                hueableSpellDetail.m_SpellName = "Invisibility";
                hueableSpellDetail.m_IconItemId = 8363;
                break;
                */

                /*
                case HueableSpell.Mark:
                hueableSpellDetail.m_SpellName = "Mark";
                hueableSpellDetail.m_IconItemId = 8364;
                break;
                */

                case HueableSpell.MassCurse:
                hueableSpellDetail.m_SpellName = "Mass Curse";
                hueableSpellDetail.m_IconItemId = 8365;
                break;

                case HueableSpell.ParalyzeField:
                hueableSpellDetail.m_SpellName = "Paralyze Field";
                hueableSpellDetail.m_IconItemId = 8366;
                break;

                /*
                case HueableSpell.Reveal:
                hueableSpellDetail.m_SpellName = "Reveal";
                hueableSpellDetail.m_IconItemId = 8367;
                break;
                */

                case HueableSpell.EnergyField:
                hueableSpellDetail.m_SpellName = "Energy Field";
                hueableSpellDetail.m_IconItemId = 8369;
                break;

                case HueableSpell.Flamestrike:
                hueableSpellDetail.m_SpellName = "Flamestrike";
                hueableSpellDetail.m_IconItemId = 8370;
                break;

                case HueableSpell.MassDispel:
                hueableSpellDetail.m_SpellName = "Mass Dispel";
                hueableSpellDetail.m_IconItemId = 8373;
                break;

                case HueableSpell.MeteorSwarm:
                hueableSpellDetail.m_SpellName = "Meteor Swarm";
                hueableSpellDetail.m_IconItemId = 8374;
                break;

                case HueableSpell.AirElemental:
                hueableSpellDetail.m_SpellName = "Air Elemental";
                hueableSpellDetail.m_IconItemId = 8379;
                break;

                case HueableSpell.EarthElemental:
                hueableSpellDetail.m_SpellName = "Earth Elemental";
                hueableSpellDetail.m_IconItemId = 8381;
                break;

                case HueableSpell.FireElemental:
                hueableSpellDetail.m_SpellName = "Fire Elemental";
                hueableSpellDetail.m_IconItemId = 8382;
                break;

                case HueableSpell.WaterElemental:
                hueableSpellDetail.m_SpellName = "Water Elemental";
                hueableSpellDetail.m_IconItemId = 8383;
                break;

                case HueableSpell.SummonDaemon:
                hueableSpellDetail.m_SpellName = "Summon Daemon";
                hueableSpellDetail.m_IconItemId = 8380;
                break;

                case HueableSpell.EnergyVortex:
                hueableSpellDetail.m_SpellName = "Energy Vortex";
                hueableSpellDetail.m_IconItemId = 8377;
                break;

                /*
                case HueableSpell.Resurrection:
                hueableSpellDetail.m_SpellName = "Resurrection";
                hueableSpellDetail.m_IconItemId = 8378;
                break;
                */
            }

            return hueableSpellDetail;
        }        

        public static int GetSpellHue(SpellHueType spellHueType)
        {
            return GetSpellHueTypeDetail(spellHueType).m_AnimationHue;
        }
    }  

    public class HueableSpellDetail
    {
        public string m_SpellName = "Magic Arrow";
        public int m_IconItemId = 8324;

        public HueableSpellDetail()
        {
        }
    }

    public class SpellHueTypeDetail
    {
        public string m_Name = "Basic";
        public int m_AnimationHue = 0;
        public int m_DisplayHue = 0;

        public SpellHueTypeDetail()
        {
        }
    }

    public class SpellHueEntry
    {
        public HueableSpell m_HueableSpell = HueableSpell.AirElemental;
        public SpellHueType m_ActiveHue = SpellHueType.Basic;
        public List<SpellHueType> m_UnlockedHues = new List<SpellHueType>();

        public SpellHueEntry(HueableSpell hueableSpell, SpellHueType activeHue)
        {
            m_HueableSpell = hueableSpell;
            m_ActiveHue = activeHue;

            m_UnlockedHues.Add(SpellHueType.Basic);
        }
    }
}