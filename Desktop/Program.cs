#pragma warning disable CS8602
using System.Net;

void Listen(string[] files)
{
    if (!HttpListener.IsSupported)
    {
        Console.WriteLine("HttpListener is not supported.");
        return;
    }
    if (files == null || files.Length == 0)
    {
        throw new ArgumentException("files");
    }
    HttpListener listener = new HttpListener();
    Console.WriteLine("Listening on\n");
    foreach (string file in files)
    {
        Console.WriteLine(file);
        try
        {
            listener.Prefixes.Add(file);
        }
        catch
        {
            listener.Prefixes.Add($"{file}/");
        }
    }
    Console.WriteLine();

    while (true)
    {
        listener.Start();
        HttpListenerContext context = listener.GetContext();
        HttpListenerRequest request = context.Request;
        HttpListenerResponse response = context.Response;
        string appPath = AppDomain.CurrentDomain.BaseDirectory;
        string filePath = appPath + request.Url.LocalPath.TrimStart('/').Replace("/", "\\");
        Console.WriteLine("Request from " + filePath);
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(@$"
        <!DOCTYPE html>
        <html lang=""en"">
        <head>
            <meta charset=""UTF-8"">
            <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
            <style>
                * {{
                    font-family: system-ui, -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, 'Open Sans', 'Helvetica Neue', sans-serif;
                }}
            </style>
            <title>Page not found</title>
        </head>
        <body>
            <h1>Page not found.</h1>
            <p>HTTP ERROR CODE 404</p>
        </body>
        </html>
        ");
        try
        {
            buffer = File.ReadAllBytes(filePath);
        }
        catch { }
        if (filePath.EndsWith(".svg"))
        {
            response.ContentType = "image/svg+xml";
        }
        else if (filePath.EndsWith(".html"))
        {
            response.ContentType = "text/html";
        }
        else if (filePath.EndsWith(".css"))
        {
            response.ContentType = "text/css";
        }
        else if (filePath == AppDomain.CurrentDomain.BaseDirectory)
        {
            buffer = File.ReadAllBytes($"{AppDomain.CurrentDomain.BaseDirectory}index.html");
        }

        response.ContentLength64 = buffer.Length;
        using (Stream output = response.OutputStream)
        {
            output.Write(buffer, 0, buffer.Length);
        }
    }
}

int port = new Random().Next(1000, 65535);
if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Localhoster\\"))
{
    Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Localhoster\\");
}
File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Localhoster\\127.0.0.1.port", port.ToString());
Console.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Localhoster\\127.0.0.1.port");

Listen(
[
    $"http://127.0.0.1:{port}/",
    $"http://127.0.0.1:{port}/index.html",
    $"http://127.0.0.1:{port}/eco-gemini.html",
    $"http://127.0.0.1:{port}/todo.html",
    $"http://127.0.0.1:{port}/daily.html",
    $"http://127.0.0.1:{port}/leaderboard.html",
    $"http://127.0.0.1:{port}/about.html",
    $"http://127.0.0.1:{port}/style.css",
    $"http://127.0.0.1:{port}/Assets/User.svg",
    $"http://127.0.0.1:{port}/Assets/Sun.svg",
    $"http://127.0.0.1:{port}/Assets/Moon.svg",
    $"http://127.0.0.1:{port}/Assets/Changelog.svg",
    $"http://127.0.0.1:{port}/Assets/Gemini.svg",
    $"http://127.0.0.1:{port}/Assets/Home.svg",
    $"http://127.0.0.1:{port}/Assets/Quiz.svg",
    $"http://127.0.0.1:{port}/Assets/File.svg",
    $"http://127.0.0.1:{port}/Assets/Todo.svg",
    $"http://127.0.0.1:{port}/Assets/Submit.svg",
    $"http://127.0.0.1:{port}/Assets/Settings.svg",
    $"http://127.0.0.1:{port}/Assets/About.svg",
    $"http://127.0.0.1:{port}/Assets/Close.svg",
    $"http://127.0.0.1:{port}/Assets/Download.svg",
    $"http://127.0.0.1:{port}/Assets/Reset.svg",
    $"http://127.0.0.1:{port}/Assets/Leaderboard.svg"
]);