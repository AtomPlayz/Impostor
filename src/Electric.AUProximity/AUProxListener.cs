using Electric.AUProximity.Hub;
using Electric.AUProximity.Models;
using Impostor.Api.Events;
using Impostor.Api.Events.Meeting;
using Impostor.Api.Events.Player;
using Microsoft.AspNetCore.SignalR;

namespace Electric.AUProximity
{
    public class PlayerMoveListener : IEventListener
    { 
        private readonly IHubContext<ProximityHub, IProximityHub> _proximityHub;

        public PlayerMoveListener(IHubContext<ProximityHub, IProximityHub> proximityHub)
        {
            this._proximityHub = proximityHub;
        }

        [EventListener]
        public void onChat(IPlayerChatEvent e)
        {
            if (e.Message == "!maingroup")
            {
                _proximityHub.Clients.Group(e.Game.Code).GameStarted();
            } else if (e.Message == "!spectatorgroup")
            {
                _proximityHub.Clients.Group(e.Game.Code).GameEnd();
            } else if (e.Message == "!mutedgroup")
            {
                _proximityHub.Clients.Group(e.Game.Code).CommsSabotage(false);
            }
        }

        [EventListener]
        public void GameStart(IGameStartedEvent e)
        {
            _proximityHub.Clients.Group(e.Game.Code).GameStarted();
            _proximityHub.Clients.Group(e.Game.Code).MapChange(e.Game.Options.Map);
        }
        
        
        [EventListener]
        public void OnPlayerMove(IPlayerMovementEvent e)
        {
            _proximityHub.Clients.Group(e.Game.Code).PlayerMove(
                e.PlayerControl.PlayerInfo.PlayerName,
                new Pose(e.TargetPosition.X, e.TargetPosition.Y));
        }
        [EventListener]
        public void MeetingCalled(IMeetingStartedEvent e)
        {
            _proximityHub.Clients.Group(e.Game.Code).MeetingCalled();
        }
        [EventListener]
        public void PlayerMurdered(IPlayerMurderEvent e)
        {
            _proximityHub.Clients.Group(e.Game.Code).PlayerExiled(e.Victim.PlayerInfo.PlayerName);
        }

        [EventListener]
        public void PlayerExiled(IPlayerExileEvent e)
        {
            // Called when MeetingHud RPC VotingComplete has a player to be voted out.
            _proximityHub.Clients.Group(e.Game.Code).PlayerExiled(e.PlayerControl.PlayerInfo.PlayerName);
        }
        [EventListener]
        public void GameEnd(IGameEndedEvent e)
        {
            _proximityHub.Clients.Group(e.Game.Code).GameEnd();
        }
    }
}
