using System;
using Server;
using Server.Items;
using Server.Spells;
using Server.Spells.Fourth;
using System.Collections;
using System.Collections.Generic;
using Server.Network;

namespace Server.Mobiles
{
	[CorpseName( "a lava elemental corpse" )]
	public class LavaElemental : BaseCreature
	{
        public DateTime m_NextFireBarrageAllowed;
        public TimeSpan NextFireBarrageDelay = TimeSpan.FromSeconds(20);

        public int LavaDuration = 600;
        public int LavaMinDamage = 3;
        public int LavaMaxDamage = 5;

		[Constructable]
		public LavaElemental () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a lava elemental";

			Body = 199;
            Hue = 2075;
            BaseSoundID = 838;

            SetStr(100);
            SetDex(50);
            SetInt(75);

            SetHits(350);
            SetMana(1000);

            SetDamage(15, 25);

            SetSkill(SkillName.Wrestling, 95);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Magery, 50);
            SetSkill(SkillName.EvalInt, 50);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.MagicResist, 100);

            VirtualArmor = 25;

			Fame = 10000;
			Karma = -10000;
		}

        public override bool AlwaysBossMinion { get { return true; } }
        public override bool AllowParagon { get { return false; } }
        public override bool BardImmune { get { return true; } }      

        public override void SetUniqueAI()
        {
            CastOnlyFireSpells = true;

            DictCombatTargetingWeight[CombatTargetingWeight.LeastCombatants] = 10;

            DictCombatFlee[CombatFlee.Flee50] = 0;
            DictCombatFlee[CombatFlee.Flee25] = 0;
            DictCombatFlee[CombatFlee.Flee10] = 0;
            DictCombatFlee[CombatFlee.Flee5] = 0;
           
            UniqueCreatureDifficultyScalar = 1.25;
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);
           
            if (Utility.RandomDouble() < .1 && defender != null)
            {
                PlaySound(0x1DD);

                int impactItemId = 0x3709;
                int impactHue = 2074;

                Effects.SendLocationParticles(EffectItem.Create(defender.Location, defender.Map, EffectItem.DefaultDuration), impactItemId, 20, 20, impactHue, 0, 0, 0);

                LavaField lavaField = new LavaField(this, 0, 1, LavaDuration, LavaMinDamage, LavaMaxDamage, false, false, true, -1, true);
                lavaField.MoveToWorld(defender.Location, defender.Map);
            }            
        }

        public void FireBarrage()
        {
            if (!SpecialAbilities.Exists(this))
                return;

            int range = 12;

            Mobile mobileTarget = Combatant;

            if (mobileTarget == null) return;
            if (!SpecialAbilities.MonsterCanDamage(this, mobileTarget)) return;
            if (mobileTarget.Hidden) return;
            if (!Map.InLOS(Location, mobileTarget.Location)) return;
            if (Utility.GetDistance(Location, mobileTarget.Location) > range) return;

            m_NextFireBarrageAllowed = DateTime.UtcNow + NextFireBarrageDelay;
            
            int fireballs = 8;

            double directionDelay = .25;
            double initialDelay = 1;
            double fireballDelay = .1;
            double totalDelay = 1 + directionDelay + initialDelay + ((double)fireballs * fireballDelay);

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, totalDelay, true, 0, false, "", "", "-1");
            
            PublicOverheadMessage(MessageType.Regular, 0, false, "*ignites*");

            Point3D location = Location;
            Map map = Map;

            Point3D targetLocation = mobileTarget.Location;
            Map targetMap = mobileTarget.Map;

            Direction = Utility.GetDirection(Location, targetLocation);

            Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;

                Animate(6, 4, 1, true, false, 0);
                PlaySound(GetAngerSound());               

                for (int a = 0; a < fireballs; a++)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(a * fireballDelay), delegate
                    {
                        if (!SpecialAbilities.Exists(this)) 
                            return;                       

                        bool mobileTargetValid = true;

                        if (mobileTarget == null)
                            mobileTargetValid = false;

                        else if (mobileTarget.Deleted || !mobileTarget.Alive)
                            mobileTargetValid = false;

                        else
                        {
                            if (mobileTarget.Hidden || Utility.GetDistance(Location, mobileTarget.Location) >= range * 2)
                                mobileTargetValid = false;
                        }

                        if (mobileTargetValid)
                        {
                            targetLocation = mobileTarget.Location;
                            targetMap = mobileTarget.Map;
                        }

                        int effectSound = 0x357;
                        int itemID = 0x36D4;
                        int itemHue = 0;

                        int impactSound = 0x226;
                        int impactHue = 0;

                        int xOffset = 0;
                        int yOffset = 0;

                        int distance = Utility.GetDistance(Location, targetLocation);

                        if (distance > 1)
                        {
                            if (Utility.RandomDouble() <= .5)
                                xOffset = Utility.RandomList(-1, 1);

                            if (Utility.RandomDouble() <= .5)
                                yOffset = Utility.RandomList(-1, 1);
                        }

                        IEntity startLocation = new Entity(Serial.Zero, new Point3D(location.X, location.Y, location.Z + 10), map);

                        Point3D adjustedLocation = new Point3D(targetLocation.X + xOffset, targetLocation.Y + yOffset, targetLocation.Z);
                        SpellHelper.AdjustField(ref adjustedLocation, targetMap, 12, false);

                        IEntity endLocation = new Entity(Serial.Zero, new Point3D(adjustedLocation.X, adjustedLocation.Y, adjustedLocation.Z + 10), targetMap);

                        Effects.PlaySound(location, map, effectSound);
                        Effects.SendMovingEffect(startLocation, endLocation, itemID, 8, 0, false, false, itemHue, 0);

                        double targetDistance = Utility.GetDistanceToSqrt(location, adjustedLocation);
                        double destinationDelay = (double)targetDistance * .06;

                        Direction newDirection = Utility.GetDirection(location, adjustedLocation);

                        if (Direction != newDirection)
                            Direction = newDirection;

                        Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                        {
                            Effects.PlaySound(adjustedLocation, targetMap, impactSound);
                            Effects.SendLocationParticles(EffectItem.Create(adjustedLocation, targetMap, EffectItem.DefaultDuration), 0x3709, 20, 20, impactHue, 0, 0, 0);

                            Queue m_Queue = new Queue();

                            IPooledEnumerable nearbyMobiles = targetMap.GetMobilesInRange(adjustedLocation, 0);

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

                                int minDamage = (int)(Math.Round((double)DamageMin / 2));
                                int maxDamage = DamageMin;

                                int damage = Utility.RandomMinMax(minDamage, maxDamage);

                                if (mobile is BaseCreature)
                                    damage *= 3;

                                else
                                {
                                    if (Utility.GetDistance(Location, mobile.Location) <= 1)
                                        damage = (int)(Math.Round((double)damage * .5));
                                }

                                DoHarmful(mobile);

                                new Blood().MoveToWorld(mobile.Location, mobile.Map);
                                AOS.Damage(mobile, this, damage, 100, 0, 0, 0, 0);
                            }
                        });
                    });
                }                
            });
        }

        public override void OnThink()
        {
            if (Combatant != null && DateTime.UtcNow >= m_NextFireBarrageAllowed)            
                FireBarrage();            

            base.OnThink();
        }

        protected override bool OnMove(Direction d)
        {
            if (Utility.RandomDouble() <= .10)
            {
                PlaySound(0x1DD);

                int impactItemId = 0x3709;
                int impactHue = 2074;

                Effects.SendLocationParticles(EffectItem.Create(Location, Map, EffectItem.DefaultDuration), impactItemId, 20, 20, impactHue, 0, 0, 0);

                LavaField lavaField = new LavaField(this, 0, 1, LavaDuration, LavaMinDamage, LavaMaxDamage, false, false, true, -1, true);
                lavaField.MoveToWorld(Location, Map);
            }

            return base.OnMove(d);
        }

        public override bool OnBeforeDeath()
        {
            PlaySound(0x56D);

            int impactItemId = 0x3709;
            int impactHue = 2074;

            for (int a = 0; a < 5; a++)
            {
                Point3D newLocation = new Point3D(Location.X + Utility.RandomMinMax(-2, 2), Location.Y + Utility.RandomMinMax(-2, 2), Location.Z);
                newLocation.Z = Map.GetSurfaceZ(Location, 30);

                LavaField lavaField = new LavaField(this, 0, 1, LavaDuration, LavaMinDamage, LavaMaxDamage, false, false, true, -1, true);
                lavaField.MoveToWorld(newLocation, Map);
                
                Effects.SendLocationParticles(EffectItem.Create(newLocation, Map, EffectItem.DefaultDuration), impactItemId, 20, 20, impactHue, 0, 0, 0);
            }

            return base.OnBeforeDeath();
        }
        
        public override void OnDeath(Container c)
        {
            Timer.DelayCall(TimeSpan.FromSeconds(3), delegate
            {
                if (c == null) return;
                if (c.Deleted) return;

                c.ItemID = 8198;
            });

            base.OnDeath(c);
        }

        public LavaElemental(Serial serial): base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}
