using System;

namespace Server.Custom.Townsystem
{
	public class Minoc : Town
	{
		public Minoc()
		{
            Definition =
                new TownDefinition(
                    4,
                    "Minoc",
                    "Minoc",
                    "MINOC",
                    "The Town Crystal of Minoc",
                    "a Minoc town control brazier",
                    "a Minoc town flag",
                    new Point3D(2508,560,0),
                    new Point3D[] { new Point3D(2423,523,0) },
                    new Point3D[] { },
                    new Point3D(2416,523,0),
                    3648,
                    new Point3D(2426,523,0),
                    new Rectangle3D[] { new Rectangle3D(new Point3D(2416,520,0), new Point3D(2424,527,20)) },
                    150
                    );
		}
	}
}