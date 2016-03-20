using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an elder sprite corpse")]
    public class ElderWoodlandSprite : BaseCreature
    {
        [Constructable]
        public ElderWoodlandSprite(): base(AIType.AI_Generic, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "an elder woodland sprite";

            Body = 184;
            Hue = 2542;

            HairItemID = 8252;
            HairHue = 2210;

            Female = true;

            SetStr(75);
            SetDex(75);
            SetInt(100);

            SetHits(500);
            SetStam(500);
            SetMana(2000);

            SetDamage(15, 25);

            SetSkill(SkillName.Archery, 90);
            SetSkill(SkillName.Fencing, 90);

            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 125);

            SetSkill(SkillName.Magery, 75);
            SetSkill(SkillName.EvalInt, 75);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 75;

            SpellHue = 2210;

            Fame = 1000;
            Karma = 0;
           
            AddItem(new Bow() { Movable = false, Hue = 2003, Name = "Woodland Bow" });
            AddItem(new Cloak() { Movable = false, Hue = 0, ItemID = 12215, Name = "Woodland Quiver" });
                    
            PackItem(new Bandage(Utility.RandomMinMax(7, 18)));
            PackItem(new Arrow(Utility.RandomMinMax(20, 35)));

            AddItem(new LeatherBustierArms() { Movable = false, Hue = 2003, Name = "Woodland Sprite Chest Armor" });
            AddItem(new LeatherArms() { Movable = false, Hue = 2003, ItemID = 12232, Name = "Woodland Sprite Arms" });
            AddItem(new LeatherLegs() { Movable = false, Hue = 2003, ItemID = 12233, Name = "Woodland Sprite Legs" });
            AddItem(new Sandals() { Movable = false, Hue = 2003, Name = "Woodland Sprite Sandals" });            
        }
        
        public override void SetUniqueAI()
        {            
            UniqueCreatureDifficultyScalar = 1.25;

            SetSubGroup(AISubgroup.GroupHealerMage3);
            UpdateAI(false);

            SpellDelayMin *= 2;
            SpellDelayMax *= 2;

            CombatHealActionMinDelay = 15;
            CombatHealActionMaxDelay = 30;
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < .2)
            {
                Effects.SendLocationParticles(EffectItem.Create(defender.Location, defender.Map, TimeSpan.FromSeconds(0.25)), 0x3779, 10, 20, 2003, 0, 5029, 0);

                SpecialAbilities.CourageSpecialAbility(1.0, this, defender, .05, 30, -1, false, "", "", "*marks target*");
            }
        }

        public override bool AllowParagon { get { return false; } }
        public override bool ShowFameTitle { get {  return false; } }
        public override bool AlwaysMurderer { get { return true; } }

        public override bool OnBeforeDeath()
        {
            if (Utility.RandomMinMax(1, 250) == 1)
                PackItem(new WoodlandSpriteSandals());            

            if (Utility.RandomMinMax(1, 18) == 1)
            {
                PouchOfGypsyGoods rewardPouch = new PouchOfGypsyGoods();

                rewardPouch.Name = "a stolen pouch of gyspy goods";
                PackItem(rewardPouch);
            }

            return base.OnBeforeDeath();
        }

        //public override int GetAttackSound() { return 0x467; }
        public override int GetHurtSound() { return 0x375; }
        public override int GetAngerSound() { return 0x370; }
        public override int GetIdleSound() { return 0x372; }
        public override int GetDeathSound() { return 0x376; }

        public ElderWoodlandSprite(Serial serial) : base(serial) { }

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