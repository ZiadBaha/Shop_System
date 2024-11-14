using Account.Apis.Errors;
using Account.Apis.Helpers;
using Account.Core.Services;
using Account.Core.Services.Programe;
using Account.Reposatory.Reposatories.Programe;
using Account.services;
using Microsoft.AspNetCore.Mvc;

namespace Account.Apis.Extentions
{
    public static class ApplictionServiceExtention
    {
        public static IServiceCollection AddAplictionService(this IServiceCollection service)
        {
            service.AddAutoMapper(typeof(MappingProfile));


            // Configure API behavior options
            service.Configure<ApiBehaviorOptions>(Options =>
            {
                // Customize the behavior for handling invalid model state
                Options.InvalidModelStateResponseFactory = (actionContext) =>
                {
                    // Extract validation errors from the ModelState
                    var Errors = actionContext.ModelState
                        .Where(P => P.Value.Errors.Count() > 0)
                        .SelectMany(P => P.Value.Errors)
                        .Select(E => E.ErrorMessage)
                        .ToArray();

                    // Create a response object with validation errors
                    var ValidationErrorResponse = new ApiValidationErrorResponse()
                    {
                        Errors = Errors
                    };

                    // Return a BadRequestObjectResult with the validation error response
                    return new BadRequestObjectResult(ValidationErrorResponse);
                };
            });


            service.AddScoped(typeof(IGenericRepository<>),typeof(GenericRepository<>));
            service.AddScoped<IFileService,FileService>();
            service.AddScoped<IUserRepository,UserRepository>();

            //  service.AddScoped<IBarberService, BarberService>();

            service.AddScoped<ICategoryRepository, CategoryService>();
            service.AddScoped<ICustomerRepository, CustomerService>();
            service.AddScoped<IExpenseRepository, ExpenseService>();
            service.AddScoped<IMerchantRepository, MerchantService>();
            service.AddScoped<IOrderRepository, OrderService>();
            service.AddScoped<IPaymentRepository, PaymentService>();
            service.AddScoped<IProductRepository, ProductService>();
            service.AddScoped<IPurchaseRepository, PurchaseService>();




            service.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    policy =>
                    {
                        policy.WithOrigins("*").AllowAnyMethod().AllowAnyHeader(); ;
                    });
            });






            //Add here anny otehrt injection related to program....
            return service;
        }
    }
}
