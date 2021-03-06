﻿namespace EssentialsPlugin.ChatHandlers
{
    using System;
    using System.Linq;
    using System.Threading;
    using EssentialsPlugin.Utility;
    using Sandbox.Common.ObjectBuilders;
    using Sandbox.Engine.Multiplayer;
    using Sandbox.Game.Replication;
    using Sandbox.ModAPI;
    using SEModAPIInternal.API.Entity;
    using SEModAPIInternal.API.Entity.Sector.SectorObject;
    using VRage;
    using VRage.ModAPI;
    using VRageMath;

    public class HandleAdminMoveGridTo : ChatHandlerBase
	{
		public override string GetHelp()
		{
			return "This command allows you to move a grid to a location near another player or grid.  Usage: /admin move grid to [SOURCE_SHIP|STATION] [TARGET_USERNAME] (DISTANCE)";
		}
		public override string GetCommandText()
		{
			return "/admin move grid to";
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

        public override bool IsAdminCommand()
		{
			return true;
		}

		public override bool AllowedInConsole()
		{
			return true;
		}

		// /admin movefrom x y z x y z radius
		public override bool HandleCommand(ulong userId, string[] words)
		{
			if (words.Count() < 2)
			{
				Communication.SendPrivateInformation(userId, GetHelp());
				return true;
			}
			
			string sourceName = words[0];
			float distance = 50f;

			// TODO Allow quotes so we can do distance?
			bool parse = false;
			if (words.Count() > 2)
			{
				parse = float.TryParse(words[words.Count() - 1], out distance);
			}

			string targetName;
			if(parse)
				targetName = string.Join(" ", words.Skip(1).Take(words.Count() - 2).ToArray());
			else
				targetName = string.Join(" ", words.Skip(1).ToArray());

			Communication.SendPrivateInformation(userId, string.Format("Moving {0} to within {1}m of {2}.  This may take about 20 seconds.", sourceName, distance, targetName));

			Vector3D position;
            IMyEntity entity = Player.FindControlledEntity( targetName );
            if (entity == null)
			{
                entity = CubeGrids.Find( targetName );
                if ( entity == null )
                {
                    Communication.SendPrivateInformation( userId, string.Format( "Can not find user or grid with the name: {0}", targetName ) );
                    return true;
                }
                position = entity.GetPosition();
			}
			else
				position = entity.GetPosition();

			Vector3D startPosition = MathUtility.RandomPositionFromPoint((Vector3)position, distance);
            IMyEntity gridToMove = CubeGrids.Find( sourceName );
            if (gridToMove == null)
			{
				Communication.SendPrivateInformation(userId, string.Format("Unable to find: {0}", sourceName));
				return true;
			}
            
            Communication.MoveMessage( 0, "normal", startPosition.X, startPosition.Y, startPosition.Z, gridToMove.EntityId );

            //Wrapper.GameAction( ( ) =>
             //{
             //    gridToMove.GetTopMostParent( ).SetPosition( startPosition );
             //    Log.Info( string.Format( "Moving '{0}' from {1} to {2}", gridToMove.DisplayName, gridToMove.GetPosition( ), startPosition ) );
                // MyMultiplayer.ReplicateImmediatelly( MyExternalReplicable.FindByObject( gridToMove.GetTopMostParent() ) );
            //} );
            /*
                        Thread.Sleep(5000);

                        Wrapper.GameAction(() =>
                        {
                            MyAPIGateway.Entities.RemoveFromClosedEntities(entity);
                            Log.Info(string.Format("Removing '{0}' for move", entity.DisplayName));
                        });

                        Thread.Sleep(10000);

                        Wrapper.GameAction(() =>
                        {
                            gridBuilder.PositionAndOrientation = new MyPositionAndOrientation(startPosition, gridBuilder.PositionAndOrientation.Value.Forward, gridBuilder.PositionAndOrientation.Value.Up);
                            Log.Info(string.Format("Adding '{0}' for move", gridBuilder.DisplayName));
                            SectorObjectManager.Instance.AddEntity(new CubeGridEntity(gridBuilder));
                        });*/

            Communication.SendPrivateInformation(userId, string.Format("Moved {0} to within {1}m of {2}", sourceName, (int)Math.Round(Vector3D.Distance(startPosition, position)), targetName));
			return true;
		}
	}
}
