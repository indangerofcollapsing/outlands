using System;
using Server.Items;
using Server.Targeting;
using System.Collections;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;
using Server.Spells;

namespace Server.Mobiles
{
    [CorpseName("almonjin's corpse")]
    public class KhaldunLichAlmonjin : BaseCreature
    {
        public DateTime m_NextAIChangeAllowed;
        public TimeSpan NextAIChangeDelay = TimeSpan.FromSeconds(30);

        public DateTime m_NextSpeechAllowed;
        public TimeSpan NextSpeechDelay = TimeSpan.FromSeconds(30);

        public DateTime m_NextRevealAllowed;
        public TimeSpan NextRevealDelay = TimeSpan.FromSeconds(5);

        public int damageIntervalThreshold = 1500;
        public int damageProgress = 0;

        public int intervalCount = 0;
        public int totalIntervals = 20;

        public int m_OrbMainHue = 0;
        public int m_OrbRevealHue = 2515;
        public int m_OrbAttackHue = 2608;
        public int m_OrbIntervalAttackHue = 2608;

        public bool AbilityInProgress = false;

        public Item m_Item;

        public string[] idleSpeech
        {
            get { return new string[] { "*chants*" }; }
        }

        public string[] combatSpeech
        {
            get { return new string[] { "" }; }
        }

        [Constructable]
        public KhaldunLichAlmonjin(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Almonjin the All-Seeing Eye";

            Body = 830;
            Hue = 2653;

            BaseSoundID = 0x388;

            SetStr(100);
            SetDex(50);
            SetInt(50);

            SetHits(30000);
            SetStam(5000);
            SetMana(30000);

            SetDamage(20, 30);

            SetSkill(SkillName.Wrestling, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Magery, 150);
            SetSkill(SkillName.EvalInt, 150);
            SetSkill(SkillName.Meditation, 300);

            SetSkill(SkillName.MagicResist, 150);

            Fame = 20000;
            Karma = -20000;

            VirtualArmor = 25;

            Timer.DelayCall(TimeSpan.FromMilliseconds(50), delegate { BuildSkull(); });
        }

        public void BuildSkull()
        {
            Point3D skullLocation = new Point3D(Location.X, Location.Y, Z + 35);

            Static item = new Static(7960);

            m_Item = item;
            m_Item.Hue = m_OrbMainHue;
            m_Item.Name = "Orb of Omnipresence";           

            item.MoveToWorld(skullLocation, Map);
        }

        public override void SetUniqueAI()
        {                           
            UniqueCreatureDifficultyScalar = 1.5;

            DictCombatHealSelf[CombatHealSelf.SpellHealSelf100] = 0;
            DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 0;
            DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 0;
            DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 0;
            DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 0;

            DictCombatAction[CombatAction.CombatHealSelf] = 0;

            DictWanderAction[WanderAction.None] = 0;
            DictWanderAction[WanderAction.SpellHealSelf100] = 0;
            DictWanderAction[WanderAction.SpellCureSelf] = 0;

            ResolveAcquireTargetDelay = 0.5;
            RangePerception = 24;

            SpellHue = Hue - 1;

            damageIntervalThreshold = (int)(Math.Round((double)HitsMax / (double)totalIntervals));
            intervalCount = (int)(Math.Floor((1 - (double)Hits / (double)HitsMax) * (double)totalIntervals));
        }

        public override int PoisonResistance { get { return 5; } }

        public override bool AlwaysBoss { get { return true; } }        
        public override bool AlwaysMurderer { get { return true; } }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            damageIntervalThreshold = (int)(Math.Round((double)HitsMax / (double)totalIntervals));
            intervalCount = (int)(Math.Floor((1 - (double)Hits / (double)HitsMax) * (double)totalIntervals));
            
            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            if (!willKill)
            {
                if (from != null && amount > 10)
                {
                    BaseWeapon weapon = from.Weapon as BaseWeapon;

                    if (weapon != null)
                    {
                        //Ranged Weapon
                        if (weapon is BaseRanged)
                        {
                            if (Utility.RandomDouble() < .1)
                                DamageEffect(from);
                        }

                        //Melee Weapon
                        else if (weapon is BaseMeleeWeapon || weapon is Fists)
                        {
                            if (Utility.RandomDouble() < .05)
                                DamageEffect(from);
                        }
                    }

                    else
                    {
                        //Spell or Special Effect
                        if (Utility.RandomDouble() < .15)
                            DamageEffect(from);
                    }
                }

                damageProgress += amount;

                if (damageProgress >= damageIntervalThreshold)
                {
                    Effects.PlaySound(Location, Map, GetAngerSound());

                    damageProgress = 0;

                    IntervalEffect();
                }
            }

            base.OnDamage(amount, from, willKill);
        }

        public void DamageEffect(Mobile from)
        {
            if (AbilityInProgress)
                return;

            if (from == null) return;
            if (from.Deleted || !from.Alive) return;
            if (this == null)
            if (Deleted || !Alive) return;
            if (m_Item == null) return;
            if (m_Item.Deleted) return;

            double spawnPercent = (double)intervalCount / (double)totalIntervals;            
           
            Point3D location = m_Item.Location;
            Map map = m_Item.Map;

            int bolts = 1 + (int)(Math.Ceiling(5 * spawnPercent));
            double boltDuration = .25;

            int boltMinDamage = 5;
            int boltMaxDamage = 10;

            Animate(15, 8, 1, true, false, 0); //Staff

            for (int a = 0; a < bolts; a++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(a * boltDuration), delegate
                {
                    if (m_Item == null) return;
                    if (m_Item.Deleted) return;
                    if (from == null) return;
                    if (from.Deleted || !from.Alive) return;
                    if (Utility.GetDistance(location, from.Location) > 30) return;

                    m_Item.Hue = m_OrbAttackHue;

                    Effects.PlaySound(m_Item.Location, m_Item.Map, 0x5C3);

                    TimedStatic discharge = new TimedStatic(0x3779, .5);
                    discharge.Hue = m_OrbAttackHue;
                    discharge.Name = "dissipated energy";
                    discharge.MoveToWorld(m_Item.Location, m_Item.Map);

                    IEntity startLocation = new Entity(Serial.Zero, new Point3D(m_Item.X, m_Item.Y, m_Item.Z), m_Item.Map);
                    IEntity endLocation = new Entity(Serial.Zero, new Point3D(from.X, from.Y, from.Z + 5), from.Map);

                    int particleSpeed = 5;

                    Effects.SendMovingParticles(startLocation, endLocation, 0x3818, particleSpeed, 0, false, false, 2603, 0, 9501, 0, 0, 0x100);

                    double distance = Utility.GetDistanceToSqrt(location, from.Location);
                    double distanceDelay = (double)distance * .08;

                    Timer.DelayCall(TimeSpan.FromSeconds(distanceDelay), delegate
                    {
                        if (m_Item == null) return;
                        if (m_Item.Deleted) return;
                        if (from == null) return;
                        if (from.Deleted || !from.Alive) return;
                        if (Utility.GetDistance(location, from.Location) > 30) return;

                        int damage = Utility.RandomMinMax(boltMinDamage, boltMaxDamage);
                        double duration = Utility.RandomMinMax(2, 4);

                        if (from is BaseCreature)
                        {
                            damage *= 2;
                            duration *= 2;
                        }

                        from.FixedParticles(0x3967, 10, 40, 5036, 2603, 0, EffectLayer.CenterFeet);

                        SpecialAbilities.HinderSpecialAbility(1.0, null, from, 1.0, duration, false, -1, false, "", "You have been shocked!", "-1");

                        new Blood().MoveToWorld(from.Location, from.Map);
                        AOS.Damage(from, damage, 0, 100, 0, 0, 0);

                    });
                });
            }

            Timer.DelayCall(TimeSpan.FromSeconds(bolts * boltDuration), delegate
            {
                if (m_Item == null) return;
                if (m_Item.Deleted) return;

                m_Item.Hue = m_OrbMainHue;
            });
        }

        public void IntervalEffect()
        {           
            if (this == null)
                if (Deleted || !Alive) return;
            if (m_Item == null) return;
            if (m_Item.Deleted) return;

            AbilityInProgress = true;

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            int range = 24;
            int cycles = 10 + (int)(Math.Ceiling(50 * spawnPercent));
            int loops = (int)(Math.Ceiling((double)cycles / 10));

            double stationaryDelay = loops + 1;

            int boltMinDamage = 5;
            int boltMaxDamage = 15;

            m_Item.Hue = m_OrbIntervalAttackHue;            
            
            Combatant = null;
            NextDecisionTime = DateTime.UtcNow + TimeSpan.FromSeconds(stationaryDelay);

            AIObject.NextMove = AIObject.NextMove + TimeSpan.FromSeconds(stationaryDelay);
            LastSwingTime = LastSwingTime + TimeSpan.FromSeconds(stationaryDelay);
            NextSpellTime = NextSpellTime + TimeSpan.FromSeconds(stationaryDelay);
            NextCombatHealActionAllowed = NextCombatHealActionAllowed + TimeSpan.FromSeconds(stationaryDelay);
            NextCombatSpecialActionAllowed = NextCombatSpecialActionAllowed + TimeSpan.FromSeconds(stationaryDelay);
            NextCombatEpicActionAllowed = NextCombatEpicActionAllowed + TimeSpan.FromSeconds(stationaryDelay);

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1, stationaryDelay, true, 0, false, "", "", "-1");            

            Point3D location = Location;
            Map map = Map;

            for (int a = 0; a < loops; a++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(a * 1), delegate
                {
                    if (this == null) return;
                    if (Deleted || !Alive) return;

                    CantWalk = true;
                    Frozen = true;

                    Animate(12, 12, 1, true, false, 0);

                    PlaySound(GetAngerSound());
                });
            }

            Timer.DelayCall(TimeSpan.FromSeconds(stationaryDelay), delegate
            {
                if (this == null) return;
                if (Deleted || !Alive) return;

                m_Item.Hue = m_OrbMainHue;

                CantWalk = false;
                Frozen = false;

                AbilityInProgress = false;

                PlaySound(0x211);                
            });

            for (int a = 0; a < cycles; a++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(a * .06), delegate
                {
                    if (this == null) return;
                    if (Deleted || !Alive) return;
                    if (m_Item == null) return;
                    if (m_Item.Deleted) return;

                    List<Mobile> m_ValidMobiles = new List<Mobile>();

                    IPooledEnumerable mobilesInRange = map.GetMobilesInRange(location, range);

                    foreach (Mobile mobile in mobilesInRange)
                    {
                        if (mobile == null) continue;
                        if (mobile == this) continue;
                        if (!mobile.CanBeDamaged()) continue;
                        if (mobile.AccessLevel > AccessLevel.Player) continue;

                        bool validTarget = false;

                        PlayerMobile pm_Target = mobile as PlayerMobile;
                        BaseCreature bc_Target = mobile as BaseCreature;

                        if (pm_Target != null)
                            validTarget = true;

                        if (bc_Target != null)
                        {
                            if (bc_Target.Controlled && bc_Target.ControlMaster is PlayerMobile)
                                validTarget = true;
                        }

                        if (validTarget)
                            m_ValidMobiles.Add(mobile);
                    }

                    mobilesInRange.Free();

                    if (m_ValidMobiles.Count > 0)
                    {
                        Mobile target = m_ValidMobiles[Utility.RandomMinMax(0, m_ValidMobiles.Count - 1)];

                        m_Item.Hue = m_OrbIntervalAttackHue;

                        Effects.PlaySound(m_Item.Location, m_Item.Map, 0x211);

                        TimedStatic discharge = new TimedStatic(0x3779, .5);
                        discharge.Hue = m_OrbIntervalAttackHue;
                        discharge.Name = "dissipated energy";
                        discharge.MoveToWorld(m_Item.Location, m_Item.Map);

                        IEntity startLocation = new Entity(Serial.Zero, new Point3D(m_Item.X, m_Item.Y, m_Item.Z), m_Item.Map);
                        IEntity endLocation = new Entity(Serial.Zero, new Point3D(target.X, target.Y, target.Z + 5), target.Map);

                        int particleSpeed = 5;

                        Effects.SendMovingParticles(startLocation, endLocation, 0x3818, particleSpeed, 0, false, false, 2603, 0, 9501, 0, 0, 0x100);

                        double distance = Utility.GetDistanceToSqrt(location, target.Location);
                        double distanceDelay = (double)distance * .08;

                        Timer.DelayCall(TimeSpan.FromSeconds(distanceDelay), delegate
                        {
                            if (m_Item == null) return;
                            if (m_Item.Deleted) return;
                            if (target == null) return;
                            if (target.Deleted || !target.Alive) return;
                            if (Utility.GetDistance(location, target.Location) > 30) return;

                            int damage = Utility.RandomMinMax(boltMinDamage, boltMaxDamage);
                            double duration = Utility.RandomMinMax(2, 4);

                            if (target is BaseCreature)
                            {
                                damage *= 2;
                                duration *= 2;
                            }

                            target.FixedParticles(0x3967, 10, 40, 5036, 2603, 0, EffectLayer.CenterFeet);

                            SpecialAbilities.HinderSpecialAbility(1.0, null, target, 1.0, duration, false, -1, false, "", "You have been shocked!", "-1");

                            new Blood().MoveToWorld(target.Location, target.Map);
                            AOS.Damage(target, damage, 0, 100, 0, 0, 0);
                        });
                    }
                });
            }     
        }

        public override void OnThink()
        {
            base.OnThink();

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            //Reveal Hidden)
            if (m_NextRevealAllowed <= DateTime.UtcNow)
            {
                Queue m_Queue = new Queue();

                IPooledEnumerable nearbyMobiles = Map.GetMobilesInRange(Location, 30);

                bool creatureWasRevealed = false;

                foreach (Mobile mobile in nearbyMobiles)
                {
                    if (mobile.Deleted) continue;
                    if (!mobile.Alive) continue;
                    if (mobile.AccessLevel > AccessLevel.Player) continue;
                    if (!mobile.Hidden) continue;

                    bool validMobile = false;

                    BaseCreature bc_Creature = mobile as BaseCreature;
                    PlayerMobile pm_Player = mobile as PlayerMobile;

                    if (bc_Creature != null)
                    {
                        if (bc_Creature.Controlled && bc_Creature.ControlMaster is PlayerMobile)
                            validMobile = true;
                    }

                    if (pm_Player != null)
                        validMobile = true;

                    if (validMobile)
                        m_Queue.Enqueue(mobile);
                }

                nearbyMobiles.Free();

                while (m_Queue.Count > 0)
                {
                    Mobile mobile = (Mobile)m_Queue.Dequeue();

                    creatureWasRevealed = true;    

                    mobile.RevealingAction();
                    mobile.SendMessage("You have been revealed by Almonjin!");

                    Effects.PlaySound(mobile.Location, mobile.Map, 0x58F);
                    mobile.FixedParticles(0x376A, 10, 20, 5036, m_OrbRevealHue - 1, 0, EffectLayer.CenterFeet);                    

                    int damage = DamageMax;

                    if (mobile is BaseCreature)
                        damage *= 2;

                    new Blood().MoveToWorld(mobile.Location, mobile.Map);
                    AOS.Damage(mobile, damage, 0, 100, 0, 0, 0);
                }

                if (creatureWasRevealed)
                {
                    if (m_Item != null)
                    {
                        if (!m_Item.Deleted)
                        {
                            TimedStatic discharge = new TimedStatic(0x3779, .5);
                            discharge.Hue = m_OrbRevealHue;
                            discharge.Name = "dissipated energy";
                            discharge.MoveToWorld(m_Item.Location, m_Item.Map);

                            m_Item.PublicOverheadMessage(MessageType.Regular, 0x482, false, "The Eye Sees All");
                            m_Item.Hue = m_OrbRevealHue;

                            Timer.DelayCall(TimeSpan.FromSeconds(1.0), delegate
                            {
                                if (m_Item == null) return;
                                if (m_Item.Deleted) return;

                                m_Item.Hue = m_OrbMainHue;
                            });
                        }
                    }
                }

                m_NextRevealAllowed = DateTime.UtcNow + NextRevealDelay;
            }

            if (Utility.RandomDouble() < 0.01 && DateTime.UtcNow > m_NextSpeechAllowed)
            {
                if (Combatant == null)
                    Say(idleSpeech[Utility.Random(idleSpeech.Length - 1)]);

                m_NextSpeechAllowed = DateTime.UtcNow + NextSpeechDelay;
            }

            if (DateTime.UtcNow > m_NextAIChangeAllowed)
            {
                Effects.PlaySound(Location, Map, GetAngerSound());

                switch (Utility.RandomMinMax(1, 5))
                {
                    case 1:
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*cackles*");

                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestResist] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                    break;

                    case 2:
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*sneers*");

                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 10;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestResist] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                    break;

                    case 3:
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*scowls*");

                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 10;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestResist] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                    break;

                    case 4:
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*seethes*");

                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestResist] = 10;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                    break;

                    case 5:
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*curses*");

                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestResist] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 10;
                    break;
                }

                Animate(27, 10, 1, true, false, 0); //Sneer
                m_NextAIChangeAllowed = DateTime.UtcNow + NextAIChangeDelay;
            }            
        }

        public override bool OnBeforeDeath()
        {
            return base.OnBeforeDeath();
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (m_Item != null)
                m_Item.Delete();
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (m_Item != null)
                m_Item.Delete();
        }

        public override bool IsHighSeasBodyType { get { return true; } }

        public override int AttackAnimation { get { return 4; } }
        public override int AttackFrames { get { return 10; } }

        public override int HurtAnimation { get { return 10; } }
        public override int HurtFrames { get { return 8; } }

        public override int IdleAnimation { get { return -1; } }
        public override int IdleFrames { get { return 0; } }

        public override int GetAngerSound() { return 0x2BC; }
        public override int GetIdleSound() { return 0x2B9; }
        public override int GetAttackSound() { return 0x2BA; }
        public override int GetHurtSound() { return 0x621; }
        public override int GetDeathSound() { return 0x58D; }

        protected override bool OnMove(Direction d)
        {
            Effects.PlaySound(Location, Map, 0x654);            

            return base.OnMove(d);
        }

        protected override void OnLocationChange(Point3D oldLocation)
        {
            base.OnLocationChange(oldLocation);

            if (m_Item != null)
            {
                Point3D skullLocation = new Point3D(Location.X, Location.Y, Z + 35);
                m_Item.Location = skullLocation;
            }
        }

        public KhaldunLichAlmonjin(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            writer.Write(damageIntervalThreshold);
            writer.Write(damageProgress);
            writer.Write(intervalCount);
            writer.Write(totalIntervals);

            writer.Write(m_Item);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            damageIntervalThreshold = reader.ReadInt();
            damageProgress = reader.ReadInt();
            intervalCount = reader.ReadInt();
            totalIntervals = reader.ReadInt();

            m_Item = reader.ReadItem();
        }
    }
}



