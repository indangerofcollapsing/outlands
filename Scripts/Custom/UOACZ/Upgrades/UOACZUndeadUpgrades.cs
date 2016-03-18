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

namespace Server
{
    public enum UOACZUndeadUpgradeType
    {
        Zombie,
        ZombieMagi,            
        BloodyZombie,
        FlamingZombie,
        RottingCorpse,

        Skeleton,        
        PatchworkSkeleton,
        SkeletalMage,
        SkeletalKnight,
        GiantSkeleton,        

        Ghoul,
        CorpseEater,
        FailedExperiment,        
        Mummy,
        FleshGolem,
        
        Spectre,
        Ghost,        
        Corpsebride,
        Phantom,
        RagWitch,        

        Necromancer,
        VampireThrall,
        Lich,
        VampireCountess,
        LichLord,        

        VoidSlime,
        PitTentacle,
        DarkWisp,
        FountainOfEvil,
        EnvelopingDarkness,

        SkeletalCritter,
        SkeletalHorse,
        Nightmare,
        SkeletalDrake,
        ShadowDragon,
    }

    public static class UOACZUndeadUpgrades
    {
        public static UOACZUndeadUpgradeDetail GetUpgradeDetail(UOACZUndeadUpgradeType upgrade)
        {
            UOACZUndeadUpgradeDetail upgradeDetail = new UOACZUndeadUpgradeDetail();

            switch (upgrade)
            {
                case UOACZUndeadUpgradeType.Zombie:
                    upgradeDetail.m_Name = "Zombie";
                   
                    upgradeDetail.m_BodyValue = 3;
                    upgradeDetail.m_HueMod = 0;

                    upgradeDetail.m_IconItemID = 8428;
                    upgradeDetail.m_IconHue = 0;
                    upgradeDetail.m_IconOffsetX = 5;
                    upgradeDetail.m_IconOffsetY = 0;

                    upgradeDetail.m_AngerSound = 0x1D7;
                    upgradeDetail.m_IdleSound = 0x1D8;
                    upgradeDetail.m_AttackSound = 0x1D9;
                    upgradeDetail.m_HurtSound = 0x1DA;
                    upgradeDetail.m_DeathSound = 0x1DB;

                   upgradeDetail.m_CastingAnimation = 11;
                    upgradeDetail.m_CastingAnimationFrames = 5;

                    upgradeDetail.m_SpecialAnimation = 6;
                    upgradeDetail.m_SpecialAnimationFrames = 4;

                    upgradeDetail.m_MonsterTier = 1;

                    upgradeDetail.m_Stats[StatType.Str] = 300;
                    upgradeDetail.m_Stats[StatType.Dex] = 50;
                    upgradeDetail.m_Stats[StatType.Int] = 30;

                    upgradeDetail.m_Skills[SkillName.Wrestling] = 60;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 100;
                    upgradeDetail.m_Skills[SkillName.Meditation] = 20;

                    upgradeDetail.m_DamageMin = 12;
                    upgradeDetail.m_DamageMax = 24;

                    upgradeDetail.m_VirtualArmor = 10;

                    upgradeDetail.m_MaxFollowers = 2;

                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.RecruitZombie);
                break;

                case UOACZUndeadUpgradeType.ZombieMagi:
                    upgradeDetail.m_Name = "Zombie Magi";

                    upgradeDetail.m_BodyValue = 3;
                    upgradeDetail.m_HueMod = 2585;

                    upgradeDetail.m_IconItemID = 8428;
                    upgradeDetail.m_IconHue = 2585;
                    upgradeDetail.m_IconOffsetX = 5;
                    upgradeDetail.m_IconOffsetY = 0;

                    upgradeDetail.m_AngerSound = 0x1D7;
                    upgradeDetail.m_IdleSound = 0x1D8;
                    upgradeDetail.m_AttackSound = 0x1D9;
                    upgradeDetail.m_HurtSound = 0x1DA;
                    upgradeDetail.m_DeathSound = 0x1DB;

                    upgradeDetail.m_CastingAnimation = 11;
                    upgradeDetail.m_CastingAnimationFrames = 5;

                    upgradeDetail.m_SpecialAnimation = 6;
                    upgradeDetail.m_SpecialAnimationFrames = 4;
                 
                    upgradeDetail.m_MonsterTier = 2;

                    upgradeDetail.m_Stats[StatType.Str] = 325;
                    upgradeDetail.m_Stats[StatType.Dex] = 60;
                    upgradeDetail.m_Stats[StatType.Int] = 40;

                    upgradeDetail.m_Skills[SkillName.Wrestling] = 70;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 100;
                    upgradeDetail.m_Skills[SkillName.Meditation] = 30;

                    upgradeDetail.m_DamageMin = 14;
                    upgradeDetail.m_DamageMax = 28;

                    upgradeDetail.m_VirtualArmor = 20;

                    upgradeDetail.m_MaxFollowers = 4;

                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.RecruitZombie);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.LightningBolt);
                break;                                   

                case UOACZUndeadUpgradeType.BloodyZombie:
                    upgradeDetail.m_Name = "Bloody Zombie";

                    upgradeDetail.m_BodyValue = 3;
                    upgradeDetail.m_HueMod = 1779;

                    upgradeDetail.m_IconItemID = 8428;
                    upgradeDetail.m_IconHue = 40;
                    upgradeDetail.m_IconOffsetX = 5;
                    upgradeDetail.m_IconOffsetY = 0;

                    upgradeDetail.m_AngerSound = 0x1D7;
                    upgradeDetail.m_IdleSound = 0x1D8;
                    upgradeDetail.m_AttackSound = 0x1D9;
                    upgradeDetail.m_HurtSound = 0x1DA;
                    upgradeDetail.m_DeathSound = 0x1DB;

                   upgradeDetail.m_CastingAnimation = 11;
                    upgradeDetail.m_CastingAnimationFrames = 5;

                    upgradeDetail.m_SpecialAnimation = 6;
                    upgradeDetail.m_SpecialAnimationFrames = 4;
                  
                    upgradeDetail.m_MonsterTier = 3;

                    upgradeDetail.m_Stats[StatType.Str] = 350;
                    upgradeDetail.m_Stats[StatType.Dex] = 70;
                    upgradeDetail.m_Stats[StatType.Int] = 50;

                    upgradeDetail.m_Skills[SkillName.Wrestling] = 80;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 100;
                    upgradeDetail.m_Skills[SkillName.Meditation] = 40;

                    upgradeDetail.m_DamageMin = 16;
                    upgradeDetail.m_DamageMax = 32;

                    upgradeDetail.m_VirtualArmor = 30;

                    upgradeDetail.m_MaxFollowers = 6;

                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.RecruitZombie);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.AuraOfDecay);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.BloodyMess);
                break;

                case UOACZUndeadUpgradeType.FlamingZombie:
                    upgradeDetail.m_Name = "Flaming Zombie";

                    upgradeDetail.m_BodyValue = 3;
                    upgradeDetail.m_HueMod = 1359;

                    upgradeDetail.m_IconItemID = 8428;
                    upgradeDetail.m_IconHue = 1161;
                    upgradeDetail.m_IconOffsetX = 5;
                    upgradeDetail.m_IconOffsetY = 0;

                     upgradeDetail.m_AngerSound = 0x1D7;
                    upgradeDetail.m_IdleSound = 0x1D8;
                    upgradeDetail.m_AttackSound = 0x1D9;
                    upgradeDetail.m_HurtSound = 0x1DA;
                    upgradeDetail.m_DeathSound = 0x1DB;

                    upgradeDetail.m_CastingAnimation = 11;
                    upgradeDetail.m_CastingAnimationFrames = 5;

                    upgradeDetail.m_SpecialAnimation = 6;
                    upgradeDetail.m_SpecialAnimationFrames = 4;
                    
                    upgradeDetail.m_MonsterTier = 4;

                    upgradeDetail.m_Stats[StatType.Str] = 375;
                    upgradeDetail.m_Stats[StatType.Dex] = 80;
                    upgradeDetail.m_Stats[StatType.Int] = 60;

                    upgradeDetail.m_Skills[SkillName.Wrestling] = 90;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 100;
                    upgradeDetail.m_Skills[SkillName.Meditation] = 50;

                    upgradeDetail.m_DamageMin = 18;
                    upgradeDetail.m_DamageMax = 36;

                    upgradeDetail.m_VirtualArmor = 40;

                    upgradeDetail.m_MaxFollowers = 8;

                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.RecruitZombie);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.AuraOfDecay);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.Ignite);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.Combust);
                break;

                case UOACZUndeadUpgradeType.RottingCorpse:
                    upgradeDetail.m_Name = "Rotting Corpse";

                    upgradeDetail.m_BodyValue = 3;
                    upgradeDetail.m_HueMod = 2210;

                    upgradeDetail.m_IconItemID = 8428;
                    upgradeDetail.m_IconHue = 165;
                    upgradeDetail.m_IconOffsetX = 5;
                    upgradeDetail.m_IconOffsetY = 0;

                    upgradeDetail.m_AngerSound = 0x1D7;
                    upgradeDetail.m_IdleSound = 0x1D8;
                    upgradeDetail.m_AttackSound = 0x1D9;
                    upgradeDetail.m_HurtSound = 0x1DA;
                    upgradeDetail.m_DeathSound = 0x1DB;

                    upgradeDetail.m_CastingAnimation = 11;
                    upgradeDetail.m_CastingAnimationFrames = 5;

                    upgradeDetail.m_SpecialAnimation = 6;
                    upgradeDetail.m_SpecialAnimationFrames = 4;
                    
                    upgradeDetail.m_MonsterTier = 5;

                    upgradeDetail.m_Stats[StatType.Str] = 400;
                    upgradeDetail.m_Stats[StatType.Dex] = 90;
                    upgradeDetail.m_Stats[StatType.Int] = 70;

                    upgradeDetail.m_Skills[SkillName.Wrestling] = 100;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 100;
                    upgradeDetail.m_Skills[SkillName.Meditation] = 60;

                    upgradeDetail.m_DamageMin = 20;
                    upgradeDetail.m_DamageMax = 40;

                    upgradeDetail.m_VirtualArmor = 50;

                    upgradeDetail.m_MaxFollowers = 10;

                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.RecruitZombie);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.AuraOfDecay);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.Bile);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.Virus);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.Plague);
                break;

                case UOACZUndeadUpgradeType.Skeleton:
                    upgradeDetail.m_Name = "Skeleton";

                    upgradeDetail.m_BodyValue = 50;
                    upgradeDetail.m_HueMod = 0;

                    upgradeDetail.m_IconItemID = 8423;
                    upgradeDetail.m_IconHue = 0;
                    upgradeDetail.m_IconOffsetX = 5;
                    upgradeDetail.m_IconOffsetY = 0;

                    upgradeDetail.m_AngerSound = 0x48D;
                    upgradeDetail.m_IdleSound = 0x48D + 1;
                    upgradeDetail.m_AttackSound = 0x48D + 2;
                    upgradeDetail.m_HurtSound = 0x48D + 3;
                    upgradeDetail.m_DeathSound = 0x48D + 4;

                    upgradeDetail.m_CastingAnimation = 6;
                    upgradeDetail.m_CastingAnimationFrames = 4;

                    upgradeDetail.m_SpecialAnimation = 6;
                    upgradeDetail.m_SpecialAnimationFrames = 4;
                    
                    upgradeDetail.m_MonsterTier = 1;

                    upgradeDetail.m_Stats[StatType.Str] = 150;
                    upgradeDetail.m_Stats[StatType.Dex] = 60;
                    upgradeDetail.m_Stats[StatType.Int] = 30;

                    upgradeDetail.m_Skills[SkillName.Wrestling] = 70;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 100;
                    upgradeDetail.m_Skills[SkillName.Meditation] = 20;

                    upgradeDetail.m_DamageMin = 12;
                    upgradeDetail.m_DamageMax = 24;

                    upgradeDetail.m_VirtualArmor = 25;

                    upgradeDetail.m_MaxFollowers = 2;

                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.RecruitSkeleton);
                break;                

                case UOACZUndeadUpgradeType.PatchworkSkeleton:
                    upgradeDetail.m_Name = "Patchwork Skeleton";

                    upgradeDetail.m_BodyValue = 309;
                    upgradeDetail.m_HueMod = 0;

                    upgradeDetail.m_IconItemID = 9769;
                    upgradeDetail.m_IconHue = 0;
                    upgradeDetail.m_IconOffsetX = 0;
                    upgradeDetail.m_IconOffsetY = 0;

                    upgradeDetail.m_AngerSound = 0x48D;
                    upgradeDetail.m_IdleSound = 0x48D + 1;
                    upgradeDetail.m_AttackSound = 0x48D + 2;
                    upgradeDetail.m_HurtSound = 0x48D + 3;
                    upgradeDetail.m_DeathSound = 0x48D + 4;

                    upgradeDetail.m_CastingAnimation = 6;
                    upgradeDetail.m_CastingAnimationFrames = 4;

                    upgradeDetail.m_SpecialAnimation = 6;
                    upgradeDetail.m_SpecialAnimationFrames = 4;
                  
                    upgradeDetail.m_MonsterTier = 2;

                    upgradeDetail.m_Stats[StatType.Str] = 175;
                    upgradeDetail.m_Stats[StatType.Dex] = 70;
                    upgradeDetail.m_Stats[StatType.Int] = 40;

                    upgradeDetail.m_Skills[SkillName.Wrestling] = 80;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 100;
                    upgradeDetail.m_Skills[SkillName.Meditation] = 30;

                    upgradeDetail.m_DamageMin = 14;
                    upgradeDetail.m_DamageMax = 28;

                    upgradeDetail.m_VirtualArmor = 50;

                    upgradeDetail.m_MaxFollowers = 4;

                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.RecruitSkeleton);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.ShieldOfBones);
                break;

                case UOACZUndeadUpgradeType.SkeletalMage:
                    upgradeDetail.m_Name = "Skeletal Mage";

                    upgradeDetail.m_BodyValue = 50;
                    upgradeDetail.m_HueMod = 2117;

                    upgradeDetail.m_IconItemID = 9660;
                    upgradeDetail.m_IconHue = 2501;
                    upgradeDetail.m_IconOffsetX = 15;
                    upgradeDetail.m_IconOffsetY = 0;

                    upgradeDetail.m_AngerSound = 0x48D;
                    upgradeDetail.m_IdleSound = 0x48D + 1;
                    upgradeDetail.m_AttackSound = 0x48D + 2;
                    upgradeDetail.m_HurtSound = 0x48D + 3;
                    upgradeDetail.m_DeathSound = 0x48D + 4;

                    upgradeDetail.m_CastingAnimation = 6;
                    upgradeDetail.m_CastingAnimationFrames = 4;

                    upgradeDetail.m_SpecialAnimation = 6;
                    upgradeDetail.m_SpecialAnimationFrames = 4;
                  
                    upgradeDetail.m_MonsterTier = 3;

                    upgradeDetail.m_Stats[StatType.Str] = 200;
                    upgradeDetail.m_Stats[StatType.Dex] = 80;
                    upgradeDetail.m_Stats[StatType.Int] = 50;

                    upgradeDetail.m_Skills[SkillName.Wrestling] = 90;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 100;
                    upgradeDetail.m_Skills[SkillName.Meditation] = 40;

                    upgradeDetail.m_DamageMin = 16;
                    upgradeDetail.m_DamageMax = 32;

                    upgradeDetail.m_VirtualArmor = 75;

                    upgradeDetail.m_MaxFollowers = 6;

                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.RecruitSkeleton);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.ShieldOfBones);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.LightningBolt);
                break;            

                case UOACZUndeadUpgradeType.SkeletalKnight:
                    upgradeDetail.m_Name = "Skeletal Knight";

                    upgradeDetail.m_BodyValue = 57;
                    upgradeDetail.m_HueMod = 2610;

                    upgradeDetail.m_IconItemID = 8423;
                    upgradeDetail.m_IconHue = 2610;
                    upgradeDetail.m_IconOffsetX = 5;
                    upgradeDetail.m_IconOffsetY = 0;

                    upgradeDetail.m_AngerSound = 0x48D;
                    upgradeDetail.m_IdleSound = 0x48D + 1;
                    upgradeDetail.m_AttackSound = 0x48D + 2;
                    upgradeDetail.m_HurtSound = 0x48D + 3;
                    upgradeDetail.m_DeathSound = 0x48D + 4;

                    upgradeDetail.m_CastingAnimation = 18;
                    upgradeDetail.m_CastingAnimationFrames = 5;

                    upgradeDetail.m_SpecialAnimation = 18;
                    upgradeDetail.m_SpecialAnimationFrames = 5;
                   
                    upgradeDetail.m_MonsterTier = 4;

                    upgradeDetail.m_Stats[StatType.Str] = 225;
                    upgradeDetail.m_Stats[StatType.Dex] = 90;
                    upgradeDetail.m_Stats[StatType.Int] = 60;

                    upgradeDetail.m_Skills[SkillName.Wrestling] = 100;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 100;
                    upgradeDetail.m_Skills[SkillName.Meditation] = 50;

                    upgradeDetail.m_DamageMin = 18;
                    upgradeDetail.m_DamageMax = 36;

                    upgradeDetail.m_VirtualArmor = 100;

                    upgradeDetail.m_MaxFollowers = 8;

                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.RecruitSkeleton);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.ShieldOfBones);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.Sunder);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.Demolish);
                break;

                case UOACZUndeadUpgradeType.GiantSkeleton:
                    upgradeDetail.m_Name = "Giant Skeleton";

                    upgradeDetail.m_BodyValue = 308;
                    upgradeDetail.m_HueMod = 0;

                    upgradeDetail.m_IconItemID = 9768;
                    upgradeDetail.m_IconHue = 0;
                    upgradeDetail.m_IconOffsetX = 5;
                    upgradeDetail.m_IconOffsetY = 0;

                    upgradeDetail.m_AngerSound = 0x4FE;
                    upgradeDetail.m_IdleSound = 0x4ED;
                    upgradeDetail.m_AttackSound = 0x627;
                    upgradeDetail.m_HurtSound = 0x628;
                    upgradeDetail.m_DeathSound = 0x489;

                    upgradeDetail.m_CastingAnimation = 6;
                    upgradeDetail.m_CastingAnimationFrames = 4;

                    upgradeDetail.m_SpecialAnimation = 11;
                    upgradeDetail.m_SpecialAnimationFrames = 5;
                    
                    upgradeDetail.m_MonsterTier = 5;

                    upgradeDetail.m_Stats[StatType.Str] = 250;
                    upgradeDetail.m_Stats[StatType.Dex] = 100;
                    upgradeDetail.m_Stats[StatType.Int] = 70;

                    upgradeDetail.m_Skills[SkillName.Wrestling] = 110;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 100;
                    upgradeDetail.m_Skills[SkillName.Meditation] = 60;

                    upgradeDetail.m_DamageMin = 20;
                    upgradeDetail.m_DamageMax = 40;

                    upgradeDetail.m_VirtualArmor = 125;

                    upgradeDetail.m_MaxFollowers = 10;

                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.RecruitSkeleton);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.ShieldOfBones);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.Sunder);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.Demolish);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.RestlessDead);
                break;

                case UOACZUndeadUpgradeType.Ghoul:
                    upgradeDetail.m_Name = "Ghoul";

                    upgradeDetail.m_BodyValue = 153;
                    upgradeDetail.m_HueMod = 0;

                    upgradeDetail.m_IconItemID = 8457;
                    upgradeDetail.m_IconHue = 0;
                    upgradeDetail.m_IconOffsetX = 5;
                    upgradeDetail.m_IconOffsetY = 0;

                    upgradeDetail.m_AngerSound = 0x482;
                    upgradeDetail.m_IdleSound = 0x482 + 1;
                    upgradeDetail.m_AttackSound = 0x482 + 2;
                    upgradeDetail.m_HurtSound = 0x482 + 3;
                    upgradeDetail.m_DeathSound = 0x482 + 4;

                    upgradeDetail.m_CastingAnimation = 5;
                    upgradeDetail.m_CastingAnimationFrames = 4;

                    upgradeDetail.m_SpecialAnimation = 18;
                    upgradeDetail.m_SpecialAnimationFrames = 5;
                  
                    upgradeDetail.m_MonsterTier = 1;

                    upgradeDetail.m_Stats[StatType.Str] = 150;
                    upgradeDetail.m_Stats[StatType.Dex] = 60;
                    upgradeDetail.m_Stats[StatType.Int] = 30;

                    upgradeDetail.m_Skills[SkillName.Wrestling] = 70;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 100;
                    upgradeDetail.m_Skills[SkillName.Meditation] = 20;

                    upgradeDetail.m_DamageMin = 16;
                    upgradeDetail.m_DamageMax = 32;

                    upgradeDetail.m_VirtualArmor = 10;

                    upgradeDetail.m_MaxFollowers = 2;

                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.CarrionSwarm);
                break;

                case UOACZUndeadUpgradeType.CorpseEater:
                    upgradeDetail.m_Name = "Corpse Eater";

                    upgradeDetail.m_BodyValue = 732;
                    upgradeDetail.m_HueMod = 0;

                    upgradeDetail.m_IconItemID = 17053;
                    upgradeDetail.m_IconHue = 0;
                    upgradeDetail.m_IconOffsetX = 0;
                    upgradeDetail.m_IconOffsetY = 0;

                    upgradeDetail.m_AngerSound = 0x581;
                    upgradeDetail.m_IdleSound = 0x582;
                    upgradeDetail.m_AttackSound = 0x580;
                    upgradeDetail.m_HurtSound = 0x5DA;
                    upgradeDetail.m_DeathSound = 0x57F;

                    upgradeDetail.m_AttackAnimation = 4;
                    upgradeDetail.m_AttackAnimationFrames = 8;

                    upgradeDetail.m_HurtAnimation = 10;
                    upgradeDetail.m_HurtAnimationFrames = 6;

                    upgradeDetail.m_IdleAnimation = 17;
                    upgradeDetail.m_IdleAnimationFrames = 8;

                    upgradeDetail.m_SpecialAnimation = 4;
                    upgradeDetail.m_SpecialAnimationFrames = 8;

                    upgradeDetail.m_CastingAnimation = 4;
                    upgradeDetail.m_CastingAnimationFrames = 8;
                    
                    upgradeDetail.m_MonsterTier = 2;

                    upgradeDetail.m_Stats[StatType.Str] = 175;
                    upgradeDetail.m_Stats[StatType.Dex] = 70;
                    upgradeDetail.m_Stats[StatType.Int] = 40;

                    upgradeDetail.m_Skills[SkillName.Wrestling] = 80;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 100;
                    upgradeDetail.m_Skills[SkillName.Meditation] = 30;

                    upgradeDetail.m_DamageMin = 18;
                    upgradeDetail.m_DamageMax = 36;

                    upgradeDetail.m_VirtualArmor = 20;

                    upgradeDetail.m_MaxFollowers = 4;

                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.CarrionSwarm);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.ConsumeCorpse);
                break;

                case UOACZUndeadUpgradeType.FailedExperiment:
                    upgradeDetail.m_Name = "Failed Experiment";

                    upgradeDetail.m_BodyValue = 305;
                    upgradeDetail.m_HueMod = 0;

                    upgradeDetail.m_IconItemID = 9765;
                    upgradeDetail.m_IconHue = 0;
                    upgradeDetail.m_IconOffsetX = 0;
                    upgradeDetail.m_IconOffsetY = 0;

                    upgradeDetail.m_AngerSound = 0x584;
                    upgradeDetail.m_IdleSound = 0x383;
                    upgradeDetail.m_AttackSound = 0x382;
                    upgradeDetail.m_HurtSound = 0x385;
                    upgradeDetail.m_DeathSound = 0x455;

                    upgradeDetail.m_CastingAnimation = 6;
                    upgradeDetail.m_CastingAnimationFrames = 4;

                    upgradeDetail.m_SpecialAnimation = 11;
                    upgradeDetail.m_SpecialAnimationFrames = 5;
                   
                    upgradeDetail.m_MonsterTier = 3;

                    upgradeDetail.m_Stats[StatType.Str] = 200;
                    upgradeDetail.m_Stats[StatType.Dex] = 80;
                    upgradeDetail.m_Stats[StatType.Int] = 50;

                    upgradeDetail.m_Skills[SkillName.Wrestling] = 90;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 100;
                    upgradeDetail.m_Skills[SkillName.Meditation] = 40;

                    upgradeDetail.m_DamageMin = 20;
                    upgradeDetail.m_DamageMax = 40;

                    upgradeDetail.m_VirtualArmor = 30;

                    upgradeDetail.m_MaxFollowers = 6;

                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.CarrionSwarm);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.ConsumeCorpse);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.CorpseBreath);
                break;

                case UOACZUndeadUpgradeType.Mummy:
                    upgradeDetail.m_Name = "Mummy";

                    upgradeDetail.m_BodyValue = 154;
                    upgradeDetail.m_HueMod = 0;

                    upgradeDetail.m_IconItemID = 9639;
                    upgradeDetail.m_IconHue = 0;
                    upgradeDetail.m_IconOffsetX = 10;
                    upgradeDetail.m_IconOffsetY = 0;

                    upgradeDetail.m_AngerSound = 0x27A;
                    upgradeDetail.m_IdleSound = 0x27A + 1;
                    upgradeDetail.m_AttackSound = 0x27A + 2;
                    upgradeDetail.m_HurtSound = 0x27A + 3;
                    upgradeDetail.m_DeathSound = 0x27A + 4;

                    upgradeDetail.m_CastingAnimation = 18;
                    upgradeDetail.m_CastingAnimationFrames = 5;

                    upgradeDetail.m_SpecialAnimation = 18;
                    upgradeDetail.m_SpecialAnimationFrames = 5;
                   
                    upgradeDetail.m_MonsterTier = 4;

                    upgradeDetail.m_Stats[StatType.Str] = 225;
                    upgradeDetail.m_Stats[StatType.Dex] = 90;
                    upgradeDetail.m_Stats[StatType.Int] = 60;

                    upgradeDetail.m_Skills[SkillName.Wrestling] = 100;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 100;
                    upgradeDetail.m_Skills[SkillName.Meditation] = 50;

                    upgradeDetail.m_DamageMin = 22;
                    upgradeDetail.m_DamageMax = 44;

                    upgradeDetail.m_VirtualArmor = 40;

                    upgradeDetail.m_MaxFollowers = 8;

                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.CarrionSwarm);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.ConsumeCorpse);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.CorpseBreath);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.FeedingFrenzy);                    
                break;  

                case UOACZUndeadUpgradeType.FleshGolem:
                    upgradeDetail.m_Name = "Flesh Golem";

                    upgradeDetail.m_BodyValue = 304;
                    upgradeDetail.m_HueMod = 0;

                    upgradeDetail.m_IconItemID = 9764;
                    upgradeDetail.m_IconHue = 0;
                    upgradeDetail.m_IconOffsetX = 0;
                    upgradeDetail.m_IconOffsetY = 0;

                    upgradeDetail.m_AngerSound = 0x36B;
                    upgradeDetail.m_IdleSound = 0x2FA;
                    upgradeDetail.m_AttackSound = 0x2F8;
                    upgradeDetail.m_HurtSound = 0x2F9;
                    upgradeDetail.m_DeathSound = 0x2F7;

                    upgradeDetail.m_CastingAnimation = 18;
                    upgradeDetail.m_CastingAnimationFrames = 5;

                    upgradeDetail.m_SpecialAnimation = 11;
                    upgradeDetail.m_SpecialAnimationFrames = 5;
                   
                    upgradeDetail.m_MonsterTier = 5;

                    upgradeDetail.m_Stats[StatType.Str] = 250;
                    upgradeDetail.m_Stats[StatType.Dex] = 100;
                    upgradeDetail.m_Stats[StatType.Int] = 70;

                    upgradeDetail.m_Skills[SkillName.Wrestling] = 110;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 100;
                    upgradeDetail.m_Skills[SkillName.Meditation] = 60;

                    upgradeDetail.m_DamageMin = 24;
                    upgradeDetail.m_DamageMax = 48;

                    upgradeDetail.m_VirtualArmor = 50;

                    upgradeDetail.m_MaxFollowers = 10;

                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.CarrionSwarm);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.ConsumeCorpse);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.CorpseBreath);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.FeedingFrenzy); 
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.LivingBomb);
                break;                              

                case UOACZUndeadUpgradeType.Spectre:
                    upgradeDetail.m_Name = "Spectre";

                    upgradeDetail.m_BodyValue = 26;
                    upgradeDetail.m_HueMod = 25000;

                    upgradeDetail.m_IconItemID = 8457;
                    upgradeDetail.m_IconHue = 2949;
                    upgradeDetail.m_IconOffsetX = 5;
                    upgradeDetail.m_IconOffsetY = 0;

                    upgradeDetail.m_AngerSound = 0x482;
                    upgradeDetail.m_IdleSound = 0x482 + 1;
                    upgradeDetail.m_AttackSound = 0x482 + 2;
                    upgradeDetail.m_HurtSound = 0x482 + 3;
                    upgradeDetail.m_DeathSound = 0x482 + 4;

                    upgradeDetail.m_CastingAnimation = 5;
                    upgradeDetail.m_CastingAnimationFrames = 4;

                    upgradeDetail.m_SpecialAnimation = 18;
                    upgradeDetail.m_SpecialAnimationFrames = 5;
                   
                    upgradeDetail.m_MonsterTier = 1;

                    upgradeDetail.m_Stats[StatType.Str] = 100;
                    upgradeDetail.m_Stats[StatType.Dex] = 60;
                    upgradeDetail.m_Stats[StatType.Int] = 30;

                    upgradeDetail.m_Skills[SkillName.Wrestling] = 90;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 100;
                    upgradeDetail.m_Skills[SkillName.Meditation] = 20;

                    upgradeDetail.m_DamageMin = 12;
                    upgradeDetail.m_DamageMax = 24;

                    upgradeDetail.m_VirtualArmor = 5;

                    upgradeDetail.m_MaxFollowers = 1;

                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.Shadows);
                break;

                case UOACZUndeadUpgradeType.Ghost:
                    upgradeDetail.m_Name = "Ghost";

                    upgradeDetail.m_BodyValue = 146;
                    upgradeDetail.m_HueMod = 25000;

                    upgradeDetail.m_IconItemID = 9659;
                    upgradeDetail.m_IconHue = 2075;
                    upgradeDetail.m_IconOffsetX = 10;
                    upgradeDetail.m_IconOffsetY = 0;

                    upgradeDetail.m_AngerSound = 0x482;
                    upgradeDetail.m_IdleSound = 0x482 + 1;
                    upgradeDetail.m_AttackSound = 0x482 + 2;
                    upgradeDetail.m_HurtSound = 0x482 + 3;
                    upgradeDetail.m_DeathSound = 0x482 + 4;

                    upgradeDetail.m_CastingAnimation = 18;
                    upgradeDetail.m_CastingAnimationFrames = 5;

                    upgradeDetail.m_SpecialAnimation = 18;
                    upgradeDetail.m_SpecialAnimationFrames = 5;
                    
                    upgradeDetail.m_MonsterTier = 2;

                    upgradeDetail.m_Stats[StatType.Str] = 125;
                    upgradeDetail.m_Stats[StatType.Dex] = 70;
                    upgradeDetail.m_Stats[StatType.Int] = 40;

                    upgradeDetail.m_Skills[SkillName.Wrestling] = 100;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 100;
                    upgradeDetail.m_Skills[SkillName.Meditation] = 30;

                    upgradeDetail.m_DamageMin = 14;
                    upgradeDetail.m_DamageMax = 18;

                    upgradeDetail.m_VirtualArmor = 10;

                    upgradeDetail.m_MaxFollowers = 2;

                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.Shadows);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.Wail);
                break;

                case UOACZUndeadUpgradeType.Corpsebride:
                    upgradeDetail.m_Name = "Corpsebride";

                    upgradeDetail.m_BodyValue = 252;
                    upgradeDetail.m_HueMod = 1102;

                    upgradeDetail.m_IconItemID = 10092;
                    upgradeDetail.m_IconHue = 0;
                    upgradeDetail.m_IconOffsetX = 0;
                    upgradeDetail.m_IconOffsetY = 0;

                    upgradeDetail.m_AngerSound = 0x4B0;
                    upgradeDetail.m_IdleSound = 0x4B0 + 1;
                    upgradeDetail.m_AttackSound = 0x4B0 + 2;
                    upgradeDetail.m_HurtSound = 0x4B0 + 3;
                    upgradeDetail.m_DeathSound = 0x4B0 + 4;

                    upgradeDetail.m_CastingAnimation = 17;
                    upgradeDetail.m_CastingAnimationFrames = 5;

                    upgradeDetail.m_SpecialAnimation = 6;
                    upgradeDetail.m_SpecialAnimationFrames = 4;
                   
                    upgradeDetail.m_MonsterTier = 3;

                    upgradeDetail.m_Stats[StatType.Str] = 150;
                    upgradeDetail.m_Stats[StatType.Dex] = 80;
                    upgradeDetail.m_Stats[StatType.Int] = 50;

                    upgradeDetail.m_Skills[SkillName.Wrestling] = 110;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 100;
                    upgradeDetail.m_Skills[SkillName.Meditation] = 40;

                    upgradeDetail.m_DamageMin = 16;
                    upgradeDetail.m_DamageMax = 32;

                    upgradeDetail.m_VirtualArmor = 15;

                    upgradeDetail.m_MaxFollowers = 3;

                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.Shadows);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.Wail);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.GhostlyStrike);
                break;

                case UOACZUndeadUpgradeType.Phantom:
                    upgradeDetail.m_Name = "Phantom";

                    upgradeDetail.m_BodyValue = 779;
                    upgradeDetail.m_HueMod = 25000;

                    upgradeDetail.m_IconItemID = 8397;
                    upgradeDetail.m_IconHue = 2950;
                    upgradeDetail.m_IconOffsetX = 5;
                    upgradeDetail.m_IconOffsetY = 0;

                    upgradeDetail.m_AngerSound = 0x300;
                    upgradeDetail.m_IdleSound = 0x301;
                    upgradeDetail.m_AttackSound = 0x2BA;
                    upgradeDetail.m_HurtSound = 0x303;
                    upgradeDetail.m_DeathSound = 0x2FA;

                    upgradeDetail.m_CastingAnimation = 4;
                    upgradeDetail.m_CastingAnimationFrames = 4;

                    upgradeDetail.m_SpecialAnimation = 4;
                    upgradeDetail.m_SpecialAnimationFrames = 4;
                   
                    upgradeDetail.m_MonsterTier = 4;

                    upgradeDetail.m_Stats[StatType.Str] = 175;
                    upgradeDetail.m_Stats[StatType.Dex] = 90;
                    upgradeDetail.m_Stats[StatType.Int] = 60;

                    upgradeDetail.m_Skills[SkillName.Wrestling] = 120;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 100;
                    upgradeDetail.m_Skills[SkillName.Meditation] = 50;

                    upgradeDetail.m_DamageMin = 18;
                    upgradeDetail.m_DamageMax = 36;

                    upgradeDetail.m_VirtualArmor = 20;

                    upgradeDetail.m_MaxFollowers = 4;

                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.Shadows);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.Wail);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.GhostlyStrike);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.Dematerialize);
                break;

                case UOACZUndeadUpgradeType.RagWitch:
                    upgradeDetail.m_Name = "Rag Witch";

                    upgradeDetail.m_BodyValue = 740;
                    upgradeDetail.m_HueMod = 2500;

                    upgradeDetail.m_IconItemID = 9671;
                    upgradeDetail.m_IconHue = 1102;
                    upgradeDetail.m_IconOffsetX = 5;
                    upgradeDetail.m_IconOffsetY = 0;

                    upgradeDetail.m_AngerSound = 0x284;
                    upgradeDetail.m_IdleSound = 0x285;
                    upgradeDetail.m_AttackSound = 0x286;
                    upgradeDetail.m_HurtSound = 0x287;
                    upgradeDetail.m_DeathSound = 0x288;

                    upgradeDetail.m_AttackAnimation = 5;
                    upgradeDetail.m_AttackAnimationFrames = 8;

                    upgradeDetail.m_HurtAnimation = 10;
                    upgradeDetail.m_HurtAnimationFrames = 6;

                    upgradeDetail.m_CastingAnimation = 4;
                    upgradeDetail.m_CastingAnimationFrames = 8;

                    upgradeDetail.m_SpecialAnimation = 4;
                    upgradeDetail.m_SpecialAnimationFrames = 8;
                    
                    upgradeDetail.m_MonsterTier = 5;

                    upgradeDetail.m_Stats[StatType.Str] = 200;
                    upgradeDetail.m_Stats[StatType.Dex] = 100;
                    upgradeDetail.m_Stats[StatType.Int] = 70;

                    upgradeDetail.m_Skills[SkillName.Wrestling] = 130;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 100;
                    upgradeDetail.m_Skills[SkillName.Meditation] = 60;

                    upgradeDetail.m_DamageMin = 20;
                    upgradeDetail.m_DamageMax = 40;

                    upgradeDetail.m_VirtualArmor = 25;

                    upgradeDetail.m_MaxFollowers = 5;

                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.Shadows);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.Wail);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.GhostlyStrike);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.Dematerialize);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.Malediction);
                break;

                case UOACZUndeadUpgradeType.Necromancer:
                    upgradeDetail.m_Name = "Necromancer";

                    upgradeDetail.m_BodyValue = 148;
                    upgradeDetail.m_HueMod = 2587;

                    upgradeDetail.m_IconItemID = 9662;
                    upgradeDetail.m_IconHue = 2587;
                    upgradeDetail.m_IconOffsetX = 15;
                    upgradeDetail.m_IconOffsetY = 0;

                    upgradeDetail.m_AngerSound = 0x48D;
                    upgradeDetail.m_IdleSound = 0x48D + 1;
                    upgradeDetail.m_AttackSound = 0x48D + 2;
                    upgradeDetail.m_HurtSound = 0x48D + 3;
                    upgradeDetail.m_DeathSound = 0x48D + 4;

                    upgradeDetail.m_CastingAnimation = 18;
                    upgradeDetail.m_CastingAnimationFrames = 5;

                    upgradeDetail.m_SpecialAnimation = 18;
                    upgradeDetail.m_SpecialAnimationFrames = 5;
                  
                    upgradeDetail.m_MonsterTier = 1;

                    upgradeDetail.m_Stats[StatType.Str] = 125;
                    upgradeDetail.m_Stats[StatType.Dex] = 40;
                    upgradeDetail.m_Stats[StatType.Int] = 40;

                    upgradeDetail.m_Skills[SkillName.Wrestling] = 60;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 100;
                    upgradeDetail.m_Skills[SkillName.Meditation] = 30;

                    upgradeDetail.m_DamageMin = 10;
                    upgradeDetail.m_DamageMax = 20;

                    upgradeDetail.m_VirtualArmor = 10;

                    upgradeDetail.m_MaxFollowers = 1;

                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.Deathbolt);
                break;

                case UOACZUndeadUpgradeType.VampireThrall:
                    upgradeDetail.m_Name = "Vampire Thrall";

                    upgradeDetail.m_BodyValue = 722;
                    upgradeDetail.m_HueMod = 0;

                    upgradeDetail.m_IconItemID = 9614;
                    upgradeDetail.m_IconHue = 2500;
                    upgradeDetail.m_IconOffsetX = 0;
                    upgradeDetail.m_IconOffsetY = 0;

                    upgradeDetail.m_AngerSound = 0x47D;
                    upgradeDetail.m_IdleSound = 0x47D;
                    upgradeDetail.m_AttackSound = 372 + 2;
                    upgradeDetail.m_HurtSound = 0x5F9;
                    upgradeDetail.m_DeathSound = 0x5F5;

                    upgradeDetail.m_AttackAnimation = 6;
                    upgradeDetail.m_AttackAnimationFrames = 8;

                    upgradeDetail.m_CastingAnimation = 12;
                    upgradeDetail.m_CastingAnimationFrames = 8;

                    upgradeDetail.m_SpecialAnimation = 5;
                    upgradeDetail.m_SpecialAnimationFrames = 8;

                    upgradeDetail.m_HurtAnimation = 10;
                    upgradeDetail.m_HurtAnimationFrames = 6;

                    upgradeDetail.m_IdleAnimation = 15;
                    upgradeDetail.m_IdleAnimationFrames = 5;
                                       
                    upgradeDetail.m_MonsterTier = 2;

                    upgradeDetail.m_Stats[StatType.Str] = 225;
                    upgradeDetail.m_Stats[StatType.Dex] = 80;
                    upgradeDetail.m_Stats[StatType.Int] = 40;

                    upgradeDetail.m_Skills[SkillName.Wrestling] = 90;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 100;
                    upgradeDetail.m_Skills[SkillName.Meditation] = 30;

                    upgradeDetail.m_DamageMin = 14;
                    upgradeDetail.m_DamageMax = 28;

                    upgradeDetail.m_VirtualArmor = 20;

                    upgradeDetail.m_MaxFollowers = 2;

                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.BatForm);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.Transfix);
                break;

                case UOACZUndeadUpgradeType.Lich:
                    upgradeDetail.m_Name = "Lich";

                    upgradeDetail.m_BodyValue = 24;
                    upgradeDetail.m_HueMod = 0;

                    upgradeDetail.m_IconItemID = 8440;
                    upgradeDetail.m_IconHue = 0;
                    upgradeDetail.m_IconOffsetX = 10;
                    upgradeDetail.m_IconOffsetY = 0;

                    upgradeDetail.m_AngerSound = 0x3E9;
                    upgradeDetail.m_IdleSound = 0x3E9 + 1;
                    upgradeDetail.m_AttackSound = 0x3E9 + 2;
                    upgradeDetail.m_HurtSound = 0x3E9 + 3;
                    upgradeDetail.m_DeathSound = 0x3E9 + 4;

                    upgradeDetail.m_CastingAnimation = 12;
                    upgradeDetail.m_CastingAnimationFrames = 7;

                    upgradeDetail.m_SpecialAnimation = 13;
                    upgradeDetail.m_SpecialAnimationFrames = 7;
                   
                    upgradeDetail.m_MonsterTier = 3;

                    upgradeDetail.m_Stats[StatType.Str] = 150;
                    upgradeDetail.m_Stats[StatType.Dex] = 60;
                    upgradeDetail.m_Stats[StatType.Int] = 60;

                    upgradeDetail.m_Skills[SkillName.Wrestling] = 80;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 100;
                    upgradeDetail.m_Skills[SkillName.Meditation] = 50;

                    upgradeDetail.m_DamageMin = 14;
                    upgradeDetail.m_DamageMax = 28;

                    upgradeDetail.m_VirtualArmor = 30;

                    upgradeDetail.m_MaxFollowers = 3;

                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.Deathbolt);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.LightningBolt);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.EnergyBolt);
                break;

                case UOACZUndeadUpgradeType.VampireCountess:
                    upgradeDetail.m_Name = "Vampire Countess";

                    upgradeDetail.m_BodyValue = 258;
                    upgradeDetail.m_HueMod = 2500;

                    upgradeDetail.m_IconItemID = 11652;
                    upgradeDetail.m_IconHue = 0;
                    upgradeDetail.m_IconOffsetX = 5;
                    upgradeDetail.m_IconOffsetY = -5;

                    upgradeDetail.m_AngerSound = 0x370;
                    upgradeDetail.m_IdleSound = 0x57D;
                    upgradeDetail.m_AttackSound = 0x374;
                    upgradeDetail.m_HurtSound = 0x375;
                    upgradeDetail.m_DeathSound = 0x376;

                    upgradeDetail.m_AttackAnimation = 6;
                    upgradeDetail.m_AttackAnimationFrames = 6;

                    upgradeDetail.m_CastingAnimation = 4;
                    upgradeDetail.m_CastingAnimationFrames = 4;

                    upgradeDetail.m_SpecialAnimation = 11;
                    upgradeDetail.m_SpecialAnimationFrames = 5;
                    
                    upgradeDetail.m_MonsterTier = 4;

                    upgradeDetail.m_Stats[StatType.Str] = 275;
                    upgradeDetail.m_Stats[StatType.Dex] = 100;
                    upgradeDetail.m_Stats[StatType.Int] = 60;

                    upgradeDetail.m_Skills[SkillName.Wrestling] = 110;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 100;
                    upgradeDetail.m_Skills[SkillName.Meditation] = 50;

                    upgradeDetail.m_DamageMin = 18;
                    upgradeDetail.m_DamageMax = 36;

                    upgradeDetail.m_VirtualArmor = 40;

                    upgradeDetail.m_MaxFollowers = 4;

                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.BatForm);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.Transfix);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.Embrace);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.Unlife);
                break;

                case UOACZUndeadUpgradeType.LichLord:
                    upgradeDetail.m_Name = "Lich lord";

                    upgradeDetail.m_BodyValue = 24;
                    upgradeDetail.m_HueMod = 2500;

                    upgradeDetail.m_IconItemID = 8440;
                    upgradeDetail.m_IconHue = 2587;
                    upgradeDetail.m_IconOffsetX = 10;
                    upgradeDetail.m_IconOffsetY = 0;

                    upgradeDetail.m_AngerSound = 0x3E9;
                    upgradeDetail.m_IdleSound = 0x3E9 + 1;
                    upgradeDetail.m_AttackSound = 0x3E9 + 2;
                    upgradeDetail.m_HurtSound = 0x3E9 + 3;
                    upgradeDetail.m_DeathSound = 0x3E9 + 4;

                   upgradeDetail.m_CastingAnimation = 12;
                    upgradeDetail.m_CastingAnimationFrames = 7;

                    upgradeDetail.m_SpecialAnimation = 13;
                    upgradeDetail.m_SpecialAnimationFrames = 7;
                   
                    upgradeDetail.m_MonsterTier = 5;

                    upgradeDetail.m_Stats[StatType.Str] = 200;
                    upgradeDetail.m_Stats[StatType.Dex] = 80;
                    upgradeDetail.m_Stats[StatType.Int] = 80;

                    upgradeDetail.m_Skills[SkillName.Wrestling] = 100;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 100;
                    upgradeDetail.m_Skills[SkillName.Meditation] = 70;

                    upgradeDetail.m_DamageMin = 18;
                    upgradeDetail.m_DamageMax = 36;

                    upgradeDetail.m_VirtualArmor = 50;

                    upgradeDetail.m_MaxFollowers = 5;

                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.Deathbolt);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.LightningBolt);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.EnergyBolt);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.Unlife);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.ChainLightning);
                break;                

                case UOACZUndeadUpgradeType.VoidSlime:
                    upgradeDetail.m_Name = "Void Slime";

                    upgradeDetail.m_BodyValue = 51;
                    upgradeDetail.m_HueMod = 2200;

                    upgradeDetail.m_IconItemID = 8424;
                    upgradeDetail.m_IconHue = 1108;
                    upgradeDetail.m_IconOffsetX = -10;
                    upgradeDetail.m_IconOffsetY = 0;

                    upgradeDetail.m_AngerSound = 456;
                    upgradeDetail.m_IdleSound = 456 + 1;
                    upgradeDetail.m_AttackSound = 456 + 2;
                    upgradeDetail.m_HurtSound = 456 + 3;
                    upgradeDetail.m_DeathSound = 456 + 4;

                    upgradeDetail.m_CastingAnimation = 11;
                    upgradeDetail.m_CastingAnimationFrames = 5;

                    upgradeDetail.m_SpecialAnimation = 11;
                    upgradeDetail.m_SpecialAnimationFrames = 5;
                   
                    upgradeDetail.m_MonsterTier = 1;

                    upgradeDetail.m_Stats[StatType.Str] = 125;
                    upgradeDetail.m_Stats[StatType.Dex] = 40;
                    upgradeDetail.m_Stats[StatType.Int] = 40;

                    upgradeDetail.m_Skills[SkillName.Wrestling] = 60;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 100;
                    upgradeDetail.m_Skills[SkillName.Meditation] = 30;

                    upgradeDetail.m_DamageMin = 10;
                    upgradeDetail.m_DamageMax = 20;

                    upgradeDetail.m_VirtualArmor = 10;

                    upgradeDetail.m_MaxFollowers = 1;

                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.VoidRift);
                break;

                case UOACZUndeadUpgradeType.PitTentacle:
                    upgradeDetail.m_Name = "Pit Tentacle";

                    upgradeDetail.m_BodyValue = 8;
                    upgradeDetail.m_HueMod = 2052;

                    upgradeDetail.m_IconItemID = 8402;
                    upgradeDetail.m_IconHue = 2052;
                    upgradeDetail.m_IconOffsetX = 0;
                    upgradeDetail.m_IconOffsetY = 0;

                    upgradeDetail.m_AngerSound = 684;
                    upgradeDetail.m_IdleSound = 684 + 1;
                    upgradeDetail.m_AttackSound = 684 + 2;
                    upgradeDetail.m_HurtSound = 684 + 3;
                    upgradeDetail.m_DeathSound = 684 + 4;

                    upgradeDetail.m_CastingAnimation = 12;
                    upgradeDetail.m_CastingAnimationFrames = 15;

                    upgradeDetail.m_SpecialAnimation = 12;
                    upgradeDetail.m_SpecialAnimationFrames = 15;
                                       
                    upgradeDetail.m_MonsterTier = 2;

                    upgradeDetail.m_Stats[StatType.Str] = 150;
                    upgradeDetail.m_Stats[StatType.Dex] = 50;
                    upgradeDetail.m_Stats[StatType.Int] = 50;

                    upgradeDetail.m_Skills[SkillName.Wrestling] = 70;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 100;
                    upgradeDetail.m_Skills[SkillName.Meditation] = 40;

                    upgradeDetail.m_DamageMin = 12;
                    upgradeDetail.m_DamageMax = 24;

                    upgradeDetail.m_VirtualArmor = 20;

                    upgradeDetail.m_MaxFollowers = 2;

                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.VoidRift);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.Enervate);
                break;

                case UOACZUndeadUpgradeType.DarkWisp:
                    upgradeDetail.m_Name = "Dark Wisp";

                    upgradeDetail.m_BodyValue = 165;
                    upgradeDetail.m_HueMod = 1102;

                    upgradeDetail.m_IconItemID = 8448;
                    upgradeDetail.m_IconHue = 1107;
                    upgradeDetail.m_IconOffsetX = 0;
                    upgradeDetail.m_IconOffsetY = 0;

                    upgradeDetail.m_AngerSound = 466;
                    upgradeDetail.m_IdleSound = 466 + 1;
                    upgradeDetail.m_AttackSound = 466 + 2;
                    upgradeDetail.m_HurtSound = 466 + 3;
                    upgradeDetail.m_DeathSound = 466 + 4;

                    upgradeDetail.m_CastingAnimation = 11;
                    upgradeDetail.m_CastingAnimationFrames = 5;

                    upgradeDetail.m_SpecialAnimation = 11;
                    upgradeDetail.m_SpecialAnimationFrames = 5;
                  
                    upgradeDetail.m_MonsterTier = 3;

                    upgradeDetail.m_Stats[StatType.Str] = 175;
                    upgradeDetail.m_Stats[StatType.Dex] = 60;
                    upgradeDetail.m_Stats[StatType.Int] = 60;

                    upgradeDetail.m_Skills[SkillName.Wrestling] = 80;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 100;
                    upgradeDetail.m_Skills[SkillName.Meditation] = 50;

                    upgradeDetail.m_DamageMin = 14;
                    upgradeDetail.m_DamageMax = 28;

                    upgradeDetail.m_VirtualArmor = 30;

                    upgradeDetail.m_MaxFollowers = 3;

                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.VoidRift);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.Enervate);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.EnergyBolt);
                break;

                case UOACZUndeadUpgradeType.FountainOfEvil:
                    upgradeDetail.m_Name = "Fountain of Evil";

                    upgradeDetail.m_BodyValue = 16;
                    upgradeDetail.m_HueMod = 1761;

                    upgradeDetail.m_IconItemID = 8459;
                    upgradeDetail.m_IconHue = 1102;
                    upgradeDetail.m_IconOffsetX = -10;
                    upgradeDetail.m_IconOffsetY = -5;

                    upgradeDetail.m_AngerSound = 0x27F;
                    upgradeDetail.m_IdleSound = 0x280;
                    upgradeDetail.m_AttackSound = 0x281;
                    upgradeDetail.m_HurtSound = 0x282;
                    upgradeDetail.m_DeathSound = 0x283;

                    upgradeDetail.m_CastingAnimation = 12;
                    upgradeDetail.m_CastingAnimationFrames = 15;

                    upgradeDetail.m_SpecialAnimation = 12;
                    upgradeDetail.m_SpecialAnimationFrames = 15;
                   
                    upgradeDetail.m_MonsterTier = 4;

                    upgradeDetail.m_Stats[StatType.Str] = 200;
                    upgradeDetail.m_Stats[StatType.Dex] = 70;
                    upgradeDetail.m_Stats[StatType.Int] = 70;

                    upgradeDetail.m_Skills[SkillName.Wrestling] = 95;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 100;
                    upgradeDetail.m_Skills[SkillName.Meditation] = 60;

                    upgradeDetail.m_DamageMin = 16;
                    upgradeDetail.m_DamageMax = 32;

                    upgradeDetail.m_VirtualArmor = 40;

                    upgradeDetail.m_MaxFollowers = 4;

                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.VoidRift);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.Enervate);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.EnergyBolt);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.Darkblast);
                break;

                case UOACZUndeadUpgradeType.EnvelopingDarkness:
                    upgradeDetail.m_Name = "Enveloping Darkness";

                    upgradeDetail.m_BodyValue = 780;
                    upgradeDetail.m_HueMod = 2250;

                    upgradeDetail.m_IconItemID = 9736;
                    upgradeDetail.m_IconHue = 2052;
                    upgradeDetail.m_IconOffsetX = -10;
                    upgradeDetail.m_IconOffsetY = 0;

                    upgradeDetail.m_AngerSound = 0x300;
                    upgradeDetail.m_IdleSound = 0x301;
                    upgradeDetail.m_AttackSound = 0x302;
                    upgradeDetail.m_HurtSound = 0x303;
                    upgradeDetail.m_DeathSound = 0x304;

                    upgradeDetail.m_CastingAnimation = 5;
                    upgradeDetail.m_CastingAnimationFrames = 4;

                    upgradeDetail.m_SpecialAnimation = 5;
                    upgradeDetail.m_SpecialAnimationFrames = 4;
                  
                    upgradeDetail.m_MonsterTier = 5;

                    upgradeDetail.m_Stats[StatType.Str] = 225;
                    upgradeDetail.m_Stats[StatType.Dex] = 80;
                    upgradeDetail.m_Stats[StatType.Int] = 80;

                    upgradeDetail.m_Skills[SkillName.Wrestling] = 100;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 100;
                    upgradeDetail.m_Skills[SkillName.Meditation] = 70;

                    upgradeDetail.m_DamageMin = 18;
                    upgradeDetail.m_DamageMax = 36;

                    upgradeDetail.m_VirtualArmor = 50;

                    upgradeDetail.m_MaxFollowers = 5;

                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.VoidRift);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.Enervate);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.EnergyBolt);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.Darkblast);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.Engulf);
                break;

                case UOACZUndeadUpgradeType.SkeletalCritter:
                    upgradeDetail.m_Name = "Skeletal Critter";

                    upgradeDetail.m_BodyValue = 302;
                    upgradeDetail.m_HueMod = 0;

                    upgradeDetail.m_IconItemID = 9762;
                    upgradeDetail.m_IconHue = 0;
                    upgradeDetail.m_IconOffsetX = 0;
                    upgradeDetail.m_IconOffsetY = 0;

                    upgradeDetail.m_AngerSound = 0x625;
                    upgradeDetail.m_IdleSound = 0x624;
                    upgradeDetail.m_AttackSound = 0x636;
                    upgradeDetail.m_HurtSound = 0x637;
                    upgradeDetail.m_DeathSound = 0x26F;

                    upgradeDetail.m_CastingAnimation = 6;
                    upgradeDetail.m_CastingAnimation = 4;

                    upgradeDetail.m_SpecialAnimation = 6;
                    upgradeDetail.m_SpecialAnimationFrames = 4;
                                       
                    upgradeDetail.m_MonsterTier = 1;

                    upgradeDetail.m_Stats[StatType.Str] = 175;
                    upgradeDetail.m_Stats[StatType.Dex] = 70;
                    upgradeDetail.m_Stats[StatType.Int] = 30;

                    upgradeDetail.m_Skills[SkillName.Wrestling] = 80;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 100;
                    upgradeDetail.m_Skills[SkillName.Meditation] = 20;

                    upgradeDetail.m_DamageMin = 14;
                    upgradeDetail.m_DamageMax = 28;

                    upgradeDetail.m_VirtualArmor = 15;

                    upgradeDetail.m_MaxFollowers = 1;

                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.WildHunt);
                break;

                case UOACZUndeadUpgradeType.SkeletalHorse:
                    upgradeDetail.m_Name = "Skeletal Horse";

                    upgradeDetail.m_BodyValue = 793;
                    upgradeDetail.m_HueMod = 0;

                    upgradeDetail.m_IconItemID = 9751;
                    upgradeDetail.m_IconHue = 0;
                    upgradeDetail.m_IconOffsetX = -5;
                    upgradeDetail.m_IconOffsetY = 0;

                    upgradeDetail.m_AngerSound = 0x4BE;
                    upgradeDetail.m_IdleSound = 0x4BD;
                    upgradeDetail.m_AttackSound = 0x4F6;
                    upgradeDetail.m_HurtSound = 0x4BF;
                    upgradeDetail.m_DeathSound = 0x4C0;

                     upgradeDetail.m_CastingAnimation = 3;
                    upgradeDetail.m_CastingAnimationFrames = 3;

                    upgradeDetail.m_SpecialAnimation = 3;
                    upgradeDetail.m_SpecialAnimationFrames = 5;                   
                    
                    upgradeDetail.m_MonsterTier = 2;

                    upgradeDetail.m_Stats[StatType.Str] = 200;
                    upgradeDetail.m_Stats[StatType.Dex] = 80;
                    upgradeDetail.m_Stats[StatType.Int] = 40;

                    upgradeDetail.m_Skills[SkillName.Wrestling] = 90;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 100;
                    upgradeDetail.m_Skills[SkillName.Meditation] = 30;

                    upgradeDetail.m_DamageMin = 16;
                    upgradeDetail.m_DamageMax = 32;

                    upgradeDetail.m_VirtualArmor = 30;

                    upgradeDetail.m_MaxFollowers = 2;

                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.WildHunt);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.Charge);
                break;

                case UOACZUndeadUpgradeType.Nightmare:
                    upgradeDetail.m_Name = "Nightmare";

                    upgradeDetail.m_BodyValue = 116;
                    upgradeDetail.m_HueMod = 0;

                    upgradeDetail.m_IconItemID = 8480;
                    upgradeDetail.m_IconHue = 2019;
                    upgradeDetail.m_IconOffsetX = 0;
                    upgradeDetail.m_IconOffsetY = 0;

                    upgradeDetail.m_AngerSound = 0x4BE;
                    upgradeDetail.m_IdleSound = 0x4BD;
                    upgradeDetail.m_AttackSound = 0x4F6;
                    upgradeDetail.m_HurtSound = 0x4BF;
                    upgradeDetail.m_DeathSound = 0x4C0;

                    upgradeDetail.m_CastingAnimation = 3;
                    upgradeDetail.m_CastingAnimationFrames = 5;

                    upgradeDetail.m_SpecialAnimation = 3;
                    upgradeDetail.m_SpecialAnimationFrames = 5;                   
                    
                    upgradeDetail.m_MonsterTier = 3;

                    upgradeDetail.m_Stats[StatType.Str] = 225;
                    upgradeDetail.m_Stats[StatType.Dex] = 90;
                    upgradeDetail.m_Stats[StatType.Int] = 50;

                    upgradeDetail.m_Skills[SkillName.Wrestling] = 100;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 100;
                    upgradeDetail.m_Skills[SkillName.Meditation] = 40;

                    upgradeDetail.m_DamageMin = 18;
                    upgradeDetail.m_DamageMax = 36;

                    upgradeDetail.m_VirtualArmor = 45;

                    upgradeDetail.m_MaxFollowers = 3;

                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.WildHunt);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.Charge);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.Firebreath);
                break;

                case UOACZUndeadUpgradeType.SkeletalDrake:
                    upgradeDetail.m_Name = "Skeletal Drake";

                    upgradeDetail.m_BodyValue = 104;
                    upgradeDetail.m_HueMod = 0;

                    upgradeDetail.m_IconItemID = 11669;
                    upgradeDetail.m_IconHue = 2653;
                    upgradeDetail.m_IconOffsetX = 0;
                    upgradeDetail.m_IconOffsetY = 0;

                    upgradeDetail.m_AngerSound = 0x488;
                    upgradeDetail.m_IdleSound = 0x488 + 1;
                    upgradeDetail.m_AttackSound = 0x488 + 2;
                    upgradeDetail.m_HurtSound = 0x488 + 3;
                    upgradeDetail.m_DeathSound = 0x488 + 4;

                    upgradeDetail.m_CastingAnimation = 6;
                    upgradeDetail.m_CastingAnimationFrames = 4;

                    upgradeDetail.m_SpecialAnimation = 19;
                    upgradeDetail.m_SpecialAnimationFrames = 10;
                   
                    upgradeDetail.m_MonsterTier = 4;

                    upgradeDetail.m_Stats[StatType.Str] = 250;
                    upgradeDetail.m_Stats[StatType.Dex] = 100;
                    upgradeDetail.m_Stats[StatType.Int] = 60;

                    upgradeDetail.m_Skills[SkillName.Wrestling] = 110;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 100;
                    upgradeDetail.m_Skills[SkillName.Meditation] = 50;

                    upgradeDetail.m_DamageMin = 20;
                    upgradeDetail.m_DamageMax = 40;

                    upgradeDetail.m_VirtualArmor = 60;

                    upgradeDetail.m_MaxFollowers = 4;

                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.WildHunt);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.Charge);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.Firebreath);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.WingBuffet);
                break;

                case UOACZUndeadUpgradeType.ShadowDragon:
                    upgradeDetail.m_Name = "Shadow Dragon";

                    upgradeDetail.m_BodyValue = 106;
                    upgradeDetail.m_HueMod = 0;

                    upgradeDetail.m_IconItemID = 9781;
                    upgradeDetail.m_IconHue = 0;
                    upgradeDetail.m_IconOffsetX = 0;
                    upgradeDetail.m_IconOffsetY = -5;

                    upgradeDetail.m_AngerSound = 362;
                    upgradeDetail.m_IdleSound = 362 + 1;
                    upgradeDetail.m_AttackSound = 362 + 2;
                    upgradeDetail.m_HurtSound = 362 + 3;
                    upgradeDetail.m_DeathSound = 362 + 4;

                    upgradeDetail.m_CastingAnimation = 12;
                    upgradeDetail.m_CastingAnimationFrames = 15;

                    upgradeDetail.m_SpecialAnimation = 19;
                    upgradeDetail.m_SpecialAnimationFrames = 10;
                   
                    upgradeDetail.m_MonsterTier = 5;

                    upgradeDetail.m_Stats[StatType.Str] = 275;
                    upgradeDetail.m_Stats[StatType.Dex] = 110;
                    upgradeDetail.m_Stats[StatType.Int] = 70;

                    upgradeDetail.m_Skills[SkillName.Wrestling] = 120;
                    upgradeDetail.m_Skills[SkillName.Tactics] = 100;
                    upgradeDetail.m_Skills[SkillName.Meditation] = 60;

                    upgradeDetail.m_DamageMin = 22;
                    upgradeDetail.m_DamageMax = 44;

                    upgradeDetail.m_VirtualArmor = 75;

                    upgradeDetail.m_MaxFollowers = 5;

                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.WildHunt);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.Charge);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.Firebreath);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.WingBuffet);
                    upgradeDetail.m_Abilities.Add(UOACZUndeadAbilityType.BoneBreath);
                break;
            }

            return upgradeDetail;
        }

        public static UOACZUndeadUpgradeType GetRandomizedUpgrade(UOACZAccountEntry playerEntry)
        {
            UOACZUndeadUpgradeType newUpgradeType = UOACZUndeadUpgradeType.Zombie;

            if (playerEntry == null)
                return newUpgradeType;

            List<UOACZUndeadUpgradeType> m_PossibleUpgrades = new List<UOACZUndeadUpgradeType>();

            int upgradeCount = Enum.GetNames(typeof(UOACZUndeadUpgradeType)).Length;

            for (int a = 0; a < upgradeCount; a++)
            {
                UOACZUndeadUpgradeType upgradeType = (UOACZUndeadUpgradeType)a;

                if (playerEntry.UndeadProfile.ActiveForm == upgradeType)
                    continue;

                UOACZUndeadUpgradeDetail upgradeDetail = UOACZUndeadUpgrades.GetUpgradeDetail(upgradeType);

                if (upgradeDetail.m_MonsterTier == playerEntry.UndeadProfile.MonsterTier)
                    m_PossibleUpgrades.Add(upgradeType);
            }

            if (m_PossibleUpgrades.Count > 0)
                return m_PossibleUpgrades[Utility.RandomMinMax(0, m_PossibleUpgrades.Count - 1)];

            return newUpgradeType;
        }

        public static void SetActiveForm(UOACZAccountEntry entry, UOACZUndeadUpgradeType upgradeType)
        {   
            UOACZUndeadUpgradeDetail upgradeDetail = GetUpgradeDetail(upgradeType);

            entry.UndeadProfile.ActiveForm = upgradeType;

            entry.UndeadProfile.BodyValue = upgradeDetail.m_BodyValue;
            entry.UndeadProfile.HueMod = upgradeDetail.m_HueMod;

            entry.UndeadProfile.IconItemID = upgradeDetail.m_IconItemID;
            entry.UndeadProfile.IconHue = upgradeDetail.m_IconHue;
            entry.UndeadProfile.IconOffsetX = upgradeDetail.m_IconOffsetX;
            entry.UndeadProfile.IconOffsetY = upgradeDetail.m_IconOffsetY;

            entry.UndeadProfile.AngerSound = upgradeDetail.m_AngerSound;
            entry.UndeadProfile.IdleSound = upgradeDetail.m_IdleSound;
            entry.UndeadProfile.AttackSound = upgradeDetail.m_AttackSound;
            entry.UndeadProfile.HurtSound = upgradeDetail.m_HurtSound;
            entry.UndeadProfile.DeathSound = upgradeDetail.m_DeathSound;

            entry.UndeadProfile.AttackAnimation = upgradeDetail.m_AttackAnimation;
            entry.UndeadProfile.AttackAnimationFrames = upgradeDetail.m_AttackAnimationFrames;

            entry.UndeadProfile.IdleAnimation = upgradeDetail.m_IdleAnimation;
            entry.UndeadProfile.IdleAnimationFrames = upgradeDetail.m_IdleAnimationFrames;

            entry.UndeadProfile.HurtAnimation = upgradeDetail.m_HurtAnimation;
            entry.UndeadProfile.HurtAnimationFrames = upgradeDetail.m_HurtAnimationFrames;

            entry.UndeadProfile.SpecialAnimation = upgradeDetail.m_SpecialAnimation;
            entry.UndeadProfile.SpecialAnimationFrames = upgradeDetail.m_SpecialAnimationFrames;

            entry.UndeadProfile.CastingAnimation = upgradeDetail.m_CastingAnimation;
            entry.UndeadProfile.CastingAnimationFrames = upgradeDetail.m_CastingAnimationFrames;            

            entry.UndeadProfile.FollowersMax = upgradeDetail.m_MaxFollowers;
            entry.UndeadProfile.DamageMin = upgradeDetail.m_DamageMin;
            entry.UndeadProfile.DamageMax = upgradeDetail.m_DamageMax;
            entry.UndeadProfile.VirtualArmor = upgradeDetail.m_VirtualArmor;

            entry.UndeadProfile.m_Stats = new Dictionary<StatType, int>();            

            entry.UndeadProfile.m_Stats[StatType.Str] = upgradeDetail.m_Stats[StatType.Str];
            entry.UndeadProfile.m_Stats[StatType.Dex] = upgradeDetail.m_Stats[StatType.Dex];
            entry.UndeadProfile.m_Stats[StatType.Int] = upgradeDetail.m_Stats[StatType.Int];

            entry.UndeadProfile.m_Stats[StatType.Str] += (int)(Math.Round((double)upgradeDetail.m_Stats[StatType.Str] * (double)entry.UndeadProfile.PostTier5Upgrades * UOACZSystem.UndeadPostTier5StatSkillIncrease));
            entry.UndeadProfile.m_Stats[StatType.Dex] += (int)(Math.Round((double)upgradeDetail.m_Stats[StatType.Dex] * (double)entry.UndeadProfile.PostTier5Upgrades * UOACZSystem.UndeadPostTier5StatSkillIncrease));
            entry.UndeadProfile.m_Stats[StatType.Int] += (int)(Math.Round((double)upgradeDetail.m_Stats[StatType.Int] * (double)entry.UndeadProfile.PostTier5Upgrades * UOACZSystem.UndeadPostTier5StatSkillIncrease));
                
            entry.UndeadProfile.m_Skills = new Dictionary<SkillName, double>();

            int skillcount = Enum.GetNames(typeof(SkillName)).Length;

            for (int a = 0; a < skillcount; a++)
            {
                entry.UndeadProfile.m_Skills[(SkillName)a] = 0;
            }

            foreach (KeyValuePair<SkillName, double> skill in upgradeDetail.m_Skills)
            {
                SkillName skillName = skill.Key;
                double skillValue = skill.Value;

                entry.UndeadProfile.m_Skills[skillName] = skillValue;

                if (skillName == SkillName.Tactics)
                    continue;
               
                entry.UndeadProfile.m_Skills[skillName] += Math.Round((double)upgradeDetail.m_Stats[StatType.Str] * (double)entry.UndeadProfile.PostTier5Upgrades * UOACZSystem.UndeadPostTier5StatSkillIncrease);  
            }
            
            UpdateAbilities(entry);   
        }

        public static void UpdateAbilities(UOACZAccountEntry entry)
        {
            if (entry == null)
                return;

            //Store Existing Ability Cooldowns
            Dictionary<UOACZUndeadAbilityType, DateTime> m_ExistingAbilities = new Dictionary<UOACZUndeadAbilityType, DateTime>();

            foreach (UOACZUndeadAbilityEntry abilityEntry in entry.UndeadProfile.m_Abilities)
            {
                if (abilityEntry == null)
                    continue;

                m_ExistingAbilities.Add(abilityEntry.m_AbilityType, abilityEntry.m_NextUsageAllowed);
            }
            
            entry.UndeadProfile.m_Abilities.Clear();

            UOACZUndeadUpgradeType upgradeType = entry.UndeadProfile.ActiveForm;
            UOACZUndeadUpgradeDetail upgradeDetail = UOACZUndeadUpgrades.GetUpgradeDetail(upgradeType);

            List<UOACZUndeadAbilityType> m_Abilities = new List<UOACZUndeadAbilityType>();

            //Current Form Abilities
            for (int a = 0; a < upgradeDetail.m_Abilities.Count; a++)
            {
                m_Abilities.Add(upgradeDetail.m_Abilities[a]);
            }

            //Generic Abilities
            m_Abilities.Add(UOACZUndeadAbilityType.Regeneration);
            m_Abilities.Add(UOACZUndeadAbilityType.Sacrifice);
            m_Abilities.Add(UOACZUndeadAbilityType.GiftOfCorruption);
            m_Abilities.Add(UOACZUndeadAbilityType.Creep);
            m_Abilities.Add(UOACZUndeadAbilityType.Dig);
            m_Abilities.Add(UOACZUndeadAbilityType.Rally);

            for (int a = 0; a < m_Abilities.Count; a++)
            {
                UOACZUndeadAbilityType abilityType = m_Abilities[a];
                UOACZUndeadAbilityDetail abilityDetail = UOACZUndeadAbilities.GetAbilityDetail(abilityType);
                UOACZUndeadAbilityEntry abilityEntry = new UOACZUndeadAbilityEntry(abilityType, abilityDetail.CooldownMinutes, DateTime.UtcNow);
                                                    
                abilityEntry.m_NextUsageAllowed = DateTime.UtcNow + TimeSpan.FromMinutes(abilityEntry.m_CooldownMinutes);

                entry.UndeadProfile.m_Abilities.Add(abilityEntry);
            }

            //Restore Existing Ability Cooldowns if Ability Still Owned
            foreach (UOACZUndeadAbilityEntry abilityEntry in entry.UndeadProfile.m_Abilities)
            {
                if (abilityEntry == null)
                    continue;

                if (m_ExistingAbilities.ContainsKey(abilityEntry.m_AbilityType))
                    abilityEntry.m_NextUsageAllowed = m_ExistingAbilities[abilityEntry.m_AbilityType];
            }
        }
    }

    public class UOACZUndeadUpgradeDetail
    {
        public string m_Name = "Zombie";
        public string[] m_Description = new string[] { "",
                                                                 "",                                                                 
                                                               };      
      
        public int m_MonsterTier = 1;

        public int m_BodyValue = 3;
        public int m_HueMod = 0;

        public int m_IconItemID = 8428;
        public int m_IconHue = 0;
        public int m_IconOffsetX = 0;
        public int m_IconOffsetY = 0;
        
        public int m_AngerSound = 471;
        public int m_IdleSound = 471;
        public int m_AttackSound = 471;
        public int m_HurtSound = 471;
        public int m_DeathSound = 471;

        public int m_AttackAnimation = -1;
        public int m_AttackAnimationFrames = 0;

        public int m_IdleAnimation = -1;
        public int m_IdleAnimationFrames = 0;

        public int m_HurtAnimation = -1;
        public int m_HurtAnimationFrames = 0;

        public int m_SpecialAnimation = 11;
        public int m_SpecialAnimationFrames = 5;

        public int m_CastingAnimation = 4;
        public int m_CastingAnimationFrames = 5;

        public int m_MaxFollowers = 0;

        public int m_DamageMin = 0;
        public int m_DamageMax = 0;

        public int m_VirtualArmor = 0;  

        public Dictionary<StatType, int> m_Stats = new Dictionary<StatType, int>();
        public Dictionary<SkillName, double> m_Skills = new Dictionary<SkillName, double>();
        public List<UOACZUndeadAbilityType> m_Abilities = new List<UOACZUndeadAbilityType>();

        public UOACZUndeadUpgradeDetail()
        {
            #region Stats and Skills

            m_Stats.Add(StatType.Str, 0);
            m_Stats.Add(StatType.Dex, 0);
            m_Stats.Add(StatType.Int, 0);

            int skillsCount = Enum.GetNames(typeof(SkillName)).Length;

            for (int a = 0; a < skillsCount; a++)
            {
                m_Skills.Add((SkillName)a, 0);
            }

            #endregion
        }
    }
}