using System;
using Server.Items;
using Server.Spells;
using System.Collections;
using System.Collections.Generic;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("a tree of life corpse")]
    public class TreeOfLife : BaseCreature
    {
        public DateTime m_NextMushroomExplosionAllowed;
        public TimeSpan NextMushroomExplosionDelay = TimeSpan.FromSeconds(Utility.RandomMinMax(10, 20));

        public DateTime m_NextEntangleAllowed;
        public TimeSpan NextEntangleDelay = TimeSpan.FromSeconds(Utility.RandomMinMax(10, 20));

        public int BranchItemId { get { return Utility.RandomList(3387, 3388, 3889); } }
        public int MushroomItemId { get { return Utility.RandomList(3340, 3341, 3342, 3343, 3344, 3345, 3346, 3347, 3348, 3349, 3350, 3351, 3352, 3353); } }
        public int GrassItemId { get { return Utility.RandomList(3220, 3255, 3256, 3223, 3245, 3246, 3248, 3254, 3257, 3258, 3259, 3260, 3261, 3269, 3270, 3267, 3237, 3267, 3239, 3332); } }
        public int FlowerItemId { get { return Utility.RandomList(3204, 3205, 3206, 3207, 3208, 3209, 3210, 3211, 3212, 3213, 3214, 3262, 3263, 3264, 3265,
                6809, 6810, 6811); } }
                   
        [Constructable]
        public TreeOfLife() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a tree of life";

            Body = 47;
            BaseSoundID = 442;
            Hue = Utility.RandomList(2001, 2526, 2527, 2528, 2515, 2207);

            SetStr(100);
            SetDex(50);
            SetInt(100);

            SetHits(10000);
            SetStam(5000);
            SetMana(10000);

            SetDamage(20, 40);

            SetSkill(SkillName.Wrestling, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 150);

            SetSkill(SkillName.Magery, 150);
            SetSkill(SkillName.EvalInt, 150);
            SetSkill(SkillName.Meditation, 200);

            VirtualArmor = 150;

            SpellHue = 2210;

            Fame = 10000;
            Karma = 0;            
        }

        public enum ExplosionType
        {
            Branch,
            Mushroom,
            Flower,
            PoisonousSap
        }

        public override bool AllowParagon { get { return false; } }
        public override int AttackRange { get { return 3; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        public override void SetUniqueAI()
        {
            ResolveAcquireTargetDelay = 0.5;
            RangePerception = 18;

            CombatEpicActionMinDelay = 20;
            CombatEpicActionMaxDelay = 30;

            DictCombatAction[CombatAction.CombatEpicAction] = 1;
            DictCombatEpicAction[CombatEpicAction.MassivePlantBreathAttack] = 25;

            UniqueCreatureDifficultyScalar = 1.33;

            MassiveBreathRange = 10;
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            base.OnDamage(amount, from, willKill);

            if (amount > 15 && !willKill)
            {
                if (Utility.RandomDouble() < .1)
                {
                    Point3D location = Location;
                    Map map = Map;

                    Effects.PlaySound(Location, Map, 0x36B);

                    int itemCount = Utility.Random(10, 20);

                    int minRadius = 0;
                    int maxRadius = 10;

                    List<Point3D> m_ValidLocations = SpecialAbilities.GetSpawnableTiles(location, true, true, location, map, itemCount, 25, minRadius, maxRadius, true);

                    if (m_ValidLocations.Count == 0)
                        return;

                    for (int a = 0; a < itemCount; a++)
                    {
                        Timer.DelayCall(TimeSpan.FromSeconds(a * .025), delegate
                        {
                            Point3D newLocation = m_ValidLocations[Utility.RandomMinMax(0, m_ValidLocations.Count - 1)];

                            SpellHelper.AdjustField(ref newLocation, map, 12, false);

                            Effects.PlaySound(newLocation, Map, 0x4F1);
                            
                            IEntity effectStartLocation = new Entity(Serial.Zero, location, map);
                            IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + 3), map);

                            double itemResult = Utility.RandomDouble();

                            ExplosionType m_ExplosionType = ExplosionType.Branch;

                            int itemId = 0;
                            string itemName = "";

                            if (itemResult < .25)
                            {
                                m_ExplosionType = ExplosionType.Branch;
                                itemId = BranchItemId;
                                itemName = "branches";
                            }

                            else if (itemResult < .5)
                            {
                                m_ExplosionType = ExplosionType.Mushroom;
                                itemId = MushroomItemId;
                                itemName = "mushrooms";
                            }

                            else if (itemResult < .75)
                            {
                                m_ExplosionType = ExplosionType.Flower;
                                itemId = FlowerItemId;
                                itemName = "flowers";
                            }

                            else
                            {
                                m_ExplosionType = ExplosionType.PoisonousSap;
                                itemId = Utility.RandomList(4651, 4652, 4653, 4654);
                            }

                            Effects.SendMovingEffect(effectStartLocation, effectEndLocation, itemId, 5, 0, false, false, 0, 0);

                            double distance = GetDistanceToSqrt(newLocation);
                            double destinationDelay = (double)distance * .08;

                            Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                            {
                                if (m_ExplosionType == ExplosionType.PoisonousSap)
                                    new PoisonousSap().MoveToWorld(newLocation, map);

                                else
                                {
                                    Blood explosionItem = new Blood();
                                    explosionItem.Name = itemName;
                                    explosionItem.ItemID = itemId;
                                    explosionItem.MoveToWorld(newLocation, map);
                                }                                
                            });
                        });
                    } 
                }
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            if (Utility.RandomDouble() < 0.05 && DateTime.UtcNow > m_NextMushroomExplosionAllowed)
            {
                if (Combatant != null)
                {
                    SpecialAbilities.MushroomExplosionAbility(this, 10, 12, 0, 6, true);

                    m_NextMushroomExplosionAllowed = DateTime.UtcNow + NextMushroomExplosionDelay;

                    return;
                }
            } 
        }
        
        protected override bool OnMove(Direction d)
        {
            Effects.PlaySound(Location, Map, Utility.RandomList(0x11F, 0x120));

            int items = Utility.RandomMinMax(1, 3);

            for (int a = 0; a < items; a++)
            {
                Point3D moveItemLocation = new Point3D(Location.X + Utility.RandomList(-1, 1), Location.Y + Utility.RandomList(-1, 1), Location.Z);
                SpellHelper.AdjustField(ref moveItemLocation, Map, 12, false);

                Blood moveItem = new Blood();
                moveItem.Name = "tall grass";
                moveItem.ItemID = GrassItemId;
                moveItem.MoveToWorld(moveItemLocation, Map);
            }

            return base.OnMove(d);
        }

        public override bool OnBeforeDeath()
        {
            Effects.PlaySound(Location, Map, 0x4CF);

            for (int a = 0; a < 100; a++)
            {
                Blood corpseItem = new Blood();

                double itemChance = Utility.RandomDouble();

                if (Utility.RandomDouble() < .2)
                {
                    corpseItem.Name = "branches";
                    corpseItem.ItemID = BranchItemId;
                }

                else if (Utility.RandomDouble() < .4)
                {
                    corpseItem.Name = "mushroom";
                    corpseItem.ItemID = MushroomItemId;
                }

                else
                {
                    corpseItem.Name = "flowers";
                    corpseItem.ItemID = FlowerItemId;
                }

                Point3D corpseItemLocation = new Point3D(Location.X + Utility.RandomMinMax(-8, 8), Location.Y + Utility.RandomMinMax(-8, 8), Location.Z);
                corpseItem.MoveToWorld(corpseItemLocation, Map);
            }

            PackItem(new Log(250));
            PackItem(new MandrakeRoot(250));
            PackItem(new FertileDirt(Utility.RandomMinMax(15, 25)));

            for (int a = 0; a < 25; a++)
            {
                PackItem(new Engines.Plants.Seed());
            }

            PackItem(new EntAppendage());

            PackItem(new MagicSpringwood());
            PackItem(new MagicSpringwood());
            PackItem(new MagicSpringwood());

            return base.OnBeforeDeath();
        }   

        public TreeOfLife(Serial serial): base(serial)
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