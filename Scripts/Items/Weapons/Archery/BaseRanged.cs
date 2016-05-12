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

        public override TimeSpan GetStationaryDelayRequired(Mobile attacker)
        {
            return TimeSpan.FromSeconds(.5);

            /*
            if (attacker is BaseCreature)
                return TimeSpan.FromSeconds(.25);
             
            double minDelay = .50;
            double maxDelay = .75;

            double minDex = 25;
            double maxDex = 100;

            int effectiveDex = attacker.Dex;

            if (effectiveDex > 100)
                effectiveDex = 100;

            if (effectiveDex < 25)
                effectiveDex = 25;

            double dexScalar = (effectiveDex - minDex) / (maxDex - minDex);

            return TimeSpan.FromSeconds(maxDelay - ((maxDelay - minDelay) * dexScalar));
            */
        }

        public override TimeSpan OnSwing(Mobile attacker, Mobile defender)
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

            attacker.RevealingAction();

            return GetDelay(attacker, false);            
        }

        public override void OnHit(Mobile attacker, Mobile defender, double damageBonus)
        {
            double arrowChance = 0.4;
            
            if (UOACZSystem.IsUOACZValidMobile(attacker))
                arrowChance = .66;   

            if (attacker.Player && !defender.Player && (defender.Body.IsAnimal || defender.Body.IsMonster) && arrowChance >= Utility.RandomDouble())            
                defender.AddToBackpack(Ammo);            

            base.OnHit(attacker, defender, damageBonus);

            if (!(attacker is PlayerMobile && defender is PlayerMobile))            
                AttemptWeaponPoison(attacker, defender);            
        }

        public override void OnMiss(Mobile attacker, Mobile defender)
        {
            double arrowChance = 0.4;

            if (UOACZSystem.IsUOACZValidMobile(attacker))
                arrowChance = .66;            

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
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
            }
        }
    }
}