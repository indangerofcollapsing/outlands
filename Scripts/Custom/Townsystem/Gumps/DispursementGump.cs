
using System;
using System.Collections.Generic;
using System.Text;
using Server.Custom.Townsystem;

namespace Server.Gumps
{
    public class DispursementNoteGump : Gump
    {
        public DispursementNoteGump(Mobile from, Town town)
            : base(0, 0)
            {
                this.Closable=true;
                this.Disposable=true;
                this.Dragable=true;
                this.Resizable=false;
                this.AddPage(0);
                this.AddImage(1, 3, 3500);
                this.AddImage(260, 14, 3505);
                this.AddImage(1, 13, 3503);
                this.AddImage(260, 3, 3502);
                this.AddImage(27, 3, 3501);
                this.AddImage(1, 233, 3506);
                this.AddImage(27, 15, 3504);
                this.AddImage(27, 233, 3507);
                this.AddImage(260, 233, 3508);
                this.AddLabel(21, 20, 195, String.Format(@"Dear {0},",from.RawName));

                string kingName = "The King";
                if (town != null && town.King != null)
                    kingName = (town.King.Female ? "Queen " : "King ") + town.King.RawName;

                string text = String.Format("{0} thanks you for your loyalty. As a token of the King's gratitude, a portion of the town’s wealth is shared. {0} hopes for your support in the next election. Long live the King!", kingName);
                            
                this.AddHtml( 22, 47, 243, 189, text, (bool)true, (bool)true);
                this.AddImage(252, 229, 9004);
            }

    }
}
