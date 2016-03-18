using System;
using Server.Items;
using Server.Network;
using Server.Engines.Harvest;
using Server.Mobiles;

namespace Server.Items
{
	[FlipableAttribute( 0x13B0, 0x13AF )]
	public class WarAxe : BaseAxe
	{
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.ArmorIgnore; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.BleedAttack; } }

        public override int AosStrengthReq { get { return 40; } }
        public override int AosMinDamage { get { return 11; } }
        public override int AosMaxDamage { get { return 13; } }
        public override int AosSpeed { get { return 44; } }
        public override float MlSpeed { get { return 2.50f; } }

        public override int OldStrengthReq { get { return 10; } }
        public override int OldMinDamage { get { return 12; } }
        public override int OldMaxDamage { get { return 20; } }
        public override int OldSpeed { get { return 50; } }

        public override int InitMinHits { get { return 30; } }
        public override int InitMaxHits { get { return 75; } }

        public override int IconItemId { get { return 5039; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return -3; } }
        public override int IconOffsetY { get { return -3; } }

		public override SkillName DefSkill{ get{ return SkillName.Macing; } }
		public override WeaponType DefType{ get{ return WeaponType.Bashing; } }
		public override WeaponAnimation DefAnimation{ get{ return WeaponAnimation.Bash1H; } }

		public override HarvestSystem HarvestSystem{ get{ return null; } }

		[Constructable]
		public WarAxe() : base( 0x13B0 )
		{
            Name = "war axe";

			Weight = 8.0;
		}

		public WarAxe( Serial serial ) : base( serial )
		{
		}

    public override void OnHit( Mobile attacker, Mobile defender, double damageBonus )
    {
      base.OnHit( attacker, defender, damageBonus );
                        
      int toReduce =  Utility.Random( 3, 3 );

      if (attacker is BaseCreature && defender is PlayerMobile)
          toReduce = (int)(Math.Floor((double)toReduce / 2));

      defender.Stam = defender.Stam > toReduce ? defender.Stam - toReduce : 1;
    }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

            Name = "war axe";
		}
	}
}