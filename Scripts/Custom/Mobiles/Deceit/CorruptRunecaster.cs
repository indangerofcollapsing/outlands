using Server.Achievements;
using Server.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Mobiles
{
    [CorpseName("a corrupt runecaster's corpse")]
    public class CorruptRunecaster : BaseCorrupt
    {
        [Constructable]
        public CorruptRunecaster(): base()
        {
            Name = "a corrupt runecaster";

            SetStr(50);
            SetDex(25);
            SetInt(100);

            SetHits(200);
            SetMana(1000);

            SetDamage(5, 10);

            SetSkill(SkillName.Wrestling, 70);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Magery, 75);
            SetSkill(SkillName.EvalInt, 75);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.MagicResist, 75);

            VirtualArmor = 25;

            Fame = 3000;
            Karma = -3000;

            PackReg(23);

            Utility.AssignRandomHair(this, 0);

            AddItem(new Sandals() { Hue = 1175, Movable = false });
            AddItem(new BoneHelm() { Hue = 1766, Movable = false });
            AddItem(new BoneChest() { Hue = 1766, Movable = false });
            AddItem(new Kilt() { Hue = 1175, Movable = false });
            AddItem(new Cloak() { Hue = 1175, Movable = false });

            AddItem(new Spellbook() { Hue = 1175, Movable = false, Name = "a necromantic tome" });            
        }

        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        public override bool OnBeforeDeath()
        {
            AwardDailyAchievementForKiller(PvECategory.KillCorruptRunecasters);

            return base.OnBeforeDeath();
        }

        public override int Meat { get { return 1; } }

        public CorruptRunecaster(Serial serial): base(serial)
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
