BEGIN QUIZ TEXT (encoding: koi8_r)

Task for the vacancy "C# programmer"
========================================

Write a small C# program that will
connect to the same address/port, but instead of sending
"Greetings<LINE FEED", it sends an integer from 1 to 2018
in text view, also with a newline at the end.
In response to the sent number, the server will respond with another integer
(0 <= x< 1e7) - a line with a line feed character at the end and any
the amount of garbage to the left and right of the number.
As a result, it turns out that in this way you can get 2018 whole
numbers. For each input number, the answer is always the same
the same output number, that is, they can be requested in any order,
and, if necessary, several times.
You need to calculate the median of these 2018 numerical values.
The difficulty lies in the fact that the server deliberately delays the response,
so it will take a very long time to get all the numbers one by one.
It is necessary that the program be multi-threaded and make requests
parallel. Take into account in the program that under load
network connection may not always be available or not always
stable, or the server may crash or drop the connection
under load.
The correctness of the answer can also be checked through the server:
to do this, send a "Check <ANSWER\n" request, where ANSWER is
the value you received.
