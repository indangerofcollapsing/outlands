// Delceri
using Server.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("a bunny corpse")]
    public class EasterBunny : Rabbit
    {
        public enum BunnyType
        {
            Quick,
            Elusive,            
            Devious,
            Ferocious,
            Mysterious
        }

        public enum EggType
        {
            Explosive,
            Ice,
            Poison,
            Entangle,
            Gust,
            Banish,
            Bloody
        }

        public int m_EggsDropped = 0;
        public int MaximumEggsToDrop = 5;

        public BunnyType m_BunnyType;
        public EggType m_EggType;

        public bool digging = false;

        [Constructable]
        public EasterBunny()
        {
            Body = 205;
            Hue = 1150;

            Name = "an easter bunny";

            BardImmune = true;

            m_BunnyType = (BunnyType)Utility.RandomMinMax(0, 4);           

            switch (m_BunnyType)
            {
                case BunnyType.Quick:
                    Name = "a speedy easter bunny";

                    SetStr(50);
                    SetDex(75);
                    SetInt(25);

                    SetHits(500);
                    SetStam(500);
                    SetMana(25);

                    SetDamage(3, 5);

                    SetSkill(SkillName.Wrestling, 75);
                    SetSkill(SkillName.Tactics, 100);

                    SetSkill(SkillName.MagicResist, 50);

                    VirtualArmor = 25;
                break;

                case BunnyType.Elusive:
                    Name = "an elusive easter bunny";

                   SetStr(75);
                    SetDex(100);
                    SetInt(25);

                    SetHits(500);
                    SetStam(500);
                    SetMana(25);

                    SetDamage(3, 5);

                    SetSkill(SkillName.Wrestling, 75);
                    SetSkill(SkillName.Tactics, 100);

                    SetSkill(SkillName.MagicResist, 50);

                    VirtualArmor = 25;
                break;

                case BunnyType.Devious:
                    Name = "a devious easter bunny";

                    SetStr(75);
                    SetDex(75);
                    SetInt(75);

                    SetHits(500);
                    SetStam(500);
                    SetMana(25);

                    SetDamage(3, 5);

                    SetSkill(SkillName.Wrestling, 75);
                    SetSkill(SkillName.Tactics, 100);

                    SetSkill(SkillName.MagicResist, 50);

                    VirtualArmor = 25;
                break;

                case BunnyType.Ferocious:
                    Name = "a ferocious easter bunny";

                    SetStr(100);
                    SetDex(75);
                    SetInt(25);

                    SetHits(750);
                    SetStam(500);
                    SetMana(25);

                    SetDamage(15, 30);

                    AttackSpeed = 50;

                    SetSkill(SkillName.Wrestling, 125);
                    SetSkill(SkillName.Tactics, 100);

                    SetSkill(SkillName.MagicResist, 50);

                    VirtualArmor = 25;
                break;

                case BunnyType.Mysterious:
                    Name = "a mysterious easter bunny";

                    SetStr(50);
                    SetDex(50);
                    SetInt(100);

                    SetHits(500);
                    SetStam(500);
                    SetMana(2000);

                    SetDamage(3, 5);

                    SetSkill(SkillName.Wrestling, 75);
                    SetSkill(SkillName.Tactics, 100);

                    SetSkill(SkillName.Magery, 100);
                    SetSkill(SkillName.EvalInt, 100);
                    SetSkill(SkillName.Meditation, 100);

                    SetSkill(SkillName.MagicResist, 200);

                    VirtualArmor = 25;
                break;
            }    

            Fame = 150;
            Karma = 0;

            DelayBeginTunnel();

            Tameable = false;
        }

        public override Poison PoisonImmune 
        { 
            get 
            {
                if (m_BunnyType == BunnyType.Devious)
                    return Poison.Lethal;

                else
                    return null;
            } 
        }

        public EasterBunny(Serial serial): base(serial)
        {
        }

        public override void SetUniqueAI()
        {
            switch (m_BunnyType)
            {
                case BunnyType.Quick:
                    DictCombatRange[CombatRange.Withdraw] = 100;
                    CreatureWithdrawRange = 20;

                    ActiveSpeed = .25;
                    PassiveSpeed = .35;
                break;

                case BunnyType.Elusive:
                    DictCombatRange[CombatRange.Withdraw] = 100;
                    CreatureWithdrawRange = 20;
                break;

                case BunnyType.Devious:
                    DictCombatRange[CombatRange.Withdraw] = 100;
                    CreatureWithdrawRange = 20;
                break;

                case BunnyType.Ferocious:
                    SetSubGroup(AISubgroup.Duelist);
                    UpdateAI(false);                    
                break;

                case BunnyType.Mysterious:
                    SetSubGroup(AISubgroup.Mage4);
                    UpdateAI(false);

                    DictCombatRange[CombatRange.Withdraw] = 5;
                break;
            }

            RangePerception = 18;
            DefaultPerceptionRange = 18;
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (m_BunnyType != BunnyType.Ferocious)
                return;

            double effectChance = .15;

            if (Controlled && ControlMaster != null)
            {
                if (ControlMaster is PlayerMobile)
                {
                    if (defender is PlayerMobile)
                        effectChance = .02;
                    else
                        effectChance = .25;
                }
            }

            SpecialAbilities.FrenzySpecialAbility(effectChance, this, defender, .25, 10, -1, true, "", "", "*becomes frenzied!");
        }

        public override void OnThink()
        {
            base.OnThink();

            if (Utility.RandomDouble() <= .05 && !digging)
            {
                Effects.PlaySound(Location, Map, 729);

                ResolveEggChance(Location, Map, 1.0, .33);
            }

            switch (m_BunnyType)
            {
                case BunnyType.Quick:
                break;

                case BunnyType.Elusive:
                if ((Frozen || CantWalk) && !digging)
                    {
                        Say("*wriggles free*");

                        Frozen = false;
                        CantWalk = false;

                        if (AIObject != null)
                            AIObject.NextMove = DateTime.UtcNow;
                    }
                break;

                case BunnyType.Devious:
                break;

                case BunnyType.Ferocious:
                break;

                case BunnyType.Mysterious:
                break;
            }
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            if (m_BunnyType == BunnyType.Devious && !willKill && Utility.RandomDouble() <= .5 && !digging)
            {
                if (from != null)
                {
                    BaseWeapon weapon = from.Weapon as BaseWeapon;

                    //Weapon Attack
                    if (weapon != null)
                    {
                        if (SpecialAbilities.TeleportAbility(this, 1.0, true, -1, 5, 10, null))
                            Say("*poof*");                        
                    }
                }
            }

            if (m_BunnyType == BunnyType.Mysterious && !willKill && Utility.RandomDouble() <= .5 && !digging)
            {
                if (from != null && Combatant != null)
                {
                    BaseWeapon weapon = from.Weapon as BaseWeapon;

                    //Weapon Attack
                    if (weapon != null && Combatant.Alive)
                    {
                        Point3D location = Combatant.Location;
                        Map map = Combatant.Map;

                        Effects.PlaySound(location, map, 0x657);
                        Effects.SendLocationParticles(EffectItem.Create(location, map, TimeSpan.FromSeconds(5)), 0x3728, 10, 10, 2023);
                        
                        ResolveEggChance(Location, Map, 1.0, .2);
                    }
                }
            }   

            base.OnDamage(amount, from, willKill);
        }

        public override void OnDamagedBySpell(Mobile from)
        {
            if (m_BunnyType == BunnyType.Devious && Utility.RandomDouble() <= .33 && !digging)
            {
                if (from != null)
                {
                    if (SpecialAbilities.TeleportAbility(this, 1.0, true, -1, 5, 10, null))
                        Say("*poof*");                    
                }
            }

            if (m_BunnyType == BunnyType.Mysterious && Utility.RandomDouble() <= .33 && !digging)
            {
                if (from != null && Combatant != null)
                {
                    if (Combatant.Alive)
                    {
                        Point3D location = Combatant.Location;
                        Map map = Combatant.Map;

                        ResolveEggChance(location, map, 1.0, .2);

                        Effects.PlaySound(location, map, 0x657);
                        Effects.SendLocationParticles(EffectItem.Create(location, map, TimeSpan.FromSeconds(5)), 0x3728, 10, 10, 2023);
                    }
                }
            } 

            base.OnDamagedBySpell(from);
        }

        protected override bool OnMove(Direction d)
        {
            return base.OnMove(d);
        }

        public void ResolveEggChance(Point3D location, Map map, double eggChance, double goodEggChance)
        {
            if (Utility.RandomDouble() <= eggChance)
            {
                if (Utility.RandomDouble() <= goodEggChance && m_EggsDropped < MaximumEggsToDrop)
                {
                    EasterEgg Egg = new EasterEgg();

                    Egg.MoveToWorld(location, map);

                    m_EggsDropped++;
                }

                else
                {

                    switch (m_BunnyType)
                    {
                        case BunnyType.Quick:
                            SetTimedEgg(location, map, (EggType)(Utility.RandomList((int)EggType.Ice)));
                        break;

                        case BunnyType.Elusive:
                            SetTimedEgg(location, map, (EggType)(Utility.RandomList((int)EggType.Entangle)));
                        break;

                        case BunnyType.Devious:
                            SetTimedEgg(location, map, (EggType)(Utility.RandomList((int)EggType.Explosive, (int)EggType.Poison, (int)EggType.Gust)));
                        break;

                        case BunnyType.Ferocious:
                            SetTimedEgg(location, map, (EggType)(Utility.RandomList((int)EggType.Bloody)));
                        break;

                        case BunnyType.Mysterious:
                            SetTimedEgg(location, map, (EggType)(Utility.RandomList((int)EggType.Banish)));
                        break;
                    }
                }
            }  
        }

        public virtual void DelayBeginTunnel()
        {
            Timer.DelayCall(TimeSpan.FromSeconds(30), delegate
            {
                if (this == null) return;
                if (Deleted || !Alive) return;

                BeginTunnel();
            });
        }

        public virtual void BeginTunnel()
        {
            if (this == null) return;
            if (Deleted || !Alive) return;            

            int digSound = 0x247;

            CantWalk = true;

            Say("*begins to dig a tunnel back to its magical lair*");
            SpecialAbilities.HinderSpecialAbility(1.0, this, this, 1.0, 1, true, digSound, false, "", "");

            digging = true;

            //Dirt
            int dirtCount = Utility.RandomMinMax(2, 3);

            for (int b = 0; b < dirtCount; b++)
            {
                Blood dirt = new Blood();
                dirt.Name = "dirt";
                dirt.ItemID = Utility.RandomList(7681, 7682);
                Point3D dirtLocation = new Point3D(Location.X + Utility.RandomList(-2, -1, 1, 2), Location.Y + Utility.RandomList(-2, -1, 1, 2), Z);
                dirt.MoveToWorld(dirtLocation, Map);
            }

            new BunnyHole().MoveToWorld(Location, Map);

            Animate(3, 5, 1, true, false, 0);
            
            for (int a = 1; a < 5; a++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(a), delegate
                {
                    if (this == null) return;
                    if (Deleted || !Alive) return;

                    Animate(3, 5, 1, true, false, 0);

                    Say("*digs*");
                    SpecialAbilities.HinderSpecialAbility(1.0, this, this, 1.0, 1, false, digSound, false, "", "");    

                    //Dirt
                    int extraDirtCount = Utility.RandomMinMax(2, 3);

                    for (int b = 0; b < extraDirtCount; b++)
                    {
                        Blood extraDirt = new Blood();
                        extraDirt.Name = "dirt";
                        extraDirt.ItemID = Utility.RandomList(7681, 7682);
                        Point3D extraDirtLocation = new Point3D(Location.X + Utility.RandomList(-2, -1, 1, 2), Location.Y + Utility.RandomList(-2, -1, 1, 2), Z);
                        extraDirt.MoveToWorld(extraDirtLocation, Map);
                    }
                });
            }

            Timer.DelayCall(TimeSpan.FromSeconds(5.0), delegate
            {
                if (this == null) return;
                if (Deleted || !Alive) return;

                Say("*poof*");

                Effects.PlaySound(Location, Map, 0x657);
                Effects.SendLocationParticles(EffectItem.Create(Location, Map, TimeSpan.FromSeconds(5)), 0x3728, 10, 10, 2023);

                Delete();
            });
        }

        public void SetTimedEgg(Point3D location, Map map, EggType eggType)
        {
            int eggHue = Hue = Utility.RandomMinMax(2, 362);           

            DecoyEasterEgg decoyEasterEgg = new DecoyEasterEgg();

            decoyEasterEgg.Hue = eggHue;
            decoyEasterEgg.MoveToWorld(location, map);

            int delayTime = Utility.RandomMinMax(1, 10);

            Effects.SendLocationParticles(EffectItem.Create(location, map, TimeSpan.FromSeconds(0.25)), 0x9B5, 10, delayTime * 25, eggHue, 0, 5029, 0);
            
            Timer.DelayCall(TimeSpan.FromSeconds(delayTime), delegate
            {
                if (decoyEasterEgg != null)
                {
                    if (!decoyEasterEgg.Deleted)
                    {
                        if (decoyEasterEgg.ParentEntity is PlayerMobile)
                        {
                            PlayerMobile pm_Owner = decoyEasterEgg.ParentEntity as PlayerMobile;
                            
                            pm_Owner.SendMessage("The eggs you gathered appear to be some sort of decoy, and they crumble in your backpack...");
                            pm_Owner.SendSound(0x134);
                        }

                        decoyEasterEgg.Delete();
                    }
                }

                int radius = 2;

                int effectSound = 0;

                switch (eggType)
                {
                    case EggType.Explosive: effectSound = 0x359; break;
                    case EggType.Entangle: effectSound = 0x211; break;
                    case EggType.Poison: effectSound = 0x22F; break;
                    case EggType.Ice: effectSound = 0x64F; break;
                    case EggType.Banish: effectSound = 0x655; break;
                    case EggType.Gust: effectSound = 0x64C; break;
                    case EggType.Bloody: effectSound = 0x62B; break;
                }                

                Effects.PlaySound(location, map, effectSound);

                Dictionary<Point3D, double> m_ExplosionLocations = new Dictionary<Point3D, double>();

                m_ExplosionLocations.Add(location, 0);

                for (int a = 1; a < radius + 1; a++)
                {
                    m_ExplosionLocations.Add(new Point3D(location.X - a, location.Y - a, location.Z), a);
                    m_ExplosionLocations.Add(new Point3D(location.X, location.Y - a, location.Z), a);
                    m_ExplosionLocations.Add(new Point3D(location.X + a, location.Y - a, location.Z), a);
                    m_ExplosionLocations.Add(new Point3D(location.X + a, location.Y, location.Z), a);
                    m_ExplosionLocations.Add(new Point3D(location.X + a, location.Y + a, location.Z), a);
                    m_ExplosionLocations.Add(new Point3D(location.X, location.Y + a, location.Z), a);
                    m_ExplosionLocations.Add(new Point3D(location.X - a, location.Y + a, location.Z), a);
                    m_ExplosionLocations.Add(new Point3D(location.X - a, location.Y, location.Z), a);
                }

                foreach (KeyValuePair<Point3D, double> pair in m_ExplosionLocations)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(pair.Value * .25), delegate
                    {
                        switch (eggType)
                        {
                            case EggType.Explosive:                                
                                Effects.SendLocationParticles(EffectItem.Create(pair.Key, map, TimeSpan.FromSeconds(0.5)), 0x36BD, 20, 10, 5044);
                            break;

                            case EggType.Entangle:
                                radius = 3;
                                Effects.SendLocationParticles(EffectItem.Create(pair.Key, map, TimeSpan.FromSeconds(.5)), 0x3973, 10, 50, 5029);
                            break;

                            case EggType.Poison:
                                Effects.SendLocationParticles(EffectItem.Create(pair.Key, map, TimeSpan.FromSeconds(0.5)), 0x372A, 10, 20, 59, 0, 5029, 0);
                            break;

                            case EggType.Ice:
                                radius = 3;
                                Effects.SendLocationParticles(EffectItem.Create(pair.Key, map, TimeSpan.FromSeconds(0.25)), 0x3779, 10, 20, 1153, 0, 5029, 0);
                            break;

                            case EggType.Banish:
                                radius = 3;
                                Effects.SendLocationParticles(EffectItem.Create(pair.Key, map, TimeSpan.FromSeconds(.25)), 0x3763, 10, 20, 2199, 0, 5029, 0);
                            break;

                            case EggType.Gust:
                                radius = 4;
                                Effects.SendLocationParticles(EffectItem.Create(pair.Key, map, TimeSpan.FromSeconds(.25)), 0x1FB2, 10, 20, 0, 0, 5029, 0);
                            break;

                            case EggType.Bloody:
                                Effects.SendLocationParticles(EffectItem.Create(pair.Key, map, TimeSpan.FromSeconds(.25)), Utility.RandomList(0x1645, 0x122A, 0x122B, 0x122C, 0x122D, 0x122E, 0x122F), 10, 20, 0, 0, 5029, 0);
                            break;
                        }
                    });
                }

                List<Mobile> m_TargetsHit = new List<Mobile>();

                IPooledEnumerable eable = map.GetMobilesInRange(location, radius);

                foreach (Mobile mobile in eable)
                {
                    if (mobile is EasterBunny)
                        continue;

                    if (!mobile.CanBeDamaged())
                        continue;
                   
                    m_TargetsHit.Add(mobile);
                }

                eable.Free();

                int targets = m_TargetsHit.Count;

                for (int a = 0; a < targets; a++)
                {
                    double damage = 0;

                    Mobile mobile = m_TargetsHit[a];

                    if (mobile == null) continue;
                    if (!mobile.Alive || mobile.Deleted) continue;

                    switch (eggType)
                    {
                        case EggType.Explosive:
                            damage = (double)(Utility.RandomMinMax(20, 40));

                            if (mobile is BaseCreature)
                                damage *= 1.5;

                            AOS.Damage(mobile, (int)damage, 0, 100, 0, 0, 0);
                        break;

                        case EggType.Entangle:
                            if (mobile != null)
                                SpecialAbilities.EntangleSpecialAbility(1.0, null, mobile, 1, 10, -1, true, "", "You are held in place!");
                        break;

                        case EggType.Poison:
                            Poison poison = Poison.GetPoison(Utility.RandomMinMax(2,4));
                            mobile.ApplyPoison(mobile, poison);
                        break;

                        case EggType.Ice:
                            damage = (double)(Utility.RandomMinMax(10, 20));

                            if (mobile is BaseCreature)
                                damage *= 1.5;
                                                        
                                SpecialAbilities.CrippleSpecialAbility(1.0, null, mobile, .5, 10, -1, true, "", "A blast of ice has slowed your actions!");

                            AOS.Damage(mobile, (int)damage, 0, 100, 0, 0, 0);
                        break;

                        case EggType.Gust:
                            SpecialAbilities.KnockbackSpecialAbility(1.0, Location, null, mobile, 40, 8, -1, "", "A gust of wind knocks you off your feet!");
                        break;

                        case EggType.Banish:
                            SpecialAbilities.HinderSpecialAbility(1.0, null, mobile, 1.0, 5, false, -1, false, "", "You cannot move or speak!");
            
                            mobile.Squelched = true;
                            mobile.Hidden = true;

                            Timer.DelayCall(TimeSpan.FromSeconds(5), delegate
                            {
                                if (mobile == null) return;

                                mobile.Squelched = false;
                                mobile.Hidden = false;
                            });
                        break;

                        case EggType.Bloody:
                            SpecialAbilities.BleedSpecialAbility(1.0, null, mobile, 40, 8.0, 0x44D, true, "", "You begin to bleed!");
                        break;
                    }
                }

            });
        }

        public override void OnDeath(Container c)
        {
            PackItem(new EasterBasket());

            base.OnDeath(c);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            writer.Write(m_EggsDropped);

            Delete();
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version >= 0)
            {
                m_EggsDropped = reader.ReadInt();
            }
        }

       public class BunnyHole : Item
        {
            [Constructable]
            public BunnyHole(): base(0x913)
            {
                Movable = false;
                Hue = 1;
                Name = "an easter bunny hole";

                Timer.DelayCall(TimeSpan.FromSeconds(30), delegate
                {
                    if (this != null)
                        Delete();
                });
            }

            public BunnyHole(Serial serial): base(serial)
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

                Delete();
            }
        }
    }
}