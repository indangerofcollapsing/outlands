using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.ExtensionMethods
{
    public static class EnumExtensionMethods
    {
        public static TAttribute GetAttribute<TAttribute>(this Enum value)
        where TAttribute : Attribute
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            return type.GetField(name)
                .GetCustomAttributes(false)
                .OfType<TAttribute>()
                .SingleOrDefault();
        }
    }
}

namespace Server.Achievements
{
    public class AchievementTargetAttribute : Attribute
    {
        public int Target { get; private set; }
        internal AchievementTargetAttribute(int target)
        {
            Target = target;
        }
    }


    enum Category
    {
        PvE,
        PvP,
        Newb,
        Crafter
    }

    public enum PvECategory
    {
        [Description("Kill Dragons")] // DONE
        [AchievementTarget(3)]
        KillDragons, 
        
        [Description("Kill Demons")] // DONE
        [AchievementTarget(2)]
        KillDemons,
        
        [Description("Kill Lich Lords")] // DONE
        [AchievementTarget(2)]
        KillLichLords,
        
        [Description("Kill Blood Elementals")] // DONE
        [AchievementTarget(2)]
        KillBloodElementals,
        
        [Description("Kill Balrons")] // DONE
        [AchievementTarget(1)]
        KillBalrons,
        
        [Description("Kill Elder Gazers")] // DONE
        [AchievementTarget(3)]
        KillElderGazers,

        [Description("Kill Dragon Whelps")] // DONE
        [AchievementTarget(5)]
        KillDragonWhelps,

        [Description("Kill Drake Whelps")] // DONE
        [AchievementTarget(6)]
        KillDrakeWhelps,

        [Description("Kill Ice Skitters")] // DONE
        [AchievementTarget(6)]
        KillIceSkitters,

        [Description("Kill Wyvern Hatchlings")] // DONE
        [AchievementTarget(5)]
        KillWyvernHatchlings,

        [Description("Kill Elder Mojokas")] // DONE
        [AchievementTarget(3)]
        KillElderMojokas,

        [Description("Kill Orcish Executioners")] // DONE
        [AchievementTarget(3)]
        KillOrcishExecutioners,

        [Description("Kill Orcish Surijins")] // DONE
        [AchievementTarget(5)]
        KillOrcishSurijins,

        [Description("Kill Orcish Mojokas")] // DONE 
        [AchievementTarget(5)]
        KillOrcishMojokas,

        [Description("Kill Corrupt Warmages")] //DONE 
        [AchievementTarget(5)]
        KillCorruptWarmages,

        [Description("Kill Corrupt Runecasters")] // DONE 
        [AchievementTarget(5)]
        KillCorruptRunecasters,

        [Description("Kill Corrupt Reavers")] // DONE
        [AchievementTarget(2)]
        KillCorruptReavers,

        [Description("Kill Creepers")] // DONE
        [AchievementTarget(7)]
        KillCreepers,

        [Description("Kill Decayed Zombies")] // DONE
        [AchievementTarget(7)]
        KillDecayedZombies,

        [Description("Kill Flaming Zombies")] // DONE
        [AchievementTarget(7)]
        KillFlamingZombies,

        [Description("Kill Bloody Zombies")] // DONE
        [AchievementTarget(7)]
        KillBloodyZombies,
        
        [Description("Kill Poison Elementals")] // DONE
        [AchievementTarget(1)]
        KillPoisonElementals,

        [Description("Kill Undead Ogre Lords")] // DONE
        [AchievementTarget(3)]
        KillUndeadOgreLords,

        [Description("Kill Gargoyles")] // DONE
        [AchievementTarget(5)]
        KillGargoyles,

        [Description("Kill Gazers")] // DONE 
        [AchievementTarget(6)]
        KillGazers,

        [Description("Kill Hell Hounds")] // DONE
        [AchievementTarget(6)]
        KillHellHounds,

        [Description("Kill Armored Titans")] // DONE
        [AchievementTarget(1)]
        KillArmoredTitans,

        [Description("Kill Gusts")] // DONE
        [AchievementTarget(6)]
        KillGusts,

        [Description("Kill Puddles")] // DONE
        [AchievementTarget(6)]
        KillPuddles,

        [Description("Kill Succubi")] // DONE
        [AchievementTarget(2)]
        KillSuccubi,

        [Description("Kill Bone Knights")] // DONE
        [AchievementTarget(6)]
        KillBoneKnights,

        [Description("Kill Ogre Lords")] // DONE
        [AchievementTarget(3)]
        KillOgreLords,

        [Description("Kill Rotting Corpses")] // DONE
        [AchievementTarget(2)]
        KillRottingCorpses,

        [Description("Kill Skeleton Knights")] // DONE
        [AchievementTarget(6)]
        KillSkeletonKnights,

    }

    public enum PvPCategory
    {
        [Description("Kill Enemy Militia")] // DONE
        [AchievementTarget(3)]
        KillPlayers,
        
        [Description("Douse Brazier")] // DONE
        [AchievementTarget(3)]
        DouseBraziers,
        
        [Description("Light Brazier")] // DONE
        [AchievementTarget(3)]
        LightBraziers,
        
        [Description("Kill Order Elementals")] // DONE
        [AchievementTarget(4)]
        KillOrderElementals,
        
        [Description("Kill Chaos Elementals")] // DONE
        [AchievementTarget(4)]
        KillChaosElementals,

        [Description("Complete a Battleground")] // DONE
        [AchievementTarget(2)]
        CompleteBattleground,
    }

    public enum NewbCategory
    {
        [Description("Gain Skill")] // DONE
        [AchievementTarget(30)]
        GainSkill,
        
        [Description("Visit Shame")] // DONE
        [AchievementTarget(1)]
        VisitShame,
        
        [Description("Kill Earth Elementals")] // DONE
        [AchievementTarget(6)]
        KillEarthElementals,
        
        [Description("Kill Scorpions")] // DONE
        [AchievementTarget(6)]
        KillScorpions,
        
        [Description("Kill Air Elementals")] // DONE
        [AchievementTarget(6)]
        KillAirElementals,
        
        [Description("Kill Orcs")] // DONE
        [AchievementTarget(6)]
        KillOrcs,
        
        [Description("Visit Despise")] // DONE
        [AchievementTarget(1)]
        VisitDespise,
        
        [Description("Kill Trolls")] // DONE
        [AchievementTarget(6)]
        KillTrolls,
        
        [Description("Kill Ettins")] // DONE
        [AchievementTarget(6)]
        KillEttins,

        [Description("Kill Skeletons")] // DONE
        [AchievementTarget(8)]
        KillSkeletons,

        [Description("Kill Zombies")] // DONE
        [AchievementTarget(8)]
        KillZombies,

    }

    public enum CrafterCategory
    {
        [Description("Turn in BODs")] // DONE
        [AchievementTarget(3)]
        TurnInBod,
        
        [Description("Craft Armor")] // DONE
        [AchievementTarget(10)]
        CraftArmor,
        
        [Description("Craft Weapons")] // Done
        [AchievementTarget(10)]
        CraftWeapon,
        
        [Description("Mine Ore")] // DONE
        [AchievementTarget(200)]
        MineOre,
        
        [Description("Chop Trees")] // DONE
        [AchievementTarget(200)]
        ChopTree,
        
        [Description("Craft Clothing")] // DONE
        [AchievementTarget(10)]
        CraftClothing,

        [Description("Write Scrolls")] // DONE
        [AchievementTarget(15)]
        WriteScrolls,        

        [Description("Fill Potions")] // DONE
        [AchievementTarget(20)]
        FillPotions,

        [Description("Poison Weapons")] // DONE
        [AchievementTarget(15)]
        PoisonWeapons,

    }
}
