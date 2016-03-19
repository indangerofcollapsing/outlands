using System;
using Server;
using System.Collections.Generic;
using Server.Network;
using Server.Prompts;
using Server.Multis;
using Server.Mobiles;
using Server.Items;
using Server.Targeting;
using Server.Spells;
using Server.SkillHandlers;

namespace Server.Custom
{
    public class SpidersilkFishingNet : Item
    {
        public int m_MaxCharges;

        private int m_Charges = Utility.RandomMinMax(100, 200);
        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges
        {
            get { return m_Charges; }
            set { m_Charges = value; }
        }

        public int HueColor = 1154;
        
        [Constructable]
        public SpidersilkFishingNet(): base(3530)
        {
            Name = "a spidersilk fishing net";
            Hue = HueColor; //2498 Old

            m_MaxCharges = m_Charges;
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            LabelTo(from, "[uses remaining: " + m_Charges + "]");
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

            if (from.Skills.Fishing.Value < 100)
            {
                from.SendMessage("You must at least be a grandmaster fisherman to properly utilize this.");
                return;
            }

            if (player.BoatOccupied == null)
            {
                from.SendMessage("You must be onboard a boat in order to use that.");
                return;
            }

            if (!from.CanBeginAction(typeof(Hiding)))
            {
                from.SendMessage("You must wait a few moments to use another skill.");
                return;
            }

            from.SendMessage("Target the area where you wish to place this or the stack which you wish to combine this with.");
            from.Target = new SpidersilkNetTarget(this);            
        }

        public class SpidersilkNetTarget : Target
        {
            private SpidersilkFishingNet m_SpidersilkNet;
            private IEntity targetLocation;

            public SpidersilkNetTarget(SpidersilkFishingNet spidersilkNet): base(6, true, TargetFlags.None)
            {
                m_SpidersilkNet = spidersilkNet;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                if (from == null || m_SpidersilkNet == null) return;
                if (from.Deleted || !from.Alive || m_SpidersilkNet.Deleted || m_SpidersilkNet.RootParent != from) return;

                PlayerMobile player = from as PlayerMobile;

                if (player == null)
                    return;

                if (player.BoatOccupied == null)
                {
                    from.SendMessage("You must stay onboard your boat to continue your fishing action.");                    
                    return;
                }

                else if (player.BoatOccupied.Deleted)
                {
                    from.SendMessage("The boat you were fishing from no longer exists.");
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

                bool isWaterTile = BaseBoat.IsWaterTile(targetLocation.Location, map);

                if (!isWaterTile)
                {
                    from.SendMessage("That is not a valid location for that.");
                    return;
                }

                else
                {
                    FishingSpot fishingSpot = null;

                    IPooledEnumerable itemsNearTarget = map.GetItemsInRange(targetLocation.Location, 6);

                    foreach (Item item in itemsNearTarget)
                    {
                        if (item is FishingSpot)
                        {
                            fishingSpot = item as FishingSpot;
                            break;
                        }
                    }

                    itemsNearTarget.Free();

                    if (fishingSpot != null)
                    {
                        if (!fishingSpot.Deleted)
                        {
                            m_SpidersilkNet.ThrowNet(from, targetLocation.Location, fishingSpot);

                            return;
                        }             
                    }
                    
                    from.SendMessage("That is not a valid location for that.");
                    return;
                }
            }
        }

        public void ThrowNet(Mobile from, Point3D location, FishingSpot fishingSpot)
        {
            if (fishingSpot.FishingActionsRemaining > 0)
            {
                if (from.BeginAction((typeof(SpidersilkFishingNet))))
                {
                    from.RevealingAction();

                    Timer.DelayCall(TimeSpan.FromSeconds(6), delegate 
                    {
                        if (from != null)
                            from.EndAction(typeof(SpidersilkFishingNet));
                    });

                    int itemId = ItemID;
                    int itemHue = Hue - 1;

                    int throwSound = 0x5D3;

                    Point3D targetLocation = location;
                    Map map = from.Map;

                    m_Charges--;

                    if (m_Charges <= 0)
                        Delete();

                    if (from.Body.IsHuman && !from.Mounted)
                        from.Animate(31, 7, 1, true, false, 0);                    

                    Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate
                    {
                        if (!SpecialAbilities.Exists(from)) return;
                        if (!from.Alive) return;   

                        Effects.PlaySound(from.Location, map, throwSound);                        

                        SpellHelper.AdjustField(ref targetLocation, map, 12, false);                        

                        IEntity startLocation = new Entity(Serial.Zero, new Point3D(from.Location.X, from.Location.Y, from.Location.Z + 7), map);
                        IEntity endLocation = new Entity(Serial.Zero, new Point3D(targetLocation.X, targetLocation.Y, targetLocation.Z), map);

                        Effects.SendMovingEffect(startLocation, endLocation, itemId, 5, 0, false, false, itemHue, 0);

                        double distance = from.GetDistanceToSqrt(targetLocation);
                        double destinationDelay = (double)distance * .08;

                        Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                        {
                            Effects.PlaySound(targetLocation, map, 0x364);  //0x148
                            Effects.SendLocationEffect(endLocation, map, 0x352D, 7);

                            Timer.DelayCall(TimeSpan.FromSeconds(.25), delegate
                            {
                                for (int a = 0; a < 9; a++)
                                {
                                    Blood net = new Blood();   
                                    Point3D netLocation = targetLocation;                                                                                                                                                                                                                

                                    #region NetLayout
                                    switch (a)
                                    {
                                        //Row 1
                                        case 0:
                                            net.ItemID = 3538;
                                            netLocation = new Point3D(targetLocation.X - 1, targetLocation.Y - 1, targetLocation.Z);
                                        break;

                                        case 1:
                                            net.ItemID = 3528;
                                            netLocation = new Point3D(targetLocation.X, targetLocation.Y - 1, targetLocation.Z);
                                        break;

                                        case 2:
                                            net.ItemID = 3537;
                                            netLocation = new Point3D(targetLocation.X + 1, targetLocation.Y - 1, targetLocation.Z);
                                        break;

                                        //Row 2
                                        case 3:
                                            net.ItemID = 3539;
                                            netLocation = new Point3D(targetLocation.X - 1, targetLocation.Y, targetLocation.Z);
                                        break;

                                        case 4:
                                            net.ItemID = 3530;
                                            netLocation = new Point3D(targetLocation.X, targetLocation.Y, targetLocation.Z);
                                        break;

                                        case 5:
                                             net.ItemID = 3531;
                                            netLocation = new Point3D(targetLocation.X + 1, targetLocation.Y, targetLocation.Z);
                                        break;

                                        //Row 3
                                        case 6:
                                            net.ItemID = 3540;
                                            netLocation = new Point3D(targetLocation.X - 1, targetLocation.Y + 1, targetLocation.Z);
                                        break;

                                        case 7:
                                            net.ItemID = 3529;
                                            netLocation = new Point3D(targetLocation.X, targetLocation.Y + 1, targetLocation.Z);
                                        break;

                                        case 8:
                                            net.ItemID = 3541;
                                            netLocation = new Point3D(targetLocation.X + 1, targetLocation.Y + 1, targetLocation.Z);
                                        break;
                                    }

                                    #endregion

                                    net.Hue = Hue;
                                    net.Name = "a spidersilk net";
                                    net.MoveToWorld(netLocation, map);
                                }                           

                                Timer.DelayCall(TimeSpan.FromSeconds(5.5), delegate
                                {
                                    if (from == null) return;
                                    if (from.Deleted) return;
                                    if (!from.Alive)
                                    {
                                        from.SendMessage("You died before you were able to finish your fishing action.");
                                        return;
                                    }

                                    if (Utility.GetDistanceToSqrt(from.Location, targetLocation) > 10)
                                    {
                                        from.SendMessage("You have moved too far away from your fishing spot to continue fishing.");
                                        return;
                                    }

                                    else
                                    {
                                        bool fishingSpotValid = true;

                                        if (fishingSpot == null)                                    
                                            fishingSpotValid = false;                                            
                                    
                                        else if (fishingSpot.Deleted)
                                            fishingSpotValid = false;   

                                        if (fishingSpotValid)
                                        {
                                            fishingSpot.FishingAction(from, targetLocation, map);
                                            return;
                                        }

                                        else
                                        {
                                            from.SendMessage("That fishing spot has recently been exhausted.");
                                            return;
                                        }                                       
                                    }
                                });
                            });
                        });                       
                    });
                }

                else
                {
                    from.SendMessage("You must wait a moment before performing another action.");
                    return;
                }
            }

            else
            {
                from.SendMessage("The water there appears to be exhausted.");
                return;
            }
        }

        public SpidersilkFishingNet(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            writer.Write(m_Charges);
            writer.Write(m_MaxCharges);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Charges = reader.ReadInt();
            m_MaxCharges = reader.ReadInt();

            //------

            Hue = HueColor;
        }
    }
}