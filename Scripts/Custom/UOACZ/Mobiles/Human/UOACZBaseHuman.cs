using System;
using Server.Items;
using Server.ContextMenus;
using Server.Multis;
using Server.Misc;
using Server.Network;
using Server.Custom;
using System.Collections;
using System.Collections.Generic;
using Server.Achievements;

namespace Server.Mobiles
{
	public class UOACZBaseHuman : BaseCreature
	{
        public static List<UOACZBaseHuman> m_Creatures = new List<UOACZBaseHuman>();

        private bool m_InWilderness = false;
        [CommandProperty(AccessLevel.Counselor)]
        public bool InWilderness
        {
            get { return m_InWilderness; }
            set { m_InWilderness = value; }
        }

        public virtual UOACZSystem.StockpileContributionType StockpileContributionType { get { return UOACZSystem.StockpileContributionType.None; } }
        public virtual double StockpileContributionScalar { get { return 1.0; } }
        
        public DateTime LastAttackMade = DateTime.UtcNow;
        public DateTime LastMovement = DateTime.UtcNow;

        public static TimeSpan AbortTargetDelay = TimeSpan.FromSeconds(10);

        public virtual bool Sentry { get { return false; } }

        public virtual int DifficultyValue { get { return 1; } }

        public override bool CanSwitchWeapons { get { return true; } }
        public override bool IsRangedPrimary { get { return false; } }       

        public override bool AllowParagon { get { return false; } }
        public override bool CanRummageCorpses { get { return false; } }

        public virtual string[] wildernessIdleSpeech { get { return new string[0]; } }
        public virtual string[] idleSpeech { get{ return new string[0];} }
        public virtual string[] undeadCombatSpeech { get { return new string[0]; } }
        public virtual string[] humanCombatSpeech { get { return new string[0]; } }        
        
        [Constructable]
		public UOACZBaseHuman() : base( AIType.AI_Archer, FightMode.Closest, 15, 1, 0.2, 0.4 )
		{
            SetStr(50);

            SpeechHue = 0;
            Hue = Utility.RandomSkinHue();

            if (this.Female = Utility.RandomBool())
            {
                Body = 0x191;
                Name = NameList.RandomName("female");
            }

            else
            {  
                Body = 0x190;
                Name = NameList.RandomName("male");
            }

            m_Creatures.Add(this);
		}

        public override void SetUniqueAI()
        {
            base.SetUniqueAI();
            
            DictCombatTargeting[CombatTargeting.UOACZEvilWildlife] = 1;
            //DictCombatTargeting[CombatTargeting.UOACZEvilHumanPlayer] = 2;
            DictCombatTargeting[CombatTargeting.UOACZUndeadPlayer] = 3;
            DictCombatTargeting[CombatTargeting.UOACZUndead] = 4;

            ActiveSpeed = 0.4;
            PassiveSpeed = 0.5;

            ResolveAcquireTargetDelay = 2;

            RangePerception = 10;
            DefaultPerceptionRange = 10;
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            base.OnDamage(amount, from, willKill);

            if (!willKill)
            {
                //Custom sound
            }
        }

        public override void OnSwing(Mobile defender)
        {
            base.OnSwing(defender);

            LastAttackMade = DateTime.UtcNow;
        }

        protected override bool OnMove(Direction d)
        {
            LastMovement = DateTime.UtcNow;

            return base.OnMove(d);
        }

        public override void OnThink()
        {
            base.OnThink();

            if (Combatant != null)
            {
                if ((LastMovement + AbortTargetDelay <= DateTime.UtcNow) && (LastAttackMade + AbortTargetDelay) <= DateTime.UtcNow)                
                    Combatant = null;                
            }

            if (Utility.RandomDouble() < 0.001)
            {
                if (Combatant == null)
                {
                    if (InWilderness && wildernessIdleSpeech.Length > 0)
                        Say(wildernessIdleSpeech[Utility.Random(wildernessIdleSpeech.Length - 1)]);

                    else if (idleSpeech.Length > 0)
                        Say(idleSpeech[Utility.Random(idleSpeech.Length - 1)]);
                }

                else
                {
                    bool undeadCombatant = false;

                    if (Combatant is UOACZBaseUndead)
                        undeadCombatant = true;

                    PlayerMobile pm_Combatant = Combatant as PlayerMobile;

                    if (pm_Combatant != null)
                    {
                        UOACZPersistance.CheckAndCreateUOACZAccountEntry(pm_Combatant);

                        if (pm_Combatant.m_UOACZAccountEntry.ActiveProfile == UOACZAccountEntry.ActiveProfileType.Undead)
                            undeadCombatant = true;
                    }

                    if (undeadCombatant && undeadCombatSpeech.Length > 0)
                        Say(undeadCombatSpeech[Utility.Random(undeadCombatSpeech.Length - 1)]);

                    else if (humanCombatSpeech.Length > 0)
                        Say(humanCombatSpeech[Utility.Random(humanCombatSpeech.Length - 1)]);
                }
            }
        }

        public virtual void UOACZCarve(Mobile from, Corpse corpse, Item with)
        {
            from.Animate(32, 3, 1, true, false, 0);
            Effects.PlaySound(from.Location, from.Map, 0x3E3);

            new Blood(0x122D).MoveToWorld(corpse.Location, corpse.Map);
            corpse.Carved = true;

            if (Utility.RandomDouble() <= .2)
                corpse.DropItem(new UOACZIntestines());

            from.SendMessage("You carve the corpse.");
        }

        public override void OnCarve(Mobile from, Corpse corpse, Item with)
        {
            UOACZCarve(from, corpse, with);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            Dictionary<PlayerMobile, int> m_PlayerDamageDealt = new Dictionary<PlayerMobile, int>();
            List<PlayerMobile> m_PotentialPlayers = new List<PlayerMobile>();            

            int scoreValue = 1;

            double rewardChance = 0.0;
            double corruptionStoneChance = 0.0;
            double upgradeTokenChance = 0.0;

            #region Upgrade Chances

            switch (DifficultyValue)
            {
                //Civilian
                case 1:
                    scoreValue = 1;

                    rewardChance = .8;
                    corruptionStoneChance = .6;
                    upgradeTokenChance = .4;
                break;

                case 2:
                    scoreValue = 2;

                    rewardChance = .8;
                    corruptionStoneChance = .6;
                    upgradeTokenChance = .4;
                break;

                //Militia
                case 3:
                    scoreValue = 3;

                    rewardChance = 1.0;
                    corruptionStoneChance = .8;
                    upgradeTokenChance = .6;
                break;

                case 4:
                    scoreValue = 4;

                    rewardChance = 1.0;
                    corruptionStoneChance = .8;
                    upgradeTokenChance = .6;
                break;

                //Elite
                case 5:
                    scoreValue = 5;

                    rewardChance = 1.2;
                    corruptionStoneChance = 1.0;
                    upgradeTokenChance = .8;
                break;

                case 6:
                    scoreValue = 6;

                    rewardChance = 1.2;
                    corruptionStoneChance = 1.0;
                    upgradeTokenChance = .8;
                break;

                //Sentry
                case 7:
                    scoreValue = 7;

                    rewardChance = 1.4;
                    corruptionStoneChance = 1.2;
                    upgradeTokenChance = 1.0;
                break;

                case 8:
                    scoreValue = 8;

                    rewardChance = 1.4;
                    corruptionStoneChance = 1.2;
                    upgradeTokenChance = 1.0;
                break;

                //Unique
                case 9:
                    scoreValue = 9;

                    rewardChance = 2;
                    corruptionStoneChance = 2;
                    upgradeTokenChance = 2;
                break;

                case 10:
                    scoreValue = 40;

                    rewardChance = 2;
                    corruptionStoneChance = 2;
                    upgradeTokenChance = 2;
                break;

                case 11:
                    scoreValue = 80;

                    rewardChance = 2;
                    corruptionStoneChance = 2;
                    upgradeTokenChance = 2;
                break;
            }

            #endregion

            bool playerThresholdReached = false;

            int totalDamage = 0;
            int totalPlayerDamage = 0;

            //Determine Total Damaged Inflicted and Per Player
            foreach (DamageEntry entry in DamageEntries)
            {
                if (!entry.HasExpired)
                {
                    Mobile damager = entry.Damager;

                    if (damager == null)
                        continue;

                    totalDamage += entry.DamageGiven;

                    PlayerMobile playerDamager = damager as PlayerMobile;

                    if (playerDamager != null)
                        totalPlayerDamage += entry.DamageGiven;

                    BaseCreature creatureDamager = damager as BaseCreature;

                    if (creatureDamager != null)
                    {
                        if (creatureDamager.ControlMaster is PlayerMobile)
                            totalPlayerDamage += entry.DamageGiven;
                    }
                }
            }

            foreach (DamageEntry entry in DamageEntries)
            {
                if (!entry.HasExpired && entry.DamageGiven > 0)
                {
                    PlayerMobile player = null;

                    Mobile damager = entry.Damager;

                    if (damager == null) continue;
                    if (damager.Deleted) continue;

                    PlayerMobile pm_Damager = damager as PlayerMobile;
                    BaseCreature bc_Damager = damager as BaseCreature;

                    if (pm_Damager != null)
                        player = pm_Damager;

                    if (bc_Damager != null)
                    {
                        if (bc_Damager.Controlled && bc_Damager.ControlMaster is PlayerMobile)
                        {
                            if (!bc_Damager.ControlMaster.Deleted)
                                player = bc_Damager.ControlMaster as PlayerMobile;
                        }
                    }

                    if (player != null)
                    {
                        if (m_PlayerDamageDealt.ContainsKey(player))
                            m_PlayerDamageDealt[player] += entry.DamageGiven;

                        else
                            m_PlayerDamageDealt.Add(player, entry.DamageGiven);
                    }
                }
            }

            Queue m_Queue = new Queue();

            foreach(KeyValuePair<PlayerMobile, int> playerEntry in m_PlayerDamageDealt)
            {
                PlayerMobile player = playerEntry.Key;
                int damage = playerEntry.Value;

                if (player.IsUOACZUndead)
                {
                    double damagePercentOfTotal = (double)damage / totalDamage;

                    if (damage >= 100 || damagePercentOfTotal > .10)
                    {
                        AchievementSystemImpl.Instance.TickProgressMulti(player, AchievementTriggers.Trigger_UOACZScoreFromKillingHumans, scoreValue);
                        AchievementHandling(player, GetType());

                        UOACZSystem.ChangeStat(player, UOACZSystem.UOACZStatType.UndeadScore, scoreValue, true);

                        if (!player.m_UOACZAccountEntry.UndeadProfile.m_FormsKilledWith.Contains(player.m_UOACZAccountEntry.UndeadProfile.ActiveForm))
                        {
                            player.m_UOACZAccountEntry.UndeadProfile.m_FormsKilledWith.Add(player.m_UOACZAccountEntry.UndeadProfile.ActiveForm);

                            if (player.m_UOACZAccountEntry.UndeadProfile.m_FormsKilledWith.Count == 8)
                                AchievementSystemImpl.Instance.TickProgressMulti(player, AchievementTriggers.Trigger_UOACZKillWithDifferentForms1, 1);

                            if (player.m_UOACZAccountEntry.UndeadProfile.m_FormsKilledWith.Count == 12)
                                AchievementSystemImpl.Instance.TickProgressMulti(player, AchievementTriggers.Trigger_UOACZKillWithDifferentForms2, 1);

                            if (player.m_UOACZAccountEntry.UndeadProfile.m_FormsKilledWith.Count == 16)
                                AchievementSystemImpl.Instance.TickProgressMulti(player, AchievementTriggers.Trigger_UOACZKillWithDifferentForms3, 1);
                        }

                        if (this is UOACZBaseCivilian)
                            player.m_UOACZAccountEntry.CiviliansKilledAsUndead++;

                        if (this is UOACZBaseMilitia)
                            player.m_UOACZAccountEntry.MilitiaKilledAsUndead++;                        

                        m_PotentialPlayers.Add(player);
                    }
                }

                else if (player.IsUOACZHuman)
                    m_Queue.Enqueue(player);
            }

            while (m_Queue.Count > 0)
            {
                PlayerMobile player = (PlayerMobile)m_Queue.Dequeue();

                player.SendMessage("You have killed a human.");

                UOACZSystem.ChangeStat(player, UOACZSystem.UOACZStatType.Honor, UOACZSystem.HumanKillCivilianHonorLoss, true);                
            }

            double playerDamageRatio = (double)totalPlayerDamage / (double)totalDamage;
            double npcHelpScalar = playerDamageRatio;

            if (m_PotentialPlayers.Count >= 3)
            {
                rewardChance *= .75;
                corruptionStoneChance *= .75;
                upgradeTokenChance *= .75;
            }

            if (m_PotentialPlayers.Count >= 5)
            {
                rewardChance *= .75;
                corruptionStoneChance *= .75;
                upgradeTokenChance *= .75;
            }

            rewardChance *= UOACZPersistance.UndeadBalanceScalar;
            corruptionStoneChance *= UOACZPersistance.UndeadBalanceScalar;
            upgradeTokenChance *= UOACZPersistance.UndeadBalanceScalar;

            foreach (PlayerMobile player in m_PotentialPlayers)
            {
                bool dropCorruptionStone = false;
                bool dropUpgradeToken = false;                

                if (Utility.RandomDouble() <= (rewardChance * npcHelpScalar))
                {
                    if (player.Backpack != null)
                    {
                        player.Backpack.DropItem(new UOACZBrains());
                        player.SendMessage(UOACZSystem.greenTextHue, "You have received a reward.");
                    }
                }

                if (Utility.RandomDouble() <= (corruptionStoneChance * npcHelpScalar))
                {
                    if (player.Backpack != null)
                    {
                        player.Backpack.DropItem(new UOACZCorruptionStone(player));
                        player.SendMessage(UOACZSystem.greenTextHue, "You have received a corruption stone.");
                    }
                }

                if (Utility.RandomDouble() <= (upgradeTokenChance * npcHelpScalar))
                {
                    if (player.Backpack != null)
                    {
                        player.Backpack.DropItem(new UOACZUndeadUpgradeToken(player));
                        player.SendMessage(UOACZSystem.greenTextHue, "You have received an upgrade token.");
                    }
                }
            }
        }

        public void AchievementHandling(PlayerMobile player, Type type)
        {
            if (type == typeof(UOACZFirstRanger))
                AchievementSystemImpl.Instance.TickProgressMulti(player, AchievementTriggers.Trigger_UOACZKillHumanChampion, 1);

            if (type == typeof(UOACZFortCommander))
                AchievementSystemImpl.Instance.TickProgressMulti(player, AchievementTriggers.Trigger_UOACZKillHumanBoss, 1);
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (m_Creatures.Contains(this))
                m_Creatures.Remove(this);
        }

        public UOACZBaseHuman(Serial serial): base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version

            //Version 0
            writer.Write(m_InWilderness);           
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_InWilderness = reader.ReadBool();               
            }

            //----------

            m_Creatures.Add(this);
		}
	}
}
