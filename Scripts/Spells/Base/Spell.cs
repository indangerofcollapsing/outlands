using System;
using Server.Items;
using Server.Network;
using Server.Targeting;
using Server.Mobiles;
using Server.Spells.Second;
using System.Collections.Generic;

namespace Server.Spells
{
    public abstract class Spell : ISpell
    {
        private Mobile m_Caster;
        private Item m_Scroll;
        private SpellInfo m_Info;
        private SpellState m_State;
        private DateTime m_StartCastTime;

        public SpellState State { get { return m_State; } set { m_State = value; } }
        public Mobile Caster { get { return m_Caster; } }
        public SpellInfo Info { get { return m_Info; } }
        public string Name { get { return m_Info.Name; } }
        public string Mantra { get { return m_Info.Mantra; } }
        public Type[] Reagents { get { return m_Info.Reagents; } }
        public Item Scroll { get { return m_Scroll; } }
        public DateTime StartCastTime { get { return m_StartCastTime; } }
        
        private static TimeSpan NextSpellDelay = TimeSpan.FromSeconds(1.25);
        private static TimeSpan AnimateDelay = TimeSpan.FromSeconds(1.5);

        public virtual SkillName CastSkill { get { return SkillName.Magery; } }
        public virtual SkillName DamageSkill { get { return SkillName.EvalInt; } }

        public virtual bool RevealOnCast { get { return true; } }
        public virtual bool ClearHandsOnCast { get { return true; } }
        public virtual bool ShowHandMovement { get { return true; } }         

        private static Dictionary<Type, DelayedDamageContextWrapper> m_ContextTable = new Dictionary<Type, DelayedDamageContextWrapper>();

        private class DelayedDamageContextWrapper
        {
            private Dictionary<Mobile, Timer> m_Contexts = new Dictionary<Mobile, Timer>();

            public void Add(Mobile m, Timer t)
            {
                Timer oldTimer;

                if (m_Contexts.TryGetValue(m, out oldTimer))
                {
                    oldTimer.Stop();
                    m_Contexts.Remove(m);
                }

                m_Contexts.Add(m, t);
            }

            public void Remove(Mobile m)
            {
                m_Contexts.Remove(m);
            }
        }

        public void StartDelayedDamageContext(Mobile m, Timer t)
        {
            DelayedDamageContextWrapper contexts;

            if (!m_ContextTable.TryGetValue(GetType(), out contexts))
            {
                contexts = new DelayedDamageContextWrapper();
                m_ContextTable.Add(GetType(), contexts);
            }

            contexts.Add(m, t);
        }

        public void RemoveDelayedDamageContext(Mobile m)
        {
            DelayedDamageContextWrapper contexts;

            if (!m_ContextTable.TryGetValue(GetType(), out contexts))
                return;

            contexts.Remove(m);
        }

        public void HarmfulSpell(Mobile m)
        {
            if (m is BaseCreature)
                ((BaseCreature)m).OnHarmfulSpell(m_Caster);
        }

        public Spell(Mobile caster, Item scroll, SpellInfo info)
        {
            m_Caster = caster;
            m_Scroll = scroll;
            m_Info = info;
        }

        public virtual bool IsCasting { get { return m_State == SpellState.Casting; } }

        public virtual void OnCasterHurt()
        {
            if (!Caster.Player)
                return;

            if (IsCasting)            
                Disturb(DisturbType.Hurt, false, true);            
        }

        public virtual void OnCasterKilled()
        {
            Disturb(DisturbType.Kill);
        }

        public virtual void OnConnectionChanged()
        {
            FinishSequence();
        }

        public virtual bool OnCasterMoving(Direction d)
        {
            if (IsCasting && BlocksMovement)
            {
                m_Caster.SendLocalizedMessage(500111); // You are frozen and can not move.
                return false;
            }

            return true;
        }

        public virtual bool OnCasterEquiping(Item item)
        {
            if (IsCasting)
                Disturb(DisturbType.EquipRequest);

            return true;
        }

        public virtual bool OnCasterUsingObject(object o)
        {
            if (m_State == SpellState.Sequencing)
                Disturb(DisturbType.UseRequest);

            return true;
        }

        public virtual bool OnCastInTown(Region r)
        {
            return true;
        }

        public virtual bool ConsumeReagents()
        {
            if (m_Scroll != null || !m_Caster.Player)
                return true;
            
            if (Engines.ConPVP.DuelContext.IsFreeConsume(m_Caster))
                return true;

            Container pack = m_Caster.Backpack;

            if (pack == null)
                return false;

            if (pack.ConsumeTotal(m_Info.Reagents, m_Info.Amounts) == -1)
                return true;

            return false;
        }

        public virtual double GetInscribeSkill(Mobile m)
        {
            return m.Skills[SkillName.Inscribe].Value;
        }

        public virtual int GetInscribeFixed(Mobile m)
        {
            return m.Skills[SkillName.Inscribe].Fixed;
        }

        public virtual int GetDamageFixed(Mobile m)
        {
            m.CheckSkill(DamageSkill, 0.0, 100.0, 1.0);

            return m.Skills[DamageSkill].Fixed;
        }

        public virtual double GetDamageSkill(Mobile m)
        {
            m.CheckSkill(DamageSkill, 0.0, 100.0, 1.0);

            return m.Skills[DamageSkill].Value;
        }

        public virtual double GetResistSkill(Mobile m)
        {
            return m.Skills[SkillName.MagicResist].Value;
        }

        public virtual double GetDamageScalar(Mobile target, double damageBonus)
        {   
            PlayerMobile pm_Caster = m_Caster as PlayerMobile;
            BaseCreature bc_Caster = m_Caster as BaseCreature;

            PlayerMobile pm_Target = target as PlayerMobile;            
            BaseCreature bc_Target = target as BaseCreature;

            bool PlayerCaster = false;
            bool PlayerTarget = false;

            bool CreatureCaster = false;           
            bool CreatureTarget = false;

            bool TamedCaster = false;
            bool TamedTarget = false;

            if (pm_Caster != null)
                PlayerCaster = true;

            if (bc_Caster != null)
                CreatureCaster = true;

            if (pm_Target != null)
                PlayerTarget = true;

            if (bc_Target != null)
                CreatureTarget = true;
            
            if (CreatureCaster)
            {
                if (bc_Caster.Controlled && bc_Caster.ControlMaster != null)
                {
                    if (bc_Caster.ControlMaster is PlayerMobile)
                        TamedCaster = true;
                }
            }
           
            if (CreatureTarget)
            {
                if (bc_Target.Controlled && bc_Target.ControlMaster != null)
                {
                    if (bc_Target.ControlMaster is PlayerMobile)
                        TamedTarget = true;
                }
            }

            DungeonArmor.PlayerDungeonArmorProfile casterDungeonArmor = new DungeonArmor.PlayerDungeonArmorProfile(m_Caster, null);
            DungeonArmor.PlayerDungeonArmorProfile targetDungeonArmor = new DungeonArmor.PlayerDungeonArmorProfile(target, null);

            double damageScalar = .75;

            double evalIntBonus = (m_Caster.Skills[SkillName.EvalInt].Value / 100) * .5;
            double spiritSpeakBonus = (m_Caster.Skills[SkillName.SpiritSpeak].Value / 100) * .2;
            double inscriptionBonus = (m_Caster.Skills[SkillName.Inscribe].Value / 100) * .1;
            double slayerBonus = GetSlayerDamageBonus(target); 
            double spellDamageInflictedBonus = 0;
            double spellDamageReceivedBonus = 0;                       
                       
            if (casterDungeonArmor.MatchingSet && !casterDungeonArmor.InPlayerCombat)
                spellDamageInflictedBonus = casterDungeonArmor.DungeonArmorDetail.SpellDamageInflictedBonus;

            if (targetDungeonArmor.MatchingSet && !targetDungeonArmor.InPlayerCombat)
                spellDamageReceivedBonus = targetDungeonArmor.DungeonArmorDetail.SpellDamageReceivedBonus; 

            m_Caster.CheckSkill(SkillName.SpiritSpeak, 0.0, 120.0, 1.0);

            double targetMagicResist = target.Skills[SkillName.MagicResist].Value;      
            double magicResistBonus = target.GetSpecialAbilityEntryValue(SpecialAbilityEffect.MagicResist);
            
            //Player vs Player
            if (m_Caster is PlayerMobile && target is PlayerMobile)
            {
                evalIntBonus *= .5;
                spiritSpeakBonus *= .5;
                inscriptionBonus *= .5;

                slayerBonus = 0;     
                spellDamageInflictedBonus = 0;
                damageBonus = 0;

                spellDamageReceivedBonus = 0;

                magicResistBonus = 0;
            }

            damageScalar += evalIntBonus + spiritSpeakBonus + inscriptionBonus + slayerBonus + spellDamageInflictedBonus + damageBonus - spellDamageReceivedBonus;

            targetMagicResist += magicResistBonus;

            //Player Caster
            if (PlayerCaster)
            {   
                //Player Target
                if (PlayerTarget)
                    damageScalar *= 1.0;

                //Tamed Target
                else if (TamedTarget)                
                    damageScalar *= 2.0;                

                //Creature Target
                else
                    damageScalar *= 2.0;
            }

            //Tamed Creature Casting
            else if (TamedCaster)
            {
                //Player Target
                if (PlayerTarget)
                    damageScalar *= 1.0;

                //Tamed Target
                else if (TamedTarget)
                    damageScalar *= 2.0;

                //Creature Target
                else
                    damageScalar *= 2.0;             
            }

            else //Normal Creature Casting
            {
                //Player Target
                if (PlayerTarget)
                    damageScalar *= 1;

                //Tamed Target
                else if (TamedTarget)
                    damageScalar *= 2.0;

                //Creature Target
                else
                    damageScalar *= 2.0;            
            }     

            if (CreatureTarget)
                bc_Target.AlterDamageScalarFrom(m_Caster, ref damageScalar);
                      
            if (CreatureCaster)
                bc_Caster.AlterDamageScalarTo(target, ref damageScalar);           

            target.Region.SpellDamageScalar(m_Caster, target, ref damageScalar);

            //Magic Resist
            double minDamageReduction = (targetMagicResist * .125) / 100;
            double maxDamageReduction = (targetMagicResist * .25) / 100;

            double magicResistScalar = 1 - (minDamageReduction + ((maxDamageReduction - minDamageReduction) * Utility.RandomDouble()));
            
            damageScalar *= magicResistScalar;

            if (damageScalar < 0)
                damageScalar = 0;

            return damageScalar;
        }
        
        public virtual double GetSlayerDamageBonus(Mobile defender)
        {
            double slayerBonus = 0;

            PlayerMobile pm = m_Caster as PlayerMobile;

            if (pm == null)
                return slayerBonus;            

            Spellbook atkBook;
            BaseCreature bc_Defender = defender as BaseCreature;

            //Player Has a Bound Enhanced Spellbook
            if (pm.FindItemOnLayer(Layer.OneHanded) is EnhancedSpellbook)
            {
                EnhancedSpellbook enhancedSpellbook = pm.FindItemOnLayer(Layer.OneHanded) as EnhancedSpellbook;

                //Enhanced Spellbook is Currently Equipped on Player and is Slayer Type
                if (enhancedSpellbook.EnhancedType == EnhancedSpellbookType.Slayer && bc_Defender != null)
                {
                    if (enhancedSpellbook.SlayerGroup == bc_Defender.SlayerGroup)
                    {
                        defender.FixedEffect(0x37B9, 10, 5);

                        Boolean isTamedTarget = SpellHelper.IsTamedTarget(m_Caster, defender);

                        if (isTamedTarget)
                            slayerBonus = SpellHelper.SlayerTamedBonus;

                        else
                            slayerBonus = SpellHelper.SlayerBonus;

                        enhancedSpellbook.OnSpellCast(m_Caster);
                    }
                }

                else                                   
                    return slayerBonus;                
            }

            else
                return slayerBonus;            

            return slayerBonus;
        }

        public virtual void DoFizzle()
        {
            m_Caster.LocalOverheadMessage(MessageType.Regular, 0x3B2, 502632); // The spell fizzles.

            if (m_Caster.Player)
            {               
                m_Caster.FixedEffect(0x3735, 6, 30);
                m_Caster.PlaySound(0x5C);
            }
        }

        private CastTimer m_CastTimer;
        private AnimTimer m_AnimTimer;

        public void Disturb(DisturbType type)
        {
            Disturb(type, true, false);
        }

        public virtual bool CheckDisturb(DisturbType type, bool firstCircle, bool resistable)
        {
            if (resistable && m_Scroll is BaseWand)
                return false;

            return true;
        }

        public void Disturb(DisturbType type, bool firstCircle, bool resistable)
        {
            if (!CheckDisturb(type, firstCircle, resistable))
                return;

            if (m_State == SpellState.Casting)
            {
                if (!firstCircle && !Core.AOS && this is MagerySpell && ((MagerySpell)this).Circle == SpellCircle.First)
                    return;

                m_State = SpellState.None;
                m_Caster.Spell = null;

                OnDisturb(type, true);

                if (m_CastTimer != null)
                    m_CastTimer.Stop();

                if (m_AnimTimer != null)
                    m_AnimTimer.Stop();
                
                m_Caster.NextSpellTime = DateTime.UtcNow + TimeSpan.FromMilliseconds(GetDisturbRecovery().TotalMilliseconds);
            }

            else if (m_State == SpellState.Sequencing)
            {
                if (!firstCircle && !Core.AOS && this is MagerySpell && ((MagerySpell)this).Circle == SpellCircle.First)
                    return;

                m_State = SpellState.None;
                m_Caster.Spell = null;

                OnDisturb(type, false);

                Targeting.Target.Cancel(m_Caster);
            }
        }

        public virtual void DoHurtFizzle()
        {
            m_Caster.FixedEffect(0x3735, 6, 30);
            m_Caster.PlaySound(0x5C);
        }

        public virtual void OnDisturb(DisturbType type, bool message)
        {
            if (message)
                m_Caster.SendLocalizedMessage(500641); // Your concentration is disturbed, thus ruining thy spell.
        }

        public virtual bool CheckCast()
        {
            return true;
        }

        public virtual void SayMantra()
        {
            if (m_Scroll is BaseWand)
                return;

            bool no_LOS_check = false;

            if (m_Info.Mantra != null && m_Info.Mantra.Length > 0 && (m_Caster.Player))
                m_Caster.PublicOverheadMessage(MessageType.Spell, m_Caster.SpeechHue, true, m_Info.Mantra, no_LOS_check);
        }

        public virtual bool BlockedByHorrificBeast { get { return true; } }
        public virtual bool BlockedByAnimalForm { get { return true; } }
        public virtual bool BlocksMovement { get { return true; } }

        public virtual bool CheckNextSpellTime { get { return !(m_Scroll is BaseWand); } }

        public bool Cast()
        {
            m_StartCastTime = DateTime.UtcNow;
 
            if (m_Caster.Player)
            {
                Item item = m_Caster.FindItemOnLayer(Layer.OneHanded);

                if (item != null && !(item is Spellbook) && !(item is Runebook) && !(item is BaseWand))
                {
                    m_Caster.SendLocalizedMessage(502626); // Your hands must be free to cast spells or meditate
                    return false;
                }

                item = m_Caster.FindItemOnLayer(Layer.TwoHanded);

                if (item != null && !(item is Spellbook) && !(item is Runebook))
                {
                    m_Caster.SendLocalizedMessage(502626); // Your hands must be free to cast spells or meditate
                    return false;
                }
            }

            if (!m_Caster.CheckAlive())            
                return false;            

            else if (m_Scroll is BaseWand && m_Caster.Spell != null && m_Caster.Spell.IsCasting)            
                m_Caster.SendLocalizedMessage(502643); // You can not cast a spell while frozen.            

            else if (m_Caster.Spell != null && m_Caster.Spell.IsCasting)            
                m_Caster.SendLocalizedMessage(502642); // You are already casting a spell.            

            else if (!(m_Scroll is BaseWand) && (m_Caster.Paralyzed || m_Caster.Frozen))            
                m_Caster.SendLocalizedMessage(502643); // You can not cast a spell while frozen.            

            else if (CheckNextSpellTime && DateTime.UtcNow < m_Caster.NextSpellTime)            
                m_Caster.SendLocalizedMessage(502644); // You have not yet recovered from casting a spell.            

            #region Dueling

            else if (m_Caster is PlayerMobile && ((PlayerMobile)m_Caster).DuelContext != null && !((PlayerMobile)m_Caster).DuelContext.AllowSpellCast(m_Caster, this))
            {
            }

            #endregion

            else if (m_Caster.Mana >= ScaleMana(GetMana()))
            {
                if (m_Caster.Spell == null && m_Caster.CheckSpellCast(this) && CheckCast() && m_Caster.Region.OnBeginSpellCast(m_Caster, this))
                {
                    m_State = SpellState.Casting;
                    m_Caster.Spell = this;

                    if (RevealOnCast)
                        m_Caster.RevealingAction();

                    SayMantra();

                    TimeSpan castDelay = this.GetCastDelay();

                    int count = (int)Math.Ceiling(castDelay.TotalSeconds / AnimateDelay.TotalSeconds);

                    if (ShowHandMovement && m_Caster.Body.IsHuman && m_Caster.Player)
                    {
                        if (count != 0)
                        {
                            m_AnimTimer = new AnimTimer(this, count);
                            m_AnimTimer.Start();
                        }

                        if (m_Info.LeftHandEffect > 0)
                            Caster.FixedParticles(0, 10, 5, m_Info.LeftHandEffect, EffectLayer.LeftHand);

                        if (m_Info.RightHandEffect > 0)
                            Caster.FixedParticles(0, 10, 5, m_Info.RightHandEffect, EffectLayer.RightHand);
                    }

                    //Creature Casting
                    else if (m_Caster is BaseCreature)
                    {
                        if (count != 0)
                        {
                            BaseCreature creature = m_Caster as BaseCreature;

                            //Animation Duration
                            double castDelayInSeconds = castDelay.TotalSeconds;
                            double spellSpeedSCalar = creature.SpellSpeedScalar;

                            castDelay = castDelay + TimeSpan.FromSeconds(castDelayInSeconds * .4 * spellSpeedSCalar);

                            count = (int)Math.Ceiling((castDelay.TotalSeconds) / AnimateDelay.TotalSeconds);

                            if (count > 0)
                            {
                                m_AnimTimer = new AnimTimer(this, count);
                                m_AnimTimer.Start();
                            }

                            //Next Spellcast Allowed Delay
                            double speedScalar = 1 + creature.GetSpecialAbilityEntryValue(SpecialAbilityEffect.Cripple);

                            double mageryModifier = 1.0;
                            double experienceModifier = 1.0;

                            if (creature.Controlled && creature.ControlMaster is PlayerMobile)
                            {
                                mageryModifier = 1 / creature.TamedBaseMageryCreationScalar;
                                experienceModifier = 1 - (.20 * (double)creature.Experience / (double)creature.MaxExperience);                               
                            }
                            
                            double SpellDelay = (creature.SpellDelayMin + ((creature.SpellDelayMax - creature.SpellDelayMin) * Utility.RandomDouble())) * speedScalar * mageryModifier * experienceModifier;

                            if (SpellDelay < .1)
                                SpellDelay = .1;

                            creature.NextSpellTime = DateTime.UtcNow + TimeSpan.FromSeconds(castDelay.TotalSeconds) + TimeSpan.FromSeconds(SpellDelay);
                        }
                    }

                    //Players Clear Hands / NPCs keep weapons equipped when casting
                    if (ClearHandsOnCast && m_Caster is PlayerMobile)                    
                        m_Caster.ClearHands();                    
                    
                    //Increase SwingDelay for BaseCreatures While Casting
                    if (m_Caster is BaseCreature)
                    {
                        BaseWeapon weapon = m_Caster.Weapon as BaseWeapon;

                        if (weapon != null)                        
                            m_Caster.LastSwingTime = DateTime.UtcNow + TimeSpan.FromSeconds((double)count);                        
                    }

                    m_CastTimer = new CastTimer(this, castDelay);

                    OnBeginCast();

                    if (castDelay > TimeSpan.Zero)                    
                        m_CastTimer.Start();                    

                    else                    
                        m_CastTimer.Tick();                    

                    return true;
                }

                else                
                    return false;                
            }

            else            
                m_Caster.LocalOverheadMessage(MessageType.Regular, 0x22, 502625); // Insufficient mana            

            return false;
        }

        public abstract void OnCast();

        public virtual void OnBeginCast()
        {
        }

        public virtual void GetCastSkills(out double min, out double max)
        {
            min = max = 0;
        }

        public virtual bool CheckFizzle()
        {
            if (m_Scroll is BaseWand)
                return true;

            double minSkill, maxSkill;

            GetCastSkills(out minSkill, out maxSkill);

            if (Caster is BaseCreature)
                return true;

            return Caster.CheckSkill(CastSkill, minSkill, maxSkill, 1.0);
        }

        public abstract int GetMana();

        public virtual int ScaleMana(int mana)
        {
            double scalar = 1.0;
            
            scalar -= (double)AosAttributes.GetValue(m_Caster, AosAttribute.LowerManaCost) / 100;

            return (int)(mana * scalar);
        }

        public virtual TimeSpan GetDisturbRecovery()
        {
            double secs_since_cast = (DateTime.UtcNow - m_StartCastTime).TotalSeconds;
            double delay = 1.0 - Math.Sqrt(secs_since_cast / GetCastDelay().TotalSeconds);

            if (delay < 0.35)
                delay = 0.35;

            return TimeSpan.FromSeconds(delay);
        }

        public virtual int CastRecoveryBase { get { return 6; } }
        public virtual int CastRecoveryFastScalar { get { return 1; } }
        public virtual int CastRecoveryPerSecond { get { return 4; } }
        public virtual int CastRecoveryMinimum { get { return 0; } }

        public virtual TimeSpan GetCastRecovery()
        {           
            return NextSpellDelay;
        }

        public abstract TimeSpan CastDelayBase { get; }

        public virtual double CastDelayFastScalar { get { return 1; } }
        public virtual double CastDelaySecondsPerTick { get { return 0.25; } }
        public virtual TimeSpan CastDelayMinimum { get { return TimeSpan.FromSeconds(0.25); } }

        public virtual TimeSpan GetCastDelay()
        {
            if (m_Scroll is BaseWand)
                return TimeSpan.Zero;

            int fcMax = 2;
            
            int fc = AosAttributes.GetValue(m_Caster, AosAttribute.CastSpeed);

            if (fc > fcMax)
                fc = fcMax;

            if (ProtectionSpell.Registry.Contains(m_Caster))
                fc -= 2;

            TimeSpan baseDelay = CastDelayBase;

            TimeSpan fcDelay = TimeSpan.FromSeconds(-(CastDelayFastScalar * fc * CastDelaySecondsPerTick));
                       
            TimeSpan delay = baseDelay + fcDelay;

            if (delay < CastDelayMinimum)
                delay = CastDelayMinimum;

            return delay;
        }

        public virtual void FinishSequence()
        {
            if (m_Caster is BaseCreature)
            {
            }

            m_State = SpellState.None;

            if (m_Caster.Spell == this)
                m_Caster.Spell = null;
        }

        public virtual int ComputeKarmaAward()
        {
            return 0;
        }

        public virtual bool CheckSequence()
        {
            int mana = ScaleMana(GetMana());

            if (m_Caster.Player)
            {
                Item item = m_Caster.FindItemOnLayer(Layer.OneHanded);

                if (item != null && !(item is Spellbook) && !(item is Runebook))
                {
                    m_Caster.SendLocalizedMessage(502626); // Your hands must be free to cast spells or meditate
                    DoFizzle();

                    return false;
                }

                item = m_Caster.FindItemOnLayer(Layer.TwoHanded);

                if (item != null && !(item is Spellbook) && !(item is Runebook))
                {
                    m_Caster.SendLocalizedMessage(502626); // Your hands must be free to cast spells or meditate
                    DoFizzle();

                    return false;
                }
            }

            if (m_Caster.Deleted || !m_Caster.Alive || m_Caster.Spell != this || m_State != SpellState.Sequencing)            
                DoFizzle();            

            else if (m_Scroll != null && !(m_Scroll is Runebook) && (m_Scroll.Amount <= 0 || m_Scroll.Deleted || m_Scroll.RootParent != m_Caster || (m_Scroll is BaseWand && (((BaseWand)m_Scroll).Charges <= 0 || m_Scroll.Parent != m_Caster))))
                DoFizzle();            

            else if (!ConsumeReagents())            
                m_Caster.LocalOverheadMessage(MessageType.Regular, 0x22, 502630); // More reagents are needed for this spell.            

            else if (m_Caster.Mana < mana)            
                m_Caster.LocalOverheadMessage(MessageType.Regular, 0x22, 502625); // Insufficient mana for this spell.            
                
            else if (CheckFizzle())
            {
                //Backlash Effect: Potential to Cancel Spell
                double totalValue = 0;

                BaseCreature bc_Creature = m_Caster as BaseCreature;
                PlayerMobile player = m_Caster as PlayerMobile;

                double backlashValue = m_Caster.GetSpecialAbilityEntryValue(SpecialAbilityEffect.Backlash);
               
                if (Utility.RandomDouble() <= backlashValue)
                {
                    DoFizzle();
                    m_Caster.SendMessage("Backlash causes your spell to fizzle."); 

                    return false;
                }                

                DungeonArmor.PlayerDungeonArmorProfile casterDungeonArmor = new DungeonArmor.PlayerDungeonArmorProfile(m_Caster, null);

                if (casterDungeonArmor.MatchingSet && !casterDungeonArmor.InPlayerCombat)
                {
                    if (Utility.RandomDouble() <= casterDungeonArmor.DungeonArmorDetail.ReducedSpellManaCostChance)
                    {
                        mana = (int)(Math.Round((double)mana * casterDungeonArmor.DungeonArmorDetail.ReducedSpellManaCostScalar));
                        Caster.SendMessage("You feel a rush of energy from your armor, fueling mana into the spell.");

                        Effects.PlaySound(Caster.Location, Caster.Map, 0x64B);
                        Effects.SendLocationParticles(EffectItem.Create(Caster.Location, Caster.Map, EffectItem.DefaultDuration), 0x376A, 9, 32, casterDungeonArmor.DungeonArmorDetail.EffectHue, 0, 5005, 0);
                    }                    
                }

                m_Caster.Mana -= mana;

                if (m_Scroll is SpellScroll)
                    m_Scroll.Consume();

                else if (m_Scroll is BaseWand)
                {
                    ((BaseWand)m_Scroll).ConsumeCharge(m_Caster);
                    m_Caster.RevealingAction();
                }

                if (m_Scroll is BaseWand)
                {
                    bool m = m_Scroll.Movable;

                    m_Scroll.Movable = false;

                    if (ClearHandsOnCast && m_Caster is PlayerMobile)
                        m_Caster.ClearHands();

                    m_Scroll.Movable = m;
                }

                else
                {
                    if (ClearHandsOnCast && m_Caster is PlayerMobile)
                        m_Caster.ClearHands();
                }

                int karma = ComputeKarmaAward();

                if (karma != 0)
                    Misc.FameKarmaTitles.AwardKarma(Caster, karma, true);

                return true;
            }

            else            
                DoFizzle();            

            return false;
        }

        public bool CheckBSequence(Mobile target)
        {
            return CheckBSequence(target, false);
        }

        public bool CheckBSequence(Mobile target, bool allowDead)
        {
            if (!target.Alive && !allowDead)
            {
                m_Caster.SendLocalizedMessage(501857); // This spell won't work on that!
                return false;
            }

            else if (Caster.CanBeBeneficial(target, true, allowDead) && CheckSequence())
            {
                Caster.DoBeneficial(target);
                return true;
            }

            else            
                return false;            
        }

        public bool CheckHSequence(Mobile target)
        {
            //Prevent Spells Following Targets Long Distance
            if (Caster is BaseCreature && Caster != target && target != null && Name != "Explosion")
            {
                if (Utility.GetDistance(Caster.Location, target.Location) > 30)
                    return false;
            }

            if (!target.Alive)
            {
                m_Caster.SendLocalizedMessage(501857); // This spell won't work on that!
                return false;
            }

            else if (Caster.CanBeHarmful(target) && CheckSequence())
            {               
                Caster.DoHarmful(target);
                return true;
            }

            else            
                return false;            
        }

        private class AnimTimer : Timer
        {
            private Spell m_Spell;

            public AnimTimer(Spell spell, int count): base(TimeSpan.Zero, AnimateDelay, count)
            {
                m_Spell = spell;

                Priority = TimerPriority.FiftyMS;
            }

            protected override void OnTick()
            {
                if (m_Spell.State != SpellState.Casting || m_Spell.m_Caster.Spell != m_Spell)
                {
                    Stop();
                    return;
                }

                if (!m_Spell.Caster.Mounted && m_Spell.Caster.Body.IsHuman && m_Spell.m_Info.Action >= 0)
                    m_Spell.Caster.Animate(m_Spell.m_Info.Action, 7, 1, true, false, 0);

                else if (m_Spell.Caster is BaseCreature && m_Spell.m_Info.Action >= 0)
                {
                    BaseCreature creature = m_Spell.Caster as BaseCreature;

                    CastingAnimationInfo.GetCastAnimationForBody(creature);

                    m_Spell.Caster.Animate(creature.SpellCastAnimation, creature.SpellCastFrameCount, 1, true, false, 0);
                }

                if (!Running)
                    m_Spell.m_AnimTimer = null;
            }
        }

        private class CastTimer : Timer
        {
            private Spell m_Spell;

            public CastTimer(Spell spell, TimeSpan castDelay): base(castDelay)
            {
                m_Spell = spell;
                Priority = TimerPriority.TwentyFiveMS;
            }

            protected override void OnTick()
            {
                if (m_Spell == null || m_Spell.m_Caster == null)                
                    return;                

                else if (m_Spell.m_State == SpellState.Casting && m_Spell.m_Caster.Spell == m_Spell)
                {
                    m_Spell.m_State = SpellState.Sequencing;
                    m_Spell.m_CastTimer = null;
                    m_Spell.m_Caster.OnSpellCast(m_Spell);

                    if (m_Spell.m_Caster.Region != null)
                        m_Spell.m_Caster.Region.OnSpellCast(m_Spell.m_Caster, m_Spell);

                    if (!(m_Spell.m_Caster is BaseCreature))
                        m_Spell.m_Caster.NextSpellTime = DateTime.UtcNow + TimeSpan.FromMilliseconds(250);

                    Target originalTarget = m_Spell.m_Caster.Target;                    

                    m_Spell.OnCast();

                    if (m_Spell.m_Caster.Player && m_Spell.m_Caster.Target != originalTarget && m_Spell.Caster.Target != null)
                        m_Spell.m_Caster.Target.BeginTimeout(m_Spell.m_Caster, TimeSpan.FromSeconds(30.0));

                    m_Spell.m_CastTimer = null;
                }
            }

            public void Tick()
            {
                OnTick();
            }
        }
    }    
}