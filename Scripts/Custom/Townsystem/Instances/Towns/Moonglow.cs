using System;

namespace Server.Custom.Townsystem
{
	public class Moonglow : Town
	{
        public Moonglow()
		{
            Definition =
                new TownDefinition(
                    5,
                    "Moonglow",
                    "Moonglow",
                    "MOONGLOW",
                    "The Town Crystal of Moonglow",
                    "a Moonglow town control brazier",
                    "a Moonglow town flag",
                    new Point3D(4479,1168,0),
                    new Point3D[] { new Point3D(4475, 1124, 0), new Point3D(4475, 1125, 0) },
                    new Point3D[] { },
                    new Point3D(4468,1125,0),
                    3648,
                    new Point3D(4479,1125,0),
                    new Rectangle3D[] { new Rectangle3D(new Point3D(4467,1117,0), new Point3D(4476,1133,20)) },
                    200
                    );
		}
	}
}