using System;
using Server.Items;
using Server.Custom;

namespace Server.Mobiles
{
    public class LoHMarrowfiendEvent : LoHEvent
    {
        public override Type CreatureType { get { return typeof(LoHMarrowfiend); } }
        public override string DisplayName { get { return "Marrowfiend"; } }

        public override string AnnouncementText { get { return "A Marrowfiend has been sighted in the shifting sands of the northeast Britain desert!"; } }

        public override int RewardItemId { get { return 0; } }
        public override int RewardHue { get { return 2619; } }

        public override Point3D MonsterLocation { get { return new Point3D(1908, 837, -1); } }
        public override Point3D PortalLocation { get { return new Point3D(1887, 843, 7); } }
    }

    [CorpseName("a marrowfiend corpse")]
    public class LoHMarrowfiend : LoHMonster
    {
        [Constructable]
        public LoHMarrowfiend(): base()
        {
            Name = "Marrowfiend";

            Body = 314;
            Hue = 2575;

            BaseSoundID = 357;

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
        
        public LoHMarrowfiend(Serial serial): base(serial)
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
