namespace DreamLuso.Application.Common.Responses;

public sealed record Error(string Code, string Description)
{
    // General Errors
    public static readonly Error None = new(string.Empty, string.Empty);
    public static readonly Error NotImplemented = new("NotImplemented", "Método não implementado");
    public static readonly Error EmptyDatabase = new("EmptyDatabase", "Nenhum dado encontrado");
    public static readonly Error InvalidInput = new("InvalidInput", "Os dados fornecidos são inválidos");
    public static readonly Error UnauthorizedAccess = new("UnauthorizedAccess", "Você não tem autorização para realizar esta ação");
    public static readonly Error OperationFailed = new("OperationFailed", "A operação falhou");
    public static readonly Error Timeout = new("Timeout", "A operação excedeu o tempo limite");
    public static readonly Error ServiceUnavailable = new("ServiceUnavailable", "O serviço está temporariamente indisponível");
    public static readonly Error DatabaseError = new("DatabaseError", "Ocorreu um erro ao aceder à base de dados");
    public static readonly Error ConcurrencyConflict = new("ConcurrencyConflict", "Ocorreu um conflito de concorrência");
    public static readonly Error ResourceConflict = new("ResourceConflict", "Ocorreu um conflito de recursos");
    public static readonly Error InsufficientPermissions = new("InsufficientPermissions", "Permissões insuficientes para realizar esta ação");
    public static readonly Error NotFound = new("NotFound", "O recurso solicitado não foi encontrado");
    public static readonly Error BadRequest = new("BadRequest", "O pedido é inválido ou não pode ser processado");
    public static readonly Error Conflict = new("Conflict", "O pedido não pôde ser concluído devido a um conflito");
    public static readonly Error ValidationFailed = new("ValidationFailed", "A validação dos dados falhou");
    public static readonly Error ServerError = new("ServerError", "Ocorreu um erro inesperado no servidor");

    // User Errors
    public static readonly Error UserNotFound = new("UserNotFound", "Utilizador não encontrado");
    public static readonly Error UserExists = new("UserExists", "Já existe um utilizador com este email");
    public static readonly Error InvalidCredentials = new("InvalidCredentials", "As credenciais fornecidas são inválidas");
    public static readonly Error InvalidPassword = new("InvalidPassword", "A password é inválida");
    public static readonly Error PasswordTooWeak = new("PasswordTooWeak", "A password não cumpre os requisitos de segurança");
    public static readonly Error PasswordsDoNotMatch = new("PasswordsDoNotMatch", "As passwords não coincidem");
    public static readonly Error EmailAlreadyExists = new("EmailAlreadyExists", "Este email já está registado");
    public static readonly Error InvalidToken = new("InvalidToken", "O token fornecido é inválido ou expirou");
    public static readonly Error RefreshTokenExpired = new("RefreshTokenExpired", "O token de atualização expirou");
    public static readonly Error InvalidRefreshToken = new("InvalidRefreshToken", "Token de atualização inválido");
    public static readonly Error UserInactive = new("UserInactive", "A conta do utilizador está inativa");

    // Client Errors
    public static readonly Error ClientNotFound = new("ClientNotFound", "Cliente não encontrado");
    public static readonly Error ClientExists = new("ClientExists", "Já existe um perfil de cliente para este utilizador");
    public static readonly Error NifAlreadyExists = new("NifAlreadyExists", "Já existe um cliente com este NIF");

    // RealEstateAgent Errors
    public static readonly Error AgentNotFound = new("AgentNotFound", "Agente imobiliário não encontrado");
    public static readonly Error AgentExists = new("AgentExists", "Já existe um perfil de agente para este utilizador");
    public static readonly Error LicenseExpired = new("LicenseExpired", "A licença do agente expirou");
    public static readonly Error InvalidLicense = new("InvalidLicense", "Licença profissional inválida");

    // Property Errors
    public static readonly Error PropertyNotFound = new("PropertyNotFound", "Imóvel não encontrado");
    public static readonly Error PropertyUnavailable = new("PropertyUnavailable", "O imóvel não está disponível");
    public static readonly Error PropertyAlreadySold = new("PropertyAlreadySold", "O imóvel já foi vendido");
    public static readonly Error PropertyAlreadyRented = new("PropertyAlreadyRented", "O imóvel já foi arrendado");
    public static readonly Error PropertyAlreadyFavorited = new("PropertyAlreadyFavorited", "O imóvel já está nos favoritos");
    public static readonly Error PropertyNotFavorited = new("PropertyNotFavorited", "O imóvel não está nos favoritos");

    // PropertyVisit Errors
    public static readonly Error VisitNotFound = new("VisitNotFound", "Visita não encontrada");
    public static readonly Error TimeSlotUnavailable = new("TimeSlotUnavailable", "O horário selecionado não está disponível");
    public static readonly Error VisitAlreadyConfirmed = new("VisitAlreadyConfirmed", "A visita já foi confirmada");
    public static readonly Error VisitAlreadyCancelled = new("VisitAlreadyCancelled", "A visita já foi cancelada");
    public static readonly Error InvalidConfirmationToken = new("InvalidConfirmationToken", "Token de confirmação inválido");
    public static readonly Error VisitDatePast = new("VisitDatePast", "A data da visita não pode ser no passado");

    // Contract Errors
    public static readonly Error ContractNotFound = new("ContractNotFound", "Contrato não encontrado");
    public static readonly Error ContractAlreadyActive = new("ContractAlreadyActive", "O contrato já está ativo");
    public static readonly Error ContractAlreadyTerminated = new("ContractAlreadyTerminated", "O contrato já foi terminado");
    public static readonly Error ContractExpired = new("ContractExpired", "O contrato expirou");
    public static readonly Error InvalidContractDates = new("InvalidContractDates", "As datas do contrato são inválidas");
}

