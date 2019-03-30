#region References
using System;

using Server.Accounting;
using Server.Network;
using Server.Services.TownCryer;
#endregion

namespace Server
{
	public class CurrentExpansion
	{
		public static readonly Expansion Expansion = Config.GetEnum("Expansion.CurrentExpansion", Expansion.EJ);

		[CallPriority(Int32.MinValue)]
		public static void Configure()
		{
			Core.Expansion = Expansion;

			AccountGold.Enabled = Core.TOL;
			AccountGold.ConvertOnBank = true;
			AccountGold.ConvertOnTrade = false;
			VirtualCheck.UseEditGump = true;
            
			TownCryerSystem.Enabled = Core.TOL;

			ObjectPropertyList.Enabled = true;

            Mobile.InsuranceEnabled = Core.AOS && !Siege.SiegeShard;
			Mobile.VisibleDamageType = VisibleDamageType.Everyone;
			Mobile.GuildClickMessage = !Core.AOS;
			Mobile.AsciiClickMessage = !Core.AOS;
            Mobile.ActionDelay = 0;
            PacketHandlers.SingleClickProps = true;

            if (!Core.AOS)
			{
				return;
			}

			AOS.DisableStatInfluences();

			if (ObjectPropertyList.Enabled)
			{
				PacketHandlers.SingleClickProps = true; // single click for everything is overriden to check object property list
			}

            Mobile.ActionDelay = 0;// Core.TOL ? 500 : Core.AOS ? 1000 : 500;
			Mobile.AOSStatusHandler = AOS.GetStatus;
		}
	}
}
