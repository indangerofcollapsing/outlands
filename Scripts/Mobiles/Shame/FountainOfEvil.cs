using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a fountain of evil corpse")]
    public class FountainOfEvil: BaseCreature
	{
		[Constructable]
		public FountainOfEvil () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a fountain of evil";
			Body = 16;
		    Hue = 1761;

            SetStr(100);
            SetDex(50);
            SetInt(100);

            SetHits(1800);
            SetMana(3000);

            SetDamage(20, 40);

            SetSkill(SkillName.Wrestling, 105);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.MagicResist, 150);

            VirtualArmor = 25;

			Fame = 4500;
			Karma = -4500;			

			CanSwim = true;
		}

        public override void SetUniqueAI()
        {
            UniqueCreatureDifficultyScalar = 1.15;
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);
            
            if (Utility.RandomDouble() < .33)
            {
                Blood ichor = new Blood();
                ichor.Hue = 2051;
                ichor.Name = "ichor";
                ichor.ItemID = Utility.RandomList(4650, 4651, 4652, 4653, 4654, 4655);
                ichor.MoveToWorld(defender.Location, Map);

                for (int a = 0; a < 4; a++)
                {
                    Blood blood = new Blood();
                    blood.Hue = 2051;
                    blood.Name = "ichor";
                    blood.ItemID = Utility.RandomList(4650, 4651, 4652, 4653, 4654, 4655);
                    blood.MoveToWorld(new Point3D(defender.X + Utility.RandomMinMax(-1, 1), defender.Y + Utility.RandomMinMax(-1, 1), defender.Z), Map);
                }

                Effects.PlaySound(defender.Location, defender.Map, 0x580);
                defender.FixedParticles(0x374A, 10, 20, 5021, 1107, 0, EffectLayer.Head);

                defender.SendMessage("You have been covered in an evil ichor!");

                SpecialAbilities.EntangleSpecialAbility(1.0, this, defender, 1.0, 3, -1, false, "", "");
                SpecialAbilities.BleedSpecialAbility(1.0, this, defender, DamageMax, 10, -1, false, "", "");
                SpecialAbilities.PierceSpecialAbility(1.0, this, defender, .33, 15, -1, false, "", "");
                SpecialAbilities.CrippleSpecialAbility(1.0, this, defender, .15, 15, -1, false, "", "");
                SpecialAbilities.StunSpecialAbility(1.0, this, defender, .10, 15, -1, false, "", "");
            }            
        }

        protected override bool OnMove(Direction d)
        {            
            Blood blood = new Blood();
            blood.Hue = 2051;
            blood.Name = "ichor";
            blood.ItemID = Utility.RandomList(4650, 4651, 4652, 4653, 4654, 4655);

            blood.MoveToWorld(Location, Map);

            Effects.PlaySound(Location, Map, Utility.RandomList(0x101));            

            return base.OnMove(d);
        }

        public override int GetAngerSound() { return Utility.RandomList(0x4B0, 0x4B1); }
        public override int GetIdleSound() { return Utility.RandomList(0x4B0, 0x4B1); }
        public override int GetAttackSound() { return 0x595; }
        public override int GetHurtSound() { return 0x4B4; }
        public override int GetDeathSound() { return 0x381; }

		public override void OnDeath( Container c )
		{			
    		base.OnDeath( c );
		}	

        public FountainOfEvil(Serial serial): base(serial)
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
