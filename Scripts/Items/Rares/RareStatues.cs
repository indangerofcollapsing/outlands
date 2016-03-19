using System;
using Server.Items;
using System.Collections;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;
using Server.Mobiles;

namespace Server
{
    public class ValkyrieTotem : RareStatue
    {
        public override string DisplayName { get { return "valkyrie totem"; } }
        public override int DisplayItemId { get { return 17048; } }
        public override int DisplayItemHue { get { return 0; } }
        public override int ClickSound { get { return Utility.RandomList(-1); } }

        [Constructable]
        public ValkyrieTotem(): base()
        {
        }

        public ValkyrieTotem(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }

    public class PeradunsSkull : RareStatue
    {
        public override string DisplayName { get { return "peradun's skull"; } }
        public override int DisplayItemId { get { return 7960; } }
        public override int DisplayItemHue { get { return 2410; } }
        public override int ClickSound { get { return Utility.RandomList(-1); } }

        [Constructable]
        public PeradunsSkull(): base()
        {
        }

        public PeradunsSkull(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }

    public class ForgottenGravestone : RareStatue
    {
        public override string DisplayName { get { return "forgotten gravestone"; } }
        public override int DisplayItemId { get { return 3798; } }
        public override int DisplayItemHue { get { return 2500; } }
        public override int ClickSound { get { return Utility.RandomList(-1); } }

        [Constructable]
        public ForgottenGravestone(): base()
        {
        }

        public ForgottenGravestone(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }

    public class WarbossWaraxe : RareStatue
    {
        public override string DisplayName { get { return "warboss waraxe"; } }
        public override int DisplayItemId { get { return 9575; } }
        public override int DisplayItemHue { get { return 2413; } }
        public override int ClickSound { get { return Utility.RandomList(-1); } }

        [Constructable]
        public WarbossWaraxe(): base()
        {
        }

        public WarbossWaraxe(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }

    public class HydraSpawnling : RareStatue
    {
        public override string DisplayName { get { return "hydra spawnling"; } }
        public override int DisplayItemId { get { return 11659; } }
        public override int DisplayItemHue { get { return 0; } }
        public override int ClickSound { get { return Utility.RandomList(-1); } }

        [Constructable]
        public HydraSpawnling(): base()
        {
        }

        public HydraSpawnling(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }

    public class DragonEgg : RareStatue
    {
        public override string DisplayName { get { return "dragon egg"; } }
        public override int DisplayItemId { get { return 4963; } }
        public override int DisplayItemHue { get { return 2539; } }
        public override int ClickSound { get { return Utility.RandomList(-1); } }

        [Constructable]
        public DragonEgg(): base()
        {
        }

        public DragonEgg(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }

    public class GiantMushroom : RareStatue
    {
        public override string DisplayName { get { return "giant mushroom"; } }
        public override int DisplayItemId { get { return 8752; } }
        public override int DisplayItemHue { get { return 1758; } }
        public override int ClickSound { get { return Utility.RandomList(-1); } }

        [Constructable]
        public GiantMushroom(): base()
        {
        }

        public GiantMushroom(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }

    public class MummifiedCorpse : RareStatue
    {
        public override string DisplayName { get { return "mummified corpse"; } }
        public override int DisplayItemId { get { return 7200; } }
        public override int DisplayItemHue { get { return 0; } } 
        public override int ClickSound { get { return Utility.RandomList(-1); } }

        [Constructable]
        public MummifiedCorpse(): base()
        {
        }

        public MummifiedCorpse(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }

    public class AncientNecromancerStatue : RareStatue
    {
        public override string DisplayName { get { return "trophy: ancient necromancer"; } }
        public override int DisplayItemId { get { return 17050; } }
        public override int DisplayItemHue { get { return 0; } } 
        public override int ClickSound { get { return Utility.RandomList(0x2BC, 0x2B9, 0x2BA, 0x621, 0x58D); } }

        [Constructable]
        public AncientNecromancerStatue(): base()
        {
        }

        public AncientNecromancerStatue(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }

    public class MaggotStatue : RareStatue
    {
        public override string DisplayName { get { return "trophy: maggot"; } }
        public override int DisplayItemId { get { return 11650; } }
        public override int DisplayItemHue { get { return 0; } } 
        public override int ClickSound { get { return Utility.RandomList(0x59F, 0x571, 0x573, 0x570, 0x59D); } }

        [Constructable]
        public MaggotStatue(): base()
        {
        }

        public MaggotStatue(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }

    public class LythTheDestroyerStatue : RareStatue
    {
        public override string DisplayName { get { return "trophy: lyth the destroyer"; } }
        public override int DisplayItemId { get { return 17062; } }
        public override int DisplayItemHue { get { return 0; } } 
        public override int ClickSound { get { return Utility.RandomList(0x4FC, 0x4FD, 0x4FC, 0x4EC, 0x4EA); } }

        [Constructable]
        public LythTheDestroyerStatue(): base()
        {
        }

        public LythTheDestroyerStatue(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }

    public class DemonwebQueenStatue : RareStatue
    {
        public override string DisplayName { get { return "trophy: demonweb queen"; } }
        public override int DisplayItemId { get { return 8531; } }
        public override int DisplayItemHue { get { return 0; } } 
        public override int ClickSound { get { return Utility.RandomList(0x4FF, 0x4FD, 0x4EE, 0x4EF, 0x4EB); } }  

        [Constructable]
        public DemonwebQueenStatue(): base() 
        {           
        }

        public DemonwebQueenStatue(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }

    public class TheDeepOneStatue : RareStatue
    {
        public override string DisplayName { get { return "trophy: the deep one"; } }
        public override int DisplayItemId { get { return 17037; } }
        public override int DisplayItemHue { get { return 2569; } }
        public override int ClickSound { get { return Utility.RandomList(0x4E2, 0x4E3, 0x626, 0x628, 0x4F5); } }

        [Constructable]
        public TheDeepOneStatue(): base()
        {
        }

        public TheDeepOneStatue(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }

    public class RareStatue : Item
    {
        public virtual string DisplayName { get { return "a rare statue"; } }
        public virtual int DisplayItemId { get { return 17058; } } 
        public virtual int DisplayItemHue { get { return 0; } } 
        public virtual int ClickSound { get { return 0x001; } }        

        public override bool Decays { get { return false; } }

        [Constructable]
        public RareStatue(): base(0x1224) 
        {
            Weight = 1.0;
            ItemID = DisplayItemId;
            Hue = DisplayItemHue;
        }

        public RareStatue(Serial serial) : base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, DisplayName);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (ClickSound != -1)
                from.PlaySound(ClickSound);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version
            
            //--

            ItemID = DisplayItemId; 
            Hue = DisplayItemHue;
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }
}