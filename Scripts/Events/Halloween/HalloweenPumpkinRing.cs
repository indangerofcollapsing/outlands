using System;
using Server.Mobiles;
using Server.Targeting;
using Server.Spells;

namespace Server.Items
{
    public class HalloweenPumpkinRing : GoldRing
    {
        [Constructable]
        public HalloweenPumpkinRing()
        {
            Name = "Halloween Pumpkin Ring";
            Hue = 1360;
           
            LootType = Server.LootType.Blessed;

            IsArcaneDustRechargable = true;
            ArcaneDustBasedChargesRemaining = 50;
            ArcaneDustBasedChargesMaximumCharges = 50;
            ArcaneDustBasedChargesRegainedPerArcaneDust = 50;
        }

        public HalloweenPumpkinRing(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            LabelTo(from, "(" + ArcaneDustBasedChargesRemaining.ToString() + " charges)");
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

            if (ParentEntity != player)
            {
                player.SendMessage("This item must be equipped in order to be used.");
                return;
            }

            if (ArcaneDustBasedChargesRemaining == 0)
            {
                player.SendMessage("This item has run out of charges. Use arcane dust to add more charges.");
                return;
            }

            if (!player.CanBeginAction(typeof(HalloweenPumpkinRing)))
            {
                player.SendMessage("This item may only be used once every 5 seconds.");
                return;
            }

            player.BeginAction(typeof(HalloweenPumpkinRing));            

            Timer.DelayCall(TimeSpan.FromSeconds(5), delegate
            {
                if (player != null)
                    player.EndAction(typeof(HalloweenPumpkinRing));
            });

            ArcaneDustBasedChargesRemaining--;

            StartItemAction(player);
        }

        public void StartItemAction(PlayerMobile player)
        {
            player.SendMessage("Target a location.");
            player.Target = new ItemTarget(this);
        }

        public class ItemTarget : Target
        {
            private IEntity targetLocation;
            private HalloweenPumpkinRing m_Item;

            public ItemTarget(HalloweenPumpkinRing item): base(25, true, TargetFlags.None, false)
            {
                m_Item = item;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile player = from as PlayerMobile;

                if (player == null) return;
                if (player.Deleted || !player.Alive) return;
                if (m_Item == null) return;
                if (m_Item.Deleted) return;

                if (m_Item.ParentEntity != player)
                {
                    player.SendMessage("This item must be equipped in order to be used.");
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

                if (Utility.GetDistance(player.Location, targetLocation.Location) > 16)
                {
                    player.SendMessage("That target is too far away.");
                    return;
                }

                if (!player.Map.InLOS(player.Location, targetLocation.Location))
                {
                    player.SendMessage("That is not within in your line of sight.");
                    return;
                }

                int pumpkins = Utility.RandomMinMax(3, 5);

                double directionDelay = .25;
                double initialDelay = .5;
                double pumpkinDelay = .1;
                double totalDelay = 1 + directionDelay + initialDelay + ((double)pumpkins * pumpkinDelay);

                SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1.0, totalDelay, true, 0, false, "", "");

                player.Direction = Utility.GetDirection(player.Location, targetLocation.Location);

                Point3D initialLocation = player.Location;
                Map initialMap = player.Map;

                Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
                {                    
                    if (player == null) return;
                    if (player.Deleted || !player.Alive) return;

                    player.Animate(33, 5, 1, true, false, 0);                       

                    Timer.DelayCall(TimeSpan.FromSeconds(initialDelay), delegate
                    {
                        if (player == null) return;
                        if (player.Deleted || !player.Alive) return;

                        for (int a = 0; a < pumpkins; a++)
                        {
                            Timer.DelayCall(TimeSpan.FromSeconds(a * pumpkinDelay), delegate
                            {
                                if (player == null) return;
                                if (player.Deleted || !player.Alive) return;
                                if (Utility.GetDistance(initialLocation, targetLocation.Location) > 30) return;

                                int effectSound = Utility.RandomList(0x5D3, 0x5D2, 0x5A2, 0x580);
                                int itemID = Utility.RandomList(3178, 3179, 3180);
                                int itemHue = 0;

                                int impactSound = Utility.RandomList(0x5DE, 0x5DA, 0x5D8);
                                int impactHue = 0;

                                int xOffset = 0;
                                int yOffset = 0;

                                if (Utility.RandomDouble() <= .33)
                                    xOffset = Utility.RandomMinMax(-1, 1);

                                if (Utility.RandomDouble() <= .33)
                                    yOffset = Utility.RandomMinMax(-1, 1);

                                IEntity startLocation = new Entity(Serial.Zero, new Point3D(initialLocation.X, initialLocation.Y, initialLocation.Z + 10), initialMap);

                                Point3D adjustedLocation = new Point3D(targetLocation.X + xOffset, targetLocation.Y + yOffset, targetLocation.Z);
                                SpellHelper.AdjustField(ref adjustedLocation, initialMap, 12, false);

                                IEntity endLocation = new Entity(Serial.Zero, new Point3D(adjustedLocation.X, adjustedLocation.Y, adjustedLocation.Z + 10), initialMap);

                                Effects.PlaySound(location, map, effectSound);
                                Effects.SendMovingEffect(startLocation, endLocation, itemID, 4, 0, false, false, itemHue, 0);

                                double targetDistance = Utility.GetDistanceToSqrt(initialLocation, adjustedLocation);
                                double destinationDelay = (double)targetDistance * .08;

                                Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                                {
                                    Effects.PlaySound(adjustedLocation, initialMap, impactSound);

                                    TimedStatic pumpkinExplosion = new TimedStatic(Utility.RandomList(4650, 4651, 4653, 4655), 5);
                                    pumpkinExplosion.Name = "smashed pumpkin";
                                    pumpkinExplosion.Hue = 1359;
                                    pumpkinExplosion.MoveToWorld(adjustedLocation, initialMap);
                                });
                            });
                        }
                    });
                });
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); //Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }
}
