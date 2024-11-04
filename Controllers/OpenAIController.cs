using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ehr_csharp.Models;
using Microsoft.AspNetCore.Identity;
using RestSharp;
using System.Text;
using System;
using OpenAI.Chat;

namespace ehr_csharp.Controllers
{
    public class OpenAIController : Controller
    {
        private readonly IConfiguration _configuration;

        public OpenAIController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<ActionResult> Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ViewBag.Message = "Por favor, selecione um arquivo.";
                return View();
            }

            // Salvar o arquivo temporariamente
            var filePath = Path.Combine(Path.GetTempPath(), file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Extrair texto do PDF
            var extractedText = await ExtractTextFromPdf(filePath);

            // Chamar OpenAI API para analisar os dados
            var analysisResult = await AnalyzeTextWithOpenAI(extractedText);

            // Retornar o resultado para a View
            ViewBag.Analysis = analysisResult;
            return View("AnalysisResult");
        }

        private async Task<string> ExtractTextFromPdf(string filePath)
        {
            // Utilize uma biblioteca de extração de PDF, como PdfSharp ou iTextSharp, para extrair o texto.
            // Exemplo de código com iTextSharp:
            string extractedText = string.Empty;

            using (var reader = new iTextSharp.text.pdf.PdfReader(filePath))
            {
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    extractedText += iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(reader, i);
                }
            }

            return extractedText;
        }

        private async Task<string> AnalyzeTextWithOpenAI(string text)
        {
            var client = new RestClient("https://api.openai.com/v1/chat/completions");
            var request = new RestRequest("", Method.Post);

            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", $"Bearer {_configuration["OpenAI:ApiKey"]}");

            var requestBody = new
            {
                model = "gpt-3.5-turbo-1106", // Ajuste o modelo conforme necessário
                messages = new[]
                {
            new { role = "user", content = $"Analise o texto fornecido, extraído de um exame de hemograma em PDF. Devolva a resposta no formato de texto mas formatado como um json, contendo somente um objeto com o somente o valor.  Use esse json como template para o tipo da resposta e não crie nenhuma chave e nem tire uma chave somente preencha os valores, lembrando q os valores são do tipo double :\r\n\r\n{{\r\n  \"Hemograma\": {{\r\n    \"Eritrocitos\": \"\",\r\n    \"Hemoglobina\": \"\",\r\n    \"Hematocrito\": \"\",\r\n    \"VCM\": \"\",\r\n    \"HCM\": \"\",\r\n    \"CHCM\": \"\",\r\n    \"RDW\": \"\",\r\n    \"Leucocitos\": {{\r\n      \"Absoluto\": \"\",\r\n      \"Relativo\": \"\"\r\n    }},\r\n    \"Bastonetes\": {{\r\n      \"Absoluto\": \"\",\r\n      \"Relativo\": \"\"\r\n    }},\r\n    \"Segmentados\": {{\r\n      \"Absoluto\": \"\",\r\n      \"Relativo\": \"\"\r\n    }},\r\n    \"Eosinofilos\": {{\r\n      \"Absoluto\": \"\",\r\n      \"Relativo\": \"\"\r\n    }},\r\n    \"Basofilos\": {{\r\n      \"Absoluto\": \"\",\r\n      \"Relativo\": \"\"\r\n    }},\r\n    \"Linfocitos\": {{\r\n      \"Absoluto\": \"\",\r\n      \"Relativo\": \"\"\r\n    }},\r\n    \"Monocitos\": {{\r\n      \"Absoluto\": \"\",\r\n      \"Relativo\": \"\"\r\n    }},\r\n    \"Plaquetas\":  \"\",\r\n    \"VPM\":  \"\",\r\n    \"Glicemia\": \"\",\r\n    \"Creatinina\": \"\",\r\n    \"AcidoUrico\": \"\",\r\n    \"Prolactina\": \"\",\r\n    \"Testosterona\": \"\",\r\n    \"ColesterolTotal\": \"\",\r\n    \"HDL\": \"\",\r\n    \"Triglicerides\": \"\",\r\n    \"LDL\": \"\",\r\n    \"NaoHDL\": \"\"\r\n  }}\r\n}}\r\n\r\n. Utilize os dados do texto: {text}" }
        },
                temperature = 0.5,
                max_tokens = 1500,
                response_format = new { type = "json_object" } // Adicionando o response_format

            };

            request.AddJsonBody(requestBody);

            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                // Deserializar o conteúdo da resposta
                var result = JsonConvert.DeserializeObject<dynamic>(response.Content);

                // Extrair o JSON do conteúdo da mensagem
                string jsonContent = result?.choices[0]?.message?.content;

                return jsonContent;
            }
            else
            {
                // Trate o erro conforme necessário, por exemplo, logando a resposta
                throw new Exception($"Erro na chamada para OpenAI: {response.StatusCode} - {response.Content}");
            }
        }

    }
}