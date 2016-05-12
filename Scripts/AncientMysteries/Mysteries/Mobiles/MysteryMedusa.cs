using System;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("a medusa's corpse")]
    public class MysteryMedusa : BaseCreature
    {
        public List<Mobile> m_Mobiles = new List<Mobile>();

        public DateTime m_NextArrowAllowed;
        public TimeSpan NextArrowDelay = TimeSpan.FromSeconds(1.5);

        public DateTime m_NextArrowStormAllowed = DateTime.UtcNow + TimeSpan.FromSeconds(10);
        public TimeSpan NextArrowStormDelay = TimeSpan.FromSeconds(Utility.RandomMinMax(8, 12));

        public DateTime m_NextSnakesAllowed = DateTime.UtcNow + TimeSpan.FromSeconds(60);
        public TimeSpan NextSnakeDelay = TimeSpan.FromSeconds(Utility.RandomMinMax(60, 120));

        public int snakeEvents = 0;
        public static int maxSnakeEvents = 3;

        public DateTime m_NextAIChangeAllowed;
        public TimeSpan NextAIChangeDelay = TimeSpan.FromSeconds(30);       

        [Constructable]
        public MysteryMedusa(): base(AIType.AI_Generic, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            SpeechHue = 1326;
            Name = "an ancient medusa";

            Body = 728;
            Hue = 2500;

            SetStr(100);
            SetDex(75);
            SetInt(25);

            SetHits(4000);
            SetStam(4000);

            SetDamage(20, 30);

            AttackSpeed = 50;

            VirtualArmor = 75;

            SetSkill(SkillName.Archery, 110);
            SetSkill(SkillName.Wrestling, 100);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 150);

            Fame = 1500;
            Karma = -1500;

            //-------

            m_AncientMysteryCreature = true;
        }

        public override bool AlwaysBoss { get { return true; } }

        public override int PoisonResistance { get { return 5; } }
        public override bool AlwaysMurderer { get { return true; } }

        public override bool IsHighSeasBodyType { get { return true; } }

        public override void SetUniqueAI()
        {            
            UniqueCreatureDifficultyScalar = 1.75;

            DictCombatRange[CombatRange.WeaponAttackRange] = 0;
            DictCombatRange[CombatRange.SpellRange] = 14;
            DictCombatRange[CombatRange.Withdraw] = 1;

            ResolveAcquireTargetDelay = 1.0;
            RangePerception = 18;
        }               

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            m_NextArrowAllowed = DateTime.UtcNow + NextArrowDelay;
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            if (!willKill)
            {
                if (from != null)
                {
                    BaseWeapon weapon = from.Weapon as BaseWeapon;

                    if (weapon != null)
                    {
                        //Ranged Weapon
                        if (weapon is BaseRanged)
                        {
                            if (Utility.RandomDouble() < .20)
                                Petrify(from);
                        }

                        //Melee Weapon
                        else if (weapon is BaseMeleeWeapon || weapon is Fists)
                        {
                            if (Utility.RandomDouble() < .10)
                                Petrify(from);
                        }
                    }

                    else
                    {
                        //Spell or Special Effect
                        if (Utility.RandomDouble() < .30)
                            Petrify(from);
                    }
                }                
            }

            base.OnDamage(amount, from, willKill);
        }

        public void Petrify(Mobile from)
        {
            if (from == null)
                return;

            PlaySound(GetAngerSound());

            FixedParticles(0x375A, 10, 15, 5037, 2499, 0, EffectLayer.Waist);

            PublicOverheadMessage(MessageType.Regular, 0, false, "*gaze turns them into stone*");
            SpecialAbilities.PetrifySpecialAbility(1.0, this, from, 1.0, 5.0, -1, true, "", "You are petrified by their gaze!", "-1"); 
        }

        public override void OnThink()
        {
            base.OnThink();

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

            if (m_NextSnakesAllowed < DateTime.UtcNow && snakeEvents < maxSnakeEvents)
            {
                snakeEvents++;

                PlaySound(GetAngerSound());
                PublicOverheadMessage(MessageType.Regular, 0, false, "*snakes emerge from hair*");       

                for (int a = 0; a < 3; a++)
                {
                    BaseCreature bc_Creature = null;

                    double creatureResult = Utility.RandomDouble();

                    if (creatureResult < .01)
                        bc_Creature = new SilverSerpent();

                    else if (creatureResult < .03)
                        bc_Creature = new IceSerpent();

                    else if (creatureResult < .5)
                        bc_Creature = new LavaSerpent();

                    else if (creatureResult < .10)
                        bc_Creature = new GiantSerpent();

                    else if (creatureResult < .25)
                        bc_Creature = new GiantCoralSnake();

                    else if (creatureResult < .50)
                        bc_Creature = new CoralSnake();

                    else
                        bc_Creature = new Snake();

                    if (bc_Creature != null)
                    {
                        bc_Creature.MoveToWorld(Location, Map);
                        bc_Creature.PlaySound(bc_Creature.GetAngerSound());

                        m_Mobiles.Add(bc_Creature);
                    }
                }

                m_NextSnakesAllowed = DateTime.UtcNow + NextSnakeDelay;

                return;
            }

            if (m_NextArrowAllowed < DateTime.UtcNow && AIObject != null && Combatant != null)
            {
                if (m_NextArrowStormAllowed < DateTime.UtcNow && !Hidden && AIObject.currentCombatRange != CombatRange.Withdraw)
                {
                    int attacks = Utility.RandomMinMax(5, 10);

                    double preAttackDelay = .1;
                    double attackInterval = .4;

                    double hitChance = .8;

                    double actionsCooldown = attacks * (preAttackDelay + attackInterval);

                    m_NextArrowAllowed = DateTime.UtcNow + TimeSpan.FromSeconds(actionsCooldown) + NextArrowDelay;
                    m_NextArrowStormAllowed = m_NextArrowAllowed + NextArrowStormDelay;

                    if (AIObject != null)
                    {
                        AIObject.NextMove = DateTime.UtcNow + TimeSpan.FromSeconds(actionsCooldown);
                        LastSwingTime = LastSwingTime + TimeSpan.FromSeconds(actionsCooldown);
                        NextSpellTime = NextSpellTime + TimeSpan.FromSeconds(actionsCooldown);
                        NextCombatHealActionAllowed = NextCombatHealActionAllowed + TimeSpan.FromSeconds(actionsCooldown);
                        NextCombatSpecialActionAllowed = NextCombatSpecialActionAllowed + TimeSpan.FromSeconds(actionsCooldown);
                        NextCombatEpicActionAllowed = NextCombatEpicActionAllowed + TimeSpan.FromSeconds(actionsCooldown);
                    }

                    Point3D location = Location;
                    Map map = Map;

                    for (int a = 0; a < attacks; a++)
                    {
                        Timer.DelayCall(TimeSpan.FromSeconds(a * (preAttackDelay + attackInterval)), delegate
                        {
                            if (this == null) return;
                            if (Deleted || !Alive) return;

                            var mobilesNearby = map.GetMobilesInRange(location, 14);

                            List<Mobile> m_MobilesToTarget = new List<Mobile>();

                            foreach (Mobile mobile in mobilesNearby)
                            {
                                if (mobile == null) continue;
                                if (mobile.Deleted || !mobile.Alive) continue;
                                if (mobile.AccessLevel > AccessLevel.Player) continue;

                                bool validTarget = false;

                                PlayerMobile pm_Mobile = mobile as PlayerMobile;
                                BaseCreature bc_Mobile = mobile as BaseCreature;                                

                                if (pm_Mobile != null)
                                    validTarget = true;

                                if (bc_Mobile != null)
                                {
                                    if (bc_Mobile.Controlled && bc_Mobile.ControlMaster is PlayerMobile)
                                        validTarget = true;
                                }                                

                                if (validTarget)
                                    m_MobilesToTarget.Add(mobile);
                            }

                            mobilesNearby.Free();

                            if (m_MobilesToTarget.Count == 0)
                                return;

                            Mobile target = m_MobilesToTarget[Utility.RandomMinMax(0, m_MobilesToTarget.Count - 1)];

                            Direction = GetDirectionTo(target);

                            Effects.PlaySound(location, map, GetAttackSound());

                            Timer.DelayCall(TimeSpan.FromSeconds(preAttackDelay), delegate
                            {
                                if (this == null) return;
                                if (Deleted || !this.Alive) return;
                                if (target == null) return;
                                if (target.Deleted || !target.Alive) return;

                                Animate(Utility.RandomList(4), 5, 1, true, false, 0);

                                Timer.DelayCall(TimeSpan.FromSeconds(attackInterval), delegate
                                {
                                    if (this == null) return;
                                    if (!this.Alive || this.Deleted) return;
                                    if (target == null) return;
                                    if (!target.Alive || target.Deleted) return;

                                    MovingEffect(target, 0xF42, 18, 1, false, false, 2576, 0);

                                    double distance = GetDistanceToSqrt(target.Location);
                                    double destinationDelay = (double)distance * .08;

                                    Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                                    {
                                        if (this == null) return;
                                        if (!Alive || Deleted) return;
                                        if (target == null) return;
                                        if (!target.Alive || target.Deleted) return;

                                        if (Utility.RandomDouble() < hitChance)
                                        {
                                            Effects.PlaySound(location, map, 0x234);

                                            int damage = Utility.RandomMinMax(DamageMin, DamageMax);

                                            if (damage < 1)
                                                damage = 1;

                                            //Manual Parry Handling
                                            BaseShield shield = target.FindItemOnLayer(Layer.TwoHanded) as BaseShield;

                                            if (shield != null)
                                                damage = shield.OnHit(Weapon as BaseWeapon, damage, this);

                                            BaseWeapon weapon = target.FindItemOnLayer(Layer.TwoHanded) as BaseWeapon;

                                            if (!(weapon is BaseRanged) && weapon != null)
                                                damage = weapon.WeaponParry(weapon, damage, target);

                                            DoHarmful(target);

                                            List<Point3D> m_ExplosionLocations = new List<Point3D>();
                                            Point3D destination = target.Location;

                                            m_ExplosionLocations.Add(target.Location);

                                            int radius = 1;

                                            for (int b = 1; b < radius + 1; b++)
                                            {
                                                m_ExplosionLocations.Add(new Point3D(destination.X - b, destination.Y - b, destination.Z));
                                                m_ExplosionLocations.Add(new Point3D(destination.X, destination.Y - b, destination.Z));
                                                m_ExplosionLocations.Add(new Point3D(destination.X + b, destination.Y - b, destination.Z));
                                                m_ExplosionLocations.Add(new Point3D(destination.X + b, destination.Y, destination.Z));
                                                m_ExplosionLocations.Add(new Point3D(destination.X + b, destination.Y + b, destination.Z));
                                                m_ExplosionLocations.Add(new Point3D(destination.X, destination.Y + b, destination.Z));
                                                m_ExplosionLocations.Add(new Point3D(destination.X - b, destination.Y + b, destination.Z));
                                                m_ExplosionLocations.Add(new Point3D(destination.X - b, destination.Y, destination.Z));
                                            }

                                            foreach (Point3D explosionLocation in m_ExplosionLocations)
                                            {
                                                Effects.SendLocationParticles(EffectItem.Create(explosionLocation, target.Map, TimeSpan.FromSeconds(0.25)), 0x3779, 10, 15, 1153, 0, 5029, 0);
                                            }

                                            SpecialAbilities.CrippleSpecialAbility(1.0, this, target, .25, 10, 0x64F, false, "", "The medusa's arrow has slowed your actions!", "-1");

                                            new Blood().MoveToWorld(target.Location, target.Map);
                                            AOS.Damage(target, this, damage, 100, 0, 0, 0, 0);                                            
                                        }

                                        else
                                            Effects.PlaySound(location, map, 0x238);
                                    });
                                });
                            });
                        });
                    }
                }

                else
                {
                    double hitChance = .66;

                    int minDamage = DamageMin;
                    int maxDamage = DamageMax;

                    AIObject.NextMove = DateTime.UtcNow + TimeSpan.FromSeconds(1.5);
                    LastSwingTime = LastSwingTime + TimeSpan.FromSeconds(3);

                    m_NextArrowAllowed = DateTime.UtcNow + NextArrowDelay;

                    Animate(Utility.RandomList(4), 5, 1, true, false, 0);
                    Effects.PlaySound(Location, Map, this.GetAttackSound());

                    Point3D location = Location;
                    Map map = Map;

                    Timer.DelayCall(TimeSpan.FromSeconds(.475), delegate
                    {
                        if (this == null) return;
                        if (!Alive || Deleted) return;
                        if (Combatant == null) return;
                        if (!Combatant.Alive || Combatant.Deleted) return;

                        MovingEffect(Combatant, 0xF42, 18, 1, false, false, 2576, 0);

                        double distance = GetDistanceToSqrt(Combatant.Location);
                        double destinationDelay = (double)distance * .08;                                                

                        Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                        {
                            if (this == null) return;
                            if (!Alive || Deleted) return;
                            if (Combatant == null) return;
                            if (!Combatant.Alive || Combatant.Deleted) return;

                            if (Utility.RandomDouble() < hitChance)
                            {
                                Effects.PlaySound(location, map, 0x234);

                                int damage = Utility.RandomMinMax(minDamage, maxDamage);

                                //Manual Parry Handling
                                BaseShield shield = Combatant.FindItemOnLayer(Layer.TwoHanded) as BaseShield;

                                if (shield != null)
                                    damage = shield.OnHit(Weapon as BaseWeapon, damage, this);

                                BaseWeapon weapon = Combatant.FindItemOnLayer(Layer.TwoHanded) as BaseWeapon;

                                if (!(weapon is BaseRanged) && weapon != null)
                                    damage = weapon.WeaponParry(weapon, damage, Combatant);

                                if (damage < 1)
                                    damage = 1;

                                DoHarmful(Combatant);

                                SpecialAbilities.EntangleSpecialAbility(0.33, this, Combatant, 1.0, 5, -1, true, "", "Their arrow pins you in place!", "-1");

                                new Blood().MoveToWorld(Combatant.Location, Combatant.Map);
                                AOS.Damage(Combatant, this, damage, 100, 0, 0, 0, 0);                                
                            }

                            else
                                Effects.PlaySound(Location, Map, 0x238);
                        });
                    });
                }
            }
        }

        public override int GetAngerSound() { return 0x370; }
        public override int GetIdleSound() { return 0x373; }
        public override int GetAttackSound() { return 0x612; }
        public override int GetHurtSound() { return 0x375; }
        public override int GetDeathSound() { return 0x376; }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            c.AddItem(new MedusaMirror());

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

            for (int a = 0; a < m_Mobiles.Count; ++a)
            {
                if (m_Mobiles[a] != null)
                {
                    if (m_Mobiles[a].Alive)
                        m_Mobiles[a].Kill();
                }
            } 
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            for (int a = 0; a < m_Mobiles.Count; ++a)
            {
                if (m_Mobiles[a] != null)
                {
                    if (!m_Mobiles[a].Deleted)
                        m_Mobiles[a].Delete();
                }
            }            
        }

        public MysteryMedusa(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version 

            //Version 0
            writer.Write(m_Mobiles.Count);
            for (int a = 0; a < m_Mobiles.Count; a++)
            {
                writer.Write(m_Mobiles[a]);
            }

            writer.Write(snakeEvents);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Mobiles = new List<Mobile>();

            //Version 0
            if (version >= 0)
            {
                int creaturesCount = reader.ReadInt();
                for (int a = 0; a < creaturesCount; a++)
                {
                    Mobile creature = reader.ReadMobile();
                    m_Mobiles.Add(creature);
                }

                snakeEvents = reader.ReadInt();
            }
        }
    }
}