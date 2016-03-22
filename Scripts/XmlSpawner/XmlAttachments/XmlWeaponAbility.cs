using System;
using Server;
using Server.Items;
using Server.Network;
using Server.Mobiles;
using System.Reflection;

namespace Server.Engines.XmlSpawner2
{
	public class XmlWeaponAbility : XmlAttachment
	{
		[CommandProperty( AccessLevel.GameMaster )]
		public string Ability 
		{ 
			get 
			{			
				return null;				
			} 
			set 
			{
			} 
		}


		// These are the various ways in which the message attachment can be constructed.  
		// These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword.
		// Other overloads could be defined to handle other types of arguments
       
		// a serial constructor is REQUIRED
		public XmlWeaponAbility(ASerial serial) : base(serial)
		{
		}

		[Attachable]
		public XmlWeaponAbility(string weaponability)
		{
			Ability = weaponability;
		}

		[Attachable]
		public XmlWeaponAbility(string name, string weaponability)
		{
			Name = name;
			Ability = weaponability;
		}

		[Attachable]
		public XmlWeaponAbility(string name, string weaponability, double expiresin)
		{
			Name = name;
			Ability = weaponability;
			Expiration = TimeSpan.FromMinutes(expiresin);

		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize(writer);

			writer.Write( (int) 0 );
			// version 0
			writer.Write(Ability);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
			// version 0
			Ability = reader.ReadString();
		}

		public override string OnIdentify(Mobile from)
		{
			if(from == null || from.AccessLevel == AccessLevel.Player) return null;

			if(Expiration > TimeSpan.Zero)
			{
				return String.Format("{2}: Weapon ability {0} expires in {1} mins", Ability, Expiration.TotalMinutes, Name);
			} 
			else
			{
				return String.Format("{1}: Weapon ability {0}", Ability, Name);
			}
		}
	}
}
