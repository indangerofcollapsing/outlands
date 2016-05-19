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

        Hardness,
        Charged,
        
        Mender,
        Potency,

        Immunity,
        Vicious,

        Shadows,
        Toxicity,
    }

    public static class FollowerTraits
    {
        public static FollowerTraitDetail GetFollowerTraitDetail(FollowerTraitType followerTraitType)
        {
            FollowerTraitDetail traitDetail = new FollowerTraitDetail();

            switch (followerTraitType)
            {
                //Level 1
                case FollowerTraitType.Toughness:
                    traitDetail.Name = "Toughness";
                    traitDetail.Description = "Hit Points increased by 10%";

                    traitDetail.IconItemId = 3849;
                    traitDetail.IconHue = 0;
                    traitDetail.IconOffsetX = 10;
                    traitDetail.IconOffsetY = 8;                    
                break;

                case FollowerTraitType.Quickness:
                    traitDetail.Name = "Quickness";
                    traitDetail.Description = "Dexterity increased by 10%";

                    traitDetail.IconItemId = 3848;
                    traitDetail.IconHue = 0;
                    traitDetail.IconOffsetX = 10;
                    traitDetail.IconOffsetY = 8;
                break;

                //Level 2
                case FollowerTraitType.Hardness:
                    traitDetail.Name = "Hardness";
                    traitDetail.Description = "Armor increased by 20%";

                    traitDetail.IconItemId = 7028;
                    traitDetail.IconHue = 0;
                    traitDetail.IconOffsetX = 0;
                    traitDetail.IconOffsetY = 0;
                break;

                case FollowerTraitType.Charged:
                    traitDetail.Name = "Charged";
                    traitDetail.Description = "Chance to charge spells increased by 20%";

                    traitDetail.IconItemId = 3834;
                    traitDetail.IconHue = 2589;
                    traitDetail.IconOffsetX = 5;
                    traitDetail.IconOffsetY = 7;
                break;

                //Level 3
                case FollowerTraitType.Mender:
                    traitDetail.Name = "Mender";
                    traitDetail.Description = "Healing done to creature increased by 10%";

                    traitDetail.IconItemId = 3618;
                    traitDetail.IconHue = 0;
                    traitDetail.IconOffsetX = 5;
                    traitDetail.IconOffsetY = 8;
                break;

                case FollowerTraitType.Potency:
                    traitDetail.Name = "Potency";
                    traitDetail.Description = "Poisoning skill increased by 20%";

                    traitDetail.IconItemId = 3850;
                    traitDetail.IconHue = 0;
                    traitDetail.IconOffsetX = 10;
                    traitDetail.IconOffsetY = 8;
                break;

                //Level 4
                case FollowerTraitType.Immunity:
                    traitDetail.Name = "Immunity";
                    traitDetail.Description = "Poison Resistance increased by 1 level";

                    traitDetail.IconItemId = 3847;
                    traitDetail.IconHue = 0;
                    traitDetail.IconOffsetX = 10;
                    traitDetail.IconOffsetY = 8;
                break;

                case FollowerTraitType.Vicious:
                    traitDetail.Name = "Vicious";
                    traitDetail.Description = "Melee damage increased by 10%";

                    traitDetail.IconItemId = 11704; //4650; //7392
                    traitDetail.IconHue = 0;
                    traitDetail.IconOffsetX = 5;
                    traitDetail.IconOffsetY = 8;
                break;

                //Level 4
                case FollowerTraitType.Shadows:
                    traitDetail.Name = "Shadows";
                    traitDetail.Description = "10% chance to avoid melee hit";

                    traitDetail.IconItemId = 14002;
                    traitDetail.IconHue = 2401;
                    traitDetail.IconOffsetX = -10;
                    traitDetail.IconOffsetY = 3;
                break;

                case FollowerTraitType.Toxicity:
                    traitDetail.Name = "Toxicity";
                    traitDetail.Description = "20% chance to increase level of poison inflicted";

                    traitDetail.IconItemId = 540;
                    traitDetail.IconHue = 0;
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