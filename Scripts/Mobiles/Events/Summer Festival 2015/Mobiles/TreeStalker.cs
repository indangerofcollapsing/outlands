using System;
using System.Collections;
using Server.Items;
using Server.Targeting;


namespace Server.Mobiles
{
    [CorpseName("an treestalker corpse")]
    public class TreeStalker : BaseCreature
    {
        public DateTime m_NextMushroomExplosionAllowed;
        public TimeSpan NextMushroomExplosionDelay = TimeSpan.FromSeconds(Utility.RandomMinMax(15, 30));

        [Constructable]
        public TreeStalker(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a treestalker";

            Body = 285;
            BaseSoundID = 461;

            Hue = Utility.RandomList(2001, 2526, 2527, 2528, 2515, 2207);

            SetStr(75);
            SetDex(25);
            SetInt(25);

            SetHits(600);

            SetDamage(15, 25);

            SetSkill(SkillName.Wrestling, 80);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 75);

            VirtualArmor = 75;

            Fame = 3500;
            Karma = -3500;
        }

        public override void SetUniqueAI()
        {
            UniqueCreatureDifficultyScalar = 2.1;
        }        

        public override bool AllowParagon { get { return false; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        public override void OnThink()
        {
            base.OnThink();
            
            if (Utility.RandomDouble() < 0.05 && DateTime.UtcNow > m_NextMushroomExplosionAllowed)
            {
                if (Combatant != null)
                {
                    SpecialAbilities.MushroomExplosionAbility(this, 6, 8, 0, 4, true);

                    m_NextMushroomExplosionAllowed = DateTime.UtcNow + NextMushroomExplosionDelay;
                }
            }            
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            if (willKill)
                SpecialAbilities.MushroomExplosionAbility(this, 8, 10, 0, 4, false);            

            base.OnDamage(amount, from, willKill);
        }

        public override bool OnBeforeDeath()
        {
            if (Utility.RandomMinMax(1, 50) == 1)
                PackItem(new TreeStalkerVine());

            if (Utility.RandomMinMax(1, 30) == 1)
                PackItem(new MagicSpringwood());

            PackItem(new Engines.Plants.Seed());
            PackItem(new Engines.Plants.Seed());
            PackItem(new FertileDirt(Utility.RandomMinMax(3, 6)));

            return base.OnBeforeDeath();
        }

        public override int GetAngerSound() { return 0x452; }
        public override int GetAttackSound() { return 0x453; }
        public override int GetHurtSound() { return 0x454; }
        public override int GetDeathSound() { return 0x455; }
        public override int GetIdleSound() { return 0x451; }

        public TreeStalker(Serial serial) : base(serial) { }

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
