using System;
using System.Net;
using System.IO;
using System.Timers;

namespace consoleWather
{
    class ConsoleWeb
    {

        private static System.Timers.Timer timer;
        
        // Метод для данных из web.
        public void Web(Object source, ElapsedEventArgs e)
        {
            try
            {
                // Создание объекта WebRequest.
                WebRequest request = WebRequest.Create("https://gismeteo.ru/");
                // Отправка запроса и получение ответа.
                WebResponse response = request.GetResponse();
                // Получение потока ответа и манипуляции с ним.
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string linewriter = "";
                        string line = "";
                        while ((line = reader.ReadLine()) != null)
                        {
                            linewriter = linewriter + line;
                        }
                        WebStreamWriter(linewriter);
                        WebStreamReader();
                    }
                }
                WebHeaderCollection headers = response.Headers;
                for (int i = 0; i < headers.Count; i++)
                {
                    Console.WriteLine("{0}:   {1}", headers.GetKey(i), headers[i]);
                }
                response.Close();
            } catch(WebException ex)
            {
                WebExceptionStatus status = ex.Status;
                if(status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)ex.Response;
                    Console.WriteLine("Статусный код ошибки:   {0} - {1}", (int)httpResponse.StatusCode, httpResponse.StatusCode);
                }
            }
            Console.WriteLine("Запрос выполнен.");
            // abm delete 14012021 Console.ReadKey();
        }

        // Метод для записи в файл web контента.
        public void WebStreamWriter(string linewriter)
        {
            try
            {
                using (StreamWriter streamWriter = new StreamWriter("c:\\abm_file\\gismeteo.html", false, System.Text.Encoding.Default))
                {
                    streamWriter.WriteLine(linewriter);
                }
            } catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        // Метод для чтения файла с web контетом. Поиск значения температуры. Запись файл csv.
        public void WebStreamReader()
        {
            string stringtemp = "";
            try
            {
                using (StreamReader streamReader = new StreamReader("c:\\abm_file\\gismeteo.html"))
                {
                    string linereader = streamReader.ReadToEnd();
                    string stringfind = "\"js_meas_container temperature\" data-value=";
                    long indexoflinereader = linereader.IndexOf(stringfind);
                    char[] charlinereader = linereader.ToCharArray();
                    long count = stringfind.Length + indexoflinereader;
                    int charcount = 0;
                    for(long i = count; i < charlinereader.Length; i++)
                    {
                        stringtemp = stringtemp + charlinereader[i].ToString();
                        if (charlinereader[i] == '"' && charcount <= 1)
                        {
                            charcount++;
                        }
                        if (charcount > 1) break;
                    }
                }
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.Write("Дата и время:                              {0}\r\n", DateTime.Now);
            try
            {
                using (StreamWriter streamWritercsv = new StreamWriter("c:\\abm_file\\gismeteo.csv", true, System.Text.Encoding.Default))
                {
                    streamWritercsv.WriteLine(DateTime.Now + "," + stringtemp + ",");
                }
            } catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            stringtemp = stringtemp.Remove(0, 1);
            stringtemp = stringtemp.Remove(stringtemp.Length - 1, 1);
            Console.Write("Температура на улице, градусах С:         {0}\r\n", stringtemp);
        }

        // Метод для таймера.
        public void TimerWeb()
        {
            timer = new System.Timers.Timer(10000);
            timer.Elapsed += Web;
            timer.AutoReset = true;
            timer.Enabled = true;
            Console.Write("Нажмите на любую клавишу для выхода...\r\n");
            Console.ReadKey();
            timer.Stop();
            timer.Dispose();
        }
    }
}
