using FluentValidation;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Behaviours
{
    public class ValidationBehavior<TTRequest, TTResponse> : IPipelineBehavior<TTRequest, TTResponse>
        where TTRequest : IRequest<TTResponse>
    {
        private readonly IEnumerable<IValidator<TTRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TTRequest>> validators)
        {
            _validators = validators;
        }
        public async Task<TTResponse> Handle(TTRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TTResponse> next)
        {
            if (_validators.Any())
            {
                var context = new FluentValidation.ValidationContext<TTRequest>(request);
                var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
                var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();
            }
            return await next();
        }
    }
}
