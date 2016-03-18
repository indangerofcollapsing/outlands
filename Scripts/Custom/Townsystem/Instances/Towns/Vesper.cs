using System;

namespace Server.Custom.Townsystem
{
	public class Vesper : Town
	{
        public Vesper()
		{
            Definition =
                new TownDefinition(
                    11,
                    "Vesper",
                    "Vesper",
                    "VESPER",
                    "The Town Crystal of Vesper",
                    "a Vesper town control brazier",
                    "a Vesper town flag",
                    new Point3D(2889,684,0),
                    new Point3D[] {new Point3D(2839,943,0),new Point3D(2839,944,0)},
                    new Point3D[] {},
                    new Point3D(2832,943,0),
                    3648,
                    new Point3D(2842,943,0),
                    new Rectangle3D[] { new Rectangle3D(new Point3D(2831,935,0), new Point3D(2843,952,20))},
                    200
                    );
		}
	}
}