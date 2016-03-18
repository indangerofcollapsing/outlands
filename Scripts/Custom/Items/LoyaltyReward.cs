using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Server.Custom.Items
{
    class LoyaltyReward : Item
    {
        private string fileName = "loyalty_rewards.log";
        private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public override string DefaultName
        {
            get
            {
                return "a loyalty deed";
            }
        }

        [Constructable]
        public LoyaltyReward()
            : base(0x14F0)
        {
            Hue = 2410;
        }

        public LoyaltyReward(Serial serial)
            : base(serial)
        {

        }

        public override void OnDoubleClick(Mobile from)
        {
            try
            {
                using (StreamWriter op = new StreamWriter(fileName, true))
                {
                    string code = GenerateCode();
                    op.WriteLine("{0},{1},{2} ", DateTime.UtcNow.ToShortDateString(), from.Name.ToUpper(), code);
                    from.SendMessage(String.Format("Please write down this code for relaunch: {0}", code));
                    this.Delete();
                }
            }
            catch (Exception ex)
            {
                from.SendMessage("Sorry, there was an error, please try using your deed again.");
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        break;
                    }
            }
        }

        private string GenerateCode()
        {
            int size = 8;
            char[] buffer = new char[size];

            for (int i = 0; i < size; i++)
            {
                buffer[i] = chars[Utility.Random(chars.Length)];
            }
            return new string(buffer);
        }
    }
}
