using System;
using Server;
using Server.Items;
using Server.Spells;

namespace Server.Mobiles
{
    [CorpseName("a tunneler's corpse")]
    public class DeDOSTunneler : BaseCreature
    {
        public DateTime m_NextVanishAllowed;
        public TimeSpan NextVanishDelay = TimeSpan.FromSeconds(15);

        [Constructable]
        public DeDOSTunneler(): base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a tunneler";

            Body = 145;
            Hue = 23999;

            BaseSoundID = 447; 

            SetStr(100);
            SetDex(50);
            SetInt(25);

            SetHits(1500);
            SetDex(1000);

            SetDamage(15, 25);

            SetSkill(SkillName.Wrestling, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            SetSkill(SkillName.Stealth, 120);
            SetSkill(SkillName.Hiding, 120);

            VirtualArmor = 100;

            Fame = 2500;
            Karma = -2500;
        }

        public override bool AlwaysEventMinion { get { return true; } }
        public override bool AllowParagon { get { return false; } }

        public override void SetUniqueAI()
        {           
            DictWanderAction[WanderAction.None] = 1;
            DictWanderAction[WanderAction.Stealth] = 1;

            UniqueCreatureDifficultyScalar = .25;

            DictCombatFlee[CombatFlee.Flee50] = 0;
            DictCombatFlee[CombatFlee.Flee25] = 0;
            DictCombatFlee[CombatFlee.Flee10] = 0;
            DictCombatFlee[CombatFlee.Flee5] = 0;
        }
        
        public override bool IsHighSeasBodyType { get { return true; } }

        public override void OnThink()
        {
            base.OnThink();
            
            if (Utility.RandomDouble() < 0.05 && DateTime.UtcNow > m_NextVanishAllowed)
            {
                if (Combatant != null && !Hidden && !Paralyzed && !BardProvoked && !BardPacified)
                {
                    TimedStatic rock = new TimedStatic(Utility.RandomList(4967, 4970, 4973), 2);
                    rock.Name = "rock";
                    rock.MoveToWorld(Location, Map);

                    for (int a = 0; a < 6; a++)
                    {
                        Point3D dirtLocation = new Point3D(Location.X + Utility.RandomList(-1, 1), Location.Y + Utility.RandomList(-1, 1), Location.Z);

                        TimedStatic dirt = new TimedStatic(Utility.RandomList(7681, 7682), 5);
                        rock.Name = "dirt";
                        SpellHelper.AdjustField(ref dirtLocation, Map, 12, false);
                        rock.MoveToWorld(dirtLocation, Map);
                    }

                    if (SpecialAbilities.VanishAbility(this, 3.0, true, -1, 3, 6, true, null))
                        Say("*tunnels*");

                    m_NextVanishAllowed = DateTime.UtcNow + NextVanishDelay;
                }
            }            
        }

        protected override bool OnMove(Direction d)
        {
            StealthFootprintChance = 0;

            TimedStatic floorCrack = new TimedStatic(Utility.RandomList(6913, 6914, 6915, 6916, 6917, 6918, 6919, 6920), 5);
            floorCrack.Name = "floor crack";

            floorCrack.MoveToWorld(Location, Map);

            if (Utility.RandomDouble() <= .5)
                Effects.PlaySound(Location, Map, Utility.RandomList(0x11F, 0x120));

            return base.OnMove(d);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);            
        }

        public DeDOSTunneler(Serial serial): base(serial)
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
