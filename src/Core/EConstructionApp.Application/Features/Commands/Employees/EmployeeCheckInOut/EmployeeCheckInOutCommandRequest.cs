using MediatR;

namespace EConstructionApp.Application.Features.Commands.Employees.EmployeeCheckInOut
{
    public class EmployeeCheckInOutCommandRequest : IRequest<EmployeeCheckInOutCommandResponse>
    {
        public Guid EmployeeId { get; set; } /*
                                              * Type of this employee is AppUser, but in according service it will be checked for another kind of users like admin & moderator.
                                              * EmployeeId from AppUser must be bind with Employee data!
                                              */
        public DateTime ScanTime { get; set; }
    }
}
