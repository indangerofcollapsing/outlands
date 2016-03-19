using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;

namespace Server.Items
{
    public class ParagonWand : Item
    {
        public static int MaxCharges;

        public static TimeSpan SpawnCooldown = TimeSpan.FromMinutes(5);
        public static TimeSpan CombatCooldown = TimeSpan.FromMinutes(1);

        public static TimeSpan UsageCooldown = TimeSpan.FromHours(1);

        private int m_Charges;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges
        {
            get { return m_Charges; }
            set { m_Charges = value;}
        }     

        [Constructable]
        public ParagonWand(): base(0x26BC)
        {
            Name = "a paragon wand";
            Hue = 2615; 

            Charges = Utility.RandomMinMax(1, 2); 
        }

        public ParagonWand(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, Name);
            LabelTo(from, "(charges: " + m_Charges.ToString() +")");
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("That item must be in your pack in order to use it.");
                return;
            }

            from.SendMessage("Target the creature you wish to bestow paragonhood upon.");
            from.Target = new ParagonConvertTarget(this);
        }

        public class ParagonConvertTarget : Target
        {
            private ParagonWand m_ParagonWand;

            public ParagonConvertTarget(ParagonWand ParagonWand): base(18, false, TargetFlags.None)
            {
                m_ParagonWand = ParagonWand;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                if (m_ParagonWand.Deleted || m_ParagonWand.RootParent != from)
                    return;

                PlayerMobile player = from as PlayerMobile;

                if (player == null)
                    return;

                BaseCreature bc_Target = target as BaseCreature;

                if (bc_Target == null)
                {
                    from.SendMessage("That is not a valid creature.");
                    return;
                }                

                if (!bc_Target.AllowParagon)
                {
                    from.SendMessage("That creature is not capable of becoming a paragon.");
                    return;
                }

                if (bc_Target.IsParagon)
                {
                    from.SendMessage("That creature is already a paragon.");
                    return;
                }

                if (bc_Target.Controlled)
                {
                    from.SendMessage("That creature is currently controlled.");
                    return;
                }

                if (player.AccessLevel == AccessLevel.Player)
                {

                    if (!bc_Target.Map.InLOS(from.Location, bc_Target.Location))
                    {
                        from.SendMessage("You must have a valid line-of-sight to the creature you wish to target.");
                        return;
                    }

                    if (bc_Target.Combatant != null)
                    {
                        from.SendMessage("You may not target creatures currently engaged in combat.");
                        return;
                    }

                    if (bc_Target.Hits < bc_Target.HitsMax)
                    {
                        from.SendMessage("You may not target creatures that are currently injured.");
                        return;
                    }

                    /*
                    if (bc_Target.CreationTime + SpawnCooldown >= DateTime.UtcNow)
                    {
                        string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, bc_Target.CreationTime + SpawnCooldown, false, false, true, true, true);

                        from.SendMessage("That has spawned too recently to be converted. You must wait another + " + timeRemaining + ".");
                        return;
                    }
                    */

                    if (bc_Target.LastCombatTime + CombatCooldown >= DateTime.UtcNow)
                    {
                        string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, bc_Target.LastCombatTime + CombatCooldown, false, false, true, true, true);

                        from.SendMessage("That has been in combat too recently. You must wait another " + timeRemaining + ".");
                        return;
                    }

                    if (!from.CanBeginAction(typeof(ParagonWand)))
                    {
                        from.SendMessage("You may only use a single paragon wand per hour.");
                        return;
                    }
                }

                bool targetValid = true;

                IPooledEnumerable mobilesNearby = bc_Target.Map.GetMobilesInRange(bc_Target.Location, 12);

                if (player.AccessLevel == AccessLevel.Player)
                {
                    foreach (Mobile mobile in mobilesNearby)
                    {
                        if (mobile.Combatant == bc_Target)
                        {
                            targetValid = false;
                            from.SendMessage("That creature is currently being engaged in combat by someone.");

                            break;
                        }

                        BaseCreature nearbyCreature = mobile as BaseCreature;
                        PlayerMobile nearbyPlayer = mobile as PlayerMobile;

                        if (mobile != from)
                        {
                            if (nearbyCreature != null)
                            {
                                if (nearbyCreature.Controlled && nearbyCreature.ControlMaster is PlayerMobile && nearbyCreature.ControlMaster != from)
                                {
                                    targetValid = false;
                                    from.SendMessage("That target is too near another player's follower.");
                                    break;
                                }
                            }

                            if (nearbyPlayer != null)
                            {
                                targetValid = false;
                                from.SendMessage("That target is too near another player.");
                                break;
                            }
                        }
                    }
                }

                mobilesNearby.Free();

                if (targetValid)
                {
                    bc_Target.PlaySound(0x652);
                    
                    Effects.SendLocationEffect(bc_Target.Location, bc_Target.Map, 0x3967, 30, 15, 2586, 0);
                    
                    bc_Target.IsParagon = true;
                    bc_Target.ConvertedParagon = true;

                    from.SendMessage("You transform the creature into a mighty paragon.");

                    m_ParagonWand.m_Charges--;

                    if (from.AccessLevel == AccessLevel.Player)
                    {
                        from.BeginAction(typeof(ParagonDevolveWand));

                        Timer.DelayCall(ParagonDevolveWand.UsageCooldown, delegate
                        {
                            if (from != null)
                                from.EndAction(typeof(ParagonDevolveWand));
                        });
                    }

                    if (m_ParagonWand.m_Charges == 0)
                        m_ParagonWand.Delete();
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version

            writer.Write(m_Charges);           
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_Charges = reader.ReadInt();              
            }
        }
    }
}