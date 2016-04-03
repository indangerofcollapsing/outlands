using System;
using System.Collections;
using Server.Targeting;
using Server.Network;
using Server.Misc;
using Server.Items;
using Server.Mobiles;
using Server.Multis;
using Server.Custom;

namespace Server.Spells.Fifth
{
    public class PoisonFieldSpell : MagerySpell
    {
        private static SpellInfo m_Info = new SpellInfo(
                "Poison Field", "In Nox Grav",
                230,
                9052,
                false,
                Reagent.BlackPearl,
                Reagent.Nightshade,
                Reagent.SpidersSilk
            );

        public override SpellCircle Circle { get { return SpellCircle.Fifth; } }

        public PoisonFieldSpell(Mobile caster, Item scroll): base(caster, scroll, m_Info)
        {
        }

        public override void OnCast()
        {
            BaseCreature casterCreature = Caster as BaseCreature;

            if (casterCreature != null)
            {
                if (casterCreature.SpellTarget != null)                
                    this.Target(casterCreature.SpellTarget);                
            }

            else            
                Caster.Target = new InternalTarget(this);            
        }

        public void Target(IPoint3D p)
        {
            if (!Caster.CanSee(p))            
                Caster.SendLocalizedMessage(500237); // Target can not be seen.            

            else if (BaseBoat.FindBoatAt(p, Caster.Map) != null)
                Caster.SendMessage("That location is blocked.");

            else if (SpellHelper.CheckTown(p, Caster) && CheckSequence())
            {
                SpellHelper.Turn(Caster, p);

                SpellHelper.GetSurfaceTop(ref p);

                int dx = Caster.Location.X - p.X;
                int dy = Caster.Location.Y - p.Y;
                int rx = (dx - dy) * 44;
                int ry = (dx + dy) * 44;

                bool eastToWest;

                if (rx >= 0 && ry >= 0)                
                    eastToWest = false;                

                else if (rx >= 0)                
                    eastToWest = true;                

                else if (ry >= 0)                
                    eastToWest = true;                

                else                
                    eastToWest = false;                

                Effects.PlaySound(p, Caster.Map, 0x20B);

                int itemID = eastToWest ? 0x3915 : 0x3922;
               
                TimeSpan duration = TimeSpan.FromSeconds(3.0 + (Caster.Skills[SkillName.Magery].Value * 0.4));

                bool chargeUsed = false;

                bool enhancedSpellcast = SpellHelper.IsEnhancedSpell(Caster, null, EnhancedSpellbookType.Warlock, true, true);
                
                for (int i = -2; i <= 2; ++i)
                {
                    Point3D loc = new Point3D(eastToWest ? p.X + i : p.X, eastToWest ? p.Y : p.Y + i, p.Z);

                    InternalItem item = new InternalItem(itemID, loc, Caster, Caster.Map, duration, i);

                    if (enhancedSpellcast)                    
                        item.m_Enhanced = true;                    
                }

                DungeonArmor.PlayerDungeonArmorProfile casterDungeonArmor = new DungeonArmor.PlayerDungeonArmorProfile(Caster, null);

                if (casterDungeonArmor.MatchingSet && !casterDungeonArmor.InPlayerCombat)
                {
                    if (Utility.RandomDouble() <= casterDungeonArmor.DungeonArmorDetail.PoisonSpellNoManaCostChance)
                    {
                        Caster.Mana += 15;
                        Caster.SendMessage("You feel a rush of energy from your armor, fueling mana into the spell.");

                        Effects.PlaySound(Caster.Location, Caster.Map, 0x64B);
                        Effects.SendLocationParticles(EffectItem.Create(Caster.Location, Caster.Map, EffectItem.DefaultDuration), 0x376A, 9, 32, casterDungeonArmor.DungeonArmorDetail.EffectHue, 0, 5005, 0);
                    }
                }
            }

            FinishSequence();
        }

        [DispellableField]
        public class InternalItem : Item
        {
            private Timer m_Timer;
            private DateTime m_End;
            private Mobile m_Caster;

            public Boolean m_Enhanced = false;

            public override bool BlocksFit { get { return true; } }

            public InternalItem(int itemID, Point3D loc, Mobile caster, Map map, TimeSpan duration, int val): base(itemID)
            {
                bool canFit = SpellHelper.AdjustField(ref loc, map, 12, false);

                Visible = false;
                Movable = false;
                Light = LightType.Circle300;

                MoveToWorld(loc, map);

                bool enhancedSpellcast = SpellHelper.IsEnhancedSpell(m_Caster, null, EnhancedSpellbookType.Warlock, false, true);

                if (enhancedSpellcast)
                    m_Enhanced = true;

                else
                    m_Enhanced = false; 

                m_Caster = caster;

                m_End = DateTime.UtcNow + duration;

                m_Timer = new InternalTimer(this, TimeSpan.FromSeconds(Math.Abs(val) * 0.2), caster.InLOS(this), canFit);
                m_Timer.Start();
            }

            public override void OnAfterDelete()
            {
                base.OnAfterDelete();

                if (m_Timer != null)
                    m_Timer.Stop();
            }

            public InternalItem(Serial serial): base(serial)
            {
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);

                writer.Write((int)0); //version

                //Version 0
                writer.Write(m_Caster);
                writer.WriteDeltaTime(m_End);
                writer.Write(m_Enhanced);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);

                int version = reader.ReadInt();

                //Version 0
                if (version >= 0)
                {
                    m_Caster = reader.ReadMobile();
                    m_End = reader.ReadDeltaTime();
                    m_Enhanced = reader.ReadBool();
                }

                //-----

                m_Timer = new InternalTimer(this, TimeSpan.Zero, true, true);
                m_Timer.Start();
            }

            public void ApplyPoisonTo(Mobile mobile)
            {
                if (m_Caster == null)
                    return;

                double magerySkill = m_Caster.Skills[SkillName.Magery].Value;
                double poisoningSkill = m_Caster.Skills[SkillName.Poisoning].Value;

                //Poisoning Skill is Capped by Magery
                if (poisoningSkill > magerySkill)
                    poisoningSkill = magerySkill;

                //Defaults to Regular Poison
                int poisonLevel = 1;

                //Against Non-Players
                if (!(mobile is PlayerMobile))
                {
                    double poisonResult = Utility.RandomDouble();

                    double greaterUpgradeChance = 1.0 * (poisoningSkill / 100 / 5);
                    double deadlyUpgradeChance = .30 * (poisoningSkill / 100 / 5);
                    double lethalUpgradeChance = .10 * (poisoningSkill / 100 / 5);

                    if (m_Enhanced)
                    {
                        greaterUpgradeChance *= 1.5;
                        deadlyUpgradeChance *= 1.5;
                        lethalUpgradeChance *= 1.5;
                    }

                    if (poisonResult <= greaterUpgradeChance && poisoningSkill >= 25)
                    {
                        if (poisonLevel < 2)
                            poisonLevel = 2;
                    }

                    if (poisonResult <= deadlyUpgradeChance && poisoningSkill >= 50)
                    {
                        if (poisonLevel < 3)
                            poisonLevel = 3;
                    }

                    if (poisonResult <= lethalUpgradeChance && poisoningSkill >= 75)
                    {
                        if (poisonLevel < 4)
                            poisonLevel = 4;
                    }
                }

                if (mobile.ApplyPoison(m_Caster, Poison.GetPoison(poisonLevel)) == ApplyPoisonResult.Poisoned)
                {
                }

                if (mobile is BaseCreature)
                    ((BaseCreature)mobile).OnHarmfulSpell(m_Caster);
            }
            
            public override bool OnMoveOver(Mobile mobile)
            {
                if (Visible && m_Caster != null && SpellHelper.ValidIndirectTarget(m_Caster, mobile) && m_Caster.CanBeHarmful(mobile, false))
                {
                    m_Caster.DoHarmful(mobile);

                    //Player Enhancement Customization: Venomous
                    bool venomous = PlayerEnhancementPersistance.IsCustomizationEntryActive(m_Caster, CustomizationType.Venomous);

                    if (venomous)
                        CustomizationAbilities.Venomous(mobile);
                    else
                        mobile.PlaySound(0x474);

                    ApplyPoisonTo(mobile);                   
                }

                return true;
            }

            private class InternalTimer : Timer
            {
                private InternalItem m_Item;
                private bool m_InLOS, m_CanFit;

                private static Queue m_Queue = new Queue();

                public InternalTimer(InternalItem item, TimeSpan delay, bool inLOS, bool canFit): base(delay, TimeSpan.FromSeconds(1.5))
                {
                    m_Item = item;
                    m_InLOS = inLOS;
                    m_CanFit = canFit;

                    Priority = TimerPriority.FiftyMS;
                }

                protected override void OnTick()
                {
                    if (m_Item.Deleted)
                        return;

                    if (!m_Item.Visible)
                    {
                        if (m_InLOS && m_CanFit)
                            m_Item.Visible = true;

                        else
                            m_Item.Delete();

                        if (!m_Item.Deleted)
                        {
                            m_Item.ProcessDelta();
                            Effects.SendLocationParticles(EffectItem.Create(m_Item.Location, m_Item.Map, EffectItem.DefaultDuration), 0x376A, 9, 10, 5040);
                        }
                    }

                    else if (DateTime.UtcNow > m_Item.m_End)
                    {
                        m_Item.Delete();
                        Stop();
                    }

                    else
                    {
                        Map map = m_Item.Map;
                        Mobile caster = m_Item.m_Caster;

                        if (map != null && caster != null)
                        {
                            bool eastToWest = (m_Item.ItemID == 0x3915);
                            IPooledEnumerable eable = map.GetMobilesInBounds(new Rectangle2D(m_Item.X - (eastToWest ? 0 : 1), m_Item.Y - (eastToWest ? 1 : 0), (eastToWest ? 1 : 2), (eastToWest ? 2 : 1)));

                            foreach (Mobile m in eable)
                            {
                                if ((m.Z + 16) > m_Item.Z && (m_Item.Z + 12) > m.Z && SpellHelper.ValidIndirectTarget(caster, m) && caster.CanBeHarmful(m, false))
                                    m_Queue.Enqueue(m);
                            }

                            eable.Free();

                            while (m_Queue.Count > 0)
                            {
                                Mobile mobile = (Mobile)m_Queue.Dequeue();

                                caster.DoHarmful(mobile);

                                //Player Enhancement Customization: Venomous
                                bool venomous = PlayerEnhancementPersistance.IsCustomizationEntryActive(caster, CustomizationType.Venomous);

                                if (venomous)
                                    CustomizationAbilities.Venomous(mobile);
                                else
                                    mobile.PlaySound(0x474);

                                m_Item.ApplyPoisonTo(mobile);                                
                            }
                        }
                    }
                }
            }
        }

        private class InternalTarget : Target
        {
            private PoisonFieldSpell m_Owner;

            public InternalTarget(PoisonFieldSpell owner)
                : base(12, true, TargetFlags.None)
            {
                m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is IPoint3D)
                    m_Owner.Target((IPoint3D)o);
            }

            protected override void OnTargetFinish(Mobile from)
            {
                m_Owner.FinishSequence();
            }
        }
    }
}