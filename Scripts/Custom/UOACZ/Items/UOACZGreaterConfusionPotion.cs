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
using System.Collections;
using System.Collections.Generic;

namespace Server.Items
{            
    public class UOACZGreaterConfusionPotion: BasePotion
    {
        [Constructable]
        public UOACZGreaterConfusionPotion(): base(3846, PotionEffect.Custom)
        {
            Name = "a greater confusion potion";
            Hue = 2960;

            Weight = 1.0;
            Movable = true;
        }

        public UOACZGreaterConfusionPotion(Serial serial): base(serial)
        {
        }       

        public override void Drink(Mobile from)
        {
            if (this != null && ParentEntity != from.Backpack)            
                from.SendMessage("The potion must be in your pack to drink it.");            

            else
            {
                if (!from.CanBeginAction(typeof(UOACZGreaterConfusionPotion)))
                {
                    from.SendMessage("You must wait before using another potion of that type.");
                    return;                
                }

                from.BeginAction(typeof(UOACZGreaterConfusionPotion));  

                Timer.DelayCall(TimeSpan.FromSeconds(60), delegate
                { 
                    from.EndAction(typeof(UOACZGreaterConfusionPotion));
                });                

                Consume();

                Point3D location = from.Location;
                Map map = from.Map;

                int range = 4;
                double duration = 8;

                Queue m_Queue = new Queue();

                IPooledEnumerable nearbyMobiles = map.GetMobilesInRange(location, range);

                foreach (Mobile mobile in nearbyMobiles)
                {
                    if (mobile == from) continue;
                    if (!UOACZSystem.IsUOACZValidMobile(mobile)) continue;
                    if (!(mobile is BaseCreature)) continue;

                    bool validCombatant = false;

                    if (mobile.Combatant != null && mobile.Combatant == from)
                        validCombatant = true;

                    foreach (AggressorInfo aggressor in from.Aggressors)
                    {
                        if (aggressor.Attacker == mobile || aggressor.Defender == mobile)
                            validCombatant = true;
                    }

                    foreach (AggressorInfo aggressed in from.Aggressed)
                    {
                        if (aggressed.Attacker == mobile || aggressed.Defender == mobile)
                            validCombatant = true;
                    }

                    if (mobile is UOACZBaseHuman && !validCombatant)
                        continue;

                    m_Queue.Enqueue(mobile);
                }

                nearbyMobiles.Free();

                while (m_Queue.Count > 0)
                {
                    BaseCreature bc_Creature = (BaseCreature)m_Queue.Dequeue();
                    
                    bc_Creature.Pacify(from, DateTime.UtcNow + TimeSpan.FromSeconds(duration), false);
                }

                for (int a = 0; a < 30; a++)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(a * .05), delegate
                    {
                        if (!UOACZPersistance.Active) return;

                        Point3D effectLocation = new Point3D(location.X + Utility.RandomMinMax(-1 * range, range), location.Y + Utility.RandomMinMax(-1 * range, range), location.Z);
                        SpellHelper.AdjustField(ref effectLocation, map, 12, false);

                        IEntity explosionLocationEntity = new Entity(Serial.Zero, new Point3D(effectLocation.X, effectLocation.Y, effectLocation.Z - 1), map);

                        switch (Utility.RandomMinMax(1, 3))
                        {
                            case 1: Effects.SendLocationParticles(explosionLocationEntity, 0x37B9, 10, 6, 0, 0, 5044, 0); break;
                            case 2: Effects.SendLocationParticles(explosionLocationEntity, 0x372A, 10, 20, 0, 0, 5029, 0); break;
                            case 3: Effects.SendLocationParticles(explosionLocationEntity, Utility.RandomList(0x36BD, 0x36BF, 0x36CB, 0x36BC), 30, 7, 2499, 0, 5044, 0); break;                                
                        }

                        Effects.SendLocationParticles(explosionLocationEntity, 0x3709, 10, 30, 2499, 0, 5029, 0);
                        Effects.PlaySound(explosionLocationEntity.Location, map, Utility.RandomList(0x22A, 0x229));
                    });
                }
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