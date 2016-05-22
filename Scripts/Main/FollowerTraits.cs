using System;
using Server;
using Server.Network;
using Server.Mobiles;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Spells;

namespace Server
{
    public enum FollowerTraitType
    {
        None,
        Toughness,
        Quickness,
        Vicious,
        Hardness,
        Unerring,
        Evasion,
        Arcane,
        SpellPenetration,
        SpellPotency,
        Charged,

        Enchanted,
        Venomous,
        Toxicity,
        Tolerance,
        Mender,
        Regeneration,
        Ethereal,
        Nullify,
        Mirror,
        Taunt,

        Steadfast,
        Purge,
        PoisonEater,
        VileFury,
        Berserk,
        Bloodlust,
        Execute,
        Ambush,
        Surprise,
        Disorient,

        Debilitate,
        Cripple,
        Pierce,
        Frenzy,
        Enrage,
        Entangle,
        Hinder,
        Rend,
        Contagion,
    }

    public static class FollowerTraits
    {
        public static FollowerTraitDetail GetFollowerTraitDetail(FollowerTraitType followerTraitType)
        {
            FollowerTraitDetail traitDetail = new FollowerTraitDetail();

            switch (followerTraitType)
            {
                case FollowerTraitType.Toughness:
                    traitDetail.Name = "Toughness";
                    traitDetail.Description = "Hit Points increased by 10%";

                    traitDetail.IconItemId = 3849;
                    traitDetail.IconHue = 0;
                    traitDetail.IconOffsetX = 5;
                    traitDetail.IconOffsetY = 8;
                    break;

                case FollowerTraitType.Quickness:
                    traitDetail.Name = "Quickness";
                    traitDetail.Description = "Dexterity increased by 10%";

                    traitDetail.IconItemId = 3848;
                    traitDetail.IconHue = 0;
                    traitDetail.IconOffsetX = 5;
                    traitDetail.IconOffsetY = 8;
                break;

                case FollowerTraitType.Vicious:
                    traitDetail.Name = "Vicious";
                    traitDetail.Description = "Melee damage increased by 10%";

                    traitDetail.IconItemId = 11704;
                    traitDetail.IconHue = 0;
                    traitDetail.IconOffsetX = 5;
                    traitDetail.IconOffsetY = 8;
                break;

                case FollowerTraitType.Hardness:
                    traitDetail.Name = "Hardness";
                    traitDetail.Description = "Armor increased by 20%";

                    traitDetail.IconItemId = 7028;
                    traitDetail.IconHue = 0;
                    traitDetail.IconOffsetX = -5;
                    traitDetail.IconOffsetY = 0;
                break;

                case FollowerTraitType.Unerring:
                    traitDetail.Name = "Unerring";
                    traitDetail.Description = "Melee Accuracy increased by 5%";

                    traitDetail.IconItemId = 7729;
                    traitDetail.IconHue = 0;
                    traitDetail.IconOffsetX = 5;
                    traitDetail.IconOffsetY = 3;
                break;

                case FollowerTraitType.Evasion:
                    traitDetail.Name = "Evasion";
                    traitDetail.Description = "Melee Defense increased by 5%";

                    traitDetail.IconItemId = 14268;
                    traitDetail.IconHue = 0;
                    traitDetail.IconOffsetX = -15;
                    traitDetail.IconOffsetY = -51;
                break;

                case FollowerTraitType.Arcane:
                    traitDetail.Name = "Arcane";
                    traitDetail.Description = "Magery increased by 20%";

                   traitDetail.IconItemId = 3834;
                    traitDetail.IconHue = 2632;
                    traitDetail.IconOffsetX = 0;
                    traitDetail.IconOffsetY = 7;
                break;

                case FollowerTraitType.SpellPenetration:
                    traitDetail.Name = "Spell Penetration";
                    traitDetail.Description = "Eval Int increased by 20%";

                    traitDetail.IconItemId = 3570;
                    traitDetail.IconHue = 2632;
                    traitDetail.IconOffsetX = 5;
                    traitDetail.IconOffsetY = 7;
                break;

                case FollowerTraitType.SpellPotency:
                    traitDetail.Name = "Spell Potency";
                    traitDetail.Description = "Spell damage increased by 10%";

                    traitDetail.IconItemId = 3638;
                    traitDetail.IconHue = 2632;
                    traitDetail.IconOffsetX = 0;
                    traitDetail.IconOffsetY = 8;
                break;

                case FollowerTraitType.Charged:
                    traitDetail.Name = "Charged";
                    traitDetail.Description = "Charged spell chance increased by 10%";

                    traitDetail.IconItemId = 14720;
                    traitDetail.IconHue = 0;
                    traitDetail.IconOffsetX = 5;
                    traitDetail.IconOffsetY = -100;
                break;

                case FollowerTraitType.Enchanted:
                    traitDetail.Name = "Enchanted";
                    traitDetail.Description = "Magic Resist increased by 20%";

                    traitDetail.IconItemId = 3688;
                    traitDetail.IconHue = 2595;
                    traitDetail.IconOffsetX = 0;
                    traitDetail.IconOffsetY = 0;
                break;

                case FollowerTraitType.Venomous:
                    traitDetail.Name = "Venomous";
                    traitDetail.Description = "Poisoning skill increased by 20%";

                    traitDetail.IconItemId = 540;
                    traitDetail.IconHue = 0;
                    traitDetail.IconOffsetX = -10;
                    traitDetail.IconOffsetY = 5;
                break;

                case FollowerTraitType.Toxicity:
                    traitDetail.Name = "Toxicity";
                    traitDetail.Description = "Poison inflicted has 20% chance to increase by 1 Level";

                    traitDetail.IconItemId = 3850;
                    traitDetail.IconHue = 0;
                    traitDetail.IconOffsetX = 5;
                    traitDetail.IconOffsetY = 8;
                break;

                case FollowerTraitType.Tolerance:
                    traitDetail.Name = "Tolerance";
                    traitDetail.Description = "Poison inflicted on creature has 50% chance to be reduced by 1 Level";

                    traitDetail.IconItemId = 3624;
                    traitDetail.IconHue = 1359;
                    traitDetail.IconOffsetX = -8;
                    traitDetail.IconOffsetY = 8;
                break;

                case FollowerTraitType.Mender:
                    traitDetail.Name = "Mender";
                    traitDetail.Description = "Healing done to creature increased by 10%";

                    traitDetail.IconItemId = 3618;
                    traitDetail.IconHue = 0;
                    traitDetail.IconOffsetX = -3;
                    traitDetail.IconOffsetY = 10;
                break;

                case FollowerTraitType.Regeneration:
                    traitDetail.Name = "Regeneration";
                    traitDetail.Description = "Hit Point regeneration while out of combat increased by 1000%";

                    traitDetail.IconItemId = 3624;
                    traitDetail.IconHue = 2515;
                    traitDetail.IconOffsetX = -8;
                    traitDetail.IconOffsetY = 8;
                break;

                case FollowerTraitType.Ethereal:
                    traitDetail.Name = "Ethereal";
                    traitDetail.Description = "Melee hit against creature has 10% chance to be reduced to 1 damage";

                    traitDetail.IconItemId = 14122;
                    traitDetail.IconHue = 2653;
                    traitDetail.IconOffsetX = -15;
                    traitDetail.IconOffsetY = -48;
                break;

                case FollowerTraitType.Nullify:
                    traitDetail.Name = "Nullify";
                    traitDetail.Description = "Spell cast against creature has 10% chance to be reduced to 1 damage";

                    traitDetail.IconItemId = 14122;
                    traitDetail.IconHue = 2620;
                    traitDetail.IconOffsetX = -15;
                    traitDetail.IconOffsetY = -48;
                break;

                case FollowerTraitType.Mirror:
                    traitDetail.Name = "Mirror";
                    traitDetail.Description = "Spell cast against creature has 5% chance to be reflected";

                    traitDetail.IconItemId = 18705;
                    traitDetail.IconHue = 0;
                    traitDetail.IconOffsetX = 3;
                    traitDetail.IconOffsetY = 0;
                break;

                case FollowerTraitType.Taunt:
                    traitDetail.Name = "Taunt";
                    traitDetail.Description = "Melee Hit has 10% chance to cause 30 seconds of Aggression against target (30 second cooldown)";

                    traitDetail.IconItemId = 3740;
                    traitDetail.IconHue = 0;
                    traitDetail.IconOffsetX = 0;
                    traitDetail.IconOffsetY = 3;
                break;

                case FollowerTraitType.Steadfast:
                    traitDetail.Name = "Steadfast";
                    traitDetail.Description = "Melee Damage taken from Aggressed targets reduced by 20%";

                    traitDetail.IconItemId = 16155;
                    traitDetail.IconHue = 0;
                    traitDetail.IconOffsetX = 0;
                    traitDetail.IconOffsetY = -2;
                break; 
               
                case FollowerTraitType.Purge:
                    traitDetail.Name = "Purge";
                    traitDetail.Description = "After taking poison damage, has a 10% chance to cancel current poison";

                    traitDetail.IconItemId = 3847;
                    traitDetail.IconHue = 0;
                    traitDetail.IconOffsetX = 5;
                    traitDetail.IconOffsetY = 8;
                break;

                case FollowerTraitType.PoisonEater:
                    traitDetail.Name = "Poison Eater";
                    traitDetail.Description = "Melee Damage increased by 10% if target is poisoned";

                    traitDetail.IconItemId = 2515;
                    traitDetail.IconHue = 2209;
                    traitDetail.IconOffsetX = 0;
                    traitDetail.IconOffsetY = 13;
                break;

                case FollowerTraitType.VileFury:
                    traitDetail.Name = "Vile Fury";
                    traitDetail.Description = "Melee Damage increased by 10% if poisoned";

                    traitDetail.IconItemId = 11704;
                    traitDetail.IconHue = 2542;
                    traitDetail.IconOffsetX = 0;
                    traitDetail.IconOffsetY = 11;
                break;

                case FollowerTraitType.Berserk:
                    traitDetail.Name = "Berserk";
                    traitDetail.Description = "Melee Damage increased by 20% if below 50% Hit Points";

                    traitDetail.IconItemId = 4722;
                    traitDetail.IconHue = 0;
                    traitDetail.IconOffsetX = -10;
                    traitDetail.IconOffsetY = 0;
                break;

                case FollowerTraitType.Bloodlust:
                    traitDetail.Name = "Bloodlust";
                    traitDetail.Description = "Melee Damage increased by 10% if target is below 50% Hit Points";

                    traitDetail.IconItemId = 3619;
                    traitDetail.IconHue = 0;
                    traitDetail.IconOffsetX = 0;
                    traitDetail.IconOffsetY = 8;
                break;

                case FollowerTraitType.Execute:
                traitDetail.Name = "Execute";
                    traitDetail.Description = "Melee Damage increased by 20% if target is below 25% Hit Points";

                    traitDetail.IconItemId = 7390;
                    traitDetail.IconHue = 0;
                    traitDetail.IconOffsetX = -5;
                    traitDetail.IconOffsetY = 0;
                break;

                case FollowerTraitType.Ambush:
                    traitDetail.Name = "Ambush";
                    traitDetail.Description = "Stealth Attack damage increased by 20%";

                    traitDetail.IconItemId = 7960;
                    traitDetail.IconHue = 2303;
                    traitDetail.IconOffsetX = 0;
                    traitDetail.IconOffsetY = 2;
                break;

                case FollowerTraitType.Surprise:
                    traitDetail.Name = "Surprise";
                    traitDetail.Description = "Melee Defense increased by 10% for 10 seconds after making a Stealth Attack";

                    traitDetail.IconItemId = 7683;
                    traitDetail.IconHue = 0;
                    traitDetail.IconOffsetX = 0;
                    traitDetail.IconOffsetY = 2;
                break;

                case FollowerTraitType.Disorient:
                    traitDetail.Name = "Disorient";
                    traitDetail.Description = "Melee attacks have a 10% chance to Disorient target";

                    traitDetail.IconItemId = 17103;
                    traitDetail.IconHue = 2500;
                    traitDetail.IconOffsetX = -13;
                    traitDetail.IconOffsetY = 0;
                break;

                case FollowerTraitType.Debilitate:
                    traitDetail.Name = "Debilitate";
                    traitDetail.Description = "Melee attacks have a 10% chance to Debilitate target";

                    traitDetail.IconItemId = 17103;
                    traitDetail.IconHue = 2075;
                    traitDetail.IconOffsetX = -13;
                    traitDetail.IconOffsetY = 0;
                break;

                case FollowerTraitType.Cripple:
                    traitDetail.Name = "Cripple";
                    traitDetail.Description = "Melee attacks have a 10% chance to Cripple target";

                    traitDetail.IconItemId = 17103;
                    traitDetail.IconHue = 2591;
                    traitDetail.IconOffsetX = -13;
                    traitDetail.IconOffsetY = 0;
                break;

                case FollowerTraitType.Pierce:
                    traitDetail.Name = "Pierce";
                    traitDetail.Description = "Melee attacks have a 10% chance to Pierce target";

                    traitDetail.IconItemId = 6915;
                    traitDetail.IconHue = 0;
                    traitDetail.IconOffsetX = 3;
                    traitDetail.IconOffsetY = 5;
                break;

                case FollowerTraitType.Frenzy:
                    traitDetail.Name = "Frenzy";
                    traitDetail.Description = "Melee attacks have a 10% chance to cause Frenzy";

                    traitDetail.IconItemId = 16381;
                    traitDetail.IconHue = 0;
                    traitDetail.IconOffsetX = -3;
                    traitDetail.IconOffsetY = 0;
                break;

                case FollowerTraitType.Enrage:
                    traitDetail.Name = "Enrage";
                    traitDetail.Description = "Melee attacks have a 10% chance to cause Enrage";

                    traitDetail.IconItemId = 17083;
                    traitDetail.IconHue = 0;
                    traitDetail.IconOffsetX = 0;
                    traitDetail.IconOffsetY = 0;
                break;

                case FollowerTraitType.Entangle:
                    traitDetail.Name = "Entangle";
                    traitDetail.Description = "Melee attacks have a 10% chance to Entangle target";

                    traitDetail.IconItemId = 5368;
                    traitDetail.IconHue = 0;
                    traitDetail.IconOffsetX = 0;
                    traitDetail.IconOffsetY = 5;
                break;

                case FollowerTraitType.Hinder:
                    traitDetail.Name = "Hinder";
                    traitDetail.Description = "Melee attacks have a 10% chance to Hinder target";

                    traitDetail.IconItemId = 3530;
                    traitDetail.IconHue = 0;
                    traitDetail.IconOffsetX = 0;
                    traitDetail.IconOffsetY = -2;
                break;

                case FollowerTraitType.Rend:
                    traitDetail.Name = "Rend";
                    traitDetail.Description = "Bleed damage inflicted increased by 10%";

                    traitDetail.IconItemId = 4651;
                    traitDetail.IconHue = 0;
                    traitDetail.IconOffsetX = 0;
                    traitDetail.IconOffsetY = 3;
                break;

                case FollowerTraitType.Contagion:
                    traitDetail.Name = "Contagion";
                    traitDetail.Description = "Disease damage inflicted increased by 10%";

                    traitDetail.IconItemId = 540;
                    traitDetail.IconHue = 2401;
                    traitDetail.IconOffsetX = -10;
                    traitDetail.IconOffsetY = 5;
                break;
            }

            return traitDetail;
        }

        public static int GetFollowerTraitsAvailable(BaseCreature creature)
        {
            int traitsAvailable = 0;

            if (!creature.Tameable)
                return 0;

            int level = creature.ExperienceLevel;

            for (int a = 0; a < level; a++)
            {
                FollowerTraitType trait = creature.m_FollowerTraitSelections[a];

                if (trait == FollowerTraitType.None)
                    traitsAvailable++;
            }            

            return traitsAvailable;
        }
    }

    public class FollowerTraitDetail
    {
        public string Name = "Trait Name";
        public string Description = "";

        public int IconItemId = 3637;
        public int IconHue = 0;
        public int IconOffsetX = 0;
        public int IconOffsetY = 0;
    }
}