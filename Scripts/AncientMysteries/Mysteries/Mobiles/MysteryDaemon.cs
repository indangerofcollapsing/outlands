using System;
using Server;
using Server.Items;
using System.Collections;
using System.Collections.Generic;
using Server.Spells;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("a mysterious daemon corpse")]
    public class MysteryDaemon : BaseCreature
    {
        public DateTime m_NextAIChangeAllowed;
        public TimeSpan NextAIChangeDelay = TimeSpan.FromSeconds(30);

        public DateTime m_NextFireBlastAllowed;
        public TimeSpan NextFireBlastDelay = TimeSpan.FromSeconds(Utility.RandomMinMax(20, 40));

        public DateTime m_NextSunburstAllowed;
        public TimeSpan NextSunburstDelay = TimeSpan.FromSeconds(Utility.RandomMinMax(20, 40));

        public DateTime m_NextAbilityAllowed;
        public TimeSpan NextAbilityAllowedDelay = TimeSpan.FromSeconds(10);

        [Constructable]
        public MysteryDaemon(): base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "an ancient mysterious daemon";

            Body = 1071;
            BaseSoundID = 357;

            Hue = 2515;

            SetStr(100);
            SetDex(100);
            SetInt(25);

            SetHits(5000);
            SetStam(5000);
            SetMana(5000);

            SetDamage(30, 50);

            SetSkill(SkillName.Wrestling, 100);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.MagicResist, 100);

            VirtualArmor = 50;            

            Fame = 9500;
            Karma = -9500;

            //-------

            m_AncientMysteryCreature = true;
        }

        public override bool AlwaysBoss { get { return true; } }
        
        public override int PoisonResistance { get { return 5; } }
        public override bool AlwaysMurderer { get { return true; } }

        public override void SetUniqueAI()
        {            
            UniqueCreatureDifficultyScalar = 1.5;

            ResolveAcquireTargetDelay = 1.0;
            RangePerception = 18;
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() <= .20)
            {
                PublicOverheadMessage(MessageType.Regular, 0, false, "*swings wildly*");

                double damage = DamageMin;

                if (defender is BaseCreature)
                    damage *= 1.5;

                SpecialAbilities.KnockbackSpecialAbility(1.0, Location, this, defender, damage, 3, -1, "", "The creature flings you aside!");

                Combatant = null;
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            if (Combatant != null && m_NextSunburstAllowed < DateTime.UtcNow && m_NextAbilityAllowed < DateTime.UtcNow)
            {
                if (Combatant.Alive && !Combatant.Hidden && InLOS(Combatant) && Utility.GetDistance(Location, Combatant.Location) <= 8)
                {
                    m_NextSunburstAllowed = DateTime.UtcNow + NextSunburstDelay;
                    m_NextAbilityAllowed = DateTime.UtcNow + NextAbilityAllowedDelay;

                    Point3D location = Location;
                    Point3D combatantLocation = Combatant.Location;
                    Map map = Map;

                    int effectRadius = Utility.RandomMinMax(6, 10);

                    int rows = (effectRadius * 2) + 1;
                    int columns = (effectRadius * 2) + 1;

                    List<Point3D> m_EffectLocations = new List<Point3D>();

                    for (int a = 1; a < rows + 1; a++)
                    {
                        for (int b = 1; b < columns + 1; b++)
                        {
                            Point3D newPoint = new Point3D(location.X + (-1 * (effectRadius + 1)) + a, location.Y + (-1 * (effectRadius + 1)) + b, location.Z);
                            SpellHelper.AdjustField(ref newPoint, map, 12, false);

                            if (!map.InLOS(newPoint, location))
                                continue;

                            if (!m_EffectLocations.Contains(newPoint))
                                m_EffectLocations.Add(newPoint);
                        }
                    }

                    int explosionHue = 2515; //2586;

                    int cycles = 50;

                    int minDamage = DamageMin;
                    int maxDamage = DamageMax;

                    if (m_EffectLocations.Count > 0)
                    {
                        for (int a = 0; a < cycles; a++)
                        {
                            Timer.DelayCall(TimeSpan.FromSeconds(a * .1), delegate
                            {
                                Effects.PlaySound(location, map, 0x5CF);

                                Point3D newLocation = m_EffectLocations[Utility.RandomMinMax(0, m_EffectLocations.Count - 1)];
                                SpellHelper.AdjustField(ref newLocation, map, 12, false);

                                Effects.SendLocationParticles(EffectItem.Create(newLocation, map, TimeSpan.FromSeconds(0.25)), 0x3709, 10, 30, explosionHue, 0, 5029, 0);

                                IPooledEnumerable mobilesOnTile = map.GetMobilesInRange(newLocation, 0);

                                Queue m_Queue = new Queue();

                                foreach (Mobile mobile in mobilesOnTile)
                                {
                                    if (!mobile.CanBeDamaged() || !mobile.Alive || mobile.AccessLevel > AccessLevel.Player)
                                            continue;                                  

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
                                        m_Queue.Enqueue(mobile);
                                }

                                mobilesOnTile.Free();

                                while (m_Queue.Count > 0)
                                {
                                    Mobile mobile = (Mobile)m_Queue.Dequeue();

                                    double damage = Utility.RandomMinMax(minDamage, maxDamage);

                                    if (mobile is BaseCreature)
                                        damage *= 1.5;

                                    new Blood().MoveToWorld(mobile.Location, mobile.Map);

                                    SingleFireField singleFireField = new SingleFireField(this, explosionHue, 1, 20, 3, 5, false, false, true, -1, true);
                                    singleFireField.MoveToWorld(mobile.Location, map);
                                    
                                    AOS.Damage(mobile, this, (int)(Math.Round(damage)), 0, 100, 0, 0, 0);
                                }
                            });
                        }
                    }

                    return;
                }
            }

            if (Combatant != null && m_NextFireBlastAllowed < DateTime.UtcNow && m_NextAbilityAllowed < DateTime.UtcNow)
            {
                if (Combatant.Alive && !Combatant.Hidden && InLOS(Combatant) && GetDistanceToSqrt(Combatant) <= 12)
                {
                    m_NextFireBlastAllowed = DateTime.UtcNow + NextFireBlastDelay;
                    m_NextAbilityAllowed = DateTime.UtcNow + NextAbilityAllowedDelay;

                    double effectTime = 1.5;
                    double actionsCooldown = 3;

                    PlaySound(GetAngerSound());

                    AIObject.NextMove = DateTime.UtcNow + TimeSpan.FromSeconds(effectTime);
                    LastSwingTime = LastSwingTime + TimeSpan.FromSeconds(effectTime);

                    NextSpellTime = NextSpellTime + TimeSpan.FromSeconds(actionsCooldown);
                    NextCombatHealActionAllowed = NextCombatHealActionAllowed + TimeSpan.FromSeconds(actionsCooldown);
                    NextCombatSpecialActionAllowed = NextCombatSpecialActionAllowed + TimeSpan.FromSeconds(actionsCooldown);
                    NextCombatEpicActionAllowed = NextCombatEpicActionAllowed + TimeSpan.FromSeconds(actionsCooldown);

                    Animate(6, 10, 1, true, false, 0);

                    int itemId = 0x36D4;
                    int itemHue = 2515; //2613;

                    int explosionHue = 2515; //2586;

                    int minDamage = DamageMin;
                    int maxDamage = DamageMin;

                    Point3D location = Location;
                    Point3D targetLocation = Combatant.Location;
                    Map map = Combatant.Map;

                    Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate
                    {
                        if (this == null) return;
                        if (Deleted || !Alive) return;

                        Effects.PlaySound(targetLocation, map, 0x5FC);

                        IEntity startLocation = new Entity(Serial.Zero, new Point3D(location.X, location.Y, location.Z + 20), map);
                        IEntity endLocation = new Entity(Serial.Zero, new Point3D(targetLocation.X, targetLocation.Y, targetLocation.Z + 10), map);

                        Effects.SendMovingParticles(startLocation, endLocation, itemId, 8, 0, false, false, itemHue, 0, 9501, 0, 0, 0x100);

                        double distance = GetDistanceToSqrt(targetLocation);
                        double destinationDelay = (double)distance * .06;

                        Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                        {
                            Effects.PlaySound(targetLocation, map, 0x357);

                            int radius = Utility.RandomMinMax(2, 4);

                            int minRange = radius * -1;
                            int maxRange = radius;

                            for (int a = minRange; a < maxRange + 1; a++)
                            {
                                for (int b = minRange; b < maxRange + 1; b++)
                                {
                                    Point3D newPoint = new Point3D(targetLocation.X + a, targetLocation.Y + b, targetLocation.Z);
                                    SpellHelper.AdjustField(ref newPoint, map, 12, false);

                                    if (!map.InLOS(targetLocation, newPoint))
                                        continue;

                                    int distanceDelay = Utility.GetDistance(targetLocation, newPoint);

                                    Timer.DelayCall(TimeSpan.FromSeconds(distanceDelay * .15), delegate
                                    {
                                        Effects.PlaySound(newPoint, map, Utility.RandomList(0x4F1, 0x5D8, 0x5DA, 0x580));
                                        Effects.SendLocationParticles(EffectItem.Create(newPoint, map, TimeSpan.FromSeconds(0.25)), 0x3709, 10, 30, explosionHue, 0, 5029, 0);

                                        IPooledEnumerable mobilesOnTile = map.GetMobilesInRange(newPoint, 0);

                                        Queue m_Queue = new Queue();

                                        foreach (Mobile mobile in mobilesOnTile)
                                        {
                                            if (!mobile.CanBeDamaged() || !mobile.Alive || mobile.AccessLevel > AccessLevel.Player)
                                                continue;

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
                                                m_Queue.Enqueue(mobile);
                                        }

                                        mobilesOnTile.Free();

                                        while (m_Queue.Count > 0)
                                        {
                                            Mobile mobile = (Mobile)m_Queue.Dequeue();

                                            double damage = Utility.RandomMinMax(minDamage, maxDamage);

                                            if (mobile is BaseCreature)
                                                damage *= 1.5;

                                            new Blood().MoveToWorld(mobile.Location, mobile.Map);
                                            AOS.Damage(mobile, this, (int)(Math.Round(damage)), 0, 100, 0, 0, 0);
                                        }
                                    });
                                }
                            }
                        });
                    });
                }
            }

            if (Utility.RandomDouble() < .01 && DateTime.UtcNow > m_NextAIChangeAllowed)
            {
                Effects.PlaySound(Location, Map, GetAngerSound());

                switch (Utility.RandomMinMax(1, 5))
                {
                    case 1:
                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.HighestHitPoints] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                        break;

                    case 2:
                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 10;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.HighestHitPoints] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                        break;

                    case 3:
                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 10;
                        DictCombatTargetingWeight[CombatTargetingWeight.HighestHitPoints] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                        break;

                    case 4:
                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.HighestHitPoints] = 10;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                        break;

                    case 5:
                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.HighestHitPoints] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 10;
                        break;
                }

                m_NextAIChangeAllowed = DateTime.UtcNow + NextAIChangeDelay;
            }
        }

        public override bool IsHighSeasBodyType { get { return true; } }

        public override int GetAngerSound() { return 0x572; } //0x4F9
        public override int GetIdleSound() { return 0x167; }
        public override int GetAttackSound() { return 0x60B; } //0x632
        public override int GetHurtSound() { return 0x4F7; }
        public override int GetDeathSound() { return 0x4E0; }

        protected override bool OnMove(Direction d)
        {
            Effects.PlaySound(Location, Map, Utility.RandomList(0x4CF));

            return base.OnMove(d);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            c.AddItem(new SoulfireBrazier());

            int treasurePileLevel = 1;

            if (Utility.RandomMinMax(1, 3) == 1)
                treasurePileLevel = 2;

            if (Utility.RandomMinMax(1, 6) == 1)
                treasurePileLevel = 3;

            if (Utility.RandomMinMax(1, 10) == 1)
                treasurePileLevel = 4;

            switch (treasurePileLevel)
            {
                case 1: c.AddItem(new TreasurePileSmallAddonDeed()); break;
                case 2: c.AddItem(new TreasurePileMediumAddonDeed()); break;
                case 3: c.AddItem(new TreasurePileLargeAddonDeed()); break;
                case 4: c.AddItem(new TreasurePileHugeAddonDeed()); break;
            }
        }

        public MysteryDaemon(Serial serial): base(serial)
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