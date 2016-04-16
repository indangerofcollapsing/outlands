using System;
using Server;
using System.Collections.Generic;
using Server.Network;
using Server.Prompts;
using Server.Multis;
using Server.Mobiles;
using Server.Targeting;
using Server.Gumps;
using Server.Custom;

namespace Server.Items
{
    public class CannonShot : Item
    {
        [Constructable]
        public CannonShot(): this(1)
        {
        }

        [Constructable]
        public CannonShot(int amount): base(0x0E73)
        {
            Stackable = true;
            Weight = 0.1;
            Amount = amount;
        }

        public CannonShot(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            if (Amount > 1)
                LabelTo(from, "cannon shot : " + Amount.ToString());

            else
                LabelTo(from, "cannon shot");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (RootParent != from)
            {
                from.SendMessage("This item must be in your pack in order to use it");
                return;
            }

            else if (!from.Alive)
            {
                from.SendMessage("You cannot use this while dead.");
                return;
            }

            from.SendMessage("Which cannons do you wish to reload?");
            from.Target = new CannonShotReloadTarget(this);
            from.RevealingAction();

            base.OnDoubleClick(from);
        }

        private class CannonShotReloadTarget : Target
        {
            private CannonShot m_CannonShot;

            public CannonShotReloadTarget(CannonShot b)
                : base(2, true, TargetFlags.None, false)
            {
                m_CannonShot = b;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                IPoint3D p = o as IPoint3D;

                if (p != null)
                {
                    if (p is Item)
                    {
                        Item item = p as Item;

                        if (item is Server.Custom.Pirates.BaseCannon)
                        {
                            Custom.Pirates.BaseCannon cannon = item as Custom.Pirates.BaseCannon;

                            if (cannon != null)
                                m_CannonShot.OnTarget(from, cannon);

                            else
                                from.SendMessage("That is not a cannon.");
                        }

                        else if (item is LandCannon)
                        {
                            LandCannon landCannon = item as LandCannon;

                            if (from.AccessLevel < landCannon.UsageAccessLevel)
                            {
                                from.SendMessage("You are not allowed to reload that.");
                                return;
                            }

                            if (landCannon.Charges == landCannon.MaxCharges)
                            {
                                from.SendMessage("That does not need reloading.");
                                return;
                            }

                            int chargedNeeded = landCannon.MaxCharges - landCannon.Charges;
                            int cannonShotUsed = 0;

                            if (chargedNeeded >= m_CannonShot.Amount)
                                cannonShotUsed = m_CannonShot.Amount;
                            else                            
                                cannonShotUsed = chargedNeeded;

                            m_CannonShot.Amount -= cannonShotUsed;
                            landCannon.Charges += cannonShotUsed;

                            TimeSpan reloadTime = TimeSpan.FromSeconds(landCannon.ReloadDelay * cannonShotUsed);

                            if (landCannon.NextUsageAllowed > DateTime.UtcNow)
                                landCannon.NextUsageAllowed = landCannon.NextUsageAllowed + reloadTime;

                            else
                                landCannon.NextUsageAllowed = DateTime.UtcNow + reloadTime;

                            Effects.PlaySound(landCannon.Location, landCannon.Map, 0x3e4);
                            landCannon.PublicOverheadMessage(MessageType.Regular, 0, false, "*reloading*");

                            Timer.DelayCall(reloadTime, delegate
                            {
                                if (landCannon == null) return;
                                if (landCannon.Deleted) return;

                                landCannon.PublicOverheadMessage(MessageType.Regular, 0, false, "*reloaded*");
                            }); 

                            if (m_CannonShot.Amount == 0)
                                m_CannonShot.Delete();
                        }

                        else
                            from.SendMessage("That is not a cannon.");
                    }
                }
            }
        }

        public void OnTarget(Mobile from, Custom.Pirates.BaseCannon cannon)
        {
            if (from == null)
                return;

            if (cannon == null)
                return;

            BaseBoat boat = cannon.m_Boat as BaseBoat;

            if (boat != null)
            {
                if (!(boat.IsOwner(from) || boat.IsCoOwner(from)))
                {
                    from.SendMessage("You don't have permission to reload those.");
                    return;
                }

                bool reloadedAny = false;

                int totalReloadTime = 0;

                foreach (Custom.Pirates.BaseCannon shipCannon in boat.Cannons)
                {
                    //Cannon Is Part of Volley
                    if (shipCannon.Facing == cannon.Facing)
                    {
                        //Needs Reloading
                        if (shipCannon.CurrentCharges < Custom.Pirates.BaseCannon.MaxCharges)
                        {
                            int chargesNeeded = Custom.Pirates.BaseCannon.MaxCharges - shipCannon.CurrentCharges;

                            if (chargesNeeded > 0 && this.Amount > 0)
                            {
                                if (this.Amount >= chargesNeeded)
                                {
                                    shipCannon.CurrentCharges += chargesNeeded;
                                    this.Amount -= chargesNeeded;

                                    reloadedAny = true;

                                    totalReloadTime += shipCannon.ReloadTime;
                                }

                                else
                                {
                                    shipCannon.CurrentCharges += this.Amount;
                                    this.Amount = 0;

                                    reloadedAny = true;

                                    totalReloadTime += shipCannon.ReloadTime;
                                }
                            }
                        }
                    }
                }

                if (reloadedAny)
                {
                    if (boat.TillerMan != null)
                        boat.TillerMan.Say("Reloading the cannons!");

                    Effects.PlaySound(from.Location, from.Map, 0x3e4);

                    from.SendMessage("You load the cannon shot into the cannons.");                                  

                    double finalReloadTime = (double)totalReloadTime * boat.CannonReloadTimeScalar;

                    if (boat.CannonCooldown <= DateTime.UtcNow)
                        boat.CannonCooldown = DateTime.UtcNow + TimeSpan.FromSeconds(finalReloadTime);

                    else
                        boat.CannonCooldown += TimeSpan.FromSeconds(finalReloadTime);

                    boat.StartCannonCooldown();
                }

                else
                    from.SendMessage("None of those cannons need reloading.");

                if (this.Amount == 0)
                    this.Delete();
            }

            else
            {
                //NPC Cannons: TEMP - Need Fixing
                from.SendMessage("TEMP: Non-ship Cannon");

                return;
            }
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

            Stackable = true;
        }
    }
}