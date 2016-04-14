using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public abstract class BaseStaff : BaseMeleeWeapon
	{
		public override int BaseHitSound{ get{ return 0x233; } }
		public override int BaseMissSound{ get{ return 0x239; } }

		public override SkillName BaseSkill{ get{ return SkillName.Macing; } }
		public override WeaponType BaseType{ get{ return WeaponType.Bashing; } }
		public override WeaponAnimation BaseAnimation { get{ return WeaponAnimation.Bash2H; } }

        public override CraftResource DefaultResource { get { return CraftResource.RegularWood; } }

		public BaseStaff( int itemID ) : base( itemID )
		{
		}

		public BaseStaff( Serial serial ) : base( serial )
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
                    switch (Utility.RandomMinMax(1, 6))
                    {
                        case 1: animation = WeaponAnimation.Bash2H; break;
                        case 2: animation = WeaponAnimation.Bash2H; break;
                        case 3: animation = WeaponAnimation.Bash2H; break;
                        case 4: animation = WeaponAnimation.Slash2H; break;
                        case 5: animation = WeaponAnimation.Slash2H; break;
                        case 6: animation = WeaponAnimation.Pierce2H; break;                        
                    }

                    return animation;
                }
                
                else if (attacker.FindItemOnLayer(Layer.TwoHanded) != null)
				{
                    switch (Utility.RandomMinMax(1, 5))
					{
						case 1: animation = WeaponAnimation.Bash2H; break;
						case 2: animation = WeaponAnimation.Bash2H; break;
                        case 3: animation = WeaponAnimation.Bash2H; break;
						case 4: animation = WeaponAnimation.Slash2H; break;
						case 5: animation = WeaponAnimation.Slash2H; break;	
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

			defender.Stam -= Utility.Random( 3, 3 ); // 3-5 points of stamina loss
		}
	}
}