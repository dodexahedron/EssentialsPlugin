﻿namespace EssentialsPlugin.ChatHandlers.Waypoints
{
	using EssentialsPlugin.Utility;

	public class HandleWaypointRefresh : ChatHandlerBase
	{
		public override string GetHelp()
		{
			return "Refreshes Waypoints in case your can't see them.  Usage: /waypoint refresh";
        }

        public override Communication.ServerDialogItem GetHelpDialog( )
        {
            Communication.ServerDialogItem DialogItem = new Communication.ServerDialogItem( );
            DialogItem.title = "Help";
            DialogItem.header = "";
            DialogItem.content = GetHelp( );
            DialogItem.buttonText = "close";
            return DialogItem;
        }


        public override string GetCommandText()
		{
			return "/waypoint refresh";
		}

		public override string[] GetMultipleCommandText()
		{
			return new string[] { "/waypoint refresh", "/wp refresh" };
		}

		public override bool IsAdminCommand()
		{
			return false;
		}

		public override bool AllowedInConsole()
		{
			return false;
		}

		public override bool IsClientOnly()
		{
			return true;
		}

		public override bool HandleCommand(ulong userId, string[] words)
		{
			if (!PluginSettings.Instance.WaypointsEnabled)
				return false;

			Waypoints.SendClientWaypoints(userId);

			Communication.SendPrivateInformation(userId, string.Format("Refreshed all waypoints"));
			return true;
		}

	}
}
