var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<DbCodingTest>(o => o.UseSqlite("DataSource=CodingTest.db;Cache=Shared", b => b.MigrationsAssembly("CodingTest")));
builder.Services.AddScoped<IAtmService, AtmService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
//builder.Services.AddHostedService<SimulateParallelRequestsToCustomerAPI>();

builder.Services.AddControllers();
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
    var dbContext = services.GetRequiredService<DbCodingTest>();
    dbContext.Database.Migrate();
}

app.Run();
