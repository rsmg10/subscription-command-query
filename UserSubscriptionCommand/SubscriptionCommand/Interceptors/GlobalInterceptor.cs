using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;

namespace SubscriptionQuery.Interceptors
{
   
 
        public class ServerLoggerInterceptor : Interceptor
        {
            private readonly ILogger<ServerLoggerInterceptor> _logger;

            public ServerLoggerInterceptor(ILogger<ServerLoggerInterceptor> logger)
            {
                _logger = logger;
            }

            public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
                TRequest request,
                ServerCallContext context,
                UnaryServerMethod<TRequest, TResponse> continuation)
            {
                //LogCall<TRequest, TResponse>(MethodType.Unary, context);

                try
                {
                    return await continuation(request, context);
                }
                catch (Exception ex)
                {
                    // Note: The gRPC framework also logs exceptions thrown by handlers to .NET Core logging.
                    _logger.LogError(ex, $"Error thrown by {context.Method}.");
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.InnerException);

                    throw;
                }
            }

        } 

}
