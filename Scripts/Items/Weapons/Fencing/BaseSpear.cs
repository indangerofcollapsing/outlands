
using System;
using Server;
using Server.Items;

namespace Server.Items
{
    public abstract class BaseSpear : BaseMeleeWeapon
    {
        public override int BaseHitSound { get { return 0x23C; } }
        public override int BaseMissSound { get { return 0x238; } }

        public override SkillName BaseSkill { get { return SkillName.Fencing; } }
        public override WeaponType BaseType { get { return WeaponType.Piercing; } }
        public override WeaponAnimation BaseAnimation { get { return WeaponAnimation.Pierce2H; } }

        public BaseSpear(int itemID): base(itemID)
        {
        }

        public BaseSpear(Serial serial): base(serial)
        {
        }

        public override WeaponAnimation GetAnimation()
        {
            WeaponAnimation animation = WeaponAnimation.Slash1H;
            Mobile attacker = this.Parent as Mobile;

            //WarFork Override
            if (this is WarFork)
            {
                switch (Utility.RandomMinMax(1, 6))
                {
                    case 1: animation = WeaponAnimation.Slash1H; break;
                    case 2: animation = WeaponAnimation.Slash1H; break;
                    case 3: animation = WeaponAnimation.Slash2H; break;
                    case 4: animation = WeaponAnimation.Bash1H; break;
                    case 5: animation = WeaponAnimation.Pierce1H; break;
                    case 6: animation = WeaponAnimation.Pierce1H; break; 
                }

                return animation;
            }

            //ShortSpear Override
            if (this is ShortSpear)
            {
                switch (Utility.RandomMinMax(1, 7))
                {
                    case 1: animation = WeaponAnimation.Slash1H; break;
                    case 2: animation = WeaponAnimation.Slash1H; break;
                    case 3: animation = WeaponAnimation.Slash2H; break;
                    case 4: animation = WeaponAnimation.Bash1H; break;
                    case 5: animation = WeaponAnimation.Bash2H; break;
                    case 6: animation = WeaponAnimation.Pierce1H; break;
                    case 7: animation = WeaponAnimation.Pierce1H; break;
                }

                return animation;
            }  
            
            if (attacker != null)
            {                
                if (attacker.FindItemOnLayer(Layer.TwoHanded) is BaseShield)
                {                    
                    switch (Utility.RandomMinMax(1, 5))
                    {
                        case 1: animation = WeaponAnimation.Pierce2H; break;
                        case 2: animation = WeaponAnimation.Pierce2H; break;
                        case 3: animation = WeaponAnimation.Pierce2H; break;
                        case 4: animation = WeaponAnimation.ShootXBow; break;
                        case 5: animation = WeaponAnimation.Slash2H; break;    
                    }

                    return animation;
                }

                else if (attacker.FindItemOnLayer(Layer.TwoHanded) != null)
                {
                    switch (Utility.RandomMinMax(1, 5))
                    {
                        case 1: animation = WeaponAnimation.Pierce2H; break;
                        case 2: animation = WeaponAnimation.Pierce2H; break;
                        case 3: animation = WeaponAnimation.Pierce2H; break;
                        case 4: animation = WeaponAnimation.ShootXBow; break;
                        case 5: animation = WeaponAnimation.Slash2H; break;                        
                    }

                    return animation;
                }

                else
                {
                    switch (Utility.RandomMinMax(1, 5))
                    {
                        case 1: animation = WeaponAnimation.Pierce2H; break;
                        case 2: animation = WeaponAnimation.Pierce2H; break;
                        case 3: animation = WeaponAnimation.Pierce2H; break;
                        case 4: animation = WeaponAnimation.ShootXBow; break;
                        case 5: animation = WeaponAnimation.Slash2H; break;                     
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

        public override void OnHit(Mobile attacker, Mobile defender, double damageBonus)
        {
            base.OnHit(attacker, defender, damageBonus);

            AttemptWeaponPoison(attacker, defender);
        }
    }
}