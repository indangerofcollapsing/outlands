using System;
using System.Collections.Generic;
using Server.Network;
using Server.Mobiles;
using Server.Targeting;
using Server.Items;

namespace Server.Spells.Necromancy
{
	public class AnimateDeadSpell : NecromancerSpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Animate Dead", "Uus Corp",
				203,
				9031,
				Reagent.GraveDust,
				Reagent.DaemonBlood
			);

		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 1.5 ); } }

		public override double RequiredSkill{ get{ return 40.0; } }
		public override int RequiredMana{ get{ return 23; } }

		public AnimateDeadSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
			Caster.SendLocalizedMessage( 1061083 ); // Animate what corpse?
		}

		private class CreatureGroup
		{
			public Type[] m_Types;
			public SummonEntry[] m_Entries;

			public CreatureGroup( Type[] types, SummonEntry[] entries )
			{
				m_Types = types;
				m_Entries = entries;
			}
		}

		private class SummonEntry
		{
			public Type[] m_ToSummon;
			public int m_Requirement;

			public SummonEntry( int requirement, params Type[] toSummon )
			{
				m_ToSummon = toSummon;
				m_Requirement = requirement;
			}
		}

		public void Target( object obj )
		{
			Corpse c = obj as Corpse;

			if( c == null )
			{
				Caster.SendLocalizedMessage( 1061084 ); // You cannot animate that.
			}
			else
			{
				Type type = null;

				if( c.Owner != null )
				{
					type = c.Owner.GetType();
				}

				if( c.ItemID != 0x2006  || type == typeof( PlayerMobile ) || type == null || ( c.Owner != null && c.Owner.Fame < 100 ) || ( ( c.Owner != null ) && ( c.Owner is BaseCreature ) && ( ( ( BaseCreature )c.Owner ).Summoned || ( ( BaseCreature )c.Owner ).IsBonded ) ) )
				{
					Caster.SendLocalizedMessage( 1061085 ); // There's not enough life force there to animate.
				}

				else
				{					
				}
			}

			FinishSequence();
		}

		private static Dictionary<Mobile, List<Mobile>> m_Table = new Dictionary<Mobile, List<Mobile>>();

		public static void Unregister( Mobile master, Mobile summoned )
		{
			if ( master == null )
				return;

			List<Mobile> list = null;
			m_Table.TryGetValue( master, out list );

			if ( list == null )
				return;

			list.Remove( summoned );

			if ( list.Count == 0 )
				m_Table.Remove( master );
		}

		public static void Register( Mobile master, Mobile summoned )
		{
			if ( master == null )
				return;

			List<Mobile> list = null;
			m_Table.TryGetValue( master, out list );

			if ( list == null )
				m_Table[master] = list = new List<Mobile>();

			for ( int i = list.Count - 1; i >= 0; --i )
			{
				if ( i >= list.Count )
					continue;

				Mobile mob = list[i];

				if ( mob.Deleted )
					list.RemoveAt( i-- );
			}

			list.Add( summoned );

			if ( list.Count > 3 )
				Timer.DelayCall( TimeSpan.Zero, new TimerCallback( list[0].Kill ) );

			Timer.DelayCall( TimeSpan.FromSeconds( 2.0 ), TimeSpan.FromSeconds( 2.0 ), new TimerStateCallback( Summoned_Damage ), summoned );
		}

		private static void Summoned_Damage( object state )
		{
			Mobile mob = (Mobile)state;

			if ( mob.Hits > 0 )
				--mob.Hits;
			else
				mob.Kill();
		}

		private static void SummonDelay_Callback( object state )
		{
			object[] states = (object[])state;

			Mobile caster = (Mobile)states[0];
			Corpse corpse = (Corpse)states[1];
			Point3D loc = (Point3D)states[2];
			Map map = (Map)states[3];
			CreatureGroup group = (CreatureGroup)states[4];

			//if ( corpse.Animated )
			//    return;

			Mobile owner = corpse.Owner;

			if ( owner == null )
				return;

			double necromancy = caster.Skills[SkillName.Necromancy].Value;
			double spiritSpeak = caster.Skills[SkillName.SpiritSpeak].Value;

			int casterAbility = 0;

			casterAbility += (int)(necromancy * 30);
			casterAbility += (int)(spiritSpeak * 70);
			casterAbility /= 10;
			casterAbility *= 18;

			if ( casterAbility > owner.Fame )
				casterAbility = owner.Fame;

			if ( casterAbility < 0 )
				casterAbility = 0;

			Type toSummon = null;
			SummonEntry[] entries = group.m_Entries;

			for ( int i = 0; toSummon == null && i < entries.Length; ++i )
			{
				SummonEntry entry = entries[i];

				if ( casterAbility < entry.m_Requirement )
					continue;

				Type[] animates = entry.m_ToSummon;

				if ( animates.Length >= 0 )
					toSummon = animates[Utility.Random( animates.Length )];
			}

			if ( toSummon == null )
				return;

			Mobile summoned = null;

			try{ summoned = Activator.CreateInstance( toSummon ) as Mobile; }
			catch{}

			if ( summoned == null )
				return;

			if ( summoned is BaseCreature )
			{
				BaseCreature bc = (BaseCreature)summoned;

				// to be sure
				bc.Tameable = false;

				if ( bc is BaseMount )
					bc.ControlSlots = 1;
				else
					bc.ControlSlots = 0;

				Effects.PlaySound( loc, map, bc.GetAngerSound() );

				BaseCreature.Summon( (BaseCreature)summoned, false, caster, loc, 0x28, TimeSpan.FromDays( 1.0 ) );
			}

			if ( summoned is SkeletalDragon )
				Scale( (SkeletalDragon)summoned, 50 ); // lose 50% hp and strength

			summoned.Fame = 0;
			summoned.Karma = -1500;

			summoned.MoveToWorld( loc, map );

			corpse.Hue = 1109;
			//corpse.Animated = true;

			Register( caster, summoned );
		}

		public static void Scale( BaseCreature bc, int scalar )
		{
			int toScale;

			toScale = bc.RawStr;
			bc.RawStr = AOS.Scale( toScale, scalar );

			toScale = bc.HitsMaxSeed;

			if ( toScale > 0 )
				bc.HitsMaxSeed = AOS.Scale( toScale, scalar );

			bc.Hits = bc.Hits; // refresh hits
		}

		private class InternalTarget : Target
		{
			private AnimateDeadSpell m_Owner;

			public InternalTarget( AnimateDeadSpell owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.None )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				m_Owner.Target( o );
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}