using System;
using System.Collections;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Factions;
using Server.Spells;
using Server.Spells.Spellweaving;
using Server.Achievements;

namespace Server.SkillHandlers
{
    public class AnimalTaming
    {
        private static Hashtable m_BeingTamed = new Hashtable();

        public static void Initialize()
        {
            SkillInfo.Table[(int)SkillName.AnimalTaming].Callback = new SkillUseCallback(OnUse);
        }

        private static bool m_DisableMessage;

        public static bool DisableMessage
        {
            get { return m_DisableMessage; }
            set { m_DisableMessage = value; }
        }

        public static TimeSpan OnUse(Mobile m)
        {
            m.RevealingAction();

            m.Target = new InternalTarget();
            m.RevealingAction();

            if (!m_DisableMessage)
                m.SendLocalizedMessage(502789); //Tame which animal?

            return TimeSpan.FromHours(6.0);
        }

        public static bool MustBeSubdued(BaseCreature bc)
        {
            if (bc.Owners.Count > 0) { return false; } //Checks to see if the animal has been tamed before
            return bc.SubdueBeforeTame && (bc.Hits > (bc.HitsMax / 10));
        }

        private class InternalTarget : Target
        {
            private bool m_SetSkillTime = true;

            public InternalTarget(): base(10, false, TargetFlags.None)
            {
            }

            protected override void OnTargetFinish(Mobile from)
            {
                if (m_SetSkillTime)
                    from.NextSkillTime = Core.TickCount;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                from.RevealingAction();

                if (targeted is Mobile)
                {
                    if (targeted is BaseCreature)
                    {
                        BaseCreature creature = (BaseCreature)targeted;

                        if (creature.IsHenchman)
                        {
                            from.SendMessage("You must recruit that individual with a henchman recruitment deed.");
                            return;
                        }

                        if (!creature.Tamable)
                            creature.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1049655, from.NetState); // That creature cannot be tamed.

                        else if (creature.Controlled)
                            creature.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 502804, from.NetState); // That animal looks tame already.

                        else if (from.Female && !creature.AllowFemaleTamer)
                            creature.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1049653, from.NetState); // That creature can only be tamed by males.

                        else if (!from.Female && !creature.AllowMaleTamer)
                            creature.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1049652, from.NetState); // That creature can only be tamed by females.

                        else if (creature is CuSidhe && from.Race != Race.Elf)
                            creature.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 502801, from.NetState); // You can't tame that!

                        else if (from.Followers + creature.ControlSlots > from.FollowersMax)
                            from.SendLocalizedMessage(1049611); // You have too many followers to tame that creature.

                        else if (creature.Owners.Count >= BaseCreature.MaxOwners && !creature.Owners.Contains(from))
                            creature.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1005615, from.NetState); // This animal has had too many owners and is too upset for you to tame.

                        else if (MustBeSubdued(creature))
                            creature.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1054025, from.NetState); // You must subdue this creature before you can tame it!

                        else if (from.Skills[SkillName.AnimalTaming].Value >= creature.MinTameSkill)
                        {
                            if (m_BeingTamed.Contains(targeted))
                                creature.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 502802, from.NetState); // Someone else is already taming this.

                            else
                            {
                                m_BeingTamed[targeted] = from;

                                from.LocalOverheadMessage(MessageType.Emote, 0x59, 1010597); // You start to tame the creature.
                                from.NonlocalOverheadMessage(MessageType.Emote, 0x59, 1010598); // *begins taming a creature.*

                                new InternalTimer(from, creature, Utility.Random(3, 2)).Start();

                                m_SetSkillTime = false;
                            }
                        }

                        else
                            creature.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 502806, from.NetState); // You have no chance of taming this creature.						
                    }

                    else
                        ((Mobile)targeted).PrivateOverheadMessage(MessageType.Regular, 0x3B2, 502469, from.NetState); // That being cannot be tamed.					
                }

                else
                    from.SendLocalizedMessage(502801); // You can't tame that!				
            }

            private class InternalTimer : Timer
            {
                private Mobile m_Tamer;
                private BaseCreature m_Creature;
                private int m_MaxCount;
                private int m_Count;
                private bool m_Paralyzed;
                private DateTime m_StartTime;

                public InternalTimer(Mobile tamer, BaseCreature creature, int count): base(TimeSpan.FromSeconds(3.0), TimeSpan.FromSeconds(3.0), count)
                {
                    m_Tamer = tamer;
                    m_Creature = creature;
                    m_MaxCount = count;
                    m_Paralyzed = creature.Paralyzed;
                    m_StartTime = DateTime.UtcNow;
                    Priority = TimerPriority.TwoFiftyMS;
                }

                protected override void OnTick()
                {
                    m_Count++;

                    DamageEntry de = m_Creature.FindMostRecentDamageEntry(false);
                    bool alreadyOwned = m_Creature.Owners.Contains(m_Tamer);

                    if (!m_Tamer.InRange(m_Creature, 10))
                    {
                        m_BeingTamed.Remove(m_Creature);
                        m_Tamer.NextSkillTime = Core.TickCount;
                        m_Creature.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 502795, m_Tamer.NetState); // You are too far away to continue taming.
                        Stop();
                    }

                    else if (!m_Tamer.CheckAlive())
                    {
                        m_BeingTamed.Remove(m_Creature);
                        m_Tamer.NextSkillTime = Core.TickCount;
                        m_Creature.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 502796, m_Tamer.NetState); // You are dead, and cannot continue taming.
                        Stop();
                    }

                    else if (!m_Tamer.CanSee(m_Creature) || !m_Tamer.InLOS(m_Creature) || !CanPath())
                    {
                        m_BeingTamed.Remove(m_Creature);
                        m_Tamer.NextSkillTime = Core.TickCount;
                        m_Tamer.SendLocalizedMessage(1049654); // You do not have a clear path to the animal you are taming, and must cease your attempt.
                        Stop();
                    }

                    else if (m_Creature.IsHenchman)
                    {
                        m_BeingTamed.Remove(m_Creature);
                        m_Tamer.NextSkillTime = Core.TickCount;
                        m_Creature.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1049655, m_Tamer.NetState); // That creature cannot be tamed.
                        Stop();
                    }

                    else if (!m_Creature.Tamable)
                    {
                        m_BeingTamed.Remove(m_Creature);
                        m_Tamer.NextSkillTime = Core.TickCount;
                        m_Creature.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1049655, m_Tamer.NetState); // That creature cannot be tamed.
                        Stop();
                    }

                    else if (m_Creature.Controlled)
                    {
                        m_BeingTamed.Remove(m_Creature);
                        m_Tamer.NextSkillTime = Core.TickCount;
                        m_Creature.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 502804, m_Tamer.NetState); // That animal looks tame already.
                        Stop();
                    }

                    else if (m_Creature.Owners.Count >= BaseCreature.MaxOwners && !m_Creature.Owners.Contains(m_Tamer))
                    {
                        m_BeingTamed.Remove(m_Creature);
                        m_Tamer.NextSkillTime = Core.TickCount;
                        m_Creature.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1005615, m_Tamer.NetState); // This animal has had too many owners and is too upset for you to tame.
                        Stop();
                    }

                    else if (MustBeSubdued(m_Creature))
                    {
                        m_BeingTamed.Remove(m_Creature);
                        m_Tamer.NextSkillTime = Core.TickCount;
                        m_Creature.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1054025, m_Tamer.NetState); // You must subdue this creature before you can tame it!
                        Stop();
                    }

                    else if (de != null && de.LastDamage > m_StartTime)
                    {
                        m_BeingTamed.Remove(m_Creature);
                        m_Tamer.NextSkillTime = Core.TickCount;
                        m_Creature.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 502794, m_Tamer.NetState); // The animal is too angry to continue taming.
                        Stop();
                    }

                    else if (m_Count < m_MaxCount)
                    {
                        m_Tamer.RevealingAction();

                        switch (Utility.Random(3))
                        {
                            case 0: m_Tamer.PublicOverheadMessage(MessageType.Regular, 0x3B2, Utility.Random(502790, 4)); break;
                            case 1: m_Tamer.PublicOverheadMessage(MessageType.Regular, 0x3B2, Utility.Random(1005608, 6)); break;
                            case 2: m_Tamer.PublicOverheadMessage(MessageType.Regular, 0x3B2, Utility.Random(1010593, 4)); break;
                        }

                        if (!alreadyOwned) // Passively check animal lore for gain
                            m_Tamer.CheckTargetSkill(SkillName.AnimalLore, m_Creature, 0.0, 120.0);

                        if (m_Creature.Paralyzed)
                            m_Paralyzed = true;
                    }

                    else
                    {
                        m_Tamer.RevealingAction();
                        m_Tamer.NextSkillTime = Core.TickCount;
                        m_BeingTamed.Remove(m_Creature);

                        if (m_Creature.Paralyzed)
                            m_Paralyzed = true;

                        double minSkill = m_Creature.MinTameSkill + (m_Creature.Owners.Count * 6.0);

                        //Check for Skillgain (Success Chance Calculated Separately)
                        if (!alreadyOwned)
                        {
                            m_Tamer.CheckTargetSkill(SkillName.AnimalTaming, m_Creature, minSkill, minSkill + 25.0);
                            m_Tamer.CheckTargetSkill(SkillName.AnimalLore, m_Creature, 0.0, 120.0);
                        }

                        double successChance = 0;
                        double chanceResult = Utility.RandomDouble();

                        successChance = (m_Tamer.Skills[SkillName.AnimalTaming].Value - m_Creature.MinTameSkill) * .04;

                        var pmTamer = m_Tamer as PlayerMobile;
                        
                        if (alreadyOwned || chanceResult <= successChance)
                        {
                            if (alreadyOwned)
                                m_Tamer.SendLocalizedMessage(502797); // That wasn't even challenging.							

                            else
                            {
                                m_Creature.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 502799, m_Tamer.NetState); // It seems to accept you as master.
                                m_Creature.Owners.Add(m_Tamer);

                                // IPY ACHIEVEMENTS
                                TrackTamingAchievements();
                                // IPY ACHIEVEMENTS
                            }

                            m_Creature.TimesTamed++;
                            m_Creature.SetControlMaster(m_Tamer);
                            m_Creature.IsBonded = false;
                            m_Creature.OwnerAbandonTime = DateTime.UtcNow + m_Creature.AbandonDelay;
                        }

                        else
                            m_Creature.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 502798, m_Tamer.NetState); // You fail to tame the creature.						
                    }
                }

                private bool CanPath()
                {
                    IPoint3D p = m_Tamer as IPoint3D;

                    if (p == null)
                        return false;

                    if (m_Creature.InRange(new Point3D(p), 1))
                        return true;

                    MovementPath path = new MovementPath(m_Creature, new Point3D(p));

                    return path.Success;
                }

                private void TrackTamingAchievements()
                {
                    if (!(m_Tamer is PlayerMobile))
                        return;

                    if (m_Creature is Chicken) AchievementSystem.Instance.TickProgress(m_Tamer, AchievementTriggers.Trigger_TameChicken);
                    else if (m_Creature is BlackBear || m_Creature is BrownBear || m_Creature is GrizzlyBear) AchievementSystem.Instance.TickProgress(m_Tamer, AchievementTriggers.Trigger_TameBear);
                    else if (m_Creature is Dog) AchievementSystem.Instance.TickProgress(m_Tamer, AchievementTriggers.Trigger_TameDog);
                    else if (m_Creature is TimberWolf) AchievementSystem.Instance.TickProgress(m_Tamer, AchievementTriggers.Trigger_TameTimberWolf);
                    else if (m_Creature is Hind) AchievementSystem.Instance.TickProgress(m_Tamer, AchievementTriggers.Trigger_TameHind);
                    else if (m_Creature is Cat) AchievementSystem.Instance.TickProgress(m_Tamer, AchievementTriggers.Trigger_TameCat);
                    else if (m_Creature is GiantToad) AchievementSystem.Instance.TickProgress(m_Tamer, AchievementTriggers.Trigger_TameGiantToad);
                    else if (m_Creature is JackRabbit) AchievementSystem.Instance.TickProgress(m_Tamer, AchievementTriggers.Trigger_TameJackRabbit);
                    else if (m_Creature is Rabbit) AchievementSystem.Instance.TickProgress(m_Tamer, AchievementTriggers.Trigger_TameRabbit);
                    else if (m_Creature is Rat) AchievementSystem.Instance.TickProgress(m_Tamer, AchievementTriggers.Trigger_TameRat);
                    else if (m_Creature is Gorilla) AchievementSystem.Instance.TickProgress(m_Tamer, AchievementTriggers.Trigger_TameGorilla);
                    else if (m_Creature is Walrus) AchievementSystem.Instance.TickProgress(m_Tamer, AchievementTriggers.Trigger_TameWalrus);
                    else if (m_Creature is PolarBear) AchievementSystem.Instance.TickProgress(m_Tamer, AchievementTriggers.Trigger_TamePolarBear);
                    else if (m_Creature is HellHound) AchievementSystem.Instance.TickProgress(m_Tamer, AchievementTriggers.Trigger_TameHellHound);

                    AchievementSystem.Instance.TickProgress(m_Tamer, AchievementTriggers.Trigger_TameAnyAnimal);
                }
            }
        }
    }
}