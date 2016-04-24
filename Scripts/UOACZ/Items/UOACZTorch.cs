using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Spells;
using Server.Mobiles;
using System.Collections;
using System.Collections.Generic;

namespace Server.Items
{
    public class UOACZTorch : Torch
    {
        public static int ThrowRange = 10;

        public static double CooldownSeconds = 10;

        [Constructable]
        public UOACZTorch(): base()
        {
            Name = "a torch";

            Weight = 1;

            if (Burnout)
                Duration = TimeSpan.FromMinutes(30);
            else
                Duration = TimeSpan.Zero;

            Burning = false;
            Light = LightType.Circle300;
            Weight = 1.0;
        }

        public override void OnDoubleClick(Mobile from)
        {
            PlayerMobile player = from as PlayerMobile;

            if (!UOACZSystem.IsUOACZValidMobile(player)) return;
            if (!player.IsUOACZHuman) return;

            if (!player.CanBeginAction(typeof(UOACZTorch)))
            {
                player.SendMessage("You may only use a torch once every " + CooldownSeconds.ToString() + " seconds.");
                return;
            }
            
            player.SendMessage("Target a creature or location to throw the torch at.");
            player.Target = new TorchThrowTarget(this);            
        }

        public class TorchThrowTarget : Target
        {
            private UOACZTorch m_Torch;
            private IEntity targetLocation;

            public TorchThrowTarget(UOACZTorch torch): base(25, true, TargetFlags.None, false)
            {
                m_Torch = torch;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile player = from as PlayerMobile;

                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!player.IsUOACZHuman) return;

                if (m_Torch == null) return;
                if (m_Torch.Deleted) return;
               
                IPoint3D location = target as IPoint3D;

                if (location == null)
                    return;

                Map map = player.Map;

                if (map == null)
                    return;

                SpellHelper.GetSurfaceTop(ref location);                
                                
                Mobile mobileTarget = null;                

                targetLocation = new Entity(Serial.Zero, new Point3D(location), map);

                if (new Point3D(location) == from.Location)
                    return; 
               
                if (!player.CanBeginAction(typeof(UOACZTorch)))
                {
                    player.SendMessage("You must wait a few moments before throwing another torch.");
                    return;
                }

                if (Utility.GetDistance(player.Location, targetLocation.Location) > ThrowRange)
                {
                    player.SendMessage("That location is too far away.");
                    return;
                }
                
                player.RevealingAction();

                SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1.0, 1, true, 0, false, "", "", "-1");

                m_Torch.Delete();

                player.Animate(31, 7, 1, true, false, 0);                

                int throwSound = 0x5D3;
                int hitSound = 0x5CF;
                int itemID = 2578;
                int itemHue = 0;                     

                Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (!player.IsUOACZHuman) return;

                    player.BeginAction(typeof(UOACZTorch));

                    Timer.DelayCall(TimeSpan.FromSeconds(UOACZTorch.CooldownSeconds), delegate
                    {
                        if (player != null)
                            player.EndAction(typeof(UOACZTorch));
                    });

                    Effects.PlaySound(player.Location, player.Map, throwSound);

                    IEntity startLocation = new Entity(Serial.Zero, new Point3D(player.Location.X, player.Location.Y, player.Location.Z + 5), player.Map);
                    IEntity endLocation = new Entity(Serial.Zero, new Point3D(targetLocation.Location.X, targetLocation.Location.Y, targetLocation.Location.Z + 5), targetLocation.Map);

                    Effects.SendMovingEffect(startLocation, endLocation, itemID, 15, 0, false, false, itemHue, 0);

                    double distance = player.GetDistanceToSqrt(endLocation.Location);
                    double destinationDelay = (double)distance * .04;

                    Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                    {                        
                        Effects.PlaySound(endLocation.Location, endLocation.Map, hitSound);
                        Effects.SendLocationParticles(EffectItem.Create(endLocation.Location, endLocation.Map, TimeSpan.FromSeconds(1.0)), 0x3709, 10, 30, 0, 0, 5052, 0);

                        //Mobiles
                        Queue m_Queue = new Queue();

                        IPooledEnumerable nearbyMobiles = endLocation.Map.GetMobilesInRange(endLocation.Location, 0);

                        int mobilesHit = 0;

                        foreach (Mobile mobile in nearbyMobiles)
                        {
                            if (!UOACZSystem.IsUOACZValidMobile(mobile)) continue;

                            m_Queue.Enqueue(mobile);
                            mobilesHit++;
                        }

                        nearbyMobiles.Free();

                        while (m_Queue.Count > 0)
                        {
                            Mobile mobile = (Mobile)m_Queue.Dequeue();

                            BaseCreature bc_Target = mobile as BaseCreature;
                            PlayerMobile pm_Target = mobile as PlayerMobile;

                            int minDamage = 20;
                            int maxDamage = 40;

                            double damageScalar = 2;

                            if (pm_Target != null)
                            {
                                if (pm_Target.IsUOACZHuman)
                                    damageScalar = 0;

                                if (pm_Target.IsUOACZUndead)
                                    damageScalar = .66;
                            }

                            int damage = (int)(Math.Round(((double)Utility.RandomMinMax(minDamage, maxDamage)) * damageScalar));

                            bool humanSource = false;
                            
                            if (player != null)
                            {
                                if (player.IsUOACZHuman)
                                    humanSource = true;
                            }

                            if (damage > 0)
                            {
                                if (humanSource)
                                {
                                    player.DoHarmful(mobile);

                                    new Blood().MoveToWorld(mobile.Location, mobile.Map);
                                    AOS.Damage(mobile, player, damage, 100, 0, 0, 0, 0);
                                }

                                else
                                {
                                    new Blood().MoveToWorld(mobile.Location, mobile.Map);
                                    AOS.Damage(mobile, damage, 100, 0, 0, 0, 0);
                                }
                            }
                        }

                        if (mobilesHit > 0)
                        {
                            Effects.PlaySound(endLocation.Location, endLocation.Map, 0x054);
                            Effects.SendLocationParticles(EffectItem.Create(endLocation.Location, endLocation.Map, TimeSpan.FromSeconds(1.0)), 0x3709, 10, 30, 0, 0, 5052, 0);
                        }

                        bool oilLocationHit = false;

                        //Items
                        IPooledEnumerable nearbyItems = endLocation.Map.GetItemsInRange(endLocation.Location, 1);

                        foreach (Item item in nearbyItems)
                        {
                            if (item is UOACZOilLocation)
                                m_Queue.Enqueue(item);
                        }

                        nearbyItems.Free();

                        while (m_Queue.Count > 0)
                        {
                            UOACZOilLocation oilLocation = (UOACZOilLocation)m_Queue.Dequeue();

                            if (oilLocation == null) continue;
                            if (oilLocation.Deleted) continue;                            
                            if (oilLocation.Burning) continue;
                                
                            oilLocation.Ignite(player);
                            oilLocationHit = true;
                        }

                        if (!oilLocationHit)
                        {
                            Effects.PlaySound(endLocation.Location, endLocation.Map, 0x3BE);
                            Effects.SendLocationParticles(EffectItem.Create(endLocation.Location, endLocation.Map, TimeSpan.FromSeconds(1.0)), 0x3735, 10, 30, 0, 0, 5052, 0);
                        }                        
                    });
                });                
            }
        }

        public UOACZTorch(Serial serial): base(serial)
        {
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