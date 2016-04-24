using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Spells;

namespace Server.Items
{
    public class UOACZOilFlask : Item
    {
        public static TimeSpan usageCooldown = TimeSpan.FromSeconds(10);
        public static int ThrowRange = 10;

        [Constructable]
        public UOACZOilFlask(): base(7192)
        {
            Name = "an oil flask";
            Weight = 1;
        }

        public UOACZOilFlask(Serial serial): base(serial)
        {
        }
        
        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            PlayerMobile player = from as PlayerMobile;

            if (!UOACZSystem.IsUOACZValidMobile(player)) return;
            if (!player.IsUOACZHuman) return;                

            if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("That item must be in your pack in order to use it.");
                return;
            }

            if (!from.CanBeginAction(typeof(UOACZOilFlask)))
            {
                from.SendMessage("You must wait a few moments before using another oil flask.");
                return;
            }

            player.SendMessage("Target a location to create an oil surface at.");
            player.Target = new OilFlaskTarget(this);            
        }

        public class OilFlaskTarget : Target
        {
            private UOACZOilFlask m_OilFlask;
            private IEntity targetLocation;

            public OilFlaskTarget(UOACZOilFlask oilFlask): base(25, true, TargetFlags.None)
            {
                m_OilFlask = oilFlask;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile player = from as PlayerMobile;

                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!player.IsUOACZHuman) return;

                if (m_OilFlask == null) return;
                if (m_OilFlask.Deleted) return;

                if (!m_OilFlask.IsChildOf(from.Backpack))
                {
                    from.SendMessage("The oil flask is no longer in your pack.");
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

                if (Utility.GetDistance(player.Location, targetLocation.Location) > ThrowRange)
                {
                    player.SendMessage("That location is too far away.");
                    return;
                }

                if (!player.Map.InLOS(player.Location, targetLocation.Location))
                {
                    player.SendMessage("That is not within in your line of sight.");
                    return;
                }

                player.RevealingAction();

                SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1.0, 1, true, 0, false, "", "", "-1");

                m_OilFlask.Delete();

                player.Animate(31, 7, 1, true, false, 0);

                Timer.DelayCall(usageCooldown, delegate
                {
                    if (player != null)
                        player.EndAction(typeof(UOACZOilFlask));                      
                });

                int throwSound = 0x5D3;
                int hitSound = Utility.RandomList(0x38E, 0x38F, 0x390);
                int effectSound = 0x5D8;
                int itemID = 7192;
                int itemHue = 0;

                int radius = 2;
                double additionalOilChance = .5;                

                Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (!player.IsUOACZHuman) return;

                    Effects.PlaySound(player.Location, player.Map, throwSound);

                    IEntity startLocation = new Entity(Serial.Zero, new Point3D(player.Location.X, player.Location.Y, player.Location.Z + 5), player.Map);
                    IEntity endLocation = new Entity(Serial.Zero, new Point3D(targetLocation.Location.X, targetLocation.Location.Y, targetLocation.Location.Z + 5), targetLocation.Map);

                    Effects.SendMovingEffect(startLocation, endLocation, itemID, 10, 0, false, false, itemHue, 0); //5

                    double distance = player.GetDistanceToSqrt(endLocation.Location);
                    double destinationDelay = (double)distance * .06; //.08

                    Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                    {
                        if (!UOACZPersistance.Active) return;

                        Effects.PlaySound(endLocation.Location, endLocation.Map, hitSound);
                        
                        for (int a = -1 * radius; a < radius + 1; a++)
                        {
                            for (int b = -1 * radius; b < radius + 1; b++)
                            {
                                Point3D newPoint = new Point3D(endLocation.Location.X + a, endLocation.Location.Y + b, endLocation.Location.Z);
                                SpellHelper.AdjustField(ref newPoint, map, 12, false);

                                if (!map.InLOS(endLocation.Location, newPoint))
                                    continue;

                                double distanceFromCenter = Utility.GetDistanceToSqrt(endLocation.Location, newPoint);

                                bool validLocation = true;

                                double extraOilChance = 1;
                                
                                if (distanceFromCenter >= 1)
                                    extraOilChance = (1 / (distanceFromCenter)) * additionalOilChance;

                                if (Utility.RandomDouble() <= extraOilChance)
                                {
                                    Timer.DelayCall(TimeSpan.FromSeconds(distanceFromCenter * .1), delegate
                                    {
                                        if (!UOACZPersistance.Active)
                                            return;

                                        bool foundAnother = false;

                                        IPooledEnumerable nearbyItems = map.GetItemsInRange(newPoint, 0);

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
                    });
                });
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
    }
}