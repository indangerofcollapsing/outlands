using System;
using Server.Items;
using Server.Custom;

namespace Server.Mobiles
{
    public class LoHBoneDragonEvent : LoHEvent
    {
        public override Type CreatureType { get { return typeof(LoHBoneDragon); } }
        public override string DisplayName { get { return "Bone Dragon"; } }

        public override string AnnouncementText { get { return "A Bone Dragon has been sighted in the shifting sands of the northeast Britain desert!"; } }

        public override int RewardItemId { get { return 0; } }
        public override int RewardHue { get { return 2500; } }

        public override Point3D MonsterLocation { get { return new Point3D(1908, 837, -1); } }
        public override Point3D PortalLocation { get { return new Point3D(1887, 843, 7); } }
    }

    [CorpseName("a bone dragon corpse")]
    public class LoHBoneDragon : LoHMonster
    {
        public DateTime m_NextMassiveBreathAllowed;
        public TimeSpan NextMassiveBreathDelay = TimeSpan.FromSeconds(20);

        public DateTime m_NextFireBreathAllowed;
        public TimeSpan NextFireBreathDelay = TimeSpan.FromSeconds(10);

        public override int MinBaseHits { get { return 2000; } }
        public override int MaxBaseHits { get { return 2500; } }

        public override int MinExtraHitsPerPlayer { get { return 1000; } }
        public override int MaxExtraHitsPerPlayer { get { return 1250; } }

        [Constructable]
        public LoHBoneDragon(): base()
        {
            Name = "Bone Dragon";

            Body = 104;            
            Hue = 2500;

            BaseSoundID = 0x488;
            
            SetSkill(SkillName.Wrestling, 100);
            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 100;          
        }        

        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        public override bool CanFly { get { return true; } }

        public override void ConfigureCreature()
        {
            base.ConfigureCreature();
        }

        public override void SetUniqueAI()
        {
            base.SetUniqueAI();
        }

        public override void OnThink()
        {
            base.OnThink();

            MassiveBreathRange = (int)(Math.Round(3 + (5 * SpawnPercent)));

            if (Combatant != null && DateTime.UtcNow >= m_NextAbilityAllowed && !Frozen)
            {
                if (DateTime.UtcNow >= m_NextMassiveBreathAllowed && AICombatEpicAction.CanDoMassiveBoneBreathAttack(this))
                {
                    double totalDelay = 3;

                    SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, totalDelay, true, 0, false, "", "");

                    Effects.PlaySound(Location, Map, GetAngerSound());

                    Direction direction = Utility.GetDirection(Location, Combatant.Location);

                    SpecialAbilities.DoMassiveBreathAttack(this, Location, direction, MassiveBreathRange, true, BreathType.Bone, true);

                    m_NextMassiveBreathAllowed = DateTime.UtcNow + NextMassiveBreathDelay + TimeSpan.FromSeconds(totalDelay);
                    m_NextAbilityAllowed = DateTime.UtcNow + GetNextAbilityDelay();

                    NextCombatTime = DateTime.UtcNow + TimeSpan.FromSeconds(totalDelay + 2);
                    NextSpellTime = DateTime.UtcNow + TimeSpan.FromSeconds(totalDelay + 2);

                    return;
                }

                if (DateTime.UtcNow >= m_NextFireBreathAllowed && AICombatSpecialAction.CanDoFireBreathAttack(this))
                {
                    AICombatSpecialAction.DoFireBreathAttack(this, Combatant);

                    m_NextFireBreathAllowed = DateTime.UtcNow + NextFireBreathDelay;
                    m_NextAbilityAllowed = DateTime.UtcNow + GetNextAbilityDelay();

                    NextCombatTime = DateTime.UtcNow + TimeSpan.FromSeconds(3);
                    NextSpellTime = DateTime.UtcNow + TimeSpan.FromSeconds(3);

                    return;
                }
            }  
        }

        public LoHBoneDragon(Serial serial): base(serial)
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
