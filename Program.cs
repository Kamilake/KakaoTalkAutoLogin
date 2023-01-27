using System;
using static System.Net.Mime.MediaTypeNames;
using System;
using System.Runtime.InteropServices; // for DllImport
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using System.Diagnostics;
using Newtonsoft.Json;
using static MyApp.Invoke;
using Newtonsoft.Json.Linq;
using System.Security;

namespace MyApp
{

  public class Program
  {
    static void sendKey(IntPtr hWnd, VirtualKeys key)
    {
      PostMessage(hWnd, WM.KEYDOWN, VirtualKeys.Return, new IntPtr(0x00000001));
      Thread.Sleep(50);
      PostMessage(hWnd, WM.KEYUP, VirtualKeys.Return, new IntPtr(0x00000001));
    }
    // openssl enc -d -aes-256-cbc -a -salt -pass pass:kamikami -iter 100
    // file: credentials.json
    // static Dictionary<string, string> getCredential()
    // {
    //   JsonTextReader reader = new JsonTextReader(new StringReader(File.ReadAllText("credentials.json")));
    //   // {"kakaoAccount" : "exjang0@gmail.com","password" : "qwerty1234"}
    //   Dictionary<string, string> credentials = new Dictionary<string, string>() { { "kakaoAccount", "" }, { "password", "" } };
    //   String id = "";
    //   String pw = "";
    //   String pw_enc = "";
    //   String pw_dec = "";
    //   while (reader.Read())
    //   {
    //     if (reader.Value != null)
    //     {
    //       if ((reader.Value?.ToString() ?? "") == "kakaoAccount")
    //       {
    //         reader.Read();
    //         id = reader.Value?.ToString() ?? "";
    //       }
    //       if ((reader.Value?.ToString() ?? "") == "password")
    //       {
    //         reader.Read();
    //         pw_enc = reader.Value?.ToString() ?? "";
    //       }
    //     }
    //   }
    //   //decrtpt pw_enc
    //   ProcessStartInfo startInfo = new ProcessStartInfo();
    //   startInfo.FileName = "cmd.exe";
    //   startInfo.Arguments = "/C openssl enc -d -aes-256-cbc -a -salt -pass pass:kamikami -iter 100";
    //   startInfo.RedirectStandardInput = true;
    //   startInfo.RedirectStandardOutput = true;
    //   startInfo.UseShellExecute = false;
    //   startInfo.CreateNoWindow = true;
    //   Process process = new Process();
    //   process.StartInfo = startInfo;
    //   process.Start();
    //   process.StandardInput.WriteLine(pw_enc);
    //   process.StandardInput.Close();
    //   pw_dec = process.StandardOutput.ReadToEnd();
    //   process.WaitForExit();
    //   process.Close();
    //   try
    //   {
    //     pw = pw_dec.Substring(0, pw_dec.Length - 2);
    //   }
    //   catch (Exception e)
    //   {
    //     Console.WriteLine(e);
    //     Console.WriteLine("password decryption failed");
    //     throw new Exception("password decryption failed");
    //   }

    //   if (id == null || pw == null)
    //   {
    //     Console.WriteLine("id or pw is null");
    //     throw new Exception("id or pw is null");
    //   }
    //   credentials["kakaoAccount"] = id;
    //   credentials["password"] = pw;
    //   return credentials;

    // }

    static void Login()
    {
      // 로그아웃 되었습니다 창 찾는 부분 시작
      IntPtr hWnd_logoutNoti = new IntPtr(FindWindow("EVA_Window_Dblclk", ""));
      // 로그아웃 되었습니다 창 찾는 부분 끝
      if (hWnd_logoutNoti == IntPtr.Zero)
      {
        // Console.WriteLine("대기중");
        return;
      }

      sendKey(hWnd_logoutNoti, VirtualKeys.Return);
      Console.WriteLine("logoutNoti:" + hWnd_logoutNoti);


      // 로그인 창 찾는 부분 시작
      IntPtr hWnd_loginPage = new IntPtr(FindWindow("EVA_Window", "카카오톡"));
      IntPtr hEdit_id = FindWindowEx(hWnd_loginPage, IntPtr.Zero, "Edit", null);
      IntPtr hEdit_pw = FindWindowEx(hWnd_loginPage, hEdit_id, "Edit", null);

      Console.WriteLine("loginPage:" + hWnd_loginPage);
      Console.WriteLine("hEdit_id:" + hEdit_id);
      Console.WriteLine("hEdit_pw:" + hEdit_pw);
      // 로그인 창 찾는 부분 끝

      if (hWnd_loginPage == IntPtr.Zero)
        return;

      if (hEdit_id == IntPtr.Zero)
        return;

      if (hEdit_pw == IntPtr.Zero)
        return;
      // 로그인 정보 입력 시작
      SendMessage(hEdit_id, (int)WM.SETTEXT, IntPtr.Zero, new StringBuilder(getCredentials()["kakaoAccount"]));
      SendMessage(hEdit_pw, (int)WM.SETTEXT, IntPtr.Zero, new StringBuilder(getCredentials()["password"]));
      // 엔터
      sendKey(hWnd_loginPage, VirtualKeys.Return);
      // 로그인 정보 입력 끝

      // 로그인 확인 팝업이 출력될 때까지 대기 시작
      IntPtr hWnd_loginConfirm = new IntPtr();
      for (int i = 0; i < 100; i++)
      {
        Thread.Sleep(100);
        hWnd_loginConfirm = new IntPtr(FindWindow("EVA_Window_Dblclk", ""));
        if (hWnd_loginConfirm != IntPtr.Zero)
        {
          Console.WriteLine("loginConfirm:" + hWnd_loginConfirm);
          break;
        }
      }
      // 로그인 확인 팝업이 출력될 때까지 대기 끝
      if (hWnd_loginConfirm == IntPtr.Zero)
      {
        Console.WriteLine("loginConfirm is null");
        throw new Exception("loginConfirm is null");
      }
      // 로그인 확인 팝업이 출력되면 확인 버튼 클릭 시작
      sendKey(hWnd_loginConfirm, VirtualKeys.Return);
      Console.WriteLine("loginConfirm:" + hWnd_loginConfirm);
      // 로그인 확인 팝업이 출력되면 확인 버튼 클릭 끝
    }
    static void Main(string[] args)
    {
      Console.Write("[카카오톡 자동 로그인]\n");
      getCredentials();
      IntPtr hWnd_logoutNotice = new IntPtr();
      while (true)
      {
        var idleTime = IdleTimeDetector.GetIdleTimeInfo();
        if (idleTime.IdleTime.TotalSeconds >= 0.1f)
        {
          Thread.Sleep(100);
          continue;
        }
        hWnd_logoutNotice = new IntPtr(FindWindow("EVA_Window_Dblclk", ""));
        if (IsWindowVisible(hWnd_logoutNotice) == false)
        {
          Console.Write(".");
          Thread.Sleep(1000);
          //   getCredentials()["password"]
          continue;
        }
        Login();
        Thread.Sleep(1000);
      }
    }

    static Dictionary<string, string> getCredentials()
    {
      //check credentials.json
      string kakaoAccount;
      string password = "";
      if (File.Exists("credentials.json") == false)
      {
        return createNewCredentials();
      }
      else
      {
        string json = File.ReadAllText("credentials.json");
        JObject jObject = JObject.Parse(json);
        kakaoAccount = jObject["kakaoAccount"].ToString();
        password = jObject["password"].ToString();
        if (kakaoAccount == "" || password == "")
        {
          return createNewCredentials();
        }
      }

      //decrypt password
      password = encrypt(password, true);

      return new Dictionary<string, string>() { { "kakaoAccount", kakaoAccount }, { "password", password } };
    }

    /* credentials.example.json
    {"kakaoAccount" : "exjang0@gmail.com", "password" : "U2FsdGVkX1+0K1uV0ikO3uQCOQJiZ5fBgDwQxH8vCUulH50clT5Z2Onkl/XUAstr"}
*/
    private static Dictionary<string, string> createNewCredentials()
    {
      //   throw new NotImplementedException();

      Console.Write("새로운 자격 증명을 만듭니다.");
      string id = "";
      while (id == "")
      {
        Console.Write("카카오계정을 입력하세요 : ");
        id = Console.ReadLine() ?? "";
      }
      string pw = "";
      while (pw == "")
      {
        Console.Write("비밀번호를 입력하세요 : ");
        // pw = GetPassword().ToString() ?? "";
        pw = new System.Net.NetworkCredential(string.Empty, GetPassword()).Password;
        // Console.Write("입력한 비밀번호는 " + pw);
      }

      //encrypt
      pw = encrypt(pw).Replace("\r", "").Replace("\n", "");
      string json = "{\"kakaoAccount\" : \"" + id + "\", \"password\" : \"" + pw + "\"}";
      File.WriteAllText("credentials.json", json);
      Console.WriteLine("새로운 자격 증명을 만들었습니다.");
      return new Dictionary<string, string>() { { "kakaoAccount", id }, { "password", pw } };
    }
    private static SecureString GetPassword()
    {
      var pwd = new SecureString();
      while (true)
      {
        ConsoleKeyInfo i = Console.ReadKey(true);
        if (i.Key == ConsoleKey.Enter)
        {
          break;
        }
        else if (i.Key == ConsoleKey.Backspace)
        {
          if (pwd.Length > 0)
          {
            pwd.RemoveAt(pwd.Length - 1);
            Console.Write("\b \b");
          }
        }
        else if (i.KeyChar != '\u0000') // KeyChar == '\u0000' if the key pressed does not correspond to a printable character, e.g. F1, Pause-Break, etc
        {
          pwd.AppendChar(i.KeyChar);
          Console.Write("*");
        }
      }
      return pwd;
    }


    private static string encrypt(string pw, bool decrypt = false)
    {
      ProcessStartInfo startInfo = new ProcessStartInfo();
      startInfo.FileName = "cmd.exe";
      if (decrypt == true)
        startInfo.Arguments = "/C openssl enc -d -aes-256-cbc -a -salt -pass pass:kamikami -iter 100";
      else
        startInfo.Arguments = "/C openssl enc -aes-256-cbc -a -salt -pass pass:kamikami -iter 100";
      startInfo.RedirectStandardInput = true;
      startInfo.RedirectStandardOutput = true;
      startInfo.UseShellExecute = false;
      startInfo.CreateNoWindow = true;
      Process process = new Process();
      process.StartInfo = startInfo;
      process.Start();
      process.StandardInput.WriteLine(pw);
      process.StandardInput.Close();
      pw = process.StandardOutput.ReadToEnd();
      process.WaitForExit();
      process.Close();
      return pw.Trim();
    }
  }


}
