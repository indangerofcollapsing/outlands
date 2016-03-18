using System;

namespace Server.Custom.Townsystem
{
	public class Yew : Town
	{
        public Yew()
		{
            Definition =
                new TownDefinition(
                    12,
                    "Yew",
                    "Yew",
                    "YEW",
                    "The Town Crystal of Yew",
                    "a Yew town control brazier",
                    "a Yew town flag",
                    new Point3D(639,856,0),
                    new Point3D[] {new Point3D(631,851,0)},
                    new Point3D[] {},
                    new Point3D(627,851,5),
                    3648,
                    new Point3D(634,851,0),
                    new Rectangle3D[] { new Rectangle3D(new Point3D(624,848,0), new Point3D(632,855,20))},
                    100
                    );
		}
	}
}