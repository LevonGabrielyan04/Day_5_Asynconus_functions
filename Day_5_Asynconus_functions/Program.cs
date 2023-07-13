using System.Drawing;

class Task1
{
    string path;
    public Task task;
    public Task1(string Path)
    {
        task = new Task(() =>
        {
            string p = File.ReadAllText(this.path);
            lock (Program.bigText)
            {
                Program.bigText += p + "\n";
            }
        });
        path = Path;
    }

}
class Task2
{
    string[] paths;
    public Task2(string dir)
    {
        paths = Directory.GetFiles(dir);
    }
    public void Resize()
    {
        Bitmap bitmap = new Bitmap(paths[0]);
    }
}
class Program
{
    public static string bigText = String.Empty;
    static void Main()
    {
        //Task 1
        //var a = new Task1(@"C:\A\someText.txt");
        //var b = new Task1(@"C:\A\someText2.txt");
        //a.task.Start();
        //b.task.Start();
        //Task.WaitAll(a.task, b.task);
        //Console.WriteLine(Program.bigText);

        //Task 2
        Task2 task = new Task2(@"C:\B");
    }
}