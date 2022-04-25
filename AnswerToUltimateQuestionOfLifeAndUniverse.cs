namespace TakeprofitTechnologyQuiz;

public static class AnswerToUltimateQuestionOfLifeAndUniverse
{
    public static void GetAnswer()
    {
        Console.WriteLine("** Answer for QWIZ  **\n\n\n");
        TakeprofitTechServerInteraction server = new TakeprofitTechServerInteraction();
        List<uint> dictionary = server.GetServerAnswersDictionary();

        if (dictionary != null)
        {
            foreach (uint answer in dictionary)
            {
                Console.WriteLine(answer);
            }

            Console.WriteLine($"Congratulations! The answer to the quiz is the number = {GetMediana(dictionary)}");
            return;
        }

        using (StreamWriter str = new StreamWriter("zadanie.txt", false))
        {
            foreach (var number in dictionary)
            {
                str.Write(number.ToString());
            }

            str.WriteLine($"\n\n Answer = {GetMediana(dictionary)}!");
        }

        Console.WriteLine("File writed!\n");


        Console.WriteLine("Failed to get number. An error occurred while getting data from the server!");
        Console.WriteLine("\n\n\n**********END*********");
    }

    private static uint GetMediana(List<uint> numbers)
    {
        numbers.Sort();

        if (numbers.Count % 2 != 0)
        {
            //"Не четное"
            return numbers[numbers.Count / 2];
        }

        //"Четное"
        return (numbers[numbers.Count / 2 - 1] + numbers[numbers.Count / 2]) / 2;
    }
}