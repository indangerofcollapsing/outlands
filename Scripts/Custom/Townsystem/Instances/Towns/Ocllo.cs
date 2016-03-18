using System;

namespace Server.Custom.Townsystem
{
	public class Ocllo : Town
	{
        public Ocllo()
		{
            Definition =
                new TownDefinition(
                    7,
                    "Ocllo",
                    "Ocllo",
                    "OCLLO",
                    "The Town Crystal of Ocllo",
                    "an Ocllo town control brazier",
                    "an Ocllo town flag",
                    new Point3D(3688,2520,0),
                    new Point3D[] {new Point3D(3681,2475,0)},
                    new Point3D[] {},
                    new Point3D(3692,2475,2),
                    3648,
                    new Point3D(3678,2475,0),
                    new Rectangle3D[] { new Rectangle3D(new Point3D(3681,2472,0), new Point3D(3695,2479,20))},
                    100
                    );
		}
	}
}