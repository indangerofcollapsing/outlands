/***************************************************************************
 *                            PaladinOrderNPC.cs
 *                            ------------------
 *   begin                : July 2010
 *   author               : Sean Stavropoulos
 *   email                : sean.stavro@gmail.com
 *
 *
 ***************************************************************************/
using System;
using Server;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Targeting;
using Server.ContextMenus;
using Server.Gumps;
using Server.Misc;
using Server.Network;
using Server.Spells;
using Server.Commands;
using Server.Mobiles;
using Server.Engines.Quests;


namespace Server.Engines.Quests.Paladin
{
	public class PaladinOrderGuard : BaseQuester
	{
        public override int GetAutoTalkRange(PlayerMobile m) { return 5; }

		[Constructable]
		public PaladinOrderGuard() : base( "the Order Guard" )
		{
		}

		public override void InitOutfit()
		{
            AddItem(new Boots(1150));
            AddItem(new Cloak(1150));
            AddItem(new BodySash(1150));
            AddItem(new VikingSword());

            AddItem(new PlateArms());
            AddItem(new PlateChest());
            AddItem(new PlateGloves());
            AddItem(new PlateGorget());
            AddItem(new PlateLegs());

            Item shield = new MetalKiteShield();
            shield.Hue = 1150;
            AddItem(shield);

			int hairHue = GetHairHue();

			Utility.AssignRandomHair( this, hairHue );
			Utility.AssignRandomFacialHair( this, hairHue );
		}

		public PaladinOrderGuard( Serial serial ) : base( serial )
		{
		}

        public override bool HandlesOnSpeech(Mobile from)
        {
            if (from.InRange(this.Location, 2))
                return true;

            return base.HandlesOnSpeech(from);
        }

        public override void  OnTalk(PlayerMobile player, bool contextMenu)
        {
            Direction = GetDirectionTo(player);

            QuestSystem qs = player.Quest;

            if (qs is PaladinInitiationQuest)
            {
                if (qs.IsObjectiveInProgress(typeof(ReturnDeceitItem)))
                {
                    PaladinQuestItem item = (PaladinQuestItem)player.Backpack.FindItemByType(typeof(PaladinQuestItem));
                    if (item != null && item.ItemID == 5049)
                    {
                        item.Delete();
                        QuestObjective qo = qs.FindObjective(typeof(ReturnDeceitItem));
                        qo.Complete();
                    }
                }
                else if (qs.IsObjectiveInProgress(typeof(ReturnHythlothItem)))
                {
                    PaladinQuestItem item = (PaladinQuestItem)player.Backpack.FindItemByType(typeof(PaladinQuestItem));
                    if (item != null && item.ItemID == 7108)
                    {
                        item.Delete();
                        QuestObjective qo = qs.FindObjective(typeof(ReturnHythlothItem));
                        qo.Complete();
                    }
                }
                else if (qs.IsObjectiveInProgress(typeof(WaitingForSiege)))
                {
                    if (PaladinSiege.CanBeginSiege())
                    {
                        QuestObjective qo = qs.FindObjective(typeof(WaitingForSiege));
                        qo.Complete();
                    }
                    else
                        Say("The town is under attack! Come back later!");
                }

            }
            else
            {
                string req = PaladinSiege.CheckReqs(player);

                if (req.Length > 0)
                {
                    if (contextMenu)
                        Say(req);

                    return;
                }

                QuestSystem newQuest = new PaladinInitiationQuest(player);

                if (qs != null)
                {
                    newQuest.AddConversation(new DontOfferConversation());
                }
                else if (QuestSystem.CanOfferQuest(player, typeof(PaladinInitiationQuest)))
                {
                    PlaySound(0x20);
                    PlaySound(0x206);
                    newQuest.SendOffer();
                }
            }
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version

            bool writeQuest = !PaladinSiege.Saved;
            writer.Write(writeQuest);
            if (writeQuest)
                PaladinSiege.Serialize(writer);

		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
            switch (version)
            {
                case 1:
                    {
                        if (reader.ReadBool())
                            PaladinSiege.Deserialize(reader);
                        break;
                    }
            }
		}
	}
}