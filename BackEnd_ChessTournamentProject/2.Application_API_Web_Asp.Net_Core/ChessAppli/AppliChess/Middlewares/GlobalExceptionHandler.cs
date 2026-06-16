using Microsoft.AspNetCore.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace API.Middlewares
{
	public class GlobalExceptionHandler : IExceptionHandler
	{
		//TryHandleAsync attrape l'exception
		public async ValueTask<bool> TryHandleAsync(
								HttpContext httpContext,
								Exception exception,
								CancellationToken cancellationToken)
		{
			//Choisit le bon code HTTP(400, 404, 500...)
			httpContext.Response.StatusCode = exception switch
			{
				ArgumentException => StatusCodes.Status400BadRequest,
				KeyNotFoundException => StatusCodes.Status404NotFound,
				InvalidOperationException => StatusCodes.Status409Conflict,
				_ => StatusCodes.Status500InternalServerError
			};

			//Retourne un JSON propre au client
			await httpContext.Response.WriteAsJsonAsync(new
			{
				StatusCode = httpContext.Response.StatusCode,
				Message = exception.Message
			}, cancellationToken);

			//return true → erreur gérée !
			return true;
		}
	}

	//BLL lance une exception
    //        ↓
	//TryHandleAsync l'attrape
	//        ↓
	//Choisit le bon code HTTP(400, 404, 500...)
	//        ↓
	//Retourne un JSON propre au client
	//        ↓
	//return true → erreur gérée !

}
