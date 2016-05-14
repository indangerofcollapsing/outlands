using System;
using Server;
using Server.Mobiles;
using System.Collections;
using System.Collections.Generic;
using Server.Network;

namespace Server
{
    public class ArcaneItem
    {
        public static void RecordDamage(PlayerMobile player, BaseCreature creature, int amount)
        {
            if (player == null || creature == null) return;
            if (creature.Summoned || creature.NoKillAwards && creature.DiedByShipSinking) return;

            ArcaneItemExperienceEntry playerEntry = GetArcaneItemExperienceEntry(player, creature);

            if (playerEntry == null)
            {
                playerEntry = new ArcaneItemExperienceEntry();

                creature.ArcaneItemExperienceEntries.Add(playerEntry);
            }
            


            //TEST: FINISH
        }

        public static ArcaneItemExperienceEntry GetArcaneItemExperienceEntry(PlayerMobile player, BaseCreature creature)
        {
             ArcaneItemExperienceEntry playerEntry = null;

            if (player == null || creature == null)
                return playerEntry;

            foreach (ArcaneItemExperienceEntry entry in creature.ArcaneItemExperienceEntries)
            {
                if (entry == null)
                    continue;

                if (entry.Player == player)
                    return entry;
            }

            return playerEntry;
        }

        public static void RemoveArcaneItemExperienceEntry(PlayerMobile player, BaseCreature creature)
        {
            ArcaneItemExperienceEntry playerEntry = null;

            if (player == null || creature == null)
                return;

            foreach (ArcaneItemExperienceEntry entry in creature.ArcaneItemExperienceEntries)
            {
                if (entry == null)
                    continue;

                if (entry.Player == player)
                {
                    playerEntry = entry;
                    break;
                }
            }

            if (playerEntry != null)
            {
                //TEST: Remove references in
                //creature.ArcaneItems

                if (creature.ArcaneItemExperienceEntries.Contains(playerEntry))
                    creature.ArcaneItemExperienceEntries.Remove(playerEntry);
            }
        }
    }

    public class ArcaneItemExperienceEntry
    {
        public PlayerMobile Player;
        public Item item;
    }

        //Arcane Items
        /*
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

    if (creature.IsChamp())
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

    public class DungeonWeaponDamageEntry
    {
    public PlayerMobile Player;
    public BaseWeapon Weapon;
    public int Damage;
    }
    */

        /*
        //Dungeon Weapon Experience
        DungeonWeaponDamageEntry bestDungeonWeaponDamageEntry = null;
        int bestDungeonWeaponDamage = 0;

        foreach (DungeonWeaponDamageEntry dungeonWeaponDamageEntry in DungeonWeaponDamageEntries)
        {
            if (dungeonWeaponDamageEntry == null) continue;
            if (dungeonWeaponDamageEntry.Weapon == null) continue;
            if (dungeonWeaponDamageEntry.Weapon.Deleted) continue;

            if (dungeonWeaponDamageEntry.Damage > bestDungeonWeaponDamage)
            {
                bestDungeonWeaponDamageEntry = dungeonWeaponDamageEntry;
                bestDungeonWeaponDamage = dungeonWeaponDamageEntry.Damage;
            }
        }

        if (bestDungeonWeaponDamageEntry != null)
            DungeonWeapon.CreatureKilled(this, bestDungeonWeaponDamageEntry);
        */
    
}
