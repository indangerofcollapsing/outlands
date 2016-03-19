using System;
using System.Collections;
using Server;
using Server.Network;
using Server.Targets;
using Server.Targeting;
using Server.Mobiles;
using Server.Spells;

namespace Server.Items
{
    public class ExplodingAnimal : Item
    {
        public virtual Type CreatureType { get { return typeof(Sheep); }}
        public virtual int CreatureItemId { get { return 8427; } }
        public virtual String CreatureName { get { return "an exploding sheep"; } }

        public virtual int Radius { get { return 3; }}

        public virtual int MinDamage { get { return 200; } }
        public virtual int MaxDamage { get { return 300; } }

        [Constructable]
        public ExplodingAnimal(): base()
        {
            Weight = 5.0;
            Hue = 2620;
            ItemID = CreatureItemId;

            Name = "an exploding animal";
        }

        public ExplodingAnimal(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, CreatureName);            
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("That item must be in your pack in order to use it.");
                return;
            }

            from.SendMessage("Target the location you wish to place this at.");
            from.Target = new ExplodingAnimalTarget(this);
        }

        public class ExplodingAnimalTarget : Target
        {
            private ExplodingAnimal m_ExplodingAnimal;
            private IEntity targetLocation;

            public ExplodingAnimalTarget(ExplodingAnimal explodingAnimal): base(5, true, TargetFlags.None)
            {
                m_ExplodingAnimal = explodingAnimal;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                if (m_ExplodingAnimal.Deleted || m_ExplodingAnimal.RootParent != from)
                    return;

                PlayerMobile pm = from as PlayerMobile;

                if (pm == null)
                    return;

                 IPoint3D targetPoint = target as IPoint3D;

                if (targetPoint == null)
                    return;

                Map map = from.Map;

                if (map == null)
                    return;

                SpellHelper.GetSurfaceTop(ref targetPoint);

                if (targetPoint is Mobile)                
                    targetLocation = (Mobile)targetPoint;
                
                else                
                    targetLocation = new Entity(Serial.Zero, new Point3D(targetPoint), map);                

                if (!map.CanSpawnMobile(targetLocation.Location))
                {
                    from.SendLocalizedMessage(501942); // That location is blocked.
                    return;
                }
      
                from.RevealingAction();

                BaseCreature bc_Creature = (BaseCreature)Activator.CreateInstance(m_ExplodingAnimal.CreatureType);

                bc_Creature.Tamable = false;
                bc_Creature.Blessed = true;
                bc_Creature.Hue = 2620;
                bc_Creature.Name = m_ExplodingAnimal.CreatureName;
                bc_Creature.MoveToWorld(targetLocation.Location, targetLocation.Map);

                bc_Creature.Frozen = true;
                bc_Creature.CantWalk = true;

                bc_Creature.PlaySound(bc_Creature.GetAngerSound());
                
                bc_Creature.FaceRandomDirection();

                for (int a = 0; a < 11; a++)
                {
                    int tick = a;

                    Timer.DelayCall(TimeSpan.FromSeconds(a), delegate
                    {
                        if (bc_Creature == null) return;
                        if (bc_Creature.Deleted || !bc_Creature.Alive) return;

                        if (tick == 10)
                        {
                            bc_Creature.Blessed = false;

                            SpecialAbilities.AnimalExplosion(from, bc_Creature.Location, bc_Creature.Map, m_ExplodingAnimal.CreatureType, m_ExplodingAnimal.Radius, m_ExplodingAnimal.MinDamage, m_ExplodingAnimal.MaxDamage, 30, -1, true, false);

                            bc_Creature.Kill();
                        }

                        else
                        {
                            int countDown = 10 - tick;
                            bc_Creature.PublicOverheadMessage(MessageType.Regular, 0, false, countDown.ToString());

                            if (Utility.RandomDouble() <= .33)
                                bc_Creature.FaceRandomDirection();

                            if (Utility.RandomMinMax(1, 10) <= tick)
                                bc_Creature.PlaySound(bc_Creature.GetIdleSound());
                        }                        
                    });
                }

                if (from.AccessLevel == AccessLevel.Player)
                {
                    from.BeginAction(typeof(ExplodingAnimal));

                    Timer.DelayCall(TimeSpan.FromMinutes(60), delegate
                    {
                        if (from != null)
                            from.EndAction(typeof(ExplodingAnimal));
                    });
                }

                m_ExplodingAnimal.Delete();
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