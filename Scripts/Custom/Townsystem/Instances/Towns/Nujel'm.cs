using System;

namespace Server.Custom.Townsystem
{
	public class Nujelm : Town
	{
        public Nujelm()
		{
            Definition =
                new TownDefinition(
                    6,
                    "Nujel'm",
                    "Nujel'm",
                    "NUJEL'M",
                    "The Town Crystal of Nujel'm",
                    "a Nujel'm town control brazier",
                    "a Nujel'm town flag",
                    new Point3D(3763,1328,0),
                    new Point3D[] {new Point3D(3765,1185,30), new Point3D(3765,1184,30)},
                    new Point3D[] { new Point3D(3765, 1185, 30), new Point3D(3764, 1185, 30), new Point3D(3763, 1185, 30) },
                    new Point3D(3767,1188,36),
                    3648,
                    new Point3D(3763,1184,27),
                    new Rectangle3D[] { new Rectangle3D(new Point3D(3759,1183,0), new Point3D(3771,1192,40)) },
                    50
                    );
		}
	}
}