using System;
using Server.Items;
using Server.Custom;

namespace Server.Mobiles
{
    public class LoHEnvelopingDarknessEvent : LoHEvent
    {
        public override Type CreatureType { get { return typeof(LoHEnvelopingDarkness); } }
        public override string DisplayName { get { return "Enveloping Darkness"; } }

        public override string AnnouncementText { get { return "An Enveloping Darkness has been sighted in the shifting sands of the northeast Britain desert!"; } }

        public override int RewardItemId { get { return 0; } }
        public override int RewardHue { get { return 2500; } }

        public override Point3D MonsterLocation { get { return new Point3D(1908, 837, -1); } }
        public override Point3D PortalLocation { get { return new Point3D(1887, 843, 7); } }
    }

    [CorpseName("an enveloping darkness corpse")]
    public class LoHEnvelopingDarkness : LoHMonster
    {
        [Constructable]
        public LoHEnvelopingDarkness(): base()
        {
            Name = "Enveloping Darkness";

            Body = 780;
            Hue = 23999;

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

        public override int GetAngerSound() { return 0x300; }
        public override int GetIdleSound() { return 0x301; }
        public override int GetAttackSound() { return 0x302; }
        public override int GetHurtSound() { return 0x303; }
        public override int GetDeathSound() { return 0x304; }

        public LoHEnvelopingDarkness(Serial serial): base(serial)
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
