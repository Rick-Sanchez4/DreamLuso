// =====================================================
// DreamLuso - Script para criar Usuário Admin
// =====================================================
// Uso: dotnet script CreateAdminUser.cs
// =====================================================

using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
Console.WriteLine("🎯 CRIANDO USUÁRIO ADMIN VIA API");
Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
Console.WriteLine();

var apiUrl = "http://localhost:5149/api/accounts/register";

var adminData = new
{
    email = "admin@gmail.com",
    password = "Admin123!",
    firstName = "Admin",
    lastName = "DreamLuso",
    role = "Admin"
};

using var client = new HttpClient();
client.Timeout = TimeSpan.FromSeconds(30);

try
{
    Console.WriteLine("📤 Enviando requisição para criar Admin...");
    Console.WriteLine($"   URL: {apiUrl}");
    Console.WriteLine($"   Email: {adminData.email}");
    Console.WriteLine();

    var response = await client.PostAsJsonAsync(apiUrl, adminData);
    var content = await response.Content.ReadAsStringAsync();

    if (response.IsSuccessStatusCode)
    {
        Console.WriteLine("✅ ADMIN CRIADO COM SUCESSO!");
        Console.WriteLine();
        Console.WriteLine("📋 Resposta da API:");
        
        try
        {
            var jsonDoc = JsonDocument.Parse(content);
            var formatted = JsonSerializer.Serialize(jsonDoc, new JsonSerializerOptions { WriteIndented = true });
            Console.WriteLine(formatted);
        }
        catch
        {
            Console.WriteLine(content);
        }
        
        Console.WriteLine();
        Console.WriteLine("🔑 CREDENCIAIS:");
        Console.WriteLine($"   Email: {adminData.email}");
        Console.WriteLine($"   Senha: {adminData.password}");
        Console.WriteLine();
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
    }
    else
    {
        Console.WriteLine($"❌ ERRO ao criar Admin (HTTP {response.StatusCode})");
        Console.WriteLine();
        Console.WriteLine("📋 Resposta:");
        Console.WriteLine(content);
        Console.WriteLine();
        
        if (content.Contains("already exists") || content.Contains("já existe"))
        {
            Console.WriteLine("⚠️  O usuário admin@gmail.com já existe!");
            Console.WriteLine("    Pode fazer login com as credenciais:");
            Console.WriteLine($"    Email: {adminData.email}");
            Console.WriteLine($"    Senha: {adminData.password}");
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ ERRO: {ex.Message}");
    Console.WriteLine();
    Console.WriteLine("⚠️  Verifique se:");
    Console.WriteLine("   1. A API está rodando (dotnet run)");
    Console.WriteLine("   2. O firewall do Azure está configurado");
    Console.WriteLine("   3. A connection string está correta");
}

Console.WriteLine();

