using System;
using Server.Items;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("an earthly tendril corpse")]
    public class EarthlyTendril : BaseCreature
    {
        [Constructable]
        public EarthlyTendril() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "an earthly tendril";

            Body = 8;
            Hue = Utility.RandomList(2526, 2527, 2528, 2515, 2207);
            BaseSoundID = 684;

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(300);

            SetDamage(9, 18);

            SetSkill(SkillName.Wrestling, 75);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

            Fame = 2000;
            Karma = 0;
        }

        public override void SetUniqueAI()
        {
            UniqueCreatureDifficultyScalar = 1.6;
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < .15)
            {
                defender.PublicOverheadMessage(MessageType.Regular, 0, false, "*wrapped in vines*");
                

                Point3D newLocation = defender.Location;
                newLocation.Z += 2;

                Effects.SendLocationParticles(EffectItem.Create(newLocation, defender.Map, TimeSpan.FromSeconds(5.0)), Utility.RandomList(3241), 0, 125, 0, 0, 5029, 0);

                for (int a = 0; a < 6; a++)
                {
                    newLocation = new Point3D(defender.Location.X + Utility.RandomList(-1, 1), defender.Location.Y + Utility.RandomList(-1, 1), defender.Z);
                    
                    Effects.SendLocationParticles(EffectItem.Create(newLocation, defender.Map, TimeSpan.FromSeconds(5.0)), Utility.RandomList(3257, 3258, 3260, 3261, 3267, 3269), 0, 125, 0, 0, 5029, 0);
                }

                SpecialAbilities.HinderSpecialAbility(1.0, this, defender, 1.0, Utility.RandomMinMax(2, 4), false, 0x580, false, "", "The creature wraps you with its vines!");
            }            
        }

        public override bool AllowParagon { get { return false; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        public override bool OnBeforeDeath()
        {
            if (Utility.RandomMinMax(1, 50) == 1)
                PackItem(new MagicSpringwood());

            if (Utility.RandomMinMax(1, 75) == 1)
                PackItem(new MossyRock());

            if (0.25 > Utility.RandomDouble())
                PackItem(new Board(10));
            else
                PackItem(new Log(10));

            PackItem(new MandrakeRoot(3));
            PackItem(new Engines.Plants.Seed());
            PackItem(new FertileDirt(Utility.RandomMinMax(2, 6)));

            return base.OnBeforeDeath();
        }

        public EarthlyTendril(Serial serial) : base(serial) { }        

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