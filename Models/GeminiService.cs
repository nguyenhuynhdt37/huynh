using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Diagnostics;

namespace OnlineCourse.Models
{
    public class GeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "AIzaSyDGGBE0z5cBAh_1mYso6BkUNqh0peofVGs";
        private List<string> _chatHistory = new List<string>();
        private const int MaxHistorySize = 10;
        private readonly string _systemPrompt = @"Bạn là trợ lý AI chuyên về lập trình, giúp đỡ học viên trong khóa học lập trình. 
                    Hãy trả lời các câu hỏi một cách chính xác, rõ ràng và hữu ích.

                    Khi giải thích khái niệm:
                    - Sử dụng ngôn ngữ đơn giản, dễ hiểu
                    - Đưa ra ví dụ cụ thể minh họa
                    - Liên hệ với các ứng dụng thực tế

                    Khi giúp với code:
                    - Cung cấp mã nguồn rõ ràng, có comment giải thích
                    - Tuân thủ các quy tắc coding standards
                    - Giải thích logic và cách hoạt động của code

                    Khi giải quyết bài tập:
                    - Hướng dẫn từng bước giải quyết vấn đề
                    - Khuyến khích tư duy phản biện
                    - Không chỉ đưa ra đáp án mà còn giải thích cách tiếp cận

                    Ngôn ngữ và công nghệ chính trong khóa học:
                    - C#, ASP.NET Core
                    - HTML, CSS, JavaScript
                    - SQL Server
                    - Git và GitHub

                    Hãy luôn giữ giọng điệu thân thiện, khuyến khích và hỗ trợ học viên phát triển kỹ năng lập trình.";

        public GeminiService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<string> GetChatResponseAsync(string userMessage)
        {
            try
            {
                // Add user message to history
                _chatHistory.Add($"User: {userMessage}");

                // Keep only the last 10 messages for context
                while (_chatHistory.Count > MaxHistorySize)
                {
                    _chatHistory.RemoveAt(0);
                }

                // Build a prompt with system instructions and the chat history
                string fullPrompt = _systemPrompt + "\n\n" + string.Join("\n\n", _chatHistory);

                // Correct URL for Gemini API
                var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={_apiKey}";

                // Create request with correct structure
                var requestJson = @"
                {
                  ""contents"": [
                    {
                      ""parts"": [
                        {
                          ""text"": """ + fullPrompt.Replace("\"", "\\\"") + @"""
                        }
                      ]
                    }
                  ],
                  ""generationConfig"": {
                    ""temperature"": 0.7,
                    ""topK"": 40,
                    ""topP"": 0.95,
                    ""maxOutputTokens"": 1024
                  }
                }";

                Debug.WriteLine($"Request JSON: {requestJson}");

                var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(url, content);

                // Log response for debugging
                var responseContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Response Status: {response.StatusCode}");
                Debug.WriteLine($"Response Content: {responseContent}");

                if (!response.IsSuccessStatusCode)
                {
                    return $"API Error: {response.StatusCode} - {responseContent}";
                }

                // Try to parse the response
                try
                {
                    var responseJson = JsonDocument.Parse(responseContent);
                    var text = responseJson.RootElement.GetProperty("candidates")[0]
                                          .GetProperty("content")
                                          .GetProperty("parts")[0]
                                          .GetProperty("text").GetString();

                    // Add AI response to history
                    _chatHistory.Add($"AI: {text}");

                    return text ?? "Sorry, I couldn't generate a response.";
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error parsing response: {ex.Message}");
                    return $"Error parsing response: {ex.Message}";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return $"Error: {ex.Message}";
            }
        }

        public void ResetChat()
        {
            _chatHistory.Clear();
        }
    }
}