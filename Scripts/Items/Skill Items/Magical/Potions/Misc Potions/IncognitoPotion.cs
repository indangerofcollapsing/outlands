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
    public class IncognitoPotion: BasePotion
    {
        [Constructable]
        public IncognitoPotion(): base(0xF0B, PotionEffect.Custom)
        {
            Name = "an incognito potion";
            Hue = 2515;

            Weight = 1.0;
            Movable = true;
        }

        public IncognitoPotion(Serial serial): base(serial)
        {
        }       

        public override void Drink(Mobile from)
        {
            if (this != null && ParentEntity != from.Backpack)            
                from.SendMessage("The potion must be in your pack to drink it.");            

            else
            {
                if (!from.CanBeginAction(typeof(IncognitoSpell)))
                {
                    from.SendMessage("You are already under the influence of an incognito effect.");
                    return;
                }

                if (DisguiseTimers.IsDisguised(from))
                    from.SendMessage("You cannot can drink that while being disguised.");

                if (KinPaint.IsWearingKinPaint(from))
                {
                    from.SendMessage("You cannot can drink that while wearing kin paint.");
                    return;
                }

                if (!from.CanBeginAction(typeof(PolymorphSpell)) || from.IsBodyMod)
                {
                    from.SendMessage("You cannot can drink that while under the effect of a modification spell, ability, or item.");
                    return;                
                }

                from.FixedParticles(0x373A, 10, 15, 5036, EffectLayer.Head);
                from.PlaySound(0x3BD);
                from.Animate(34, 5, 1, true, false, 0);

                BasePotion.PlayDrinkEffect(from);

                from.HueMod = from.Race.RandomSkinHue();

                from.NameMod = from.Female ? NameList.RandomName("female") : NameList.RandomName("male");

                PlayerMobile pm = from as PlayerMobile;

                if (pm != null && pm.Race != null)
                {
                    pm.SetHairMods(pm.Race.RandomHair(pm.Female), pm.Race.RandomFacialHair(pm.Female));
                    pm.HairHue = pm.Race.RandomHairHue();
                    pm.FacialHairHue = pm.Race.RandomHairHue();
                }

                BaseArmor.ValidateMobile(from);

                from.BeginAction(typeof(IncognitoSpell));  

                Timer.DelayCall(TimeSpan.FromSeconds(300), delegate { ChangeBack(from); });                

                Consume();                              
            }
        }

        public void ChangeBack(Mobile from)
        {
            if (from is PlayerMobile)
                ((PlayerMobile)from).SetHairMods(-1, -1);

            from.BodyMod = 0;
            from.HueMod = -1;
            from.NameMod = null;
            from.EndAction(typeof(IncognitoSpell));

            BaseArmor.ValidateMobile(from);          
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