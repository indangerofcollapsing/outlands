using System;

namespace Server.Custom.Townsystem
{
	public class Jhelom : Town
	{
		public Jhelom()
		{
            Definition =
                new TownDefinition(
                    2,
                    "Jhelom",
                    "Jhelom",
                    "JHELOM",
                    "The Town Crystal of Jhelom",
                    "a Jhelom town control brazier",
                    "a Jhelom town flag",
                    new Point3D(1327,3776,0),
                    new Point3D[] {},
                    new Point3D[] {new Point3D(1427,3703,0), new Point3D(1428,3703,0)},
                    new Point3D(1408,3707,0),
                    3648,
                    new Point3D(1427,3706,0),
                    new Rectangle3D[] { new Rectangle3D(new Point3D(1407,3695,0), new Point3D(1431,3704,20)), new Rectangle3D(new Point3D(1407,3704,0), new Point3D(1423,3719,20)) },
                    100
                    );
		}
	}
}