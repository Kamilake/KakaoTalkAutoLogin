using System.Text;
using System.Diagnostics;
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
    static void Login()
    {
      // 로그아웃 되었습니다 창 찾는 부분 시작
      IntPtr hWnd_logoutNoti = new IntPtr(FindWindow("EVA_Window_Dblclk", ""));
      // 로그아웃 되었습니다 창 찾는 부분 끝
      if (hWnd_logoutNoti == IntPtr.Zero)
        return;
      //get window size
      RECT rect = new RECT();
      GetWindowRect(hWnd_logoutNoti, out rect);
      int width = rect.Right - rect.Left;
      int height = rect.Bottom - rect.Top;
      Console.WriteLine("width:" + width);
      Console.WriteLine("height:" + height);
      // 허용 가능한 해상도 쌍을 설정, 299x142, 374x177
      if (!(
        (width == 299 && height == 141) ||
        (width == 299 && height == 142) ||
        (width == 300 && height == 141) ||
        (width == 300 && height == 142) ||
        (width == 374 && height == 177)
      ))
      {
        // 허용 가능한 해상도가 아니면 종료
        return;
      }

      // 활성 창과 로그아웃 창이 서로 같은 경우에만 로그인 시도
      if (GetForegroundWindow() != hWnd_logoutNoti)
      {
        Console.WriteLine("not focused");
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
    static bool IsWindowRemoteDesktopFocused()
    {
      IntPtr hWnd = GetForegroundWindow();
      if (hWnd == IntPtr.Zero)
        return false;
      StringBuilder sb = new StringBuilder(256);
      GetWindowText(hWnd, sb, sb.Capacity);
      string title = sb.ToString();
      // Console.WriteLine("title:" + title);
      // if title has " - 가상 컴퓨터 연결" then it is remote desktop
      if (title.Contains(" - 가상 컴퓨터 연결"))
        return true;
      if (title.Contains(": 원격 데스크톱 연결"))
        return true;
      if (title.Equals("Parsec"))
        return true;
      if (title.Contains(" - VNC Viewer"))
        return true;
      return false;
    }
    static void Main(string[] args)
    {
      String? phrase = encrypt("U2FsdGVkX1+0K1uV0ikO3uQCOQJiZ5fBgDwQxH8vCUulH50clT5Z2Onkl/XUAstr", true);
      if (phrase == null || !phrase.Equals("MyPasswordHere"))
      {
        Console.WriteLine("OpenSSL이 정상적으로 작동하지 않습니다. OpenSSL을 설치하고 다시 시도해주세요.");
        Console.WriteLine("https://community.chocolatey.org/packages/openssl");
        Console.WriteLine("닫으려면 아무 키나 누르세요...");
        Console.ReadLine();
        return;
      }
      Console.Write("[카카오톡 자동 로그인]\n");
      getCredentials();
      IntPtr hWnd_logoutNotice = new IntPtr();
      Console.Write("로그인 대기중...");
      while (true)
      {
        var idleTime = IdleTimeDetector.GetIdleTimeInfo();
        if (IsWindowRemoteDesktopFocused())
        {
          // Console.Write("RDP");
          Thread.Sleep(200);
          continue;
        }
        if (idleTime.IdleTime.TotalSeconds >= 0.1f)
        {
          Thread.Sleep(200);
          continue;
        }
        hWnd_logoutNotice = new IntPtr(FindWindow("EVA_Window_Dblclk", ""));
        if (IsWindowVisible(hWnd_logoutNotice) == false)
        {
          Console.Write(".");
          Thread.Sleep(1000);
          continue;
        }
        Login();
        Thread.Sleep(1000);

      }
    }

    static Dictionary<string, string> getCredentials()
    {
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
      return new Dictionary<string, string>() { { "kakaoAccount", kakaoAccount }, { "password", encrypt(password, true) } };
    }

    private static Dictionary<string, string> createNewCredentials()
    {
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
        //TODO: Always Use SecureString
        pw = new System.Net.NetworkCredential(string.Empty, GetPassword()).Password;
      }

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
        else if (i.KeyChar != '\u0000')
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
