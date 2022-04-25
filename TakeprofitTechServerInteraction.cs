namespace TakeprofitTechnologyQuiz;

public class TakeprofitTechServerInteraction
{
    private readonly uint _requestsCount = 2018;
    public List<uint> GetServerAnswersDictionary() => GetNumbersInParallel();

    public List<uint> GetNumbersInParallel()
    {
        var resaultList = new List<uint>();

        var tasks = new Task<uint>[_requestsCount];

        for (int number = 1; number <= _requestsCount; number++)
        {
            var targetNumber = number;
            tasks[number - 1] = new Task<uint>(() => new RequestToServer().RequestUniqueNumber(targetNumber));
        }

        foreach (var targeTask in tasks)
        {
            targeTask.Start();
        }

        resaultList.AddRange(Task.WhenAll(tasks).Result);


        Console.WriteLine("Done!");
        return resaultList;
    }
}