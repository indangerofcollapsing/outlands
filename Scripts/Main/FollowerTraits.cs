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

        Sturdy,
        Mender
    }

    public static class FollowerTraits
    {
        public static FollowerTraitDetail GetFollowerTraitDetail(FollowerTraitType followerTraitType)
        {
            FollowerTraitDetail traitDetail = new FollowerTraitDetail();

            switch (followerTraitType)
            {
                case FollowerTraitType.Sturdy:
                    traitDetail.Name = "Sturdy";
                    traitDetail.Description = "Creature Armor increased by 20%";

                    traitDetail.IconItemId = 7028;
                    traitDetail.IconHue = 0;
                    traitDetail.IconOffsetX = 0;
                    traitDetail.IconOffsetY = 0;                    
                break;

                case FollowerTraitType.Mender:
                    traitDetail.Name = "Mender";
                    traitDetail.Description = "Healing done to creature increased by 20%";

                    traitDetail.IconItemId = 3618;
                    traitDetail.IconHue = 0;
                    traitDetail.IconOffsetX = 0;
                    traitDetail.IconOffsetY = 0;
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