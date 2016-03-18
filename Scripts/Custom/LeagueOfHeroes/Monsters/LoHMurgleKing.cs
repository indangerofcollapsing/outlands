using System;
using Server.Items;
using Server.Custom;

namespace Server.Mobiles
{
    public class LoHMurgleKingEvent : LoHEvent
    {
        public override Type CreatureType { get { return typeof(LoHMurgleKing); } }
        public override string DisplayName { get { return "Murgle King"; } }

        public override string AnnouncementText { get { return "A Murgle King has been sighted in the shifting sands of the northeast Britain desert!"; } }

        public override int RewardItemId { get { return 0; } }
        public override int RewardHue { get { return 2500; } }

        public override Point3D MonsterLocation { get { return new Point3D(1908, 837, -1); } }
        public override Point3D PortalLocation { get { return new Point3D(1887, 843, 7); } }
    }

    [CorpseName("a murgle king corpse")]
    public class LoHMurgleKing : LoHMonster
    {
        [Constructable]
        public LoHMurgleKing(): base()
        {
            Name = "Murgle King";

            Body = 796;            
            Hue = 2611;

            BaseSoundID = 684;

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

        public override int GetAngerSound() { return 0x254; }
        public override int GetIdleSound() { return 0x253; }
        public override int GetAttackSound() { return 0x615; }
        public override int GetHurtSound() { return 0x613; }
        public override int GetDeathSound() { return 0x256; }

        public LoHMurgleKing(Serial serial): base(serial)
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
