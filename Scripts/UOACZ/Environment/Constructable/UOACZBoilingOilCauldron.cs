using System;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.Targeting;
using Server.Spells;

namespace Server.Custom
{
    public class UOACZBoilingOilCauldron : UOACZConstructable
    {
        #region Properties

        public override int OverrideNormalItemId
        { 
            get 
            {
                if (Facing == Direction.North || Facing == Direction.South)
                    return 2420;

                else
                    return 2421;
            } 
        }
        public override int OverrideNormalHue { get { return 0; } }

        public override int OverrideLightlyDamagedItemId
        {
            get
            {
                if (Facing == Direction.North || Facing == Direction.South)
                    return 2420;

                else
                    return 2421;
            }
        }
        public override int OverrideLightlyDamagedHue { get { return 0; } }

        public override int OverrideHeavilyDamagedItemId
        {
            get
            {
                if (Facing == Direction.North || Facing == Direction.South)
                    return 2420;

                else
                    return 2421;
            }
        }

        public override int OverrideHeavilyDamagedHue { get { return 1154; } }

        public override int OverrideBrokenItemId
        {
            get
            {
                if (Facing == Direction.North || Facing == Direction.South)
                    return 2420;

                else
                    return 2421;
            }        
        }

        public override int OverrideBrokenHue { get { return 1154; } }
        
        private DateTime m_NextTimeUsable = DateTime.UtcNow;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextTimeUsable
        {
            get { return m_NextTimeUsable; }
            set { m_NextTimeUsable = value; }
        }

        public static TimeSpan Cooldown = TimeSpan.FromMinutes(5);

        #endregion

        [Constructable]
        public UOACZBoilingOilCauldron(): base()
        {
            Name = "boiling oil cauldron";
            
            MaxHitPoints = 3000;             
        }

        public UOACZBoilingOilCauldron(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            if (DamageState == DamageStateType.Broken || DamageState == DamageStateType.HeavilyDamaged)
                LabelTo(from, "(needs repairs)");

            else
            {
                if (m_NextTimeUsable <= DateTime.UtcNow)
                    LabelTo(from, "(ready)");
                else
                {
                    string cooldown = Utility.CreateTimeRemainingString(DateTime.UtcNow, m_NextTimeUsable, true, true, true, true, true);
                    LabelTo(from, "(usable in " + cooldown + ")");
                }
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            PlayerMobile player = from as PlayerMobile;

            if (!UOACZSystem.IsUOACZValidMobile(player)) return;
            if (!player.IsUOACZHuman) return;            

            if (DamageState == DamageStateType.Broken || DamageState == DamageStateType.HeavilyDamaged)
            {
                player.SendMessage("This must be repaired before it may be used.");
                return;
            }
            
            if (m_NextTimeUsable > DateTime.UtcNow)
            {
                string cooldownRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, m_NextTimeUsable, false, true, true, true, true);
                player.SendMessage("You must wait " + cooldownRemaining + " before that may be used again.");

                return;
            }

            if (Utility.GetDistance(player.Location, Location) > 1 || (Math.Abs(player.Location.Z - Location.Z) > 20))
            {
                player.SendMessage("You are too far away from that to use it.");
                return;
            }

            player.SendMessage("Target the location you wish to pour the cauldron onto.");
            player.Target = new CauldronTarget(this);            
        }

        public class CauldronTarget : Target
        {
            private UOACZBoilingOilCauldron m_Cauldron;
            private IEntity targetLocation;

            public CauldronTarget(UOACZBoilingOilCauldron cauldron): base(25, true, TargetFlags.None, false)
            {
                m_Cauldron = cauldron;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile player = from as PlayerMobile;

                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!player.IsUOACZHuman) return;

                if (m_Cauldron == null) return;
                if (m_Cauldron.Deleted) return;

                if (m_Cauldron.ConstructionTile == null) return;
                if (m_Cauldron.ConstructionTile.Deleted) return;

                if (m_Cauldron.DamageState == DamageStateType.Broken || m_Cauldron.DamageState == DamageStateType.HeavilyDamaged)
                {
                    player.SendMessage("This must be repaired before it may be used.");
                    return;
                }

                if (m_Cauldron.m_NextTimeUsable > DateTime.UtcNow)
                {
                    string cooldownRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, m_Cauldron.m_NextTimeUsable, false, true, true, true, true);
                    player.SendMessage("You must wait " + cooldownRemaining + " before that may be used again.");

                    return;
                }

                if (Utility.GetDistance(player.Location, m_Cauldron.Location) > 1 || (Math.Abs(player.Location.Z - m_Cauldron.Location.Z) > 20))
                {
                    player.SendMessage("You are too far away from the cauldron to use it.");
                    return;
                }

                IPoint3D location = target as IPoint3D;

                if (location == null)
                    return;

                Map map = player.Map;

                if (map == null)
                    return;

                SpellHelper.GetSurfaceTop(ref location);

                if (location is Mobile)
                    targetLocation = (Mobile)location;

                else
                    targetLocation = new Entity(Serial.Zero, new Point3D(location), map);

                bool foundTargeterMatch = false;

                IPooledEnumerable nearbyItems = map.GetItemsInRange(targetLocation.Location, 10);

                foreach (Item item in nearbyItems)
                {
                    if (item is UOACZConstructionObjectEffectTargeter)
                    {
                        UOACZConstructionObjectEffectTargeter targeter = item as UOACZConstructionObjectEffectTargeter;
                        
                        if (targeter.ConstructionTile != m_Cauldron.ConstructionTile)
                            continue;

                        if (Utility.GetDistance(targeter.Location, targetLocation.Location) <= targeter.Radius)
                        {
                            if (!map.InLOS(targetLocation.Location, targeter.Location))
                                continue;

                            foundTargeterMatch = true;
                            break;
                        }
                    }
                }

                nearbyItems.Free();

                if (!foundTargeterMatch || targetLocation.Location == m_Cauldron.Location)
                {
                    player.SendMessage("That location is not a valid target for this.");
                    return;
                }

                m_Cauldron.m_NextTimeUsable = DateTime.UtcNow + Cooldown;

                player.RevealingAction();

                int effectSound = 0x026;

                int radius = 3;
                double additionalOilChance = .95;

                for (int a = -1 * radius; a < radius + 1; a++)
                {
                    for (int b = -1 * radius; b < radius + 1; b++)
                    {
                        Point3D newPoint = new Point3D(targetLocation.Location.X + a, targetLocation.Location.Y + b, targetLocation.Location.Z);
                        SpellHelper.AdjustField(ref newPoint, map, 12, false);

                        if (!map.InLOS(targetLocation.Location, newPoint))
                            continue;

                        double distanceFromCenter = Utility.GetDistanceToSqrt(targetLocation.Location, newPoint);

                        bool validLocation = true;

                        double extraOilChance = 1;

                        if (distanceFromCenter >= 1)
                            extraOilChance = (1 / (distanceFromCenter)) * additionalOilChance;

                        if (Utility.RandomDouble() <= extraOilChance)
                        {
                            Timer.DelayCall(TimeSpan.FromSeconds(distanceFromCenter * .1), delegate
                            {
                                if (!UOACZPersistance.Active) return;

                                bool foundAnother = false;

                                nearbyItems = map.GetItemsInRange(newPoint, 0);

                                foreach (Item item in nearbyItems)
                                {
                                    if (item is UOACZOilLocation)
                                    {
                                        foundAnother = true;
                                        break;
                                    }
                                }

                                nearbyItems.Free();

                                if (!foundAnother)
                                {
                                    Effects.PlaySound(newPoint, map, effectSound);
                                    new UOACZOilLocation().MoveToWorld(newPoint, map);
                                }
                            });
                        }
                    }
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            //Version 0
            writer.Write(m_NextTimeUsable);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_NextTimeUsable = reader.ReadDateTime();
        }
    }
}
