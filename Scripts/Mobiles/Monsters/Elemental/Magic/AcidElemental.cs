using System;
using Server;
using Server.Items;
using Server.Spells;
using Server.Spells.Fourth;
using Server.Spells.Sixth;

namespace Server.Mobiles
{
	[TypeAlias( "Server.Mobiles.ToxicElemental" )]
	[CorpseName( "an acid elemental corpse" )]
	public class AcidElemental : BaseCreature
	{
		[Constructable]
		public AcidElemental () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "an acid elemental";
			Body = 159;
			Hue = 2006;
			BaseSoundID = 263;

            SetStr(50);
            SetDex(50);
            SetInt(75);

            SetHits(400);
            SetMana(1000);

            SetDamage(10, 20);

            SetSkill(SkillName.Wrestling, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            SetSkill(SkillName.Magery, 50);
            SetSkill(SkillName.EvalInt, 50);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.Poisoning, 30);

            VirtualArmor = 25;

			Fame = 12500;
			Karma = -12500;
		}

        public override void SetUniqueAI()
        {
            UniqueCreatureDifficultyScalar = 1.05;
        }

        public override Poison HitPoison { get { return Poison.Deadly; } }
        public override int PoisonResistance { get { return 4; } }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            SpecialAbilities.PierceSpecialAbility(.25, this, defender, 50, 15, -1, true, "", "Their acid momentarily weakens your armor!", "-1");
                        
            Acid acid = new Acid();
            acid.MoveToWorld(defender.Location, Map);

            Effects.PlaySound(defender.Location, Map, Utility.RandomList(0x22F));

            int acidAmount = Utility.RandomMinMax(0, 1);

            for (int a = 0; a < acidAmount; a++)
            {
                Acid extraAcid = new Acid();
                extraAcid.MoveToWorld(new Point3D(defender.X + Utility.RandomMinMax(-1, 1), defender.Y + Utility.RandomMinMax(-1, 1), defender.Z), Map);
            }            
        }

        protected override bool OnMove(Direction d)
        {            
            if (Utility.RandomDouble() <= .5)
            {
                Acid acid = new Acid();
                acid.MoveToWorld(Location, Map);

                Effects.PlaySound(Location, Map, Utility.RandomList(0x5D9, 0x5DB));
            }            

            return base.OnMove(d);
        }

		public AcidElemental( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}