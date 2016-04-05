using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Server.Gumps;
using Server.Items;
using Server.Commands;
using Server.Mobiles;


namespace Server.Custom.AntiRailing
{
	public class HarvestGump : Gump
	{
		int m_CorrectButton;
		private static bool m_ShowHelpGrid = false;
		private static int m_PostSwingCountMining	= 4;
		private static int m_PostSwingCountLJ		= 2;

		public static int[] m_Items = new int[] { 0x1BDC, 0x1BDD, 0x1BDE, 0x1BDF, 0x1BF6, 0x1BF7, 0x1F2E, /* extras */	0x14EF, 0x13E2, 0x1415, 0x1440, 0x153C, 0x160B, 0x1BD6, 0x1AD6, };
		public static int[] m_Backgrounds = new int[] { 9200 };
		public static int[] m_XYVariations;

		public static void Initialize()
		{
			m_XYVariations = new int[512];
			for(int i = 0; i < 512; ++i)
				m_XYVariations[i] = Utility.Random(-8, 16);

			CommandSystem.Register("HarvestTest", AccessLevel.GameMaster, new CommandEventHandler(HarvestTest_OnCommand));	
		}


		public HarvestGump(Mobile m)
			: base(100,100)
		{
			int gumpx = 100;
			int gumpy = 100;

			m.CloseGump(typeof(HarvestGump));

			this.Closable = true;
			this.Disposable = true;
			this.Dragable = true;
			this.Resizable = false;
			this.AddPage(0);


			// BACKGROUNDS
			this.AddBackground(gumpx, gumpy, 381, 250, Utility.RandomList(m_Backgrounds));
			if(Utility.Random(5) == 0)
				this.AddAlphaRegion(10+gumpx, 25+gumpy, 362, 215);
			this.AddLabel(gumpx + 100, gumpy + 5, 53, "Click the item that stands out");


			// ITEMS
			int[] xpos = new int[] { 33 + gumpx, 150 + gumpx, 268 + gumpx };
			int[] ypos = new int[] { 33 + gumpy, 103 + gumpy, 173 + gumpy };

			int dummy_dx_idx = Utility.Random(500);
			int dummy_dy_idx = Utility.Random(500);

			int winner_idx = Utility.Random(m_Items.Length);
			int loser_idx = winner_idx;
			while (loser_idx == winner_idx)
				loser_idx = Utility.Random(m_Items.Length);

			int loser_item = m_Items[loser_idx];
			int winner_item = m_Items[winner_idx];

			int m_WinnerX = Server.Utility.Random(3);
			int m_WinnerY = Server.Utility.Random(3);
			m_CorrectButton = m_WinnerX + (m_WinnerY * 3) + (int)Buttons.btnIdx0;

			this.AddItem(xpos[0] + m_XYVariations[dummy_dx_idx++], ypos[0] + m_XYVariations[dummy_dy_idx++], AddWinnerOrLoser(1, loser_item, winner_item), Utility.Random(1801, 100));
            this.AddItem(xpos[1] + m_XYVariations[dummy_dx_idx++], ypos[0] + m_XYVariations[dummy_dy_idx++], AddWinnerOrLoser(2, loser_item, winner_item), Utility.Random(1801, 100));
            this.AddItem(xpos[2] + m_XYVariations[dummy_dx_idx++], ypos[0] + m_XYVariations[dummy_dy_idx++], AddWinnerOrLoser(3, loser_item, winner_item), Utility.Random(1801, 100));

            this.AddItem(xpos[0] + m_XYVariations[dummy_dx_idx++], ypos[1] + m_XYVariations[dummy_dy_idx++], AddWinnerOrLoser(4, loser_item, winner_item), Utility.Random(1801, 100));
            this.AddItem(xpos[1] + m_XYVariations[dummy_dx_idx++], ypos[1] + m_XYVariations[dummy_dy_idx++], AddWinnerOrLoser(5, loser_item, winner_item), Utility.Random(1801, 100));
            this.AddItem(xpos[2] + m_XYVariations[dummy_dx_idx++], ypos[1] + m_XYVariations[dummy_dy_idx++], AddWinnerOrLoser(6, loser_item, winner_item), Utility.Random(1801, 100));

            this.AddItem(xpos[0] + m_XYVariations[dummy_dx_idx++], ypos[2] + m_XYVariations[dummy_dy_idx++], AddWinnerOrLoser(7, loser_item, winner_item), Utility.Random(1801, 100));
            this.AddItem(xpos[1] + m_XYVariations[dummy_dx_idx++], ypos[2] + m_XYVariations[dummy_dy_idx++], AddWinnerOrLoser(8, loser_item, winner_item), Utility.Random(1801, 100));
            this.AddItem(xpos[2] + m_XYVariations[dummy_dx_idx++], ypos[2] + m_XYVariations[dummy_dy_idx++], AddWinnerOrLoser(9, loser_item, winner_item), Utility.Random(1801, 100));

			// BUTTONS (Invisible)
			// row 1
			this.AddButton(xpos[0], ypos[0], 82, 82, (int)Buttons.btnIdx0, GumpButtonType.Reply, 0);
			this.AddButton(xpos[1], ypos[0], 82, 82, (int)Buttons.btnIdx1, GumpButtonType.Reply, 0);
			this.AddButton(xpos[2], ypos[0], 82, 82, (int)Buttons.btnIdx2, GumpButtonType.Reply, 0);
			
			// row 2
			this.AddButton(xpos[0], ypos[1], 82, 82, (int)Buttons.btnIdx3, GumpButtonType.Reply, 0);
			this.AddButton(xpos[1], ypos[1], 82, 82, (int)Buttons.btnIdx4, GumpButtonType.Reply, 0);
			this.AddButton(xpos[2], ypos[1], 82, 82, (int)Buttons.btnIdx5, GumpButtonType.Reply, 0);

			// row3
			this.AddButton(xpos[0], ypos[2], 82, 82, (int)Buttons.btnIdx6, GumpButtonType.Reply, 0);
			this.AddButton(xpos[1], ypos[2], 82, 82, (int)Buttons.btnIdx7, GumpButtonType.Reply, 0);
			this.AddButton(xpos[2], ypos[2], 82, 82, (int)Buttons.btnIdx8, GumpButtonType.Reply, 0);
		}

        private int AddWinnerOrLoser(int buttonNumber, int loser_item, int winner_item)
        {
            if (buttonNumber == m_CorrectButton)
                return winner_item;
            else
                return loser_item;
        }

		public enum Buttons
		{
			btnIdx0 = 1,
			btnIdx1,
			btnIdx2,
			btnIdx3,
			btnIdx4,
			btnIdx5,
			btnIdx6,
			btnIdx7,
			btnIdx8,
		}

		public override void OnResponse(Network.NetState sender, RelayInfo info)
		{
			if (info.ButtonID == 0)
			{
				sender.Mobile.SendMessage("You choose to discard the harvest");
			}
			else if (info.ButtonID == m_CorrectButton)
			{
				OnHarvestSuccess(sender.Mobile);
			}
			else
			{
				OnHarvestFail(sender.Mobile);
			}
			base.OnResponse(sender, info);
		}


		private void OnHarvestSuccess(Mobile m)
		{
			PlayerMobile pm = m as PlayerMobile;

			if (pm != null && pm.TempStashedHarvest != null)
			{
								if (pm.m_HarvestTimer != null && pm.m_HarvestTimer.Running)
                        pm.m_HarvestTimer.Stop();

                if (pm.TempStashedHarvestDef == null) //lockpicking / BODs
                {
                    new PostSuccessTimer(pm, 0, TimeSpan.FromTicks(1)).Start();
                }
                else
                {
                    // post-swinging and eventually resource acquisition
                    bool is_lumberjacking = pm.TempStashedHarvest is Server.Items.BaseLog;
                    new PostSuccessTimer(pm, is_lumberjacking ? m_PostSwingCountLJ : m_PostSwingCountMining, pm.TempStashedHarvestDef.EffectDelay).Start();
                }
			}
		}

		private void OnHarvestFail(Mobile m)
		{
			m.PublicOverheadMessage(Server.Network.MessageType.Emote, m.EmoteHue, true, "I should probably aim a little better");
			PlayerMobile pm = m as PlayerMobile;
			if (pm != null)
			{
				pm.FailedHarvestAttempts.Add(DateTime.UtcNow);
				if (pm.HarvestLockedout)
                {
                    if (pm.TempStashedHarvest != null)
                    {
                        if (pm.TempStashedHarvest is BaseTreasureChest)
                        {
                            ((BaseTreasureChest)pm.TempStashedHarvest).Items.ForEach(i => i.Delete());
                        } 
                        else
                            pm.TempStashedHarvest.Delete();
                    }
                }
				else
                {
                    m.SendMessage(0x20, "(Click the item that stands out)");
                    m.SendGump(new HarvestGump(m));
                }
			}
		}

		[Usage("HarvestTest")]
		[Description("Manually trigger the harvest gump")]
		public static void HarvestTest_OnCommand(CommandEventArgs e)
		{
			Mobile from = e.Mobile;
			PlayerMobile pm = from as PlayerMobile;
			if (pm != null)
			{
				Item i = new IronOre(24);
				pm.TempStashedHarvest = i;
			}
		}
	}

	class PostSuccessTimer : Timer
	{
		private PlayerMobile m_Harvester;
		public PostSuccessTimer(PlayerMobile pm ,int anim_count, TimeSpan tick_time): base(tick_time, tick_time, anim_count - 1)
		{
			m_Harvester = pm;

			if (pm.TempStashedHarvest != null && pm.TempStashedHarvestDef != null)
			{
				pm.Animate(Utility.RandomList(pm.TempStashedHarvestDef.EffectActions), 5, anim_count, true, false, 0);
				Effects.PlaySound(pm.Location, pm.Map, Utility.RandomList(pm.TempStashedHarvestDef.EffectSounds));
			}
		}

		protected override void OnTick()
		{
			if (m_Harvester.TempStashedHarvest == null)
			{
				m_Harvester.SendLocalizedMessage(500446); // That is too far away
				Stop();
			}

			else if (Running)
			{
                if (m_Harvester.TempStashedHarvestDef != null)
                {
                    Effects.PlaySound(m_Harvester.Location, m_Harvester.Map, Utility.RandomList(m_Harvester.TempStashedHarvestDef.EffectSounds));
                    m_Harvester.CheckSkill(m_Harvester.TempStashedHarvestDef.Skill, 0.0, 100.0, 1.0); // TODO : This should be resource.MinSkill / MaxSkill
                    // Check skill each tick
                }
			}

			else
			{
				Item i = m_Harvester.TempStashedHarvest;
				m_Harvester.TempStashedHarvest = null;

				Server.Engines.Harvest.HarvestSystem.GiveTo(m_Harvester, i, true);
				m_Harvester.SendMessage("You carefully gather a good amount of resources.");

                if (m_Harvester.TempStashedHarvestDef != null)
				    Effects.PlaySound(m_Harvester.Location, m_Harvester.Map, Utility.RandomList(m_Harvester.TempStashedHarvestDef.EffectSounds));
            }

            base.OnTick();
		}
	}
}
