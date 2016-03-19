using System;
using Server;
using Server.Network;
using Server.Prompts;
using Server.Multis;
using Server.Mobiles;
using Server.Targeting;
using Server.Gumps;
using System.Collections.Generic;

namespace Server.Items
{
    [FlipableAttribute(0x14F8, 0x14FA)]
    public class BoardingRope : Item
    {
        public int m_CurrentCharges;

        public double m_MinimumHullPercent = 0.75; //Hull Percent Must Be This or Lower Before Valid Boarding Attempts
        public double m_FullSuccessChancePercent = 0.5; //At 0 Hull Health, Chance of Boarding Success (Scales upwards with MinimumHullPercent)
        public double m_NPCShipBonusMultiplier = 3.0; //Bonus Multiplier to Board an NPC-Controlled Ship
        
        public static TimeSpan boardingCooldown = TimeSpan.FromSeconds(10);

        [Constructable]
        public BoardingRope(): base(0x14F8)
        {
            Weight = 2.0;
            m_CurrentCharges = 10;
        }

        public BoardingRope(Serial serial): base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int CurrentCharges
        {
            get
            {
                return m_CurrentCharges;
            }

            set
            {
                m_CurrentCharges = value;

                if (m_CurrentCharges == 0)                
                    Delete();                
            }
        }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, "Durability: " + m_CurrentCharges.ToString());
            LabelTo(from, "a boarding rope");           
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
            
            else if (from.NextBoardingAttemptAllowed > DateTime.UtcNow)
            {
                int secondsCooldown = (int)(Math.Ceiling((from.NextBoardingAttemptAllowed - DateTime.UtcNow).TotalSeconds));
                from.SendMessage("You cannot attempt to board a ship for another " + secondsCooldown.ToString() + " seconds.");

                return;
            }

            from.SendMessage("Which ship would you like to board?");
            from.RevealingAction();
            from.Target = new ShipBoardingTarget(this);

            base.OnDoubleClick(from);
        }

        private class ShipBoardingTarget : Target
        {
            private BoardingRope m_BoardingRope;

            public ShipBoardingTarget(BoardingRope b): base(4, true, TargetFlags.Harmful, false)
            {
                m_BoardingRope = b;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                IPoint3D p = o as IPoint3D;

                if (p != null)
                {
                    if (p is Item)
                    {
                        //If Targeted a Cannon on a Ship: Override Location to Ship Location For Later Lookup
                        Custom.Pirates.BaseCannon cannon = p as Custom.Pirates.BaseCannon;

                        if (cannon != null)
                        {
                            if (cannon.m_Boat != null)
                            {
                                BaseBoat boat = cannon.m_Boat as BaseBoat;

                                if (boat != null)                                
                                    p = boat.Location;                                
                            }
                        }

                        else                       
                            p = ((Item)p).GetWorldTop();                                         
                    }

                    else if (p is Mobile)                    
                        p = ((Mobile)p).Location;                    

                    m_BoardingRope.OnTarget(from, new Point3D(p.X, p.Y, p.Z));
                }
            }
        }

        public void OnTarget(Mobile from, Point3D point)
        {
            BaseBoat fromBoat = BaseBoat.FindBoatAt(from.Location, from.Map);
            BaseBoat targetBoat = BaseBoat.FindBoatAt(point, this.Map);

            if (targetBoat != null)
            {
                bool automaticBoarding = false;

                if (targetBoat.MobileControlType != MobileControlType.Player && !targetBoat.HasCrewAlive())
                    automaticBoarding = true;

                double shipHullPercent = (double)((float)targetBoat.HitPoints / (float)targetBoat.MaxHitPoints);                
                                
                if (targetBoat.IsOwner(from) || targetBoat.IsCoOwner(from) || targetBoat.IsFriend(from))
                {
                    from.SendMessage("You already have access to that boat.");
                    return;
                }

                else if (shipHullPercent > m_MinimumHullPercent && !automaticBoarding)
                {
                    from.SendMessage("The hull of that ship is not yet damaged enough to risk a boarding attempt.");
                    return;
                }

                else
                {                    
                    double baseSuccessChance = m_FullSuccessChancePercent;
                    double hullPercentFactor = (m_MinimumHullPercent - shipHullPercent) * (1 / m_MinimumHullPercent);

                    if (hullPercentFactor < 0)                    
                        hullPercentFactor = 0;                    

                    double finalSuccessChance = hullPercentFactor * baseSuccessChance;

                    //Active Ability: Boarding Hooks
                    if (fromBoat != null)
                    {
                        if (fromBoat.IsFriend(from) || fromBoat.IsCoOwner(from) || fromBoat.IsOwner(from))
                        {
                            if (fromBoat.m_ActiveAbility == ActiveAbilityType.BoardingHooks && fromBoat.m_ActiveAbilityExpiration > DateTime.UtcNow)
                                finalSuccessChance *= (1 + BaseBoat.BoardingHooksBonus);
                        }
                    }

                    if (targetBoat.MobileControlType != MobileControlType.Player);
                        finalSuccessChance *= m_NPCShipBonusMultiplier;
                    
                    //Successful Boarding
                    if ((Utility.RandomDouble() <= finalSuccessChance) || automaticBoarding)
                    {
                        //Valid Boarding
                        if (targetBoat.Embark(from, true))
                        {     
                            List<Mobile> targets = targetBoat.GetMobilesOnBoat(false, false);

                            //Mobile Boarding Boat Becomes Aggressor to Everyone on Target Boat (Who Belongs on the Boat)
                            foreach (Mobile target in targets)
                            {
                                //Target Belongs on Boat
                                if (targetBoat.Crew.Contains(target) || targetBoat.IsFriend(target) || targetBoat.IsCoOwner(target) || targetBoat.IsOwner(target))
                                {
                                    //Do Harmful Action to Target
                                    if (from.CanBeHarmful(target, false))
                                        from.DoHarmful(target);
                                }
                            }                              
                            
                            from.SendMessage("You board the ship!");
                            Effects.PlaySound(from.Location, from.Map, 0x056); //Alternate Sound 0x0EF

                            if (targetBoat.TillerMan != null)
                            {
                                if (targetBoat.HasCrewAlive())
                                    targetBoat.TillerMan.Say("Arr! We've been boarded!");
                            }

                            from.NextBoardingAttemptAllowed =  DateTime.UtcNow + boardingCooldown;
                            CurrentCharges--;

                            return;
                        }

                        else
                        {
                            from.NextBoardingAttemptAllowed = DateTime.UtcNow + boardingCooldown;
                            from.SendMessage("You fail to board the ship");
                            Effects.PlaySound(from.Location, from.Map, 0x5D2);
                            CurrentCharges--;

                            return;
                        }
                    }

                    //Failed Boarding Attempt
                    else
                    {
                        from.NextBoardingAttemptAllowed = DateTime.UtcNow + boardingCooldown;
                        from.SendMessage("You fail to board the ship");
                        Effects.PlaySound(from.Location, from.Map, 0x5D2);
                        CurrentCharges--;

                        return;
                    }
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); //version
            writer.Write((int)m_CurrentCharges);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {                        
                        m_CurrentCharges = reader.ReadInt();
                    }
                break;
            }
        } 
    }
}