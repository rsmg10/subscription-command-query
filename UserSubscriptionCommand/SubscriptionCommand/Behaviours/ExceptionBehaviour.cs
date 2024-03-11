using FluentValidation;
using MediatR;

namespace SubscriptionCommand.Behaviours;

 
    public class ExceptionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    { 
        public ExceptionBehaviour( )
        { 
        }
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            try
            {

                return await next();
            }
            catch (Exception e)
            {
            await Console.Out.WriteLineAsync(e.ToString());
            Console.WriteLine(e.InnerException);

                throw;
            }

        }
 
    }
