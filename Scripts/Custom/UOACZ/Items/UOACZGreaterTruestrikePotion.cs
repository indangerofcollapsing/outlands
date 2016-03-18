using System;
using Server.Network;
using Server;
using Server.Targets;
using Server.Targeting;
using Server.Spells;
using Server.Mobiles;
using Server.Spells;
using Server.Spells.Fifth;
using Server.Spells.Seventh;

namespace Server.Items
{            
    public class UOACZGreaterTruestrikePotion: BasePotion
    {
        [Constructable]
        public UOACZGreaterTruestrikePotion(): base(3846, PotionEffect.Custom)
        {
            Name = "a greater truestrike potion";
            Hue = 2964;

            Weight = 1.0;
            Movable = true;
        }

        public UOACZGreaterTruestrikePotion(Serial serial): base(serial)
        {
        }       

        public override void Drink(Mobile from)
        {
            if (this != null && ParentEntity != from.Backpack)            
                from.SendMessage("The potion must be in your pack to drink it.");            

            else
            {
                if (!from.CanBeginAction(typeof(UOACZGreaterTruestrikePotion)))
                {
                    from.SendMessage("You must wait before drinking another potion of that type.");
                    return;                
                }

                SpecialAbilities.CourageSpecialAbility(1.0, from, null, .10, 120, -1, true, "You begin to strike with increased accuracy.", "", "-1");

                from.BeginAction(typeof(UOACZGreaterTruestrikePotion));  

                Timer.DelayCall(TimeSpan.FromSeconds(120), delegate
                { 
                    from.EndAction(typeof(UOACZGreaterTruestrikePotion));
                });                

                Consume();                              
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
        }
    }
}