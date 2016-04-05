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
    public class UOACZThrowingNet : Item
    {
        public double CooldownSeconds = 60;

        [Constructable]
        public UOACZThrowingNet(): base(3530)
        {
            Name = "a throwing net";
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;                        

            if (!IsChildOf(player.Backpack))
            {
                player.SendMessage("That item must be in your pack in order to use it.");
                return;
            }

            if (!player.CanBeginAction(typeof(UOACZThrowingNet)))
            {
                player.SendMessage("You may only use a net once every " + CooldownSeconds.ToString() + " seconds.");
                return;
            }

            from.SendMessage("Target the player or creature to target.");
            from.Target = new UOACZNetTarget(this);            
        }

        public class UOACZNetTarget : Target
        {
            private UOACZThrowingNet m_Net;
            private IEntity targetLocation;

            public UOACZNetTarget(UOACZThrowingNet net): base(25, false, TargetFlags.Harmful, false)
            {
                m_Net = net;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                if (from == null || m_Net == null) return;
                if (from.Deleted || !from.Alive || m_Net.Deleted || m_Net.RootParent != from) return;

                PlayerMobile player = from as PlayerMobile;

                if (player == null)
                    return;

                if (!m_Net.IsChildOf(player.Backpack))
                {
                    player.SendMessage("That item must be in your pack in order to use it.");
                    return;
                }

                if (!player.CanBeginAction(typeof(UOACZThrowingNet)))
                {
                    player.SendMessage("You may only use a net once every " + m_Net.CooldownSeconds.ToString() + " seconds.");
                    return;
                }

                UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!player.IsUOACZHuman) return;

                Mobile mobileTarget = null;

                if (target is Mobile)                
                    mobileTarget = target as Mobile;                

                else
                {
                    player.SendMessage("That is not a valid target.");
                    return;
                }

                if (mobileTarget == player)
                {
                    player.SendMessage("You cannot target yourself.");
                    return;
                }

                if (!UOACZSystem.IsUOACZValidMobile(mobileTarget))
                {
                    player.SendMessage("That is not a valid target.");
                    return;
                }

                if (Utility.GetDistance(player.Location, mobileTarget.Location) > 8)
                {
                    from.SendMessage("That is too far away");
                    return;
                }

                if (!player.Map.InLOS(player.Location, mobileTarget.Location))
                {
                    player.SendMessage("That is not within in your line of sight.");
                    return;
                }

                SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1, 1, true, 0, false, "", "");

                if (!from.Mounted)
                    player.Animate(31, 7, 1, true, false, 0);                                                           

                double duration = 60;

                if (mobileTarget is UOACZBaseWildlife)
                    duration = 60;

                if (mobileTarget is UOACZBaseUndead)
                {
                    UOACZBaseUndead bc_Undead = mobileTarget as UOACZBaseUndead;

                    double reduction = 1 - (bc_Undead.Difficulty * .10);

                    if (reduction < .10)
                        reduction = .10;

                    duration *= reduction;
                }

                if (mobileTarget is UOACZBaseHuman)
                {
                    UOACZBaseCivilian bc_Human = mobileTarget as UOACZBaseCivilian;

                    double reduction = 1 - (bc_Human.Difficulty * .10);

                    if (reduction < .10)
                        reduction = .10;

                    duration *= reduction;
                }

                PlayerMobile playerTarget = mobileTarget as PlayerMobile;

                if (playerTarget != null)
                {
                    if (playerTarget.IsUOACZHuman)
                        duration = Utility.RandomMinMax(2, 4);

                    if (playerTarget.IsUOACZUndead)
                        duration = Utility.RandomMinMax(5, 10);
                }

                UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);
                player.m_UOACZAccountEntry.NetsThrown++;                 

                int itemId = m_Net.ItemID;
                int itemHue = m_Net.Hue;

                Effects.PlaySound(player.Location, player.Map, 0x5D3);

                player.DoHarmful(mobileTarget);                

                Point3D playerLocation = player.Location;
                Map playerMap = player.Map;

                Point3D mobileLocation = mobileTarget.Location;
                Map mobileMap = mobileTarget.Map;

                IEntity startLocation = new Entity(Serial.Zero, new Point3D(playerLocation.X, playerLocation.Y, playerLocation.Z + 7), playerMap);
                IEntity endLocation = new Entity(Serial.Zero, new Point3D(mobileTarget.X, mobileTarget.Y, mobileTarget.Z), mobileMap);

                double distance = from.GetDistanceToSqrt(endLocation);
                double destinationDelay = (double)distance * .08;

                m_Net.Delete();

                Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(mobileTarget)) return;
                    if (Utility.GetDistance(playerLocation, mobileLocation) >= 25) return;
                        
                    SpecialAbilities.HinderSpecialAbility(1.0, player, mobileTarget, 1, duration, true, -1, true, "You ensnare them in a net", "You have been ensnared in a net!");
                    
                    for (int a = 0; a < 9; a++)
                    {
                        TimedStatic net = new TimedStatic(3538, duration - destinationDelay);
                        Point3D netLocation = mobileTarget.Location;
                            
                        switch (a)
                        {
                            //Row 1
                            case 0:
                                net.ItemID = 3538;
                                netLocation = new Point3D(netLocation.X - 1, netLocation.Y - 1, netLocation.Z);
                            break;

                            case 1:
                                net.ItemID = 3528;
                                netLocation = new Point3D(netLocation.X, netLocation.Y - 1, netLocation.Z);
                            break;

                            case 2:
                                net.ItemID = 3537;
                                netLocation = new Point3D(netLocation.X + 1, netLocation.Y - 1, netLocation.Z);
                            break;

                            //Row 2
                            case 3:
                                net.ItemID = 3539;
                                netLocation = new Point3D(netLocation.X - 1, netLocation.Y, netLocation.Z);
                            break;

                            case 4:
                                net.ItemID = 3530;
                                netLocation = new Point3D(netLocation.X, netLocation.Y, netLocation.Z);
                            break;

                            case 5:
                                net.ItemID = 3531;
                                netLocation = new Point3D(netLocation.X + 1, netLocation.Y, netLocation.Z);
                            break;

                            //Row 3
                            case 6:
                                net.ItemID = 3540;
                                netLocation = new Point3D(netLocation.X - 1, netLocation.Y + 1, netLocation.Z);
                            break;

                            case 7:
                                net.ItemID = 3529;
                                netLocation = new Point3D(netLocation.X, netLocation.Y + 1, netLocation.Z);
                            break;

                            case 8:
                                net.ItemID = 3541;
                                netLocation = new Point3D(netLocation.X + 1, netLocation.Y + 1, netLocation.Z);
                            break;
                        }

                        net.Hue = itemHue;
                        net.Name = "a net";
                        net.MoveToWorld(netLocation, mobileMap);
                    }  
                });
                
            }
        }

        public UOACZThrowingNet(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}