using Health.Services.Abstraction.ConsultationModule;
using Health.Shared;
using Health.Shared.CommonResponses;
using Health.Shared.DTOs.ConsultationDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Text;

namespace Health.Presentation.Controllers
{
    public class ConsultationController : ApiBaseController
    {
        private readonly IConsultationService _consultationService;

        public ConsultationController(IConsultationService consultationService)
        {
            _consultationService = consultationService;
        }

        [HttpPost]
        public async Task<ActionResult<ConsultationDTO>> CreateConsultation(CreateConsultationDTO request)
        {
            var result = await _consultationService.CreateAsync(request);
            return HandleResult(result);
        }

        //[Authorize]
        [HttpGet("{cid}")]
        public async Task<ActionResult<ConsultationDTO>> GetConsultation(int cid)
        {
            var result = await _consultationService.GetByIdAsync(cid);
            return HandleResult(result)!;
        }

        [Authorize]
        [HttpGet("doctor")]
        public async Task<ActionResult<PaginatedResult<ConsultationSummaryDTO>>> GetByDoctorId([FromQuery]ConsultationSpecParams specParams)
        {
            var doctorId = GetUserId();
            var result = await _consultationService.GetByDoctorIdAsync(doctorId, specParams);
            return HandleResult(result);
        }

        [Authorize]
        [HttpGet("patient")]
        public async Task<ActionResult<PaginatedResult<ConsultationSummaryDTO>>> GetByPatientId([FromQuery] ConsultationSpecParams specParams)
        {
            var patientId = GetUserId();
            var result = await _consultationService.GetByPatientIdAsync(patientId, specParams);
            return HandleResult(result);
        }

        [HttpPut("{cId}")]
        public async Task<ActionResult<ConsultationDTO>> UpdateMedicalData(int cId, UpdateMedicalDataDTO request)
        {
            var result = await _consultationService.UpdateMedicalDataAsync(cId, request);
            return HandleResult(result);
        }

        [HttpPost("{cId}/prescriptions")]
        public async Task<ActionResult<ConsultationDTO>> AddPrescription(int cId, CreatePrescriptionDTO request)
        {
            var result = await _consultationService.AddPrescriptionAsync(cId, request);
            return HandleResult(result);
        }

        [HttpPut("{cId}/prescriptions/{pId}")]
        public async Task<ActionResult<ConsultationDTO>> UpdatePrescription(int cId, int pId, UpdatePrescriptionDTO request)
        {
            var result = await _consultationService.UpdatePrescriptionAsync(cId, pId, request);
            return HandleResult(result);
        }

        [HttpDelete("{cId}/prescriptions/{pId}")]
        public async Task<ActionResult<bool>> DeletePrescription(int cId, int pId)
        {
            var result = await _consultationService.DeletePrescriptionAsync(cId, pId);

            return HandleResult(result, "Prescription Deleted Successfully");
        }

        [HttpPost("{cId}/prescriptions/{pId}/items")]
        public async Task<ActionResult<ConsultationDTO>> AddPrescriptionItems(int cId, int pId, CreatePrescriptionItemDTO request)
        {
            var result = await _consultationService.AddPrescriptionItemAsync(cId, pId, request);
            return HandleResult(result);
        }

        [HttpPut("{cId}/prescriptions/{pId}/items/{id}")]
        public async Task<ActionResult<ConsultationDTO>> UpdatePrescriptionItems(int cId, int pId, int id, UpdatePrescriptionItemDTO request)
        {
            var result = await _consultationService.UpdatePrescriptionItemAsync(cId, pId, id, request);
            return HandleResult(result);
        }

        [HttpDelete("{cId}/prescriptions/{pId}/items/{id}")]
        public async Task<ActionResult<ConsultationDTO>> DeletePrescriptionItems(int cId, int pId, int id)
        {
            var result = await _consultationService.DeletePrescriptionItemAsync(cId, pId, id);
            return HandleResult(result, "PrescriptionItem Deleted Successfully");
        }
    }
}
