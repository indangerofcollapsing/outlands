using System;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.Achievements;

namespace Server.Custom
{
    public class UOACZBreakableStatic : BreakableStatic
    {
        public static List<UOACZBreakableStatic> m_UOACZBreakableStatics = new List<UOACZBreakableStatic>();

        public virtual bool AllowHumanDamage { get { return false; } }

        public virtual int OverrideNormalItemId { get { return -1; } }
        public virtual int OverrideNormalHue { get { return -1; } }

        public virtual int OverrideLightlyDamagedItemId { get { return -1; } }
        public virtual int OverrideLightlyDamagedHue { get { return -1; } }

        public virtual int OverrideHeavilyDamagedItemId { get { return -1; } }
        public virtual int OverrideHeavilyDamagedHue { get { return -1; } }

        public virtual int OverrideBrokenItemId { get { return -1; } }
        public virtual int OverrideBrokenHue { get { return -1; } }

        private UOACZConstructionObjectEffectTargeter m_EffectTargeter;
        [CommandProperty(AccessLevel.GameMaster)]
        public UOACZConstructionObjectEffectTargeter EffectTargeter
        {
            get { return m_EffectTargeter; }
            set { m_EffectTargeter = value; }
        }

        [Constructable]
        public UOACZBreakableStatic(): base()
        {
            UpdateOverrides();

            AllowRepair = true;
            DeleteOnBreak = false;
            CreateTimedStaticAfterBreak = false;

            ObjectBreakingDeviceDamageScalar = 0;
            MiningDamageScalar = 0;

            MaxHitPoints = 3000;
            HitPoints = 3000;

            MinInteractDamage = 3;
            MaxInteractDamage = 5;            

            m_UOACZBreakableStatics.Add(this);
        }

        public void UpdateOverrides()
        {
            if (OverrideNormalItemId != -1)
            {
                NormalItemId = OverrideNormalItemId;
                ItemID = NormalItemId;
            }

            if (OverrideNormalHue != -1)
            {
                NormalHue = OverrideNormalHue;
                Hue = NormalHue;
            }

            if (OverrideLightlyDamagedItemId != -1)
                LightlyDamagedItemId = OverrideLightlyDamagedItemId;

            if (OverrideLightlyDamagedHue != -1)
                LightlyDamagedHue = OverrideLightlyDamagedHue;

            if (OverrideHeavilyDamagedItemId != -1)
                HeavilyDamagedItemId = OverrideHeavilyDamagedItemId;

            if (OverrideHeavilyDamagedHue != -1)
                HeavilyDamagedHue = OverrideHeavilyDamagedHue;

            if (OverrideBrokenItemId != -1)
                BrokenItemId = OverrideBrokenItemId;

            if (OverrideBrokenHue != -1)
                BrokenHue = OverrideBrokenHue;
        }

        public override void OnDoubleClick(Mobile from)
        {
            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;
                        
            if (player.IsUOACZHuman && !AllowHumanDamage)
            {
                if (HitPoints == MaxHitPoints)
                    player.SendMessage("This is at full durability, but may be repaired with a repair hammer should it become damaged.");

                else
                    player.SendMessage("A repair hammer must be used to make any meaningful repairs.");

                return;
            }
                     
            base.OnDoubleClick(player);      
        }

        public override void AfterReceiveDamage(Mobile from, int damage, InteractionType interactionType)
        {
            base.AfterReceiveDamage(from, damage, interactionType);

            PlayerMobile player = from as PlayerMobile;

            if (player != null)
            {
                UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);
                player.m_UOACZAccountEntry.TotalObjectDamage++; 
            } 

            if (this is UOACZCorruptionSourcestone)
            {
                UOACZEvents.SourceOfCorruptionDamaged(false);

                if (player != null)
                {
                    UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

                    if (player.IsUOACZHuman)
                    {
                        if (Utility.RandomDouble() <= UOACZSystem.HumanDamageSourceStoneScoreChance)
                            UOACZSystem.ChangeStat(player, UOACZSystem.UOACZStatType.HumanScore, 1, true);

                        if (player.Backpack != null)
                        {
                            if (Utility.RandomDouble() <= UOACZSystem.HumanDamageSourceStoneSurvivalStoneChance * UOACZPersistance.HumanBalanceScalar)
                            {
                                player.Backpack.DropItem(new UOACZSurvivalStone(player));
                                player.SendMessage(UOACZSystem.greenTextHue, "You have earned a survival stone for damaging the corruption sourcestone!");
                            }

                            if (Utility.RandomDouble() <= UOACZSystem.HumanDamageSourceStoneUpgradeTokenChance * UOACZPersistance.HumanBalanceScalar)
                            {
                                player.Backpack.DropItem(new UOACZHumanUpgradeToken(player));
                                player.SendMessage(UOACZSystem.greenTextHue, "You have earned an upgrade token for damaging the corruption sourcestone!");
                            }
                        }
                    }
                }
            }

            else if (this is UOACZStockpile)
            { 
                if (player != null)
                {
                    UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

                    if (player.IsUOACZUndead)
                    {
                        AchievementSystemImpl.Instance.TickProgressMulti(player, AchievementTriggers.Trigger_UOACZDamageObjects, 1);

                        if (Utility.RandomDouble() <= UOACZSystem.UndeadDamageStockpileScoreChance)
                            UOACZSystem.ChangeStat(player, UOACZSystem.UOACZStatType.UndeadScore, 1, true);

                        if (Utility.RandomDouble() <= UOACZSystem.UndeadDamageStockpileCorruptionTokenChance * UOACZPersistance.UndeadBalanceScalar)
                        {
                            if (player.Backpack != null)
                            {
                                player.Backpack.DropItem(new UOACZCorruptionStone(player));
                                player.SendMessage(UOACZSystem.greenTextHue, "You have received a corruption stone for damaging a stockpile!");
                            }
                        }

                        if (Utility.RandomDouble() <= UOACZSystem.UndeadDamageStockpileUpgradeTokenChance * UOACZPersistance.UndeadBalanceScalar)
                        {
                            if (player.Backpack != null)
                            {
                                player.Backpack.DropItem(new UOACZUndeadUpgradeToken(player));
                                player.SendMessage(UOACZSystem.greenTextHue, "You have received an upgrade token for damaging a stockpile!");
                            }
                        }

                        if (Utility.RandomDouble() <= UOACZSystem.UndeadDamageStockpileRewardChance * UOACZPersistance.UndeadBalanceScalar)
                        {
                            if (player.Backpack != null)
                            {
                                player.Backpack.DropItem(new UOACZBrains());
                                player.SendMessage(UOACZSystem.greenTextHue, "You have received a reward for damaging a stockpile!");
                            }
                        }
                    }
                }                
            }

            else
            {
                if (player != null)
                {
                    UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

                    if (player.IsUOACZUndead)
                    {
                        bool outpostObject = false;

                        if (UOACZPersistance.m_OutpostComponents.Contains(this))
                            outpostObject = true;

                        double scoreChance = UOACZSystem.UndeadDamageObjectScoreChance;
                        double corruptionChance = UOACZSystem.UndeadDamageObjectCorruptionTokenChance * UOACZPersistance.UndeadBalanceScalar;
                        double upgradeChance = UOACZSystem.UndeadDamageObjectUpgradeTokenChance * UOACZPersistance.UndeadBalanceScalar;
                        double rewardChance = UOACZSystem.UndeadDamageObjectRewardChance * UOACZPersistance.UndeadBalanceScalar;

                        if (outpostObject)
                        {
                            scoreChance *= UOACZSystem.UndeadDamageOutpostRewardReduction;
                            corruptionChance *= UOACZSystem.UndeadDamageOutpostRewardReduction;
                            upgradeChance *= UOACZSystem.UndeadDamageOutpostRewardReduction;
                            rewardChance *= UOACZSystem.UndeadDamageOutpostRewardReduction;

                            if (UOACZPersistance.m_HumanObjective1 == UOACZPersistance.m_HumanObjective1Target)
                            {
                                scoreChance *= UOACZSystem.UndeadDamageOutpostAlreadyBuiltRewardReduction;
                                corruptionChance *= UOACZSystem.UndeadDamageOutpostAlreadyBuiltRewardReduction;
                                upgradeChance *= UOACZSystem.UndeadDamageOutpostAlreadyBuiltRewardReduction;
                                rewardChance *= UOACZSystem.UndeadDamageOutpostAlreadyBuiltRewardReduction;
                            }
                        }

                        if (Utility.RandomDouble() <= scoreChance)
                            UOACZSystem.ChangeStat(player, UOACZSystem.UOACZStatType.UndeadScore, 1, true);

                        AchievementSystemImpl.Instance.TickProgressMulti(player, AchievementTriggers.Trigger_UOACZDamageObjects, 1);

                        if (player.Backpack != null)
                        {
                            if (Utility.RandomDouble() <= corruptionChance)
                            {
                                player.Backpack.DropItem(new UOACZCorruptionStone(player));
                                player.SendMessage(UOACZSystem.greenTextHue, "You have earned a corruption stone for destruction!");
                            }

                            if (Utility.RandomDouble() <= upgradeChance)
                            {
                                player.Backpack.DropItem(new UOACZUndeadUpgradeToken(player));
                                player.SendMessage(UOACZSystem.greenTextHue, "You have earned an upgrade token for destruction!");
                            }

                            if (Utility.RandomDouble() <= rewardChance)
                            {
                                player.Backpack.DropItem(new UOACZBrains());
                                player.SendMessage(UOACZSystem.greenTextHue, "You have received a reward for destruction.");
                            }
                        }
                    }
                }
            }
        }

        public bool CanRepair(Mobile from, Item item, double value, bool message)
        {
            bool canRepair = true;

            PlayerMobile player = from as PlayerMobile;

            if (!UOACZSystem.IsUOACZValidMobile(player)) return false;
            if (!player.IsUOACZHuman) return false;            

            if (!AllowRepair)
            {
                if (message)
                    player.SendMessage("That is not repairable.");

                return false;
            }

            if (!player.InRange(GetWorldLocation(), InteractionRange))
            {
                if (message)
                    player.SendMessage("That is too far away.");

                return false;
            }

            if (HitPoints == MaxHitPoints)
            {
                if (message)
                    player.SendMessage("That is not damaged.");

                return false;
            }            

            return canRepair;
        }        
        
        public UOACZBreakableStatic(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)2); // version

            //Version 2
            writer.Write(m_EffectTargeter);
        }

        public override void Deserialize(GenericReader reader)
        {
            UpdateOverrides();

            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 2
            if (version >= 2)
            {
                m_EffectTargeter = (UOACZConstructionObjectEffectTargeter)reader.ReadItem();
            }

            m_UOACZBreakableStatics.Add(this);

            //----------

            UpdateDamagedState();
        }
    }
}
