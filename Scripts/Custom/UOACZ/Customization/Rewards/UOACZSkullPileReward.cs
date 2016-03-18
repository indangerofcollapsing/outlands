using System;
using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Spells;

namespace Server.Items
{
	public class UOACZSkullPileRewardAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] 
        {
			{6872, 1, 1, 0, 2500},
            {6873, 0, 1, 0, 2500},
            {6874, -1, 1, 0, 2500},
            {6875, 0, 0, 0, 2500},
            {6876, 1, 0, 0, 2500},
            {6877, 1, -1, 0, 2500},
            {6878, 2, -1, 0, 2500},
            {6879, 2, 0, 0, 2500},
		};
            
		public override BaseAddonDeed Deed { get { return new UOACZSkullPileRewardAddonDeed(); }}

        public static TimeSpan UsageCooldown = TimeSpan.FromMinutes(10);

        public DateTime NextUsageAllowed = DateTime.UtcNow;
        public BaseCreature creature = null;

		[ Constructable ]
		public UOACZSkullPileRewardAddon()
		{
            for (int i = 0; i < m_AddOnSimpleComponents.Length / 5; i++)
                AddComponent(new AddonComponent(m_AddOnSimpleComponents[i, 0], m_AddOnSimpleComponents[i, 4]), m_AddOnSimpleComponents[i, 1], m_AddOnSimpleComponents[i, 2], m_AddOnSimpleComponents[i, 3]);
        }

        public UOACZSkullPileRewardAddon(Serial serial): base(serial)
		{
        }

        public override void OnComponentUsed(AddonComponent c, Mobile from)
        {
            base.OnComponentUsed(c, from);
            
            if (!from.Alive)
                return;

            if (DateTime.UtcNow < NextUsageAllowed && from.AccessLevel == AccessLevel.Player)
            {
                string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, NextUsageAllowed, false, true, true, true, true);
                from.SendMessage("You must wait another " + timeRemaining + " before that may be used again.");

                return;
            }

            NextUsageAllowed = DateTime.UtcNow + UsageCooldown;

            BaseCreature deadCreature = null;

            switch(Utility.RandomMinMax(1, 3))
            {
                case 1: deadCreature = new Skeleton(); break;
                case 2: deadCreature = new Zombie(); break;
                case 3: deadCreature = new Ghoul(); break;
            }

            if (deadCreature == null)
                return;

            creature = deadCreature;

            Point3D creatureLocation = new Point3D(Location.X + 1, Location.Y + 1, Location.Z + 6);

            deadCreature.Blessed = true;
            deadCreature.Hidden = true;
            deadCreature.Frozen = true;

            deadCreature.MoveToWorld(creatureLocation, c.Map);

            int animation = Utility.RandomMinMax(2, 3);
            int frameCount = 4;

            int bonesItemID = 6937;

            int radius = 3;
            double smallBonesChance = .25;

            Point3D startLocation = Location;
            Map startMap = Map;

            for (int a = -1 * radius; a < radius + 1; a++)
            {
                for (int b = -1 * radius; b < radius + 1; b++)
                {
                    Point3D newPoint = new Point3D(Location.X + a, Location.Y + b, Location.Z);
                    SpellHelper.AdjustField(ref newPoint, Map, 12, false);

                    double distanceFromCenter = Utility.GetDistanceToSqrt(Location, newPoint);

                    double extraBonesChance = 1;

                    if (distanceFromCenter >= 1)
                        extraBonesChance = (1 / (distanceFromCenter)) * smallBonesChance;

                    if (Utility.RandomDouble() <= extraBonesChance)
                    {
                        Timer.DelayCall(TimeSpan.FromSeconds(distanceFromCenter * .25), delegate
                        {
                            if (this == null) return;
                            if (Deleted) return;

                            TimedStatic smallBones = new TimedStatic(Utility.RandomList(6929, 6930, 6937, 6938, 6933, 6934, 6935, 6936, 6939, 6940, 6882, 6883), 5);
                            smallBones.Name = "bones";
                            smallBones.MoveToWorld(startLocation, startMap);
                        });
                    }
                }
            }

            for (int a = 0; a < 30; a++)
            {
                Point3D newPoint = new Point3D(Location.X + Utility.RandomList(-4, -3, -2, 2, 3, 4), Location.Y + Utility.RandomList(-4, -3, -2, 2, 3, 4), Location.Z);
                SpellHelper.AdjustField(ref newPoint, Map, 12, false);

                TimedStatic smallBones = new TimedStatic(Utility.RandomList(6929, 6930, 6937, 6938, 6933, 6934, 6935, 6936, 6939, 6940, 6882, 6883), 5);
                smallBones.Name = "bones";
                smallBones.MoveToWorld(newPoint, Map);
            }

            Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate
            {
                if (deadCreature == null) return;
                if (deadCreature.Deleted || !deadCreature.Alive) return;

                deadCreature.Hidden = false;

                deadCreature.PlaySound(deadCreature.GetIdleSound());
                deadCreature.Animate(animation, frameCount, 1, false, false, 2);

                Timer.DelayCall(TimeSpan.FromSeconds(2), delegate
                {
                    if (deadCreature == null) return;
                    if (deadCreature.Deleted) return;

                    deadCreature.PlaySound(deadCreature.GetAngerSound());
                    deadCreature.Delete();
                });
            });
        }

        public override void OnLocationChange(Point3D oldLoc)
        {
            base.OnLocationChange(oldLoc);

            if (creature != null)
            {
                if (!creature.Deleted)
                    creature.Delete();
            }
        }

        public override void OnMapChange()
        {
            base.OnMapChange();

            if (creature != null)
            {
                if (!creature.Deleted)
                    creature.Delete();
            }
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (creature != null)
            {
                if (!creature.Deleted)
                    creature.Delete();
            }
        }      

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 ); // Version

            //Version 0
            writer.Write(NextUsageAllowed);
            writer.Write(creature);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                NextUsageAllowed = reader.ReadDateTime();
                creature = (BaseCreature)reader.ReadMobile();
            }

            if (creature != null)
            {
                if (!creature.Deleted)
                    creature.Delete();
            }
		}
	}

	public class UOACZSkullPileRewardAddonDeed : BaseAddonDeed
	{
        public override BaseAddon Addon { get { return new UOACZSkullPileRewardAddon(); } }

		[Constructable]
		public UOACZSkullPileRewardAddonDeed()
		{
            Name = "a skull pile deed";
		}

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            LabelTo(from, "(UOACZ Reward)");
        }

		public UOACZSkullPileRewardAddonDeed( Serial serial ) : base( serial )
        {
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 ); // Version
		}

		public override void	Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}