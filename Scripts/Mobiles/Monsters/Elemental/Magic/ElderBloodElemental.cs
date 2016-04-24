using System;
using Server;
using Server.Items;
using Server.Spells;
using Server.Spells.Fourth;


namespace Server.Mobiles
{
    [CorpseName("an elder blood elemental corpse")]
    public class ElderBloodElemental : BaseCreature
    {
        [Constructable]
        public ElderBloodElemental(): base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "an elder blood elemental";
            Body = 159;
            BaseSoundID = 278;

            SetStr(100);
            SetDex(75);
            SetInt(100);

            SetHits(1800);
            SetMana(3000);

            SetDamage(25, 40);

            SetSkill(SkillName.Wrestling, 110);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.MagicResist, 125);

            VirtualArmor = 25;

            Fame = 12500;
            Karma = -12500;            
        }

        public override void SetUniqueAI()
        {            
            UniqueCreatureDifficultyScalar = 1.12;
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);
                        
            int bloodItems = Utility.RandomMinMax(3, 6);

            for (int a = 0; a < bloodItems; a++)
            {
                Blood blood = new Blood();
                blood.ItemID = Utility.RandomList(4650, 4651, 4652, 4653, 4654, 4655);
                blood.MoveToWorld(new Point3D(defender.X + Utility.RandomMinMax(-1, 1), defender.Y + Utility.RandomMinMax(-1, 1), defender.Z), Map);
            }

            SpecialAbilities.BleedSpecialAbility(0.5, this, defender, DamageMax, 8.0, Utility.RandomList(0x5D9, 0x5DB), true, "", "Their attack causes you to bleed!", "-1");
        }

        protected override bool OnMove(Direction d)
        {            
            Blood blood = new Blood();
            blood.ItemID = Utility.RandomList(4650, 4651, 4652, 4653, 4654, 4655);

            blood.MoveToWorld(Location, Map);            

            return base.OnMove(d);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
        }

        public ElderBloodElemental(Serial serial): base(serial)
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
