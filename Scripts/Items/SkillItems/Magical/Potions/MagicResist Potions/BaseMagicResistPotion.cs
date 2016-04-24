using System;
using Server;

namespace Server.Items
{
	public abstract class BaseMagicResistPotion : BasePotion
	{
		public abstract double MagicResist { get; }
		public abstract TimeSpan Duration { get; }

        public BaseMagicResistPotion(PotionEffect effect): base(0x0F06 , effect)
		{
		}

		public BaseMagicResistPotion( Serial serial ) : base( serial )
		{
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

		public bool DoMagicResist( Mobile from )
		{
            double currentMagicResist = from.GetSpecialAbilityEntryValue(SpecialAbilityEffect.MagicResist);

            if (currentMagicResist < MagicResist)
            {
                from.FixedParticles(0x375A, 9, 40, 5027, 2500, 0, EffectLayer.Waist);
                from.PlaySound(0x20F);  //0x3BD              

                from.AddSpecialAbilityEffectEntry(new SpecialAbilityEffectEntry(SpecialAbilityEffect.MagicResist, from, MagicResist, DateTime.UtcNow + Duration));
                from.SendMessage("Your magic resist against creatures has increased by " + MagicResist.ToString() + ".");

				return true;
			}

			from.SendLocalizedMessage( 502173 ); // You are already under a similar effect.
			return false;
		}

		public override void Drink( Mobile from )
		{
            if (DoMagicResist(from))
			{
				BasePotion.PlayDrinkEffect( from );

				if ( !Engines.ConPVP.DuelContext.IsFreeConsume( from ) )
				    Consume();
			}
		}
	}
}