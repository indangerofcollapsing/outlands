/***************************************************************************
 *                            TurnableCannonGuard.cs
 *                            ------------------
 *   begin                : August 2010
 *   author               : Sean Stavropoulos
 *   email                : sean.stavro@gmail.com
 *
 *
 ***************************************************************************/

using System;
using Server.Items;
using Server.Custom.Pirates;

namespace Server.Mobiles
{
	public class TurnableCannonGuard : BaseCannonGuard
	{
        public Towns m_town;
         
        [Constructable]
		public TurnableCannonGuard(Towns town) : base()
		{
			SpeechHue = Utility.RandomDyedHue();
			Title = "the cannoneer";
			Hue = Utility.RandomSkinHue();
			this.CantWalk = true;
			
			if ( this.Female = Utility.RandomBool() )
			{
				Body = 0x191;
				Name = NameList.RandomName( "female" );
				AddItem( new Skirt( Utility.RandomNeutralHue() ) );
			}
			else
			{
				Body = 0x190;
				Name = NameList.RandomName( "male" );
				AddItem( new ShortPants( Utility.RandomNeutralHue() ) );
			}
			
			SetStr( 86, 100 );
			SetDex( 81, 95 );
			SetInt( 61, 75 );
			
			SetDamage( 10, 23 );
			
			SetSkill( SkillName.MagicResist, 25.0, 60.5 );
			SetSkill( SkillName.Tactics, 65.0, 87.5 );
			SetSkill( SkillName.Wrestling, 65.0, 90.5 );
			
			Fame = 1000;
			Karma = 1000;
			
			AddItem( new Boots( Utility.RandomNeutralHue() ) );
			AddItem( new FancyShirt());
			AddItem( new Bandana());

            m_town = town;
		}
		
		protected override void OnLocationChange(Point3D oldLoc)
		{
			if( Cannon == null )
			{
				Cannon = new CannonSouth(this);
				Direction = Direction.South;
				Cannon.MoveToWorld( new Point3D(X,Y + 1,Z), Map);
			}
			else
			{
				Direction = Direction.North;
				Cannon.MoveToWorld( new Point3D(X,Y - 3,Z), Map);
			}
			base.OnLocationChange(oldLoc);
		}

        public Direction GetCannonDirection(Mobile from)
        {
            Point3D p = from.Location;
            Point3D loc = this.Location;
            int x = p.X - loc.X;
            int y = p.Y - loc.Y;

            if (x == 0) x = 1;
            if (y == 0) y = 1;

            if (y < 0 && Math.Abs(Math.Atan(y / x)) > 0.52)
            {
                return Direction.North;
            }
            else if (x > 0 && Math.Abs(Math.Atan(x / y)) > 0.52)
            {
                return Direction.East;
            }
            else if (y > 0 && Math.Abs(Math.Atan(y / x)) > 0.52)
            {
                return Direction.South;
            }
            else if (x < 0 && Math.Abs(Math.Atan(x / y)) > 0.52)
            {
                return Direction.West;
            }

            return Direction.South;

        }

		public override void OnThink()
		{
			if (NextThink > DateTime.UtcNow)
				return;
			if( Combatant == null )
				return;
			Direction d = this.GetCannonDirection(Combatant);
			if( Direction != d )
				Direction = d;
			if( Cannon.Direction != Direction )
			{
				switch (Direction)
				{
					case Direction.Up:
					case Direction.North:
						{
							Cannon.Delete();
							Cannon = new CannonNorth(this);
							Cannon.MoveToWorld( new Point3D(X,Y - 3,Z), Map);
							Cannon.Direction = Direction;
                            Cannon.Owner = this;
							break;
						}
					case Direction.Down:
					case Direction.South:
						{
							Cannon.Delete();
							Cannon = new CannonSouth(this);
							Cannon.MoveToWorld( new Point3D(X,Y + 1,Z), Map);
							Cannon.Direction = Direction;
                            Cannon.Owner = this;
							break;
						}
					case Direction.Right:
					case Direction.East:
						{
							Cannon.Delete();
							Cannon = new CannonEast(this);
							Cannon.MoveToWorld( new Point3D(X + 1,Y,Z), Map);
							Cannon.Direction = Direction;
                            Cannon.Owner = this;
							break;
						}
					case Direction.Left:
					case Direction.West:
						{
							Cannon.Delete();
							Cannon = new CannonWest(this);
							Cannon.MoveToWorld( new Point3D(X - 3,Y,Z), Map);
							Cannon.Direction = Direction;
                            Cannon.Owner = this;
							break;
						}
				}
			}
			base.OnThink();
			NextThink = DateTime.UtcNow + TimeSpan.FromSeconds(4);
		}
		
		public TurnableCannonGuard( Serial serial ) : base( serial )
		{
		}

        public override void OnHarmfulAction(Mobile target, bool isCriminal)
        {
            if (target.Player)
                TownDockControl.AddCombatant(m_town, target);   

            base.OnHarmfulAction(target, isCriminal);
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            //base.OnDamage(amount, from, willKill);
            from.Criminal = true;
            OnHarmfulAction(from, true);
        }

        public override void OnHitsChange(int oldValue)
        {
            //base.OnHitsChange(oldValue);
            Hits = HitsMax;
        }

        public override void OnAfterDelete()
        {
            TownDockControl.CannonDestroyed(m_town, this);
            base.OnAfterDelete();
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			
			writer.Write( (int) 0 ); // version

            writer.Write((int)m_town);
		}
		
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			
			int version = reader.ReadInt();

            m_town = ((Towns)reader.ReadInt());

            TownDockControl.RegisterTownCannon(m_town, this.Location, this);
		}
	}
}
