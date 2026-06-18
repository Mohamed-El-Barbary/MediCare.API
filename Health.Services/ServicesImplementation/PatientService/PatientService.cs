using AutoMapper;
using Health.Domain.Contracts;
using Health.Domain.Entities.AppointmentModule;
using Health.Domain.Entities.ConsultationModule;
using Health.Domain.Entities.DoctorModule;
using Health.Domain.Entities.PatientModule;
using Health.Services.Abstraction.PatientServiceAbstraction;
using Health.Services.Specifications.AppointmentSpecification;
using Health.Services.Specifications.ConsultationSpecification;
using Health.Services.Specifications.DoctorSceduleSpecification;
using Health.Services.Specifications.PatientSpecification;
using Health.Shared.CommonResponses;
using Health.Shared.DTOs.ConsultationDTOs;
using Health.Shared.DTOs.DoctorDTOs;
using Health.Shared.DTOs.PatientDashboardDTOs;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Health.Services.ServicesImplementation.PatientService
{
    public class PatientService : IPatientService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PatientService(IUnitOfWork unitOfWork , IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<PatientDashboardResponce>> GetPatientDashboard(string patientId)
        {
            var profileResult = await GetPatientProfileAsync(patientId);

            if (profileResult.IsFailure)
                return profileResult.Errors.First();

            var profile = profileResult.Value;
            var profileId = profile.Id;
            var patientProfileResponce = _mapper.Map<PatientProfileResponce>(profile);

            var Appointments = await GetUpCommingAppointment(profileId);
            var UpcommingAppointments = Appointments.Value;
            var UpcommingMapping = _mapper.Map<IEnumerable<UpcommingPatientAppointments>>(UpcommingAppointments);
            var consultationsWithPrescriptions = await GetPatientRecentPrescription(profileId);
            var recentPrescriptions = consultationsWithPrescriptions.Value;
            var prescriptions = recentPrescriptions.SelectMany(c => c.Prescriptions);
            var PrescriptionsResponce = _mapper.Map<IEnumerable<PrescriptionDTO>>(prescriptions);
            var PatientDashboard = BuildDashboard
            (
                patientProfileResponce,
                UpcommingMapping,
                PrescriptionsResponce
            );

            return PatientDashboard;
        }

        #region HelperMethods
        private async Task<Result<PatientProfile>> GetPatientProfileAsync(string patientId)
        {
            var spec = new PatientByIdWithoutIncludes(patientId);

            var Patient = await _unitOfWork.GetRepository<PatientProfile, int>().GetByIdAsync(spec);

            if (Patient is null)
                return Result<PatientProfile>.Fail(
                    Error.NotFound("Doctor.NotFound", $"Doctor with user id:{patientId} not found")
                );

            return Result<PatientProfile>.Ok(Patient);
        }
        private async Task<Result<IEnumerable<Appointment>>> GetUpCommingAppointment(int patientId)
        {
            var spec = new ConfirmedAndPendingAppointmentSpec(patientId);
            var appointments = await _unitOfWork.GetRepository<Appointment , int>().GetAllAsync(spec);
            if (appointments is null)
            {
                return Result<IEnumerable<Appointment>>.Fail(Error.NotFound("Appointments.Notfound" , "Appointment Is Not Found"));
            }
            if(!appointments.Any())
            {
                return Result<IEnumerable<Appointment>>.Ok(Enumerable.Empty<Appointment>());
            }

            return Result<IEnumerable<Appointment>>.Ok(appointments);
        }
        private async Task<Result<IEnumerable<Consultation>>> GetPatientRecentPrescription(int patientId)
        {
            var spec = new PatientRecentConsultationsSpec(patientId);
            var consultations = await _unitOfWork.GetRepository<Consultation , int>().GetAllAsync(spec);
            if (consultations is null)
                return Result<IEnumerable<Consultation>>.Fail(Error.NotFound("prescriptions Not Found"));

            if (!consultations.Any())
                return Result<IEnumerable<Consultation>>.Ok(Enumerable.Empty<Consultation>());

            return Result<IEnumerable<Consultation>>.Ok(consultations);
        }
        private PatientDashboardResponce BuildDashboard
        (
            PatientProfileResponce patientProfileResponce,
            IEnumerable<UpcommingPatientAppointments>? upcommingPatientAppointments,
            IEnumerable<PrescriptionDTO> RecentPrescriptions
        )
        {
            return new PatientDashboardResponce(
                    patientProfileResponce,
                    new UpcomingAppointments(upcommingPatientAppointments!),
                    RecentPrescriptions
            );
        }

        #endregion
    }
}
