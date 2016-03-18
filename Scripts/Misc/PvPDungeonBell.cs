using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Server;
using Server.Items;
using Server.Spells;
using Server.Network;
using Server.Custom.Townsystem;
using Server.Mobiles;

/*
 *	Default state: Brazier is doused
 *	Clicked by OCB member: Ignition spell going for 10 seconds, interruptible.
 *	On ignited: Send a message to all factions "O,C,B has claimed ownership of XXX"
 *	When ignited: Chance for rare drops or more gold from monsters or w/e.
 *	Global cooldown for all bells (30 minutes)
 *	Ignition state is not serialized
 * 
 */

namespace Server.Misc
{
    public class GenericAnnouncementBell : Item
    {
        public static TimeSpan GlobalBellCooldown = new TimeSpan(0, 5, 0);
        public static DateTime NextAllowedRinging = DateTime.UtcNow;

        private string m_LocationName = "Unknown Location";
        [CommandProperty(AccessLevel.GameMaster)]
        public string LocationName
        {
            get { return m_LocationName; }
            set { m_LocationName = value; }
        }

        [Constructable]
        public GenericAnnouncementBell(): base(0x1C12)
        {
            Name = "a small glowing bell";
            Movable = false;
        }

        public GenericAnnouncementBell(Serial serial): base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(this, 3))
            {
                SendLocalizedMessageTo(from, 500295); //You are too far away to do that.
                return;
            }

            if (DateTime.UtcNow > NextAllowedRinging || from.AccessLevel > AccessLevel.Player)
                Ring(from);

            else
                from.SendMessage(String.Format("This bell was recently rung and can not be used for another {0} minutes.", (int)((NextAllowedRinging - DateTime.UtcNow).TotalMinutes)));
        }

        public void Ring(Mobile from)
        {
            var pmFrom = from as PlayerMobile;

            string guildabb = from.Guild != null ? "[" + from.Guild.Abbreviation + "] " : "";
            string s = String.Format("{0} {1}has rung the \"{2}\" " + m_LocationName + " bell.", from.Name, guildabb, m_LocationName);

            World.Broadcast(0x23, false, s);

            if (from.AccessLevel == AccessLevel.Player)
                NextAllowedRinging = DateTime.UtcNow + GlobalBellCooldown;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version

            writer.Write(m_LocationName);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version >= 0)
            {
                m_LocationName = reader.ReadString();
            }
        }
    }

    public class DuelPvPBell : Item
    {
        public static TimeSpan GlobalBellCooldown = new TimeSpan(0, 5, 0);
        public static DateTime NextAllowedRinging = new DateTime();
        private string m_LocationName;

        [Constructable]
        public DuelPvPBell() : base(0x1C12) 
        {
            Name = "a small glowing war bell";
            Movable = false;
            m_LocationName = "Arenas";
        }

        public DuelPvPBell(Serial serial) : base(serial) { }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(this, 3))
            {
                SendLocalizedMessageTo(from, 500295); //You are too far away to do that.
                return;
            }

            if (DateTime.UtcNow > NextAllowedRinging)
            {
                Ring(from);
            }
            else
            {
                from.SendMessage(String.Format("This war bell was recently rung and can not be used for another {0} minutes.", (int)((NextAllowedRinging - DateTime.UtcNow).TotalMinutes)));
            }
        }

        public void Ring(Mobile from)
        {
            var pmFrom = from as PlayerMobile;

            string guildabb = from.Guild != null ? "[" + from.Guild.Abbreviation + "] " : "";
            string s = String.Format("{0} {1}has declared war by ringing the \"{2}\" duel bell.", from.Name, guildabb, m_LocationName);

            World.Broadcast(0x23, false, s);

            NextAllowedRinging = DateTime.UtcNow + GlobalBellCooldown;
        }


        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)2); //version

            writer.Write(m_LocationName);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            switch (version)
            {
                case 2:
                    {
                        m_LocationName = reader.ReadString();
                        goto case 0;
                        break;
                    }
                case 0:
                default:
                    break;
            }
            NextAllowedRinging = DateTime.UtcNow;
            Hue = 0;
        }


    }
	public class PvPDungeonBell : Item
	{
		public static DateTime NextAllowedRinging = new DateTime();
		public static TimeSpan GlobalBellCooldown = new TimeSpan(0, 15, 0);
		public static string LastPvPBellName = "";

		
		[Constructable]
		public PvPDungeonBell()
			: base(0x1C12)
		{
			Name = "a small glowing war bell";
			Movable = false;
			m_LocationName = "(default)";
		}

		public PvPDungeonBell(Serial serial)
			: base(serial)
		{
		}

		private Town m_CapTown;
		private string m_LocationName;

		[CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
		public string LocationName
		{
			get { return m_LocationName; }
			set { m_LocationName = value; }
		}

		[CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
		public DateTime NextRinging
		{
			get { return NextAllowedRinging; }
			set { NextAllowedRinging = value; }
		}

		[CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
		public Town CapTown
		{
			get { return m_CapTown; }
			set { m_CapTown = value; }
		}

		public override void OnSingleClick(Mobile from)
		{
			if (m_CapTown != null)
			    LabelTo(from, "[" + m_CapTown.HomeFaction.Definition.FriendlyName + "]");

			base.OnSingleClick(from);
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (!from.InRange(this, 3))
			{
				SendLocalizedMessageTo(from, 500295); //You are too far away to do that.
				return;
			}

			var pmFrom = from as PlayerMobile;
			if (!pmFrom.IsInMilitia)
				return;

			if (DateTime.UtcNow > NextAllowedRinging)
			{
				new WarBellRingSpell(this, from).Cast();
			}
			else
			{
				from.SendMessage(String.Format("This war bell was recently rung and can not be used for another {0} minutes.", (int)((NextAllowedRinging - DateTime.UtcNow).TotalMinutes)));
			}
		}

		public void Ring(Mobile from)
		{
            var pmFrom = from as PlayerMobile;

			if( !pmFrom.IsInMilitia )
				return;

			string guildabb = from.Guild != null ? "["+from.Guild.Abbreviation+"] " : "";
			string town = pmFrom.Citizenship.HomeFaction.Definition.FriendlyName;
			string s = String.Format("{0} {1}of the {2} militia declared war by ringing the \"{3}\" war bell.", from.Name, guildabb, town, m_LocationName);
			World.Broadcast(0x23, false, s);
			//Faction.BroadcastAll(s);

			NextAllowedRinging = DateTime.UtcNow + GlobalBellCooldown;
			this.Hue = pmFrom.Citizenship.HomeFaction.Definition.HuePrimary;

			LastPvPBellName = m_LocationName;
			m_CapTown = pmFrom.Citizenship;
		}


		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)2); //version

			writer.Write(m_LocationName);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
			switch (version)
			{
				case 2:
					{
						m_LocationName = reader.ReadString();
						goto case 0;
						break;
					}
				case 0:
				default:
					break;
			}
			NextAllowedRinging = DateTime.UtcNow;
			Hue = 0;
		}
	}

    public class WarBellRingSpell : Spell
    {
        private static SpellInfo m_Info = new SpellInfo("Warbell ring", "");

        private PvPDungeonBell m_Bell;
        private Mobile m_Caster;

        public WarBellRingSpell(PvPDungeonBell bell, Mobile caster)
            : base(caster, null, m_Info)
        {
            m_Caster = caster;
            m_Bell = bell;
            caster.SendMessage("You begin ringing the bell...");
        }

        public override bool ClearHandsOnCast { get { return true; } }
        public override bool RevealOnCast { get { return false; } }

        public override TimeSpan GetCastRecovery()
        {
            return TimeSpan.FromSeconds(5);
        }

        public override double CastDelayFastScalar { get { return 0; } }

        public override TimeSpan CastDelayBase
        {
            get
            {
                return m_Caster.Hidden ? TimeSpan.FromSeconds(30.0) : TimeSpan.FromSeconds(10);
            }
        }

        public override int GetMana() { return 0; }

        public override bool ConsumeReagents() { return false; }

        public override bool CheckFizzle() { return true; }

        private bool m_Stop;

        public void Stop()
        {
            m_Stop = true;
            Disturb(DisturbType.Hurt, false, false);
        }

        public override bool CheckDisturb(DisturbType type, bool checkFirst, bool resistable)
        {
            if (type == DisturbType.EquipRequest || type == DisturbType.UseRequest/* || type == DisturbType.Hurt*/ )
                return false;

            return true;
        }

        public override void DoHurtFizzle()
        {
            if (!m_Stop)
                base.DoHurtFizzle();
        }

        public override void DoFizzle()
        {
            if (!m_Stop)
                base.DoFizzle();
        }

        public override void OnDisturb(DisturbType type, bool message)
        {
            if (message && !m_Stop)
                Caster.SendMessage("Your ringing was interrupted!"); // You have been disrupted while attempting to summon your pet!
        }

        public override void OnCast()
        {
            FinishSequence();
            Caster.SendMessage("The sound of the bell echoes clear and loud throughout Britannia.");
            m_Bell.Ring(m_Caster);
        }
    }
}
