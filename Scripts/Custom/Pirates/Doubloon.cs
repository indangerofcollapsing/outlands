using System;
using Server.Multis;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Items
{
    public class Doubloon : Item
    {
        public override double DefaultWeight
        {
            get { return 0.02; ; }
        }

        [Constructable]
        public Doubloon(): this(1)
        {
        }

        [Constructable]
        public Doubloon(int amountFrom, int amountTo): this(Utility.RandomMinMax(amountFrom, amountTo))
        {
        }

        [Constructable]
        public Doubloon(int amount): base(2539)
        {
            Stackable = true;
            Amount = amount;
            Hue = 2125;
            Name = "doubloon";

            PlayerClass = Server.PlayerClass.Pirate;
            PlayerClassRestricted = true;

            Server.Custom.CurrencyTracking.RegisterDoubloons(amount);
        }

        public Doubloon(Serial serial): base(serial)
        {
        }

        public override bool StackWith(Mobile from, Item dropped, bool playSound)
        {
            if (dropped.PlayerClassOwner != PlayerClassOwner)
                return false;

            return base.StackWith(from, dropped, playSound);
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            PlayerClassPersistance.PlayerClassSingleClick(this, from);
        }

        public override int GetDropSound()
        {
            if (Amount <= 1)
                return 0x2E4;
            else if (Amount <= 5)
                return 0x2E5;
            else
                return 0x2E6;
        }

        public override bool OnDragLift(Mobile from)
        {
            if (ParentEntity is Hold)
            {
                if (from.AccessLevel >= AccessLevel.GameMaster)
                {
                    from.SendMessage("Your godly powers allows you to move the doubloons.");
                    return true;
                }

                else
                {
                    from.SendMessage("You cannot remove doubloons from the hold of a ship. Docking the ship will transfer the doubloons to your bank.");
                    return false;
                }
            }

            return true;
        }

        private class DoubloonTransferTarget : Target
        {
            private Doubloon m_Doubloon;

            public DoubloonTransferTarget(Doubloon doubloon): base(50, true, TargetFlags.None, false)
            {
                m_Doubloon = doubloon;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                IPoint3D p = o as IPoint3D;

                if (p != null)
                {
                    if (p is Item)
                        p = ((Item)p).GetWorldTop();
                    else if (p is Mobile)
                        p = ((Mobile)p).Location;

                    m_Doubloon.OnTarget(from, new Point3D(p.X, p.Y, p.Z));
                }
            }
        }

        public void OnTarget(Mobile from, Point3D point)
        {
            BaseBoat boatFrom = BaseBoat.FindBoatAt(from.Location, this.Map);
            BaseBoat boatTo = BaseBoat.FindBoatAt(point, this.Map);

            double distance = 1000;

            if (boatFrom == null)
            {
                from.SendMessage("You must be onboard this ship in order to transfer its doubloons.");
                return;
            }

            if (boatTo != null)
            {
                distance = boatFrom.GetBoatToBoatDistance(boatFrom, boatTo);

                if (!(distance >= 0 && distance <= 6))
                {
                    from.SendMessage("The targeted ship is not close enough to transfer the doubloons.");
                    return;
                }

                if (boatTo.IsOwner(from) || boatTo.IsCoOwner(from))
                {
                    if (boatTo.Hold == null)
                    {
                        from.SendMessage("That ship does not have a hold to transfer doubloons to.");
                        return;
                    }

                    if (boatTo.Hold.Deleted)
                    {
                        from.SendMessage("That ship does not have a hold to transfer doubloons to.");
                        return;
                    }

                    int doubloonsFrom = boatFrom.GetHoldDoubloonTotal(boatFrom);

                    int deposited = 0;                   

                    return;
                }

                else
                {
                    from.SendMessage("You must be the owner or co-owner of the ship in order to transfer doubloons to that ship.");
                    return;
                }
            }

            else
            {
                from.SendMessage("");
                return;
            }
        }

        public override void OnAdded(object parent)
        {
            if (parent is Hold)
                Weight = 0.0;

            base.OnAdded(parent);
        }

        protected override void OnAmountChange(int oldValue)
        {
            int newValue = this.Amount;

            UpdateTotal(this, TotalType.Gold, newValue - oldValue);

            Server.Custom.CurrencyTracking.RegisterDoubloons(newValue - oldValue);
        }

        public override void OnDelete()
        {
            Server.Custom.CurrencyTracking.DeleteDoubloons(this.Amount);
            base.OnDelete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            Server.Custom.CurrencyTracking.RegisterDoubloons(this.Amount);
        }
    }
}