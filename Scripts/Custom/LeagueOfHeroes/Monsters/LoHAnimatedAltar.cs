using System;
using Server.Items;
using Server.Custom;
using Server.Network;
using System.Collections;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class LoHAnimatedAltarEvent : LoHEvent
    {
        public override Type CreatureType { get { return typeof(LoHAnimatedAltar); } }
        public override string DisplayName { get { return "Animated Altar"; } }

        public override string AnnouncementText { get { return "An Animated Altar has been sighted in the shifting sands of the northeast Britain desert!"; } }

        public override int RewardItemId { get { return 0; } }
        public override int RewardHue { get { return 2500; } }

        public override Point3D MonsterLocation { get { return new Point3D(1908, 837, -1); } }
        public override Point3D PortalLocation { get { return new Point3D(1887, 843, 7); } }
    }

    [CorpseName("an animated altar corpse")]
    public class LoHAnimatedAltar : LoHMonster
    {
        public override int MinBaseHits { get { return 2000; } }
        public override int MaxBaseHits { get { return 2500; } }

        public override int MinExtraHitsPerPlayer { get { return 1000; } }
        public override int MaxExtraHitsPerPlayer { get { return 1250; } }

        [Constructable]
        public LoHAnimatedAltar(): base()
        {
            Name = "Animated Altar";

            Body = 789;            
            Hue = 2500;

            BaseSoundID = 0x63A;

            SetDex(25);

            SetDamage(30, 50);

            SetSkill(SkillName.Wrestling, 100);
            SetSkill(SkillName.MagicResist, 100);  

            VirtualArmor = 150;

            Fame = 10000;
            Karma = -10000;            
        }        
        
        public override void ConfigureCreature()
        {
            base.ConfigureCreature();
        }

        public override void SetUniqueAI()
        {
            base.SetUniqueAI();
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            if (!willKill)
            {
                if (from != null)
                {
                    BaseWeapon weapon = from.Weapon as BaseWeapon;

                    if (weapon != null && Utility.GetDistance(Location, from.Location) <= 20)
                    {
                        double rangedReflectChance = .03 + (.03 * SpawnPercent);
                        double petrificationChance = .03 + (.03 * SpawnPercent);

                        //Ranged Weapon
                        if (weapon is BaseRanged)
                        {
                            if (Utility.RandomDouble() < rangedReflectChance)
                                RangedReflect(from, amount);

                            else if (Utility.RandomDouble() < petrificationChance)
                                PetrificationGlyph(from);
                        }

                        //Melee Weapon
                        else if (weapon is BaseMeleeWeapon || weapon is Fists)
                        {
                            if (Utility.RandomDouble() < petrificationChance)
                                PetrificationGlyph(from);
                        }
                    }
                }
            }

            base.OnDamage(amount, from, willKill);
        }

        public void RangedReflect(Mobile from, int damageAmount)
        {
            if (!SpecialAbilities.Exists(this)) return;
            if (!SpecialAbilities.Exists(from)) return;

            BaseWeapon weapon = from.Weapon as BaseWeapon;

            if (damageAmount > DamageMax)
                damageAmount = DamageMax;

            int itemId = 7166;
            int itemHue = 0;

            bool crossbow = true;

            if (weapon is Bow)
            {
                crossbow = false;
                itemId = 3906;
            }
            
            double distance = GetDistanceToSqrt(from.Location);
            double destinationDelay = (double)distance * .06;

            Point3D location = Location;
            Point3D targetLocation = from.Location;
            Map map = from.Map;

            if (crossbow)
                PublicOverheadMessage(MessageType.Regular, SpeechHue, false, "*reflects bolt*");
            else
                PublicOverheadMessage(MessageType.Regular, SpeechHue, false, "*reflects arrow*");
            
            Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
            {
                if (!SpecialAbilities.Exists(this)) return;
                if (!SpecialAbilities.Exists(from)) return;

                IEntity startLocation = new Entity(Serial.Zero, new Point3D(location.X, location.Y, location.Z + 5), map);
                IEntity endLocation = new Entity(Serial.Zero, new Point3D(targetLocation.X, targetLocation.Y, targetLocation.Z + 5), map);

                Effects.SendMovingEffect(startLocation, endLocation, itemId, 18, 1, false, false, itemHue, 0);

                Direction = Utility.GetDirection(location, targetLocation);

                PlaySound(0x3B4);
                FixedEffect(0x37B9, 10, 16);

                Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                {
                    if (!SpecialAbilities.Exists(this))
                        return;

                    Queue m_Queue = new Queue();

                    IPooledEnumerable nearbyMobiles = Map.GetMobilesInRange(targetLocation, 1);

                    foreach (Mobile mobile in nearbyMobiles)
                    {
                        if (mobile == this) continue;
                        if (!SpecialAbilities.MonsterCanDamage(this, mobile)) continue;

                        m_Queue.Enqueue(mobile);
                    }

                    nearbyMobiles.Free();

                    while (m_Queue.Count > 0)
                    {
                        Mobile mobile = (Mobile)m_Queue.Dequeue();

                        double damage = (double)damageAmount;

                        if (mobile is BaseCreature)
                            damage *= 2;

                        int finalDamage = (int)(Math.Ceiling(damage));

                        DoHarmful(mobile);

                        new Blood().MoveToWorld(mobile.Location, mobile.Map);
                        AOS.Damage(mobile, this, finalDamage, 100, 0, 0, 0, 0);
                    }
                });
            });
        }

        public void PetrificationGlyph(Mobile from)
        {
            if (!SpecialAbilities.Exists(this)) return;
            if (!SpecialAbilities.Exists(from)) return;

            Point3D location = from.Location;
            Map map = from.Map;

            int effectHue = 1102;

            PlaySound(GetAngerSound());

            double seconds = 2 - (1 * SpawnPercent);            

            Dictionary<int, Point3D> petrifyComponents = new Dictionary<int, Point3D>();

            petrifyComponents.Add(0x3083, new Point3D(location.X - 1, location.Y - 1, location.Z));
            petrifyComponents.Add(0x3080, new Point3D(location.X - 1, location.Y, location.Z));
            petrifyComponents.Add(0x3082, new Point3D(location.X, location.Y - 1, location.Z));
            petrifyComponents.Add(0x3081, new Point3D(location.X + 1, location.Y - 1, location.Z));
            petrifyComponents.Add(0x307D, new Point3D(location.X - 1, location.Y + 1, location.Z));
            petrifyComponents.Add(0x307F, new Point3D(location.X, location.Y, location.Z));
            petrifyComponents.Add(0x307E, new Point3D(location.X + 1, location.Y, location.Z));
            petrifyComponents.Add(0x307C, new Point3D(location.X, location.Y + 1, location.Z));
            petrifyComponents.Add(0x307B, new Point3D(location.X + 1, location.Y + 1, location.Z));

            foreach (KeyValuePair<int, Point3D> keyPairValue in petrifyComponents)
            {
                Point3D componentLocation = keyPairValue.Value;

                TimedStatic glyphComponenet = new TimedStatic(keyPairValue.Key, seconds);
                glyphComponenet.Name = "petrification glyph";
                glyphComponenet.Hue = effectHue;
                glyphComponenet.MoveToWorld(componentLocation, from.Map);

                Effects.SendLocationParticles(EffectItem.Create(componentLocation, map, TimeSpan.FromSeconds(0.5)), 14202, 10, 20, effectHue, 0, 5029, 0);
            }

            Effects.SendLocationParticles(EffectItem.Create(Location, Map, TimeSpan.FromSeconds(0.5)), 14202, 10, 30, effectHue, 0, 5029, 0);

            Effects.PlaySound(Location, Map, 0x650);
            Effects.PlaySound(location, Map, 0x650);            

            Timer.DelayCall(TimeSpan.FromSeconds(seconds), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;

                AbilityInProgress = false;

                foreach (KeyValuePair<int, Point3D> keyPairValue in petrifyComponents)
                {
                    Point3D componentLocation = keyPairValue.Value;

                    TimedStatic petrification = new TimedStatic(0x3709, seconds);
                    petrification.Name = "petrification";
                    petrification.Hue = effectHue;
                    petrification.MoveToWorld(componentLocation, from.Map);                   
                }

                Effects.PlaySound(location, map, 0x56E);

                Queue m_Queue = new Queue();

                IPooledEnumerable nearbyMobiles = map.GetMobilesInRange(location, 1);

                foreach (Mobile mobile in nearbyMobiles)
                {
                    if (!SpecialAbilities.Exists(this)) return;
                    if (mobile == this) continue;
                    if (!SpecialAbilities.MonsterCanDamage(this, mobile)) continue;                  

                    m_Queue.Enqueue(mobile);
                }

                nearbyMobiles.Free();

                while (m_Queue.Count > 0)
                {
                    Mobile mobile = (Mobile)m_Queue.Dequeue();                    

                    double duration = 4 + (4 * SpawnPercent);

                    if (from is BaseCreature)
                        duration *= 2;

                    mobile.PublicOverheadMessage(MessageType.Regular, 1102, false, "*turned to stone*");

                    SpecialAbilities.PetrifySpecialAbility(1.0, this, mobile, 1.0, duration, -1, true, "", "You have been petrified!");

                    int minDamage = 10;
                    int maxDamage = 20;

                    double damage = (double)Utility.RandomMinMax(minDamage, maxDamage);

                    if (mobile is BaseCreature)
                        damage *= 2;

                    new Blood().MoveToWorld(mobile.Location, mobile.Map);
                    AOS.Damage(mobile, null, (int)damage, 100, 0, 0, 0, 0);
                }
            });
        }

        public override bool OnBeforeHarmfulSpell()
        {
            double effectChance = .20 + (.20 * SpawnPercent);

            if (Utility.RandomDouble() < effectChance)
                MagicDamageAbsorb = 1;

            return true;
        }

        public override int GetAngerSound() { return 0x509; }
        public override int GetIdleSound() { return 0x50A; }
        public override int GetAttackSound() { return 0x50C; }
        public override int GetHurtSound() { return 0x63A; }
        public override int GetDeathSound() { return 0x508; }

        public LoHAnimatedAltar(Serial serial): base(serial)
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
