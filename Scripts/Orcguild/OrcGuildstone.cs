using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Guilds;
using Server.Gumps;
using Server.Network;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Custom.Orcguild
{
	class OrcGuildstone : Item
	{
		// The "human" guild this stone is tied to.
		[CommandProperty(AccessLevel.GameMaster)]		
		public string AttachedToGuild { get; set; }

		// body mods
		[CommandProperty(AccessLevel.GameMaster)]
		public int Level1Body { get; set; }
		[CommandProperty(AccessLevel.GameMaster)]
		public int Level2Body { get; set; }
		[CommandProperty(AccessLevel.GameMaster)]
		public Body Level3Body { get; set; }

		[Constructable]
        public OrcGuildstone()
            : base(0x1172)
        {
			Movable = false;
			// Default bodies

			Level1Body = 17;	// ORC
			Level2Body = 7;		// ORC CAPTAIN
			Level3Body = 0x190;	// HUMAN
        }
		public OrcGuildstone(Serial serial)
            : base(serial)
        {
			Movable = false;
					Level1Body = 17;	// ORC
			Level2Body = 7;		// ORC CAPTAIN
			Level3Body = 0x190;	// HUMAN
		}

        public override string DefaultName
        {
            get { return "A dirty slab of stone"; }
        }

		public override void OnDoubleClick(Mobile from)
		{
			if (from.Guild == null || from.Guild.Name != AttachedToGuild)
			{
				from.SendMessage("You see glyphs scribbled in a foreign language");
			}
			else if (from.Guild.Name == AttachedToGuild)
			{
				if (from.Guild is Guild)
				{
					Guild g = (from.Guild as Guild);
					from.SendGump(new OrcGuildstoneGump(from, this, g, g.Leader == from));
				}
			}
			base.OnDoubleClick(from);
		}


		public override void Serialize(GenericWriter writer)
		{
			writer.Write(AttachedToGuild);
			base.Serialize(writer);
		}

		public override void Deserialize(GenericReader reader)
		{
			AttachedToGuild = reader.ReadString();
			base.Deserialize(reader);
		}

		private class OrcGuildstoneGump : Gump
		{
			private bool m_IsLeader;
			private OrcGuildstone m_stone;
			private Guild m_realguild;

			public OrcGuildstoneGump(Mobile from, OrcGuildstone stone, Guild realguild, bool leader_clicked)
				: base(0, 0)
			{
				m_IsLeader = leader_clicked;
				m_stone = stone;
				m_realguild = realguild;

				this.Closable=true;
				this.Disposable=true;
				this.Dragable=true;
				this.Resizable=false;
				this.AddPage(0);
				this.AddBackground(191, 81, 134, 207, 83);
				this.AddButton(245, 111, 4005, 4007, (int)Buttons.BtnOrc, GumpButtonType.Reply, 0);
				this.AddButton(245, 146, 4005, 4007, (int)Buttons.BtnOrcChief, GumpButtonType.Reply, 0);
				this.AddButton(245, 180, 4005, 4007, (int)Buttons.BtnHuman, GumpButtonType.Reply, 0);
				this.AddItem(235, 249, 3556, 0);
				this.AddItem(249, 136, 7947, 0);
				this.AddItem(247, 222, 8416, 0);
				this.AddItem(264, 136, 7947, 0);
				this.AddItem(250, 100, 7947, 0);
				this.AddItem(257, 172, 7401, 0);
			}
			public enum Buttons
			{
				BtnOrc = 1,
				BtnOrcChief,
				BtnHuman,
			}

			public override void OnResponse(NetState sender, RelayInfo info)
			{
				if (info.ButtonID == (int)Buttons.BtnOrc)
					sender.Mobile.Target = new OrcBlessingTarget(m_realguild, new Body(m_stone.Level1Body));
				else if (m_IsLeader && info.ButtonID == (int)Buttons.BtnOrcChief) // only leader of guild can promote to chief!
					sender.Mobile.Target = new OrcBlessingTarget(m_realguild, new Body(m_stone.Level2Body));
				else if (info.ButtonID == (int)Buttons.BtnHuman)
					sender.Mobile.Target = new OrcBlessingTarget(m_realguild, new Body(m_stone.Level3Body));
			}
		}


		public class OrcBlessingTarget : Target
		{
			private Guild m_realguild;
			private Body m_body;

			public OrcBlessingTarget(Guild guild, Body body)
				: base(-1, false, TargetFlags.None)
			{
				m_realguild = guild;
				m_body = body;
			}
			protected override void OnTarget(Mobile from, object targeted)
			{
				if (targeted is PlayerMobile)
				{
					PlayerMobile pm = targeted as PlayerMobile;
					if (pm.Guild == m_realguild)
					{
						pm.Body = m_body;
					}
				}
			}
		}
	}
}
