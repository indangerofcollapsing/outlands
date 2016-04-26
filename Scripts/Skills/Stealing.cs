using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Targeting;
using Server.Items;
using Server.Network;

using Server.Spells.Seventh;
using Server.Spells.Fifth;
using Server.Spells;

namespace Server.SkillHandlers
{
    public class Stealing
    {
        public static void Initialize()
        {
            SkillInfo.Table[(int)SkillName.Stealing].Callback = new SkillUseCallback(OnUse);
        }

        public static readonly bool ClassicMode = true;
        public static readonly bool SuspendOnMurder = true;

        public static double MaxWeightStealable = 10.0;        
        public static double StackStealScalar = .5;
        public static double StackedMaxWeightVariance = .10;

        public static int NoticeRange = 8;

        public static double SkillSuccessChanceScalar = .75;
        public static double HidingBonus = .25;
        public static double CombatPenalty = .25;

        public static double PlayerNoticeChance = .20;
        public static double MobileNoticeChance = .10;
        public static double LOSBlockedCatchScalar = .5;

        public static bool IsInGuild(Mobile m)
        {
            return (m is PlayerMobile && ((PlayerMobile)m).NpcGuild == NpcGuild.ThievesGuild);
        }

        public static bool IsInnocentTo(Mobile from, Mobile to)
        {
            return (Notoriety.Compute(from, (Mobile)to) == Notoriety.Innocent);
        }

        private class StealingTarget : Target
        {
            private Mobile from;

            public StealingTarget(Mobile thief): base(2, false, TargetFlags.None)
            {
                from = thief;

                AllowNonlocal = true;
            }

            private Item TryStealItem(Item item, ref int amount)
            {
                Item stolen = null;

                object root = item.RootParent;

                if (!IsEmptyHanded(from))                
                    from.SendLocalizedMessage(1005584); // Both hands must be free to steal.
                                
                else if (root is Mobile && ((Mobile)root).Player && !IsInGuild(from))                
                    from.SendLocalizedMessage(1005596); // You must be in the thieves guild to steal from other players.                

                else if (SuspendOnMurder && root is Mobile && ((Mobile)root).Player && IsInGuild(from) && from.ShortTermMurders > 0)                
                    from.SendLocalizedMessage(502706); // You are currently suspended from the thieves guild.                

                else if (root is BaseVendor && ((BaseVendor)root).IsInvulnerable)                
                    from.SendLocalizedMessage(1005598); // You can't steal from shopkeepers.                

                else if (root is PlayerVendor)                
                    from.SendLocalizedMessage(502709); // You can't steal from vendors.
                
                else if (!from.CanSee(item))                
                    from.SendLocalizedMessage(500237); // Target can not be seen.
                
                else if (from.Backpack == null || !from.Backpack.CheckHold(from, item, false, true))                
                    from.SendLocalizedMessage(1048147); // Your backpack can't hold anything else.                

                else if (!from.InRange(item.GetWorldLocation(), 1))                
                    from.SendLocalizedMessage(502703); // You must be standing next to an item to steal it.
                
                else if (item.Parent is Mobile)                
                    from.SendLocalizedMessage(1005585); // You cannot steal items which are equiped.                

                else if (root == from)                
                    from.SendLocalizedMessage(502704); // You catch yourself red-handed.                

                else if (root is Mobile && ((Mobile)root).AccessLevel > AccessLevel.Player)                
                    from.SendLocalizedMessage(502710); // You can't steal that!                

                else if (root is Mobile && !from.CanBeHarmful((Mobile)root))
                {
                }

                else if (root is Corpse)                
                    from.SendLocalizedMessage(502710); // You can't steal that!                

                else if (item.Stealable && !item.AlreadyStolen)
                {
                    //TEST: Check This (Rares Stealing?) 
                    if (from.CheckTargetSkill(SkillName.Stealing, item, item.MinimumStealing, item.MaximumStealing, 1.0))
                    {
                        item.AlreadyStolen = true;
                        stolen = item;
                    }

                    else                    
                        from.SendLocalizedMessage(502723); // You fail to steal the item.                    
                }

                else if (!item.Movable || item.LootType == LootType.Newbied || item.CheckBlessed(root) || item.DonationItem)                
                    from.SendLocalizedMessage(502710); // You can't steal that!
                
                else
                {
                    double itemWeight = item.Weight + item.TotalWeight;

                    if (itemWeight > MaxWeightStealable)                    
                        from.SendMessage("That is too heavy to steal.");
                    
                    else
                    {
                        from.CheckTargetSkill(SkillName.Stealing, item, 0.0, 100.0, 1.0);

                        double stealingSkill = from.Skills[SkillName.Stealing].Value / 100;
                        double successChance = stealingSkill * SkillSuccessChanceScalar;

                        if (from.Hidden)
                        {
                            double hidingBonus = HidingBonus;

                            if (hidingBonus > stealingSkill)
                                hidingBonus = stealingSkill;

                            successChance += hidingBonus;
                        }

                        if (from.LastCombatTime + TimeSpan.FromSeconds(30) > DateTime.UtcNow)
                            successChance -= CombatPenalty;
                        
                        bool successful = (Utility.RandomDouble() <= successChance);
                        
                        double maxWeightStealable = (from.Skills[SkillName.Stealing].Value / 100) * MaxWeightStealable;

                        if (item.Stackable && item.Amount > 1)
                        {
                            double weightVariation = maxWeightStealable * StackedMaxWeightVariance;                            
                            
                            if (Utility.RandomDouble() < .5)
                                maxWeightStealable += (Utility.RandomDouble() * weightVariation);

                            else
                                maxWeightStealable -= (Utility.RandomDouble() * weightVariation);

                            maxWeightStealable *= StackStealScalar;

                            amount = (int)(Math.Round(maxWeightStealable / (double)item.Weight));

                            if (amount < 1)
                                amount = 1;

                            if (successful)
                                stolen = item;
                        }

                        else
                        {
                            if (successful)
                                stolen = item;
                        }
                    }
                }

                return stolen;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                from.RevealingAction();

                Item stolenItem = null;
                object root = null;

                Mobile mobileTarget = root as Mobile;

                bool turnPermaGrey = false;
                bool caught = false;

                int amountStolen = 0;         

                //Stealing Attempt
                if (target is Item)
                {
                    root = ((Item)target).RootParent;
                    stolenItem = TryStealItem((Item)target, ref amountStolen);
                }

                else if (target is Mobile)
                {
                    Container pack = ((Mobile)target).Backpack;

                    if (pack != null && pack.Items.Count > 0)
                    {
                        int randomIndex = Utility.Random(pack.Items.Count);

                        root = target;
                        stolenItem = TryStealItem(pack.Items[randomIndex], ref amountStolen);
                    }
                }

                else                
                    from.SendLocalizedMessage(502710); // You can't steal that!
                
                //Successful Steal Attempt
                if (stolenItem != null)
                {
                    //See if Nearby Mobiles Notice
                    IPooledEnumerable nearbyMobiles = from.Map.GetMobilesInRange(from.Location, NoticeRange);

                    foreach (Mobile mobile in nearbyMobiles)
                    {
                        double noticeChance = 0;

                        if (mobile == from) continue;
                        if (!mobile.Alive) continue;

                        if (mobile is PlayerMobile)
                        {
                            if (from.Map.InLOS(from.Location, mobile.Location))
                                noticeChance += PlayerNoticeChance;

                            else
                                noticeChance += PlayerNoticeChance * LOSBlockedCatchScalar;

                            if (Utility.RandomDouble() <= noticeChance && mobileTarget != null)
                            {
                                mobile.SendMessage("You notice " + from.Name + " trying to steal from " + mobileTarget.Name + ".");

                                caught = true;
                            }

                            continue;
                        }

                        BaseCreature bc_Creature = mobile as BaseCreature;

                        if (bc_Creature != null && mobile.Body.IsHuman)
                        {
                            if (from.Map.InLOS(from.Location, mobile.Location))
                                noticeChance += MobileNoticeChance;

                            else
                                noticeChance += MobileNoticeChance * LOSBlockedCatchScalar;

                            if (Utility.RandomDouble() <= noticeChance && mobileTarget != null)
                                caught = true;

                            continue;
                        }
                    }

                    nearbyMobiles.Free();

                    //Resolve 
                    if (caught)
                    {
                        if (root == null)
                            from.CriminalAction(false);

                        else if (root is Corpse && ((Corpse)root).IsCriminalAction(from))
                            from.CriminalAction(false);

                        else if (root is Mobile)
                        {
                            Mobile mobRoot = (Mobile)root;

                            if (!IsInGuild(mobRoot) && IsInnocentTo(from, mobRoot))
                                from.CriminalAction(false);
                        }
                    }

                    else if (root is Corpse && ((Corpse)root).IsCriminalAction(from))
                        from.CriminalAction(false);

                    //Flagging and Permagrey
                    if (root is Mobile && ((Mobile)root).Player && from is PlayerMobile && IsInnocentTo(from, (Mobile)root) && !IsInGuild((Mobile)root) && turnPermaGrey)
                    {
                        PlayerMobile pm = (PlayerMobile)from;

                        pm.PermaFlags.Add((Mobile)root);
                        pm.Delta(MobileDelta.Noto);
                    }

                    //Move Item: Delay to Allow for Guard Whacking Preventing Theft
                    Timer.DelayCall(TimeSpan.FromSeconds(.1), delegate
                    {
                        if (!SpecialAbilities.Exists(from)) return;
                        if (from.Backpack == null) return;
                        if (stolenItem == null) return;
                        if (stolenItem.Deleted) return;

                        if (stolenItem.Stackable && stolenItem.Amount > 1)
                        {
                            if (amountStolen >= stolenItem.Amount)
                            {
                                from.AddToBackpack(stolenItem);

                                if (!(stolenItem is Container || stolenItem.Stackable))
                                    StolenItem.Add(stolenItem, from, root as Mobile);

                                from.SendMessage("You successfully steal the item.");
                            }

                            else
                            {
                                Item newItem = Mobile.LiftItemDupe(stolenItem, stolenItem.Amount - amountStolen);

                                from.AddToBackpack(newItem);

                                if (!(newItem is Container || newItem.Stackable))
                                    StolenItem.Add(newItem, from, root as Mobile);

                                from.SendMessage("You successfully steal the item.");
                            }
                        }

                        else
                        {
                            from.AddToBackpack(stolenItem);

                            if (!(stolenItem is Container || stolenItem.Stackable))
                                StolenItem.Add(stolenItem, from, root as Mobile);

                            from.SendMessage("You successfully steal the item.");
                        }
                    });
                }

                else
                    from.SendMessage("You fail in your stealing attempt.");
            }
        }

        public static bool IsEmptyHanded(Mobile from)
        {
            if (from.FindItemOnLayer(Layer.OneHanded) != null)
                return false;

            if (from.FindItemOnLayer(Layer.TwoHanded) != null)
                return false;

            return true;
        }

        public static TimeSpan OnUse(Mobile m)
        {
            if (!IsEmptyHanded(m))            
                m.SendLocalizedMessage(1005584); // Both hands must be free to steal.            

            else
            {
                m.Target = new Stealing.StealingTarget(m);
                m.RevealingAction();

                m.SendLocalizedMessage(502698); // Which item do you want to steal?
            }

            return TimeSpan.FromSeconds(SkillCooldown.StealingCooldown);
        }
    }

    public class StolenItem
    {
        public static readonly TimeSpan StealTime = TimeSpan.FromMinutes(2.0);

        private Item m_Stolen;
        private Mobile m_Thief;
        private Mobile m_Victim;
        private DateTime m_Expires;

        public Item Stolen { get { return m_Stolen; } }
        public Mobile Thief { get { return m_Thief; } }
        public Mobile Victim { get { return m_Victim; } }
        public DateTime Expires { get { return m_Expires; } }

        public bool IsExpired { get { return (DateTime.UtcNow >= m_Expires); } }

        public StolenItem(Item stolen, Mobile thief, Mobile victim)
        {
            m_Stolen = stolen;
            m_Thief = thief;
            m_Victim = victim;

            m_Expires = DateTime.UtcNow + StealTime;
        }

        private static Queue<StolenItem> m_Queue = new Queue<StolenItem>();

        public static void Add(Item item, Mobile thief, Mobile victim)
        {
            Clean();

            m_Queue.Enqueue(new StolenItem(item, thief, victim));
        }

        public static bool IsStolen(Item item)
        {
            Mobile victim = null;

            return IsStolen(item, ref victim);
        }

        public static bool IsStolen(Item item, ref Mobile victim)
        {
            Clean();

            foreach (StolenItem si in m_Queue)
            {
                if (si.m_Stolen == item && !si.IsExpired)
                {
                    victim = si.m_Victim;
                    return true;
                }
            }

            return false;
        }

        public static void ReturnOnDeath(Mobile killed, Container corpse)
        {
            Clean();

            foreach (StolenItem si in m_Queue)
            {
                if (si.m_Stolen.RootParent == corpse && si.m_Victim != null && !si.IsExpired)
                {
                    if (si.m_Victim.AddToBackpack(si.m_Stolen))
                        si.m_Victim.SendLocalizedMessage(1010464); // the item that was stolen is returned to you.

                    else
                        si.m_Victim.SendLocalizedMessage(1010463); // the item that was stolen from you falls to the ground.

                    si.m_Expires = DateTime.UtcNow; // such a hack
                }
            }
        }

        public static void Clean()
        {
            while (m_Queue.Count > 0)
            {
                StolenItem si = m_Queue.Peek();

                if (si.IsExpired)
                    m_Queue.Dequeue();

                else
                    break;
            }
        }
    }
}