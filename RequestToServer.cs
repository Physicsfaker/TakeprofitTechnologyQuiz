using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TakeprofitTechnologyQuiz;

public class RequestToServer
{
    private static readonly string _address;
    private static readonly int _port;
    private static readonly Encoding _encoding;

    private readonly int _maximumCommunicationErrors = 10;

    private readonly uint _getErrorGettingNumber = 11111111; //такого не может прийдти по тз (0 <= x< 1e7), используем как ошибку

    static Semaphore semaphore = new Semaphore(_getOptimalNumberofThreads, _getOptimalNumberofThreads);
    private static int _getOptimalNumberofThreads => Environment.ProcessorCount / 2 - 1;

    static RequestToServer()
    {
        ServicePointManager.DefaultConnectionLimit = _getOptimalNumberofThreads;
        _address = ServerInfo.Address;
        _port = ServerInfo.Port;

        _encoding = CodePagesEncodingProvider.Instance.GetEncoding("koi8r") ?? Encoding.Default;

        Console.OutputEncoding = _encoding;
    }

    public uint RequestUniqueNumber(int requestData)
    {
        semaphore.WaitOne();
        for (int i = _maximumCommunicationErrors; i >= 0; i--)
        {
            var resault = GetAndCheckNumberFromServer(requestData);
            if (resault != _getErrorGettingNumber)
            {
                semaphore.Release();
                return resault;
            }
        }

        throw new Exception("Too many errors when trying to connect to the server. The application will be closed!");
    }

    private uint GetAndCheckNumberFromServer(int targetNumber)
    {
        try
        {
            // адрес и порт сервера, к которому будем подключаться
            int port = _port; // порт сервера
            string address = _address; // адрес сервера

            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


            socket.Connect(ipPoint);
            string message = $"{targetNumber}\n";

            uint resault = 0;
            var VerifiedNumbers = new List<uint>();

            //for (int i = 0; i < 20; i++)
            while (true)
            {
                // подключаемся к удаленному хосту
                byte[] data = Encoding.UTF8.GetBytes(message);
                socket.Send(data);

                // получаем ответ
                data = new byte[1024 * 1024 * 3]; // буфер для ответа
                StringBuilder builder = new StringBuilder();
                int bytes = 0; // количество полученных байт

                do
                {
                    bytes = socket.Receive(data, data.Length, 0);
                    //builder.Append(CodePagesEncodingProvider.Instance.GetEncoding("koi8r")?.GetString(data, 0, bytes));
                    builder.Append(Encoding.Default.GetString(data, 0, bytes));
                    //} while (builder.ToString().Contains('\n'));
                } while (socket.Available > 0);

                Console.WriteLine($"Ответ сервера на таску номер {targetNumber}: " + builder.ToString());


                if (uint.TryParse((string?) builder.ToString(), out resault))
                {
                    //проверяем проверялись ли подобные ответы
                    if (VerifiedNumbers.Contains(resault))
                    {
                        continue;
                    }

                    data = Encoding.UTF8.GetBytes($"Check <{resault}>\n");
                    socket.Send(data);

                    data = new byte[1024 * 1024 * 3]; // буфер для ответа
                    StringBuilder builder2 = new StringBuilder();
                    bytes = 0; // количество полученных байт

                    do
                    {
                        bytes = socket.Receive(data, data.Length, 0);
                        //builder2.Append(CodePagesEncodingProvider.Instance.GetEncoding("koi8r")
                        //    ?.GetString(data, 0, bytes));
                        builder2.Append(Encoding.Default.GetString(data, 0, bytes));
                        //} while (builder.ToString().Contains('\n'));
                    } while (socket.Available > 0);

                    Console.WriteLine(
                        $"Task №{targetNumber} - Проверка ответа '{resault}' от сервера: результат проверки = '{builder2.ToString()}'");

                    uint resault2;

                    if (uint.TryParse((string?) builder2.ToString(), out resault2))
                    {
                        if (resault == resault2)
                        {
                            break;
                        }
                    }
                }

                VerifiedNumbers.Add(resault);
            }

            // закрываем сокет
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            return resault;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Task Number {targetNumber}:    " + ex.Message);
            return _getErrorGettingNumber;
        }
    }
}