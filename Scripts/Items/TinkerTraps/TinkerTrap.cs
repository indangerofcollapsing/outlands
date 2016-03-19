using System;
using Server.Network;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.Targeting;
using Server.Spells;
using Server.Gumps;
using Server.Multis;
using Server.Achievements;

namespace Server.Custom
{
    public class TinkerTrap : Item
    {
        public virtual string TrapLabel { get { return "A Tinker Trap"; } }
        public virtual string TrapName { get { return "a tinker trap"; } }
        public virtual int TrapItemId { get { return 6173; } }
        public virtual int TrapHue { get { return 2500; } }
        public virtual int TrapTextHue { get { return 149; } }

        public virtual int TriggerRadius { get { return 1; } }
        public virtual int EffectRadius { get { return 1; } }
        public virtual bool SingleLine { get { return false; } }

        public int MinimumRangeFromOtherTrap = 5;

        public double BaseSetupTime = 5;
        public double BaseExpirationSeconds = 60;
        public double ExpirationMaxSkillModifier = 5;

        public TimeSpan TrapWindowDuration = TimeSpan.FromMinutes(10);
        
        [Constructable]
        public TinkerTrap(): base(6173)
        {
            Name = TrapName;
            ItemID = TrapItemId;
            Hue = TrapHue;

            Weight = 5;
        }

        public TinkerTrap(Serial serial): base(serial)
        {
        }

        public virtual void Resolve(TinkerTrapPlaced tinkerTrapPlaced)
        {
            if (tinkerTrapPlaced != null)
                tinkerTrapPlaced.Delete();

            Delete();
        }

        public override void OnSingleClick(Mobile from)
        {
            if (from.NetState != null)
                LabelTo(from, TrapName);
   
            if (Amount > 1)
                LabelTo(from, "amount: " + Amount.ToString());
        }

        public override void OnDoubleClick(Mobile from)
        {
            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

            if (!IsChildOf(from.Backpack))
                from.SendMessage("That item must be in your pack in order to use it.");

            else
            {               
                int maxTrapsAllowed = 1;

                if (from.Skills.RemoveTrap.Value >= 25)
                    maxTrapsAllowed++;

                if (from.Skills.RemoveTrap.Value >= 50)
                    maxTrapsAllowed++;

                if (from.Skills.RemoveTrap.Value >= 75)
                    maxTrapsAllowed++;

                if (from.Skills.RemoveTrap.Value >= 100)
                    maxTrapsAllowed++;

                if (from.Skills.RemoveTrap.Value >= 120)
                    maxTrapsAllowed++;

                if (maxTrapsAllowed <= player.TinkerTrapsPlaced && from.AccessLevel == AccessLevel.Player && !(from.Region is UOACZRegion))
                {
                    if (DateTime.UtcNow < player.TinkerTrapPlacementWindow)
                    {
                        string sTimeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, player.TinkerTrapPlacementWindow, false, true, true, true, true);

                        from.SendMessage("You must wait another " + sTimeRemaining + " before you may place another trap.");
                        return;
                    }
                }                

                from.SendMessage("Target the area where you wish to place this or the stack which you wish to combine this with.");
                from.Target = new TinkerTrapTarget(this);

                return;                
            }
        }

        public class TinkerTrapTarget : Target
        {
            private TinkerTrap m_TinkerTrap;
            private IEntity targetLocation;

            public TinkerTrapTarget(TinkerTrap tinkerTrap): base(3, true, TargetFlags.None)
            {
                m_TinkerTrap = tinkerTrap;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                if (m_TinkerTrap.Deleted || m_TinkerTrap.RootParent != from)
                    return;

                PlayerMobile pm = from as PlayerMobile;

                if (pm == null)
                    return;

                TinkerTrap targetTinkerTrap = target as TinkerTrap;

                if (targetTinkerTrap != null && targetTinkerTrap != m_TinkerTrap)
                {
                    if (!targetTinkerTrap.IsChildOf(from.Backpack))
                    {
                        from.SendMessage("The stack you wish to combine this with must be in your pack");
                        return;
                    }
                    
                    targetTinkerTrap.Amount += m_TinkerTrap.Amount;

                    from.SendMessage("You add the trap to the stack.");
                    from.SendSound(0x3e4);

                    m_TinkerTrap.Delete();

                    return; 
                }

                IPoint3D location = target as IPoint3D;

                if (location == null)
                    return;

                Map map = from.Map;

                if (map == null)
                    return;

                SpellHelper.GetSurfaceTop(ref location);

                if (location is Mobile)
                    targetLocation = (Mobile)location;

                else
                    targetLocation = new Entity(Serial.Zero, new Point3D(location), map);

                if (!map.CanSpawnMobile(targetLocation.Location) && from.AccessLevel == AccessLevel.Player)
                {
                    from.SendLocalizedMessage(501942); // That location is blocked.
                    return;
                }

                if (BaseBoat.FindBoatAt(targetLocation.Location, map) != null)
                {
                    from.SendMessage("You cannot place those on a boat.");
                    return;
                }

                if (BaseBoat.IsWaterTile(targetLocation.Location, map))
                {
                    from.SendMessage("You may only place those on dry land.");
                    return;
                }

                bool foundNearbyTrap = false;

                IPooledEnumerable itemsOnTile = map.GetItemsInRange(targetLocation.Location, m_TinkerTrap.MinimumRangeFromOtherTrap);

                foreach (Item item in itemsOnTile)
                {
                    if (item is TinkerTrapPlaced)
                    {
                        foundNearbyTrap = true;
                        break;
                    }
                }

                itemsOnTile.Free();

                if (foundNearbyTrap && from.AccessLevel == AccessLevel.Player)
                {
                    from.SendMessage("That location is too close to an existing trap.");
                    return;
                }

                from.SendGump(new TinkerTrapGump(m_TinkerTrap, from, targetLocation.Location, from.Map));       
            }
        }        

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        public static bool IsValidMobileTarget(Mobile mobile)
        {
            bool validMobile = true;

            if (mobile == null)
                return false;

            if (mobile.Deleted)
                return false;

            if (!mobile.CanBeDamaged() || !mobile.Alive || mobile.AccessLevel > AccessLevel.Player)
                validMobile = false;

            PlayerMobile pm_Target = mobile as PlayerMobile;
            BaseCreature bc_Target = mobile as BaseCreature;
            
            if (UOACZSystem.IsUOACZValidMobile(mobile))
            {
                if (mobile is UOACZBaseWildlife)
                    return true;

                if (mobile is UOACZBaseUndead)
                    return true;

                if (pm_Target != null)
                {
                    if (pm_Target.IsUOACZUndead)
                        return true;
                }

                return false;
            }            

            if (pm_Target != null)
                return false;

            if (bc_Target != null)
            {
                if (bc_Target.Controlled && bc_Target.ControlMaster is PlayerMobile)
                    return false;
            }

            return validMobile;
        }

        public void AttemptPlaceTrap(Mobile from, Point3D point, Map map, double delay)
        {
            if (from == null)  return;
            if (from.Deleted) return;
            if (!from.Alive) return;

            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

            if (Deleted || RootParent != from)
            {
                from.SendMessage("The trap you were attempting to place is no longer available.");
                return;
            }

            if (from.Map != map || !from.InRange(point, 3))
            {
                from.SendMessage("You are too far away from the target location to place the trap.");
                return;
            }

            IEntity targetLocation;
            IPoint3D location = point as IPoint3D;

            SpellHelper.GetSurfaceTop(ref location);

            if (location is Mobile)
                targetLocation = (Mobile)location;

            else
                targetLocation = new Entity(Serial.Zero, new Point3D(location), map);

            if (!map.CanSpawnMobile(targetLocation.Location))
            {
                from.SendLocalizedMessage(501942); // That location is blocked.
                return;
            }

            bool foundNearbyTrap = false;

            IPooledEnumerable itemsOnTile = map.GetItemsInRange(targetLocation.Location, MinimumRangeFromOtherTrap);

            foreach (Item item in itemsOnTile)
            {
                if (item is TinkerTrapPlaced)
                {
                    foundNearbyTrap = true;
                    break;
                }
            }

            itemsOnTile.Free();

            if (foundNearbyTrap)
            {
                from.SendMessage("That location is too close to an existing trap.");
                return;
            }

            from.RevealingAction();

            SpecialAbilities.HinderSpecialAbility(1.0, null, from, 1.0, 5, true, 0x3E3, false, "", "You begin placing the trap.");
                
            if (from.Body.IsHuman && !from.Mounted)
                from.Animate(32, 3, 1, true, false, 0);

            for (int a = 1; a < 6; a++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(a), delegate
                {
                    if (from == null) return;
                    if (from.Deleted) return;
                    if (!from.Alive) return;

                    from.RevealingAction();

                    if (from.Body.IsHuman && !from.Mounted)
                        from.Animate(32, 3, 1, true, false, 0);
                });
            }            

            int maxTrapsAllowed = 1;

            if (from.Skills.RemoveTrap.Value >= 25)
                maxTrapsAllowed++;

            if (from.Skills.RemoveTrap.Value >= 50)
                maxTrapsAllowed++;

            if (from.Skills.RemoveTrap.Value >= 75)
                maxTrapsAllowed++;

            if (from.Skills.RemoveTrap.Value >= 100)
                maxTrapsAllowed++;

            if (from.Skills.RemoveTrap.Value >= 120)
                maxTrapsAllowed++;

            if (DateTime.UtcNow < player.TinkerTrapPlacementWindow)            
                player.TinkerTrapsPlaced++;            

            else
            {
                player.TinkerTrapPlacementWindow = DateTime.UtcNow + TrapWindowDuration;
                player.TinkerTrapsPlaced = 1;
            }

            int minutes;
            int seconds;

            string sTimeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, player.TinkerTrapPlacementWindow, false, true, true, true, true);
            
            int trapsRemaining = maxTrapsAllowed - player.TinkerTrapsPlaced;

            if (trapsRemaining > 0)
                from.SendMessage("Trap placed. You may place " + trapsRemaining.ToString() + " more traps in the next " + sTimeRemaining + ".");
            else
                from.SendMessage("Trap placed. You must wait " + sTimeRemaining + " before placing another trap."); 

            TinkerTrapPlaced trapPlaced = (TinkerTrapPlaced)Activator.CreateInstance(typeof(TinkerTrapPlaced));                

            trapPlaced.Name = TrapName;
            trapPlaced.Hue = TrapHue;

            trapPlaced.TrapName = TrapName;
            trapPlaced.TrapItemId = TrapItemId;
            trapPlaced.TrapHue = TrapHue;
            trapPlaced.TrapTextHue = TrapTextHue;
            trapPlaced.TriggerRadius = TriggerRadius;
            trapPlaced.EffectRadius = EffectRadius;

            trapPlaced.SingleLine = SingleLine;
            trapPlaced.LineFacing = from.GetDirectionTo(targetLocation.Location);

            trapPlaced.Owner = from;

            double adjustedSetupTime = BaseSetupTime;
            double adjustedDuration = BaseExpirationSeconds * (1 + ((ExpirationMaxSkillModifier - 1) * (from.Skills.RemoveTrap.Value / 120)));

            if (UOACZSystem.IsUOACZValidMobile(from))
            {
                adjustedDuration = UOACZSystem.TinkerTrapDuration;
                
                AchievementSystemImpl.Instance.TickProgressMulti(player, AchievementTriggers.Trigger_UOACZSnaresTrapsNets, 1);

                UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);
                player.m_UOACZAccountEntry.TrapsPlaced++; 
            }

            trapPlaced.TrapReady = DateTime.UtcNow + TimeSpan.FromSeconds(adjustedSetupTime);
            trapPlaced.Expiration = trapPlaced.TrapReady + TimeSpan.FromSeconds(adjustedDuration);
               
            trapPlaced.TriggerDelay = delay;
            trapPlaced.TriggerDelayActivation = DateTime.MaxValue;

            trapPlaced.m_TinkerTrap = this;

            trapPlaced.MoveToWorld(targetLocation.Location, map);
            trapPlaced.PublicOverheadMessage(MessageType.Emote, 0, false, "*begins placing trap*");

            int x = X;
            int y = Y;

            Type trapType = this.GetType();

            Amount--;

            int amountRemaining = Amount;

            Internalize();

            if (amountRemaining > 0)
            {
                TinkerTrap newTrap = (TinkerTrap)Activator.CreateInstance(trapType);

                newTrap.Amount = amountRemaining;

                if (player.Backpack != null)
                {
                    player.Backpack.DropItem(newTrap);

                    newTrap.X = x;
                    newTrap.Y = y;
                }                
            }            
        }
    }

    public class TinkerTrapGump : Gump
    {
        public TinkerTrap m_TinkerTrap;
        public Mobile m_From;
        public Point3D m_Location;
        public Map m_Map;

        public TinkerTrapGump(TinkerTrap tinkerTrap, Mobile from, Point3D location, Map map): base(10, 10)
        {
            if (tinkerTrap == null)  return;
            if (tinkerTrap.Deleted) return;
            if (from == null) return;
            if (from.Deleted) return;

            m_TinkerTrap = tinkerTrap;
            m_From = from;
            m_Location = location;
            m_Map = map;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddBackground(0, 0, 300, 300, 5054);
            AddBackground(10, 10, 280, 280, 3000);

            int textHue = 2036;
            int boldHue = 149;

            int textOffset = tinkerTrap.TrapLabel.Length * 2;

            AddLabel(130 - textOffset, 12, tinkerTrap.TrapTextHue, tinkerTrap.TrapLabel);
            AddItem(124, 35, tinkerTrap.TrapItemId, tinkerTrap.TrapHue);

            AddLabel(71, 85, boldHue, @"Select a Trigger Delay");

            AddButton(116, 110, 9721, 9722, 1, GumpButtonType.Reply, 0);
            AddLabel(152, 115, textHue, @"None");

            AddButton(25, 145, 2152, 2154, 2, GumpButtonType.Reply, 0);
            AddLabel(60, 150, textHue, ".25 seconds");

            AddButton(25, 180, 2152, 2154, 3, GumpButtonType.Reply, 0);
            AddLabel(60, 185, textHue, ".5 seconds");

            AddButton(25, 215, 2152, 2154, 4, GumpButtonType.Reply, 0);
            AddLabel(60, 220, textHue, ".75 seconds");

            AddButton(25, 250, 2152, 2154, 5, GumpButtonType.Reply, 0);
            AddLabel(60, 255, textHue, "1 second");

            AddButton(160, 146, 2152, 2154, 6, GumpButtonType.Reply, 0);
            AddLabel(195, 151, textHue, "1.5 seconds");

            AddButton(160, 181, 2152, 2154, 7, GumpButtonType.Reply, 0);
            AddLabel(195, 186, textHue, "2 seconds");

            AddButton(160, 216, 2152, 2154, 8, GumpButtonType.Reply, 0);
            AddLabel(195, 221, textHue, "3 seconds");

            AddButton(160, 251, 2152, 2154, 9, GumpButtonType.Reply, 0);
            AddLabel(195, 256, textHue, "5 seconds");
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (m_TinkerTrap == null) return;
            if (m_TinkerTrap.Deleted) return;
            if (from == null) return;
            if (from.Deleted) return;

            bool selectionMade = false;
            double triggerDelay = 0;

            switch (info.ButtonID)
            {
                case 1: triggerDelay = 0; selectionMade = true; break;

                case 2: triggerDelay = .25; selectionMade = true; break;
                case 3: triggerDelay = .5; selectionMade = true; break;
                case 4: triggerDelay = .75; selectionMade = true; break;
                case 5: triggerDelay = 1; selectionMade = true; break;

                case 6: triggerDelay = 1.5; selectionMade = true; break;
                case 7: triggerDelay = 2; selectionMade = true; break;
                case 8: triggerDelay = 3; selectionMade = true; break;
                case 9: triggerDelay = 5; selectionMade = true; break;
            }

            if (selectionMade)
                m_TinkerTrap.AttemptPlaceTrap(from, m_Location, m_Map, triggerDelay);             
        }
    }
}
