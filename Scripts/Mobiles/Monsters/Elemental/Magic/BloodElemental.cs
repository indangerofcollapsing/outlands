using System;
using Server;
using Server.Items;
using Server.Spells;
using Server.Spells.Fourth;
using Server.Achievements;

namespace Server.Mobiles
{
    [CorpseName("a blood elemental corpse")]
    public class BloodElemental : BaseCreature
    {
        [Constructable]
        public BloodElemental()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a blood elemental";
            Body = 159;
            BaseSoundID = 278;

            SetStr(100);
            SetDex(75);
            SetInt(100);

            SetHits(850);
            SetMana(2000);

            SetDamage(20, 35);

            SetSkill(SkillName.Wrestling, 100);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Magery, 75);
            SetSkill(SkillName.EvalInt, 75);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.MagicResist, 125);

            VirtualArmor = 25;

            Fame = 12500;
            Karma = -12500;            
        }

        public override void SetUniqueAI()
        {
            if (Global_AllowAbilities)
                UniqueCreatureDifficultyScalar = 1.1;
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Global_AllowAbilities)
            {
                int bloodItems = Utility.RandomMinMax(3, 6);

                for (int a = 0; a < bloodItems; a++)
                {
                    Blood blood = new Blood();
                    blood.ItemID = Utility.RandomList(4650, 4651, 4652, 4653, 4654, 4655);
                    blood.MoveToWorld(new Point3D(defender.X + Utility.RandomMinMax(-1, 1), defender.Y + Utility.RandomMinMax(-1, 1), defender.Z), Map);
                }
            }

            SpecialAbilities.BleedSpecialAbility(0.25, this, defender, DamageMax, 8.0, Utility.RandomList(0x5D9, 0x5DB), true, "", "Their attack causes you to bleed!");
        }

        protected override bool OnMove(Direction d)
        {
            if (Global_AllowAbilities)
            {
                Blood blood = new Blood();
                blood.ItemID = Utility.RandomList(4650, 4651, 4652, 4653, 4654, 4655);

                blood.MoveToWorld(Location, Map);
            }

            return base.OnMove(d);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            // IPY ACHIEVEMENT TRIGGER 
            AwardAchievementForKiller(AchievementTriggers.Trigger_BloodElementalKilled);
            AwardDailyAchievementForKiller(PvECategory.KillBloodElementals);
            // END IPY ACHIEVEMENT TRIGGER

            switch (Utility.Random(1000))
            {
                case 0: { c.AddItem(SpellScroll.MakeMaster(new EnergyVortexScroll())); } break;
            }
        }

        public BloodElemental(Serial serial): base(serial)
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
