using System;
using Server;
using Server.Items;
using Server.Targets;

namespace Server.Items
{
    public abstract class BaseSword : BaseMeleeWeapon
    {
        public override SkillName DefSkill { get { return SkillName.Swords; } }
        public override WeaponType DefType { get { return WeaponType.Slashing; } }
        public override WeaponAnimation DefAnimation { get { return WeaponAnimation.Slash1H; } }

        public BaseSword(int itemID): base(itemID)
        {
        }

        public BaseSword(Serial serial): base(serial)
        {
        }

        public override WeaponAnimation GetAnimation()
        {
            WeaponAnimation animation = WeaponAnimation.Slash1H;
            Mobile attacker = this.Parent as Mobile;

            if (attacker != null)
            {
                if (attacker.FindItemOnLayer(Layer.TwoHanded) != null)
                {
                    switch (Utility.RandomMinMax(1, 7))
                    {
                        case 1: animation = WeaponAnimation.Slash1H; break;
                        case 2: animation = WeaponAnimation.Slash1H; break;
                        case 3: animation = WeaponAnimation.Bash1H; break;
                        case 4: animation = WeaponAnimation.Bash1H; break;
                        case 5: animation = WeaponAnimation.Pierce1H; break;
                        case 6: animation = WeaponAnimation.Pierce1H; break;
                        case 7: animation = WeaponAnimation.Slash1H; break;                      
                    }

                    return animation;
                }

                else
                {
                    switch (Utility.RandomMinMax(1, 5))
                    {
                        case 1: animation = WeaponAnimation.Slash1H; break;
                        case 2: animation = WeaponAnimation.Slash1H; break;
                        case 3: animation = WeaponAnimation.Slash2H; break;
                        case 4: animation = WeaponAnimation.Bash1H; break;
                        case 5: animation = WeaponAnimation.Pierce1H; break;                       
                    }

                    return animation;
                }
            }

            return animation;
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

        public override void OnDoubleClick(Mobile from)
        {
            from.SendLocalizedMessage(1010018); // What do you want to use this item on?

            from.Target = new BladedItemTarget(this);
        }

        public override void OnHit(Mobile attacker, Mobile defender, double damageBonus)
        {
            base.OnHit(attacker, defender, damageBonus);

            AttemptWeaponPoison(attacker, defender);
        }
    }
}