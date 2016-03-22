using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Items
{
	public abstract class BaseBashing : BaseMeleeWeapon
	{
		public override int BaseHitSound{ get{ return 0x233; } }
		public override int BaseMissSound{ get{ return 0x239; } }

		public override SkillName BaseSkill{ get{ return SkillName.Macing; } }
		public override WeaponType BaseType{ get{ return WeaponType.Bashing; } }
		public override WeaponAnimation BaseAnimation { get{ return WeaponAnimation.Bash1H; } }

		public BaseBashing( int itemID ) : base( itemID )
		{
		}

		public BaseBashing( Serial serial ) : base( serial )
		{
		}

		public override WeaponAnimation GetAnimation()
		{
			WeaponAnimation animation = WeaponAnimation.Slash1H;

			Mobile attacker = this.Parent as Mobile;

			if (attacker != null)
			{
				if (attacker.FindItemOnLayer(Layer.TwoHanded) is BaseShield)
				{
					switch (Utility.RandomMinMax(1, 7))
					{
                        case 1: animation = WeaponAnimation.Bash1H; break;
                        case 2: animation = WeaponAnimation.Bash1H; break;
                        case 3: animation = WeaponAnimation.Slash1H; break;
                        case 4: animation = WeaponAnimation.Slash2H; break;
                        case 5: animation = WeaponAnimation.Pierce1H; break;
                        case 6: animation = WeaponAnimation.Pierce2H; break;
                        case 7: animation = WeaponAnimation.Bash2H; break;
					}

                    return animation;
				}

				else if (attacker.FindItemOnLayer(Layer.TwoHanded) != null)
				{
					switch (Utility.RandomMinMax(1, 5))
					{
                        case 1: animation = WeaponAnimation.Bash2H; break;
                        case 2: animation = WeaponAnimation.Bash2H; break;
                        case 3: animation = WeaponAnimation.Slash2H; break;
                        case 4: animation = WeaponAnimation.Bash2H; break;
                        case 5: animation = WeaponAnimation.Slash2H; break;
					}

                    return animation;
				}

				else
				{
					switch (Utility.RandomMinMax(1, 6))
					{
                        case 1: animation = WeaponAnimation.Bash1H; break;
                        case 2: animation = WeaponAnimation.Bash1H; break;
                        case 3: animation = WeaponAnimation.Slash1H; break;
                        case 4: animation = WeaponAnimation.Slash2H; break;
                        case 5: animation = WeaponAnimation.Pierce1H; break;
                        case 6: animation = WeaponAnimation.Bash2H; break;
					}

                    return animation;
				}
			}

			return animation;
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
		}

		public override void OnHit( Mobile attacker, Mobile defender, double damageBonus )
		{
			base.OnHit( attacker, defender, damageBonus );

            if (TrainingWeapon)
                return;
                        
            int toReduce =  Utility.Random( 3, 3 );

            if (attacker is BaseCreature && defender is PlayerMobile)
                toReduce = (int)(Math.Floor((double)toReduce / 2));

            defender.Stam = defender.Stam > toReduce ? defender.Stam - toReduce : 1;
		}

		public override double GetBaseDamage( Mobile attacker )
		{
			double damage = base.GetBaseDamage( attacker );

			return damage;
		}
	}
}