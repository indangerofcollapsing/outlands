using System;
using Server.Items;
using Server.Custom;
using Server.Spells;

namespace Server.Mobiles
{
    public class LoHEarthSerpentEvent : LoHEvent
    {
        public override Type CreatureType { get { return typeof(LoHEarthSerpent); } }
        public override string DisplayName { get { return "Earth Serpent"; } }

        public override string AnnouncementText { get { return "An Earth Serpent has been sighted in the shifting sands of the northeast Britain desert!"; } }

        public override int RewardItemId { get { return 0; } }
        public override int RewardHue { get { return 2500; } }

        public override Point3D MonsterLocation { get { return new Point3D(1908, 837, -1); } }
        public override Point3D PortalLocation { get { return new Point3D(1887, 843, 7); } }
    }

    [CorpseName("an earth serpent corpse")]
    public class LoHEarthSerpent : LoHMonster
    {
        public DateTime m_NextVanishAllowed;
        public TimeSpan NextVanishDelay = TimeSpan.FromSeconds(15);

        [Constructable]
        public LoHEarthSerpent(): base()
        {
            Name = "Earth Serpent";

            Body = 145;            
            Hue = 2500;

            BaseSoundID = 447; 

            SetStr(100);
            SetDex(75);
            SetInt(25);

            SetHits(10000);
            SetStam(10000);
            SetMana(50000);

            SetDamage(30, 50);

            SetSkill(SkillName.Wrestling, 120);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            SetSkill(SkillName.Stealth, 120);
            SetSkill(SkillName.Hiding, 120);

            VirtualArmor = 75;

            Fame = 10000;
            Karma = -10000;            
        }

        public override bool IsHighSeasBodyType { get { return true; } }

        public override void ConfigureCreature()
        {
            base.ConfigureCreature();
        }

        public override void SetUniqueAI()
        {
            base.SetUniqueAI();

            DictWanderAction[WanderAction.None] = 1;
            DictWanderAction[WanderAction.Stealth] = 1;
        }

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

        public LoHEarthSerpent(Serial serial): base(serial)
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
