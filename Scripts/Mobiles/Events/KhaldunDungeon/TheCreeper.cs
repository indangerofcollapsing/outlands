using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Misc;
using Server.Spells;
using Server.Spells.Seventh;
using Server.Spells.Sixth;
using Server.Spells.Third;
using Server.Network;


namespace Server.Mobiles
{
	[CorpseName( "the creeper's corpse" )]
	public class TheCreeper : BaseCreature
	{
        public DateTime m_NextAIChangeAllowed;
        public TimeSpan NextAIChangeDelay = TimeSpan.FromSeconds(30);

        public int damageIntervalThreshold = 1000;
        public int totalIntervals = 20;

        public int damageProgress = 0;
        public int intervalCount = 0;

		[Constructable]
		public TheCreeper() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
            Name = "The Creeper";

            Body = 313;
            BaseSoundID = 0xE0;

            Hue = 2575;

			SetStr(75);
			SetDex(25);
			SetInt(100);

			SetHits(20000);
            SetStam(5000);
            SetMana(20000);

			SetDamage(15, 30);

            SetSkill(SkillName.Wrestling, 85);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Magery, 150);
            SetSkill(SkillName.EvalInt, 150);
            SetSkill(SkillName.Meditation, 150);

			SetSkill(SkillName.MagicResist, 150);

			Fame = 10000;
			Karma = -10000;

			VirtualArmor = 25;
		}        

        public override void SetUniqueAI()
        {
            ResolveAcquireTargetDelay = 0.5;
            RangePerception = 24;            

            DictCombatHealSelf[CombatHealSelf.SpellHealSelf100] = 0;
            DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 0;
            DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 0;
            DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 0;
            DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 0;

            DictCombatAction[CombatAction.CombatHealSelf] = 0;

            DictWanderAction[WanderAction.None] = 0;
            DictWanderAction[WanderAction.SpellHealSelf100] = 0;
            DictWanderAction[WanderAction.SpellCureSelf] = 0;

            UniqueCreatureDifficultyScalar = 1.33;

            damageIntervalThreshold = (int)(Math.Round((double)HitsMax / (double)totalIntervals));
            intervalCount = (int)(Math.Floor((1 - (double)Hits / (double)HitsMax) * (double)totalIntervals));
        }

        public override bool AlwaysBoss { get { return true; } }
        public override bool AlwaysMurderer { get { return true; } }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            double creepChance = .20 + (.40 * spawnPercent);

            if (Utility.RandomDouble() <= creepChance)
            {  
                Blood creep = new Blood();
                creep.Hue = 2597;
                creep.Name = "creep";
                Point3D creepLocation = new Point3D(defender.X, defender.Y, defender.Z + 2);
                creep.MoveToWorld(creepLocation, defender.Map);

                int extraCreepCount = Utility.RandomMinMax(1, 2);

                for (int a = 0; a < extraCreepCount; a++)
                {
                    Blood extraCreep = new Blood();
                    extraCreep.Hue = 2597;
                    extraCreep.Name = "creep";
                    Point3D extraCreepLocation = new Point3D(defender.X + Utility.RandomList(-1, 1), defender.Y + Utility.RandomList(-1, 1), defender.Z + 2);
                    extraCreep.MoveToWorld(extraCreepLocation, defender.Map);
                }

                Effects.PlaySound(defender.Location, defender.Map, 0x4F1);
                defender.FixedParticles(0x374A, 10, 20, 5021, 2597, 0, EffectLayer.Head);

                defender.SendMessage("You have been covered in creep, slowing your actions!");

                SpecialAbilities.CrippleSpecialAbility(1.0, this, defender, .50, 15, -1, false, "", "");
            }
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            damageIntervalThreshold = (int)(Math.Round((double)HitsMax / (double)totalIntervals));
            intervalCount = (int)(Math.Floor((1 - (double)Hits / (double)HitsMax) * (double)totalIntervals));

            if (!willKill)
            {
                damageProgress += amount;

                if (damageProgress >= damageIntervalThreshold)
                {
                    Effects.PlaySound(Location, Map, GetAngerSound());

                    damageProgress = 0;
                }
            }

            base.OnDamage(amount, from, willKill);
        }

        public override void OnSpellCast(ISpell spell)
        {
            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            base.OnSpellCast(spell);

            double creepChance = .20 + (.40 * spawnPercent);

            if (Combatant != null && Utility.RandomDouble() <= creepChance)
            {
                if (InLOS(Combatant) && GetDistanceToSqrt(Combatant.Location) <= 12 && Combatant.Alive && !Combatant.Hidden)
                {
                    bool creepPlaced = false;

                    if (!PointHasCreep(Combatant.Location, Combatant.Map))
                    {
                        Creep creep = new Creep();
                        creep.MoveToWorld(Combatant.Location, Combatant.Map);

                        Effects.PlaySound(Location, Map, GetIdleSound());
                        Effects.PlaySound(Combatant.Location, Combatant.Map, 0x62C);

                        creepPlaced = true;
                    }

                    int extraCreep = (int)(Math.Ceiling(8 * spawnPercent));

                    for (int a = 0; a < extraCreep; a++)
                    {
                        int minRange = (int)(Math.Ceiling(-4 * spawnPercent));
                        int maxRange = (int)(Math.Ceiling(4 * spawnPercent));

                        Point3D extraCreepLocation = new Point3D(Combatant.Location.X + Utility.RandomMinMax(minRange, maxRange), Combatant.Location.Y + Utility.RandomMinMax(minRange, maxRange), Combatant.Location.Z);

                        if (!PointHasCreep(extraCreepLocation, Combatant.Map))
                        {
                            Creep creep = new Creep();
                            creep.MoveToWorld(extraCreepLocation, Combatant.Map);

                            Effects.PlaySound(Location, Map, GetIdleSound());
                            Effects.PlaySound(extraCreepLocation, Combatant.Map, 0x62C);

                            creepPlaced = true;
                        }
                    }

                    if (creepPlaced)
                        PublicOverheadMessage(MessageType.Label, 0, false, "*spreads creep*");
                }
            }
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

                Combatant = null;

                m_NextAIChangeAllowed = DateTime.UtcNow + NextAIChangeDelay;
            }            
        }

        public bool PointHasCreep(Point3D point, Map map)
        {
            bool foundCreep = false;

            IPooledEnumerable itemsNearPoint = map.GetItemsInRange(point, 1);

            foreach (Item item in itemsNearPoint)
            {
                if (item.Location != point) continue;
                if (item is Creep)
                {
                    foundCreep = true;
                    break;
                }
            }

            itemsNearPoint.Free();

            return foundCreep;
        }

        public override bool OnBeforeDeath()
        {
            int extraCreep = 25;

            for (int a = 0; a < extraCreep; a++)
            {
                Point3D extreCreepLocation = new Point3D(Location.X + Utility.RandomMinMax(-4, 4), Location.Y + Utility.RandomMinMax(-4, 4), Location.Z);

                if (!PointHasCreep(extreCreepLocation, Combatant.Map))
                {
                    Creep creep = new Creep();
                    creep.MoveToWorld(extreCreepLocation, Combatant.Map);

                    Effects.PlaySound(Location, Map, GetIdleSound());
                    Effects.PlaySound(extreCreepLocation, Combatant.Map, 0x62C);
                }
            }

            return base.OnBeforeDeath();
        }
        
        public override int GetAngerSound() { return 0x584; }
        public override int GetIdleSound() { return 0x383; }
        public override int GetAttackSound() { return 0x382; }
        public override int GetHurtSound() { return 0x385; }
        public override int GetDeathSound() { return 0x455; }   

        protected override bool OnMove(Direction d)
        {
            Effects.PlaySound(Location, Map, Utility.RandomList(0x382, 0x384));

            return base.OnMove(d);
        }

        public TheCreeper(Serial serial): base(serial)
		{
		}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);

            //Version 1
            writer.Write(damageProgress);
            writer.Write(intervalCount);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 1
            if (version >= 1)
            {
                damageProgress = reader.ReadInt();
                intervalCount = reader.ReadInt();
            }
        }
	}
}
