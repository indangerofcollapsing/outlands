using System;
using Server;

namespace Server.Engines.Quests.Paladin
{
    public class DontOfferConversation : QuestConversation
    {
        public override object Message
        {
            get
            {
                string s = "<I>The Paladin Order Guard considers you for a momenent then says,</I><BR><BR>" +
                  "Hmmm... I could perhaps benefit from your assistance, but you seem to be " +
                  "busy with another task at the moment. Return to me when you complete whatever " +
                  "it is that you're working on and maybe I can still put you to good use.";

                return s;
            }
        }

        public override bool Logged { get { return false; } }

        public DontOfferConversation()
        {
        }
    }

    public class DeceitItemReturned : QuestConversation
    {
        public override object Message
        {
            get
            {
                return "Welcome back! Your bravery is truly a beacon of light in these treacherous times. Return to us the stolen Paladin Shield from the depths of Hythloth, and you shall be welcome in our ranks. Make haste, for surely the sword's disappearance will not go unnoticed.";
            }
        }

        public override void OnRead()
        {
            ReturnHythlothItem qo = new ReturnHythlothItem();
            System.AddObjective(qo);
            qo.GenerateHythlothItem(System.From);
        }

        public DeceitItemReturned()
        {
        }
    }

    public class HythlothItemReturned : QuestConversation
    {
        public override object Message
        {
            get
            {
                return "Glory be to the Paladins! Thou hast returned the holy artifacts, and have fulfilled thy promises to the Order. Now, finally I can dub thee a...<br><br>Wait!<br><br>Evil Stirs!<br><br>The removal of these holy artifacts from the clutches of these monsters has captured their attention.<br><br>Quickly! Defend the city and its citizens!<br><br>Do not let the hand of evil grasp these artifacts once again!";
        
            }
        }

        public HythlothItemReturned()
        {
        }

        public override void OnRead()
        {
            if (PaladinSiege.CanBeginSiege())
            {
                System.AddObjective(new DefendTrinsic());
                PaladinSiege.BeginSiege(System.From);
            }
            else
                System.AddConversation(new SiegeInProcess());
        }
    }

    public class SiegeInProcess : QuestConversation
    {
        public override object Message
        {
            get
            {
                return "The city is currently being attacked! Come back later!";

            }
        }

        public SiegeInProcess()
        {
        }

        public override void OnRead()
        {
            System.AddObjective(new WaitingForSiege());
        }
    }

    public class FailMessage : QuestConversation
    {
        public override object Message
        {
            get
            {
                return "You have failed in protecting the town and the artifacts! They have once again been stolen!";

            }
        }

        public FailMessage()
        {
        }
    }

    public class SuccessMessage : QuestConversation
    {
        public override object Message
        {
            get
            {
                return "Success! Thou art truly worthy of the Order's recognition! Consider thyself among the ranks of the Paladins from this day forward... For the Order! For the Light!";

            }
        }

        public SuccessMessage()
        {
        }
    }
}