using System;
using System.Collections.Generic;
using Server;
using Server.Multis;
using Server.Targeting;
using Server.ContextMenus;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
	public class PlayerClassTitleDeed : Item
	{        
        public virtual string Title { get { return ""; } } 

		[Constructable] 
		public PlayerClassTitleDeed() : base( 0x14ED )
		{           
			Weight = 1.0;
            Name = "Title: " + Title;
          
            PlayerClass = PlayerClass.None;
            PlayerClassRestricted = true;
		}

        public PlayerClassTitleDeed(Serial serial): base(serial)
		{           
		}

        public override void OnSingleClick(Mobile from)
        {
            int titleIndex = -1;

            string[] classTitles = { };
            string className = "";

            switch (PlayerClass)
            {
                case PlayerClass.Paladin: className = "Paladin";  classTitles = PlayerClassPersistance.PaladinTitles; break;
                case PlayerClass.Murderer: className = "Murderer"; classTitles = PlayerClassPersistance.MurdererTitles; break;
                case PlayerClass.Pirate: className = "Pirate"; classTitles = PlayerClassPersistance.PirateTitles; break;
            }

            int titleRank = -1;
            int currentPlayerRank = -1;

            for (int a = 0; a < classTitles.Length; a++)
            {
                if (classTitles[a] == Title)
                {
                    titleRank = a;
                    break;
                }
            }

            Name = className + " rank " + (titleRank + 1).ToString() + " title: " + Title;

            base.OnSingleClick(from);

            PlayerClassPersistance.PlayerClassSingleClick(this, from);
        }

		public override void OnDoubleClick( Mobile from )
		{
            PlayerMobile pm_From = from as PlayerMobile;

            if (PlayerClassOwner != pm_From)
            {
                from.SendMessage("Only the owner of this item may use it");
                return;
            }

            else if (from.InRange(this.GetWorldLocation(), 1))
            {
                int titleIndex = -1;

                string[] classTitles = { };

                switch (PlayerClass)
                {
                    case PlayerClass.Paladin: classTitles = PlayerClassPersistance.PaladinTitles; break;
                    case PlayerClass.Murderer: classTitles = PlayerClassPersistance.MurdererTitles; break;
                    case PlayerClass.Pirate: classTitles = PlayerClassPersistance.PirateTitles; break;
                }

                int titleRank = -1;
                int currentPlayerRank = -1;

                for (int a = 0; a < classTitles.Length; a++)
                {
                    if (classTitles[a] == Title)
                        titleRank = a;

                    if (pm_From.TitlesPrefix.Contains(classTitles[a]))
                    {
                        currentPlayerRank++;
                    }
                }

                if (pm_From.TitlesPrefix.Contains(Title))
                {
                    pm_From.SendMessage("You already have that title available to you. You may drop this deed on the vendor you purchased it from for a full refund.");
                    return;
                }

                if (titleRank == (currentPlayerRank + 1) && !pm_From.TitlesPrefix.Contains(Title))
                {
                    pm_From.TitlesPrefix.Add(Title);
                    pm_From.SendMessage("The title of " + Title + " has now been added to your list of selectable titles.");

                    pm_From.FixedParticles(0x375A, 9, 40, 5027, EffectLayer.Waist);
                    pm_From.PlaySound(0x1F7);

                    Delete();
                }

                else 
                    pm_From.SendMessage("You must first acquire the previous title ranks before activating this title.");                                  
            }

            else
            {
                from.SendMessage("That is too far away to use.");
                return;
            }
		}

        public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write((int)0); //Version

            Name = "Title: " + Title;
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();            
		}
	}
}