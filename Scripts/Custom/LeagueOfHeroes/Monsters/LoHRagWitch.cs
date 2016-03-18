using System;
using Server.Items;
using Server.Custom;
using Server.Network;

namespace Server.Mobiles
{
    public class LoHRagWitchEvent : LoHEvent
    {
        public override Type CreatureType { get { return typeof(LoHRagWitch); } }
        public override string DisplayName { get { return "Rag Witch"; } }

        public override string AnnouncementText { get { return "A Rag Witch has been sighted in the shifting sands of the northeast Britain desert!"; } }

        public override int RewardItemId { get { return 0; } }
        public override int RewardHue { get { return 2500; } }

        public override Point3D MonsterLocation { get { return new Point3D(1908, 837, -1); } }
        public override Point3D PortalLocation { get { return new Point3D(1887, 843, 7); } }
    }

    [CorpseName("a rag witch corpse")]
    public class LoHRagWitch : LoHMonster
    {
        public DateTime m_NextVanishAllowed;
        public TimeSpan NextVanishDelay = TimeSpan.FromSeconds(10);

        [Constructable]
        public LoHRagWitch(): base()
        {
            Name = "Rag Witch";

            Body = 740;
            Hue = 2500;

            BaseSoundID = 0x482;

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

            VirtualArmor = 75;

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

        public override void OnThink()
        {
            base.OnThink();

            if (Utility.RandomDouble() < 0.05 && DateTime.UtcNow > m_NextVanishAllowed)
            {
                if (Combatant != null && !Hidden && !Paralyzed && !BardProvoked && !BardPacified)
                {
                    if (SpecialAbilities.VanishAbility(this, 1.0, true, 0x659, 4, 12, true, null))
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*vanishes*");

                    m_NextVanishAllowed = DateTime.UtcNow + NextVanishDelay;
                }
            }
        }

        public override int AttackAnimation { get { return 5; } }
        public override int AttackFrames { get { return 8; } }

        public override int GetAngerSound() { return 0x284; }
        public override int GetIdleSound() { return 0x285; }
        public override int GetAttackSound() { return 0x286; }
        public override int GetHurtSound() { return 0x287; }
        public override int GetDeathSound() { return 0x288; }

        public LoHRagWitch(Serial serial): base(serial)
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
