using System;
using Server.Items;
using Server.Custom;

namespace Server.Mobiles
{
    public class LoHSandWyvernEvent : LoHEvent
    {
        public override Type CreatureType { get { return typeof(LoHSandWyvern); } }
        public override string DisplayName { get { return "Sand Wyvern"; } }

        public override string AnnouncementText { get { return "A Sand Wyvern has been sighted in the shifting sands of the northeast Britain desert!"; } }

        public override int RewardItemId { get { return 0; } }
        public override int RewardHue { get { return 2503; } }

        public override Point3D MonsterLocation { get { return new Point3D(1908, 837, -1); } }
        public override Point3D PortalLocation { get { return new Point3D(1887, 843, 7); } }
    }

    [CorpseName("a sand wyvern corpse")]
    public class LoHSandWyvern : LoHMonster
    {
        [Constructable]
        public LoHSandWyvern(): base()
        {
            Name = "Sand Wyvern";

            Body = 62;
            Hue = 2503;

            BaseSoundID = 362;

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

            SetSkill(SkillName.Poisoning, 50);    

            VirtualArmor = 75;

            Fame = 10000;
            Karma = -10000;            
        }        

        public override Poison HitPoison { get { return Poison.Lethal; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        public override void ConfigureCreature()
        {
            base.ConfigureCreature();
        }

        public override void SetUniqueAI()
        {
            base.SetUniqueAI();           
        }

        public override int Meat { get { return 20; } }
        public override int Hides { get { return 50; } }
        public override HideType HideType { get { return HideType.Horned; } }
        public override bool CanFly { get { return true; } }      

        public override int GetAttackSound(){ return 713;}
        public override int GetAngerSound() { return 718;}
        public override int GetDeathSound() { return 716; }
        public override int GetHurtSound()  {return 721; }
        public override int GetIdleSound(){return 725;}

        public LoHSandWyvern(Serial serial): base(serial)
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
