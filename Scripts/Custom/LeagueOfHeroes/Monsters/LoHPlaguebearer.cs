using System;
using Server.Items;
using Server.Custom;

namespace Server.Mobiles
{
    public class LoHPlaguebearerEvent : LoHEvent
    {
        public override Type CreatureType { get { return typeof(LoHPlaguebearer); } }
        public override string DisplayName { get { return "Plaguebearer"; } }

        public override string AnnouncementText { get { return "A Plaguebearer has been sighted in the shifting sands of the northeast Britain desert!"; } }

        public override int RewardItemId { get { return 0; } }
        public override int RewardHue { get { return 2619; } }

        public override Point3D MonsterLocation { get { return new Point3D(1908, 837, -1); } }
        public override Point3D PortalLocation { get { return new Point3D(1887, 843, 7); } }
    }

    [CorpseName("a plaguebearer corpse")]
    public class LoHPlaguebearer : LoHMonster
    {
        [Constructable]
        public LoHPlaguebearer(): base()
        {
            Name = "Plaguebearer";

            Body = 775;
            Hue = 2963;

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

        public override int GetAngerSound() { return 0x582; }
        public override int GetIdleSound() { return 0x581; }
        public override int GetAttackSound() { return 0x60E; }
        public override int GetHurtSound() { return 0x610; }
        public override int GetDeathSound() { return 0x57F; }

        protected override bool OnMove(Direction d)
        {            
            TimedStatic slime = new TimedStatic(Utility.RandomList(4650, 4651, 4652, 4653, 4654, 4655), 5);
            slime.Hue = 2963;
            slime.Name = "slime";

            slime.MoveToWorld(Location, Map);

            Effects.PlaySound(Location, Map, Utility.RandomList(0x5D9, 0x5DB));            

            return base.OnMove(d);
        }
        
        public LoHPlaguebearer(Serial serial): base(serial)
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
