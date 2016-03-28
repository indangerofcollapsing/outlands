using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;
using System.Globalization;

namespace Server.Items
{
    public class DungeonWeapon
    {
        public enum SpecialEffectType
        {
            ArcaneSurge,
            ShockStorm,
            Whirlwind,
            Execute,
            PoisonWind,
            Shadows,
            Command,
            FrostNova,
            Firestorm
        }

        public static int CoresNeededForCreation = 8;
        public static int CoresNeededForUpgrade = 4;

        public static int DistillationNeededForCreation = 2;
        public static int DistillationNeededForUpgrade = 1;

        public static int BaseCraftingSkillNeeded = 100;
        public static int ExtraCraftingSkillNeededPerTier = 2;

        public static int MaxDungeonTier = 10;
        public static int MaxDungeonExperience = 250;
        public static int ArcaneMaxCharges = 200;

        public static double BaseAccuracy = .05;
        public static double AccuracyPerTier = .02;

        public static int BaseTactics = 10;
        public static int TacticsPerTier = 3;

        public static double BaseXPGainScalar = .01;      

        public static double LowContributionThreshold = .05; //If Total Damage Inflicted is Lower Than This Percent of Total Hit Points
        public static double LowContributionScalar = .5; //Reduction to XP Gain Chance for Low Damage Contribution 

        public static int NormalGain = 1;
        public static int ChampGain = 5;
        public static int LoHGain = 5;
        public static int BossGain = 15;
        public static int EventBossGain = 25;

        public static double BaseEffectChance = .01;
        public static double BaseEffectChancePerTier = .005;

        public static int BaselineDurability = 150;
        public static int IncreasedDurabilityPerTier = 20;       

        public static double GetSpeedScalar(double speed)
        {
            double scalar = 1.0;

            double minSpeed = 25;
            double maxSpeed = 60;

            scalar += 1 * ((maxSpeed - speed) / (maxSpeed - minSpeed));

            return scalar;
        }

        public static DungeonWeaponDetail GetDungeonWeaponDetail(DungeonEnum dungeon)
        {
            DungeonWeaponDetail detail = new DungeonWeaponDetail();

            switch (dungeon)
            {
                case DungeonEnum.Covetous:
                    detail.m_SpecialEffect = SpecialEffectType.PoisonWind;
                    detail.m_EffectDisplayName = "Poison Wind";
                    detail.m_EffectDescription = "";
                break;

                case DungeonEnum.Deceit:
                    detail.m_SpecialEffect = SpecialEffectType.ShockStorm;
                    detail.m_EffectDisplayName = "Shock Storm";
                    detail.m_EffectDescription = "";
                break;

                case DungeonEnum.Despise:
                    detail.m_SpecialEffect = SpecialEffectType.Command;
                    detail.m_EffectDisplayName = "Command";
                    detail.m_EffectDescription = "";
                break;

                case DungeonEnum.Destard:
                    detail.m_SpecialEffect = SpecialEffectType.Whirlwind;
                    detail.m_EffectDisplayName = "Whirlwind";
                    detail.m_EffectDescription = "";
                break;

                case DungeonEnum.Fire:
                    detail.m_SpecialEffect = SpecialEffectType.Firestorm;
                    detail.m_EffectDisplayName = "Firestorm";
                    detail.m_EffectDescription = "";
                break;

                case DungeonEnum.Hythloth:
                    detail.m_SpecialEffect = SpecialEffectType.Execute;
                    detail.m_EffectDisplayName = "Execute";
                    detail.m_EffectDescription = "";
                break;

                case DungeonEnum.Ice:
                    detail.m_SpecialEffect = SpecialEffectType.FrostNova;
                    detail.m_EffectDisplayName = "Frost Nova";
                    detail.m_EffectDescription = "";
                break;

                case DungeonEnum.Shame:
                    detail.m_SpecialEffect = SpecialEffectType.ArcaneSurge;
                    detail.m_EffectDisplayName = "Arcane Surge";
                    detail.m_EffectDescription = "";
                break;

                case DungeonEnum.Wrong:
                    detail.m_SpecialEffect = SpecialEffectType.Shadows;
                    detail.m_EffectDisplayName = "Shadows";
                    detail.m_EffectDescription = "";
                break;
            }
            
            return detail;
        }

        public static void CreatureKilled(BaseCreature creature, DungeonWeaponDamageEntry dungeonWeaponDamageEntry)
        {
            if (creature == null || dungeonWeaponDamageEntry == null)
                return;

            PlayerMobile player = dungeonWeaponDamageEntry.Player;
            BaseWeapon weapon = dungeonWeaponDamageEntry.Weapon;
            int damage = dungeonWeaponDamageEntry.Damage;

            if (player == null || weapon == null || damage == 0) return;
            if (weapon.Deleted) return;

            if (weapon.Experience == MaxDungeonExperience)
                return;

            double baseGainChance = BaseXPGainScalar * creature.InitialDifficulty;           
            double contributionScalar = 1;

            if (((double)dungeonWeaponDamageEntry.Damage / (double)creature.HitsMax) < LowContributionThreshold)
                contributionScalar = LowContributionScalar;

            double finalChance = baseGainChance * contributionScalar;

            int xpGain = NormalGain;

            if (creature.IsMiniBoss())
            {
                xpGain = ChampGain;
                finalChance = 1.0;
            }

            if (creature.IsBoss())
            {
                xpGain = BossGain;
                finalChance = 1.0;
            }

            if (creature.IsLoHBoss())
            {
                xpGain = LoHGain;
                finalChance = 1.0;
            }

            if (creature.IsEventBoss())
            {
                xpGain = EventBossGain;
                finalChance = 1.0;
            }

            if (Utility.RandomDouble() <= finalChance)
            {
                weapon.Experience += xpGain;

                player.SendMessage("Your dungeon weapon has gained " + xpGain.ToString() + " experience.");

                if (weapon.Experience > MaxDungeonExperience)
                    weapon.Experience = MaxDungeonExperience;

                if (weapon.Experience == MaxDungeonExperience && weapon.TierLevel < MaxDungeonTier)
                {
                    player.SendMessage(0x3F, "Your dungeon weapon has acquired enough experience to increase it's tier.");
                    player.SendSound(0x5A7);
                }
            }
        }

        public static void CheckResolveSpecialEffect(BaseWeapon weapon, PlayerMobile attacker, BaseCreature defender)
        {
            if (weapon == null || attacker == null || defender == null) return;
            if (weapon.Dungeon == DungeonEnum.None) return;
            if (weapon.TierLevel == 0) return;

            double effectChance = BaseEffectChance + ((double)weapon.TierLevel * BaseEffectChancePerTier);
            double speedScalar = GetSpeedScalar(weapon.Speed);

            double finalChance = effectChance * speedScalar;
                        
            if (Utility.RandomDouble() <= finalChance)
            {
                switch (weapon.Dungeon)
                {
                }
            }
        }
    }

    public class DungeonWeaponGump : Gump
    {
        public Mobile m_GumpTarget;
        public BaseWeapon m_Weapon;
        public bool m_ArmsLoreSuccess;

        public DungeonWeaponGump(Mobile gumpTarget, BaseWeapon weapon, bool armsLoreSuccess): base(10, 10)
        {
            m_GumpTarget = gumpTarget;
            m_Weapon = weapon;
            m_ArmsLoreSuccess = armsLoreSuccess;

            if (weapon == null) return;
            if (weapon.TierLevel == 0 || weapon.Dungeon == DungeonEnum.None) return;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            int WhiteTextHue = 2655;
            int DetailHue = 149;         

            int weaponTier = m_Weapon.TierLevel;
            string dungeonName = BaseWeapon.GetDungeonName(m_Weapon.Dungeon) + " Dungeon";
            string weaponName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(m_Weapon.Name);
            int newDurability = DungeonWeapon.BaselineDurability + (DungeonWeapon.IncreasedDurabilityPerTier * weaponTier);

            int adjustedBlessedCharges = DungeonWeapon.ArcaneMaxCharges;

            double accuracy = 100 * (DungeonWeapon.BaseAccuracy + (DungeonWeapon.AccuracyPerTier * (double)weaponTier));
            double tactics = DungeonWeapon.BaseTactics + (DungeonWeapon.TacticsPerTier * (double)weaponTier);
            
            double effectChance = DungeonWeapon.BaseEffectChance + (DungeonWeapon.BaseEffectChancePerTier * (double)weaponTier);

            effectChance *= DungeonWeapon.GetSpeedScalar(m_Weapon.Speed);

            DungeonArmor.DungeonArmorDetail detail = new DungeonArmor.DungeonArmorDetail(m_Weapon.Dungeon, m_Weapon.TierLevel);

            int itemId = weapon.ItemID;
            int itemHue = detail.Hue - 1;
            int offsetX = 0;
            int offsetY = 0;

            DetailHue = itemHue;

            AddPage(0);

            AddImage(220, 123, 103);
            AddImage(220, 65, 103);
            AddImage(220, 4, 103);
            AddImage(98, 124, 103);
            AddImage(98, 65, 103);
            AddImage(98, 4, 103);
            AddImage(5, 124, 103);
            AddImage(5, 65, 103);
            AddImage(5, 4, 103);
            AddImage(18, 16, 3604, 2052);
            AddImage(18, 85, 3604, 2052);
            AddImage(102, 16, 3604, 2052);
            AddImage(102, 85, 3604, 2052);
            AddImage(222, 16, 3604, 2052);
            AddImage(222, 85, 3604, 2052);
            
            AddLabel(Utility.CenteredTextOffset(90, dungeonName), 25, DetailHue, dungeonName);
            AddLabel(Utility.CenteredTextOffset(90, weaponName), 45, DetailHue, weaponName);
            AddItem(75 + weapon.IconOffsetX, 85 + weapon.IconOffsetY, weapon.IconItemId, itemHue);
            AddLabel(69, 140, DetailHue, "Tier 1");

            string chargesText = adjustedBlessedCharges.ToString();
            string experienceText = weapon.Experience + "/" + DungeonWeapon.MaxDungeonExperience.ToString();
            string durabilityText = weapon.HitPoints + "/" + weapon.MaxHitPoints;
            string accuracyText = "+" + accuracy.ToString() + "%";
            string tacticsText = "+" + tactics.ToString();
            string effectChanceText = Utility.CreateDecimalPercentageString(effectChance, 1);

            if (!armsLoreSuccess)
                durabilityText = "?" + "/" + weapon.MaxHitPoints;

            AddLabel(217, 25, WhiteTextHue, "Charges:");
            AddLabel(280, 25, DetailHue, chargesText);

            AddLabel(201, 45, WhiteTextHue, "Experience:");
            AddLabel(280, 45, DetailHue, experienceText);

            AddLabel(206, 65, WhiteTextHue, "Durability:");
            AddLabel(280, 65, DetailHue, durabilityText);          

            AddLabel(209, 85, WhiteTextHue, "Accuracy:");
            AddLabel(280, 85, DetailHue, accuracyText);

            AddLabel(220, 105, WhiteTextHue, "Tactics:");
            AddLabel(280, 105, DetailHue, tacticsText);           

            AddLabel(176, 125, WhiteTextHue, "Effect Chance:");
            AddLabel(280, 125, DetailHue, effectChanceText);
            AddLabel(176, 145, WhiteTextHue, "(scaled for weapon speed)");

            DungeonWeaponDetail weaponDetail = DungeonWeapon.GetDungeonWeaponDetail(weapon.Dungeon);

            AddLabel(70, 185, WhiteTextHue, "Special Effect:");
            AddButton(170, 188, 1210, 1209, 1, GumpButtonType.Reply, 0);
            AddLabel(190, 185, DetailHue, weaponDetail.m_EffectDisplayName);     
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_GumpTarget == null || m_Weapon == null) return;
            if (m_Weapon.Deleted) return;

            if (m_Weapon.TierLevel == 0) return;
            if (m_Weapon.Dungeon == DungeonEnum.None) return;

            bool closeGump = true;

            if (info.ButtonID == 1)
            {
                DungeonWeaponDetail weaponDetail = DungeonWeapon.GetDungeonWeaponDetail(m_Weapon.Dungeon);

                m_GumpTarget.SendMessage(weaponDetail.m_EffectDisplayName + ": " + weaponDetail.m_EffectDescription);

                closeGump = false;
            }

            if (!closeGump)
            {
                m_GumpTarget.CloseGump(typeof(DungeonWeaponGump));
                m_GumpTarget.SendGump(new DungeonWeaponGump(m_GumpTarget, m_Weapon, m_ArmsLoreSuccess));
            }
        }
    }

    public class DungeonWeaponDetail
    {
        public DungeonWeapon.SpecialEffectType m_SpecialEffect = DungeonWeapon.SpecialEffectType.ArcaneSurge;
        public string m_EffectDisplayName = "";    
        public string m_EffectDescription = "";       
    }

    public class DungeonWeaponDamageEntry
    {
        public PlayerMobile Player;
        public BaseWeapon Weapon;
        public int Damage;
    }
}