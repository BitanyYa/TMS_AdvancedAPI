using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TmsApi.Infrastructure.Persistence;

namespace TmsApi.Api.Controllers.V2;

[ApiController]
[Route("api/v{version:apiVersion}/enrollments")]
[ApiVersion("2.0")]
public class EnrollmentsController(TmsDbContext context) : ControllerBase
{
    [HttpGet("{enrollmentId:int}/schedule")]
    public async Task<IActionResult> GetSchedule(
        int enrollmentId,
        CancellationToken ct = default)
    {
        var enrollment = await context.Enrollments
            .AsNoTracking()
            .Include(e => e.Course)
            .SingleOrDefaultAsync(e => e.Id == enrollmentId, ct);

        if (enrollment is null)
        {
            return NotFound(new { message = "Enrollment not found." });
        }

        return Ok(new
        {
            enrollmentId = enrollment.Id,
            studentId = enrollment.StudentId,
            course = new
            {
                enrollment.Course.Id,
                enrollment.Course.Code,
                enrollment.Course.Title
            },
            schedule = Array.Empty<object>()
        });
    }

    [HttpPost]
    public async Task<IActionResult> CreateEnrollment(
        [FromBody] CreateEnrollmentRequest request,
        CancellationToken ct = default)
    {
        var course = await context.Courses
            .Include(c => c.Enrollments)
            .SingleOrDefaultAsync(c => c.Code == request.CourseCode, ct);

        if (course is null)
        {
            return NotFound(new { message = "Course not found." });
        }

        if (course.Enrollments.Count >= course.MaxCapacity)
        {
            return Conflict(new { message = "Course capacity has been reached." });
        }

        if (course.Enrollments.Any(e => e.StudentId == request.StudentId))
        {
            return Conflict(new { message = "Student is already enrolled in this course." });
        }

        var enrollment = new TmsApi.Domain.Entities.Enrollment
        {
            StudentId = request.StudentId,
            CourseId = course.Id,
            EnrolledAt = DateTimeOffset.UtcNow
        };

        context.Enrollments.Add(enrollment);
        await context.SaveChangesAsync(ct);

        return Created($"/api/v2/enrollments/{enrollment.Id}", new
        {
            enrollment.Id,
            enrollment.StudentId,
            courseCode = course.Code
        });
    }
}

public sealed record CreateEnrollmentRequest(int StudentId, string CourseCode);