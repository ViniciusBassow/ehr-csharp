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
            //string apiKey = _configuration["OpenAI:ApiKey"];
            //using (HttpClient client = new HttpClient())
            //{
            //client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            //var requestBody = new
            //{
            //    model = "gpt-4o",  // Escolha o modelo apropriado da OpenAI
            //    prompt = $"Por favor, analise os seguintes dados de um exame de hemograma: {text}",
            //    temperature = 0.5,
            //    max_tokens = 1500
            //};

            //var content = new StringContent(JsonConvert.SerializeObject(requestBody), System.Text.Encoding.UTF8, "application/json");

            //var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
            //var responseContent = await response.Content.ReadAsStringAsync();

            //return result.choices[0].text;}


            var client = new RestClient("https://api.openai.com/v1/chat/completions");
                
                var request = new RestRequest("", RestSharp.Method.Post); 
                request.AddHeader("Accept", "/");
                request.AddHeader("Content-Type", "application/json");
                
                request.AddHeader("Authorization", $"Bearer {_configuration["OpenAI:ApiKey"]}");
            //request.AddParameter("model", "gpt-4o");
            //request.AddParameter("prompt", "Analise os dados completos desse exame de hemograma: {text}");
            //request.AddParameter("temperature", 0.5);
            //request.AddParameter("max_tokens", 1500);

            var requestBody = new
            {
                model = "gpt-4o",  // Escolha o modelo apropriado da OpenAI
                messages = new[] { new {  role = "user", content = $"Retorne todos os dados desse documento em json, quero os dados em portugues, sem nenhum comentario seu, somente o json : {text}" } },
                temperature = 0.5,
                max_tokens = 1500,
                response_format = new{ type = "json_object"} 
        };

            request.AddJsonBody(requestBody);
            var content = new StringContent(JsonConvert.SerializeObject(requestBody), System.Text.Encoding.UTF8, "application/json");
            
            request.AddJsonBody(content);
            
            var response = client.Execute(request);


            dynamic result = JsonConvert.DeserializeObject(response.Content.Replace("\\n", " "));

            var test = response.Content.Replace("\\n", "").Replace("\\\"", "");

            return null;
        }
    }
}