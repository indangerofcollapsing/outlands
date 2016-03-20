using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a sprite corpse")]
    public class WoodlandSprite : BaseCreature
    {
        [Constructable]
        public WoodlandSprite(): base(AIType.AI_Generic, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a woodland sprite";

            Body = 184;
            Hue = 2542;

            HairItemID = 8252;
            HairHue = 2210;

            Female = true;

            SetStr(50);
            SetDex(75);
            SetInt(75);

            SetHits(300);
            SetStam(300);
            SetMana(1000);

            SetDamage(10, 20);

            SetSkill(SkillName.Archery, 85);
            SetSkill(SkillName.Fencing, 85);

            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            SetSkill(SkillName.Magery, 50);
            SetSkill(SkillName.EvalInt, 50);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 50;

            Fame = 3000;
            Karma = 0;
            
            AddItem(new Spear() { Movable = false, Hue = 2003, Layer = Layer.FirstValid, Name = "Woodland Spear"});
            AddItem(new WoodenKiteShield() { Movable = false, Hue = 2003, Name = "Woodland Shield" });

            AddItem(new LeatherBustierArms() { Movable = false, Hue = 2003, Name = "Woodland Sprite Chest Armor" });
            AddItem(new LeatherArms() { Movable = false, Hue = 2003, ItemID = 12232, Name = "Woodland Sprite Arms" });
            AddItem(new LeatherLegs() { Movable = false, Hue = 2003, ItemID = 12233, Name = "Woodland Sprite Legs" });
            AddItem(new Sandals() { Movable = false, Hue = 2003, Name = "Woodland Sprite Sandals" });            
        }

        public override void SetUniqueAI()
        {            
            UniqueCreatureDifficultyScalar = 1.25;

            SpellDelayMin *= 1.5;
            SpellDelayMax *= 1.5;
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < .2)
            {
                Effects.SendLocationParticles(EffectItem.Create(defender.Location, defender.Map, TimeSpan.FromSeconds(0.25)), 0x3779, 10, 20, 2003, 0, 5029, 0);

                SpecialAbilities.PierceSpecialAbility(1.0, this, defender, .25, 10, 0x510, true, "", "Their spear pierces your armor!");
            }
        }

        public override bool AllowParagon { get { return false; } }
        public override bool ShowFameTitle { get {  return false; } }
        public override bool AlwaysMurderer { get { return true; } }

        public override bool OnBeforeDeath()
        {
            if (Utility.RandomMinMax(1, 500) == 1)
                PackItem(new WoodlandSpriteSandals());

            if (Utility.RandomMinMax(1, 25) == 1)
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

        public WoodlandSprite(Serial serial) : base(serial) { }

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