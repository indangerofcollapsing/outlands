using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Custom;
using Server.Spells;
using System.Collections;
using System.Collections.Generic;


namespace Server.Items
{
    public class UOACZRepairHammer : Item
    {        
        public static TimeSpan RepairActionTickDuration = TimeSpan.FromSeconds(3);
        public static int RepairActionTicksNeeded = 3;

        private int m_Charges = 100;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges
        {
            get { return m_Charges; }
            set { m_Charges = value; }
        }

        public static int MaxCharges = 100;

        public PlayerMobile m_Owner;

        public Timer m_Timer;

        [Constructable]
        public UOACZRepairHammer() : base(4020)
        {
            Name = "a repair hammer";
            Hue = 0;

            Weight = 2;
        }

        public UOACZRepairHammer(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            LabelTo(from, "uses remaining: " + m_Charges.ToString());
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            PlayerMobile player = from as PlayerMobile;
            
            if (!UOACZSystem.IsUOACZValidMobile(player)) return;
            if (!player.IsUOACZHuman) return;

            if (!IsChildOf(player.Backpack))
            {
                player.SendMessage("That item must be in your pack in order to use it.");
                return;
            }

            if (!player.CanBeginAction(typeof(UOACZRepairHammer)))
            {
                player.SendMessage("You are already using a repair hammer.");
                return;
            }

            if (m_Owner != null && m_Owner != player)
            {
                player.SendMessage("Someone else is using that repair hammer.");
                return;
            }

            if (!player.CanBeginAction(typeof(BreakableStatic)))
            {
                player.SendMessage("You must wait a few moments before attempting another action.");
                return;
            }

            if (GetNearbyBreakableStatics(player).Count == 0)
            {
                player.SendMessage("There are no nearby objects in need of repairs.");
                return;
            }

            m_Timer = null;
            m_Timer = new InternalTimer(this, player);
            m_Timer.Start();

            player.SendMessage("You begin making repairs.");
        }

        public static List<UOACZBreakableStatic> GetNearbyBreakableStatics(PlayerMobile player)
        {
            List<UOACZBreakableStatic> m_NearbyStatics = new List<UOACZBreakableStatic>();
            
            IPooledEnumerable nearbyItems = player.Map.GetItemsInRange(player.Location, 1);

            foreach (Item item in nearbyItems)
            {
                if (item.Deleted)
                    continue;

                if (item is UOACZStockpile)
                {
                    player.SendMessage("That is not repairable.");
                    continue;
                }

                if (item is UOACZBreakableStatic)
                    m_NearbyStatics.Add(item as UOACZBreakableStatic);                
            }

            nearbyItems.Free();

            return m_NearbyStatics;
        }
        
        private class InternalTimer : Timer
        {
            UOACZRepairHammer m_RepairHammer;
            PlayerMobile m_Player;

            int m_RepairTicks = 0;

            public InternalTimer(UOACZRepairHammer repairHammer, PlayerMobile player): base(TimeSpan.Zero, UOACZRepairHammer.RepairActionTickDuration)
            {
                Priority = TimerPriority.TwoFiftyMS;

                m_RepairHammer = repairHammer;
                m_Player = player;
            }

            protected override void OnTick()
            {
                if (m_RepairHammer == null || m_Player == null)
                {
                    if (m_RepairHammer != null)
                        m_RepairHammer.m_Owner = null;

                    if (m_RepairHammer != null)
                        m_Player.EndAction(typeof(UOACZRepairHammer));

                    Stop();
                    

                    return;
                }

                if (m_RepairHammer.Deleted || m_Player.Deleted)
                {
                    m_RepairHammer.m_Owner = null;
                    m_Player.EndAction(typeof(UOACZRepairHammer));

                    Stop();
                    return;
                }

                List<UOACZBreakableStatic> m_NearbyBreakableStatics = GetNearbyBreakableStatics(m_Player);

                if (m_NearbyBreakableStatics.Count == 0)
                {
                    m_RepairHammer.m_Owner = null;
                    m_Player.EndAction(typeof(UOACZRepairHammer));

                    Stop();
                    return;
                }
                
                UOACZBreakableStatic randomBreakableStatic = m_NearbyBreakableStatics[Utility.RandomMinMax(0, m_NearbyBreakableStatics.Count - 1)];

                int repairableCount = 0;

                foreach (UOACZBreakableStatic breakableStatic in m_NearbyBreakableStatics)
                {
                    if (breakableStatic.CanRepair(m_Player, m_RepairHammer, 1.0, false))
                        repairableCount++;
                }

                if (repairableCount == 0)
                {
                    m_RepairHammer.m_Owner = null;
                    m_Player.EndAction(typeof(UOACZRepairHammer));

                    m_Player.SendMessage("You stop making repairs.");

                    Stop();
                    return;
                }

                if (m_RepairTicks == 0)
                {
                    m_Player.BeginAction(typeof(BreakableStatic));
                    
                    TimeSpan repairCooldown = TimeSpan.FromSeconds(RepairActionTickDuration.TotalSeconds * (double)RepairActionTicksNeeded);

                    Timer.DelayCall(repairCooldown, delegate
                    {
                        if (m_Player != null)
                            m_Player.EndAction(typeof(BreakableStatic));
                    });                    
                }

                m_RepairTicks++;

                if (randomBreakableStatic.RepairSound != -1)
                    Effects.PlaySound(m_Player.Location, m_Player.Map, randomBreakableStatic.RepairSound);

                m_Player.Animate(12, 5, 1, true, false, 0);
                m_Player.RevealingAction();

                if (m_RepairTicks >= UOACZRepairHammer.RepairActionTicksNeeded)
                {
                    m_RepairTicks = 0;

                    int minRepairAmount = 40;
                    int maxRepairAmount = 60;

                    double baseRepairScalarBonus = 1.0;

                    double carpentryScalar = 1 + (baseRepairScalarBonus * m_Player.Skills.Carpentry.Value / 100);
                    double blacksmithingScalar = 1 + (baseRepairScalarBonus * m_Player.Skills.Carpentry.Value / 100);
                    double tinkeringScalar = 1 + (baseRepairScalarBonus * m_Player.Skills.Carpentry.Value / 100);

                    double bestScalar = 1;

                    if (carpentryScalar > bestScalar)
                        bestScalar = carpentryScalar;

                    if (blacksmithingScalar > bestScalar)
                        bestScalar = blacksmithingScalar;

                    if (tinkeringScalar > bestScalar)
                        bestScalar = tinkeringScalar;

                    double repairValue = m_Player.GetSpecialAbilityEntryValue(SpecialAbilityEffect.EmergencyRepairs);

                    bestScalar += repairValue;  
                  
                    bool outpostWasRepaired = false;

                    foreach (UOACZBreakableStatic breakableStatic in m_NearbyBreakableStatics)
                    {
                        int repairAmount = Utility.RandomMinMax(minRepairAmount, maxRepairAmount);
                        repairAmount = (int)(Math.Round(((double)repairAmount * bestScalar)));

                        if (breakableStatic.RequiresFullRepair)
                        {
                            BreakableStatic.DamageStateType damageState = breakableStatic.DamageState;

                            breakableStatic.HitPoints += repairAmount;
                            breakableStatic.PublicOverheadMessage(MessageType.Regular, UOACZSystem.greenTextHue, false, "+" + repairAmount);

                            if (breakableStatic.HitPoints < breakableStatic.MaxHitPoints)
                                breakableStatic.DamageState = damageState;

                            else
                                breakableStatic.DamageState = BreakableStatic.DamageStateType.Normal;
                        }

                        else
                        {
                            breakableStatic.HitPoints += repairAmount;
                            breakableStatic.PublicOverheadMessage(MessageType.Regular, UOACZSystem.greenTextHue, false, "+" + repairAmount);
                        }

                        UOACZPersistance.CheckAndCreateUOACZAccountEntry(m_Player);
                        m_Player.m_UOACZAccountEntry.TotalRepairAmount += repairAmount;

                        m_Player.SendMessage("You repair an object for " + repairAmount.ToString() + " hitpoints.");

                        if (UOACZPersistance.m_OutpostComponents.Contains(breakableStatic))
                        {
                            outpostWasRepaired = true;
                            UOACZEvents.RepairOutpostComponent();
                        }
                    }
                    
                    UOACZPersistance.CheckAndCreateUOACZAccountEntry(m_Player);
                    m_Player.m_UOACZAccountEntry.TimesRepaired++; 
                    
                    bool scored = false;

                    double scoreChance = UOACZSystem.HumanRepairScoreChance;

                    if (outpostWasRepaired)
                        scoreChance += UOACZSystem.HumanOutpostRepairScoreChance;

                    if (Utility.RandomDouble() <= UOACZSystem.HumanRepairScoreChance)
                    {
                        UOACZSystem.ChangeStat(m_Player, UOACZSystem.UOACZStatType.HumanScore, 1, true);
                        scored = true;
                    }

                    if (m_Player.Backpack != null)
                    {
                        if (Utility.RandomDouble() <= UOACZSystem.HumanRepairSurvivalStoneChance * UOACZPersistance.HumanBalanceScalar)
                        {                        
                            m_Player.Backpack.DropItem(new UOACZSurvivalStone(m_Player));
                            m_Player.SendMessage(UOACZSystem.greenTextHue, "You have earned a survival stone for your repair efforts!");
                        }

                        if (Utility.RandomDouble() <= UOACZSystem.HumanRepairUpgradeTokenChance * UOACZPersistance.HumanBalanceScalar)
                        {
                            m_Player.Backpack.DropItem(new UOACZHumanUpgradeToken(m_Player));
                            m_Player.SendMessage(UOACZSystem.greenTextHue, "You have earned an upgrade token for your repair efforts!");
                        }
                    }

                    m_RepairHammer.Charges--;

                    if (m_RepairHammer.Charges <= 0)
                        m_RepairHammer.Delete();
                }                 
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version    
      
            //Version 0
            writer.Write(m_Charges);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_Charges = reader.ReadInt();
            }
        }
    }
}