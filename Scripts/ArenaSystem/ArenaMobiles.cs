using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Server.Mobiles;
using Server.Items;

namespace Server.ArenaSystem
{
    public class ArenaAnnouncer : BaseCreature
    {
        public ArenaAnnouncer()
            : base(AIType.AI_Vendor, FightMode.None, 10, 0, 0, 0)
        {
            AccessLevel = AccessLevel.Player;
            InitBody();
            InitOutfit();

            Frozen = true;
            CantWalk = true;
            Blessed = true;
        }
        public ArenaAnnouncer(Serial serial)
            : base(AIType.AI_Vendor, FightMode.None, 10, 0, 0, 0)
        {
            Frozen = true;
            CantWalk = true;
            Blessed = true;
        }
        public override void Serialize(GenericWriter writer)
        {
        }
        public override void Deserialize(GenericReader reader)
        {
        }
        public virtual void InitBody()
        {
            //SetStr(90, 100);
            //SetDex(90, 100);
            //SetInt(15, 25);

            Hue = Utility.RandomSkinHue();

            if (Female = Utility.RandomBool())
            {
                Body = 401;
                Name = NameList.RandomName("female");
            }
            else
            {
                Body = 400;
                Name = NameList.RandomName("male");
            }
        }
        public virtual void InitOutfit()
        {
            AddItem(new FancyShirt(Utility.RandomNeutralHue()));
            AddItem(new ShortPants(Utility.RandomNeutralHue()));
            AddItem(new Boots(Utility.RandomNeutralHue()));
            Utility.AssignRandomHair(this);
        }
    }
}
