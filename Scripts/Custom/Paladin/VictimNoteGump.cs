using Server;
using Server.Gumps;
using Server.Commands;
using Server.Mobiles;
using Server.Accounting;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Server.Custom.Paladin
{
    public class VictimNoteGump : Gump
    {
        public string m_PaladinName;
        public string m_MurdererName;
        public string m_VictimName;

        public int m_Restitution = 0;

        public VictimNoteGump(string paladinName, string murdererName, string victimName, int restitution): base(0, 0)
        {
            if (paladinName == null || murdererName == null || victimName == null)
                return;

            m_PaladinName = paladinName;
            m_MurdererName = murdererName;
            m_VictimName = victimName;
            m_Restitution = restitution;

            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            AddPage(0);            

            int textHue = 2036;

            AddImage(7, 4, 1247);

            AddItem(161, 253, 3823);
            AddItem(137, 102, 3342);
            AddItem(73, 22, 3274);
            AddItem(82, 145, 3348);
            AddItem(150, 129, 3351);
            AddItem(265, 97, 5);
            AddItem(243, 98, 4);
            AddItem(117, 134, 3808);
            AddItem(95, 119, 3816);
            AddItem(77, 95, 490);
            AddItem(75, 84, 3799);
            AddItem(118, 139, 3248);
            AddItem(8, 25, 3320);
            AddItem(241, 165, 4615);
            AddItem(220, 145, 4619);
            AddItem(236, 152, 7109);
            AddItem(227, 151, 5050);

            AddLabel(180, 30, textHue, "The murderous " + m_MurdererName);
            AddLabel(180, 50, textHue, "has been brought to justice");
            AddLabel(180, 70, textHue, "by the paladin " + m_PaladinName);

            AddLabel(61, 210, textHue, @"As a past victim of this criminal, the Order");
            AddLabel(61, 230, textHue, @"has seen fit to grant " + m_VictimName + " the sum of");
            AddLabel(207, 256, textHue, m_Restitution.ToString());
        }      
    }
}