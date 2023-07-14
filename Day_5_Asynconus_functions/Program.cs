using System.Diagnostics.Metrics;
using System.Drawing;
using System.Net.Http.Headers;

class Task1
{
    string path;
    public Task1(string Path)
    {
        path = Path;
    }
    public Task Run()
    {
        return Task.Run(() =>
        {
            string p = File.ReadAllText(this.path);
            lock (Program.bigText)
            {
                Program.bigText += p + "\n";
            }
        });
    } 
}
class Task2
{
    public string[] paths;
    public Task2(string dir)
    {
        paths = Directory.GetFiles(dir);
    }
    public Task Resize()
    {
        return Task.Run(() =>
        {
            int count = 1;
            foreach (var item in paths)
            {
                Bitmap bitmap = new Bitmap(item);
                bitmap = new Bitmap(bitmap, new Size(256, bitmap.Height));
                bitmap.Save(@"C:\ResizedImages\ResizedImage" + count + ".jpeg");
                count++;
            }
        });
    }
    public Task MakeBlackAndWhite()
    {
        return Task.Run(() =>
        {
            int count = 1;
            foreach (var item in paths)
            {
                // создаём Bitmap из изображения, находящегося в pictureBox1
                Bitmap input = new Bitmap(item);
                // создаём Bitmap для черно-белого изображения
                Bitmap output = new Bitmap(input.Width, input.Height);
                // перебираем в циклах все пиксели исходного изображения
                for (int j = 0; j < input.Height; j++)
                    for (int i = 0; i < input.Width; i++)
                    {
                        // получаем (i, j) пиксель
                        UInt32 pixel = (UInt32)(input.GetPixel(i, j).ToArgb());
                        // получаем компоненты цветов пикселя
                        float R = (float)((pixel & 0x00FF0000) >> 16); // красный
                        float G = (float)((pixel & 0x0000FF00) >> 8); // зеленый
                        float B = (float)(pixel & 0x000000FF); // синий
                                                               // делаем цвет черно-белым (оттенки серого) - находим среднее арифметическое
                        R = G = B = (R + G + B) / 3.0f;
                        // собираем новый пиксель по частям (по каналам)
                        UInt32 newPixel = 0xFF000000 | ((UInt32)R << 16) | ((UInt32)G << 8) | ((UInt32)B);
                        // добавляем его в Bitmap нового изображения
                        output.SetPixel(i, j, Color.FromArgb((int)newPixel));
                    }
                // выводим черно-белый Bitmap в pictureBox2
                count++;
                output.Save(@"C:\BWImages\ColorlessImage" + count + ".jpeg");
            }
        });
    }
}
class Task3
{
    public List<Func<Task>> funcsToExecute;
    public Task3()
    {
        funcsToExecute = new List<Func<Task>>();
    }
    public Task RunAtIntervals(int miliseconds,CancellationToken token)
    {
        return Task.Run(() =>
        {
            bool flag = false;
            while (true)
            {
                foreach (var item in funcsToExecute)
                {
                    item();
                    if (token.IsCancellationRequested) { 
                        flag = true;
                        break;
                    }
                }
                Thread.Sleep(miliseconds);
                if(flag)
                    break;
            }
            Program.synchronization.Wait(); 
        });
    }
    public Task RunAtTime(DateTime date,CancellationToken token)
    {
        return Task.Run(() =>
        {
            Thread.Sleep(date - DateTime.Now);
            foreach (var item in funcsToExecute)
            {
                item();
                if (token.IsCancellationRequested) { 
                    break;
                }
            }
            Program.synchronization.Wait();
        });
    }
}
class Synchronization
{
    int count;
    object lockObj;
    public Synchronization(int count)
    {
        this.count = count;
        lockObj = new object();
    }
    public void Wait()
    {
        lock (lockObj)
        { 
            Monitor.Wait(lockObj);
        }
    }
    public void Pulse()
    {
        lock (lockObj)
        {
            count--;
            if (count == 0)
                Monitor.Pulse(lockObj);
        }
    }
    public void PulseAll()
    {
        lock (lockObj)
        { 
            Monitor.Pulse(lockObj);
        }
    }
}
class Program
{
    public static Synchronization synchronization;
    static Task Test()
    {
        return Task.Run(() =>
        {
            Console.WriteLine("Running");
            synchronization.Pulse();
        });
    }
    public static string bigText = String.Empty;
    async static Task Main()
    {
        //Task 1
        //var a = new Task1(@"C:\A\someText.txt");
        //var b = new Task1(@"C:\A\someText2.txt");
        //a.Run();
        //b.Run();
        //Console.WriteLine(Program.bigText);

        //Task 2
        //Task2 imgs = new Task2(@"C:\OriginalImages");
        //await imgs.MakeBlackAndWhite();
        //imgs.paths = Directory.GetFiles(@"C:\BWImages");
        //await imgs.Resize();

        //Task 3
        //Task3 task3 = new Task3();
        //task3.funcsToExecute.Add(Test);

        //CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        //cancellationTokenSource.CancelAfter(3);

        //synchronization = new SynchronizationByLevon(task3.funcsToExecute.Count);
        //await task3.RunAtTime(DateTime.Now.AddSeconds(1),cancellationTokenSource.Token); //dont run this string and string below at the same time
        //await task3.RunAtIntervals(3000, cancellationTokenSource.Token);
        
    }
}