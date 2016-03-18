using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using System.Collections;
using System.Collections.Generic;
using Server.Gumps;

namespace Server.Custom
{
    public class PetBattleGrimoire : Item
    {   
        [Constructable]
        public PetBattleGrimoire()
        {
            ItemID = 8787;
            Name = "pet battle creature grimoire";
            Hue = 966;
        }

        public PetBattleGrimoire(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            PlayerMobile pm_From = from as PlayerMobile;

            if (pm_From == null)
                return;

            PetBattleCreatureCollection playerCreatureCollection = PetBattlePersistance.GetPlayerPetBattleCreatureCollection(pm_From);

            if (playerCreatureCollection != null)
            {
                pm_From.PlaySound(0x055);
                
                pm_From.CloseGump(typeof(Gumps.PetBattleGrimoireGump));
                pm_From.SendGump(new Gumps.PetBattleGrimoireGump(1, null, pm_From, playerCreatureCollection));
            }            
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