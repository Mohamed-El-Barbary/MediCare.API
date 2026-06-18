using Health.Services.Abstraction.ConsultationModule;
using Health.Shared.CommonResponses;
using Health.Shared.DTOs.ConsultationSessionDTOs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Health.Presentation.Hubs
{
    public class VideoCallHub : Hub
    {
        private readonly IConsultationSessionService _sessionService;

        public VideoCallHub(IConsultationSessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public async Task RegisterConnection()
        {
            var userId = await GetProfileIdAsync();
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
        }

        public async Task CallPatient(int consultationId)
        {
            var consultation = await _sessionService.GetSessionAsync(consultationId);
            if (consultation.IsFailure) return;

            var patientId = consultation.Value!.PatientId;

            await Clients.Group($"user-{patientId}").SendAsync("IncomingCall", new
            {
                consultationId,
                doctorName = consultation.Value.DoctorName
            });
        }

        public async Task JoinSession(int consultationId)
        {
            var userId = await GetProfileIdAsync();

            var canJoin = await _sessionService.CanJoinAsync(consultationId, userId);

            if (canJoin.IsFailure)
            {
                await Clients.Caller.SendAsync("Error", canJoin.Errors.First());
                return;
            }

            var session = await _sessionService.GetSessionAsync(consultationId);

            if (session.IsSuccess && session.Value!.Status!.ToString() == "Scheduled")
                await _sessionService.StartSessionAsync(consultationId);

            await _sessionService.AddConnectionAsync(consultationId, userId, Context.ConnectionId);

            await Groups.AddToGroupAsync(Context.ConnectionId, consultationId.ToString());
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");

            var joinedSession = await _sessionService.JoinSessionAsync(consultationId, userId);

            var participants = await _sessionService.GetParticipantsAsync(consultationId);

            await Clients.Caller.SendAsync("SessionJoined", joinedSession, participants);

            await Clients.OthersInGroup(consultationId.ToString()).SendAsync("ParticipantJoined", userId);

        }

        public async Task LeaveSession(int consultationId)
        {
            var userId = await GetProfileIdAsync();

            await _sessionService.LeaveSessionAsync(consultationId, userId);
            await _sessionService.RemoveConnectionAsync(Context.ConnectionId);

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, consultationId.ToString());

            await Clients.OthersInGroup(consultationId.ToString()).SendAsync("ParticipantLeft", userId);
        }

        public async Task EndSession(int consultationId)
        {
            var userId = await GetProfileIdAsync();

            var session = await _sessionService.GetSessionAsync(consultationId);
            if (session is null || session.IsFailure)
            {
                await Clients.Caller.SendAsync("Error", "Session not found");
                return;
            }

            if (session.Value!.DoctorId! != userId)
            {
                await Clients.Caller.SendAsync("Error", "Only the doctor can end the session");
                return;
            }

            var endedSession = await _sessionService.EndSessionAsync(consultationId);

            if (endedSession.IsSuccess)
                await Clients.Group(consultationId.ToString()).SendAsync("SessionEnded", endedSession.Value);
            else
                await Clients.Caller.SendAsync("Error", "Failed to end session");
        }

        public async Task SendMediaState(int consultationId, bool mic, bool camera)
        {
            var userId = await GetProfileIdAsync();
            await Clients.OthersInGroup(consultationId.ToString())
                         .SendAsync("ReceiveMediaState", userId, mic, camera);
        }

        // ── WebRTC SIGNALING ───
        public async Task SendOffer(int consultationId, SdpSignalDTO offer)
        {
            var userId = await GetProfileIdAsync();
            await Clients.OthersInGroup(consultationId.ToString()).SendAsync("ReceiveOffer", userId, offer);
        }

        public async Task SendAnswer(int consultationId, SdpSignalDTO answer)
        {
            var userId = await GetProfileIdAsync();
            await Clients.OthersInGroup(consultationId.ToString()).SendAsync("ReceiveAnswer", userId, answer);
        }

        public async Task SendIceCandidate(int consultationId, IceCandidateDTO candidate)
        {
            var userId = await GetProfileIdAsync();
            await Clients.OthersInGroup(consultationId.ToString()).SendAsync("ReceiveIceCandidate", userId, candidate);
        }

        private string GetUserId()
        {
            if (Context.User == null)
                throw new HubException("Context.User is null");

            if (!Context.User.Identity!.IsAuthenticated)
                throw new HubException("User not authenticated");

            var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                throw new HubException("Claim not found");

            return userId;
        }

        private string GetRole()
        {
            return Context.User?.FindFirst(ClaimTypes.Role)?.Value
                ?? throw new HubException("Unauthorized");
        }

        private async Task<int> GetProfileIdAsync()
        {
            var userId = GetUserId();
            var role = GetRole();

            var result = await _sessionService.GetProfileIdAsync(role, userId);

            if (!result.IsSuccess)
                throw new HubException(result.Errors.First().Description);

            return result.Value;
        }

    }
}
