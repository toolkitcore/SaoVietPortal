﻿using System.Net.Mime;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Portal.Api.Models;
using Portal.Application.Cache;
using Portal.Application.Services;
using Portal.Application.Transaction;

namespace Portal.Api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class StudentController : ControllerBase
{
    private readonly StudentService _studentService;
    private readonly TransactionService _transactionService;
    private readonly ILogger<StudentController> _logger;
    private readonly IMapper _mapper;
    private readonly IValidator<Student> _validator;
    private readonly IRedisCacheService _redisCacheService;

    public StudentController(
        StudentService studentService, 
        TransactionService transactionService, 
        ILogger<StudentController> logger, 
        IMapper mapper,
        IValidator<Student> validator,
        IRedisCacheService redisCacheService
    )
    {
        _studentService = studentService;
        _transactionService = transactionService;
        _logger = logger;
        _mapper = mapper;
        _validator = validator;
        _redisCacheService = redisCacheService;
    }

    /// <summary>
    /// Lấy danh sách học viên
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /api/v1/Student
    /// </remarks>
    /// <respone code="200">Trả về danh sách học viên</respone>
    /// <respone code="401">Không có quyền</respone>
    /// <respone code="404">Không tìm thấy học viên</respone>
    /// <respone code="429">Quá nhiều yêu cầu</respone>
    /// <respone code="500">Lỗi server</respone>
    [HttpGet]
    [Authorize]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(200, Type = typeof(List<Student>))]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    [ProducesResponseType(429)]
    [ProducesResponseType(500)]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
    [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
    public ActionResult GetStudents()
    {
        try
        {
            return (_redisCacheService.GetOrSet("StudentData", 
                    () => _studentService.GetAllStudents().ToList())) switch
            {
                { Count: > 0 } students=> Ok(students),
                _ => NotFound()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting students");
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Tìm thông tin học viên theo mã học viên
    /// </summary>
    /// <returns></returns>
    /// <param name="id">Mã học viên</param>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /api/v1/Student/{id}
    /// </remarks>
    /// <respone code="200">Trả về thông tin học viên</respone>
    /// <respone code="401">Không có quyền</respone>
    /// <respone code="404">Không tìm thấy học viên</respone>
    /// <respone code="429">Quá nhiều yêu cầu</respone>
    /// <respone code="500">Lỗi server</respone>
    [HttpGet("{id}")]
    [Authorize]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(200, Type = typeof(Student))]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    [ProducesResponseType(429)]
    [ProducesResponseType(500)]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
    public ActionResult GetStudentById(
        [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)] 
        [FromRoute] string id)
    {
        try
        {
            return _redisCacheService
                    .GetOrSet("StudentData", () => _studentService.GetAllStudents().ToList())
                    .FirstOrDefault(s => s.studentId == id) switch
            {
                { } student => Ok(student),
                _ => NotFound()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting student by id");
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Thêm học viên
    /// </summary>
    /// <param name="student">Đối tượng học viện</param>
    /// <returns></returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/v1/Student
    ///     {
    ///         "fullname": "string",
    ///         "gender": true,
    ///         "address": "string",
    ///         "dob": "string",
    ///         "pod": "string",
    ///         "occupation": "string",
    ///         "socialNetwork": "string"
    ///     }
    /// </remarks>
    /// <respone code="200">Thêm thành công</respone>
    /// <respone code="400">Dữ liệu không hợp lệ</respone>
    /// <respone code="401">Không có quyền</respone>
    /// <respone code="409">Mã học viên đã tồn tại</respone>
    /// <respone code="429">Quá nhiều yêu cầu</respone>
    /// <respone code="500">Lỗi server</respone>
    [HttpPost]
    [Authorize]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(409)]
    [ProducesResponseType(429)]
    [ProducesResponseType(500)]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
    public ActionResult AddStudent(
        [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
        [FromBody] Student student)
    {
        try
        {
            if (!_validator.Validate(student).IsValid)
                return BadRequest();

            if (student.studentId != null && _studentService.GetStudentById(student.studentId) != null) 
                return Conflict();

            _transactionService.ExecuteTransaction((() => { _studentService.AddStudent(_mapper.Map<Domain.Entities.Student>(student)); }));

            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while adding student");
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Xoá học viên
    /// </summary>
    /// <param name="id">Mã học viên</param>
    /// <returns></returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     DELETE /api/v1/Student/{id}
    /// </remarks>
    /// <respone code="200">Xoá thành công</respone>
    /// <respone code="400">Dữ liệu không hợp lệ</respone>
    /// <respone code="401">Không có quyền</respone>
    /// <respone code="404">Không tìm thấy học viên</respone>
    /// <respone code="429">Quá nhiều yêu cầu</respone>
    /// <respone code="500">Lỗi server</respone>
    [HttpDelete("{id}")]
    [Authorize]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    [ProducesResponseType(429)]
    [ProducesResponseType(500)]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
    public ActionResult DeleteStudent(
        [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
        [FromRoute] string id)
    {
        try
        {
            if(_studentService.GetStudentById(id) == null)
                return NotFound();

            _transactionService.ExecuteTransaction((() => { _studentService.DeleteStudent(id); }));

            // update cache
            if (_redisCacheService.GetOrSet("StudentData", () => _studentService.GetAllStudents().ToList()) is
                { Count: > 0 } students)
            {
                students.RemoveAll(s => s.studentId == id);
            }


            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while deleting student");
            return StatusCode(500);
        }
    }
}