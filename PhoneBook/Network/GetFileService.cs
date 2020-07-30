using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace PhoneBook.Network
{
    public class GetFileService
    {
        private readonly HttpClient _client;

        public GetFileService(HttpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<string> TryGetXmlFileAsync(string fileUrl)
        {
            var response = await _client.GetAsync(fileUrl);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }

            return null;
        }
    }
}
