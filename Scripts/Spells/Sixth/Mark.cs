using System;
using Server.Targeting;
using Server.Network;
using Server.Regions;
using Server.Multis;
using Server.Items;
using Server.Custom;

namespace Server.Spells.Sixth
{
	public class MarkSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Mark", "Kal Por Ylem",
				218,
				9002,
				Reagent.BlackPearl,
				Reagent.Bloodmoss,
				Reagent.MandrakeRoot
			);

		public override SpellCircle Circle { get { return SpellCircle.Sixth; } }

		public MarkSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public override bool CheckCast()
		{
            return SpellHelper.CheckTravel(Caster, TravelCheckType.Mark);
		}

		public void Target( RecallRune rune )
		{
            // IPY : Protection for DungeonMiningSystem.
            if (Caster.Map == Map.Ilshenar)
            {
                Caster.SendLocalizedMessage(501802); // Thy spell doth not appear to work...
                return;
            }

            WarpBlockerTotem recallBlocker = WarpBlockerTotem.RecallBlockerTriggered(Caster, WarpBlockerTotem.MovementMode.Mark, Caster.Location, Caster.Map);

            if (recallBlocker != null)
            {
                if (recallBlocker.PreventMarkResponse != "")
                    Caster.SendMessage(recallBlocker.PreventMarkResponse);
                else
                    Caster.SendMessage(WarpBlockerTotem.DefaultMarkResponse);   
            }

			else if ( !Caster.CanSee( rune ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}

            else if (!SpellHelper.CheckTravel(Caster, TravelCheckType.Mark))
            {
                Caster.SendLocalizedMessage(501802); // Thy spell doth not appear to work...
            }

            else if (Caster.Map != Map.Felucca)
            {
                Caster.SendLocalizedMessage(501802); // Thy spell doth not appear to work...
            }

            else if (SpellHelper.CheckMulti(Caster.Location, Caster.Map))
            {
                Caster.SendLocalizedMessage(501942); // That location is blocked.
            }

            else if (SpellHelper.IsSolenHiveLoc(Caster.Location))
            {
                Caster.SendLocalizedMessage(501802); // Thy spell doth not appear to work...
            }

            else if (SpellHelper.IsStarRoom(Caster.Location))
            {
                Caster.SendLocalizedMessage(501802); // Thy spell doth not appear to work...
            }

            else if (SpellHelper.IsWindLoc(Caster.Location))
            {
                Caster.SendLocalizedMessage(501802); // Thy spell doth not appear to work...
            }

            else if (BaseBoat.FindBoatAt(Caster.Location, Caster.Map) != null)
            {
                Caster.SendMessage("You cannot mark a location at sea.");
            }

            else if (CheckSequence())
            {
                rune.Mark(Caster);

                int spellHue = 0; // PlayerEnhancementPersistance.GetSpellHueFor(Caster, HueableSpell.Mark);

                Caster.PlaySound(0x1FA);
                Effects.SendLocationEffect(Caster, Caster.Map, 14201, 16, spellHue, 0);
            }

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private MarkSpell m_Owner;

			public InternalTarget( MarkSpell owner ) : base( 12, false, TargetFlags.None )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				RecallRune rune = o as RecallRune;

				if ( rune != null && !rune.IsLockedDown )
				{
					m_Owner.Target( rune );
				}

				else
				{
					from.Send( new MessageLocalized( from.Serial, from.Body, MessageType.Regular, 0x3B2, 3, 501797, from.Name, "" ) ); // I cannot mark that object.
				}
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}