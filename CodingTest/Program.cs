var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DbCoding>(o => o.UseSqlite("DataSource=CodingTest.db;Cache=Shared", b => b.MigrationsAssembly("CodingTest")));
builder.Services.AddMemoryCache();
builder.Services.AddScoped<IInternalNotificationService, InternalNotificationService>();
builder.Services.AddScoped<IAtmService, AtmService>();
builder.Services.AddValidatorsFromAssemblyContaining<AddCustomersValidator>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddHostedService<SimulateParallelRequestsToCustomerApi>();

builder.Services.AddControllers().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionHandleMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<DbCoding>();
    dbContext.Database.Migrate();
}

app.Run();
