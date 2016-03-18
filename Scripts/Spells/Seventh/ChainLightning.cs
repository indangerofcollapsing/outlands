using System;
using System.Collections.Generic;
using Server.Network;
using Server.Items;
using Server.Targeting;
using Server.Mobiles;
using System.Collections;
using System.Collections.Generic;

namespace Server.Spells.Seventh
{
    public class ChainLightningSpell : MagerySpell
    {
        private static SpellInfo m_Info = new SpellInfo(
            "Chain Lightning", "Vas Ort Grav",
            209,
            9022,
            false,
            Reagent.BlackPearl,
            Reagent.Bloodmoss,
            Reagent.MandrakeRoot,
            Reagent.SulfurousAsh
            );

        public override SpellCircle Circle { get { return SpellCircle.Seventh; } }

        public ChainLightningSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override void OnCast()
        {
            BaseCreature casterCreature = Caster as BaseCreature;

            if (casterCreature != null)
            {
                if (casterCreature.SpellTarget != null)
                {
                    IPoint3D targetLocation = casterCreature.SpellTarget.Location as IPoint3D;

                    if (targetLocation != null)
                    {
                        this.Target(targetLocation);
                    }
                }
            }

            else
            {
                Caster.Target = new InternalTarget(this);
            }
        }

        public override bool DelayedDamage { get { return true; } }

        public void Target(IPoint3D p)
        {
            if (!Caster.CanSee(p))            
                Caster.SendLocalizedMessage(500237); // Target can not be seen.            

            else if (SpellHelper.CheckTown(p, Caster) && CheckSequence())
            {
                int damageMin = 20;
                int damageMax = 25;

                double damage = 0;

                BaseCreature creatureCaster = Caster as BaseCreature;

                //Creature
                if (creatureCaster != null)
                {
                    SpellHelper.Turn(creatureCaster, p);

                    if (p is Item)
                        p = ((Item)p).GetWorldLocation();

                    Map map = creatureCaster.Map;

                    //Increased Range for High Level Creature Casters
                    double magerySkill = creatureCaster.Skills[SkillName.Magery].Value;

                    int radius = 2 + (int)(Math.Floor((magerySkill - 75) / 25));

                    if (radius < 2)
                        radius = 2;

                    IPooledEnumerable nearbyMobiles = map.GetMobilesInRange(new Point3D(p), radius);

                    Queue m_Queue = new Queue();

                    foreach (Mobile mobile in nearbyMobiles)
                    {
                        if (creatureCaster == mobile)
                            continue;

                        if (!creatureCaster.CanBeHarmful(mobile, false))
                            continue;

                        if (mobile.AccessLevel > AccessLevel.Player)
                            continue;

                        if (creatureCaster.ControlMaster is PlayerMobile && !creatureCaster.IsBarded())
                        {
                            if (creatureCaster.ControlMaster == mobile)
                                continue;

                            if (mobile is BaseCreature)
                            {
                                BaseCreature bc_Target = mobile as BaseCreature;

                                if (bc_Target.ControlMaster == creatureCaster.ControlMaster && !creatureCaster.IsBarded())
                                    continue;
                            }
                        }

                        bool validTarget = false;

                        if (mobile == creatureCaster.Combatant)
                            validTarget = true;

                        if (creatureCaster.ControlMaster == null)
                        {
                            if (creatureCaster.DictCombatTargeting[CombatTargeting.PlayerAny] > 0)
                            {
                                if (mobile is PlayerMobile)
                                    validTarget = true;
                            }
                        }

                        foreach (AggressorInfo aggressor in creatureCaster.Aggressors)
                        {
                            if (aggressor.Attacker == mobile || aggressor.Defender == mobile)
                                validTarget = true;
                        }

                        foreach (AggressorInfo aggressed in creatureCaster.Aggressed)
                        {
                            if (aggressed.Attacker == mobile || aggressed.Defender == mobile)
                                validTarget = true;
                        }

                        if (creatureCaster.GetFactionAllegiance(mobile) == BaseCreature.Allegiance.Enemy || creatureCaster.GetEthicAllegiance(mobile) == BaseCreature.Allegiance.Enemy)
                            validTarget = true;

                        if (!validTarget)
                            continue;

                        m_Queue.Enqueue(mobile);
                    }

                    nearbyMobiles.Free();

                    while (m_Queue.Count > 0)
                    {
                        damage = (double)Utility.RandomMinMax(damageMin, damageMax);

                        Mobile mobile = (Mobile)m_Queue.Dequeue();

                        if (CheckResisted(mobile))
                        {
                            damage *= 0.75;
                            mobile.SendLocalizedMessage(501783); //You feel yourself resisting magical energy.
                        }

                        Effects.PlaySound(p, creatureCaster.Map, 0x29);

                        damage *= GetDamageScalar(mobile);

                        SpellHelper.Damage(this, mobile, damage, 0, 0, 0, 0, 100);

                        mobile.BoltEffect(0);
                    }
                }

                //Player
                else
                {
                    int radius = 2;

                    SpellHelper.Turn(Caster, p);

                    if (p is Item)
                        p = ((Item)p).GetWorldLocation();

                    List<Mobile> targets = new List<Mobile>();

                    Map map = Caster.Map;

                    if (map != null)
                    {
                        IPooledEnumerable eable = map.GetMobilesInRange(new Point3D(p), radius);

                        foreach (Mobile m in eable)
                        {
                            if (Caster.CanBeHarmful(m, false))
                            {
                                targets.Add(m);
                            }
                        }

                        eable.Free();
                    }

                    if (targets.Count > 0)
                    {
                        Effects.PlaySound(p, Caster.Map, 0x29);

                        bool enhancedSpellcast = SpellHelper.IsEnhancedSpell(Caster, null, EnhancedSpellbookType.Energy, false, true);
                        Boolean chargedSpellcast = SpellHelper.IsChargedSpell(Caster, null, false, Scroll != null);
                        
                        for (int i = 0; i < targets.Count; ++i)
                        {
                            damage = (double)Utility.RandomMinMax(damageMin, damageMax);

                            Mobile m = targets[i];

                            if (CheckResisted(m))
                            {
                                damage *= 0.75;
                                m.SendLocalizedMessage(501783); //You feel yourself resisting magical energy.
                            }

                            Boolean isTamedTarget = SpellHelper.IsTamedTarget(Caster, m);

                            if (enhancedSpellcast && m is BaseCreature)
                            {
                                if (isTamedTarget)
                                    damage *= SpellHelper.enhancedTamedCreatureMultiplier;

                                else
                                    damage *= SpellHelper.enhancedMultiplier;
                            }

                            if (chargedSpellcast && m is BaseCreature)
                            {
                                if (isTamedTarget)
                                    damage *= SpellHelper.chargedTamedCreatureMultiplier;

                                else
                                    damage *= SpellHelper.chargedMultiplier;

                                m.BoltEffect(0);
                            }

                            else
                            {
                                m.BoltEffect(0);
                            }

                            if (m is PlayerMobile)
                                damage *= .5;

                            damage *= GetDamageScalar(m);

                            SpellHelper.Damage(this, m, damage, 0, 0, 0, 0, 100);
                        }
                    }
                }
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private ChainLightningSpell m_Owner;

            public InternalTarget(ChainLightningSpell owner)
                : base(Core.ML ? 10 : 12, true, TargetFlags.None)
            {
                m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                IPoint3D p = o as IPoint3D;

                if (p != null)
                    m_Owner.Target(p);
            }

            protected override void OnTargetFinish(Mobile from)
            {
                m_Owner.FinishSequence();
            }
        }
    }
}