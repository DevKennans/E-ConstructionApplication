using MediatR;

namespace EConstructionApp.Application.Features.Queries.Employees.GetEmployeeById
{
    public class GetEmployeeByIdQueryRequest : IRequest<GetEmployeeByIdQueryResponse>
    {
        public Guid EmployeeId { get; set; }
    }
}
