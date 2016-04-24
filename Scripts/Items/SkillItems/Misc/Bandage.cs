using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

using Server.Multis;
using Server.Spells;

namespace Server.Items
{
    public class Bandage : Item, IDyable
    {
        public static int Range = 1;

        public override double DefaultWeight
        {
            get { return 0.1; }
        }

        public static void Initialize()
        {
            EventSink.BandageTargetRequest += new BandageTargetRequestEventHandler(EventSink_BandageTargetRequest);
        }

        [Constructable]
        public Bandage(): this(1)
        {
        }

        [Constructable]
        public Bandage(int amount): base(0xE21)
        {
            Name = "bandage";

            Stackable = true;
            Amount = amount;
        }

        public Bandage(Serial serial): base(serial)
        {
        }

        public virtual bool Dye(Mobile from, DyeTub sender)
        {
            if (Deleted)
                return false;

            Hue = sender.DyedHue;

            return true;
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
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(GetWorldLocation(), Range))
            {
                from.RevealingAction();

                from.SendLocalizedMessage(500948); // Who will you use the bandages on?

                from.Target = new InternalTarget(this);
            }

            else
            {
                from.SendLocalizedMessage(500295); // You are too far away to do that.
            }
        }

        private static void EventSink_BandageTargetRequest(BandageTargetRequestEventArgs e)
        {
            Bandage b = e.Bandage as Bandage;

            if (b == null || b.Deleted)
                return;

            Mobile from = e.Mobile;

            if (from.InRange(b.GetWorldLocation(), Range))
            {
                Target t = from.Target;

                if (t != null)
                {
                    Target.Cancel(from);
                    from.Target = null;
                }

                from.RevealingAction();
                from.SendLocalizedMessage(500948); // Who will you use the bandages on?

                new InternalTarget(b).Invoke(from, e.Target);
            }

            else
            {
                from.SendLocalizedMessage(500295); // You are too far away to do that.
            }
        }

        private class InternalTarget : Target
        {
            private Bandage m_Bandage;

            public InternalTarget(Bandage bandage): base(Bandage.Range, false, TargetFlags.Beneficial)
            {
                m_Bandage = bandage;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Bandage.Deleted)
                    return;

                if (targeted is Mobile)
                {
                    Mobile target = targeted as Mobile;

                    if (target.Hidden)
                    {
                        from.SendMessage("That cannot be seen.");
                        return;
                    }

                    else if (from.InRange(m_Bandage.GetWorldLocation(), Bandage.Range))
                    {
                        PlayerMobile playerTarget = targeted as PlayerMobile;
                        
                        if (BandageContext.BeginHeal(from, (Mobile)targeted) != null)
                        {
                            if (!Engines.ConPVP.DuelContext.IsFreeConsume(from))
                                m_Bandage.Consume();
                        }
                    }

                    else
                        from.SendLocalizedMessage(500295); // You are too far away to do that.                    
                }

                else if (targeted is PlagueBeastInnard)
                {
                    if (((PlagueBeastInnard)targeted).OnBandage(from))
                        m_Bandage.Consume();
                }

                else
                    from.SendLocalizedMessage(500970); // Bandages can not be used on that.                
            }

            protected override void OnNonlocalTarget(Mobile from, object targeted)
            {
                if (targeted is PlagueBeastInnard)
                {
                    if (((PlagueBeastInnard)targeted).OnBandage(from))
                        m_Bandage.Consume();
                }

                else
                    base.OnNonlocalTarget(from, targeted);
            }
        }
    }

    public class BandageContext
    {
        private Mobile m_Healer;
        private Mobile m_Patient;
        private int m_Slips;
        private Timer m_Timer;

        public Mobile Healer { get { return m_Healer; } }
        public Mobile Patient { get { return m_Patient; } }
        public int Slips { get { return m_Slips; } set { m_Slips = value; } }
        public Timer Timer { get { return m_Timer; } }

        public void Slip()
        {
            m_Healer.SendLocalizedMessage(500961); // Your fingers slip!
            ++m_Slips;
        }

        public BandageContext(Mobile healer, Mobile patient, TimeSpan delay)
        {
            m_Healer = healer;
            m_Patient = patient;

            m_Timer = new InternalTimer(this, delay);
            m_Timer.Start();
        }

        public void StopHeal()
        {
            m_Table.Remove(m_Healer);

            if (m_Timer != null)
                m_Timer.Stop();

            m_Timer = null;
        }

        private static Dictionary<Mobile, BandageContext> m_Table = new Dictionary<Mobile, BandageContext>();

        public static BandageContext GetContext(Mobile healer)
        {
            BandageContext bc = null;
            m_Table.TryGetValue(healer, out bc);

            return bc;
        }

        public static SkillName GetPrimarySkill(Mobile m)
        {
            BaseCreature bc_Creature = m as BaseCreature;

            if (bc_Creature != null)
            {
                if (bc_Creature.IsHenchman)
                    return SkillName.Healing;

                if (bc_Creature.Tameable)
                    return SkillName.Veterinary;

                else if (!bc_Creature.Body.IsHuman)
                    return SkillName.Veterinary;                
            }
                return SkillName.Healing;
        }

        public static SkillName GetSecondarySkill(Mobile m)
        {
            BaseCreature bc_Creature = m as BaseCreature;

            if (bc_Creature != null)
            {
                if (bc_Creature.IsHenchman)
                    return SkillName.Anatomy;

                if (bc_Creature.Tameable)
                    return SkillName.AnimalLore;
            }

            return SkillName.Anatomy;
        }

        public void EndHeal()
        {
            StopHeal();

            int healerNumber = -1, patientNumber = -1;
            bool playSound = true;
            bool checkSkills = false;

            SkillName primarySkill = GetPrimarySkill(m_Patient);
            SkillName secondarySkill = GetSecondarySkill(m_Patient);

            BaseCreature petPatient = m_Patient as BaseCreature;

            bool healDamage = true;
            bool healThroughPoison = true;
            double BandageHealThroughPoisonScalar = SpellHelper.HealThroughPoisonScalar;

            int effectHue = 0;

            DungeonArmor.PlayerDungeonArmorProfile bandagerDungeonArmor = new DungeonArmor.PlayerDungeonArmorProfile(m_Healer, null);

            if (bandagerDungeonArmor.MatchingSet && !bandagerDungeonArmor.InPlayerCombat)
            {
                BandageHealThroughPoisonScalar += bandagerDungeonArmor.DungeonArmorDetail.BandageHealThroughPoisonScalar;
                effectHue = bandagerDungeonArmor.DungeonArmorDetail.EffectHue;                
            }

            if (!m_Healer.Alive)
            {
                healDamage = false;

                healerNumber = 500962; // You were unable to finish your work before you died.
                patientNumber = -1;
                playSound = false;
            }

            else if (Engines.ConPVP.DuelContext.CheckSuddenDeath(m_Patient))
            {
                healDamage = false;

                m_Healer.SendMessage(0x22, "You cannot use this item when in sudden death.");

                return;
            }

            else if (m_Patient.Hidden && m_Patient != m_Healer)
            {
                healDamage = false;

                m_Healer.SendMessage("You can no longer see your patient.");

                return;
            }

            else if (!m_Healer.InRange(m_Patient, Bandage.Range))
            {
                healDamage = false;

                healerNumber = 500963; // You did not stay close enough to heal your target.
                patientNumber = -1;
                playSound = false;
            }

            else if ((SpellHelper.CheckMulti(m_Healer.Location, m_Healer.Map) || SpellHelper.CheckMulti(m_Patient.Location, m_Patient.Map)) && !m_Healer.InLOS(m_Patient))
            {
                healDamage = false;

                healerNumber = 500963; // You did not stay close enough to heal your target.
                patientNumber = -1;
                playSound = false;
            }

            else if (!m_Patient.Alive || (petPatient != null && petPatient.IsDeadPet))
            {
                healDamage = false;

                double healing = m_Healer.Skills[primarySkill].Value;
                double anatomy = m_Healer.Skills[secondarySkill].Value;
                double chance = ((healing - 68.0) / 50.0) - (m_Slips * 0.02);

                if (((checkSkills = (healing >= 80.0 && anatomy >= 80.0)) && chance > Utility.RandomDouble())
                      || (Core.SE && petPatient is Factions.FactionWarHorse && petPatient.ControlMaster == m_Healer))	//TODO: Dbl check doesn't check for faction of the horse here?
                {
                    if (m_Patient.Map == null || !m_Patient.Map.CanFit(m_Patient.Location, 16, false, false))
                    {
                        healerNumber = 501042; // Target can not be resurrected at that location.
                        patientNumber = 502391; // Thou can not be resurrected there!
                    }

                    else
                    {
                        healerNumber = 500965; // You are able to resurrect your patient.
                        patientNumber = -1;

                        m_Patient.PlaySound(0x214);
                        m_Patient.FixedEffect(0x376A, 10, 16);

                        if (petPatient != null && petPatient.IsDeadPet)
                        {
                            Mobile master = petPatient.ControlMaster;

                            /*
                            if (!petPatient.CanBeResurrectedThroughVeterinary)
                            {
                                m_Healer.SendMessage("Another item is required to resurrect this creature");
                                healerNumber = 500966; // You are unable to resurrect your patient.
                            }
                             * */

                            if (master != null && master.InRange(petPatient, 3))
                            {
                                healerNumber = 503255; // You are able to resurrect the creature.

                                master.CloseGump(typeof(PetResurrectGump));
                                master.SendGump(new PetResurrectGump(m_Healer, petPatient));
                            }

                            else
                            {
                                bool found = false;

                                if (!found)
                                    healerNumber = 1049670; // The pet's owner must be nearby to attempt resurrection.
                            }
                        }

                        else
                        {
                            m_Patient.CloseGump(typeof(ResurrectGump));
                            m_Patient.SendGump(new ResurrectGump(m_Patient, m_Healer));
                        }
                    }
                }

                else
                {
                    if (petPatient != null && petPatient.IsDeadPet)
                        healerNumber = 503256; // You fail to resurrect the creature.
                    else
                        healerNumber = 500966; // You are unable to resurrect your patient.

                    patientNumber = -1;
                }
            }

            else if (m_Patient.Poisoned)
            {
                healDamage = false;

                m_Healer.SendLocalizedMessage(500969); // You finish applying the bandages.

                double healing = m_Healer.Skills[primarySkill].Value;
                double anatomy = m_Healer.Skills[secondarySkill].Value;
                double chance = ((healing - 30.0) / 50.0) - (m_Patient.Poison.Level * 0.05) - (m_Slips * 0.02);

                if ((checkSkills = (healing >= 60.0 && anatomy >= 60.0)) && chance > Utility.RandomDouble())
                {
                    if (m_Patient.CurePoison(m_Healer))
                    {
                        healerNumber = (m_Healer == m_Patient) ? -1 : 1010058; // You have cured the target of all poisons.
                        patientNumber = 1010059; // You have been cured of all poisons.
                    }

                    else
                    {
                        healerNumber = -1;
                        patientNumber = -1;
                    }
                }

                else
                {
                    healerNumber = 1010060; // You have failed to cure your target!
                    patientNumber = -1;
                }                
            }

            else if (m_Patient.Hits == m_Patient.HitsMax)
            {
                healDamage = false;

                healerNumber = 500967; // You heal what little damage your patient had.
                patientNumber = -1;
            }
            
            if (healDamage)
            {
                checkSkills = true;
                patientNumber = -1;

                double healing = m_Healer.Skills[primarySkill].Value;
                double anatomy = m_Healer.Skills[secondarySkill].Value;
                double chance = ((healing + 20.0) / 100.0) - (m_Slips * 0.02);

                if (chance >= Utility.RandomDouble())
                {
                    healerNumber = 500969; // You finish applying the bandages.

                    double min, max;

                    if (Core.AOS)
                    {
                        min = (anatomy / 8.0) + (healing / 5.0) + 4.0;
                        max = (anatomy / 6.0) + (healing / 2.5) + 4.0;
                    }
                    else
                    {
                        min = (anatomy / 5.0) + (healing / 5.0) + 3.0;
                        max = (anatomy / 5.0) + (healing / 2.0) + 10.0;
                    }

                    double toHeal = min + (Utility.RandomDouble() * (max - min));

                    if (m_Patient.Body.IsMonster || m_Patient.Body.IsAnimal)
                        toHeal += m_Patient.HitsMax / 100;
                                       
                     toHeal -= m_Slips * 2.5;

                    if (healThroughPoison)
                        toHeal *= BandageHealThroughPoisonScalar;

                    PlayerMobile playerHealer = m_Healer as PlayerMobile;

                    if (playerHealer != null)
                    {
                        if (UOACZSystem.IsUOACZValidMobile(playerHealer))
                        {
                            if (playerHealer.IsUOACZHuman)
                                toHeal += 20;
                        }

                        double superiorHealing = playerHealer.GetSpecialAbilityEntryValue(SpecialAbilityEffect.SuperiorHealing);

                        if (superiorHealing > 0)
                            toHeal *= superiorHealing;
                    }

                    if (toHeal < 1)
                    {
                        toHeal = 1;
                        healerNumber = 500968; // You apply the bandages, but they barely help.
                    }                   

                    int finalHeal = (int)toHeal;

                    m_Patient.Heal(finalHeal, m_Healer, false);
                }

                else
                {
                    healerNumber = 500968; // You apply the bandages, but they barely help.
                    playSound = false;
                }
            }

            if (healerNumber != -1)
                m_Healer.SendLocalizedMessage(healerNumber);

            if (patientNumber != -1)
                m_Patient.SendLocalizedMessage(patientNumber);

            if (healThroughPoison)
            {
                Effects.PlaySound(m_Healer.Location, m_Healer.Map, 0x64B);
                Effects.SendLocationParticles(EffectItem.Create(m_Healer.Location, m_Healer.Map, EffectItem.DefaultDuration), 0x376A, 9, 32, effectHue, 0, 5005, 0);
            }
            
            if (playSound)
                m_Patient.PlaySound(0x57);

            if (checkSkills)
            {
                m_Healer.CheckSkill(secondarySkill, 0.0, 120.0, 1.0);
                m_Healer.CheckSkill(primarySkill, 0.0, 120.0, 1.0);
            }
        }

        private class InternalTimer : Timer
        {
            private BandageContext m_Context;

            public InternalTimer(BandageContext context, TimeSpan delay): base(delay)
            {
                m_Context = context;
                Priority = TimerPriority.FiftyMS;
            }

            protected override void OnTick()
            {
                m_Context.EndHeal();
            }
        }

        public static BandageContext BeginHeal(Mobile healer, Mobile patient)
        {
            bool isDeadPet = (patient is BaseCreature && ((BaseCreature)patient).IsDeadPet);

            PlayerMobile playerPatient = patient as PlayerMobile;

            if (patient is Golem)
            {
                healer.SendLocalizedMessage(500970); // Bandages cannot be used on that.
                return null;
            }

            else if (patient is BaseCreature && ((BaseCreature)patient).IsAnimatedDead)
            {
                healer.SendLocalizedMessage(500951); // You cannot heal that.
                return null;
            }

            else if (!patient.Poisoned && patient.Hits == patient.HitsMax && !isDeadPet)
            {
                healer.SendLocalizedMessage(500955); // That being is not damaged!
                return null;
            }

            else if (!patient.Alive && (patient.Map == null || !patient.Map.CanFit(patient.Location, 16, false, false)))
            {
                healer.SendLocalizedMessage(501042); // Target cannot be resurrected at that location.
                return null;
            }

            if (healer.CanBeBeneficial(patient, true, true))
            {
                healer.DoBeneficial(patient);

                bool onSelf = (healer == patient);
                int dex = healer.Dex;

                double seconds;
                double resDelay = 0;
                
                if (!patient.Alive)
                    resDelay += 5;

                if (onSelf)
                {
                    seconds = SkillCooldown.HealingSelfCooldown;

                    DungeonArmor.PlayerDungeonArmorProfile bandagerDungeonArmor = new DungeonArmor.PlayerDungeonArmorProfile(healer, null);

                    if (bandagerDungeonArmor.MatchingSet && !bandagerDungeonArmor.InPlayerCombat)                    
                        seconds -= bandagerDungeonArmor.DungeonArmorDetail.BandageSelfTimeReduction;                    
                }

                else
                    seconds = SkillCooldown.HealingOtherCooldown + resDelay;

                BandageContext context = GetContext(healer);

                if (context != null)
                    context.StopHeal();

                if (patient.Region is UOACZRegion)
                    seconds = 10;

                double rapidTreatmentValue = healer.GetSpecialAbilityEntryValue(SpecialAbilityEffect.RapidTreatment);

                if (rapidTreatmentValue > 0)
                    seconds *= rapidTreatmentValue;

                context = new BandageContext(healer, patient, TimeSpan.FromSeconds(seconds));

                m_Table[healer] = context;

                if (!onSelf)
                    patient.SendLocalizedMessage(1008078, false, healer.Name); //  : Attempting to heal you.

                healer.SendLocalizedMessage(500956); // You begin applying the bandages.

                return context;
            }

            return null;
        }
    }
}