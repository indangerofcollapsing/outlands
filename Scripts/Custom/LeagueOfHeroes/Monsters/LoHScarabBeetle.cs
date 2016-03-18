using System;
using Server.Items;
using Server.Custom;

namespace Server.Mobiles
{
    public class LoHScarabBeetleEvent : LoHEvent
    {
        public override Type CreatureType { get { return typeof(LoHScarabBeetle); } }
        public override string DisplayName { get { return "Scarab Beetle"; } }

        public override string AnnouncementText { get { return "A Scarab Beetle has been sighted in the shifting sands of the northeast Britain desert!"; } }

        public override int RewardItemId { get { return 0; } }
        public override int RewardHue { get { return 2500; } }

        public override Point3D MonsterLocation { get { return new Point3D(1908, 837, -1); } }
        public override Point3D PortalLocation { get { return new Point3D(1887, 843, 7); } }
    }

    [CorpseName("a scarab beetle corpse")]
    public class LoHScarabBeetle : LoHMonster
    {
        [Constructable]
        public LoHScarabBeetle(): base()
        {
            Name = "Scarab Beetle";

            Body = 244;            
            Hue = 2214;

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

        public override int GetAngerSound() { return 0x4F3; }
        public override int GetIdleSound() { return 0x4F2; }
        public override int GetAttackSound() { return 0x607; }
        public override int GetHurtSound() { return 0x608; }
        public override int GetDeathSound() { return 0x4F0; }

        public LoHScarabBeetle(Serial serial): base(serial)
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
