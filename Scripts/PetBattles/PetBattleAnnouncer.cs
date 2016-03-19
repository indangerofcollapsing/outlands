using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Collections.Generic;
using Server;
using Server.ContextMenus;
using Server.Mobiles;
using Server.Items;
using Server.Custom;

namespace Server.ArenaSystem
{
    public class PetBattleAnnouncer : BaseVendor
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }
        
        [Constructable]
        public PetBattleAnnouncer(): base("the announcer")
        {   
            SpeechHue = Utility.RandomDyedHue();
            Hue = Utility.RandomSkinHue();

            Blessed = true;
            Frozen = true;
            Direction = Direction.South;            
            
            AddItem(new FancyShirt(Utility.RandomNeutralHue()));
            AddItem(new ShortPants(Utility.RandomNeutralHue()));
            AddItem(new Boots(Utility.RandomNeutralHue()));

            int hairHue = GetHairHue();

            Utility.AssignRandomHair(this, hairHue);
            Utility.AssignRandomFacialHair(this, hairHue);
        }

        public PetBattleAnnouncer(Serial serial): base(serial)
		{
		}

        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBPetBattle());
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }        
    }
}
