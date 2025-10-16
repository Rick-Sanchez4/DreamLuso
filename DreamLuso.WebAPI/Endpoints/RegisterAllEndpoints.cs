namespace DreamLuso.WebAPI.Endpoints;

public static class RegisterAllEndpoints
{
    public static void RegisterEndpoints(this IEndpointRouteBuilder routes)
    {
        // Account management
        routes.RegisterAccountEndpoints();
        routes.RegisterUserEndpoints();
        
        // Core entities
        routes.RegisterPropertyEndpoints();
        routes.RegisterClientEndpoints();
        routes.RegisterRealEstateAgentEndpoints();
        
        // Property visits and contracts
        routes.RegisterPropertyVisitEndpoints();
        routes.RegisterContractEndpoints();
        
        // Dashboard
        routes.RegisterDashboardEndpoints();
        
        // New Features
        routes.RegisterNotificationEndpoints();
        routes.RegisterPropertyProposalEndpoints();
        routes.RegisterCommentEndpoints();
        
        // Image Upload
        routes.RegisterImageUploadEndpoints();
    }
}

