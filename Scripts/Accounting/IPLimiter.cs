using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Server;
using Server.Network;

namespace Server.Misc
{
	public class IPLimiter
	{
		public static bool Enabled = true;
		public static bool SocketBlock = true; // true to block at connection, false to block at login request

		public static int MaxAddresses = 4;
		
		public static IPAddress[] Exemptions = new IPAddress[]	//For hosting services where there are cases where IPs can be proxied
		{
            // Staff
            // Jimmy
            IPAddress.Parse("76.20.148.224"),
            // Saboo
            IPAddress.Parse("50.88.142.200"),
            // Gabriel
            IPAddress.Parse("174.100.155.201"),
            // Roland
            IPAddress.Parse("24.57.86.15"),
            // Mongbat
            IPAddress.Parse("108.218.144.146"),

			// SexyKimChi / KL
            IPAddress.Parse("14.42.47.67"),
            IPAddress.Parse("59.7.203.153"),
            IPAddress.Parse("112.161.232.54"),
            IPAddress.Parse("211.202.136.163"),
            IPAddress.Parse("23.25.160.78"),

            //Chineese /G/W
            IPAddress.Parse("122.225.243.42"),
            IPAddress.Parse("122.225.243.43"),
            IPAddress.Parse("122.225.243.243"),
            IPAddress.Parse("122.225.243.242"),
            IPAddress.Parse("60.191.254.114"),
            IPAddress.Parse("60.191.254.117"),
            IPAddress.Parse("60.191.254.118"),
            IPAddress.Parse("122.225.243.245"),

            // zio and roommate
            IPAddress.Parse("93.44.190.69"),

            // illfonic company ip
            IPAddress.Parse("38.88.53.130"),

            //skunkworks
            IPAddress.Parse("24.101.209.132"),

            // wazza
            IPAddress.Parse("83.93.55.154"),

            // ajax wern'gard
            IPAddress.Parse("24.40.88.64"),

            // ghandi
            IPAddress.Parse("83.89.3.156"),

            // kalanmyr
            IPAddress.Parse("74.215.189.177"),

            // LJs
            IPAddress.Parse("68.203.167.61"),

            // Afroman
            IPAddress.Parse("75.128.75.2"),

            // nuki
            IPAddress.Parse("135.23.116.158"),

            // abigor neighbors
            IPAddress.Parse("208.91.143.2"),

            // jon snow
            IPAddress.Parse("98.202.247.10"),

            IPAddress.Parse("127.0.0.1"),
		};

		public static bool IsExempt( IPAddress ip )
		{
			for ( int i = 0; i < Exemptions.Length; i++ )
			{
				if ( ip.Equals( Exemptions[i] ) )
					return true;
			}

			return false;
		}

		public static bool Verify( IPAddress ourAddress )
		{
			if ( !Enabled || IsExempt( ourAddress ) )
				return true;

			List<NetState> netStates = NetState.Instances;

			int count = 0;

			for ( int i = 0; i < netStates.Count; ++i )
			{
				NetState compState = netStates[i];

				if ( ourAddress.Equals( compState.Address ) )
				{
					++count;

					if ( count >= MaxAddresses )
						return false;
				}
			}

			return true;
		}
	}
}