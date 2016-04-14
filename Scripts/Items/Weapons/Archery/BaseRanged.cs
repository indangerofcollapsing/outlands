using System;
using Server.Items;
using Server.Network;
using Server.Spells;
using Server.Mobiles;
using Server.Engines.ConPVP;

namespace Server.Items
{
    public abstract class BaseRanged : BaseMeleeWeapon
    {
        public abstract int EffectID { get; }
        public abstract Type AmmoType { get; }
        public abstract Item Ammo { get; }

        public override int BaseHitSound { get { return 0x234; } }
        public override int BaseMissSound { get { return 0x238; } }

        public override SkillName BaseSkill { get { return SkillName.Archery; } }
        public override WeaponType BaseType { get { return WeaponType.Ranged; } }
        public override WeaponAnimation BaseAnimation { get { return WeaponAnimation.ShootXBow; } }

        public override CraftResource DefaultResource { get { return CraftResource.RegularWood; } }
        
        public BaseRanged(int itemID): base(itemID)
        {
        }

        public BaseRanged(Serial serial): base(serial)
        {
        }

        public static int RangedShotDelay(int stam)
        {
            double maxDelay = 750; //ms Delay At 25 Dex
            double minDelay = 500; //ms Delay at 100 Dex          
            double range = maxDelay - minDelay;
            double rangeRatio = range / 75;

            if (stam > 100)
                stam = 100;

            if (stam < 25)
                stam = 25;

            int delay = (int)(maxDelay - (rangeRatio * ((double)stam - 25)));
            
            return delay;             

            //Old Version
            //350ms at 100 stam & 996ms at 25 stam
            //return (int)(1000 * Math.Max(0.35, Math.Cos(stam * stam / (2400 * Math.PI))));
        }

        public override TimeSpan OnSwing(Mobile attacker, Mobile defender)
        {   
            // Make sure we've been standing still for the required time
            bool still_enough = Core.TickCount - attacker.LastMoveTime >= RangedShotDelay(attacker.Dex);

            if (still_enough)
            {
                if (attacker.HarmfulCheck(defender))
                {
                    attacker.DisruptiveAction();
                    attacker.Send(new Swing(0, attacker, defender));
                    
                    if (OnFired(attacker, defender))
                    {
                        if (attacker is BaseCreature)
                        {
                            BaseCreature bc_Attacker = attacker as BaseCreature;
                            bc_Attacker.OnSwing(defender);
                        }

                        if (CheckHit(attacker, defender))
                            OnHit(attacker, defender);
                        else
                            OnMiss(attacker, defender);
                    }
                }

                //See what is that
                attacker.RevealingAction();

                return GetDelay(attacker, false);
            }

            else
            {
                attacker.RevealingAction();

                return TimeSpan.FromSeconds(0.25);
            }
        }

        public override void OnHit(Mobile attacker, Mobile defender, double damageBonus)
        {
            double arrowChance = 0.4;
            
            if (UOACZSystem.IsUOACZValidMobile(attacker))
                arrowChance = .66;   

            if (attacker.Player && !defender.Player && (defender.Body.IsAnimal || defender.Body.IsMonster) && arrowChance >= Utility.RandomDouble())            
                defender.AddToBackpack(Ammo);            

            base.OnHit(attacker, defender, damageBonus);

            //Disabled for PvP
            if (!(attacker is PlayerMobile && defender is PlayerMobile))
            {
                AttemptWeaponPoison(attacker, defender);
            }
        }

        public override void OnMiss(Mobile attacker, Mobile defender)
        {
            double arrowChance = 0.4;

            if (UOACZSystem.IsUOACZValidMobile(attacker))
                arrowChance = .66;            

            // no dropping arrows if we don't use any
            if (attacker.Player && arrowChance >= Utility.RandomDouble() && !DuelContext.IsFreeConsume(attacker))
                Ammo.MoveToWorld(new Point3D(defender.X + Utility.RandomMinMax(-1, 1), defender.Y + Utility.RandomMinMax(-1, 1), defender.Z), defender.Map);

            base.OnMiss(attacker, defender);
        }

        public virtual bool OnFired(Mobile attacker, Mobile defender)
        {
            Container pack = attacker.Backpack;

            if (attacker.Player && !DuelContext.IsFreeConsume(attacker) && (pack == null || !pack.ConsumeTotal(AmmoType, 1)))
            {
                attacker.StealthAttackActive = false;
                attacker.StealthAttackReady = false;

                return false;
            }

            attacker.MovingEffect(defender, EffectID, 18, 1, false, false);

            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 2:
                case 1:
                    {
                        break;
                    }
                case 0:
                    {
                        /*m_EffectID =*/
                        reader.ReadInt();
                        break;
                    }
            }
        }
    }
}