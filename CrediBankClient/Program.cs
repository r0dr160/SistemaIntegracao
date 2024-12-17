using System;
using System.Net.Http;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // Dados de entrada
        string creditAccountId = "1234567890123456"; // ID fictício
        int value = 100; // Valor a ser debitado

        // URL da API
        string url = $"https://credibank.intsis.utad.pt:8080/check/{creditAccountId}/ammount/{value}/";

        // Chamar API
        Console.WriteLine("Chamando API do CrediBank...");
        var result = await CallApiAsync(url);

        // Exibir resultado
        if (result != null)
        {
            Console.WriteLine($"Cheque gerado com sucesso! ID: {result.CheckID}");
            Console.WriteLine($"Data: {result.Date}");
        }
        else
        {
            Console.WriteLine("Falha ao gerar cheque.");
        }
    }

    static async Task<ChequeResponse> CallApiAsync(string url)
    {
        // Ignorar certificado SSL (para testes apenas)
        HttpClientHandler handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        };

        using HttpClient client = new HttpClient(handler);
        try
        {
            // Fazer requisição GET
            var response = await client.GetAsync(url);

            // Verificar o status da resposta
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ChequeResponse[]>(jsonString);

                return result?[0]; // Retorna o primeiro cheque
            }
            else
            {
                Console.WriteLine($"Erro na API. Status code: {response.StatusCode}");
            }
        }
        catch (HttpRequestException httpEx)
        {
            Console.WriteLine($"Erro na requisição HTTP: {httpEx.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro inesperado: {ex.Message}");
        }

        return null;
    }
}

// Classe para mapear a resposta da API
public class ChequeResponse
{
    public string Date { get; set; }
    public string CheckID { get; set; }
}
