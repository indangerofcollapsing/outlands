using Server.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Mobiles
{
    [CorpseName("gralgurz the warboss' corpse")]
    public class OrcWarboss : BaseOrc
    {
        public DateTime m_NextAIChangeAllowed;
        public TimeSpan NextAIChangeDelay = TimeSpan.FromSeconds(30);

        [Constructable]
        public OrcWarboss(): base()
        {
            Name = "Gralgurz the Warboss";

            Body = 0x190;
            Hue = 2417;
            BaseSoundID = 0x45A;

            SetStr(100);
            SetDex(75);
            SetInt(25);

            SetHits(5000);
            SetStam(3000);

            SetDamage(30, 40);

            SetSkill(SkillName.Swords, 100);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 115);

            VirtualArmor = 50;

            Fame = 4500;
            Karma = -4500;

            AddItem(new OrcHelm() { Movable = false, Hue = 1175, Name = "Helm of the Warboss" });
            AddItem(new RingmailGloves() { Movable = false, Hue = 2406 });
            AddItem(new RingmailChest() { Movable = false, Hue = 2406 });
            AddItem(new RingmailLegs() { Movable = false, Hue = 2406 });
            AddItem(new PlateArms() { Movable = false, Hue = 2406 });
            AddItem(new Boots() { Movable = false, Hue = 1775 });
            AddItem(new Cloak() { Movable = false, Hue = 1808, Name = "Cloak of the Warboss" });
            AddItem(new BodySash() { Movable = false, Hue = 2118, Name = "Sash of the Warboss" });

            AddItem(new DoubleAxe() { Movable = false, Hue = 1779, Name = "Axe of the Warboss" });
        }

        public override void SetUniqueAI()
        {
            ResolveAcquireTargetDelay = 0.5;
            RangePerception = 24;

            UniqueCreatureDifficultyScalar = 1.52;
        }

        public override void OnAfterSpawn()
        {
            base.OnAfterSpawn();
            
            ElderWardr elderWardr = new ElderWardr();
            elderWardr.MoveToWorld(Location, Map);            
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
        }

        public override bool AlwaysMiniBoss { get { return true; } }
        public override bool AlwaysMurderer { get { return true; } }
        
        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
        }

        public OrcWarboss(Serial serial): base(serial)
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
