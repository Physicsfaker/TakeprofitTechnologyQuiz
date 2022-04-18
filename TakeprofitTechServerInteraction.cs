using System.Net;

namespace TakeprofitTechnologyQuiz;

public class TakeprofitTechServerInteraction
{
    private HttpClient _httpClient;
    private readonly string _address;
    private readonly int _port;

    //private readonly int _requestsCount = 2018;
    private readonly int _requestsCount = 2;

    private string _serverUrl => $"http://{_address}:{_port}/";


    private int _getOptimalNumberofThreads => Environment.ProcessorCount / 2 - 1;

    public TakeprofitTechServerInteraction() : this(ServerInfo.Address, ServerInfo.Port)
    {
    }

    public TakeprofitTechServerInteraction(string address, int port)
    {
        _httpClient = new HttpClient();
        ServicePointManager.DefaultConnectionLimit = _getOptimalNumberofThreads;
        _address = address;
        _port = port;
    }

    public List<string> GetServerAnswersDictionary()
    {
        return GetNumbersInParallel();
    }

    public List<string> GetNumbersInParallel()
    {
        var resaultList = new List<string>();
        var numberOfRequest = 0;

        while (numberOfRequest < _requestsCount + 1)
        {
            var remainingRequests = _requestsCount - numberOfRequest;
            var batchOfTasks = remainingRequests < _getOptimalNumberofThreads
                ? remainingRequests
                : _getOptimalNumberofThreads;

            var tasks = new Task<string>[batchOfTasks];

            for (int i = 0; i < batchOfTasks; i++)
            {
                tasks[i] = RequestUniqueNumber(++numberOfRequest);
            }

            //foreach (var t in tasks)
            //{
            //    t.Start();
            //}

            resaultList.AddRange( Task.WhenAll(tasks).Result);
        }

        return resaultList;
    }

    private async Task<string> RequestUniqueNumber(int requestData)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync(_serverUrl + requestData);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            Console.WriteLine(responseBody);
            return responseBody;
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine("\nException Caught!");
            Console.WriteLine("Message :{0} ", e.Message);
            return String.Empty;
        }
    }
}